Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.IO
Imports System.Text.Json

Namespace FileCabinet.Tests
    <TestClass>
    Public Class CatalogServiceTests
        <TestMethod>
        Sub LoadOrCreateSupportsEmptyVaultAndCreatesFolders()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim catalogPath = Path.Combine(workspace, "appdata", "catalog.json")
            Dim vaultRoot = Path.Combine(workspace, "vault")

            Try
                Dim service As New Global.FileCabinet.CatalogService(catalogPath, vaultRoot)
                Dim catalog = service.LoadOrCreate()

                Assert.AreEqual(1, catalog.SchemaVersion)
                Assert.AreEqual(vaultRoot, catalog.VaultRootPath)
                Assert.AreEqual("Move", catalog.DefaultIngestMode)
                Assert.AreEqual(0, catalog.Artifacts.Count)
                Assert.IsTrue(File.Exists(catalogPath))
                Assert.IsTrue(Directory.Exists(Path.Combine(vaultRoot, "items")))
                Assert.IsTrue(Directory.Exists(Path.Combine(vaultRoot, "quarantine")))
                Assert.IsTrue(Directory.Exists(Path.Combine(vaultRoot, "exports")))
                Assert.IsTrue(Directory.Exists(Path.Combine(vaultRoot, "extracted-text")))
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub

        <TestMethod>
        Sub ExportSnapshotWritesPortableCatalogCopy()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim catalogPath = Path.Combine(workspace, "appdata", "catalog.json")
            Dim vaultRoot = Path.Combine(workspace, "vault")

            Try
                Dim service As New Global.FileCabinet.CatalogService(catalogPath, vaultRoot)
                Dim catalog = service.LoadOrCreate()
                catalog.Artifacts.Add(New Global.FileCabinet.ArtifactModel With {
                    .Id = "artifact-1",
                    .Name = "sample.txt",
                    .RelativePath = Path.Combine("items", "sample.txt")
                })

                Dim backupPath = service.ExportSnapshot(catalog, Path.Combine(vaultRoot, "exports"))
                Dim json = File.ReadAllText(backupPath)
                Dim exported = JsonSerializer.Deserialize(Of Global.FileCabinet.CatalogData)(json)

                Assert.IsTrue(File.Exists(backupPath))
                Assert.AreEqual(backupPath, catalog.LastBackupPath)
                Assert.IsNotNull(exported)
                Assert.AreEqual(1, exported.Artifacts.Count)
                Assert.AreEqual("artifact-1", exported.Artifacts(0).Id)
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub

        <TestMethod>
        Sub ExportSnapshotWithValidationConfirmsReadableCatalogBackup()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim catalogPath = Path.Combine(workspace, "appdata", "catalog.json")
            Dim vaultRoot = Path.Combine(workspace, "vault")

            Try
                Dim service As New Global.FileCabinet.CatalogService(catalogPath, vaultRoot)
                Dim catalog = service.LoadOrCreate()
                catalog.Artifacts.Add(New Global.FileCabinet.ArtifactModel With {
                    .Id = "artifact-1",
                    .Name = "sample.txt"
                })

                Dim validation = service.ExportSnapshotWithValidation(catalog, Path.Combine(vaultRoot, "exports"))

                Assert.IsTrue(validation.IsValid)
                Assert.IsTrue(File.Exists(validation.BackupPath))
                StringAssert.Contains(validation.Detail, "1 artifact")
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub

        <TestMethod>
        Sub ValidateBackupRejectsCorruptOrIncompleteBackup()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim backupPath = Path.Combine(workspace, "exports", "catalog-backup-corrupt.json")
            Directory.CreateDirectory(Path.GetDirectoryName(backupPath))

            Try
                File.WriteAllText(backupPath, "{""SchemaVersion"":1,""Vaults"":null,""Artifacts"":null}")
                Dim service As New Global.FileCabinet.CatalogService(Path.Combine(workspace, "appdata", "catalog.json"), Path.Combine(workspace, "vault"))

                Dim validation = service.ValidateBackup(backupPath)

                Assert.IsFalse(validation.IsValid)
                StringAssert.Contains(validation.Detail, "required catalog collections")
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub

        <TestMethod>
        Sub LoadOrCreateAddsPreferenceDefaultsToOlderCatalog()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim catalogPath = Path.Combine(workspace, "appdata", "catalog.json")
            Dim vaultRoot = Path.Combine(workspace, "vault")
            Directory.CreateDirectory(Path.GetDirectoryName(catalogPath))
            File.WriteAllText(catalogPath, $"{{""SchemaVersion"":1,""CurrentVaultId"":""main"",""VaultRootPath"":""{vaultRoot.Replace("\", "\\")}"",""DefaultIngestMode"":""Move"",""DuplicatePolicy"":""Rename"",""Vaults"":[{{""Id"":""main"",""Name"":""MainVault"",""Path"":""{vaultRoot.Replace("\", "\\")}""}}],""Artifacts"":[{{""Id"":""artifact-1"",""Name"":""legacy.txt""}}]}}")

            Try
                Dim service As New Global.FileCabinet.CatalogService(catalogPath, vaultRoot)
                Dim catalog = service.LoadOrCreate()

                Assert.AreEqual("Comfortable", catalog.TableDensity)
                Assert.AreEqual("Full", catalog.ColumnPreset)
                Assert.AreEqual("All", catalog.ActiveScope)
                Assert.AreEqual("", catalog.SearchText)
                Assert.AreEqual("", catalog.TagSearchText)
                Assert.AreEqual("", catalog.SelectedTag)
                Assert.AreEqual("", catalog.SelectedCategory)
                Assert.AreEqual(1, catalog.Artifacts.Count)
                Assert.AreEqual("Unknown", catalog.Artifacts(0).TrustClassification)
                Assert.AreEqual("Normal", catalog.Artifacts(0).RetentionPriority)
                Assert.AreEqual("Active", catalog.Artifacts(0).ArchiveStatus)

                catalog.Artifacts(0).RetentionReason = "Keep for recovery"
                catalog.Artifacts(0).WhyThisMatters = "Documents restore context"
                catalog.Artifacts(0).SourceProvenance = "Aptlantis release share"
                catalog.Artifacts(0).AcquisitionMethod = "Manual import"
                catalog.Artifacts(0).TrustClassification = "Trusted"
                catalog.Artifacts(0).RetentionPriority = "High"
                catalog.Artifacts(0).ArchiveStatus = "Archived"
                service.Save(catalog)

                Dim reloaded = service.LoadOrCreate()
                Assert.AreEqual("Keep for recovery", reloaded.Artifacts(0).RetentionReason)
                Assert.AreEqual("Documents restore context", reloaded.Artifacts(0).WhyThisMatters)
                Assert.AreEqual("Aptlantis release share", reloaded.Artifacts(0).SourceProvenance)
                Assert.AreEqual("Manual import", reloaded.Artifacts(0).AcquisitionMethod)
                Assert.AreEqual("Trusted", reloaded.Artifacts(0).TrustClassification)
                Assert.AreEqual("High", reloaded.Artifacts(0).RetentionPriority)
                Assert.AreEqual("Archived", reloaded.Artifacts(0).ArchiveStatus)
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub
    End Class
End Namespace
