Public Class ShellIngestRequest
    Public Property Mode As IngestMode?
    Public Property Paths As New List(Of String)

    Public ReadOnly Property HasPaths As Boolean
        Get
            Return Paths IsNot Nothing AndAlso Paths.Count > 0
        End Get
    End Property

    Public Shared Function Parse(args As IEnumerable(Of String)) As ShellIngestRequest
        Dim request As New ShellIngestRequest()

        If args Is Nothing Then
            Return request
        End If

        For Each arg In args
            If String.IsNullOrWhiteSpace(arg) Then
                Continue For
            End If

            Select Case arg.Trim().ToLowerInvariant()
                Case "--copy", "/copy", "-copy"
                    request.Mode = IngestMode.Copy
                Case "--move", "/move", "-move"
                    request.Mode = IngestMode.Move
                Case Else
                    request.Paths.Add(arg)
            End Select
        Next

        Return request
    End Function
End Class
