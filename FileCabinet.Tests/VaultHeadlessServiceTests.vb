Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.IO

Namespace FileCabinet.Tests
    <TestClass>
    Public Class VaultHeadlessServiceTests
        <TestMethod>
        Sub IngestCopyUpdatesCatalogAndKeepsOriginal()
            Dim workspace = TestWorkspace()
            Dim source = Path.Combine(workspace, "source.txt")
            File.WriteAllText(source, "retained cli artifact")

            Try
                Dim service = CreateService(workspace)
                Dim result = service.Ingest({source}, Global.FileCabinet.IngestMode.Copy)
                Dim catalog = service.LoadCatalog()

                Assert.AreEqual(0, result.ExitCode)
                Assert.AreEqual(1, result.IngestedArtifacts.Count)
                Assert.AreEqual(1, catalog.Artifacts.Count)
                Assert.IsTrue(File.Exists(source))
                Assert.IsTrue(File.Exists(catalog.Artifacts(0).Path))
            Finally
                DeleteWorkspace(workspace)
            End Try
        End Sub

        <TestMethod>
        Sub IngestReturnsPartialExitCodeWhenARequestedPathIsSkipped()
            Dim workspace = TestWorkspace()
            Dim source = Path.Combine(workspace, "source.txt")
            File.WriteAllText(source, "retained cli artifact")

            Try
                Dim service = CreateService(workspace)
                Dim result = service.Ingest({source, Path.Combine(workspace, "missing.txt")}, Global.FileCabinet.IngestMode.Copy)

                Assert.AreEqual(3, result.ExitCode)
                Assert.AreEqual(1, result.IngestedArtifacts.Count)
            Finally
                DeleteWorkspace(workspace)
            End Try
        End Sub

        <TestMethod>
        Sub SearchMatchesExtractedText()
            Dim workspace = TestWorkspace()
            Dim source = Path.Combine(workspace, "manifest.txt")
            File.WriteAllText(source, "special deterministic keyword")

            Try
                Dim service = CreateService(workspace)
                service.Ingest({source}, Global.FileCabinet.IngestMode.Copy)

                Dim results = service.Search(New Global.FileCabinet.VaultSearchOptions With {.Query = "deterministic keyword"})

                Assert.AreEqual(1, results.Count)
                Assert.AreEqual("manifest.txt", results(0).Name)
            Finally
                DeleteWorkspace(workspace)
            End Try
        End Sub

        <TestMethod>
        Sub VerifyReturnsThresholdExitCodeForMissingFile()
            Dim workspace = TestWorkspace()

            Try
                Dim catalogService = CreateCatalogService(workspace)
                Dim catalog = catalogService.LoadOrCreate()
                catalog.Artifacts.Add(New Global.FileCabinet.ArtifactModel With {
                    .Name = "missing.iso",
                    .Path = Path.Combine(workspace, "vault", "items", "missing.iso")
                })
                catalogService.Save(catalog)

                Dim result = CreateService(workspace).Verify("any")

                Assert.AreEqual(2, result.ExitCode)
                Assert.IsTrue(result.HealthReport.Findings.Any(Function(finding) finding.FindingType = "Missing file"))
            Finally
                DeleteWorkspace(workspace)
            End Try
        End Sub

        <TestMethod>
        Sub ExportWritesValidatedSnapshot()
            Dim workspace = TestWorkspace()

            Try
                Dim service = CreateService(workspace)
                service.LoadCatalog()
                Dim output = Path.Combine(workspace, "exports")

                Dim result = service.ExportSnapshot(output)

                Assert.IsTrue(result.Validation.IsValid)
                Assert.IsTrue(File.Exists(result.Validation.BackupPath))
            Finally
                DeleteWorkspace(workspace)
            End Try
        End Sub

        <TestMethod>
        Sub ReportWritesTextAndJsonFiles()
            Dim workspace = TestWorkspace()

            Try
                Dim service = CreateService(workspace)
                service.LoadCatalog()
                Dim textPath = Path.Combine(workspace, "health.txt")
                Dim jsonPath = Path.Combine(workspace, "health.json")

                Dim textResult = service.GenerateReport(textPath, "text")
                Dim jsonResult = service.GenerateReport(jsonPath, "json")

                Assert.IsTrue(File.Exists(textResult.OutputPath))
                StringAssert.Contains(File.ReadAllText(textPath), "FileCabinet Vault Health Report")
                Assert.IsTrue(File.Exists(jsonResult.OutputPath))
                StringAssert.Contains(File.ReadAllText(jsonPath), "summary")
            Finally
                DeleteWorkspace(workspace)
            End Try
        End Sub

        <TestMethod>
        Sub RepairPreviewListsCandidatesWithoutSavingCatalogMutation()
            Dim workspace = TestWorkspace()

            Try
                Dim catalogService = CreateCatalogService(workspace)
                Dim catalog = catalogService.LoadOrCreate()
                catalog.Artifacts.Add(New Global.FileCabinet.ArtifactModel With {
                    .Name = "unhashed.txt",
                    .Path = Path.Combine(workspace, "vault", "items", "unhashed.txt")
                })
                Directory.CreateDirectory(Path.GetDirectoryName(catalog.Artifacts(0).Path))
                File.WriteAllText(catalog.Artifacts(0).Path, "needs hash")
                catalogService.Save(catalog)

                Dim beforeJson = File.ReadAllText(catalogService.CatalogPath)
                Dim result = CreateService(workspace).RepairPreview()
                Dim afterJson = File.ReadAllText(catalogService.CatalogPath)

                Assert.IsTrue(result.RepairCandidates.Any(Function(candidate) candidate.ActionType = "RecomputeHash"))
                Assert.AreEqual(beforeJson, afterJson)
            Finally
                DeleteWorkspace(workspace)
            End Try
        End Sub

        <TestMethod>
        Sub RepairApplyRecomputesMissingHashesWithExplicitApproval()
            Dim workspace = TestWorkspace()

            Try
                Dim catalogService = CreateCatalogService(workspace)
                Dim catalog = catalogService.LoadOrCreate()
                Dim storedPath = Path.Combine(workspace, "vault", "items", "needs-hash.txt")
                Directory.CreateDirectory(Path.GetDirectoryName(storedPath))
                File.WriteAllText(storedPath, "needs hash")
                catalog.Artifacts.Add(New Global.FileCabinet.ArtifactModel With {
                    .Name = "needs-hash.txt",
                    .Path = storedPath,
                    .RelativePath = Path.GetRelativePath(catalog.VaultRootPath, storedPath)
                })
                catalogService.Save(catalog)

                Dim result = CreateService(workspace).Repair(apply:=True)
                Dim reloaded = catalogService.LoadOrCreate()

                Assert.AreEqual(1, result.AppliedCount)
                Assert.IsFalse(String.IsNullOrWhiteSpace(reloaded.Artifacts(0).Sha256))
                Assert.AreEqual("Verified", reloaded.Artifacts(0).HashStatus)
            Finally
                DeleteWorkspace(workspace)
            End Try
        End Sub

        <TestMethod>
        Sub RescanPreviewAndApplyAdoptsOrphanStoredFile()
            Dim workspace = TestWorkspace()

            Try
                Dim service = CreateService(workspace)
                Dim catalog = service.LoadCatalog()
                Dim orphanPath = Path.Combine(catalog.VaultRootPath, "items", "2026", "05", "orphan.txt")
                Directory.CreateDirectory(Path.GetDirectoryName(orphanPath))
                File.WriteAllText(orphanPath, "orphan")

                Dim preview = service.Rescan(apply:=False)
                Dim applied = service.Rescan(apply:=True)
                Dim reloaded = service.LoadCatalog()

                Assert.AreEqual(1, preview.OrphanFiles.Count)
                Assert.AreEqual(1, applied.AdoptedArtifacts.Count)
                Assert.AreEqual(1, reloaded.Artifacts.Count)
            Finally
                DeleteWorkspace(workspace)
            End Try
        End Sub

        <TestMethod>
        Sub RescanApplySkipsOrphanWhenStoredFileCannotBeRead()
            Dim workspace = TestWorkspace()
            Dim lockStream As FileStream = Nothing

            Try
                Dim service = CreateService(workspace)
                Dim catalog = service.LoadCatalog()
                Dim orphanPath = Path.Combine(catalog.VaultRootPath, "items", "2026", "05", "locked.txt")
                Directory.CreateDirectory(Path.GetDirectoryName(orphanPath))
                File.WriteAllText(orphanPath, "locked")
                lockStream = New FileStream(orphanPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None)

                Dim applied = service.Rescan(apply:=True)

                Assert.AreEqual(1, applied.OrphanFiles.Count)
                Assert.AreEqual(0, applied.AdoptedArtifacts.Count)
            Finally
                If lockStream IsNot Nothing Then
                    lockStream.Dispose()
                End If

                DeleteWorkspace(workspace)
            End Try
        End Sub

        <TestMethod>
        Sub PackageWritesStandardizedExportFolder()
            Dim workspace = TestWorkspace()
            Dim source = Path.Combine(workspace, "package-source.txt")
            File.WriteAllText(source, "package me")

            Try
                Dim service = CreateService(workspace)
                service.Ingest({source}, Global.FileCabinet.IngestMode.Copy)
                Dim output = Path.Combine(workspace, "package")

                Dim result = service.CreatePackage(output, zipPackage:=False)

                Assert.AreEqual(output, result.OutputPath)
                Assert.IsTrue(File.Exists(Path.Combine(output, "manifest.json")))
                Assert.IsTrue(File.Exists(Path.Combine(output, "catalog", "catalog.json")))
                Assert.IsTrue(File.Exists(Path.Combine(output, "catalog", "catalog.jsonl")))
                Assert.IsTrue(File.Exists(Path.Combine(output, "integrity", "vault-health.json")))
                Assert.IsTrue(result.FileCount > 0)
            Finally
                DeleteWorkspace(workspace)
            End Try
        End Sub

        Private Shared Function CreateService(workspace As String) As Global.FileCabinet.VaultHeadlessService
            Return New Global.FileCabinet.VaultHeadlessService(New Global.FileCabinet.HeadlessOptions With {
                .CatalogPath = Path.Combine(workspace, "catalog.json"),
                .VaultRootPath = Path.Combine(workspace, "vault")
            })
        End Function

        Private Shared Function CreateCatalogService(workspace As String) As Global.FileCabinet.CatalogService
            Return New Global.FileCabinet.CatalogService(Path.Combine(workspace, "catalog.json"), Path.Combine(workspace, "vault"))
        End Function

        Private Shared Function TestWorkspace() As String
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetCliTests", Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(workspace)
            Return workspace
        End Function

        Private Shared Sub DeleteWorkspace(workspace As String)
            If Directory.Exists(workspace) Then
                Directory.Delete(workspace, recursive:=True)
            End If
        End Sub
    End Class
End Namespace
