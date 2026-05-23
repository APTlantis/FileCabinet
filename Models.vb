Imports System.Text.Json.Serialization
Imports System.ComponentModel
Imports System.Runtime.CompilerServices

Public Class CatalogData
    Public Property SchemaVersion As Integer = 1
    Public Property CurrentVaultId As String = ""
    Public Property VaultRootPath As String = ""
    Public Property DefaultIngestMode As String = "Move"
    Public Property DuplicatePolicy As String = "Rename"
    Public Property LastBackupPath As String = ""
    Public Property Vaults As New List(Of VaultModel)
    Public Property Stats As New List(Of StatCardModel)
    Public Property Categories As New List(Of CategoryModel)
    Public Property Tags As New List(Of String)
    Public Property Activities As New List(Of ActivityEntryModel)
    Public Property Artifacts As New List(Of ArtifactModel)
End Class

Public Class VaultRepairReport
    Public Property MissingFiles As Integer
    Public Property DuplicateHashGroups As Integer
    Public Property OrphanFiles As Integer
    Public Property AdoptedFiles As Integer

    Public ReadOnly Property Summary As String
        Get
            Return $"{MissingFiles:N0} missing file(s), {DuplicateHashGroups:N0} duplicate hash group(s), {OrphanFiles:N0} orphan file(s), {AdoptedFiles:N0} adopted"
        End Get
    End Property
End Class

Public Class VaultModel
    Public Property Id As String = ""
    Public Property Name As String = ""
    Public Property Path As String = ""
    Public Property IsSelected As Boolean

    <JsonIgnore>
    Public ReadOnly Property DisplayName As String
        Get
            If String.IsNullOrWhiteSpace(Path) Then
                Return Name
            End If

            Return $"{Name} ({Path})"
        End Get
    End Property
End Class

Public Class StatCardModel
    Public Property Label As String = ""
    Public Property Value As String = ""
    Public Property Icon As String = ""
    Public Property IconBrush As String = "#4DA3FF"
    Public Property IconBackground As String = "#153C67"
End Class

Public Class CategoryModel
    Public Property Name As String = ""
    Public Property Count As String = ""
End Class

Public Class ActivityEntryModel
    Public Property ActionText As String = ""
    Public Property DetailText As String = ""
    Public Property Icon As String = ""
    Public Property IconBrush As String = "#40557A"
    Public Property IconBackground As String = "#BAC8EF"
End Class

Public Class ArtifactModel
    Implements INotifyPropertyChanged

    Private _name As String = ""
    Private _id As String = ""
    Private _type As String = ""
    Private _typeFamily As String = ""
    Private _category As String = ""
    Private _size As String = ""
    Private _sizeBytes As Long
    Private _dateModified As String = ""
    Private _path As String = ""
    Private _relativePath As String = ""
    Private _created As String = ""
    Private _sha256 As String = ""
    Private _blake3 As String = ""
    Private _hashStatus As String = "Not checked"
    Private _rating As Integer
    Private _notes As String = ""
    Private _isStarred As Boolean
    Private _originalPath As String = ""
    Private _ingestedAt As String = ""
    Private _tags As New List(Of String)

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Property Name As String
        Get
            Return _name
        End Get
        Set(value As String)
            SetValue(_name, If(value, ""))
        End Set
    End Property

    Public Property Id As String
        Get
            Return _id
        End Get
        Set(value As String)
            SetValue(_id, If(value, ""))
        End Set
    End Property

    Public Property Type As String
        Get
            Return _type
        End Get
        Set(value As String)
            SetValue(_type, If(value, ""))
            OnPropertyChanged(NameOf(SummaryText))
        End Set
    End Property

    Public Property TypeFamily As String
        Get
            Return _typeFamily
        End Get
        Set(value As String)
            SetValue(_typeFamily, If(value, ""))
        End Set
    End Property

    Public Property Category As String
        Get
            Return _category
        End Get
        Set(value As String)
            SetValue(_category, If(value, ""))
        End Set
    End Property

    Public Property Size As String
        Get
            Return _size
        End Get
        Set(value As String)
            SetValue(_size, If(value, ""))
            OnPropertyChanged(NameOf(SummaryText))
        End Set
    End Property

    Public Property SizeBytes As Long
        Get
            Return _sizeBytes
        End Get
        Set(value As Long)
            SetValue(_sizeBytes, Math.Max(0, value))
        End Set
    End Property

    Public Property DateModified As String
        Get
            Return _dateModified
        End Get
        Set(value As String)
            SetValue(_dateModified, If(value, ""))
        End Set
    End Property

    Public Property Path As String
        Get
            Return _path
        End Get
        Set(value As String)
            SetValue(_path, If(value, ""))
        End Set
    End Property

    Public Property RelativePath As String
        Get
            Return _relativePath
        End Get
        Set(value As String)
            SetValue(_relativePath, If(value, ""))
        End Set
    End Property

    Public Property Created As String
        Get
            Return _created
        End Get
        Set(value As String)
            SetValue(_created, If(value, ""))
        End Set
    End Property

    Public Property Sha256 As String
        Get
            Return _sha256
        End Get
        Set(value As String)
            SetValue(_sha256, If(value, ""))
        End Set
    End Property

    Public Property Blake3 As String
        Get
            Return _blake3
        End Get
        Set(value As String)
            SetValue(_blake3, If(value, ""))
        End Set
    End Property

    Public Property HashStatus As String
        Get
            Return _hashStatus
        End Get
        Set(value As String)
            SetValue(_hashStatus, If(value, "Not checked"))
        End Set
    End Property

    Public Property Rating As Integer
        Get
            Return _rating
        End Get
        Set(value As Integer)
            If SetValue(_rating, Math.Max(0, Math.Min(5, value))) Then
                OnPropertyChanged(NameOf(RatingText))
            End If
        End Set
    End Property

    Public Property Notes As String
        Get
            Return _notes
        End Get
        Set(value As String)
            SetValue(_notes, If(value, ""))
        End Set
    End Property

    Public Property IsStarred As Boolean
        Get
            Return _isStarred
        End Get
        Set(value As Boolean)
            If SetValue(_isStarred, value) Then
                OnPropertyChanged(NameOf(StarText))
            End If
        End Set
    End Property

    Public Property OriginalPath As String
        Get
            Return _originalPath
        End Get
        Set(value As String)
            SetValue(_originalPath, If(value, ""))
        End Set
    End Property

    Public Property IngestedAt As String
        Get
            Return _ingestedAt
        End Get
        Set(value As String)
            SetValue(_ingestedAt, If(value, ""))
        End Set
    End Property

    Public Property Tags As List(Of String)
        Get
            Return _tags
        End Get
        Set(value As List(Of String))
            If value Is Nothing Then
                value = New List(Of String)
            End If

            If SetValue(_tags, value) Then
                OnPropertyChanged(NameOf(TagsText))
            End If
        End Set
    End Property

    <JsonIgnore>
    Public ReadOnly Property TagsText As String
        Get
            Return String.Join(", ", Tags)
        End Get
    End Property

    <JsonIgnore>
    Public ReadOnly Property SummaryText As String
        Get
            Return $"{Type}  •  {Size}"
        End Get
    End Property

    <JsonIgnore>
    Public ReadOnly Property RatingText As String
        Get
            Dim filled = Math.Max(0, Math.Min(5, Rating))
            Return New String("★"c, filled) & New String("☆"c, 5 - filled)
        End Get
    End Property

    <JsonIgnore>
    Public ReadOnly Property StarText As String
        Get
            If IsStarred Then
                Return "★"
            End If

            Return ""
        End Get
    End Property

    Private Function SetValue(Of T)(ByRef storage As T, value As T, <CallerMemberName> Optional propertyName As String = Nothing) As Boolean
        If EqualityComparer(Of T).Default.Equals(storage, value) Then
            Return False
        End If

        storage = value
        OnPropertyChanged(propertyName)
        Return True
    End Function

    Private Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
End Class
