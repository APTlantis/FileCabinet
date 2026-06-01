Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.IO

Namespace FileCabinet.Tests
    <TestClass>
    Public Class DiscoveryScopeTests
        <TestMethod>
        Sub SavedDiscoveryScopesMatchDeterministicArtifactState()
            Dim vaultRoot = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(vaultRoot)

            Try
                Dim trusted = Artifact("trusted.txt")
                trusted.HashStatus = "Verified"

                Dim unverified = Artifact("unverified.txt")
                unverified.TrustClassification = "Unverified"

                Dim missingPreview = Artifact("image.png")
                missingPreview.ThumbnailStatus = Global.FileCabinet.ThumbnailService.GeneratedStatus
                missingPreview.ThumbnailRelativePath = Path.Combine("thumbnails", "missing.png")

                Dim duplicateA = Artifact("duplicate-a.zip")
                duplicateA.Sha256 = "same-hash"
                Dim duplicateB = Artifact("duplicate-b.zip")
                duplicateB.Sha256 = "same-hash"

                Dim selectedBatch = Artifact("batch-a.json")
                selectedBatch.OriginalPath = Path.Combine(vaultRoot, "source", "batch-a.json")
                selectedBatch.IngestedAt = "2026-05-23 10:00"
                Dim sameBatch = Artifact("batch-b.json")
                sameBatch.OriginalPath = Path.Combine(vaultRoot, "source", "batch-b.json")
                sameBatch.IngestedAt = "2026-05-23 12:00"

                Dim large = Artifact("large.iso")
                large.SizeBytes = 1024L * 1024L * 1024L

                Dim artifacts = {trusted, unverified, missingPreview, duplicateA, duplicateB, selectedBatch, sameBatch, large}

                Assert.IsTrue(Global.FileCabinet.MainViewModel.ArtifactMatchesDiscoveryScope(unverified, "Unverified", artifacts, vaultRoot))
                Assert.IsTrue(Global.FileCabinet.MainViewModel.ArtifactMatchesDiscoveryScope(missingPreview, "Missing preview", artifacts, vaultRoot))
                Assert.IsTrue(Global.FileCabinet.MainViewModel.ArtifactMatchesDiscoveryScope(missingPreview, "Repair needed", artifacts, vaultRoot))
                Assert.IsTrue(Global.FileCabinet.MainViewModel.ArtifactMatchesDiscoveryScope(duplicateA, "Duplicate candidates", artifacts, vaultRoot))
                Assert.IsTrue(Global.FileCabinet.MainViewModel.ArtifactMatchesDiscoveryScope(sameBatch, "Same source batch", artifacts, vaultRoot, selectedBatch))
                Assert.IsTrue(Global.FileCabinet.MainViewModel.ArtifactMatchesDiscoveryScope(large, "Large artifacts", artifacts, vaultRoot))

                Assert.IsFalse(Global.FileCabinet.MainViewModel.ArtifactMatchesDiscoveryScope(trusted, "Unverified", artifacts, vaultRoot))
            Finally
                If Directory.Exists(vaultRoot) Then
                    Directory.Delete(vaultRoot, recursive:=True)
                End If
            End Try
        End Sub

        <TestMethod>
        Sub SameSourceBatchScopeKeysMatchSelectedArtifactBatch()
            Dim sourceRoot = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"), "source")
            Dim selected = Artifact("batch-a.json")
            selected.OriginalPath = Path.Combine(sourceRoot, "batch-a.json")
            selected.IngestedAt = "2026-05-23 10:00"

            Dim sameBatch = Artifact("batch-b.json")
            sameBatch.OriginalPath = Path.Combine(sourceRoot, "batch-b.json")
            sameBatch.IngestedAt = "2026-05-23 12:00"

            Dim tooLate = Artifact("batch-c.json")
            tooLate.OriginalPath = Path.Combine(sourceRoot, "batch-c.json")
            tooLate.IngestedAt = "2026-05-23 18:30"

            Dim otherFolder = Artifact("other.json")
            otherFolder.OriginalPath = Path.Combine(sourceRoot, "other", "other.json")
            otherFolder.IngestedAt = "2026-05-23 12:00"

            Dim keys = Global.FileCabinet.MainViewModel.BuildSameSourceBatchScopeKeys({selected, sameBatch, tooLate, otherFolder}, selected)

            Assert.IsTrue(keys.Contains(sameBatch.Id))
            Assert.IsFalse(keys.Contains(selected.Id))
            Assert.IsFalse(keys.Contains(tooLate.Id))
            Assert.IsFalse(keys.Contains(otherFolder.Id))
        End Sub

        <TestMethod>
        Sub SameSourceBatchScopeKeysFindBatchPeersWithoutSelectedArtifact()
            Dim sourceRoot = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"), "source")
            Dim batchA = Artifact("batch-a.json")
            batchA.OriginalPath = Path.Combine(sourceRoot, "batch-a.json")
            batchA.IngestedAt = "2026-05-23 10:00"

            Dim batchB = Artifact("batch-b.json")
            batchB.OriginalPath = Path.Combine(sourceRoot, "batch-b.json")
            batchB.IngestedAt = "2026-05-23 12:00"

            Dim isolated = Artifact("isolated.json")
            isolated.OriginalPath = Path.Combine(sourceRoot, "isolated.json")
            isolated.IngestedAt = "2026-05-23 18:30"

            Dim keys = Global.FileCabinet.MainViewModel.BuildSameSourceBatchScopeKeys({batchA, batchB, isolated}, Nothing)

            Assert.IsTrue(keys.Contains(batchA.Id))
            Assert.IsTrue(keys.Contains(batchB.Id))
            Assert.IsFalse(keys.Contains(isolated.Id))
        End Sub

        <TestMethod>
        Sub SameSourceBatchScopeKeysHandleLargeCatalogShape()
            Dim sourceRoot = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"), "source")
            Dim artifacts As New List(Of Global.FileCabinet.ArtifactModel)

            For index = 0 To 999
                Dim artifactItem = Artifact($"artifact-{index}.txt")
                artifactItem.OriginalPath = Path.Combine(sourceRoot, $"folder-{index}", artifactItem.Name)
                artifactItem.IngestedAt = "2026-05-23 10:00"
                artifacts.Add(artifactItem)
            Next

            Dim batchA = Artifact("batch-a.json")
            batchA.OriginalPath = Path.Combine(sourceRoot, "batched", "batch-a.json")
            batchA.IngestedAt = "2026-05-23 10:00"
            artifacts.Add(batchA)

            Dim batchB = Artifact("batch-b.json")
            batchB.OriginalPath = Path.Combine(sourceRoot, "batched", "batch-b.json")
            batchB.IngestedAt = "2026-05-23 13:30"
            artifacts.Add(batchB)

            Dim keys = Global.FileCabinet.MainViewModel.BuildSameSourceBatchScopeKeys(artifacts, Nothing)

            Assert.AreEqual(2, keys.Count)
            Assert.IsTrue(keys.Contains(batchA.Id))
            Assert.IsTrue(keys.Contains(batchB.Id))
        End Sub

        Private Shared Function Artifact(name As String) As Global.FileCabinet.ArtifactModel
            Return New Global.FileCabinet.ArtifactModel With {
                .Id = Guid.NewGuid().ToString("N"),
                .Name = name,
                .Path = Path.Combine(Path.GetTempPath(), name),
                .HashStatus = "Verified",
                .TrustClassification = "Trusted",
                .RetentionPriority = "Normal",
                .ArchiveStatus = "Active"
            }
        End Function
    End Class
End Namespace
