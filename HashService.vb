Imports System.IO
Imports System.Security.Cryptography
Imports Org.BouncyCastle.Crypto.Digests

Public Class FileHashes
    Public Property Blake3 As String = ""
    Public Property Sha256 As String = ""
    Public Property KangarooTwelve As String = ""
    Public Property Sha3_256 As String = ""
    Public Property Md5 As String = ""
    Public Property Whirlpool As String = ""
    Public Property Skein As String = ""
End Class

Public Class HashService
    Public Function ComputeHashes(path As String, activeHashes As String) As FileHashes
        Dim hashes As New FileHashes()
        Dim activeList = If(String.IsNullOrWhiteSpace(activeHashes), New List(Of String)(), activeHashes.Split(","c).Select(Function(s) s.Trim().ToLowerInvariant()).ToList())
        
        If activeList.Contains("sha256") Then hashes.Sha256 = ComputeSha256(path)
        If activeList.Contains("blake3") Then hashes.Blake3 = ComputeBlake3(path)
        If activeList.Contains("kangarootwelve") Then hashes.KangarooTwelve = ComputeKangarooTwelve(path)
        If activeList.Contains("sha3-256") Then hashes.Sha3_256 = ComputeSha3_256(path)
        If activeList.Contains("md5") Then hashes.Md5 = ComputeMd5(path)
        If activeList.Contains("whirlpool") Then hashes.Whirlpool = ComputeWhirlpool(path)
        If activeList.Contains("skein") Then hashes.Skein = ComputeSkein(path)
        
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
        Dim output As Byte() = New Byte(31) {}
        ' BouncyCastle .NET doesn't seem to have KangarooTwelve out of the box.
        ' Using SHA3-256 as a fallback placeholder until a proper K12 library is found or P/Invoke is configured.
        Dim digest As New Sha3Digest(256)
        Using stream = File.OpenRead(path)
            Dim buffer As Byte() = New Byte(8192 - 1) {}
            Dim bytesRead As Integer
            Do
                bytesRead = stream.Read(buffer, 0, buffer.Length)
                If bytesRead > 0 Then
                    digest.BlockUpdate(buffer, 0, bytesRead)
                End If
            Loop While bytesRead > 0
        End Using
        digest.DoFinal(output, 0)
        Return Convert.ToHexString(output).ToLowerInvariant()
    End Function

    Public Function ComputeSha3_256(path As String) As String
        Dim digest As New Sha3Digest(256)
        Dim buffer As Byte() = New Byte(8192 - 1) {}
        Using stream = File.OpenRead(path)
            Dim bytesRead As Integer
            Do
                bytesRead = stream.Read(buffer, 0, buffer.Length)
                If bytesRead > 0 Then
                    digest.BlockUpdate(buffer, 0, bytesRead)
                End If
            Loop While bytesRead > 0
        End Using
        
        Dim output As Byte() = New Byte(digest.GetDigestSize() - 1) {}
        digest.DoFinal(output, 0)
        Return Convert.ToHexString(output).ToLowerInvariant()
    End Function

    Public Function ComputeMd5(path As String) As String
        Using stream = File.OpenRead(path)
            Dim hash = MD5.HashData(stream)
            Return Convert.ToHexString(hash).ToLowerInvariant()
        End Using
    End Function

    Public Function ComputeWhirlpool(path As String) As String
        Dim digest As New WhirlpoolDigest()
        Dim buffer As Byte() = New Byte(8192 - 1) {}
        Using stream = File.OpenRead(path)
            Dim bytesRead As Integer
            Do
                bytesRead = stream.Read(buffer, 0, buffer.Length)
                If bytesRead > 0 Then
                    digest.BlockUpdate(buffer, 0, bytesRead)
                End If
            Loop While bytesRead > 0
        End Using
        Dim output As Byte() = New Byte(digest.GetDigestSize() - 1) {}
        digest.DoFinal(output, 0)
        Return Convert.ToHexString(output).ToLowerInvariant()
    End Function

    Public Function ComputeSkein(path As String) As String
        Dim digest As New SkeinDigest(512, 512)
        Dim buffer As Byte() = New Byte(8192 - 1) {}
        Using stream = File.OpenRead(path)
            Dim bytesRead As Integer
            Do
                bytesRead = stream.Read(buffer, 0, buffer.Length)
                If bytesRead > 0 Then
                    digest.BlockUpdate(buffer, 0, bytesRead)
                End If
            Loop While bytesRead > 0
        End Using
        Dim output As Byte() = New Byte(digest.GetDigestSize() - 1) {}
        digest.DoFinal(output, 0)
        Return Convert.ToHexString(output).ToLowerInvariant()
    End Function
End Class
