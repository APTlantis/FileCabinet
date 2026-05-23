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
    End Class
End Namespace
