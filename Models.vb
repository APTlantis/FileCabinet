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
    Public Property TableDensity As String = "Comfortable"
    Public Property ColumnPreset As String = "Full"
    Public Property ActiveScope As String = "All"
    Public Property SearchText As String = ""
    Public Property TagSearchText As String = ""
    Public Property SelectedTag As String = ""
    Public Property SelectedCategory As String = ""
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
    Public Property MissingThumbnails As Integer
    Public Property RegeneratedThumbnails As Integer
    Public Property MissingSamples As New List(Of String)
    Public Property OrphanSamples As New List(Of String)
    Public Property DuplicateSamples As New List(Of String)
    Public Property ThumbnailSamples As New List(Of String)

    Public ReadOnly Property Summary As String
        Get
            Return $"{MissingFiles:N0} missing file(s), {DuplicateHashGroups:N0} duplicate hash group(s), {OrphanFiles:N0} orphan file(s), {AdoptedFiles:N0} adopted, {MissingThumbnails:N0} missing thumbnail(s), {RegeneratedThumbnails:N0} regenerated"
        End Get
    End Property

    Public ReadOnly Property Detail As String
        Get
            Dim parts As New List(Of String)

            If MissingSamples.Count > 0 Then
                parts.Add("Missing: " & String.Join(", ", MissingSamples))
            End If

            If OrphanSamples.Count > 0 Then
                parts.Add("Orphans: " & String.Join(", ", OrphanSamples))
            End If

            If DuplicateSamples.Count > 0 Then
                parts.Add("Duplicates: " & String.Join(", ", DuplicateSamples))
            End If

            If ThumbnailSamples.Count > 0 Then
                parts.Add("Thumbnails: " & String.Join(", ", ThumbnailSamples))
            End If

            Return String.Join("  |  ", parts)
        End Get
    End Property
End Class

Public Class VaultHealthFinding
    Public Property FindingType As String = ""
    Public Property Subject As String = ""
    Public Property Detail As String = ""
    Public Property ProposedAction As String = ""
    Public Property RiskLevel As String = "Low"
    Public Property MutatesCatalog As Boolean
    Public Property TouchesRetainedFiles As Boolean
End Class

Public Class VaultHealthReport
    Public Property Findings As New List(Of VaultHealthFinding)

    Public ReadOnly Property FindingCount As Integer
        Get
            Return Findings.Count
        End Get
    End Property

    Public ReadOnly Property Summary As String
        Get
            If Findings.Count = 0 Then
                Return "Vault health: no findings"
            End If

            Dim groups = Findings.
                GroupBy(Function(finding) finding.FindingType).
                OrderBy(Function(group) group.Key).
                Select(Function(group) $"{group.Count():N0} {group.Key}")

            Return "Vault health: " & String.Join(", ", groups)
        End Get
    End Property

    Public ReadOnly Property Detail As String
        Get
            Return String.Join("  |  ", Findings.Take(5).Select(Function(finding) $"{finding.FindingType}: {finding.Subject}"))
        End Get
    End Property
End Class

Public Class RepairCandidate
    Public Property Finding As VaultHealthFinding
    Public Property ActionType As String = "ReviewOnly"
    Public Property CanRepairAutomatically As Boolean
    Public Property RequiresOperatorApproval As Boolean = True

    Public ReadOnly Property FindingType As String
        Get
            Return If(Finding?.FindingType, "")
        End Get
    End Property

    Public ReadOnly Property Subject As String
        Get
            Return If(Finding?.Subject, "")
        End Get
    End Property

    Public ReadOnly Property ProposedAction As String
        Get
            Return If(Finding?.ProposedAction, "")
        End Get
    End Property

    Public ReadOnly Property RiskLevel As String
        Get
            Return If(Finding?.RiskLevel, "")
        End Get
    End Property

    Public ReadOnly Property MutatesCatalog As Boolean
        Get
            Return Finding IsNot Nothing AndAlso Finding.MutatesCatalog
        End Get
    End Property

    Public ReadOnly Property TouchesRetainedFiles As Boolean
        Get
            Return Finding IsNot Nothing AndAlso Finding.TouchesRetainedFiles
        End Get
    End Property

    Public ReadOnly Property ApprovalText As String
        Get
            If RequiresOperatorApproval Then
                Return "Approval required"
            End If

            Return "Safe automatic repair"
        End Get
    End Property
End Class

Public Class VaultModel
    Implements INotifyPropertyChanged

    Private _id As String = ""
    Private _name As String = ""
    Private _path As String = ""
    Private _isSelected As Boolean

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Property Id As String
        Get
            Return _id
        End Get
        Set(value As String)
            SetValue(_id, If(value, ""))
        End Set
    End Property

    Public Property Name As String
        Get
            Return _name
        End Get
        Set(value As String)
            If SetValue(_name, If(value, "")) Then
                OnPropertyChanged(NameOf(DisplayName))
            End If
        End Set
    End Property

    Public Property Path As String
        Get
            Return _path
        End Get
        Set(value As String)
            If SetValue(_path, If(value, "")) Then
                OnPropertyChanged(NameOf(DisplayName))
            End If
        End Set
    End Property

    Public Property IsSelected As Boolean
        Get
            Return _isSelected
        End Get
        Set(value As Boolean)
            SetValue(_isSelected, value)
        End Set
    End Property

    <JsonIgnore>
    Public ReadOnly Property DisplayName As String
        Get
            If String.IsNullOrWhiteSpace(Path) Then
                Return Name
            End If

            Return $"{Name} ({Path})"
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

Public Class ArtifactRelationModel
    Public Property Artifact As ArtifactModel
    Public Property Score As Integer
    Public Property Reasons As New List(Of String)

    Public ReadOnly Property Name As String
        Get
            Return If(Artifact?.Name, "")
        End Get
    End Property

    Public ReadOnly Property Category As String
        Get
            Return If(Artifact?.Category, "")
        End Get
    End Property

    Public ReadOnly Property Size As String
        Get
            Return If(Artifact?.Size, "")
        End Get
    End Property

    Public ReadOnly Property ReasonsText As String
        Get
            If Reasons Is Nothing OrElse Reasons.Count = 0 Then
                Return "Related by catalog metadata"
            End If

            Return String.Join("  |  ", Reasons)
        End Get
    End Property

    Public ReadOnly Property ScoreText As String
        Get
            Return $"Score {Score:N0}"
        End Get
    End Property
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
    Private _extractedTextRelativePath As String = ""
    Private _extractedTextStatus As String = "Not extracted"
    Private _thumbnailRelativePath As String = ""
    Private _thumbnailStatus As String = "Not applicable"
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

    Public Property ExtractedTextRelativePath As String
        Get
            Return _extractedTextRelativePath
        End Get
        Set(value As String)
            SetValue(_extractedTextRelativePath, If(value, ""))
        End Set
    End Property

    Public Property ExtractedTextStatus As String
        Get
            Return _extractedTextStatus
        End Get
        Set(value As String)
            SetValue(_extractedTextStatus, If(value, "Not extracted"))
        End Set
    End Property

    Public Property ThumbnailRelativePath As String
        Get
            Return _thumbnailRelativePath
        End Get
        Set(value As String)
            SetValue(_thumbnailRelativePath, If(value, ""))
        End Set
    End Property

    Public Property ThumbnailStatus As String
        Get
            Return _thumbnailStatus
        End Get
        Set(value As String)
            SetValue(_thumbnailStatus, If(value, "Not applicable"))
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
