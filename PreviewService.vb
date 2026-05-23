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

        If String.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase) Then
            Return New ArtifactPreview With {
                .Kind = ArtifactPreviewKind.GenericFile,
                .Message = "PDF retained; open file to inspect full document"
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
End Class
