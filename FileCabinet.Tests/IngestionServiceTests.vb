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
                Assert.AreEqual("Extracted", artifacts(0).ExtractedTextStatus)
                Assert.IsFalse(String.IsNullOrWhiteSpace(artifacts(0).ExtractedTextRelativePath))
                Assert.IsTrue(File.Exists(Path.Combine(vaultRoot, artifacts(0).ExtractedTextRelativePath)))
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

        <TestMethod>
        Sub StoredFileAdoptionCreatesCatalogReadyArtifact()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim vaultRoot = Path.Combine(workspace, "vault")
            Dim itemsRoot = Path.Combine(vaultRoot, "items")
            Directory.CreateDirectory(itemsRoot)
            Dim storedPath = Path.Combine(itemsRoot, "orphan.json")
            File.WriteAllText(storedPath, "{""ok"":true}")

            Try
                Dim service As New Global.FileCabinet.IngestionService()
                Dim artifact = service.CreateArtifactFromStoredFile(storedPath, vaultRoot)

                Assert.AreEqual("orphan.json", artifact.Name)
                Assert.AreEqual("Manifests / Config", artifact.Category)
                Assert.AreEqual("Text", artifact.TypeFamily)
                Assert.AreEqual("Verified", artifact.HashStatus)
                Assert.AreEqual(Path.Combine("items", "orphan.json"), artifact.RelativePath)
                Assert.AreEqual(storedPath, artifact.OriginalPath)
                Assert.AreEqual("Extracted", artifact.ExtractedTextStatus)
                Assert.IsTrue(File.Exists(Path.Combine(vaultRoot, artifact.ExtractedTextRelativePath)))
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub

        <TestMethod>
        Sub BinaryLikeStoredFileIsMarkedNotExtractable()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim vaultRoot = Path.Combine(workspace, "vault")
            Dim itemsRoot = Path.Combine(vaultRoot, "items")
            Directory.CreateDirectory(itemsRoot)
            Dim storedPath = Path.Combine(itemsRoot, "disk.iso")
            File.WriteAllBytes(storedPath, {0, 1, 2, 3})

            Try
                Dim service As New Global.FileCabinet.IngestionService()
                Dim artifact = service.CreateArtifactFromStoredFile(storedPath, vaultRoot)

                Assert.AreEqual("Not extractable", artifact.ExtractedTextStatus)
                Assert.AreEqual("", artifact.ExtractedTextRelativePath)
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub
    End Class
End Namespace
