Imports System.IO

Public Class IngestionService
    Private Shared ReadOnly HashServiceInstance As New HashService()

    Public Function Ingest(paths As IEnumerable(Of String), vaultRootPath As String, Optional progress As IProgress(Of IngestionProgress) = Nothing) As List(Of ArtifactModel)
        Dim artifacts As New List(Of ArtifactModel)

        If paths Is Nothing Then
            Return artifacts
        End If

        Report(progress, "Scanning files", "", "Scanning", 0, 0, 0, 0)
        Dim files = ExpandFiles(paths).Select(Function(path) New FileInfo(path)).Where(Function(file) file.Exists).ToList()
        Dim itemsRoot = Path.Combine(vaultRootPath, "items")
        Directory.CreateDirectory(itemsRoot)

        Dim totalBytes = files.Sum(Function(file) file.Length)
        Dim completedBytes As Long = 0
        Dim completedFiles = 0

        For Each source In files
            Try
                Dim destination = BuildDestinationPath(itemsRoot, source.Name)
                Report(progress, "Copying", source.Name, "Copy", completedFiles, files.Count, completedBytes, totalBytes)
                CopyWithProgress(source.FullName, destination, completedBytes, totalBytes, completedFiles, files.Count, progress)

                Dim stored = New FileInfo(destination)
                Report(progress, "Hashing", source.Name, "Hash", completedFiles, files.Count, completedBytes + stored.Length, totalBytes)
                artifacts.Add(CreateArtifact(source, stored))
                completedBytes += stored.Length
                completedFiles += 1
                Report(progress, "Ingested", source.Name, "Complete", completedFiles, files.Count, completedBytes, totalBytes)
            Catch
                ' Keep ingestion resilient: one bad file should not prevent the rest of the batch.
            End Try
        Next

        Report(progress, $"Finished ingesting {artifacts.Count:N0} file(s)", "", "Finished", completedFiles, files.Count, completedBytes, totalBytes)
        Return artifacts
    End Function

    Private Shared Function ExpandFiles(paths As IEnumerable(Of String)) As IEnumerable(Of String)
        Dim files As New List(Of String)

        For Each inputPath In paths
            If String.IsNullOrWhiteSpace(inputPath) Then
                Continue For
            End If

            If File.Exists(inputPath) Then
                files.Add(inputPath)
            ElseIf Directory.Exists(inputPath) Then
                Try
                    files.AddRange(Directory.EnumerateFiles(inputPath, "*", SearchOption.AllDirectories))
                Catch
                End Try
            End If
        Next

        Return files
    End Function

    Private Shared Function BuildDestinationPath(itemsRoot As String, fileName As String) As String
        Dim datedRoot = Path.Combine(itemsRoot, DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"))
        Directory.CreateDirectory(datedRoot)

        Dim baseName = Path.GetFileNameWithoutExtension(fileName)
        Dim extension = Path.GetExtension(fileName)
        Dim candidate = Path.Combine(datedRoot, fileName)
        Dim index = 1

        While File.Exists(candidate)
            candidate = Path.Combine(datedRoot, $"{baseName}-{index}{extension}")
            index += 1
        End While

        Return candidate
    End Function

    Private Shared Sub CopyWithProgress(sourcePath As String, destinationPath As String, baseCompletedBytes As Long, totalBytes As Long, completedFiles As Integer, totalFiles As Integer, progress As IProgress(Of IngestionProgress))
        Const bufferSize = 1024 * 1024
        Dim copiedForFile As Long = 0
        Dim fileName = Path.GetFileName(sourcePath)
        Dim lastReport = DateTime.MinValue

        Using source = New FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize, FileOptions.SequentialScan)
            Using destination = New FileStream(destinationPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, bufferSize, FileOptions.SequentialScan)
                Dim buffer(bufferSize - 1) As Byte

                While True
                    Dim read = source.Read(buffer, 0, buffer.Length)

                    If read <= 0 Then
                        Exit While
                    End If

                    destination.Write(buffer, 0, read)
                    copiedForFile += read

                    If (DateTime.Now - lastReport).TotalMilliseconds >= 120 Then
                        Report(progress, "Copying", fileName, "Copy", completedFiles, totalFiles, baseCompletedBytes + copiedForFile, totalBytes)
                        lastReport = DateTime.Now
                    End If
                End While
            End Using
        End Using
    End Sub

    Private Shared Sub Report(progress As IProgress(Of IngestionProgress), status As String, currentFile As String, stage As String, filesCompleted As Integer, filesTotal As Integer, bytesCompleted As Long, bytesTotal As Long)
        If progress Is Nothing Then
            Return
        End If

        progress.Report(New IngestionProgress With {
            .Status = status,
            .CurrentFile = currentFile,
            .CurrentStage = stage,
            .FilesCompleted = filesCompleted,
            .FilesTotal = filesTotal,
            .BytesCompleted = bytesCompleted,
            .BytesTotal = bytesTotal
        })
    End Sub

    Private Shared Function CreateArtifact(source As FileInfo, stored As FileInfo) As ArtifactModel
        Dim category = InferCategory(source.Extension)
        Dim typeName = InferType(source.Extension)
        Dim tags = InferTags(source, category)
        Dim nowText = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
        Dim computedHashes = HashServiceInstance.ComputeHashes(stored.FullName)

        Return New ArtifactModel With {
            .Name = stored.Name,
            .Type = typeName,
            .Category = category,
            .Size = FormatSize(stored.Length),
            .DateModified = stored.LastWriteTime.ToString("yyyy-MM-dd HH:mm"),
            .Path = stored.FullName,
            .Created = stored.CreationTime.ToString("yyyy-MM-dd HH:mm"),
            .Blake3 = computedHashes.Blake3,
            .Sha256 = computedHashes.Sha256,
            .Rating = 0,
            .Notes = $"Ingested from {source.FullName}",
            .OriginalPath = source.FullName,
            .IngestedAt = nowText,
            .Tags = tags
        }
    End Function

    Private Shared Function InferCategory(extension As String) As String
        Select Case extension.ToLowerInvariant()
            Case ".iso", ".img", ".vhd", ".vhdx"
                Return "ISOs / Disk Images"
            Case ".exe", ".msi", ".msix", ".appx"
                Return "Software / Installers"
            Case ".zip", ".7z", ".rar", ".tar", ".gz", ".xz"
                Return "Archives"
            Case ".asc", ".gpg", ".pgp", ".pem", ".key", ".pub"
                Return "Keys / Security"
            Case ".toml", ".json", ".yaml", ".yml", ".xml", ".ini", ".config"
                Return "Manifests / Config"
            Case ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".webp", ".tif", ".tiff"
                Return "Images"
            Case ".mp3", ".wav", ".flac", ".ogg", ".m4a"
                Return "Audio"
            Case ".mp4", ".mov", ".mkv", ".avi", ".webm"
                Return "Video"
            Case ".torrent"
                Return "Torrents"
            Case ".pdf", ".doc", ".docx", ".txt", ".md", ".rtf"
                Return "Documents"
            Case Else
                Return "Other"
        End Select
    End Function

    Private Shared Function InferType(extension As String) As String
        If String.IsNullOrWhiteSpace(extension) Then
            Return "File"
        End If

        Select Case extension.ToLowerInvariant()
            Case ".iso"
                Return "ISO Image"
            Case ".exe"
                Return "Installer"
            Case ".msix"
                Return "MSIX Installer"
            Case ".asc"
                Return "PGP Key"
            Case ".gpg"
                Return "GPG Encrypted File"
            Case ".toml"
                Return "TOML Document"
            Case ".json"
                Return "JSON Document"
            Case ".png"
                Return "Image (PNG)"
            Case ".torrent"
                Return "Torrent"
            Case Else
                Return extension.TrimStart("."c).ToUpperInvariant() & " File"
        End Select
    End Function

    Private Shared Function InferTags(source As FileInfo, category As String) As List(Of String)
        Dim tags As New List(Of String)
        Dim extension = source.Extension.TrimStart("."c).ToLowerInvariant()

        If Not String.IsNullOrWhiteSpace(extension) Then
            tags.Add(extension)
        End If

        For Each part In category.Split({"/"c, " "c}, StringSplitOptions.RemoveEmptyEntries)
            Dim cleaned = part.Trim().ToLowerInvariant()

            If cleaned.Length > 2 AndAlso Not tags.Contains(cleaned) Then
                tags.Add(cleaned)
            End If
        Next

        Return tags.Take(5).ToList()
    End Function

    Private Shared Function FormatSize(bytes As Long) As String
        Dim units = {"B", "KB", "MB", "GB", "TB"}
        Dim value = CDbl(bytes)
        Dim unitIndex = 0

        While value >= 1024 AndAlso unitIndex < units.Length - 1
            value /= 1024
            unitIndex += 1
        End While

        If unitIndex = 0 Then
            Return $"{bytes} B"
        End If

        Return $"{value:0.##} {units(unitIndex)}"
    End Function

End Class
