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
    Public Property Title As String = ""
    Public Property Detail As String = ""
    Public Property BadgeText As String = ""
    Public Property IconGlyph As String = ChrW(&HE8A5)
    Public Property AccentBrush As String = "#818CF8"
    Public Property AccentBackground As String = "#1F264B"
End Class

Public Class PreviewService
    Private Const CategoryDiskImages As String = "ISOs / Disk Images"
    Private Const CategoryInstallers As String = "Software / Installers"
    Private Const CategoryArchives As String = "Archives"
    Private Const CategoryTorrents As String = "Torrents"
    Private Const CategoryKeysSecurity As String = "Keys / Security"
    Private Const CategoryDocuments As String = "Documents"
    Private Const CategorySpreadsheets As String = "Spreadsheets"
    Private Const CategoryPresentations As String = "Presentations"
    Private Const CategoryManifestsConfig As String = "Manifests / Config"
    Private Const CategoryAudio As String = "Audio"
    Private Const CategoryVideo As String = "Video"
    Private Const AccentBackgroundMagenta As String = "#3B1733"

    Private ReadOnly _thumbnailService As ThumbnailService

    Private Shared ReadOnly ImageExtensions As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
        ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tif", ".tiff", ".webp"
    }

    Private Shared ReadOnly TextExtensions As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
        ".txt", ".md", ".json", ".toml", ".yaml", ".yml", ".xml", ".ini", ".log", ".csv", ".ps1", ".bat", ".cmd", ".vb", ".cs", ".xaml", ".config"
    }

    Private Shared ReadOnly DocumentExtensions As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
        ".pdf", ".doc", ".docx", ".odt", ".rtf", ".xls", ".xlsx", ".ods", ".ppt", ".pptx", ".odp"
    }

    Public Sub New(Optional thumbnailService As ThumbnailService = Nothing)
        _thumbnailService = If(thumbnailService, New ThumbnailService())
    End Sub

    Public Function LoadPreview(artifact As ArtifactModel) As ArtifactPreview
        If artifact Is Nothing OrElse String.IsNullOrWhiteSpace(artifact.Path) OrElse Not File.Exists(artifact.Path) Then
            Return New ArtifactPreview With {
                .Kind = ArtifactPreviewKind.Missing,
                .Message = "Stored file missing",
                .Title = "Missing File",
                .Detail = "The catalog entry exists, but the retained file was not found.",
                .BadgeText = "!"
            }
        End If

        Dim extension = Path.GetExtension(artifact.Path)

        If ImageExtensions.Contains(extension) Then
            Dim thumbnailPath = ResolveGeneratedThumbnailPath(artifact)
            If Not String.IsNullOrWhiteSpace(thumbnailPath) Then
                Return LoadImagePreview(thumbnailPath, "Image thumbnail")
            End If

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
                preview.Title = $"{artifact.Type} Text Preview"
                Return preview
            End If

            Dim formatPreview = CreateFormatPreview(artifact, extension)
            formatPreview.Message = $"{artifact.Type} retained in vault; use Open File to inspect the original document"
            Return formatPreview
        End If

        Return CreateFormatPreview(artifact, extension)
    End Function

    Private Function ResolveGeneratedThumbnailPath(artifact As ArtifactModel) As String
        If artifact Is Nothing OrElse Not String.Equals(artifact.ThumbnailStatus, ThumbnailService.GeneratedStatus, StringComparison.OrdinalIgnoreCase) Then
            Return ""
        End If

        Dim vaultRoot = ResolveVaultRoot(artifact)
        Dim thumbnailPath = _thumbnailService.ResolveThumbnailPath(artifact, vaultRoot)
        If String.IsNullOrWhiteSpace(thumbnailPath) OrElse Not File.Exists(thumbnailPath) Then
            Return ""
        End If

        Return thumbnailPath
    End Function

    Private Shared Function LoadImagePreview(path As String, Optional message As String = "Image preview") As ArtifactPreview
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
                .Message = message,
                .Title = message
            }
        Catch
            Return New ArtifactPreview With {
                .Kind = ArtifactPreviewKind.GenericFile,
                .Message = "Image preview unavailable",
                .Title = "Image Preview Unavailable",
                .Detail = "The image is retained in the vault, but WPF could not decode it.",
                .BadgeText = "IMG",
                .IconGlyph = ChrW(&HE91B),
                .AccentBrush = "#EC4899",
                .AccentBackground = AccentBackgroundMagenta
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
                .Message = "Text preview",
                .Title = "Text Preview"
            }
        Catch
            Return New ArtifactPreview With {
                .Kind = ArtifactPreviewKind.GenericFile,
                .Message = "Text preview unavailable",
                .Title = "Text Preview Unavailable",
                .Detail = "The file is retained in the vault, but text could not be read safely.",
                .BadgeText = "TXT",
                .IconGlyph = ChrW(&HE8A5),
                .AccentBrush = "#818CF8",
                .AccentBackground = "#1F264B"
            }
        End Try
    End Function

    Private Shared Function CreateFormatPreview(artifact As ArtifactModel, extension As String) As ArtifactPreview
        Dim title = ResolvePreviewTitle(artifact, extension)
        Dim actionHint = ResolveActionHint(artifact, extension)

        Return New ArtifactPreview With {
            .Kind = ArtifactPreviewKind.GenericFile,
            .Message = $"{artifact.Type} retained in vault; preview is not available yet",
            .Title = title,
            .Detail = BuildPreviewDetail(artifact, actionHint),
            .BadgeText = ResolveBadgeText(artifact, extension),
            .IconGlyph = ResolveIconGlyph(artifact, extension),
            .AccentBrush = ResolveAccentBrush(artifact, extension),
            .AccentBackground = ResolveAccentBackground(artifact, extension)
        }
    End Function

    Private Shared Function ResolvePreviewTitle(artifact As ArtifactModel, extension As String) As String
        Dim category = If(artifact?.Category, "")
        Dim normalizedExtension = If(extension, "").ToLowerInvariant()

        Select Case True
            Case String.Equals(category, CategoryDiskImages, StringComparison.OrdinalIgnoreCase)
                Return "Disk Image Retained"
            Case String.Equals(category, CategoryInstallers, StringComparison.OrdinalIgnoreCase)
                Return "Installer Retained"
            Case String.Equals(category, CategoryArchives, StringComparison.OrdinalIgnoreCase)
                Return "Archive Retained"
            Case String.Equals(category, CategoryTorrents, StringComparison.OrdinalIgnoreCase)
                Return "Torrent Retained"
            Case String.Equals(category, CategoryKeysSecurity, StringComparison.OrdinalIgnoreCase)
                Return "Security File Retained"
            Case String.Equals(category, CategoryDocuments, StringComparison.OrdinalIgnoreCase)
                Return If(normalizedExtension = ".pdf", "PDF Retained", "Document Retained")
            Case String.Equals(category, CategorySpreadsheets, StringComparison.OrdinalIgnoreCase)
                Return "Spreadsheet Retained"
            Case String.Equals(category, CategoryPresentations, StringComparison.OrdinalIgnoreCase)
                Return "Presentation Retained"
            Case String.Equals(category, CategoryManifestsConfig, StringComparison.OrdinalIgnoreCase)
                Return "Config File Retained"
            Case String.Equals(category, CategoryAudio, StringComparison.OrdinalIgnoreCase)
                Return "Audio Retained"
            Case String.Equals(category, CategoryVideo, StringComparison.OrdinalIgnoreCase)
                Return "Video Retained"
            Case normalizedExtension = ".pdf"
                Return "PDF Retained"
            Case Else
                Return $"{If(artifact?.Type, "File")} Retained"
        End Select
    End Function

    Private Shared Function ResolveActionHint(artifact As ArtifactModel, extension As String) As String
        Dim category = If(artifact?.Category, "")
        Dim normalizedExtension = If(extension, "").ToLowerInvariant()

        Select Case True
            Case String.Equals(category, CategoryDiskImages, StringComparison.OrdinalIgnoreCase)
                Return "Open the file to mount or inspect the image."
            Case String.Equals(category, CategoryInstallers, StringComparison.OrdinalIgnoreCase)
                Return "Open only when you are ready to run the installer."
            Case String.Equals(category, CategoryArchives, StringComparison.OrdinalIgnoreCase)
                Return "Open the archive to browse or extract contents."
            Case String.Equals(category, CategoryTorrents, StringComparison.OrdinalIgnoreCase)
                Return "Open with your torrent client if you need the source payload."
            Case String.Equals(category, CategoryKeysSecurity, StringComparison.OrdinalIgnoreCase)
                Return "Keep access limited; inspect with a trusted security tool."
            Case String.Equals(category, CategoryDocuments, StringComparison.OrdinalIgnoreCase)
                Return If(normalizedExtension = ".pdf", "Open the PDF for the original rendered document.", "Open the document for full layout fidelity.")
            Case String.Equals(category, CategorySpreadsheets, StringComparison.OrdinalIgnoreCase)
                Return "Open the workbook to inspect formulas, sheets, and formatting."
            Case String.Equals(category, CategoryPresentations, StringComparison.OrdinalIgnoreCase)
                Return "Open the deck to inspect slides, speaker notes, and media."
            Case String.Equals(category, CategoryManifestsConfig, StringComparison.OrdinalIgnoreCase)
                Return "Open the file to inspect structured configuration."
            Case normalizedExtension = ".pdf"
                Return "Open the PDF for the original rendered document."
            Case Else
                Return "Open the original file with the system default app."
        End Select
    End Function

    Private Shared Function ResolveBadgeText(artifact As ArtifactModel, extension As String) As String
        Dim normalizedExtension = If(extension, "").TrimStart("."c).ToUpperInvariant()
        If Not String.IsNullOrWhiteSpace(normalizedExtension) Then
            Return normalizedExtension
        End If

        Dim family = If(artifact?.TypeFamily, "FILE").ToUpperInvariant()
        If family.Length <= 4 Then
            Return family
        End If

        Return family.Substring(0, 4)
    End Function

    Private Shared Function ResolveIconGlyph(artifact As ArtifactModel, extension As String) As String
        Dim category = If(artifact?.Category, "")
        Dim normalizedExtension = If(extension, "").ToLowerInvariant()

        Select Case True
            Case String.Equals(category, CategoryDiskImages, StringComparison.OrdinalIgnoreCase)
                Return ChrW(&HE958)
            Case String.Equals(category, CategoryInstallers, StringComparison.OrdinalIgnoreCase)
                Return ChrW(&HE896)
            Case String.Equals(category, CategoryArchives, StringComparison.OrdinalIgnoreCase)
                Return ChrW(&HE7B8)
            Case String.Equals(category, CategoryTorrents, StringComparison.OrdinalIgnoreCase)
                Return ChrW(&HE896)
            Case String.Equals(category, CategoryKeysSecurity, StringComparison.OrdinalIgnoreCase)
                Return ChrW(&HE72E)
            Case String.Equals(category, CategoryDocuments, StringComparison.OrdinalIgnoreCase)
                Return ChrW(&HE8A5)
            Case String.Equals(category, CategorySpreadsheets, StringComparison.OrdinalIgnoreCase)
                Return ChrW(&HE9D2)
            Case String.Equals(category, CategoryPresentations, StringComparison.OrdinalIgnoreCase)
                Return ChrW(&HEBC6)
            Case String.Equals(category, CategoryManifestsConfig, StringComparison.OrdinalIgnoreCase)
                Return ChrW(&HE713)
            Case String.Equals(category, CategoryAudio, StringComparison.OrdinalIgnoreCase)
                Return ChrW(&HE8D6)
            Case String.Equals(category, CategoryVideo, StringComparison.OrdinalIgnoreCase)
                Return ChrW(&HE8B2)
            Case normalizedExtension = ".pdf"
                Return ChrW(&HE8A5)
            Case Else
                Return ChrW(&HE8A5)
        End Select
    End Function

    Private Shared Function ResolveAccentBrush(artifact As ArtifactModel, extension As String) As String
        Select Case If(artifact?.Category, "")
            Case CategoryDiskImages
                Return "#818CF8"
            Case CategoryInstallers
                Return "#C084FC"
            Case CategoryArchives
                Return "#FB923C"
            Case CategoryTorrents
                Return "#2DD4BF"
            Case CategoryKeysSecurity
                Return "#F43F5E"
            Case CategoryDocuments
                Return If(String.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase), "#F43F5E", "#22D3EE")
            Case CategorySpreadsheets
                Return "#34D399"
            Case CategoryPresentations
                Return "#F472B6"
            Case CategoryManifestsConfig
                Return "#818CF8"
            Case CategoryAudio
                Return "#EC4899"
            Case CategoryVideo
                Return "#F472B6"
            Case Else
                If String.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase) Then
                    Return "#F43F5E"
                End If

                Return "#CBD5E1"
        End Select
    End Function

    Private Shared Function ResolveAccentBackground(artifact As ArtifactModel, extension As String) As String
        Select Case If(artifact?.Category, "")
            Case CategoryDiskImages
                Return "#1F264B"
            Case CategoryInstallers
                Return "#2A214D"
            Case CategoryArchives
                Return "#3A2712"
            Case CategoryTorrents
                Return "#12343A"
            Case CategoryKeysSecurity
                Return "#3B1720"
            Case CategoryDocuments
                Return If(String.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase), "#3B1720", "#123044")
            Case CategorySpreadsheets
                Return "#123522"
            Case CategoryPresentations
                Return AccentBackgroundMagenta
            Case CategoryManifestsConfig
                Return "#1F264B"
            Case CategoryAudio
                Return AccentBackgroundMagenta
            Case CategoryVideo
                Return AccentBackgroundMagenta
            Case Else
                If String.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase) Then
                    Return "#3B1720"
                End If

                Return "#162033"
        End Select
    End Function

    Private Shared Function BuildPreviewDetail(artifact As ArtifactModel, actionHint As String) As String
        Dim parts As New List(Of String)

        If artifact IsNot Nothing Then
            If Not String.IsNullOrWhiteSpace(artifact.Size) Then
                parts.Add(artifact.Size)
            End If

            If Not String.IsNullOrWhiteSpace(artifact.Category) Then
                parts.Add(artifact.Category)
            End If

            If Not String.IsNullOrWhiteSpace(artifact.HashStatus) Then
                parts.Add($"Hash: {artifact.HashStatus}")
            End If
        End If

        If Not String.IsNullOrWhiteSpace(actionHint) Then
            parts.Add(actionHint)
        End If

        Return String.Join("  |  ", parts)
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
