Imports System.IO
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
    Public Property Blake3 As String = ""
    Public Property Sha256 As String = ""
    Public Property KangarooTwelve As String = ""
    Public Property Sha3_256 As String = ""
    Public Property Md5 As String = ""
    Public Property Whirlpool As String = ""
    Public Property Skein As String = ""
End Class

Public Class HashRegistry
    Public Shared ReadOnly Property Options As IReadOnlyList(Of HashOption) = New List(Of HashOption) From {
        New HashOption With {.Id = "SHA256", .DisplayName = "SHA-256", .CatalogPropertyName = NameOf(FileHashes.Sha256), .IsDefaultEnabled = True},
        New HashOption With {.Id = "BLAKE3", .DisplayName = "BLAKE3", .CatalogPropertyName = NameOf(FileHashes.Blake3), .IsDefaultEnabled = True},
        New HashOption With {.Id = "KangarooTwelve", .DisplayName = "KangarooTwelve", .CatalogPropertyName = NameOf(FileHashes.KangarooTwelve), .IsDefaultEnabled = True},
        New HashOption With {.Id = "SHA3-256", .DisplayName = "SHA3-256", .CatalogPropertyName = NameOf(FileHashes.Sha3_256), .IsDefaultEnabled = False},
        New HashOption With {.Id = "MD5", .DisplayName = "MD5", .CatalogPropertyName = NameOf(FileHashes.Md5), .IsDefaultEnabled = False, .Note = "Legacy compatibility only; not collision-resistant."},
        New HashOption With {.Id = "Whirlpool", .DisplayName = "Whirlpool", .CatalogPropertyName = NameOf(FileHashes.Whirlpool), .IsDefaultEnabled = False},
        New HashOption With {.Id = "Skein", .DisplayName = "Skein-512", .CatalogPropertyName = NameOf(FileHashes.Skein), .IsDefaultEnabled = False}
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
        Return ParseActiveHashIds(activeHashes).Contains(hashId, StringComparer.OrdinalIgnoreCase)
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
                Return ""
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
                Return ""
        End Select
    End Function

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
        End Select
    End Sub

    Public Shared Sub ApplyHashesToArtifact(artifact As ArtifactModel, hashes As FileHashes)
        If artifact Is Nothing OrElse hashes Is Nothing Then Return

        If Not String.IsNullOrWhiteSpace(hashes.Sha256) Then artifact.Sha256 = hashes.Sha256
        If Not String.IsNullOrWhiteSpace(hashes.Blake3) Then artifact.Blake3 = hashes.Blake3
        If Not String.IsNullOrWhiteSpace(hashes.KangarooTwelve) Then artifact.KangarooTwelve = hashes.KangarooTwelve
        If Not String.IsNullOrWhiteSpace(hashes.Sha3_256) Then artifact.Sha3_256 = hashes.Sha3_256
        If Not String.IsNullOrWhiteSpace(hashes.Md5) Then artifact.Md5 = hashes.Md5
        If Not String.IsNullOrWhiteSpace(hashes.Whirlpool) Then artifact.Whirlpool = hashes.Whirlpool
        If Not String.IsNullOrWhiteSpace(hashes.Skein) Then artifact.Skein = hashes.Skein
        artifact.HashStatus = "Verified"
    End Sub

    Private Shared Function ResolveId(hashId As String) As String
        Dim optionItem = Options.FirstOrDefault(Function(item) String.Equals(item.Id, hashId, StringComparison.OrdinalIgnoreCase) OrElse String.Equals(item.DisplayName, hashId, StringComparison.OrdinalIgnoreCase))
        Return If(optionItem?.Id, "")
    End Function
End Class

Public Class HashService
    Public Function ComputeHashes(path As String, Optional activeHashes As String = "") As FileHashes
        Dim hashes As New FileHashes()
        Dim activeList = HashRegistry.ParseActiveHashIds(HashRegistry.NormalizeActiveHashes(activeHashes))
        
        If activeList.Contains("SHA256", StringComparer.OrdinalIgnoreCase) Then hashes.Sha256 = ComputeSha256(path)
        If activeList.Contains("BLAKE3", StringComparer.OrdinalIgnoreCase) Then hashes.Blake3 = ComputeBlake3(path)
        If activeList.Contains("KangarooTwelve", StringComparer.OrdinalIgnoreCase) Then hashes.KangarooTwelve = ComputeKangarooTwelve(path)
        If activeList.Contains("SHA3-256", StringComparer.OrdinalIgnoreCase) Then hashes.Sha3_256 = ComputeSha3_256(path)
        If activeList.Contains("MD5", StringComparer.OrdinalIgnoreCase) Then hashes.Md5 = ComputeMd5(path)
        If activeList.Contains("Whirlpool", StringComparer.OrdinalIgnoreCase) Then hashes.Whirlpool = ComputeWhirlpool(path)
        If activeList.Contains("Skein", StringComparer.OrdinalIgnoreCase) Then hashes.Skein = ComputeSkein(path)
        
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
            Using stream = File.OpenRead(path)
                Dim buffer As Byte() = New Byte(8192 - 1) {}
                Dim bytesRead As Integer
                Do
                    bytesRead = stream.Read(buffer, 0, buffer.Length)
                    If bytesRead > 0 Then
                        hasher.Update(buffer, 0, bytesRead)
                    End If
                Loop While bytesRead > 0
            End Using
            Return hasher.FinalizeHex()
        End Using
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
