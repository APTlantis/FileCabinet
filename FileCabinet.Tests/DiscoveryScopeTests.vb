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
