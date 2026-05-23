Public Class IngestionProgress
    Public Property Status As String = ""
    Public Property CurrentFile As String = ""
    Public Property CurrentStage As String = ""
    Public Property FilesCompleted As Integer
    Public Property FilesTotal As Integer
    Public Property BytesCompleted As Long
    Public Property BytesTotal As Long

    Public ReadOnly Property Percent As Double
        Get
            If BytesTotal <= 0 Then
                Return 0
            End If

            Return Math.Max(0, Math.Min(100, (CDbl(BytesCompleted) / CDbl(BytesTotal)) * 100))
        End Get
    End Property

    Public ReadOnly Property Summary As String
        Get
            If FilesTotal <= 0 Then
                Return Status
            End If

            Return $"{Status}  •  {FilesCompleted}/{FilesTotal} files  •  {Percent:0}%"
        End Get
    End Property
End Class
