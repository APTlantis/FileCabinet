Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.IO
Imports System.Reflection

Namespace FilingCabinet.Tests
    <TestClass>
    Public Class PreviewServiceTests
        <TestMethod>
        Sub DiskImagePreviewUsesFormatSpecificCard()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FilingCabinetTests", Guid.NewGuid().ToString("N"))
            Dim storedPath = Path.Combine(workspace, "ubuntu.iso")
            Directory.CreateDirectory(workspace)
            File.WriteAllBytes(storedPath, {0, 1, 2, 3})

            Try
                Dim artifact As New Global.FilingCabinet.ArtifactModel With {
                    .Name = "ubuntu.iso",
                    .Path = storedPath,
                    .Type = "ISO Image",
                    .Category = "ISOs / Disk Images",
                    .Size = "4 bytes",
                    .HashStatus = "Verified"
                }

                Dim preview = New Global.FilingCabinet.PreviewService().LoadPreview(artifact)

                Assert.AreEqual(Global.FilingCabinet.ArtifactPreviewKind.GenericFile, preview.Kind)
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
            Dim workspace = Path.Combine(Path.GetTempPath(), "FilingCabinetTests", Guid.NewGuid().ToString("N"))
            Dim storedPath = Path.Combine(workspace, "FilingCabinet.msi")
            Directory.CreateDirectory(workspace)
            File.WriteAllBytes(storedPath, {0, 1, 2, 3})

            Try
                Dim artifact As New Global.FilingCabinet.ArtifactModel With {
                    .Name = "FilingCabinet.msi",
                    .Path = storedPath,
                    .Type = "MSI File",
                    .Category = "Software / Installers",
                    .Size = "4 bytes",
                    .HashStatus = "Verified"
                }

                Dim preview = New Global.FilingCabinet.PreviewService().LoadPreview(artifact)

                Assert.AreEqual(Global.FilingCabinet.ArtifactPreviewKind.GenericFile, preview.Kind)
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

            Assert.AreEqual(Global.FilingCabinet.ArtifactPreviewKind.GenericFile, preview.Kind)
            Assert.AreEqual("PDF Retained", preview.Title)
            Assert.AreEqual("PDF", preview.BadgeText)
            StringAssert.Contains(preview.Detail, "original rendered document")
        End Sub

        <TestMethod>
        Sub SpreadsheetWithoutExtractedTextUsesWorkbookCard()
            Dim preview = LoadPreviewFor("inventory.xlsx", "Spreadsheet", "Spreadsheets")

            Assert.AreEqual(Global.FilingCabinet.ArtifactPreviewKind.GenericFile, preview.Kind)
            Assert.AreEqual("Spreadsheet Retained", preview.Title)
            Assert.AreEqual("XLSX", preview.BadgeText)
            StringAssert.Contains(preview.Detail, "formulas")
        End Sub

        <TestMethod>
        Sub ArchivePreviewUsesArchiveCard()
            Dim preview = LoadPreviewFor("release.tar", "TAR File", "Archives")

            Assert.AreEqual(Global.FilingCabinet.ArtifactPreviewKind.GenericFile, preview.Kind)
            Assert.AreEqual("Archive Retained", preview.Title)
            Assert.AreEqual("TAR", preview.BadgeText)
            StringAssert.Contains(preview.Detail, "extract contents")
        End Sub

        <TestMethod>
        Sub ResolvePreviewTitleHandlesMissingArtifact()
            Dim method = GetType(Global.FilingCabinet.PreviewService).GetMethod("ResolvePreviewTitle", BindingFlags.NonPublic Or BindingFlags.Static)

            Dim title = DirectCast(method.Invoke(Nothing, {Nothing, ".bin"}), String)

            Assert.AreEqual("File Retained", title)
        End Sub

        Private Shared Function LoadPreviewFor(fileName As String, typeName As String, category As String) As Global.FilingCabinet.ArtifactPreview
            Dim workspace = Path.Combine(Path.GetTempPath(), "FilingCabinetTests", Guid.NewGuid().ToString("N"))
            Dim storedPath = Path.Combine(workspace, fileName)
            Directory.CreateDirectory(workspace)
            File.WriteAllBytes(storedPath, {0, 1, 2, 3})

            Try
                Dim artifact As New Global.FilingCabinet.ArtifactModel With {
                    .Name = fileName,
                    .Path = storedPath,
                    .Type = typeName,
                    .Category = category,
                    .Size = "4 bytes",
                    .HashStatus = "Verified"
                }

                Return New Global.FilingCabinet.PreviewService().LoadPreview(artifact)
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Function
    End Class
End Namespace

