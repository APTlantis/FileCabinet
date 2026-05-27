Imports System.IO
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Imaging

Public Class ThumbnailResult
    Public Property RelativePath As String = ""
    Public Property Status As String = "Not applicable"
End Class

Public Class ThumbnailService
    Public Const GeneratedStatus As String = "Generated"
    Public Const FallbackCardStatus As String = "Fallback card"
    Public Const NotApplicableStatus As String = "Not applicable"
    Public Const GenerationFailedStatus As String = "Generation failed"

    Private Const MaxThumbnailPixels As Integer = 320

    Private Shared ReadOnly ImageExtensions As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {
        ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".tif", ".tiff", ".webp"
    }

    Public Function GenerateForArtifact(artifact As ArtifactModel, vaultRootPath As String) As ThumbnailResult
        If artifact Is Nothing OrElse String.IsNullOrWhiteSpace(artifact.Path) OrElse Not File.Exists(artifact.Path) Then
            Return New ThumbnailResult With {.Status = NotApplicableStatus}
        End If

        Dim extension = Path.GetExtension(artifact.Path)
        If Not ImageExtensions.Contains(extension) Then
            Return New ThumbnailResult With {.Status = FallbackCardStatus}
        End If

        Try
            Dim thumbnailRoot = Path.Combine(vaultRootPath, "thumbnails", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"))
            Directory.CreateDirectory(thumbnailRoot)

            Dim thumbnailName = $"{Path.GetFileNameWithoutExtension(artifact.Name)}-{Guid.NewGuid():N}.png"
            Dim thumbnailPath = Path.Combine(thumbnailRoot, thumbnailName)
            GenerateImageThumbnail(artifact.Path, thumbnailPath)

            Return New ThumbnailResult With {
                .RelativePath = Path.GetRelativePath(vaultRootPath, thumbnailPath),
                .Status = GeneratedStatus
            }
        Catch
            Return New ThumbnailResult With {.Status = GenerationFailedStatus}
        End Try
    End Function

    Public Function ResolveThumbnailPath(artifact As ArtifactModel, vaultRootPath As String) As String
        If artifact Is Nothing OrElse String.IsNullOrWhiteSpace(artifact.ThumbnailRelativePath) Then
            Return ""
        End If

        If Path.IsPathRooted(artifact.ThumbnailRelativePath) Then
            Return artifact.ThumbnailRelativePath
        End If

        If String.IsNullOrWhiteSpace(vaultRootPath) Then
            Return ""
        End If

        Return Path.Combine(vaultRootPath, artifact.ThumbnailRelativePath)
    End Function

    Public Function IsGeneratedThumbnailMissing(artifact As ArtifactModel, vaultRootPath As String) As Boolean
        If artifact Is Nothing OrElse Not String.Equals(artifact.ThumbnailStatus, GeneratedStatus, StringComparison.OrdinalIgnoreCase) Then
            Return False
        End If

        Dim thumbnailPath = ResolveThumbnailPath(artifact, vaultRootPath)
        Return String.IsNullOrWhiteSpace(thumbnailPath) OrElse Not File.Exists(thumbnailPath)
    End Function

    Private Shared Sub GenerateImageThumbnail(sourcePath As String, destinationPath As String)
        Dim source As New BitmapImage()
        source.BeginInit()
        source.CacheOption = BitmapCacheOption.OnLoad
        source.UriSource = New Uri(sourcePath)
        source.EndInit()
        source.Freeze()

        Dim scale = Math.Min(MaxThumbnailPixels / Math.Max(1.0, CDbl(source.PixelWidth)), MaxThumbnailPixels / Math.Max(1.0, CDbl(source.PixelHeight)))
        scale = Math.Min(1.0, scale)

        Dim targetWidth = Math.Max(1, CInt(Math.Round(source.PixelWidth * scale)))
        Dim targetHeight = Math.Max(1, CInt(Math.Round(source.PixelHeight * scale)))

        Dim visual As New DrawingVisual()
        Using context = visual.RenderOpen()
            context.DrawImage(source, New Rect(0, 0, targetWidth, targetHeight))
        End Using

        Dim target As New RenderTargetBitmap(targetWidth, targetHeight, 96, 96, PixelFormats.Pbgra32)
        target.Render(visual)

        Dim encoder As New PngBitmapEncoder()
        encoder.Frames.Add(BitmapFrame.Create(target))

        Using stream = New FileStream(destinationPath, FileMode.CreateNew, FileAccess.Write, FileShare.None)
            encoder.Save(stream)
        End Using
    End Sub
End Class
