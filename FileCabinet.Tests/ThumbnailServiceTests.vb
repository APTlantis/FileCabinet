Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.IO
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Imaging

Namespace FileCabinet.Tests
    <TestClass>
    Public Class ThumbnailServiceTests
        <TestMethod>
        Sub ImageThumbnailGenerationCreatesBoundedVaultRelativePng()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim vaultRoot = Path.Combine(workspace, "vault")
            Dim sourcePath = Path.Combine(workspace, "source.png")
            Directory.CreateDirectory(workspace)
            Directory.CreateDirectory(vaultRoot)
            CreatePng(sourcePath, 640, 320)

            Try
                Dim artifact As New Global.FileCabinet.ArtifactModel With {
                    .Name = "source.png",
                    .Path = sourcePath,
                    .Category = "Images",
                    .TypeFamily = "Image"
                }

                Dim result = New Global.FileCabinet.ThumbnailService().GenerateForArtifact(artifact, vaultRoot)
                Dim thumbnailPath = Path.Combine(vaultRoot, result.RelativePath)
                Dim dimensions = ReadImageDimensions(thumbnailPath)

                Assert.AreEqual(Global.FileCabinet.ThumbnailService.GeneratedStatus, result.Status)
                Assert.IsFalse(String.IsNullOrWhiteSpace(result.RelativePath))
                Assert.IsTrue(result.RelativePath.StartsWith("thumbnails" & Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                Assert.IsTrue(File.Exists(thumbnailPath))
                Assert.IsTrue(dimensions.Width <= 320)
                Assert.IsTrue(dimensions.Height <= 320)
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub

        <TestMethod>
        Sub NonImageArtifactUsesFallbackCardStatus()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim vaultRoot = Path.Combine(workspace, "vault")
            Dim sourcePath = Path.Combine(workspace, "installer.msi")
            Directory.CreateDirectory(workspace)
            Directory.CreateDirectory(vaultRoot)
            File.WriteAllBytes(sourcePath, {0, 1, 2, 3})

            Try
                Dim artifact As New Global.FileCabinet.ArtifactModel With {
                    .Name = "installer.msi",
                    .Path = sourcePath,
                    .Category = "Software / Installers",
                    .TypeFamily = "Installer"
                }

                Dim result = New Global.FileCabinet.ThumbnailService().GenerateForArtifact(artifact, vaultRoot)

                Assert.AreEqual(Global.FileCabinet.ThumbnailService.FallbackCardStatus, result.Status)
                Assert.AreEqual("", result.RelativePath)
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub

        <TestMethod>
        Sub MissingGeneratedThumbnailIsDetected()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim vaultRoot = Path.Combine(workspace, "vault")
            Directory.CreateDirectory(vaultRoot)

            Try
                Dim artifact As New Global.FileCabinet.ArtifactModel With {
                    .ThumbnailRelativePath = Path.Combine("thumbnails", "missing.png"),
                    .ThumbnailStatus = Global.FileCabinet.ThumbnailService.GeneratedStatus
                }

                Assert.IsTrue(New Global.FileCabinet.ThumbnailService().IsGeneratedThumbnailMissing(artifact, vaultRoot))
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub

        Private Shared Sub CreatePng(path As String, width As Integer, height As Integer)
            Dim visual As New DrawingVisual()
            Using context = visual.RenderOpen()
                context.DrawRectangle(Brushes.SteelBlue, Nothing, New Rect(0, 0, width, height))
            End Using

            Dim target As New RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32)
            target.Render(visual)

            Dim encoder As New PngBitmapEncoder()
            encoder.Frames.Add(BitmapFrame.Create(target))

            Using stream = New FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None)
                encoder.Save(stream)
            End Using
        End Sub

        Private Shared Function ReadImageDimensions(path As String) As (Width As Integer, Height As Integer)
            Dim image As New BitmapImage()
            image.BeginInit()
            image.CacheOption = BitmapCacheOption.OnLoad
            image.UriSource = New Uri(path)
            image.EndInit()
            image.Freeze()

            Return (image.PixelWidth, image.PixelHeight)
        End Function
    End Class
End Namespace
