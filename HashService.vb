Imports System.IO
Imports System.Security.Cryptography

Public Class FileHashes
    Public Property Blake3 As String = ""
    Public Property Sha256 As String = ""
End Class

Public Class HashService
    Public Function ComputeHashes(path As String) As FileHashes
        Return New FileHashes With {
            .Blake3 = ComputeBlake3(path),
            .Sha256 = ComputeSha256(path)
        }
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
End Class
