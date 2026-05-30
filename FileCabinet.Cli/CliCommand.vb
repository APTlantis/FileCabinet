Imports FileCabinet

Public Class CliCommand
    Public Property CommandName As String = ""
    Public Property Paths As New List(Of String)
    Public Property Query As String = ""
    Public Property CatalogPath As String = ""
    Public Property VaultRootPath As String = ""
    Public Property OutputPath As String = ""
    Public Property Format As String = "text"
    Public Property Scope As String = "All"
    Public Property Category As String = ""
    Public Property Tag As String = ""
    Public Property Limit As Integer = 50
    Public Property FailOn As String = "any"
    Public Property Json As Boolean
    Public Property Quiet As Boolean
    Public Property Help As Boolean
    Public Property Version As Boolean
    Public Property Apply As Boolean
    Public Property Yes As Boolean
    Public Property Zip As Boolean
    Public Property Mode As IngestMode?
    Public Property Errors As New List(Of String)

    Public ReadOnly Property IsValid As Boolean
        Get
            Return Errors.Count = 0
        End Get
    End Property
End Class
