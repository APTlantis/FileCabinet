Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading.Tasks
Imports System.Windows
Imports System.Windows.Data
Imports System.Windows.Input

Public Class MainViewModel
    Implements INotifyPropertyChanged

    Private ReadOnly _catalogService As CatalogService
    Private ReadOnly _ingestionService As IngestionService
    Private ReadOnly _hashService As HashService
    Private ReadOnly _previewService As PreviewService
    Private _catalog As CatalogData
    Private _selectedArtifact As ArtifactModel
    Private _currentVault As VaultModel
    Private _filteredArtifacts As ICollectionView
    Private _searchText As String = ""
    Private _selectedCategory As CategoryModel
    Private _selectedTag As String
    Private _ingestStatus As String = "Ready to ingest files"
    Private _editName As String = ""
    Private _editCategory As String = ""
    Private _editTagsText As String = ""
    Private _editRating As Integer
    Private _editNotes As String = ""
    Private _editStatus As String = "No pending edits"
    Private _actionStatus As String = "Select an artifact to run actions"
    Private _selectedPreview As ArtifactPreview = New ArtifactPreview With {.Kind = ArtifactPreviewKind.GenericFile, .Message = "No preview"}
    Private _ingestProgress As Double
    Private _ingestDetail As String = ""
    Private _isIngesting As Boolean
    Private _isLoadingEditor As Boolean

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Property Vaults As New ObservableCollection(Of VaultModel)
    Public Property Stats As New ObservableCollection(Of StatCardModel)
    Public Property Categories As New ObservableCollection(Of CategoryModel)
    Public Property Tags As New ObservableCollection(Of String)
    Public Property Activities As New ObservableCollection(Of ActivityEntryModel)
    Public Property Artifacts As New ObservableCollection(Of ArtifactModel)
    Public Property ClearFiltersCommand As ICommand
    Public Property SaveArtifactCommand As ICommand
    Public Property RevertArtifactCommand As ICommand
    Public Property OpenLocationCommand As ICommand
    Public Property OpenFileCommand As ICommand
    Public Property HashCheckCommand As ICommand

    Public Sub New()
        _catalogService = New CatalogService()
        _ingestionService = New IngestionService()
        _hashService = New HashService()
        _previewService = New PreviewService()
        ClearFiltersCommand = New RelayCommand(Sub(parameter) ClearFilters())
        SaveArtifactCommand = New RelayCommand(Sub(parameter) SaveArtifactEdits(), Function(parameter) SelectedArtifact IsNot Nothing)
        RevertArtifactCommand = New RelayCommand(Sub(parameter) LoadEditorFromSelected(), Function(parameter) SelectedArtifact IsNot Nothing)
        OpenLocationCommand = New RelayCommand(Sub(parameter) OpenSelectedLocation(), Function(parameter) SelectedArtifact IsNot Nothing)
        OpenFileCommand = New RelayCommand(Sub(parameter) OpenSelectedFile(), Function(parameter) SelectedArtifact IsNot Nothing)
        HashCheckCommand = New RelayCommand(Sub(parameter) CheckSelectedHash(), Function(parameter) SelectedArtifact IsNot Nothing)
        LoadCatalog()
    End Sub

    Public Property CurrentVault As VaultModel
        Get
            Return _currentVault
        End Get
        Set(value As VaultModel)
            If Not ReferenceEquals(_currentVault, value) Then
                _currentVault = value
                OnPropertyChanged()
                OnPropertyChanged(NameOf(CurrentVaultTitle))
                OnPropertyChanged(NameOf(CurrentVaultSummary))
                OnPropertyChanged(NameOf(StorageUsedText))
                OnPropertyChanged(NameOf(StorageTotalText))
            End If
        End Set
    End Property

    Public Property SelectedArtifact As ArtifactModel
        Get
            Return _selectedArtifact
        End Get
        Set(value As ArtifactModel)
            If Not ReferenceEquals(_selectedArtifact, value) Then
                _selectedArtifact = value
                OnPropertyChanged()
                OnPropertyChanged(NameOf(StoredFileStatus))
                OnPropertyChanged(NameOf(SelectedBlake3Display))
                OnPropertyChanged(NameOf(SelectedSha256Display))
                ActionStatus = If(StoredFileExists(), "Stored file ready", "Stored file missing")
                LoadPreviewForSelected()
                LoadEditorFromSelected()
            End If
        End Set
    End Property

    Public Property FilteredArtifacts As ICollectionView
        Get
            Return _filteredArtifacts
        End Get
        Private Set(value As ICollectionView)
            If Not ReferenceEquals(_filteredArtifacts, value) Then
                _filteredArtifacts = value
                OnPropertyChanged()
            End If
        End Set
    End Property

    Public Property SearchText As String
        Get
            Return _searchText
        End Get
        Set(value As String)
            If _searchText <> value Then
                _searchText = If(value, "")
                OnPropertyChanged()
                RefreshFilters()
            End If
        End Set
    End Property

    Public Property SelectedCategory As CategoryModel
        Get
            Return _selectedCategory
        End Get
        Set(value As CategoryModel)
            If Not ReferenceEquals(_selectedCategory, value) Then
                _selectedCategory = value
                OnPropertyChanged()
                OnPropertyChanged(NameOf(FilterTitle))
                RefreshFilters()
            End If
        End Set
    End Property

    Public Property SelectedTag As String
        Get
            Return _selectedTag
        End Get
        Set(value As String)
            If _selectedTag <> value Then
                _selectedTag = value
                OnPropertyChanged()
                OnPropertyChanged(NameOf(FilterTitle))
                RefreshFilters()
            End If
        End Set
    End Property

    Public Property IngestStatus As String
        Get
            Return _ingestStatus
        End Get
        Set(value As String)
            If _ingestStatus <> value Then
                _ingestStatus = value
                OnPropertyChanged()
            End If
        End Set
    End Property

    Public Property EditName As String
        Get
            Return _editName
        End Get
        Set(value As String)
            SetEditValue(_editName, If(value, ""), NameOf(EditName))
        End Set
    End Property

    Public Property EditCategory As String
        Get
            Return _editCategory
        End Get
        Set(value As String)
            SetEditValue(_editCategory, If(value, ""), NameOf(EditCategory))
        End Set
    End Property

    Public Property EditTagsText As String
        Get
            Return _editTagsText
        End Get
        Set(value As String)
            SetEditValue(_editTagsText, If(value, ""), NameOf(EditTagsText))
        End Set
    End Property

    Public Property EditRating As Integer
        Get
            Return _editRating
        End Get
        Set(value As Integer)
            SetEditValue(_editRating, Math.Max(0, Math.Min(5, value)), NameOf(EditRating))
        End Set
    End Property

    Public Property EditNotes As String
        Get
            Return _editNotes
        End Get
        Set(value As String)
            SetEditValue(_editNotes, If(value, ""), NameOf(EditNotes))
        End Set
    End Property

    Public Property EditStatus As String
        Get
            Return _editStatus
        End Get
        Set(value As String)
            If _editStatus <> value Then
                _editStatus = value
                OnPropertyChanged()
            End If
        End Set
    End Property

    Public Property ActionStatus As String
        Get
            Return _actionStatus
        End Get
        Set(value As String)
            If _actionStatus <> value Then
                _actionStatus = value
                OnPropertyChanged()
            End If
        End Set
    End Property

    Public Property IngestProgress As Double
        Get
            Return _ingestProgress
        End Get
        Set(value As Double)
            If Math.Abs(_ingestProgress - value) > 0.01 Then
                _ingestProgress = value
                OnPropertyChanged()
            End If
        End Set
    End Property

    Public Property IngestDetail As String
        Get
            Return _ingestDetail
        End Get
        Set(value As String)
            If _ingestDetail <> value Then
                _ingestDetail = value
                OnPropertyChanged()
            End If
        End Set
    End Property

    Public Property IsIngesting As Boolean
        Get
            Return _isIngesting
        End Get
        Set(value As Boolean)
            If _isIngesting <> value Then
                _isIngesting = value
                OnPropertyChanged()
            End If
        End Set
    End Property

    Public Property SelectedPreview As ArtifactPreview
        Get
            Return _selectedPreview
        End Get
        Set(value As ArtifactPreview)
            If value Is Nothing Then
                value = New ArtifactPreview With {.Kind = ArtifactPreviewKind.GenericFile, .Message = "No preview"}
            End If

            If Not ReferenceEquals(_selectedPreview, value) Then
                _selectedPreview = value
                OnPropertyChanged()
                OnPropertyChanged(NameOf(IsImagePreview))
                OnPropertyChanged(NameOf(IsTextPreview))
                OnPropertyChanged(NameOf(IsGenericPreview))
                OnPropertyChanged(NameOf(IsMissingPreview))
            End If
        End Set
    End Property

    Public ReadOnly Property CurrentVaultTitle As String
        Get
            If CurrentVault Is Nothing Then
                Return "No vault selected"
            End If

            Return CurrentVault.DisplayName
        End Get
    End Property

    Public ReadOnly Property CurrentVaultSummary As String
        Get
            Return $"{Artifacts.Count:N0} items  •  247.8 GB used of 1.81 TB"
        End Get
    End Property

    Public ReadOnly Property FilterTitle As String
        Get
            Dim parts As New List(Of String)

            If SelectedCategory IsNot Nothing Then
                parts.Add(SelectedCategory.Name)
            End If

            If Not String.IsNullOrWhiteSpace(SelectedTag) Then
                parts.Add("#" & SelectedTag)
            End If

            If parts.Count = 0 AndAlso String.IsNullOrWhiteSpace(SearchText) Then
                Return "ALL ITEMS"
            End If

            If Not String.IsNullOrWhiteSpace(SearchText) Then
                parts.Add("search")
            End If

            Return "FILTERED ITEMS - " & String.Join(" / ", parts)
        End Get
    End Property

    Public ReadOnly Property StorageUsedText As String
        Get
            Return "247.8 GB used"
        End Get
    End Property

    Public ReadOnly Property StorageTotalText As String
        Get
            Return "1.81 TB total"
        End Get
    End Property

    Public ReadOnly Property CatalogPath As String
        Get
            Return _catalogService.CatalogPath
        End Get
    End Property

    Public ReadOnly Property VaultRootPath As String
        Get
            If _catalog Is Nothing OrElse String.IsNullOrWhiteSpace(_catalog.VaultRootPath) Then
                Return _catalogService.DefaultVaultRootPath
            End If

            Return _catalog.VaultRootPath
        End Get
    End Property

    Public ReadOnly Property CategoryNames As IEnumerable(Of String)
        Get
            Return Categories.Select(Function(category) category.Name)
        End Get
    End Property

    Public ReadOnly Property StoredFileStatus As String
        Get
            If SelectedArtifact Is Nothing Then
                Return "No artifact selected"
            End If

            If StoredFileExists() Then
                Return "Stored file present"
            End If

            Return "Stored file missing"
        End Get
    End Property

    Public ReadOnly Property SelectedBlake3Display As String
        Get
            If SelectedArtifact Is Nothing OrElse String.IsNullOrWhiteSpace(SelectedArtifact.Blake3) Then
                Return "(not computed yet)"
            End If

            Return SelectedArtifact.Blake3
        End Get
    End Property

    Public ReadOnly Property SelectedSha256Display As String
        Get
            If SelectedArtifact Is Nothing OrElse String.IsNullOrWhiteSpace(SelectedArtifact.Sha256) Then
                Return "(not computed yet)"
            End If

            Return SelectedArtifact.Sha256
        End Get
    End Property

    Public ReadOnly Property IsImagePreview As Boolean
        Get
            Return SelectedPreview IsNot Nothing AndAlso SelectedPreview.Kind = ArtifactPreviewKind.Image
        End Get
    End Property

    Public ReadOnly Property IsTextPreview As Boolean
        Get
            Return SelectedPreview IsNot Nothing AndAlso SelectedPreview.Kind = ArtifactPreviewKind.Text
        End Get
    End Property

    Public ReadOnly Property IsGenericPreview As Boolean
        Get
            Return SelectedPreview IsNot Nothing AndAlso SelectedPreview.Kind = ArtifactPreviewKind.GenericFile
        End Get
    End Property

    Public ReadOnly Property IsMissingPreview As Boolean
        Get
            Return SelectedPreview IsNot Nothing AndAlso SelectedPreview.Kind = ArtifactPreviewKind.Missing
        End Get
    End Property

    Private Sub LoadCatalog()
        _catalog = _catalogService.LoadOrCreate()

        ReplaceCollection(Vaults, _catalog.Vaults)
        ReplaceCollection(Stats, _catalog.Stats)
        ReplaceCollection(Categories, _catalog.Categories)
        ReplaceCollection(Tags, _catalog.Tags)
        ReplaceCollection(Activities, _catalog.Activities)
        ReplaceCollection(Artifacts, _catalog.Artifacts)
        RebuildDerivedLists()
        FilteredArtifacts = CollectionViewSource.GetDefaultView(Artifacts)
        FilteredArtifacts.Filter = AddressOf FilterArtifact

        CurrentVault = Vaults.FirstOrDefault(Function(v) v.Id = _catalog.CurrentVaultId)

        If CurrentVault Is Nothing Then
            CurrentVault = Vaults.FirstOrDefault()
        End If

        SelectFirstFilteredArtifact()
    End Sub

    Public Async Function IngestPathsAsync(paths As IEnumerable(Of String)) As Task
        If IsIngesting Then
            IngestStatus = "Ingestion already running"
            Return
        End If

        IsIngesting = True
        IngestProgress = 0
        IngestStatus = "Starting ingest"
        IngestDetail = ""

        Dim progress = New Progress(Of IngestionProgress)(Sub(update)
                                                              IngestStatus = update.Summary
                                                              IngestDetail = If(String.IsNullOrWhiteSpace(update.CurrentFile), update.CurrentStage, $"{update.CurrentStage}: {update.CurrentFile}")
                                                              IngestProgress = update.Percent
                                                          End Sub)

        Dim ingested As List(Of ArtifactModel)

        Try
            ingested = Await Task.Run(Function() _ingestionService.Ingest(paths, VaultRootPath, progress))
        Finally
            IsIngesting = False
        End Try

        If ingested.Count = 0 Then
            IngestStatus = "No files were ingested"
            IngestDetail = ""
            Return
        End If

        RunOnUiThread(Sub() ApplyIngestedArtifacts(ingested))
    End Function

    Private Sub ApplyIngestedArtifacts(ingested As List(Of ArtifactModel))
        For Each artifact In ingested
            Artifacts.Insert(0, artifact)
            _catalog.Artifacts.Insert(0, artifact)
        Next

        Dim activity = New ActivityEntryModel With {
            .ActionText = $"Ingested {ingested.Count:N0} file(s)",
            .DetailText = $"{DateTime.Now:yyyy-MM-dd HH:mm}  •  copied into vault",
            .Icon = ""
        }

        Activities.Insert(0, activity)
        _catalog.Activities.Insert(0, activity)

        RebuildDerivedLists()
        PersistDerivedCatalogLists()
        _catalogService.Save(_catalog)
        RefreshFilters()
        SelectedArtifact = ingested.First()
        IngestStatus = $"Ingested {ingested.Count:N0} file(s) into {VaultRootPath}"
        IngestDetail = "Catalog updated"
        IngestProgress = 100
        OnPropertyChanged(NameOf(CurrentVaultSummary))
    End Sub

    Private Sub LoadEditorFromSelected()
        _isLoadingEditor = True

        If SelectedArtifact Is Nothing Then
            EditName = ""
            EditCategory = ""
            EditTagsText = ""
            EditRating = 0
            EditNotes = ""
            EditStatus = "No artifact selected"
            _isLoadingEditor = False
            Return
        End If

        EditName = SelectedArtifact.Name
        EditCategory = SelectedArtifact.Category
        EditTagsText = SelectedArtifact.TagsText
        EditRating = SelectedArtifact.Rating
        EditNotes = SelectedArtifact.Notes
        EditStatus = "No pending edits"
        _isLoadingEditor = False
    End Sub

    Private Sub LoadPreviewForSelected()
        SelectedPreview = _previewService.LoadPreview(SelectedArtifact)
    End Sub

    Private Sub SaveArtifactEdits()
        If SelectedArtifact Is Nothing Then
            Return
        End If

        SelectedArtifact.Name = If(String.IsNullOrWhiteSpace(EditName), SelectedArtifact.Name, EditName.Trim())
        SelectedArtifact.Category = If(String.IsNullOrWhiteSpace(EditCategory), "Other", EditCategory.Trim())
        SelectedArtifact.Tags = ParseTags(EditTagsText)
        SelectedArtifact.Rating = EditRating
        SelectedArtifact.Notes = EditNotes.Trim()

        RebuildDerivedLists()
        PersistDerivedCatalogLists()
        _catalog.Artifacts = Artifacts.ToList()
        _catalogService.Save(_catalog)
        FilteredArtifacts.Refresh()
        EditStatus = $"Saved {SelectedArtifact.Name} at {DateTime.Now:HH:mm:ss}"
        OnPropertyChanged(NameOf(CategoryNames))
    End Sub

    Private Sub OpenSelectedLocation()
        If SelectedArtifact Is Nothing Then
            Return
        End If

        If Not StoredFileExists() Then
            ActionStatus = "Stored file missing"
            OnPropertyChanged(NameOf(StoredFileStatus))
            LoadPreviewForSelected()
            Return
        End If

        Process.Start(New ProcessStartInfo With {
            .FileName = "explorer.exe",
            .Arguments = $"/select,""{SelectedArtifact.Path}""",
            .UseShellExecute = True
        })
        ActionStatus = "Opened location"
    End Sub

    Private Sub OpenSelectedFile()
        If SelectedArtifact Is Nothing Then
            Return
        End If

        If Not StoredFileExists() Then
            ActionStatus = "Stored file missing"
            OnPropertyChanged(NameOf(StoredFileStatus))
            LoadPreviewForSelected()
            Return
        End If

        Process.Start(New ProcessStartInfo With {
            .FileName = SelectedArtifact.Path,
            .UseShellExecute = True
        })
        ActionStatus = "Opened file"
    End Sub

    Private Sub CheckSelectedHash()
        If SelectedArtifact Is Nothing Then
            Return
        End If

        If Not StoredFileExists() Then
            ActionStatus = "Stored file missing"
            OnPropertyChanged(NameOf(StoredFileStatus))
            LoadPreviewForSelected()
            Return
        End If

        Dim hashes = _hashService.ComputeHashes(SelectedArtifact.Path)
        Dim blakeMatches = String.IsNullOrWhiteSpace(SelectedArtifact.Blake3) OrElse
            String.Equals(SelectedArtifact.Blake3, hashes.Blake3, StringComparison.OrdinalIgnoreCase)
        Dim shaMatches = String.IsNullOrWhiteSpace(SelectedArtifact.Sha256) OrElse
            String.Equals(SelectedArtifact.Sha256, hashes.Sha256, StringComparison.OrdinalIgnoreCase)

        If String.IsNullOrWhiteSpace(SelectedArtifact.Blake3) Then
            SelectedArtifact.Blake3 = hashes.Blake3
        End If

        If String.IsNullOrWhiteSpace(SelectedArtifact.Sha256) Then
            SelectedArtifact.Sha256 = hashes.Sha256
        End If

        _catalog.Artifacts = Artifacts.ToList()
        _catalogService.Save(_catalog)
        OnPropertyChanged(NameOf(SelectedBlake3Display))
        OnPropertyChanged(NameOf(SelectedSha256Display))
        OnPropertyChanged(NameOf(StoredFileStatus))

        If blakeMatches AndAlso shaMatches Then
            ActionStatus = "Hash verified"
        ElseIf Not blakeMatches Then
            ActionStatus = "BLAKE3 mismatch"
        Else
            ActionStatus = "SHA-256 mismatch"
        End If
    End Sub

    Private Function StoredFileExists() As Boolean
        Return SelectedArtifact IsNot Nothing AndAlso
            Not String.IsNullOrWhiteSpace(SelectedArtifact.Path) AndAlso
            File.Exists(SelectedArtifact.Path)
    End Function

    Private Shared Function ParseTags(tagsText As String) As List(Of String)
        If String.IsNullOrWhiteSpace(tagsText) Then
            Return New List(Of String)
        End If

        Return tagsText.
            Split({","c, ";"c}, StringSplitOptions.RemoveEmptyEntries).
            Select(Function(tag) tag.Trim().ToLowerInvariant()).
            Where(Function(tag) tag.Length > 0).
            Distinct(StringComparer.OrdinalIgnoreCase).
            ToList()
    End Function

    Private Sub RebuildDerivedLists()
        Dim rebuiltCategories = Artifacts.
            GroupBy(Function(a) a.Category).
            OrderBy(Function(g) g.Key).
            Select(Function(g) New CategoryModel With {.Name = g.Key, .Count = g.Count().ToString("N0")}).
            ToList()

        ReplaceCollection(Categories, rebuiltCategories)
        OnPropertyChanged(NameOf(CategoryNames))

        Dim rebuiltTags = Artifacts.
            SelectMany(Function(a) If(a.Tags, New List(Of String))).
            Where(Function(t) Not String.IsNullOrWhiteSpace(t)).
            Distinct(StringComparer.OrdinalIgnoreCase).
            OrderBy(Function(t) t).
            ToList()

        ReplaceCollection(Tags, rebuiltTags)
    End Sub

    Private Sub SetEditValue(Of T)(ByRef storage As T, value As T, propertyName As String)
        If EqualityComparer(Of T).Default.Equals(storage, value) Then
            Return
        End If

        storage = value
        OnPropertyChanged(propertyName)

        If Not _isLoadingEditor Then
            EditStatus = "Unsaved edits"
        End If
    End Sub

    Private Sub PersistDerivedCatalogLists()
        _catalog.Categories = Categories.ToList()
        _catalog.Tags = Tags.ToList()
    End Sub

    Private Function FilterArtifact(item As Object) As Boolean
        Dim artifact = TryCast(item, ArtifactModel)

        If artifact Is Nothing Then
            Return False
        End If

        If SelectedCategory IsNot Nothing AndAlso artifact.Category <> SelectedCategory.Name Then
            Return False
        End If

        If Not String.IsNullOrWhiteSpace(SelectedTag) Then
            If artifact.Tags Is Nothing OrElse Not artifact.Tags.Any(Function(t) String.Equals(t, SelectedTag, StringComparison.OrdinalIgnoreCase)) Then
                Return False
            End If
        End If

        If String.IsNullOrWhiteSpace(SearchText) Then
            Return True
        End If

        Dim needle = SearchText.Trim()
        Return ContainsText(artifact.Name, needle) OrElse
            ContainsText(artifact.Type, needle) OrElse
            ContainsText(artifact.Category, needle) OrElse
            ContainsText(artifact.Path, needle) OrElse
            ContainsText(artifact.Notes, needle) OrElse
            ContainsText(artifact.TagsText, needle) OrElse
            ContainsText(artifact.Sha256, needle)
    End Function

    Private Shared Function ContainsText(value As String, needle As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(value) AndAlso
            value.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0
    End Function

    Private Sub RefreshFilters()
        If FilteredArtifacts Is Nothing Then
            Return
        End If

        FilteredArtifacts.Refresh()
        OnPropertyChanged(NameOf(FilterTitle))
        SelectFirstFilteredArtifact()
    End Sub

    Private Sub SelectFirstFilteredArtifact()
        If FilteredArtifacts Is Nothing Then
            SelectedArtifact = Artifacts.FirstOrDefault()
            Return
        End If

        SelectedArtifact = FilteredArtifacts.Cast(Of ArtifactModel)().FirstOrDefault()
    End Sub

    Private Sub ClearFilters()
        SelectedCategory = Nothing
        SelectedTag = Nothing
        SearchText = ""
        RefreshFilters()
    End Sub

    Private Shared Sub RunOnUiThread(action As Action)
        Dim dispatcher = Application.Current?.Dispatcher

        If dispatcher IsNot Nothing AndAlso Not dispatcher.CheckAccess() Then
            dispatcher.Invoke(action)
            Return
        End If

        action()
    End Sub

    Private Shared Sub ReplaceCollection(Of T)(target As ObservableCollection(Of T), source As IEnumerable(Of T))
        target.Clear()

        If source Is Nothing Then
            Return
        End If

        For Each item In source
            target.Add(item)
        Next
    End Sub

    Private Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = Nothing)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
    End Sub
End Class
