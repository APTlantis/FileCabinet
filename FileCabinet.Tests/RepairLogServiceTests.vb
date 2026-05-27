Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace FileCabinet.Tests
    <TestClass>
    Public Class RepairLogServiceTests
        <TestMethod>
        Sub AppendCreatesVaultLocalJsonLinesRepairLog()
            Dim vaultRoot = CreateTempVaultRoot()
            Try
                Dim service = New Global.FileCabinet.RepairLogService()

                Dim logPath = service.Append(vaultRoot, New Global.FileCabinet.RepairLogEntry With {
                    .ActionType = "RecomputeHash",
                    .FindingType = "Missing hash",
                    .Subject = "sample.iso",
                    .ProposedAction = "Recompute hash",
                    .Result = "Applied",
                    .Detail = "Repair completed",
                    .MutatesCatalog = True,
                    .TouchesRetainedFiles = False
                })

                Assert.AreEqual(Path.Combine(vaultRoot, "catalog", "repair-log.jsonl"), logPath)
                Assert.IsTrue(File.Exists(logPath))
                Assert.AreEqual(1, File.ReadAllLines(logPath).Length)
            Finally
                Directory.Delete(vaultRoot, recursive:=True)
            End Try
        End Sub

        <TestMethod>
        Sub ReadRecentReturnsNewestEntriesFirst()
            Dim vaultRoot = CreateTempVaultRoot()
            Try
                Dim service = New Global.FileCabinet.RepairLogService()

                service.Append(vaultRoot, Entry("first.iso"))
                service.Append(vaultRoot, Entry("second.iso"))

                Dim recent = service.ReadRecent(vaultRoot, count:=2)

                Assert.AreEqual(2, recent.Count)
                Assert.AreEqual("second.iso", recent(0).Subject)
                Assert.AreEqual("first.iso", recent(1).Subject)
            Finally
                Directory.Delete(vaultRoot, recursive:=True)
            End Try
        End Sub

        Private Shared Function Entry(subject As String) As Global.FileCabinet.RepairLogEntry
            Return New Global.FileCabinet.RepairLogEntry With {
                .ActionType = "RegenerateThumbnail",
                .FindingType = "Missing thumbnail",
                .Subject = subject,
                .ProposedAction = "Regenerate thumbnail",
                .Result = "Applied",
                .Detail = "Repair completed",
                .MutatesCatalog = True,
                .TouchesRetainedFiles = False
            }
        End Function

        Private Shared Function CreateTempVaultRoot() As String
            Dim tempRoot = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(tempRoot)
            Return tempRoot
        End Function
    End Class
End Namespace
