Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.IO

Namespace FileCabinet.Tests
    <TestClass>
    Public Class RelationScoringTests
        <TestMethod>
        Sub DuplicateHashAndSharedTagsProduceExplainableRelation()
            Dim selected = Artifact("release-manifest.toml", "Manifests / Config", "Text", "abc123", {"release", "config"})
            Dim candidate = Artifact("release-notes.toml", "Manifests / Config", "Text", "abc123", {"release", "review"})

            Dim relation = Global.FileCabinet.MainViewModel.BuildArtifactRelation(selected, candidate)

            Assert.IsNotNull(relation)
            Assert.IsTrue(relation.Score >= 21)
            CollectionAssert.Contains(relation.Reasons, "duplicate SHA-256")
            Assert.IsTrue(relation.Reasons.Any(Function(reason) reason.Contains("shared tag: release")))
            CollectionAssert.Contains(relation.Reasons, "same category")
            CollectionAssert.Contains(relation.Reasons, "same type family")
        End Sub

        <TestMethod>
        Sub SameOriginalFolderAndDateBatchProduceRelation()
            Dim root = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Dim sourceRoot = Path.Combine(root, "source")
            Directory.CreateDirectory(sourceRoot)

            Try
                Dim selected = Artifact("camera-roll-raw.zip", "Archives", "Archive", "hash-a", {})
                selected.OriginalPath = Path.Combine(sourceRoot, "camera-roll-raw.zip")
                selected.IngestedAt = "2026-05-23 16:00"

                Dim candidate = Artifact("camera-roll-preview.png", "Images", "Image", "hash-b", {})
                candidate.OriginalPath = Path.Combine(sourceRoot, "camera-roll-preview.png")
                candidate.IngestedAt = "2026-05-23 17:30"

                Dim relation = Global.FileCabinet.MainViewModel.BuildArtifactRelation(selected, candidate)

                Assert.IsNotNull(relation)
                CollectionAssert.Contains(relation.Reasons, "same original folder")
                CollectionAssert.Contains(relation.Reasons, "same date batch")
                Assert.IsTrue(relation.Reasons.Any(Function(reason) reason.Contains("matching name token: camera")))
            Finally
                If Directory.Exists(root) Then
                    Directory.Delete(root, recursive:=True)
                End If
            End Try
        End Sub

        Private Shared Function Artifact(name As String, category As String, typeFamily As String, sha256 As String, tags As IEnumerable(Of String)) As Global.FileCabinet.ArtifactModel
            Return New Global.FileCabinet.ArtifactModel With {
                .Name = name,
                .Category = category,
                .TypeFamily = typeFamily,
                .Sha256 = sha256,
                .Tags = tags.ToList()
            }
        End Function
    End Class
End Namespace
