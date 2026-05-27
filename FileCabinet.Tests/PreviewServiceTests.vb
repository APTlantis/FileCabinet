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

        <TestMethod>
        Sub PdfWithoutExtractedTextUsesDocumentCard()
            Dim preview = LoadPreviewFor("manual.pdf", "PDF Document", "Documents")

            Assert.AreEqual(Global.FileCabinet.ArtifactPreviewKind.GenericFile, preview.Kind)
            Assert.AreEqual("PDF Retained", preview.Title)
            Assert.AreEqual("PDF", preview.BadgeText)
            StringAssert.Contains(preview.Detail, "original rendered document")
        End Sub

        <TestMethod>
        Sub SpreadsheetWithoutExtractedTextUsesWorkbookCard()
            Dim preview = LoadPreviewFor("inventory.xlsx", "Spreadsheet", "Spreadsheets")

            Assert.AreEqual(Global.FileCabinet.ArtifactPreviewKind.GenericFile, preview.Kind)
            Assert.AreEqual("Spreadsheet Retained", preview.Title)
            Assert.AreEqual("XLSX", preview.BadgeText)
            StringAssert.Contains(preview.Detail, "formulas")
        End Sub

        <TestMethod>
        Sub ArchivePreviewUsesArchiveCard()
            Dim preview = LoadPreviewFor("release.tar", "TAR File", "Archives")

            Assert.AreEqual(Global.FileCabinet.ArtifactPreviewKind.GenericFile, preview.Kind)
            Assert.AreEqual("Archive Retained", preview.Title)
            Assert.AreEqual("TAR", preview.BadgeText)
            StringAssert.Contains(preview.Detail, "extract contents")
        End Sub

        Private Shared Function LoadPreviewFor(fileName As String, typeName As String, category As String) As Global.FileCabinet.ArtifactPreview
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim storedPath = Path.Combine(workspace, fileName)
            Directory.CreateDirectory(workspace)
            File.WriteAllBytes(storedPath, {0, 1, 2, 3})

            Try
                Dim artifact As New Global.FileCabinet.ArtifactModel With {
                    .Name = fileName,
                    .Path = storedPath,
                    .Type = typeName,
                    .Category = category,
                    .Size = "4 bytes",
                    .HashStatus = "Verified"
                }

                Return New Global.FileCabinet.PreviewService().LoadPreview(artifact)
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Function
    End Class
End Namespace
