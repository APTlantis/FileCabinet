Imports System.IO
Imports System.Text
Imports System.Windows.Media.Imaging

Public Enum ArtifactPreviewKind
    Missing
    Image
    Text
    GenericFile
End Enum

Public Class ArtifactPreview
    Public Property Kind As ArtifactPreviewKind
    Public Property Image As BitmapImage
    Public Property Text As String = ""
    Public Property Message As String = ""
End Class

Public Class PreviewService
    Private Shared ReadOnly ImageExtensions As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
        ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tif", ".tiff", ".webp"
    }

    Private Shared ReadOnly TextExtensions As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
        ".txt", ".md", ".json", ".toml", ".yaml", ".yml", ".xml", ".ini", ".log", ".csv", ".ps1", ".bat", ".cmd", ".vb", ".cs", ".xaml", ".config"
    }

    Private Shared ReadOnly DocumentExtensions As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
        ".pdf", ".doc", ".docx", ".odt", ".rtf", ".xls", ".xlsx", ".ods", ".ppt", ".pptx", ".odp"
    }

    Public Function LoadPreview(artifact As ArtifactModel) As ArtifactPreview
        If artifact Is Nothing OrElse String.IsNullOrWhiteSpace(artifact.Path) OrElse Not File.Exists(artifact.Path) Then
            Return New ArtifactPreview With {
                .Kind = ArtifactPreviewKind.Missing,
                .Message = "Stored file missing"
            }
        End If

        Dim extension = Path.GetExtension(artifact.Path)

        If ImageExtensions.Contains(extension) Then
            Return LoadImagePreview(artifact.Path)
        End If

        If TextExtensions.Contains(extension) Then
            Return LoadTextPreview(artifact.Path)
        End If

        If DocumentExtensions.Contains(extension) Then
            Dim extractedTextPath = ResolveExtractedTextPath(artifact)
            If Not String.IsNullOrWhiteSpace(extractedTextPath) AndAlso File.Exists(extractedTextPath) Then
                Dim preview = LoadTextPreview(extractedTextPath)
                preview.Message = $"{artifact.Type} text preview"
                Return preview
            End If

            Return New ArtifactPreview With {
                .Kind = ArtifactPreviewKind.GenericFile,
                .Message = $"{artifact.Type} retained in vault; use Open File to inspect the original document"
            }
        End If

        Return New ArtifactPreview With {
            .Kind = ArtifactPreviewKind.GenericFile,
            .Message = $"{artifact.Type} retained in vault; preview is not available yet"
        }
    End Function

    Private Shared Function LoadImagePreview(path As String) As ArtifactPreview
        Try
            Dim image As New BitmapImage()
            image.BeginInit()
            image.CacheOption = BitmapCacheOption.OnLoad
            image.DecodePixelWidth = 420
            image.UriSource = New Uri(path)
            image.EndInit()
            image.Freeze()

            Return New ArtifactPreview With {
                .Kind = ArtifactPreviewKind.Image,
                .Image = image,
                .Message = "Image preview"
            }
        Catch
            Return New ArtifactPreview With {
                .Kind = ArtifactPreviewKind.GenericFile,
                .Message = "Image preview unavailable"
            }
        End Try
    End Function

    Private Shared Function LoadTextPreview(path As String) As ArtifactPreview
        Try
            Const maxChars = 6000
            Dim builder As New StringBuilder()

            Using reader As New StreamReader(path, detectEncodingFromByteOrderMarks:=True)
                Dim buffer(1023) As Char

                While builder.Length < maxChars
                    Dim remaining = Math.Min(buffer.Length, maxChars - builder.Length)
                    Dim read = reader.Read(buffer, 0, remaining)

                    If read <= 0 Then
                        Exit While
                    End If

                    builder.Append(buffer, 0, read)
                End While
            End Using

            Return New ArtifactPreview With {
                .Kind = ArtifactPreviewKind.Text,
                .Text = builder.ToString(),
                .Message = "Text preview"
            }
        Catch
            Return New ArtifactPreview With {
                .Kind = ArtifactPreviewKind.GenericFile,
                .Message = "Text preview unavailable"
            }
        End Try
    End Function

    Private Shared Function ResolveExtractedTextPath(artifact As ArtifactModel) As String
        If artifact Is Nothing OrElse String.IsNullOrWhiteSpace(artifact.ExtractedTextRelativePath) Then
            Return ""
        End If

        If Path.IsPathRooted(artifact.ExtractedTextRelativePath) Then
            Return artifact.ExtractedTextRelativePath
        End If

        Dim vaultRoot = ResolveVaultRoot(artifact)
        If String.IsNullOrWhiteSpace(vaultRoot) Then
            Return ""
        End If

        Return Path.Combine(vaultRoot, artifact.ExtractedTextRelativePath)
    End Function

    Private Shared Function ResolveVaultRoot(artifact As ArtifactModel) As String
        If artifact Is Nothing OrElse String.IsNullOrWhiteSpace(artifact.Path) Then
            Return ""
        End If

        If Not String.IsNullOrWhiteSpace(artifact.RelativePath) Then
            Dim fullPath = Path.GetFullPath(artifact.Path)
            Dim relativePath = artifact.RelativePath.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            Dim comparison = If(Environment.OSVersion.Platform = PlatformID.Win32NT, StringComparison.OrdinalIgnoreCase, StringComparison.Ordinal)

            If fullPath.EndsWith(relativePath, comparison) Then
                Return fullPath.Substring(0, fullPath.Length - relativePath.Length).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            End If
        End If

        Dim directory As DirectoryInfo = System.IO.Directory.GetParent(System.IO.Path.GetFullPath(artifact.Path))
        While directory IsNot Nothing
            If String.Equals(directory.Name, "items", StringComparison.OrdinalIgnoreCase) AndAlso directory.Parent IsNot Nothing Then
                Return directory.Parent.FullName
            End If

            directory = directory.Parent
        End While

        Return ""
    End Function
End Class
