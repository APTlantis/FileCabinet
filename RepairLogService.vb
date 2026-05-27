Imports System.IO
Imports System.Linq
Imports System.Text.Json

Public Class RepairLogService
    Private Shared ReadOnly LogOptions As New JsonSerializerOptions With {
        .WriteIndented = False,
        .PropertyNameCaseInsensitive = True
    }

    Public Const RelativeLogPath As String = "catalog\repair-log.jsonl"

    Public Function Append(vaultRootPath As String, entry As RepairLogEntry) As String
        If String.IsNullOrWhiteSpace(vaultRootPath) Then
            Throw New ArgumentException("Vault root path is required.", NameOf(vaultRootPath))
        End If

        If entry Is Nothing Then
            Throw New ArgumentNullException(NameOf(entry))
        End If

        If String.IsNullOrWhiteSpace(entry.Timestamp) Then
            entry.Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        End If

        Dim logPath = ResolveLogPath(vaultRootPath)
        Dim logDirectory = Path.GetDirectoryName(logPath)
        If Not String.IsNullOrWhiteSpace(logDirectory) Then
            Directory.CreateDirectory(logDirectory)
        End If

        File.AppendAllText(logPath, JsonSerializer.Serialize(entry, LogOptions) & Environment.NewLine)
        Return logPath
    End Function

    Public Function ReadRecent(vaultRootPath As String, Optional count As Integer = 20) As List(Of RepairLogEntry)
        Dim logPath = ResolveLogPath(vaultRootPath)
        If String.IsNullOrWhiteSpace(logPath) OrElse Not File.Exists(logPath) Then
            Return New List(Of RepairLogEntry)()
        End If

        Return File.ReadLines(logPath).
            Reverse().
            Take(Math.Max(0, count)).
            Select(Function(line) DeserializeLine(line)).
            Where(Function(entry) entry IsNot Nothing).
            ToList()
    End Function

    Public Function ResolveLogPath(vaultRootPath As String) As String
        If String.IsNullOrWhiteSpace(vaultRootPath) Then
            Return ""
        End If

        Return Path.Combine(vaultRootPath, RelativeLogPath)
    End Function

    Private Shared Function DeserializeLine(line As String) As RepairLogEntry
        If String.IsNullOrWhiteSpace(line) Then
            Return Nothing
        End If

        Try
            Return JsonSerializer.Deserialize(Of RepairLogEntry)(line, LogOptions)
        Catch
            Return Nothing
        End Try
    End Function
End Class
