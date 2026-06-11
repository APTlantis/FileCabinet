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

        <TestMethod>
        Sub HealthReportDetectsHashMismatchWithoutMutatingCatalogHashes()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim vaultRoot = Path.Combine(workspace, "vault")
            Dim itemRoot = Path.Combine(vaultRoot, "items")
            Directory.CreateDirectory(itemRoot)
            Dim storedPath = Path.Combine(itemRoot, "kept.txt")
            File.WriteAllText(storedPath, "original")

            Try
                Dim hashService = New Global.FileCabinet.HashService()
                Dim originalHashes = hashService.ComputeHashes(storedPath)
                File.WriteAllText(storedPath, "changed")

                Dim artifact = New Global.FileCabinet.ArtifactModel With {
                    .Name = "kept.txt",
                    .Path = storedPath,
                    .Blake3 = originalHashes.Blake3,
                    .Sha256 = originalHashes.Sha256
                }

                Dim report = Global.FileCabinet.MainViewModel.BuildVaultHealthReport({artifact}, vaultRoot, New Global.FileCabinet.ThumbnailService(), hashService)
                Dim finding = report.Findings.FirstOrDefault(Function(item) item.FindingType = "Hash mismatch" AndAlso item.Subject = "kept.txt")

                Assert.IsNotNull(finding)
                Assert.AreEqual("High", finding.RiskLevel)
                Assert.IsFalse(finding.MutatesCatalog)
                Assert.IsFalse(finding.TouchesRetainedFiles)
                Assert.AreEqual(originalHashes.Blake3, artifact.Blake3)
                Assert.AreEqual(originalHashes.Sha256, artifact.Sha256)
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub

        <TestMethod>
        Sub HealthReportMissingHashesUsesOnlyActiveHashIds()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim vaultRoot = Path.Combine(workspace, "vault")
            Dim itemRoot = Path.Combine(vaultRoot, "items")
            Directory.CreateDirectory(itemRoot)
            Dim storedPath = Path.Combine(itemRoot, "legacy.txt")
            File.WriteAllText(storedPath, "legacy")

            Try
                Dim artifact = New Global.FileCabinet.ArtifactModel With {
                    .Name = "legacy.txt",
                    .Path = storedPath,
                    .Md5 = "228c70bfc5589c58c044e03fff0e17eb"
                }

                Dim report = Global.FileCabinet.MainViewModel.BuildVaultHealthReport({artifact}, vaultRoot, New Global.FileCabinet.ThumbnailService(), New Global.FileCabinet.HashService(), Nothing, "MD5")

                Assert.IsFalse(report.Findings.Any(Function(finding) finding.FindingType = "Missing hash"))
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub

        <TestMethod>
        Sub HealthReportMismatchUsesOnlyActiveHashIds()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim vaultRoot = Path.Combine(workspace, "vault")
            Dim itemRoot = Path.Combine(vaultRoot, "items")
            Directory.CreateDirectory(itemRoot)
            Dim storedPath = Path.Combine(itemRoot, "kept.txt")
            File.WriteAllText(storedPath, "retained")

            Try
                Dim artifact = New Global.FileCabinet.ArtifactModel With {
                    .Name = "kept.txt",
                    .Path = storedPath,
                    .Md5 = "not-the-md5",
                    .Sha256 = "inactive-sha-value"
                }

                Dim report = Global.FileCabinet.MainViewModel.BuildVaultHealthReport({artifact}, vaultRoot, New Global.FileCabinet.ThumbnailService(), New Global.FileCabinet.HashService(), Nothing, "MD5")
                Dim finding = report.Findings.FirstOrDefault(Function(item) item.FindingType = "Hash mismatch")

                Assert.IsNotNull(finding)
                StringAssert.Contains(finding.Detail, "MD5")
                Assert.IsFalse(finding.Detail.Contains("SHA256"))
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub

        <TestMethod>
        Sub HealthReportDetectsMappedHashMismatch()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim vaultRoot = Path.Combine(workspace, "vault")
            Dim itemRoot = Path.Combine(vaultRoot, "items")
            Directory.CreateDirectory(itemRoot)
            Dim storedPath = Path.Combine(itemRoot, "kept.txt")
            File.WriteAllText(storedPath, "abc")

            Try
                Dim artifact = New Global.FileCabinet.ArtifactModel With {
                    .Name = "kept.txt",
                    .Path = storedPath
                }
                artifact.Hashes("crc32") = "not-the-crc"

                Dim report = Global.FileCabinet.MainViewModel.BuildVaultHealthReport({artifact}, vaultRoot, New Global.FileCabinet.ThumbnailService(), New Global.FileCabinet.HashService(), Nothing, "crc32")
                Dim finding = report.Findings.FirstOrDefault(Function(item) item.FindingType = "Hash mismatch")

                Assert.IsNotNull(finding)
                StringAssert.Contains(finding.Detail, "crc32")
                Assert.AreEqual("not-the-crc", artifact.Hashes("crc32"))
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub

        <TestMethod>
        Sub HealthReportDetectsFilesOutsideActiveVault()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim vaultRoot = Path.Combine(workspace, "vault")
            Dim outsideRoot = Path.Combine(workspace, "outside")
            Directory.CreateDirectory(vaultRoot)
            Directory.CreateDirectory(outsideRoot)
            Dim outsidePath = Path.Combine(outsideRoot, "kept.txt")
            File.WriteAllText(outsidePath, "outside")

            Try
                Dim hashes = New Global.FileCabinet.HashService().ComputeHashes(outsidePath)
                Dim artifact = New Global.FileCabinet.ArtifactModel With {
                    .Name = "kept.txt",
                    .Path = outsidePath,
                    .Blake3 = hashes.Blake3,
                    .Sha256 = hashes.Sha256
                }

                Dim report = Global.FileCabinet.MainViewModel.BuildVaultHealthReport({artifact}, vaultRoot, New Global.FileCabinet.ThumbnailService())
                Dim finding = report.Findings.FirstOrDefault(Function(item) item.FindingType = "File outside vault" AndAlso item.Subject = "kept.txt")

                Assert.IsNotNull(finding)
                Assert.AreEqual("Medium", finding.RiskLevel)
                Assert.IsFalse(finding.MutatesCatalog)
                Assert.IsFalse(finding.TouchesRetainedFiles)
                Assert.IsFalse(report.Findings.Any(Function(item) item.FindingType = "Hash mismatch"))
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub

        <TestMethod>
        Sub HealthReportReportsTypedProgressWithoutMutatingCatalog()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim vaultRoot = Path.Combine(workspace, "vault")
            Dim itemRoot = Path.Combine(vaultRoot, "items")
            Directory.CreateDirectory(itemRoot)
            Dim storedPath = Path.Combine(itemRoot, "kept.txt")
            File.WriteAllText(storedPath, "retained")

            Try
                Dim artifact = New Global.FileCabinet.ArtifactModel With {
                    .Name = "kept.txt",
                    .Path = storedPath,
                    .RelativePath = Path.GetRelativePath(vaultRoot, storedPath),
                    .Sha256 = "existing-sha",
                    .Blake3 = "existing-blake"
                }
                Dim beforeSha = artifact.Sha256
                Dim beforeBlake = artifact.Blake3
                Dim progress As New CapturingProgress()

                Global.FileCabinet.MainViewModel.BuildVaultHealthReport({artifact}, vaultRoot, New Global.FileCabinet.ThumbnailService(), New Global.FileCabinet.HashService(), progress, "SHA256,BLAKE3")

                Assert.IsTrue(progress.Updates.Any(Function(update) update.Stage = "Checking" AndAlso update.ProcessedCount = 1 AndAlso update.TotalCount = 1))
                Assert.AreEqual(beforeSha, artifact.Sha256)
                Assert.AreEqual(beforeBlake, artifact.Blake3)
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub

        Private NotInheritable Class CapturingProgress
            Implements IProgress(Of Global.FileCabinet.VaultMaintenanceProgress)

            Public ReadOnly Property Updates As New List(Of Global.FileCabinet.VaultMaintenanceProgress)

            Public Sub Report(value As Global.FileCabinet.VaultMaintenanceProgress) Implements IProgress(Of Global.FileCabinet.VaultMaintenanceProgress).Report
                Updates.Add(value)
            End Sub
        End Class

        <TestMethod>
        Sub HealthReportDetectsIncompleteMetadataForRetainedFile()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim vaultRoot = Path.Combine(workspace, "vault")
            Dim itemRoot = Path.Combine(vaultRoot, "items")
            Directory.CreateDirectory(itemRoot)
            Dim storedPath = Path.Combine(itemRoot, "partial.txt")
            File.WriteAllText(storedPath, "partial")

            Try
                Dim artifact = New Global.FileCabinet.ArtifactModel With {
                    .Name = "partial.txt",
                    .Path = storedPath,
                    .HashStatus = ""
                }

                Dim report = Global.FileCabinet.MainViewModel.BuildVaultHealthReport({artifact}, vaultRoot, New Global.FileCabinet.ThumbnailService())
                Dim finding = report.Findings.FirstOrDefault(Function(item) item.FindingType = "Incomplete metadata" AndAlso item.Subject = "partial.txt")

                Assert.IsNotNull(finding)
                Assert.AreEqual("Medium", finding.RiskLevel)
                Assert.IsFalse(finding.MutatesCatalog)
                Assert.IsFalse(finding.TouchesRetainedFiles)
                StringAssert.Contains(finding.Detail, "type")
                StringAssert.Contains(finding.Detail, "relative path")
                StringAssert.Contains(finding.Detail, "hash status")
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub

        <TestMethod>
        Sub HealthReportDetectsPathRebindCandidateWhenRelativePathResolves()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim vaultRoot = Path.Combine(workspace, "vault")
            Dim itemRoot = Path.Combine(vaultRoot, "items")
            Directory.CreateDirectory(itemRoot)
            Dim resolvedPath = Path.Combine(itemRoot, "portable.txt")
            File.WriteAllText(resolvedPath, "portable")

            Try
                Dim artifact = New Global.FileCabinet.ArtifactModel With {
                    .Name = "portable.txt",
                    .Path = Path.Combine(workspace, "old-drive", "items", "portable.txt"),
                    .RelativePath = Path.Combine("items", "portable.txt")
                }

                Dim report = Global.FileCabinet.MainViewModel.BuildVaultHealthReport({artifact}, vaultRoot, New Global.FileCabinet.ThumbnailService())
                Dim finding = report.Findings.FirstOrDefault(Function(item) item.FindingType = "Path rebind candidate" AndAlso item.Subject = "portable.txt")

                Assert.IsNotNull(finding)
                Assert.AreEqual("Medium", finding.RiskLevel)
                Assert.IsTrue(finding.MutatesCatalog)
                Assert.IsFalse(finding.TouchesRetainedFiles)
                Assert.IsTrue(report.Findings.Any(Function(item) item.FindingType = "Missing file" AndAlso item.Subject = "portable.txt"))
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub

        <TestMethod>
        Sub RepairReportDerivesCountsFromExistingHealthReport()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim vaultRoot = Path.Combine(workspace, "vault")
            Dim itemRoot = Path.Combine(vaultRoot, "items")
            Directory.CreateDirectory(itemRoot)
            Dim retainedPath = Path.Combine(itemRoot, "kept.png")
            Dim orphanPath = Path.Combine(itemRoot, "orphan.bin")
            File.WriteAllBytes(retainedPath, {0, 1, 2, 3})
            File.WriteAllBytes(orphanPath, {4, 5, 6, 7})

            Try
                Dim artifacts = {
                    New Global.FileCabinet.ArtifactModel With {
                        .Name = "missing.iso",
                        .Path = Path.Combine(itemRoot, "missing.iso"),
                        .Sha256 = "same"
                    },
                    New Global.FileCabinet.ArtifactModel With {
                        .Name = "kept.png",
                        .Path = retainedPath,
                        .Sha256 = "same",
                        .ThumbnailStatus = Global.FileCabinet.ThumbnailService.GeneratedStatus,
                        .ThumbnailRelativePath = Path.Combine("thumbnails", "missing.png")
                    }
                }
                Dim healthReport = New Global.FileCabinet.VaultHealthReport()
                healthReport.Findings.Add(New Global.FileCabinet.VaultHealthFinding With {
                    .FindingType = "Missing thumbnail",
                    .Subject = "kept.png"
                })

                Dim report = Global.FileCabinet.MainViewModel.BuildRepairReport(artifacts, vaultRoot, healthReport, {orphanPath})

                Assert.AreEqual(1, report.MissingFiles)
                Assert.AreEqual(1, report.DuplicateHashGroups)
                Assert.AreEqual(1, report.OrphanFiles)
                Assert.AreEqual(1, report.MissingThumbnails)
                CollectionAssert.Contains(report.MissingSamples, "missing.iso")
                CollectionAssert.Contains(report.ThumbnailSamples, "kept.png")
                CollectionAssert.Contains(report.OrphanSamples, "orphan.bin")
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub
    End Class
End Namespace
