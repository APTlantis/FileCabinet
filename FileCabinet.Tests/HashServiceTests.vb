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
        Sub ComputeHashesReturnsExpandedKnownValuesForEmptyFile()
            Using workspace As New TestWorkspace()
                Dim path = workspace.SourcePath("empty.bin")
                File.WriteAllText(path, "")

                Dim hashes = New Global.FileCabinet.HashService().ComputeHashes(path, "SHA256,BLAKE3,KangarooTwelve,SHA3-256,MD5,Whirlpool,Skein")

                Assert.AreEqual("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", hashes.Sha256)
                Assert.AreEqual("af1349b9f5f9a1a6a0404dea36dcc9499bcb25c9adc112b7cc9a93cae41f3262", hashes.Blake3)
                Assert.AreEqual("1ac2d450fc3b4205d19da7bfca1b37513c0803577ac7167f06fe2ce1f0ef39e5", hashes.KangarooTwelve)
                Assert.AreEqual("a7ffc6f8bf1ed76651c14756a061d662f580ff4de43b49fa82d80a4b80f8434a", hashes.Sha3_256)
                Assert.AreEqual("d41d8cd98f00b204e9800998ecf8427e", hashes.Md5)
                Assert.AreEqual("19fa61d75522a4669b44e39c1d2e1726c530232130d407f89afee0964997f7a73e83be698b288febcf88e3e03c4f0757ea8964e59b63d93708b138cc42a66eb3", hashes.Whirlpool)
                Assert.AreEqual("bc5b4c50925519c290cc634277ae3d6257212395cba733bbad37a4af0fa06af41fca7903d06564fea7a2d3730dbdb80c1f85562dfcc070334ea4d1d9e72cba7a", hashes.Skein)
            End Using
        End Sub

        <TestMethod>
        Sub ComputeHashesOnlyComputesRequestedActiveHashes()
            Using workspace As New TestWorkspace()
                Dim path = workspace.SourcePath("legacy.txt")
                File.WriteAllText(path, "legacy")

                Dim hashes = New Global.FileCabinet.HashService().ComputeHashes(path, "MD5")

                Assert.AreEqual("228c70bfc5589c58c044e03fff0e17eb", hashes.Md5)
                Assert.AreEqual("", hashes.Sha256)
                Assert.AreEqual("", hashes.Blake3)
                Assert.AreEqual("", hashes.KangarooTwelve)
            End Using
        End Sub

        <TestMethod>
        Sub HashRegistryNormalizesUnknownAndEmptySelectionsToDefaults()
            Assert.AreEqual("SHA256,BLAKE3,KangarooTwelve", Global.FileCabinet.HashRegistry.NormalizeActiveHashes(""))
            Assert.AreEqual("SHA256,BLAKE3,KangarooTwelve", Global.FileCabinet.HashRegistry.NormalizeActiveHashes("not-a-hash"))
            Assert.AreEqual("BLAKE3,MD5", Global.FileCabinet.HashRegistry.NormalizeActiveHashes("blake3, md5, nope"))
        End Sub

        <TestMethod>
        Sub ApplyingComputedHashesPreservesInactiveCatalogValues()
            Dim artifact = New Global.FileCabinet.ArtifactModel With {
                .Sha256 = "old-sha",
                .Md5 = "legacy-md5"
            }
            Dim hashes = New Global.FileCabinet.FileHashes With {
                .Sha256 = "new-sha"
            }

            Global.FileCabinet.HashRegistry.ApplyHashesToArtifact(artifact, hashes)

            Assert.AreEqual("new-sha", artifact.Sha256)
            Assert.AreEqual("legacy-md5", artifact.Md5)
            Assert.AreEqual("Verified", artifact.HashStatus)
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
