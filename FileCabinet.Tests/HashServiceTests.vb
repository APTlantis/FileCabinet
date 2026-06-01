Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.IO

Namespace FileCabinet.Tests
    <TestClass>
    Public Class HashServiceTests
        <TestMethod>
        Sub ComputeHashesReturnsStableKnownValues()
            Using workspace As New TestWorkspace()
                Dim path = workspace.SourcePath("abc.txt")
                File.WriteAllText(path, "abc")

                Dim hashes = New Global.FileCabinet.HashService().ComputeHashes(path)

                Assert.AreEqual("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", hashes.Sha256)
                Assert.AreEqual("6437b3ac38465133ffb63b75273a8db548c558465d79db03fd359c6cd5bd9d85", hashes.Blake3)
            End Using
        End Sub

        <TestMethod>
        Sub RecomputingHashesDoesNotDependOnPathOrMutateFiles()
            Using workspace As New TestWorkspace()
                Dim firstPath = workspace.SourcePath("first.bin")
                Dim secondPath = workspace.SourcePath("second.bin")
                Dim content = New Byte() {0, 1, 2, 3, 4, 5, 250, 255}
                File.WriteAllBytes(firstPath, content)
                File.WriteAllBytes(secondPath, content)

                Dim service = New Global.FileCabinet.HashService()
                Dim firstBefore = File.ReadAllBytes(firstPath)
                Dim firstHashes = service.ComputeHashes(firstPath)
                Dim secondHashes = service.ComputeHashes(secondPath)
                Dim firstAfter = File.ReadAllBytes(firstPath)

                CollectionAssert.AreEqual(firstBefore, firstAfter)
                Assert.AreEqual(firstHashes.Sha256, secondHashes.Sha256)
                Assert.AreEqual(firstHashes.Blake3, secondHashes.Blake3)
            End Using
        End Sub
    End Class
End Namespace
