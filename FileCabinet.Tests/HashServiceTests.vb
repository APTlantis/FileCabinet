Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.IO

Namespace FileCabinet.Tests
    <TestClass>
    Public Class HashServiceTests
        Private Const ExpandedCompatibilityHashes As String = "cksum-posix,crc8,crc16,crc32,crc64,adler32,bsd-sum16,sysv-sum16,internet-checksum16,sum8,sum24,sum32,fletcher8,fletcher16,fletcher32,xor8,fnv1-32,fnv1a-32,fnv1a-64,jenkins-one-at-a-time32,djb2-32,sdbm-32,murmur3-32,xxhash64"

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
        Sub ComputeHashesReturnsExpandedCompatibilityVectors()
            Using workspace As New TestWorkspace()
                Dim emptyPath = workspace.SourcePath("empty.bin")
                Dim abcPath = workspace.SourcePath("abc.txt")
                Dim binaryPath = workspace.SourcePath("binary.bin")
                File.WriteAllBytes(emptyPath, Array.Empty(Of Byte)())
                File.WriteAllText(abcPath, "abc")
                File.WriteAllBytes(binaryPath, New Byte() {0, 1, 2, 3, 4, 5, 250, 255})

                AssertCompatibilityVectors(emptyPath, New Dictionary(Of String, String) From {
                    {"cksum-posix", "ffffffff"}, {"crc8", "00"}, {"crc16", "0000"}, {"crc32", "00000000"},
                    {"crc64", "0000000000000000"}, {"adler32", "00000001"}, {"bsd-sum16", "0000"}, {"sysv-sum16", "0000"},
                    {"internet-checksum16", "ffff"}, {"sum8", "00"}, {"sum24", "000000"}, {"sum32", "00000000"},
                    {"fletcher8", "00"}, {"fletcher16", "0000"}, {"fletcher32", "00000000"}, {"xor8", "00"},
                    {"fnv1-32", "811c9dc5"}, {"fnv1a-32", "811c9dc5"}, {"fnv1a-64", "cbf29ce484222325"},
                    {"jenkins-one-at-a-time32", "00000000"}, {"djb2-32", "00001505"}, {"sdbm-32", "00000000"},
                    {"murmur3-32", "00000000"}, {"xxhash64", "ef46db3751d8e999"}
                })

                AssertCompatibilityVectors(abcPath, New Dictionary(Of String, String) From {
                    {"cksum-posix", "48aa78a2"}, {"crc8", "5f"}, {"crc16", "9738"}, {"crc32", "352441c2"},
                    {"crc64", "66501a349a0e0855"}, {"adler32", "024d0127"}, {"bsd-sum16", "40ac"}, {"sysv-sum16", "0126"},
                    {"internet-checksum16", "3b9d"}, {"sum8", "26"}, {"sum24", "000126"}, {"sum32", "00000126"},
                    {"fletcher8", "e9"}, {"fletcher16", "4c27"}, {"fletcher32", "25c5c462"}, {"xor8", "60"},
                    {"fnv1-32", "439c2f4b"}, {"fnv1a-32", "1a47e90b"}, {"fnv1a-64", "e71fa2190541574b"},
                    {"jenkins-one-at-a-time32", "ed131f5b"}, {"djb2-32", "0b885c8b"}, {"sdbm-32", "3025f862"},
                    {"murmur3-32", "b3dd93fa"}, {"xxhash64", "44bc2cf5ad770999"}
                })

                AssertCompatibilityVectors(binaryPath, New Dictionary(Of String, String) From {
                    {"cksum-posix", "4ec57f39"}, {"crc8", "d6"}, {"crc16", "f346"}, {"crc32", "83c5bc00"},
                    {"crc64", "7ead6d7fe511d093"}, {"adler32", "033c0209"}, {"bsd-sum16", "057e"}, {"sysv-sum16", "0208"},
                    {"internet-checksum16", "fef6"}, {"sum8", "08"}, {"sum24", "000208"}, {"sum32", "00000208"},
                    {"fletcher8", "5a"}, {"fletcher16", "370a"}, {"fletcher32", "09170109"}, {"xor8", "04"},
                    {"fnv1-32", "143dbc71"}, {"fnv1a-32", "54b3f729"}, {"fnv1a-64", "a27931e2b0882289"},
                    {"jenkins-one-at-a-time32", "22e145c7"}, {"djb2-32", "cd90898d"}, {"sdbm-32", "8bd49d08"},
                    {"murmur3-32", "ab8a9277"}, {"xxhash64", "4426b4699053f315"}
                })
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
            Assert.AreEqual("BLAKE3,MD5,crc32", Global.FileCabinet.HashRegistry.NormalizeActiveHashes("blake3, md5, crc32, nope"))
            Assert.AreEqual(31, Global.FileCabinet.HashRegistry.Options.Count)
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
        Sub MappedHashesUseExtensibleCatalogDictionary()
            Dim artifact = New Global.FileCabinet.ArtifactModel With {
                .Sha256 = "legacy-sha"
            }
            Dim hashes = New Global.FileCabinet.FileHashes()
            Global.FileCabinet.HashRegistry.SetHashValue(hashes, "crc32", "352441c2")

            Global.FileCabinet.HashRegistry.ApplyHashesToArtifact(artifact, hashes)

            Assert.AreEqual("legacy-sha", artifact.Sha256)
            Assert.AreEqual("352441c2", artifact.Hashes("crc32"))
            Assert.AreEqual("352441c2", Global.FileCabinet.HashRegistry.GetArtifactHashValue(artifact, "CRC-32/IEEE"))
        End Sub

        <TestMethod>
        Sub DynamicHashSettingsExposeAllRegistryOptions()
            Dim viewModel = New Global.FileCabinet.MainViewModel()
            Dim settings = viewModel.HashSettingOptions.ToList()

            Assert.AreEqual(Global.FileCabinet.HashRegistry.Options.Count, settings.Count)
            Assert.IsTrue(settings.Any(Function(optionItem) optionItem.Id = "cksum-posix"))
            Assert.IsTrue(settings.Any(Function(optionItem) optionItem.Id = "xxhash64"))
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

        Private Shared Sub AssertCompatibilityVectors(path As String, expected As Dictionary(Of String, String))
            Dim hashes = New Global.FileCabinet.HashService().ComputeHashes(path, ExpandedCompatibilityHashes)

            For Each pair In expected
                Assert.AreEqual(pair.Value, Global.FileCabinet.HashRegistry.GetHashValue(hashes, pair.Key), pair.Key)
            Next
        End Sub
    End Class
End Namespace
