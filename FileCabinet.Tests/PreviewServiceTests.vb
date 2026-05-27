Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.IO

Namespace FileCabinet.Tests
    <TestClass>
    Public Class PreviewServiceTests
        <TestMethod>
        Sub DiskImagePreviewUsesFormatSpecificCard()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim storedPath = Path.Combine(workspace, "ubuntu.iso")
            Directory.CreateDirectory(workspace)
            File.WriteAllBytes(storedPath, {0, 1, 2, 3})

            Try
                Dim artifact As New Global.FileCabinet.ArtifactModel With {
                    .Name = "ubuntu.iso",
                    .Path = storedPath,
                    .Type = "ISO Image",
                    .Category = "ISOs / Disk Images",
                    .Size = "4 bytes",
                    .HashStatus = "Verified"
                }

                Dim preview = New Global.FileCabinet.PreviewService().LoadPreview(artifact)

                Assert.AreEqual(Global.FileCabinet.ArtifactPreviewKind.GenericFile, preview.Kind)
                Assert.AreEqual("Disk Image Retained", preview.Title)
                Assert.AreEqual("ISO", preview.BadgeText)
                StringAssert.Contains(preview.Detail, "ISOs / Disk Images")
                StringAssert.Contains(preview.Detail, "mount or inspect")
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub

        <TestMethod>
        Sub InstallerPreviewUsesFormatSpecificCard()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim storedPath = Path.Combine(workspace, "FileCabinet.msi")
            Directory.CreateDirectory(workspace)
            File.WriteAllBytes(storedPath, {0, 1, 2, 3})

            Try
                Dim artifact As New Global.FileCabinet.ArtifactModel With {
                    .Name = "FileCabinet.msi",
                    .Path = storedPath,
                    .Type = "MSI File",
                    .Category = "Software / Installers",
                    .Size = "4 bytes",
                    .HashStatus = "Verified"
                }

                Dim preview = New Global.FileCabinet.PreviewService().LoadPreview(artifact)

                Assert.AreEqual(Global.FileCabinet.ArtifactPreviewKind.GenericFile, preview.Kind)
                Assert.AreEqual("Installer Retained", preview.Title)
                Assert.AreEqual("MSI", preview.BadgeText)
                StringAssert.Contains(preview.Detail, "ready to run")
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub
    End Class
End Namespace
