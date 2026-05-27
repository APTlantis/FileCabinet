Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.IO

Namespace FileCabinet.Tests
    <TestClass>
    Public Class VaultHealthReportTests
        <TestMethod>
        Sub HealthReportDetectsMissingFileDuplicateHashMissingThumbnailAndMissingText()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim vaultRoot = Path.Combine(workspace, "vault")
            Dim itemRoot = Path.Combine(vaultRoot, "items")
            Directory.CreateDirectory(itemRoot)
            Dim storedPath = Path.Combine(itemRoot, "kept.png")
            File.WriteAllBytes(storedPath, {0, 1, 2, 3})

            Try
                Dim artifacts = {
                    New Global.FileCabinet.ArtifactModel With {
                        .Name = "missing.iso",
                        .Path = Path.Combine(itemRoot, "missing.iso"),
                        .Sha256 = "same"
                    },
                    New Global.FileCabinet.ArtifactModel With {
                        .Name = "kept.png",
                        .Path = storedPath,
                        .Sha256 = "same",
                        .Blake3 = "b3",
                        .ThumbnailStatus = Global.FileCabinet.ThumbnailService.GeneratedStatus,
                        .ThumbnailRelativePath = Path.Combine("thumbnails", "missing.png"),
                        .ExtractedTextRelativePath = Path.Combine("extracted-text", "missing.txt")
                    },
                    New Global.FileCabinet.ArtifactModel With {
                        .Name = "unhashed.txt",
                        .Path = storedPath
                    }
                }

                Dim report = Global.FileCabinet.MainViewModel.BuildVaultHealthReport(artifacts, vaultRoot, New Global.FileCabinet.ThumbnailService())

                Assert.IsTrue(report.Findings.Any(Function(finding) finding.FindingType = "Missing file" AndAlso finding.Subject = "missing.iso"))
                Assert.IsTrue(report.Findings.Any(Function(finding) finding.FindingType = "Duplicate hash"))
                Assert.IsTrue(report.Findings.Any(Function(finding) finding.FindingType = "Missing thumbnail" AndAlso finding.Subject = "kept.png"))
                Assert.IsTrue(report.Findings.Any(Function(finding) finding.FindingType = "Missing extracted text" AndAlso finding.Subject = "kept.png"))
                Assert.IsTrue(report.Findings.Any(Function(finding) finding.FindingType = "Missing hash" AndAlso finding.Subject = "unhashed.txt"))
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub

        <TestMethod>
        Sub HealthReportDoesNotMutateThumbnailMetadata()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim vaultRoot = Path.Combine(workspace, "vault")
            Dim itemRoot = Path.Combine(vaultRoot, "items")
            Directory.CreateDirectory(itemRoot)
            Dim storedPath = Path.Combine(itemRoot, "kept.png")
            File.WriteAllBytes(storedPath, {0, 1, 2, 3})

            Try
                Dim artifact = New Global.FileCabinet.ArtifactModel With {
                    .Name = "kept.png",
                    .Path = storedPath,
                    .ThumbnailStatus = Global.FileCabinet.ThumbnailService.GeneratedStatus,
                    .ThumbnailRelativePath = Path.Combine("thumbnails", "missing.png")
                }
                Dim beforePath = artifact.ThumbnailRelativePath
                Dim beforeStatus = artifact.ThumbnailStatus

                Dim report = Global.FileCabinet.MainViewModel.BuildVaultHealthReport({artifact}, vaultRoot, New Global.FileCabinet.ThumbnailService())

                Assert.AreEqual(beforePath, artifact.ThumbnailRelativePath)
                Assert.AreEqual(beforeStatus, artifact.ThumbnailStatus)
                Assert.IsTrue(report.Findings.Any(Function(finding) finding.FindingType = "Missing thumbnail"))
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub

        <TestMethod>
        Sub HealthReportDetectsOrphanThumbnailAndStaleExtractedTextIndexes()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim vaultRoot = Path.Combine(workspace, "vault")
            Dim itemRoot = Path.Combine(vaultRoot, "items")
            Dim thumbnailRoot = Path.Combine(vaultRoot, "thumbnails", "2026", "05")
            Dim extractedRoot = Path.Combine(vaultRoot, "extracted-text", "2026", "05")
            Directory.CreateDirectory(itemRoot)
            Directory.CreateDirectory(thumbnailRoot)
            Directory.CreateDirectory(extractedRoot)

            Dim storedPath = Path.Combine(itemRoot, "kept.txt")
            Dim referencedThumbnail = Path.Combine(thumbnailRoot, "referenced.png")
            Dim orphanThumbnail = Path.Combine(thumbnailRoot, "orphan.png")
            Dim referencedText = Path.Combine(extractedRoot, "referenced.txt")
            Dim staleText = Path.Combine(extractedRoot, "stale.txt")
            File.WriteAllText(storedPath, "retained")
            File.WriteAllBytes(referencedThumbnail, {0, 1, 2, 3})
            File.WriteAllBytes(orphanThumbnail, {4, 5, 6, 7})
            File.WriteAllText(referencedText, "indexed")
            File.WriteAllText(staleText, "stale")

            Try
                Dim artifact = New Global.FileCabinet.ArtifactModel With {
                    .Name = "kept.txt",
                    .Path = storedPath,
                    .Sha256 = "sha",
                    .Blake3 = "b3",
                    .ThumbnailStatus = Global.FileCabinet.ThumbnailService.GeneratedStatus,
                    .ThumbnailRelativePath = Path.GetRelativePath(vaultRoot, referencedThumbnail),
                    .ExtractedTextRelativePath = Path.GetRelativePath(vaultRoot, referencedText)
                }

                Dim report = Global.FileCabinet.MainViewModel.BuildVaultHealthReport({artifact}, vaultRoot, New Global.FileCabinet.ThumbnailService())
                Dim orphanThumbnailRelative = Path.GetRelativePath(vaultRoot, orphanThumbnail)
                Dim staleTextRelative = Path.GetRelativePath(vaultRoot, staleText)

                Assert.IsTrue(report.Findings.Any(Function(finding) finding.FindingType = "Orphan thumbnail" AndAlso finding.Subject = orphanThumbnailRelative))
                Assert.IsTrue(report.Findings.Any(Function(finding) finding.FindingType = "Stale extracted text" AndAlso finding.Subject = staleTextRelative))
                Assert.IsFalse(report.Findings.Any(Function(finding) finding.Subject = Path.GetRelativePath(vaultRoot, referencedThumbnail)))
                Assert.IsFalse(report.Findings.Any(Function(finding) finding.Subject = Path.GetRelativePath(vaultRoot, referencedText)))
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub
    End Class
End Namespace
