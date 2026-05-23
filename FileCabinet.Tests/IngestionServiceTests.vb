Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.IO

Namespace FileCabinet.Tests
    <TestClass>
    Public Class IngestionServiceTests
        <TestMethod>
        Sub MoveIngestCreatesVaultArtifactAndRemovesOriginal()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim sourceRoot = Path.Combine(workspace, "source")
            Dim vaultRoot = Path.Combine(workspace, "vault")
            Directory.CreateDirectory(sourceRoot)
            Dim sourcePath = Path.Combine(sourceRoot, "manifest.toml")
            File.WriteAllText(sourcePath, "name = ""demo""")

            Try
                Dim service As New Global.FileCabinet.IngestionService()
                Dim artifacts = service.Ingest({sourcePath}, vaultRoot, Nothing, Global.FileCabinet.IngestMode.Move)

                Assert.AreEqual(1, artifacts.Count)
                Assert.IsFalse(File.Exists(sourcePath), "Move intake should remove the original after transfer.")
                Assert.IsTrue(File.Exists(artifacts(0).Path))
                Assert.AreEqual(sourcePath, artifacts(0).OriginalPath)
                Assert.AreEqual("Manifests / Config", artifacts(0).Category)
                Assert.AreEqual("Verified", artifacts(0).HashStatus)
                Assert.IsFalse(String.IsNullOrWhiteSpace(artifacts(0).Id))
                Assert.IsFalse(String.IsNullOrWhiteSpace(artifacts(0).RelativePath))
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub

        <TestMethod>
        Sub CopyIngestRenamesDuplicateDestinations()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim sourceRoot = Path.Combine(workspace, "source")
            Dim vaultRoot = Path.Combine(workspace, "vault")
            Directory.CreateDirectory(sourceRoot)
            Dim firstPath = Path.Combine(sourceRoot, "same.txt")
            Dim secondRoot = Path.Combine(workspace, "second")
            Directory.CreateDirectory(secondRoot)
            Dim secondPath = Path.Combine(secondRoot, "same.txt")
            File.WriteAllText(firstPath, "first")
            File.WriteAllText(secondPath, "second")

            Try
                Dim service As New Global.FileCabinet.IngestionService()
                Dim artifacts = service.Ingest({firstPath, secondPath}, vaultRoot, Nothing, Global.FileCabinet.IngestMode.Copy)

                Assert.AreEqual(2, artifacts.Count)
                Assert.IsTrue(File.Exists(firstPath), "Copy intake should preserve the original.")
                Assert.IsTrue(File.Exists(secondPath), "Copy intake should preserve the original.")
                Assert.AreNotEqual(artifacts(0).Path, artifacts(1).Path)
                Assert.IsTrue(File.Exists(artifacts(0).Path))
                Assert.IsTrue(File.Exists(artifacts(1).Path))
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub
    End Class
End Namespace
