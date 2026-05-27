Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace FileCabinet.Tests
    <TestClass>
    Public Class RepairCandidateTests
        <TestMethod>
        Sub MissingThumbnailMapsToRegenerateThumbnailCandidate()
            Dim candidate = Global.FileCabinet.MainViewModel.BuildRepairCandidate(Finding("Missing thumbnail"))

            Assert.AreEqual("RegenerateThumbnail", candidate.ActionType)
            Assert.IsTrue(candidate.CanRepairAutomatically)
            Assert.IsTrue(candidate.RequiresOperatorApproval)
            Assert.AreEqual("Approval required", candidate.ApprovalText)
        End Sub

        <TestMethod>
        Sub MissingHashMapsToRecomputeHashCandidate()
            Dim candidate = Global.FileCabinet.MainViewModel.BuildRepairCandidate(Finding("Missing hash"))

            Assert.AreEqual("RecomputeHash", candidate.ActionType)
            Assert.IsTrue(candidate.CanRepairAutomatically)
            Assert.IsTrue(candidate.RequiresOperatorApproval)
        End Sub

        <TestMethod>
        Sub ReviewFindingsRemainReviewOnly()
            Dim duplicate = Global.FileCabinet.MainViewModel.BuildRepairCandidate(Finding("Duplicate hash"))
            Dim orphanThumbnail = Global.FileCabinet.MainViewModel.BuildRepairCandidate(Finding("Orphan thumbnail"))

            Assert.AreEqual("ReviewOnly", duplicate.ActionType)
            Assert.IsFalse(duplicate.CanRepairAutomatically)
            Assert.AreEqual("ReviewOnly", orphanThumbnail.ActionType)
            Assert.IsFalse(orphanThumbnail.CanRepairAutomatically)
        End Sub

        Private Shared Function Finding(findingType As String) As Global.FileCabinet.VaultHealthFinding
            Return New Global.FileCabinet.VaultHealthFinding With {
                .FindingType = findingType,
                .Subject = "sample",
                .ProposedAction = "Review"
            }
        End Function
    End Class
End Namespace
