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
    Private ReadOnly _thumbnailService As ThumbnailService
    Private _catalog As CatalogData
    Private _selectedArtifact As ArtifactModel
    Private _currentVault As VaultModel
    Private _filteredArtifacts As ICollectionView
    Private _searchText As String = ""
    Private _tagSearchText As String = ""
    Private _selectedCategory As CategoryModel
    Private _selectedTag As String
    Private _activeScope As String = "All"
    Private _ingestStatus As String = "Ready to ingest files"
    Private _editName As String = ""
    Private _editCategory As String = ""
    Private _editTagsText As String = ""
    Private _editRating As Integer
    Private _editNotes As String = ""
    Private _editStatus As String = "No pending edits"
    Private _actionStatus As String = "Select an artifact to run actions"
    Private _selectedPreview As ArtifactPreview = New ArtifactPreview With {.Kind = ArtifactPreviewKind.GenericFile, .Message = "No preview"}
    Private _rightPanelTab As String = "Preview"
    Private _ingestProgress As Double
    Private _ingestDetail As String = ""
    Private _isIngesting As Boolean
    Private _isLoadingEditor As Boolean
    Private _ingestMode As IngestMode = IngestMode.Move
    Private _settingsText As String = ""
    Private _repairStatus As String = "Repair checks not run"
    Private _artifactRowHeight As Integer = 34
    Private _columnPresetIndex As Integer
    Private _isLoadingCatalog As Boolean

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Property Vaults As New ObservableCollection(Of VaultModel)
    Public Property Stats As New ObservableCollection(Of StatCardModel)
    Public Property Categories As New ObservableCollection(Of CategoryModel)
    Public Property Tags As New ObservableCollection(Of String)
    Public Property Activities As New ObservableCollection(Of ActivityEntryModel)
    Public Property Artifacts As New ObservableCollection(Of ArtifactModel)
    Public Property RelatedArtifacts As New ObservableCollection(Of ArtifactRelationModel)
    Private _filteredTags As ICollectionView
    Public Property ClearFiltersCommand As ICommand
    Public Property ShowAllItemsCommand As ICommand
    Public Property ShowRecentCommand As ICommand
    Public Property ShowStarredCommand As ICommand
    Public Property ShowQuarantineCommand As ICommand
    Public Property RemoveCurrentVaultCommand As ICommand
    Public Property SaveArtifactCommand As ICommand
    Public Property RevertArtifactCommand As ICommand
    Public Property OpenLocationCommand As ICommand
    Public Property OpenFileCommand As ICommand
    Public Property RestoreArtifactCommand As ICommand
    Public Property PermanentlyDeleteArtifactCommand As ICommand
    Public Property HashCheckCommand As ICommand
    Public Property RefreshCommand As ICommand
    Public Property ToggleStarCommand As ICommand
    Public Property AddTagsCommand As ICommand
    Public Property ToggleIngestModeCommand As ICommand
    Public Property QuarantineCommand As ICommand
    Public Property ShowSettingsCommand As ICommand
    Public Property BackupCatalogCommand As ICommand
    Public Property RepairCatalogCommand As ICommand
    Public Property RescanVaultCommand As ICommand
    Public Property SortByNameCommand As ICommand
    Public Property SortByDateCommand As ICommand
    Public Property ToggleDensityCommand As ICommand
    Public Property CycleColumnPresetCommand As ICommand

    Public Sub New()
        _catalogService = New CatalogService()
        _ingestionService = New IngestionService()
        _hashService = New HashService()
        _thumbnailService = New ThumbnailService()
        _previewService = New PreviewService(_thumbnailService)
        ClearFiltersCommand = New RelayCommand(Sub(parameter) ClearFilters())
        ShowAllItemsCommand = New RelayCommand(Sub(parameter) SetScope("All"))
        ShowRecentCommand = New RelayCommand(Sub(parameter) SetScope("Recent"))
        ShowStarredCommand = New RelayCommand(Sub(parameter) SetScope("Starred"))
        ShowQuarantineCommand = New RelayCommand(Sub(parameter) SetScope("Quarantine"))
        RemoveCurrentVaultCommand = New RelayCommand(Sub(parameter) RemoveCurrentVault(), Function(parameter) CurrentVault IsNot Nothing AndAlso Vaults.Count > 1)
        SaveArtifactCommand = New RelayCommand(Sub(parameter) SaveArtifactEdits(), Function(parameter) SelectedArtifact IsNot Nothing)
        RevertArtifactCommand = New RelayCommand(Sub(parameter) LoadEditorFromSelected(), Function(parameter) SelectedArtifact IsNot Nothing)
        OpenLocationCommand = New RelayCommand(Sub(parameter) OpenSelectedLocation(), Function(parameter) SelectedArtifact IsNot Nothing)
        OpenFileCommand = New RelayCommand(Sub(parameter) OpenSelectedFile(), Function(parameter) SelectedArtifact IsNot Nothing)
        RestoreArtifactCommand = New RelayCommand(Sub(parameter) RestoreSelectedArtifact(TryCast(parameter, String)), Function(parameter) SelectedArtifact IsNot Nothing)
        PermanentlyDeleteArtifactCommand = New RelayCommand(Sub(parameter) PermanentlyDeleteSelectedArtifact(), Function(parameter) SelectedArtifact IsNot Nothing)
        HashCheckCommand = New RelayCommand(Sub(parameter) CheckSelectedHash(), Function(parameter) SelectedArtifact IsNot Nothing)
        RefreshCommand = New RelayCommand(Sub(parameter) RefreshVaultState())
        ToggleStarCommand = New RelayCommand(Sub(parameter) ToggleSelectedStar(), Function(parameter) SelectedArtifact IsNot Nothing)
        AddTagsCommand = New RelayCommand(Sub(parameter) FocusTagEditing(), Function(parameter) SelectedArtifact IsNot Nothing)
        ToggleIngestModeCommand = New RelayCommand(Sub(parameter) ToggleIngestMode())
        QuarantineCommand = New RelayCommand(Sub(parameter) QuarantineSelectedArtifact(), Function(parameter) SelectedArtifact IsNot Nothing)
        ShowSettingsCommand = New RelayCommand(Sub(parameter) ShowSettings())
        BackupCatalogCommand = New RelayCommand(Sub(parameter) BackupCatalog())
        RepairCatalogCommand = New RelayCommand(Sub(parameter) RepairCatalog())
        RescanVaultCommand = New RelayCommand(Sub(parameter) RescanVault())
        SortByNameCommand = New RelayCommand(Sub(parameter) ApplySort(NameOf(ArtifactModel.Name)))
        SortByDateCommand = New RelayCommand(Sub(parameter) ApplySort(NameOf(ArtifactModel.DateModified)))
        ToggleDensityCommand = New RelayCommand(Sub(parameter) ToggleDensity())
        CycleColumnPresetCommand = New RelayCommand(Sub(parameter) CycleColumnPreset())
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
                OnPropertyChanged(NameOf(LastBackupDisplay))
                If _catalog IsNot Nothing AndAlso value IsNot Nothing Then
                    _catalog.CurrentVaultId = value.Id
                    _catalog.VaultRootPath = value.Path
                    _catalogService.Save(_catalog)
                End If
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
                RebuildRelatedArtifacts()
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

    Public Property FilteredTags As ICollectionView
        Get
            Return _filteredTags
        End Get
        Private Set(value As ICollectionView)
            If Not ReferenceEquals(_filteredTags, value) Then
                _filteredTags = value
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
                SaveUiPreferences()
            End If
        End Set
    End Property

    Public Property TagSearchText As String
        Get
            Return _tagSearchText
        End Get
        Set(value As String)
            Dim normalized = If(value, "")
            If _tagSearchText <> normalized Then
                _tagSearchText = normalized
                OnPropertyChanged()
                RefreshTagFilter()
                SaveUiPreferences()
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
                SaveUiPreferences()
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
                SaveUiPreferences()
            End If
        End Set
    End Property

    Public Property ActiveScope As String
        Get
            Return _activeScope
        End Get
        Set(value As String)
            Dim normalized = If(String.IsNullOrWhiteSpace(value), "All", value)
            If _activeScope <> normalized Then
                _activeScope = normalized
                OnPropertyChanged()
                OnPropertyChanged(NameOf(FilterTitle))
                OnPropertyChanged(NameOf(ActiveScopeText))
                RefreshFilters(preserveSelection:=True)
                SaveUiPreferences()
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

    Public Property CurrentIngestMode As IngestMode
        Get
            Return _ingestMode
        End Get
        Set(value As IngestMode)
            If _ingestMode <> value Then
                _ingestMode = value
                If _catalog IsNot Nothing Then
                    _catalog.DefaultIngestMode = value.ToString()
                    _catalogService.Save(_catalog)
                End If
                OnPropertyChanged()
                OnPropertyChanged(NameOf(IngestModeText))
                OnPropertyChanged(NameOf(IngestModeDetail))
                OnPropertyChanged(NameOf(SettingsText))
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

    Public Property RightPanelTab As String
        Get
            Return _rightPanelTab
        End Get
        Set(value As String)
            Dim normalized = NormalizeRightPanelTab(value)
            If _rightPanelTab <> normalized Then
                _rightPanelTab = normalized
                OnPropertyChanged()
                OnPropertyChanged(NameOf(IsPreviewTabSelected))
                OnPropertyChanged(NameOf(IsDetailsTabSelected))
                OnPropertyChanged(NameOf(IsRelationsTabSelected))
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
            Return $"{Artifacts.Count:N0} items  •  {FormatSize(Artifacts.Sum(Function(a) a.SizeBytes))} retained"
        End Get
    End Property

    Public ReadOnly Property FilterTitle As String
        Get
            Dim parts As New List(Of String)

            If Not String.Equals(ActiveScope, "All", StringComparison.OrdinalIgnoreCase) Then
                parts.Add(ActiveScope)
            End If

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

    Public ReadOnly Property ActiveScopeText As String
        Get
            Return $"View: {ActiveScope}"
        End Get
    End Property

    Public ReadOnly Property StorageUsedText As String
        Get
            Return $"{FormatSize(Artifacts.Sum(Function(a) a.SizeBytes))} cataloged"
        End Get
    End Property

    Public ReadOnly Property StorageTotalText As String
        Get
            If Not Directory.Exists(VaultRootPath) Then
                Return "vault unavailable"
            End If

            Dim root = Path.GetPathRoot(Path.GetFullPath(VaultRootPath))
            If String.IsNullOrWhiteSpace(root) Then
                Return "storage unknown"
            End If

            Dim drive = New DriveInfo(root)
            Return $"{FormatSize(drive.AvailableFreeSpace)} free"
        End Get
    End Property

    Public ReadOnly Property IngestModeText As String
        Get
            If CurrentIngestMode = IngestMode.Move Then
                Return "Move into vault"
            End If

            Return "Copy into vault"
        End Get
    End Property

    Public ReadOnly Property IngestModeDetail As String
        Get
            If CurrentIngestMode = IngestMode.Move Then
                Return "Default intake removes the original after a verified transfer."
            End If

            Return "Default intake keeps the original file in place."
        End Get
    End Property

    Public ReadOnly Property DuplicatePolicyText As String
        Get
            Return "Duplicates: rename safely"
        End Get
    End Property

    Public ReadOnly Property InboxCountText As String
        Get
            Return Artifacts.Where(Function(a) IsRecentArtifact(a)).Count().ToString("N0")
        End Get
    End Property

    Public ReadOnly Property StarredCountText As String
        Get
            Return Artifacts.Where(Function(a) a.IsStarred).Count().ToString("N0")
        End Get
    End Property

    Public ReadOnly Property QuarantineCountText As String
        Get
            Dim quarantineRoot = Path.Combine(VaultRootPath, "quarantine")
            If Not Directory.Exists(quarantineRoot) Then
                Return "0"
            End If

            Return Directory.EnumerateFiles(quarantineRoot, "*", SearchOption.AllDirectories).Count().ToString("N0")
        End Get
    End Property

    Public ReadOnly Property SettingsText As String
        Get
            Return _settingsText
        End Get
    End Property

    Public ReadOnly Property LastBackupDisplay As String
        Get
            If _catalog Is Nothing OrElse String.IsNullOrWhiteSpace(_catalog.LastBackupPath) Then
                Return "No catalog backup yet"
            End If

            Return _catalog.LastBackupPath
        End Get
    End Property

    Public ReadOnly Property RecallStatusText As String
        Get
            Return "Deterministic metadata, text, hashes, and relations"
        End Get
    End Property

    Public Property ArtifactRowHeight As Integer
        Get
            Return _artifactRowHeight
        End Get
        Set(value As Integer)
            Dim normalized = Math.Max(28, Math.Min(42, value))
            If _artifactRowHeight <> normalized Then
                _artifactRowHeight = normalized
                OnPropertyChanged()
                OnPropertyChanged(NameOf(DensityText))
                SaveUiPreferences()
            End If
        End Set
    End Property

    Public ReadOnly Property DensityText As String
        Get
            If ArtifactRowHeight <= 30 Then
                Return "Comfortable rows"
            End If

            Return "Compact rows"
        End Get
    End Property

    Public ReadOnly Property ColumnPresetText As String
        Get
            Return $"Columns: {ColumnPresetName}"
        End Get
    End Property

    Public ReadOnly Property ShowTypeColumn As Boolean
        Get
            Return True
        End Get
    End Property

    Public ReadOnly Property ShowCategoryColumn As Boolean
        Get
            Return _columnPresetIndex = 0
        End Get
    End Property

    Public ReadOnly Property ShowSizeColumn As Boolean
        Get
            Return True
        End Get
    End Property

    Public ReadOnly Property ShowDateColumn As Boolean
        Get
            Return _columnPresetIndex <= 1
        End Get
    End Property

    Public ReadOnly Property ShowTagsColumn As Boolean
        Get
            Return _columnPresetIndex = 0
        End Get
    End Property

    Public ReadOnly Property RelatedArtifactsSummary As String
        Get
            If SelectedArtifact Is Nothing Then
                Return "No artifact selected"
            End If

            If RelatedArtifacts.Count = 0 Then
                Return "No close relations found yet"
            End If

            Return $"{RelatedArtifacts.Count:N0} related item(s)"
        End Get
    End Property

    Public ReadOnly Property RepairStatus As String
        Get
            Return _repairStatus
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

    Public Property IsPreviewTabSelected As Boolean
        Get
            Return String.Equals(RightPanelTab, "Preview", StringComparison.OrdinalIgnoreCase)
        End Get
        Set(value As Boolean)
            If value Then
                RightPanelTab = "Preview"
            End If
        End Set
    End Property

    Public Property IsDetailsTabSelected As Boolean
        Get
            Return String.Equals(RightPanelTab, "Details", StringComparison.OrdinalIgnoreCase)
        End Get
        Set(value As Boolean)
            If value Then
                RightPanelTab = "Details"
            End If
        End Set
    End Property

    Public Property IsRelationsTabSelected As Boolean
        Get
            Return String.Equals(RightPanelTab, "Relations", StringComparison.OrdinalIgnoreCase)
        End Get
        Set(value As Boolean)
            If value Then
                RightPanelTab = "Relations"
            End If
        End Set
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

    Private Shared Function NormalizeRightPanelTab(value As String) As String
        Select Case If(value, "").Trim().ToLowerInvariant()
            Case "details"
                Return "Details"
            Case "relations"
                Return "Relations"
            Case Else
                Return "Preview"
        End Select
    End Function

    Private Sub LoadCatalog()
        _isLoadingCatalog = True
        _catalog = _catalogService.LoadOrCreate()
        If String.Equals(_catalog.DefaultIngestMode, "Copy", StringComparison.OrdinalIgnoreCase) Then
            _ingestMode = IngestMode.Copy
        Else
            _ingestMode = IngestMode.Move
        End If

        _artifactRowHeight = If(String.Equals(_catalog.TableDensity, "Compact", StringComparison.OrdinalIgnoreCase), 28, 34)
        _columnPresetIndex = ParseColumnPreset(_catalog.ColumnPreset)
        _activeScope = NormalizeScope(_catalog.ActiveScope)
        _searchText = If(_catalog.SearchText, "")
        _tagSearchText = If(_catalog.TagSearchText, "")
        _selectedTag = If(_catalog.SelectedTag, "")

        ReplaceCollection(Vaults, _catalog.Vaults)
        ReplaceCollection(Stats, _catalog.Stats)
        ReplaceCollection(Categories, _catalog.Categories)
        ReplaceCollection(Tags, _catalog.Tags)
        ReplaceCollection(Activities, _catalog.Activities)
        HydrateArtifacts(_catalog.Artifacts)
        ReplaceCollection(Artifacts, _catalog.Artifacts)
        RebuildDerivedLists()
        _selectedCategory = Categories.FirstOrDefault(Function(category) String.Equals(category.Name, _catalog.SelectedCategory, StringComparison.OrdinalIgnoreCase))
        FilteredTags = CollectionViewSource.GetDefaultView(Tags)
        FilteredTags.Filter = AddressOf FilterTag
        FilteredArtifacts = CollectionViewSource.GetDefaultView(Artifacts)
        FilteredArtifacts.Filter = AddressOf FilterArtifact

        CurrentVault = Vaults.FirstOrDefault(Function(v) v.Id = _catalog.CurrentVaultId)

        If CurrentVault Is Nothing Then
            CurrentVault = Vaults.FirstOrDefault()
        End If

        SelectFirstFilteredArtifact()
        _settingsText = "Open settings for vault paths, backups, and repair status"
        _isLoadingCatalog = False
        SaveUiPreferences()
        RefreshDerivedUiState()
        OnPropertyChanged(NameOf(SearchText))
        OnPropertyChanged(NameOf(TagSearchText))
        OnPropertyChanged(NameOf(SelectedTag))
        OnPropertyChanged(NameOf(SelectedCategory))
        OnPropertyChanged(NameOf(ActiveScope))
        OnPropertyChanged(NameOf(FilterTitle))
        OnPropertyChanged(NameOf(DensityText))
        OnPropertyChanged(NameOf(ColumnPresetText))
        OnPropertyChanged(NameOf(ShowCategoryColumn))
        OnPropertyChanged(NameOf(ShowDateColumn))
        OnPropertyChanged(NameOf(ShowTagsColumn))
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
            ingested = Await Task.Run(Function() _ingestionService.Ingest(paths, VaultRootPath, progress, CurrentIngestMode))
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
        IngestDetail = $"{IngestModeText}; catalog updated"
        IngestProgress = 100
        RefreshDerivedUiState()
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

        Dim validatedName = If(EditName, "").Trim()
        If String.IsNullOrWhiteSpace(validatedName) Then
            EditStatus = "Name is required"
            Return
        End If

        Dim validatedCategory = If(EditCategory, "").Trim()
        If String.IsNullOrWhiteSpace(validatedCategory) Then
            validatedCategory = "Other"
        End If

        If validatedCategory.Length > 80 Then
            EditStatus = "Category is too long"
            Return
        End If

        SelectedArtifact.Name = validatedName
        SelectedArtifact.Category = validatedCategory
        Dim parsedTags = ParseTags(EditTagsText)
        If parsedTags.Count > 32 Then
            EditStatus = "Use 32 tags or fewer"
            Return
        End If

        If parsedTags.Any(Function(tag) tag.Length > 48) Then
            EditStatus = "Tags must be 48 characters or fewer"
            Return
        End If

        SelectedArtifact.Tags = parsedTags
        SelectedArtifact.Rating = EditRating
        SelectedArtifact.Notes = If(EditNotes, "").Trim()

        RebuildDerivedLists()
        PersistDerivedCatalogLists()
        _catalog.Artifacts = Artifacts.ToList()
        _catalogService.Save(_catalog)
        FilteredArtifacts.Refresh()
        EditStatus = $"Saved {SelectedArtifact.Name} at {DateTime.Now:HH:mm:ss}"
        OnPropertyChanged(NameOf(CategoryNames))
        RefreshDerivedUiState()
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

    Public Function GetSelectedArtifactName() As String
        If SelectedArtifact Is Nothing Then
            Return ""
        End If

        Return SelectedArtifact.Name
    End Function

    Private Sub RestoreSelectedArtifact(destinationFolder As String)
        If SelectedArtifact Is Nothing Then
            Return
        End If

        If Not StoredFileExists() Then
            ActionStatus = "Stored file missing"
            OnPropertyChanged(NameOf(StoredFileStatus))
            LoadPreviewForSelected()
            Return
        End If

        If String.IsNullOrWhiteSpace(destinationFolder) Then
            ActionStatus = "Choose a restore folder first"
            Return
        End If

        Try
            Directory.CreateDirectory(destinationFolder)
            Dim destination = BuildUniquePath(destinationFolder, SelectedArtifact.Name)
            File.Copy(SelectedArtifact.Path, destination, overwrite:=False)

            Dim activity = New ActivityEntryModel With {
                .ActionText = $"Restored {SelectedArtifact.Name}",
                .DetailText = $"{DateTime.Now:yyyy-MM-dd HH:mm}  •  {destination}",
                .Icon = ""
            }
            Activities.Insert(0, activity)
            _catalog.Activities.Insert(0, activity)
            _catalogService.Save(_catalog)
            ActionStatus = $"Restored copy to {destination}"
        Catch ex As Exception
            ActionStatus = $"Restore failed: {ex.Message}"
        End Try
    End Sub

    Private Sub PermanentlyDeleteSelectedArtifact()
        If SelectedArtifact Is Nothing Then
            Return
        End If

        Dim artifact = SelectedArtifact

        Try
            If Not String.IsNullOrWhiteSpace(artifact.Path) AndAlso File.Exists(artifact.Path) Then
                File.Delete(artifact.Path)
            End If

            Artifacts.Remove(artifact)
            _catalog.Artifacts = Artifacts.ToList()
            Dim activity = New ActivityEntryModel With {
                .ActionText = $"Deleted {artifact.Name}",
                .DetailText = $"{DateTime.Now:yyyy-MM-dd HH:mm}  •  removed from vault",
                .Icon = "",
                .IconBrush = "#F24F5F",
                .IconBackground = "#4A1F29"
            }
            Activities.Insert(0, activity)
            _catalog.Activities.Insert(0, activity)
            RebuildDerivedLists()
            PersistDerivedCatalogLists()
            _catalogService.Save(_catalog)
            RefreshFilters()
            RefreshDerivedUiState()
            ActionStatus = $"Deleted {artifact.Name}"
        Catch ex As Exception
            ActionStatus = $"Delete failed: {ex.Message}"
        End Try
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

        If blakeMatches AndAlso shaMatches Then
            SelectedArtifact.HashStatus = "Verified"
            ActionStatus = "Hash verified"
        ElseIf Not blakeMatches Then
            SelectedArtifact.HashStatus = "BLAKE3 mismatch"
            ActionStatus = "BLAKE3 mismatch"
        Else
            SelectedArtifact.HashStatus = "SHA-256 mismatch"
            ActionStatus = "SHA-256 mismatch"
        End If

        _catalog.Artifacts = Artifacts.ToList()
        _catalogService.Save(_catalog)
        OnPropertyChanged(NameOf(SelectedBlake3Display))
        OnPropertyChanged(NameOf(SelectedSha256Display))
        OnPropertyChanged(NameOf(StoredFileStatus))
    End Sub

    Private Sub ToggleSelectedStar()
        If SelectedArtifact Is Nothing Then
            Return
        End If

        SelectedArtifact.IsStarred = Not SelectedArtifact.IsStarred
        _catalog.Artifacts = Artifacts.ToList()
        _catalogService.Save(_catalog)
        ActionStatus = If(SelectedArtifact.IsStarred, "Marked starred", "Removed star")
        RefreshDerivedUiState()
    End Sub

    Private Sub FocusTagEditing()
        If SelectedArtifact Is Nothing Then
            Return
        End If

        If String.IsNullOrWhiteSpace(EditTagsText) Then
            EditTagsText = "review"
        ElseIf Not EditTagsText.Split({","c, ";"c}, StringSplitOptions.RemoveEmptyEntries).Any(Function(t) String.Equals(t.Trim(), "review", StringComparison.OrdinalIgnoreCase)) Then
            EditTagsText &= ", review"
        End If

        EditStatus = "Added review tag; save to persist"
    End Sub

    Private Sub ToggleIngestMode()
        CurrentIngestMode = If(CurrentIngestMode = IngestMode.Move, IngestMode.Copy, IngestMode.Move)
        IngestStatus = $"{IngestModeText} selected"
        IngestDetail = IngestModeDetail
    End Sub

    Private Sub QuarantineSelectedArtifact()
        If SelectedArtifact Is Nothing Then
            Return
        End If

        If Not StoredFileExists() Then
            ActionStatus = "Stored file missing"
            Return
        End If

        Dim artifact = SelectedArtifact
        Dim quarantineRoot = Path.Combine(VaultRootPath, "quarantine", DateTime.Now.ToString("yyyy"), DateTime.Now.ToString("MM"))
        Directory.CreateDirectory(quarantineRoot)
        Dim destination = BuildUniquePath(quarantineRoot, Path.GetFileName(artifact.Path))

        Try
            File.Move(artifact.Path, destination)
            artifact.Path = destination
            artifact.RelativePath = Path.GetRelativePath(VaultRootPath, destination)
            artifact.Category = "Quarantine"
            artifact.Notes = $"{artifact.Notes}{vbCrLf}Quarantined at {DateTime.Now:yyyy-MM-dd HH:mm}".Trim()
            _catalog.Artifacts = Artifacts.ToList()
            _catalogService.Save(_catalog)
            RebuildDerivedLists()
            RefreshFilters(preserveSelection:=True)
            ActionStatus = "Moved to quarantine"
            RefreshDerivedUiState()
        Catch ex As Exception
            ActionStatus = $"Quarantine failed: {ex.Message}"
        End Try
    End Sub

    Private Sub ShowSettings()
        Dim backupText = If(String.IsNullOrWhiteSpace(_catalog.LastBackupPath), "No catalog backup yet", _catalog.LastBackupPath)
        _settingsText = $"Vault: {VaultRootPath}{vbCrLf}Catalog: {CatalogPath}{vbCrLf}Last backup: {backupText}{vbCrLf}Intake: {IngestModeText}{vbCrLf}{DuplicatePolicyText}{vbCrLf}Repair: {RepairStatus}{vbCrLf}Recall: deterministic metadata, text, hashes, and relations"
        OnPropertyChanged(NameOf(SettingsText))
        OnPropertyChanged(NameOf(LastBackupDisplay))
        OnPropertyChanged(NameOf(RecallStatusText))
        ActionStatus = "Settings summarized"
    End Sub

    Private Sub BackupCatalog()
        Try
            Dim exportsRoot = Path.Combine(VaultRootPath, "exports")
            Dim backupPath = _catalogService.ExportSnapshot(_catalog, exportsRoot)
            ActionStatus = $"Catalog backup created: {backupPath}"
            OnPropertyChanged(NameOf(LastBackupDisplay))
            ShowSettings()
        Catch ex As Exception
            ActionStatus = $"Backup failed: {ex.Message}"
        End Try
    End Sub

    Private Sub RepairCatalog()
        Dim report = BuildRepairReport(adoptOrphans:=False)
        _repairStatus = BuildRepairStatus(report)
        OnPropertyChanged(NameOf(RepairStatus))
        ActionStatus = _repairStatus
        ShowSettings()
    End Sub

    Private Sub RescanVault()
        Dim report = BuildRepairReport(adoptOrphans:=True)
        _repairStatus = BuildRepairStatus(report)
        RebuildDerivedLists()
        PersistDerivedCatalogLists()
        _catalog.Artifacts = Artifacts.ToList()
        _catalogService.Save(_catalog)
        RefreshFilters(preserveSelection:=True)
        RefreshDerivedUiState()
        OnPropertyChanged(NameOf(RepairStatus))
        ActionStatus = $"Rescan complete: {_repairStatus}"
        ShowSettings()
    End Sub

    Private Sub RefreshVaultState()
        CatalogService.EnsureVaultFolders(VaultRootPath)
        RepairCatalog()
        RebuildDerivedLists()
        PersistDerivedCatalogLists()
        _catalog.Artifacts = Artifacts.ToList()
        _catalogService.Save(_catalog)
        RefreshFilters(preserveSelection:=True)
        RefreshDerivedUiState()
        IngestStatus = "Vault refreshed"
        IngestDetail = RepairStatus
    End Sub

    Private Sub SetScope(scope As String)
        ActiveScope = scope
        ActionStatus = $"Showing {ActiveScope}"
    End Sub

    Private Sub ApplySort(propertyName As String)
        If FilteredArtifacts Is Nothing Then
            Return
        End If

        FilteredArtifacts.SortDescriptions.Clear()
        Dim direction = If(propertyName = NameOf(ArtifactModel.DateModified), ListSortDirection.Descending, ListSortDirection.Ascending)
        FilteredArtifacts.SortDescriptions.Add(New SortDescription(propertyName, direction))
        FilteredArtifacts.Refresh()
        ActionStatus = $"Sorted by {propertyName}"
    End Sub

    Private Sub ToggleDensity()
        ArtifactRowHeight = If(ArtifactRowHeight <= 30, 34, 28)
        ActionStatus = If(ArtifactRowHeight <= 30, "Compact table rows", "Comfortable table rows")
    End Sub

    Private Sub CycleColumnPreset()
        _columnPresetIndex = (_columnPresetIndex + 1) Mod 3
        OnPropertyChanged(NameOf(ColumnPresetText))
        OnPropertyChanged(NameOf(ShowCategoryColumn))
        OnPropertyChanged(NameOf(ShowDateColumn))
        OnPropertyChanged(NameOf(ShowTagsColumn))
        SaveUiPreferences()
        ActionStatus = $"Table columns set to {ColumnPresetName}"
    End Sub

    Private ReadOnly Property ColumnPresetName As String
        Get
            Select Case _columnPresetIndex
                Case 1
                    Return "Compact"
                Case 2
                    Return "Minimal"
                Case Else
                    Return "Full"
            End Select
        End Get
    End Property

    Private Shared Function ParseColumnPreset(value As String) As Integer
        Select Case If(value, "").Trim().ToLowerInvariant()
            Case "compact"
                Return 1
            Case "minimal"
                Return 2
            Case Else
                Return 0
        End Select
    End Function

    Private Shared Function NormalizeScope(value As String) As String
        Select Case If(value, "").Trim().ToLowerInvariant()
            Case "recent"
                Return "Recent"
            Case "starred"
                Return "Starred"
            Case "quarantine"
                Return "Quarantine"
            Case Else
                Return "All"
        End Select
    End Function

    Private Sub SaveUiPreferences()
        If _isLoadingCatalog OrElse _catalog Is Nothing Then
            Return
        End If

        _catalog.TableDensity = If(ArtifactRowHeight <= 30, "Compact", "Comfortable")
        _catalog.ColumnPreset = ColumnPresetName
        _catalog.ActiveScope = ActiveScope
        _catalog.SearchText = SearchText
        _catalog.TagSearchText = TagSearchText
        _catalog.SelectedTag = If(SelectedTag, "")
        _catalog.SelectedCategory = If(SelectedCategory?.Name, "")
        _catalogService.Save(_catalog)
    End Sub

    Private Function StoredFileExists() As Boolean
        Return SelectedArtifact IsNot Nothing AndAlso
            Not String.IsNullOrWhiteSpace(SelectedArtifact.Path) AndAlso
            File.Exists(SelectedArtifact.Path)
    End Function

    Public Sub SetVaultRoot(path As String)
        If String.IsNullOrWhiteSpace(path) Then
            Return
        End If

        CatalogService.EnsureVaultFolders(path)
        _catalog.VaultRootPath = path
        If CurrentVault IsNot Nothing Then
            CurrentVault.Path = path
            If String.IsNullOrWhiteSpace(CurrentVault.Name) Then
                CurrentVault.Name = "MainVault"
            End If
        Else
            Dim newVault = New VaultModel With {
                .Id = Guid.NewGuid().ToString("N"),
                .Name = "MainVault",
                .Path = path,
                .IsSelected = True
            }
            Vaults.Add(newVault)
            CurrentVault = newVault
        End If
        _catalog.CurrentVaultId = CurrentVault.Id
        _catalog.Vaults = Vaults.ToList()
        _catalogService.Save(_catalog)
        OnPropertyChanged(NameOf(VaultRootPath))
        OnPropertyChanged(NameOf(CurrentVaultTitle))
        OnPropertyChanged(NameOf(StorageTotalText))
        RefreshVaultState()
        ActionStatus = $"Vault set to {path}"
    End Sub

    Private Sub RemoveCurrentVault()
        If CurrentVault Is Nothing Then
            Return
        End If

        If Vaults.Count <= 1 Then
            ActionStatus = "Keep at least one vault"
            Return
        End If

        Dim removed = CurrentVault
        Dim removedIndex = Vaults.IndexOf(removed)
        Vaults.Remove(removed)

        Dim nextIndex = Math.Min(Math.Max(removedIndex, 0), Vaults.Count - 1)
        CurrentVault = Vaults(nextIndex)
        _catalog.CurrentVaultId = CurrentVault.Id
        _catalog.VaultRootPath = CurrentVault.Path
        _catalog.Vaults = Vaults.ToList()
        _catalogService.Save(_catalog)

        OnPropertyChanged(NameOf(VaultRootPath))
        OnPropertyChanged(NameOf(CurrentVaultTitle))
        OnPropertyChanged(NameOf(StorageTotalText))
        RefreshVaultState()
        ActionStatus = $"Removed vault {removed.DisplayName}"
    End Sub

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
        RefreshTagFilter()
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

        If Not MatchesActiveScope(artifact) Then
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
            ContainsText(artifact.TypeFamily, needle) OrElse
            ContainsText(artifact.Category, needle) OrElse
            ContainsText(artifact.Path, needle) OrElse
            ContainsText(artifact.OriginalPath, needle) OrElse
            ContainsText(artifact.Notes, needle) OrElse
            ContainsText(artifact.TagsText, needle) OrElse
            ContainsText(artifact.Sha256, needle) OrElse
            ContainsText(artifact.Blake3, needle) OrElse
            ContainsExtractedText(artifact, needle)
    End Function

    Private Shared Function ContainsText(value As String, needle As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(value) AndAlso
            value.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0
    End Function

    Private Function ContainsExtractedText(artifact As ArtifactModel, needle As String) As Boolean
        If artifact Is Nothing OrElse String.IsNullOrWhiteSpace(artifact.ExtractedTextRelativePath) Then
            Return False
        End If

        Try
            Dim extractedPath = Path.Combine(VaultRootPath, artifact.ExtractedTextRelativePath)
            If Not File.Exists(extractedPath) Then
                Return False
            End If

            Return File.ReadLines(extractedPath).
                Any(Function(line) line.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0)
        Catch
            Return False
        End Try
    End Function

    Private Function FilterTag(item As Object) As Boolean
        Dim tag = TryCast(item, String)
        If String.IsNullOrWhiteSpace(tag) Then
            Return False
        End If

        If String.IsNullOrWhiteSpace(TagSearchText) Then
            Return True
        End If

        Return tag.IndexOf(TagSearchText.Trim(), StringComparison.OrdinalIgnoreCase) >= 0
    End Function

    Private Sub RefreshTagFilter()
        If FilteredTags Is Nothing Then
            Return
        End If

        Dim previous = SelectedTag
        FilteredTags.Refresh()

        If Not String.IsNullOrWhiteSpace(previous) AndAlso Not FilteredTags.Cast(Of String)().Contains(previous, StringComparer.OrdinalIgnoreCase) Then
            SelectedTag = Nothing
        End If
    End Sub

    Private Sub RefreshFilters(Optional preserveSelection As Boolean = False)
        If FilteredArtifacts Is Nothing Then
            Return
        End If

        Dim previous = If(preserveSelection, SelectedArtifact, Nothing)
        FilteredArtifacts.Refresh()
        OnPropertyChanged(NameOf(FilterTitle))

        If previous IsNot Nothing AndAlso FilteredArtifacts.Cast(Of ArtifactModel)().Contains(previous) Then
            SelectedArtifact = previous
        Else
            SelectFirstFilteredArtifact()
        End If
    End Sub

    Private Sub SelectFirstFilteredArtifact()
        If FilteredArtifacts Is Nothing Then
            SelectedArtifact = Artifacts.FirstOrDefault()
            Return
        End If

        SelectedArtifact = FilteredArtifacts.Cast(Of ArtifactModel)().FirstOrDefault()
    End Sub

    Private Sub ClearFilters()
        ActiveScope = "All"
        SelectedCategory = Nothing
        SelectedTag = Nothing
        SearchText = ""
        TagSearchText = ""
        RefreshFilters(preserveSelection:=True)
    End Sub

    Private Sub RefreshDerivedUiState()
        OnPropertyChanged(NameOf(CurrentVaultSummary))
        OnPropertyChanged(NameOf(StorageUsedText))
        OnPropertyChanged(NameOf(StorageTotalText))
        OnPropertyChanged(NameOf(InboxCountText))
        OnPropertyChanged(NameOf(StarredCountText))
        OnPropertyChanged(NameOf(QuarantineCountText))
        OnPropertyChanged(NameOf(IngestModeText))
        OnPropertyChanged(NameOf(IngestModeDetail))
        OnPropertyChanged(NameOf(ActiveScopeText))
        OnPropertyChanged(NameOf(LastBackupDisplay))
        OnPropertyChanged(NameOf(RecallStatusText))
        RebuildStats()
        RebuildRelatedArtifacts()
    End Sub

    Private Sub RebuildRelatedArtifacts()
        RelatedArtifacts.Clear()

        If SelectedArtifact Is Nothing Then
            OnPropertyChanged(NameOf(RelatedArtifactsSummary))
            Return
        End If

        Dim related = Artifacts.
            Where(Function(a) Not ReferenceEquals(a, SelectedArtifact)).
            Select(Function(a) BuildArtifactRelation(SelectedArtifact, a)).
            Where(Function(relation) relation IsNot Nothing AndAlso relation.Score > 0).
            OrderByDescending(Function(relation) relation.Score).
            ThenBy(Function(relation) relation.Name).
            Take(6).
            ToList()

        For Each relation In related
            RelatedArtifacts.Add(relation)
        Next

        OnPropertyChanged(NameOf(RelatedArtifactsSummary))
    End Sub

    Public Shared Function BuildArtifactRelation(selected As ArtifactModel, candidate As ArtifactModel) As ArtifactRelationModel
        If selected Is Nothing OrElse candidate Is Nothing OrElse ReferenceEquals(selected, candidate) Then
            Return Nothing
        End If

        Dim score = 0
        Dim reasons As New List(Of String)

        If Not String.IsNullOrWhiteSpace(candidate.Sha256) AndAlso
            String.Equals(candidate.Sha256, selected.Sha256, StringComparison.OrdinalIgnoreCase) Then
            score += 12
            reasons.Add("duplicate SHA-256")
        End If

        Dim sharedTags = SharedTagNames(selected, candidate)
        If sharedTags.Count > 0 Then
            score += Math.Min(6, sharedTags.Count * 2)
            reasons.Add("shared tag: " & String.Join(", ", sharedTags.Take(3)))
        End If

        If SameText(candidate.Category, selected.Category) Then
            score += 4
            reasons.Add("same category")
        End If

        If SameText(candidate.TypeFamily, selected.TypeFamily) Then
            score += 3
            reasons.Add("same type family")
        End If

        If SameDirectory(candidate.OriginalPath, selected.OriginalPath) Then
            score += 3
            reasons.Add("same original folder")
        ElseIf SameDirectory(candidate.Path, selected.Path) Then
            score += 2
            reasons.Add("same vault folder")
        End If

        If SameDateBatch(candidate, selected) Then
            score += 2
            reasons.Add("same date batch")
        End If

        Dim matchingNameTokens = SharedNameTokens(selected.Name, candidate.Name)
        If matchingNameTokens.Count > 0 Then
            score += Math.Min(4, matchingNameTokens.Count)
            reasons.Add("matching name token: " & String.Join(", ", matchingNameTokens.Take(3)))
        End If

        If score <= 0 Then
            Return Nothing
        End If

        Return New ArtifactRelationModel With {
            .Artifact = candidate,
            .Score = score,
            .Reasons = reasons
        }
    End Function

    Private Shared Function SharedTagNames(selected As ArtifactModel, candidate As ArtifactModel) As List(Of String)
        Dim selectedTags = If(selected.Tags, New List(Of String))
        Dim candidateTags = If(candidate.Tags, New List(Of String))

        Return candidateTags.
            Where(Function(tag) selectedTags.Any(Function(selectedTag) SameText(selectedTag, tag))).
            Distinct(StringComparer.OrdinalIgnoreCase).
            OrderBy(Function(tag) tag).
            ToList()
    End Function

    Private Shared Function SameText(left As String, right As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(left) AndAlso
            String.Equals(left, right, StringComparison.OrdinalIgnoreCase)
    End Function

    Private Shared Function SameDirectory(leftPath As String, rightPath As String) As Boolean
        If String.IsNullOrWhiteSpace(leftPath) OrElse String.IsNullOrWhiteSpace(rightPath) Then
            Return False
        End If

        Try
            Dim leftDirectory = Path.GetDirectoryName(Path.GetFullPath(leftPath))
            Dim rightDirectory = Path.GetDirectoryName(Path.GetFullPath(rightPath))
            Return Not String.IsNullOrWhiteSpace(leftDirectory) AndAlso
                String.Equals(leftDirectory, rightDirectory, StringComparison.OrdinalIgnoreCase)
        Catch
            Return False
        End Try
    End Function

    Private Shared Function SameDateBatch(candidate As ArtifactModel, selected As ArtifactModel) As Boolean
        Dim candidateDate As DateTime
        Dim selectedDate As DateTime

        If TryGetBatchDate(candidate, candidateDate) AndAlso TryGetBatchDate(selected, selectedDate) Then
            Return candidateDate.Date = selectedDate.Date
        End If

        Return False
    End Function

    Private Shared Function TryGetBatchDate(artifact As ArtifactModel, ByRef parsed As DateTime) As Boolean
        If artifact Is Nothing Then
            Return False
        End If

        Return DateTime.TryParse(artifact.IngestedAt, parsed) OrElse DateTime.TryParse(artifact.DateModified, parsed)
    End Function

    Private Shared Function SharedNameTokens(leftName As String, rightName As String) As List(Of String)
        Dim leftTokens = NameTokens(leftName)
        Dim rightTokens = NameTokens(rightName)

        Return rightTokens.
            Where(Function(token) leftTokens.Contains(token, StringComparer.OrdinalIgnoreCase)).
            Distinct(StringComparer.OrdinalIgnoreCase).
            OrderBy(Function(token) token).
            ToList()
    End Function

    Private Shared Function NameTokens(name As String) As List(Of String)
        If String.IsNullOrWhiteSpace(name) Then
            Return New List(Of String)
        End If

        Return Path.GetFileNameWithoutExtension(name).
            Split({" "c, "-"c, "_"c, "."c, "("c, ")"c, "["c, "]"c}, StringSplitOptions.RemoveEmptyEntries).
            Select(Function(token) token.Trim().ToLowerInvariant()).
            Where(Function(token) token.Length >= 3 AndAlso Not IsNumericToken(token)).
            Distinct(StringComparer.OrdinalIgnoreCase).
            ToList()
    End Function

    Private Shared Function IsNumericToken(token As String) As Boolean
        Dim ignored As Integer
        Return Integer.TryParse(token, ignored)
    End Function

    Private Function MatchesActiveScope(artifact As ArtifactModel) As Boolean
        Select Case ActiveScope
            Case "Recent"
                Return IsRecentArtifact(artifact)
            Case "Starred"
                Return artifact.IsStarred
            Case "Quarantine"
                Return String.Equals(artifact.Category, "Quarantine", StringComparison.OrdinalIgnoreCase) OrElse
                    (Not String.IsNullOrWhiteSpace(artifact.RelativePath) AndAlso artifact.RelativePath.StartsWith("quarantine", StringComparison.OrdinalIgnoreCase))
            Case Else
                Return True
        End Select
    End Function

    Private Shared Function IsRecentArtifact(artifact As ArtifactModel) As Boolean
        If artifact Is Nothing Then
            Return False
        End If

        Dim parsed As DateTime
        If DateTime.TryParse(artifact.IngestedAt, parsed) Then
            Return parsed >= DateTime.Now.AddDays(-14)
        End If

        If DateTime.TryParse(artifact.DateModified, parsed) Then
            Return parsed >= DateTime.Now.AddDays(-14)
        End If

        Return False
    End Function

    Private Sub HydrateArtifacts(artifacts As List(Of ArtifactModel))
        If artifacts Is Nothing Then
            Return
        End If

        For Each artifact In artifacts
            If String.IsNullOrWhiteSpace(artifact.Id) Then
                artifact.Id = Guid.NewGuid().ToString("N")
            End If

            If String.IsNullOrWhiteSpace(artifact.TypeFamily) Then
                artifact.TypeFamily = InferTypeFamilyFromCategory(artifact.Category)
            End If

            If artifact.SizeBytes = 0 AndAlso Not String.IsNullOrWhiteSpace(artifact.Path) AndAlso File.Exists(artifact.Path) Then
                artifact.SizeBytes = New FileInfo(artifact.Path).Length
            End If

            If String.IsNullOrWhiteSpace(artifact.RelativePath) AndAlso Not String.IsNullOrWhiteSpace(artifact.Path) Then
                Try
                    artifact.RelativePath = Path.GetRelativePath(VaultRootPath, artifact.Path)
                Catch
                End Try
            End If

            If String.IsNullOrWhiteSpace(artifact.ExtractedTextStatus) Then
                artifact.ExtractedTextStatus = If(String.IsNullOrWhiteSpace(artifact.ExtractedTextRelativePath), "Not extracted", "Extracted")
            End If

            If String.IsNullOrWhiteSpace(artifact.ThumbnailStatus) Then
                artifact.ThumbnailStatus = If(String.IsNullOrWhiteSpace(artifact.ThumbnailRelativePath), ThumbnailService.NotApplicableStatus, ThumbnailService.GeneratedStatus)
            End If
        Next
    End Sub

    Private Shared Function InferTypeFamilyFromCategory(category As String) As String
        Select Case category
            Case "Images"
                Return "Image"
            Case "Documents", "Manifests / Config"
                Return "Text"
            Case "Audio"
                Return "Audio"
            Case "Video"
                Return "Video"
            Case "Archives"
                Return "Archive"
            Case "Software / Installers"
                Return "Installer"
            Case "ISOs / Disk Images"
                Return "Disk Image"
            Case Else
                Return "File"
        End Select
    End Function

    Private Sub RebuildStats()
        Dim largeObjects = Artifacts.Where(Function(a) a.SizeBytes >= 1024L * 1024L * 1024L).Count()
        Dim indexed = Artifacts.Where(Function(a) Not String.IsNullOrWhiteSpace(a.Sha256) OrElse Not String.IsNullOrWhiteSpace(a.Blake3)).Count()
        Dim rebuilt = New List(Of StatCardModel) From {
            New StatCardModel With {.Label = "Total Items", .Value = Artifacts.Count.ToString("N0"), .Icon = "", .IconBrush = "#4DA3FF", .IconBackground = "#153C67"},
            New StatCardModel With {.Label = "Vault Size", .Value = FormatSize(Artifacts.Sum(Function(a) a.SizeBytes)), .Icon = "", .IconBrush = "#9566FF", .IconBackground = "#2C2356"},
            New StatCardModel With {.Label = "Indexed", .Value = indexed.ToString("N0"), .Icon = "", .IconBrush = "#55D680", .IconBackground = "#183C2B"},
            New StatCardModel With {.Label = "Large Objects", .Value = largeObjects.ToString("N0"), .Icon = "", .IconBrush = "#F5A623", .IconBackground = "#4A3315"},
            New StatCardModel With {.Label = "In Quarantine", .Value = QuarantineCountText, .Icon = "", .IconBrush = "#F24F5F", .IconBackground = "#4A1F29"}
        }
        ReplaceCollection(Stats, rebuilt)
        If _catalog IsNot Nothing Then
            _catalog.Stats = rebuilt
        End If
    End Sub

    Private Function BuildRepairReport(adoptOrphans As Boolean) As VaultRepairReport
        Dim healthReport = BuildVaultHealthReport()
        Dim missingArtifacts = Artifacts.Where(Function(a) String.IsNullOrWhiteSpace(a.Path) OrElse Not File.Exists(a.Path)).ToList()
        Dim duplicateGroups = Artifacts.
            Where(Function(a) Not String.IsNullOrWhiteSpace(a.Sha256)).
            GroupBy(Function(a) a.Sha256, StringComparer.OrdinalIgnoreCase).
            Where(Function(g) g.Count() > 1).
            ToList()
        Dim report As New VaultRepairReport With {
            .MissingFiles = missingArtifacts.Count,
            .DuplicateHashGroups = duplicateGroups.Count,
            .MissingSamples = missingArtifacts.Select(Function(a) a.Name).Take(3).ToList(),
            .DuplicateSamples = duplicateGroups.Select(Function(g) g.First().Name).Take(3).ToList()
        }

        Dim missingThumbnailArtifacts = healthReport.Findings.
            Where(Function(finding) String.Equals(finding.FindingType, "Missing thumbnail", StringComparison.OrdinalIgnoreCase)).
            Select(Function(finding) Artifacts.FirstOrDefault(Function(artifact) String.Equals(artifact.Name, finding.Subject, StringComparison.OrdinalIgnoreCase))).
            Where(Function(artifact) artifact IsNot Nothing).
            ToList()
        report.MissingThumbnails = missingThumbnailArtifacts.Count
        report.ThumbnailSamples = missingThumbnailArtifacts.Select(Function(a) a.Name).Take(3).ToList()

        If adoptOrphans Then
            For Each artifact In missingThumbnailArtifacts
                Dim thumbnail = _thumbnailService.GenerateForArtifact(artifact, VaultRootPath)
                artifact.ThumbnailRelativePath = thumbnail.RelativePath
                artifact.ThumbnailStatus = thumbnail.Status
                If String.Equals(thumbnail.Status, ThumbnailService.GeneratedStatus, StringComparison.OrdinalIgnoreCase) Then
                    report.RegeneratedThumbnails += 1
                End If
            Next
        End If

        Dim orphanFiles = FindOrphanStoredFiles().ToList()
        report.OrphanFiles = orphanFiles.Count
        report.OrphanSamples = orphanFiles.Select(Function(orphanPath) Path.GetFileName(orphanPath)).Take(3).ToList()

        If adoptOrphans Then
            For Each orphan In orphanFiles
                Try
                    Dim adopted = _ingestionService.CreateArtifactFromStoredFile(orphan, VaultRootPath)
                    adopted.Notes = $"Adopted during vault rescan at {DateTime.Now:yyyy-MM-dd HH:mm}"
                    Artifacts.Insert(0, adopted)
                    _catalog.Artifacts.Insert(0, adopted)
                    report.AdoptedFiles += 1
                Catch
                End Try
            Next

            If report.AdoptedFiles > 0 Then
                Dim activity = New ActivityEntryModel With {
                    .ActionText = $"Adopted {report.AdoptedFiles:N0} orphan file(s)",
                    .DetailText = $"{DateTime.Now:yyyy-MM-dd HH:mm}  •  vault rescan",
                    .Icon = ""
                }
                Activities.Insert(0, activity)
                _catalog.Activities.Insert(0, activity)
            End If
        End If

        Return report
    End Function

    Public Function BuildVaultHealthReport() As VaultHealthReport
        Return BuildVaultHealthReport(Artifacts, VaultRootPath, _thumbnailService)
    End Function

    Public Shared Function BuildVaultHealthReport(artifacts As IEnumerable(Of ArtifactModel), vaultRootPath As String, thumbnailService As ThumbnailService) As VaultHealthReport
        Dim report As New VaultHealthReport()
        Dim artifactList = If(artifacts, Enumerable.Empty(Of ArtifactModel)()).ToList()
        Dim thumbService = If(thumbnailService, New ThumbnailService())

        For Each artifact In artifactList
            If artifact Is Nothing Then
                Continue For
            End If

            If String.IsNullOrWhiteSpace(artifact.Path) OrElse Not File.Exists(artifact.Path) Then
                report.Findings.Add(New VaultHealthFinding With {
                    .FindingType = "Missing file",
                    .Subject = artifact.Name,
                    .Detail = "Catalog entry points to a stored file that is not present.",
                    .ProposedAction = "Keep catalog row and mark for operator review.",
                    .RiskLevel = "Medium",
                    .MutatesCatalog = False,
                    .TouchesRetainedFiles = False
                })
            End If

            If String.IsNullOrWhiteSpace(artifact.Blake3) OrElse String.IsNullOrWhiteSpace(artifact.Sha256) Then
                report.Findings.Add(New VaultHealthFinding With {
                    .FindingType = "Missing hash",
                    .Subject = artifact.Name,
                    .Detail = "Artifact is missing one or more integrity hashes.",
                    .ProposedAction = "Recompute hashes from the retained file.",
                    .RiskLevel = "Low",
                    .MutatesCatalog = True,
                    .TouchesRetainedFiles = False
                })
            End If

            If thumbService.IsGeneratedThumbnailMissing(artifact, vaultRootPath) Then
                report.Findings.Add(New VaultHealthFinding With {
                    .FindingType = "Missing thumbnail",
                    .Subject = artifact.Name,
                    .Detail = "Catalog references a generated thumbnail file that is not present.",
                    .ProposedAction = "Regenerate thumbnail from the retained file.",
                    .RiskLevel = "Low",
                    .MutatesCatalog = True,
                    .TouchesRetainedFiles = False
                })
            End If

            If Not String.IsNullOrWhiteSpace(artifact.ExtractedTextRelativePath) Then
                Dim extractedPath = ResolveVaultRelativePath(vaultRootPath, artifact.ExtractedTextRelativePath)
                If String.IsNullOrWhiteSpace(extractedPath) OrElse Not File.Exists(extractedPath) Then
                    report.Findings.Add(New VaultHealthFinding With {
                        .FindingType = "Missing extracted text",
                        .Subject = artifact.Name,
                        .Detail = "Catalog references an extracted-text index that is not present.",
                        .ProposedAction = "Re-extract text if the retained file format is supported.",
                        .RiskLevel = "Medium",
                        .MutatesCatalog = True,
                        .TouchesRetainedFiles = False
                    })
                End If
            End If
        Next

        Dim duplicateGroups = artifactList.
            Where(Function(artifact) artifact IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(artifact.Sha256)).
            GroupBy(Function(artifact) artifact.Sha256, StringComparer.OrdinalIgnoreCase).
            Where(Function(group) group.Count() > 1)

        For Each duplicateGroup In duplicateGroups
            report.Findings.Add(New VaultHealthFinding With {
                .FindingType = "Duplicate hash",
                .Subject = duplicateGroup.First().Sha256,
                .Detail = String.Join(", ", duplicateGroup.Select(Function(artifact) artifact.Name).Take(5)),
                .ProposedAction = "Review duplicate candidates; keep or remove only by operator decision.",
                .RiskLevel = "Low",
                .MutatesCatalog = False,
                .TouchesRetainedFiles = False
            })
        Next

        Return report
    End Function

    Private Shared Function ResolveVaultRelativePath(vaultRootPath As String, relativePath As String) As String
        If String.IsNullOrWhiteSpace(relativePath) Then
            Return ""
        End If

        If Path.IsPathRooted(relativePath) Then
            Return relativePath
        End If

        If String.IsNullOrWhiteSpace(vaultRootPath) Then
            Return ""
        End If

        Return Path.Combine(vaultRootPath, relativePath)
    End Function

    Private Shared Function BuildRepairStatus(report As VaultRepairReport) As String
        Dim status = report.Summary
        Dim detail = report.Detail

        If Not String.IsNullOrWhiteSpace(detail) Then
            status &= $"  |  {detail}"
        End If

        Return status
    End Function

    Private Function FindOrphanStoredFiles() As IEnumerable(Of String)
        Dim itemsRoot = Path.Combine(VaultRootPath, "items")
        If Not Directory.Exists(itemsRoot) Then
            Return Enumerable.Empty(Of String)()
        End If

        Dim knownPaths = New HashSet(Of String)(
            Artifacts.
                Where(Function(a) Not String.IsNullOrWhiteSpace(a.Path)).
                Select(Function(a) Path.GetFullPath(a.Path)),
            StringComparer.OrdinalIgnoreCase)

        Return Directory.
            EnumerateFiles(itemsRoot, "*", SearchOption.AllDirectories).
            Where(Function(candidatePath) Not knownPaths.Contains(Path.GetFullPath(candidatePath))).
            ToList()
    End Function

    Private Shared Function BuildUniquePath(directoryPath As String, fileName As String) As String
        Dim baseName = Path.GetFileNameWithoutExtension(fileName)
        Dim extension = Path.GetExtension(fileName)
        Dim candidate = Path.Combine(directoryPath, fileName)
        Dim index = 1

        While File.Exists(candidate)
            candidate = Path.Combine(directoryPath, $"{baseName}-{index}{extension}")
            index += 1
        End While

        Return candidate
    End Function

    Private Shared Function FormatSize(bytes As Long) As String
        Dim units = {"B", "KB", "MB", "GB", "TB"}
        Dim value = CDbl(Math.Max(0, bytes))
        Dim unitIndex = 0

        While value >= 1024 AndAlso unitIndex < units.Length - 1
            value /= 1024
            unitIndex += 1
        End While

        If unitIndex = 0 Then
            Return $"{bytes} B"
        End If

        Return $"{value:0.##} {units(unitIndex)}"
    End Function

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
