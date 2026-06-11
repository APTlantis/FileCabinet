Imports System.IO
Imports System.Numerics
Imports System.Security.Cryptography
Imports Org.BouncyCastle.Crypto.Digests

Public Class HashOption
    Public Property Id As String = ""
    Public Property DisplayName As String = ""
    Public Property CatalogPropertyName As String = ""
    Public Property IsDefaultEnabled As Boolean
    Public Property Note As String = ""
End Class

Public Class FileHashes
    Private _hashes As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

    Public Property Blake3 As String = ""
    Public Property Sha256 As String = ""
    Public Property KangarooTwelve As String = ""
    Public Property Sha3_256 As String = ""
    Public Property Md5 As String = ""
    Public Property Whirlpool As String = ""
    Public Property Skein As String = ""

    Public Property Hashes As Dictionary(Of String, String)
        Get
            Return _hashes
        End Get
        Set(value As Dictionary(Of String, String))
            If value Is Nothing Then
                _hashes = New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
            Else
                _hashes = New Dictionary(Of String, String)(value, StringComparer.OrdinalIgnoreCase)
            End If
        End Set
    End Property
End Class

Public Class HashRegistry
    Public Shared ReadOnly Property Options As IReadOnlyList(Of HashOption) = New List(Of HashOption) From {
        New HashOption With {.Id = "SHA256", .DisplayName = "SHA-256", .CatalogPropertyName = NameOf(FileHashes.Sha256), .IsDefaultEnabled = True},
        New HashOption With {.Id = "BLAKE3", .DisplayName = "BLAKE3", .CatalogPropertyName = NameOf(FileHashes.Blake3), .IsDefaultEnabled = True},
        New HashOption With {.Id = "KangarooTwelve", .DisplayName = "KangarooTwelve", .CatalogPropertyName = NameOf(FileHashes.KangarooTwelve), .IsDefaultEnabled = True},
        New HashOption With {.Id = "SHA3-256", .DisplayName = "SHA3-256", .CatalogPropertyName = NameOf(FileHashes.Sha3_256), .IsDefaultEnabled = False},
        New HashOption With {.Id = "MD5", .DisplayName = "MD5", .CatalogPropertyName = NameOf(FileHashes.Md5), .IsDefaultEnabled = False, .Note = "Legacy compatibility only; not collision-resistant."},
        New HashOption With {.Id = "Whirlpool", .DisplayName = "Whirlpool", .CatalogPropertyName = NameOf(FileHashes.Whirlpool), .IsDefaultEnabled = False},
        New HashOption With {.Id = "Skein", .DisplayName = "Skein-512", .CatalogPropertyName = NameOf(FileHashes.Skein), .IsDefaultEnabled = False},
        New HashOption With {.Id = "cksum-posix", .DisplayName = "cksum (POSIX)", .IsDefaultEnabled = False, .Note = "Compatibility checksum; not cryptographic."},
        New HashOption With {.Id = "crc8", .DisplayName = "CRC-8/SMBus", .IsDefaultEnabled = False, .Note = "Compatibility checksum; not cryptographic."},
        New HashOption With {.Id = "crc16", .DisplayName = "CRC-16/ARC", .IsDefaultEnabled = False, .Note = "Compatibility checksum; not cryptographic."},
        New HashOption With {.Id = "crc32", .DisplayName = "CRC-32/IEEE", .IsDefaultEnabled = False, .Note = "Compatibility checksum; not cryptographic."},
        New HashOption With {.Id = "crc64", .DisplayName = "CRC-64/ECMA", .IsDefaultEnabled = False, .Note = "Compatibility checksum; not cryptographic."},
        New HashOption With {.Id = "adler32", .DisplayName = "Adler-32", .IsDefaultEnabled = False, .Note = "Compatibility checksum; not cryptographic."},
        New HashOption With {.Id = "bsd-sum16", .DisplayName = "BSD sum16", .IsDefaultEnabled = False, .Note = "Compatibility checksum; not cryptographic."},
        New HashOption With {.Id = "sysv-sum16", .DisplayName = "SYSV sum16", .IsDefaultEnabled = False, .Note = "Compatibility checksum; not cryptographic."},
        New HashOption With {.Id = "internet-checksum16", .DisplayName = "Internet checksum", .IsDefaultEnabled = False, .Note = "Compatibility checksum; not cryptographic."},
        New HashOption With {.Id = "sum8", .DisplayName = "sum8", .IsDefaultEnabled = False, .Note = "Compatibility checksum; not cryptographic."},
        New HashOption With {.Id = "sum24", .DisplayName = "sum24", .IsDefaultEnabled = False, .Note = "Compatibility checksum; not cryptographic."},
        New HashOption With {.Id = "sum32", .DisplayName = "sum32", .IsDefaultEnabled = False, .Note = "Compatibility checksum; not cryptographic."},
        New HashOption With {.Id = "fletcher8", .DisplayName = "Fletcher-8", .IsDefaultEnabled = False, .Note = "Compatibility checksum; not cryptographic."},
        New HashOption With {.Id = "fletcher16", .DisplayName = "Fletcher-16", .IsDefaultEnabled = False, .Note = "Compatibility checksum; not cryptographic."},
        New HashOption With {.Id = "fletcher32", .DisplayName = "Fletcher-32", .IsDefaultEnabled = False, .Note = "Compatibility checksum; not cryptographic."},
        New HashOption With {.Id = "xor8", .DisplayName = "xor8", .IsDefaultEnabled = False, .Note = "Compatibility checksum; not cryptographic."},
        New HashOption With {.Id = "fnv1-32", .DisplayName = "FNV-1 32", .IsDefaultEnabled = False, .Note = "Compatibility hash; not cryptographic."},
        New HashOption With {.Id = "fnv1a-32", .DisplayName = "FNV-1a 32", .IsDefaultEnabled = False, .Note = "Compatibility hash; not cryptographic."},
        New HashOption With {.Id = "fnv1a-64", .DisplayName = "FNV-1a 64", .IsDefaultEnabled = False, .Note = "Compatibility hash; not cryptographic."},
        New HashOption With {.Id = "jenkins-one-at-a-time32", .DisplayName = "Jenkins one-at-a-time", .IsDefaultEnabled = False, .Note = "Compatibility hash; not cryptographic."},
        New HashOption With {.Id = "djb2-32", .DisplayName = "djb2 32", .IsDefaultEnabled = False, .Note = "Compatibility hash; not cryptographic."},
        New HashOption With {.Id = "sdbm-32", .DisplayName = "SDBM 32", .IsDefaultEnabled = False, .Note = "Compatibility hash; not cryptographic."},
        New HashOption With {.Id = "murmur3-32", .DisplayName = "Murmur3 32", .IsDefaultEnabled = False, .Note = "Compatibility hash; not cryptographic."},
        New HashOption With {.Id = "xxhash64", .DisplayName = "xxHash64", .IsDefaultEnabled = False, .Note = "Compatibility hash; not cryptographic."}
    }

    Public Shared ReadOnly Property DefaultActiveHashes As String
        Get
            Return String.Join(",", Options.Where(Function(optionItem) optionItem.IsDefaultEnabled).Select(Function(optionItem) optionItem.Id))
        End Get
    End Property

    Public Shared Function NormalizeActiveHashes(activeHashes As String) As String
        Dim active = ParseActiveHashIds(activeHashes).ToList()
        If active.Count = 0 Then
            active = ParseActiveHashIds(DefaultActiveHashes).ToList()
        End If

        Return String.Join(",", active)
    End Function

    Public Shared Function ParseActiveHashIds(activeHashes As String) As IReadOnlyList(Of String)
        Dim requested = If(activeHashes, "").
            Split(","c).
            Select(Function(value) value.Trim()).
            Where(Function(value) Not String.IsNullOrWhiteSpace(value)).
            ToList()
        Dim normalized As New List(Of String)

        For Each optionItem In Options
            If requested.Any(Function(value) String.Equals(value, optionItem.Id, StringComparison.OrdinalIgnoreCase) OrElse String.Equals(value, optionItem.DisplayName, StringComparison.OrdinalIgnoreCase)) Then
                normalized.Add(optionItem.Id)
            End If
        Next

        Return normalized
    End Function

    Public Shared Function IsActive(activeHashes As String, hashId As String) As Boolean
        Return ParseActiveHashIds(activeHashes).Contains(ResolveId(hashId), StringComparer.OrdinalIgnoreCase)
    End Function

    Public Shared Function GetHashValue(hashes As FileHashes, hashId As String) As String
        If hashes Is Nothing Then Return ""

        Select Case ResolveId(hashId)
            Case "SHA256"
                Return hashes.Sha256
            Case "BLAKE3"
                Return hashes.Blake3
            Case "KangarooTwelve"
                Return hashes.KangarooTwelve
            Case "SHA3-256"
                Return hashes.Sha3_256
            Case "MD5"
                Return hashes.Md5
            Case "Whirlpool"
                Return hashes.Whirlpool
            Case "Skein"
                Return hashes.Skein
            Case Else
                Return GetMappedHashValue(hashes.Hashes, hashId)
        End Select
    End Function

    Public Shared Function GetArtifactHashValue(artifact As ArtifactModel, hashId As String) As String
        If artifact Is Nothing Then Return ""

        Select Case ResolveId(hashId)
            Case "SHA256"
                Return artifact.Sha256
            Case "BLAKE3"
                Return artifact.Blake3
            Case "KangarooTwelve"
                Return artifact.KangarooTwelve
            Case "SHA3-256"
                Return artifact.Sha3_256
            Case "MD5"
                Return artifact.Md5
            Case "Whirlpool"
                Return artifact.Whirlpool
            Case "Skein"
                Return artifact.Skein
            Case Else
                Return GetMappedHashValue(artifact.Hashes, hashId)
        End Select
    End Function

    Public Shared Sub SetHashValue(hashes As FileHashes, hashId As String, value As String)
        If hashes Is Nothing Then Return

        Select Case ResolveId(hashId)
            Case "SHA256"
                hashes.Sha256 = If(value, "")
            Case "BLAKE3"
                hashes.Blake3 = If(value, "")
            Case "KangarooTwelve"
                hashes.KangarooTwelve = If(value, "")
            Case "SHA3-256"
                hashes.Sha3_256 = If(value, "")
            Case "MD5"
                hashes.Md5 = If(value, "")
            Case "Whirlpool"
                hashes.Whirlpool = If(value, "")
            Case "Skein"
                hashes.Skein = If(value, "")
            Case Else
                SetMappedHashValue(hashes.Hashes, hashId, value)
        End Select
    End Sub

    Public Shared Sub SetArtifactHashValue(artifact As ArtifactModel, hashId As String, value As String)
        If artifact Is Nothing Then Return

        Select Case ResolveId(hashId)
            Case "SHA256"
                artifact.Sha256 = If(value, "")
            Case "BLAKE3"
                artifact.Blake3 = If(value, "")
            Case "KangarooTwelve"
                artifact.KangarooTwelve = If(value, "")
            Case "SHA3-256"
                artifact.Sha3_256 = If(value, "")
            Case "MD5"
                artifact.Md5 = If(value, "")
            Case "Whirlpool"
                artifact.Whirlpool = If(value, "")
            Case "Skein"
                artifact.Skein = If(value, "")
            Case Else
                SetMappedHashValue(artifact.Hashes, hashId, value)
        End Select
    End Sub

    Public Shared Sub ApplyHashesToArtifact(artifact As ArtifactModel, hashes As FileHashes)
        If artifact Is Nothing OrElse hashes Is Nothing Then Return

        For Each optionItem In Options
            Dim value = GetHashValue(hashes, optionItem.Id)
            If Not String.IsNullOrWhiteSpace(value) Then
                SetArtifactHashValue(artifact, optionItem.Id, value)
            End If
        Next

        artifact.HashStatus = "Verified"
    End Sub

    Private Shared Function GetMappedHashValue(hashes As Dictionary(Of String, String), hashId As String) As String
        Dim resolved = ResolveId(hashId)
        If hashes Is Nothing OrElse String.IsNullOrWhiteSpace(resolved) Then Return ""

        Dim value = ""
        Return If(hashes.TryGetValue(resolved, value), value, "")
    End Function

    Private Shared Sub SetMappedHashValue(hashes As Dictionary(Of String, String), hashId As String, value As String)
        Dim resolved = ResolveId(hashId)
        If hashes Is Nothing OrElse String.IsNullOrWhiteSpace(resolved) Then Return

        If String.IsNullOrWhiteSpace(value) Then
            hashes.Remove(resolved)
        Else
            hashes(resolved) = value
        End If
    End Sub

    Private Shared Function ResolveId(hashId As String) As String
        Dim optionItem = Options.FirstOrDefault(Function(item) String.Equals(item.Id, hashId, StringComparison.OrdinalIgnoreCase) OrElse String.Equals(item.DisplayName, hashId, StringComparison.OrdinalIgnoreCase))
        Return If(optionItem?.Id, "")
    End Function
End Class

Public Class HashService
    Private Const BufferSize As Integer = 8192
    Private Const Mask32 As ULong = &HFFFFFFFFUL
    Private Shared ReadOnly Mask64Big As BigInteger = (BigInteger.One << 64) - BigInteger.One
    Private Const Crc64EcmaPolynomial As ULong = &H42F0E1EBA9EA3693UL
    Private Shared ReadOnly Crc32Table As UInteger() = BuildReflectedCrc32Table()
    Private Shared ReadOnly Crc64EcmaTable As ULong() = BuildCrc64EcmaTable()

    Public Function ComputeHashes(path As String, Optional activeHashes As String = "") As FileHashes
        Dim hashes As New FileHashes()
        Dim activeList = HashRegistry.ParseActiveHashIds(HashRegistry.NormalizeActiveHashes(activeHashes))

        For Each hashId In activeList
            HashRegistry.SetHashValue(hashes, hashId, ComputeHash(path, hashId))
        Next

        Return hashes
    End Function

    Public Function ComputeBlake3(path As String) As String
        Using stream = File.OpenRead(path)
            Using blakeStream As New Blake3.Blake3Stream(stream, dispose:=False)
                blakeStream.CopyTo(System.IO.Stream.Null)
                Return blakeStream.ComputeHash().ToString()
            End Using
        End Using
    End Function

    Public Function ComputeSha256(path As String) As String
        Using stream = File.OpenRead(path)
            Dim hash = SHA256.HashData(stream)
            Return Convert.ToHexString(hash).ToLowerInvariant()
        End Using
    End Function

    Public Function ComputeKangarooTwelve(path As String) As String
        Using hasher As New Global.StreamHash.Core.KangarooTwelve(32, Array.Empty(Of Byte)())
            ReadChunks(path, Sub(buffer, count) hasher.Update(buffer, 0, count))
            Return hasher.FinalizeHex()
        End Using
    End Function

    Public Function ComputeSha3_256(path As String) As String
        Return ComputeBouncyDigest(path, New Sha3Digest(256))
    End Function

    Public Function ComputeMd5(path As String) As String
        Using stream = File.OpenRead(path)
            Dim hash = MD5.HashData(stream)
            Return Convert.ToHexString(hash).ToLowerInvariant()
        End Using
    End Function

    Public Function ComputeWhirlpool(path As String) As String
        Return ComputeBouncyDigest(path, New WhirlpoolDigest())
    End Function

    Public Function ComputeSkein(path As String) As String
        Return ComputeBouncyDigest(path, New SkeinDigest(512, 512))
    End Function

    Private Function ComputeHash(path As String, hashId As String) As String
        Select Case hashId
            Case "SHA256"
                Return ComputeSha256(path)
            Case "BLAKE3"
                Return ComputeBlake3(path)
            Case "KangarooTwelve"
                Return ComputeKangarooTwelve(path)
            Case "SHA3-256"
                Return ComputeSha3_256(path)
            Case "MD5"
                Return ComputeMd5(path)
            Case "Whirlpool"
                Return ComputeWhirlpool(path)
            Case "Skein"
                Return ComputeSkein(path)
            Case "cksum-posix"
                Return ComputePosixCksum(path)
            Case "crc8"
                Return ComputeCrc8(path)
            Case "crc16"
                Return ComputeCrc16Arc(path)
            Case "crc32"
                Return ComputeCrc32(path)
            Case "crc64"
                Return ComputeCrc64Ecma(path)
            Case "adler32"
                Return ComputeAdler32(path)
            Case "bsd-sum16"
                Return ComputeBsdSum16(path)
            Case "sysv-sum16"
                Return ComputeSysvSum16(path)
            Case "internet-checksum16"
                Return ComputeInternetChecksum16(path)
            Case "sum8"
                Return ComputeSimpleSum(path, 8)
            Case "sum24"
                Return ComputeSimpleSum(path, 24)
            Case "sum32"
                Return ComputeSimpleSum(path, 32)
            Case "fletcher8"
                Return ComputeFletcher8(path)
            Case "fletcher16"
                Return ComputeFletcher16(path)
            Case "fletcher32"
                Return ComputeFletcher32(path)
            Case "xor8"
                Return ComputeXor8(path)
            Case "fnv1-32"
                Return ComputeFnv132(path)
            Case "fnv1a-32"
                Return ComputeFnv1a32(path)
            Case "fnv1a-64"
                Return ComputeFnv1a64(path)
            Case "jenkins-one-at-a-time32"
                Return ComputeJenkinsOneAtATime32(path)
            Case "djb2-32"
                Return ComputeDjb232(path)
            Case "sdbm-32"
                Return ComputeSdbm32(path)
            Case "murmur3-32"
                Return ComputeMurmur3_32(path)
            Case "xxhash64"
                Return ComputeXxHash64(path)
            Case Else
                Return ""
        End Select
    End Function

    Private Shared Function ComputeBouncyDigest(path As String, digest As Org.BouncyCastle.Crypto.IDigest) As String
        ReadChunks(path, Sub(buffer, count) digest.BlockUpdate(buffer, 0, count))
        Dim output As Byte() = New Byte(digest.GetDigestSize() - 1) {}
        digest.DoFinal(output, 0)
        Return Convert.ToHexString(output).ToLowerInvariant()
    End Function

    Private Shared Function ComputePosixCksum(path As String) As String
        Dim crc As UInteger = 0UI
        Dim totalLength As ULong = 0UL

        ReadChunks(path,
            Sub(buffer, count)
                For index = 0 To count - 1
                    crc = PosixCrcUpdate(crc, buffer(index))
                Next
                totalLength += CULng(count)
            End Sub)

        Dim lengthValue = totalLength
        While lengthValue > 0UL
            crc = PosixCrcUpdate(crc, CByte(lengthValue And &HFFUL))
            lengthValue >>= 8
        End While

        Return (Not crc).ToString("x8")
    End Function

    Private Shared Function ComputeCrc8(path As String) As String
        Dim crc As Integer = 0
        ReadChunks(path,
            Sub(buffer, count)
                For index = 0 To count - 1
                    crc = crc Xor buffer(index)
                    For bit = 0 To 7
                        If (crc And &H80) <> 0 Then
                            crc = ((crc << 1) Xor &H07) And &HFF
                        Else
                            crc = (crc << 1) And &HFF
                        End If
                    Next
                Next
            End Sub)
        Return crc.ToString("x2")
    End Function

    Private Shared Function ComputeCrc16Arc(path As String) As String
        Dim crc As UInteger = 0UI
        ReadChunks(path,
            Sub(buffer, count)
                For index = 0 To count - 1
                    crc = crc Xor buffer(index)
                    For bit = 0 To 7
                        If (crc And 1UI) <> 0UI Then
                            crc = (crc >> 1) Xor &HA001UI
                        Else
                            crc >>= 1
                        End If
                    Next
                Next
            End Sub)
        Return (crc And &HFFFFUI).ToString("x4")
    End Function

    Private Shared Function ComputeCrc32(path As String) As String
        Dim crc As UInteger = &HFFFFFFFFUI
        ReadChunks(path,
            Sub(buffer, count)
                For index = 0 To count - 1
                    crc = (crc >> 8) Xor Crc32Table(CInt((crc Xor buffer(index)) And &HFFUI))
                Next
            End Sub)
        Return (Not crc).ToString("x8")
    End Function

    Private Shared Function ComputeCrc64Ecma(path As String) As String
        Dim crc As ULong = 0UL
        ReadChunks(path,
            Sub(buffer, count)
                For index = 0 To count - 1
                    crc = (crc << 8) Xor Crc64EcmaTable(CInt(((crc >> 56) Xor buffer(index)) And &HFFUL))
                Next
            End Sub)
        Return crc.ToString("x16")
    End Function

    Private Shared Function ComputeAdler32(path As String) As String
        Const ModAdler As UInteger = 65521UI
        Dim a As UInteger = 1UI
        Dim b As UInteger = 0UI
        ReadChunks(path,
            Sub(buffer, count)
                For index = 0 To count - 1
                    a = (a + buffer(index)) Mod ModAdler
                    b = (b + a) Mod ModAdler
                Next
            End Sub)
        Return ((b << 16) Or a).ToString("x8")
    End Function

    Private Shared Function ComputeBsdSum16(path As String) As String
        Dim sum As UInteger = 0UI
        ReadChunks(path,
            Sub(buffer, count)
                For index = 0 To count - 1
                    sum = ((sum >> 1) Or ((sum And 1UI) << 15)) And &HFFFFUI
                    sum = (sum + buffer(index)) And &HFFFFUI
                Next
            End Sub)
        Return sum.ToString("x4")
    End Function

    Private Shared Function ComputeSysvSum16(path As String) As String
        Dim sum As UInteger = 0UI
        ReadChunks(path,
            Sub(buffer, count)
                For index = 0 To count - 1
                    sum += buffer(index)
                Next
            End Sub)
        sum = (sum And &HFFFFUI) + (sum >> 16)
        sum = (sum And &HFFFFUI) + (sum >> 16)
        Return (sum And &HFFFFUI).ToString("x4")
    End Function

    Private Shared Function ComputeInternetChecksum16(path As String) As String
        Dim sum As UInteger = 0UI
        Dim hasHighByte = False
        Dim highByte As Byte = 0

        ReadChunks(path,
            Sub(buffer, count)
                For index = 0 To count - 1
                    If Not hasHighByte Then
                        highByte = buffer(index)
                        hasHighByte = True
                    Else
                        sum += (CUInt(highByte) << 8) Or buffer(index)
                        sum = (sum And &HFFFFUI) + (sum >> 16)
                        hasHighByte = False
                    End If
                Next
            End Sub)

        If hasHighByte Then
            sum += CUInt(highByte) << 8
        End If

        While (sum >> 16) <> 0UI
            sum = (sum And &HFFFFUI) + (sum >> 16)
        End While

        Return ((Not sum) And &HFFFFUI).ToString("x4")
    End Function

    Private Shared Function ComputeSimpleSum(path As String, bits As Integer) As String
        Dim mask = If(bits = 32, Mask32, (1UL << bits) - 1UL)
        Dim sum As ULong = 0UL
        ReadChunks(path,
            Sub(buffer, count)
                For index = 0 To count - 1
                    sum = (sum + buffer(index)) And mask
                Next
            End Sub)
        Return sum.ToString("x" & (bits \ 4).ToString())
    End Function

    Private Shared Function ComputeFletcher8(path As String) As String
        Dim sum1 As Integer = 0
        Dim sum2 As Integer = 0
        ReadChunks(path,
            Sub(buffer, count)
                For index = 0 To count - 1
                    sum1 = (sum1 + (buffer(index) And &HF)) Mod 15
                    sum2 = (sum2 + sum1) Mod 15
                    sum1 = (sum1 + (buffer(index) >> 4)) Mod 15
                    sum2 = (sum2 + sum1) Mod 15
                Next
            End Sub)
        Return ((sum2 << 4) Or sum1).ToString("x2")
    End Function

    Private Shared Function ComputeFletcher16(path As String) As String
        Dim sum1 As Integer = 0
        Dim sum2 As Integer = 0
        ReadChunks(path,
            Sub(buffer, count)
                For index = 0 To count - 1
                    sum1 = (sum1 + buffer(index)) Mod 255
                    sum2 = (sum2 + sum1) Mod 255
                Next
            End Sub)
        Return ((sum2 << 8) Or sum1).ToString("x4")
    End Function

    Private Shared Function ComputeFletcher32(path As String) As String
        Dim sum1 As UInteger = 0UI
        Dim sum2 As UInteger = 0UI
        Dim hasHighByte = False
        Dim highByte As Byte = 0

        ReadChunks(path,
            Sub(buffer, count)
                For index = 0 To count - 1
                    If Not hasHighByte Then
                        highByte = buffer(index)
                        hasHighByte = True
                    Else
                        Dim word = (CUInt(highByte) << 8) Or buffer(index)
                        sum1 = (sum1 + word) Mod 65535UI
                        sum2 = (sum2 + sum1) Mod 65535UI
                        hasHighByte = False
                    End If
                Next
            End Sub)

        If hasHighByte Then
            Dim word = CUInt(highByte) << 8
            sum1 = (sum1 + word) Mod 65535UI
            sum2 = (sum2 + sum1) Mod 65535UI
        End If

        Return ((sum2 << 16) Or sum1).ToString("x8")
    End Function

    Private Shared Function ComputeXor8(path As String) As String
        Dim value As Byte = 0
        ReadChunks(path,
            Sub(buffer, count)
                For index = 0 To count - 1
                    value = value Xor buffer(index)
                Next
            End Sub)
        Return value.ToString("x2")
    End Function

    Private Shared Function ComputeFnv132(path As String) As String
        Dim hash As ULong = &H811C9DC5UL
        ReadChunks(path,
            Sub(buffer, count)
                For index = 0 To count - 1
                    hash = (hash * &H1000193UL) And Mask32
                    hash = hash Xor buffer(index)
                Next
            End Sub)
        Return hash.ToString("x8")
    End Function

    Private Shared Function ComputeFnv1a32(path As String) As String
        Dim hash As ULong = &H811C9DC5UL
        ReadChunks(path,
            Sub(buffer, count)
                For index = 0 To count - 1
                    hash = hash Xor buffer(index)
                    hash = (hash * &H1000193UL) And Mask32
                Next
            End Sub)
        Return hash.ToString("x8")
    End Function

    Private Shared Function ComputeFnv1a64(path As String) As String
        Dim hash As BigInteger = &HCBF29CE484222325UL
        Dim prime As BigInteger = &H100000001B3UL
        ReadChunks(path,
            Sub(buffer, count)
                For index = 0 To count - 1
                    hash = hash Xor buffer(index)
                    hash = (hash * prime) And Mask64Big
                Next
            End Sub)
        Return CULng(hash).ToString("x16")
    End Function

    Private Shared Function ComputeJenkinsOneAtATime32(path As String) As String
        Dim hash As ULong = 0UL
        ReadChunks(path,
            Sub(buffer, count)
                For index = 0 To count - 1
                    hash = (hash + buffer(index)) And Mask32
                    hash = (hash + ((hash << 10) And Mask32)) And Mask32
                    hash = (hash Xor (hash >> 6)) And Mask32
                Next
            End Sub)
        hash = (hash + ((hash << 3) And Mask32)) And Mask32
        hash = (hash Xor (hash >> 11)) And Mask32
        hash = (hash + ((hash << 15) And Mask32)) And Mask32
        Return hash.ToString("x8")
    End Function

    Private Shared Function ComputeDjb232(path As String) As String
        Dim hash As ULong = 5381UL
        ReadChunks(path,
            Sub(buffer, count)
                For index = 0 To count - 1
                    hash = (((hash << 5) + hash) + buffer(index)) And Mask32
                Next
            End Sub)
        Return hash.ToString("x8")
    End Function

    Private Shared Function ComputeSdbm32(path As String) As String
        Dim hash As ULong = 0UL
        ReadChunks(path,
            Sub(buffer, count)
                For index = 0 To count - 1
                    hash = (buffer(index) + ((hash << 6) And Mask32) + ((hash << 16) And Mask32) - hash) And Mask32
                Next
            End Sub)
        Return hash.ToString("x8")
    End Function

    Private Shared Function ComputeMurmur3_32(path As String) As String
        Dim data = File.ReadAllBytes(path)
        Const c1 As UInteger = &HCC9E2D51UI
        Const c2 As UInteger = &H1B873593UI
        Dim hash As UInteger = 0UI
        Dim offset = 0

        While offset + 4 <= data.Length
            Dim k = CUInt(data(offset)) Or (CUInt(data(offset + 1)) << 8) Or (CUInt(data(offset + 2)) << 16) Or (CUInt(data(offset + 3)) << 24)
            k = Mul32(k, c1)
            k = RotateLeft32(k, 15)
            k = Mul32(k, c2)
            hash = hash Xor k
            hash = RotateLeft32(hash, 13)
            hash = Add32(Mul32(hash, 5UI), &HE6546B64UI)
            offset += 4
        End While

        Dim tail As UInteger = 0UI
        Dim remaining = data.Length And 3
        If remaining = 3 Then tail = tail Xor (CUInt(data(offset + 2)) << 16)
        If remaining >= 2 Then tail = tail Xor (CUInt(data(offset + 1)) << 8)
        If remaining >= 1 Then
            tail = tail Xor data(offset)
            tail = Mul32(tail, c1)
            tail = RotateLeft32(tail, 15)
            tail = Mul32(tail, c2)
            hash = hash Xor tail
        End If

        hash = hash Xor CUInt(data.Length)
        hash = Fmix32(hash)
        Return hash.ToString("x8")
    End Function

    Private Shared Function ComputeXxHash64(path As String) As String
        Dim data = File.ReadAllBytes(path)
        Return Convert.ToHexString(System.IO.Hashing.XxHash64.Hash(data)).ToLowerInvariant()
    End Function

    Private Shared Sub ReadChunks(path As String, onChunk As Action(Of Byte(), Integer))
        Using stream = File.OpenRead(path)
            Dim buffer As Byte() = New Byte(BufferSize - 1) {}
            While True
                Dim bytesRead = stream.Read(buffer, 0, buffer.Length)
                If bytesRead <= 0 Then Exit While
                onChunk(buffer, bytesRead)
            End While
        End Using
    End Sub

    Private Shared Function PosixCrcUpdate(crc As UInteger, value As Byte) As UInteger
        crc = crc Xor (CUInt(value) << 24)
        For bit = 0 To 7
            If (crc And &H80000000UI) <> 0UI Then
                crc = (crc << 1) Xor &H4C11DB7UI
            Else
                crc <<= 1
            End If
        Next
        Return crc
    End Function

    Private Shared Function BuildReflectedCrc32Table() As UInteger()
        Dim table(255) As UInteger
        For i = 0 To 255
            Dim value = CUInt(i)
            For bit = 0 To 7
                If (value And 1UI) <> 0UI Then
                    value = (value >> 1) Xor &HEDB88320UI
                Else
                    value >>= 1
                End If
            Next
            table(i) = value
        Next
        Return table
    End Function

    Private Shared Function BuildCrc64EcmaTable() As ULong()
        Dim table(255) As ULong
        For i = 0 To 255
            Dim value = CULng(i) << 56
            For bit = 0 To 7
                If (value And &H8000000000000000UL) <> 0UL Then
                    value = (value << 1) Xor Crc64EcmaPolynomial
                Else
                    value <<= 1
                End If
            Next
            table(i) = value
        Next
        Return table
    End Function

    Private Shared Function RotateLeft32(value As UInteger, bits As Integer) As UInteger
        Return (value << bits) Or (value >> (32 - bits))
    End Function

    Private Shared Function Add32(left As UInteger, right As UInteger) As UInteger
        Return CUInt((CULng(left) + CULng(right)) And Mask32)
    End Function

    Private Shared Function Mul32(left As UInteger, right As UInteger) As UInteger
        Return CUInt((CULng(left) * CULng(right)) And Mask32)
    End Function

    Private Shared Function Fmix32(hash As UInteger) As UInteger
        hash = hash Xor (hash >> 16)
        hash = Mul32(hash, &H85EBCA6BUI)
        hash = hash Xor (hash >> 13)
        hash = Mul32(hash, &HC2B2AE35UI)
        hash = hash Xor (hash >> 16)
        Return hash
    End Function
End Class
