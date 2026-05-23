Imports System.IO
Imports System.Text.Json

Public Class CatalogService
    Private Shared ReadOnly CatalogOptions As New JsonSerializerOptions With {
        .WriteIndented = True,
        .PropertyNameCaseInsensitive = True
    }

    Public ReadOnly Property CatalogPath As String
    Public ReadOnly Property DefaultVaultRootPath As String

    Public Sub New(Optional catalogPathOverride As String = "", Optional defaultVaultRootPathOverride As String = "")
        If String.IsNullOrWhiteSpace(catalogPathOverride) Then
            CatalogPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "FileCabinet",
                "catalog.json")
        Else
            CatalogPath = catalogPathOverride
        End If

        If String.IsNullOrWhiteSpace(defaultVaultRootPathOverride) Then
            DefaultVaultRootPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "FileCabinetVault")
        Else
            DefaultVaultRootPath = defaultVaultRootPathOverride
        End If
    End Sub

    Public Function LoadOrCreate() As CatalogData
        Try
            If File.Exists(CatalogPath) Then
                Dim json = File.ReadAllText(CatalogPath)
                Dim loaded = JsonSerializer.Deserialize(Of CatalogData)(json, CatalogOptions)

                If IsUsable(loaded) Then
                    EnsureCatalogDefaults(loaded)
                    Return loaded
                End If
            End If
        Catch
        End Try

        Dim created = CreateEmptyCatalog()
        Save(created)
        Return created
    End Function

    Private Sub EnsureCatalogDefaults(catalog As CatalogData)
        catalog.SchemaVersion = Math.Max(1, catalog.SchemaVersion)

        If String.IsNullOrWhiteSpace(catalog.VaultRootPath) Then
            catalog.VaultRootPath = DefaultVaultRootPath
        End If

        If String.IsNullOrWhiteSpace(catalog.CurrentVaultId) Then
            catalog.CurrentVaultId = "main"
        End If

        If String.IsNullOrWhiteSpace(catalog.DefaultIngestMode) Then
            catalog.DefaultIngestMode = "Move"
        End If

        If String.IsNullOrWhiteSpace(catalog.DuplicatePolicy) Then
            catalog.DuplicatePolicy = "Rename"
        End If

        If catalog.Vaults Is Nothing Then
            catalog.Vaults = New List(Of VaultModel)
        End If

        If catalog.Vaults.Count = 0 Then
            catalog.Vaults.Add(New VaultModel With {
                .Id = catalog.CurrentVaultId,
                .Name = "MainVault",
                .Path = catalog.VaultRootPath,
                .IsSelected = True
            })
        End If

        For Each vault In catalog.Vaults
            If String.IsNullOrWhiteSpace(vault.Path) Then
                vault.Path = catalog.VaultRootPath
            End If
        Next

        EnsureVaultFolders(catalog.VaultRootPath)
        Save(catalog)
    End Sub

    Public Sub Save(catalog As CatalogData)
        Dim catalogDirectory = Path.GetDirectoryName(CatalogPath)

        If Not String.IsNullOrWhiteSpace(catalogDirectory) Then
            Directory.CreateDirectory(catalogDirectory)
        End If

        File.WriteAllText(CatalogPath, JsonSerializer.Serialize(catalog, CatalogOptions))
    End Sub

    Public Function ExportSnapshot(catalog As CatalogData, exportsRoot As String) As String
        If catalog Is Nothing Then
            Throw New ArgumentNullException(NameOf(catalog))
        End If

        Directory.CreateDirectory(exportsRoot)
        catalog.LastBackupPath = Path.Combine(exportsRoot, $"catalog-backup-{DateTime.Now:yyyyMMdd-HHmmss}.json")
        File.WriteAllText(catalog.LastBackupPath, JsonSerializer.Serialize(catalog, CatalogOptions))
        Save(catalog)
        Return catalog.LastBackupPath
    End Function

    Private Shared Function IsUsable(catalog As CatalogData) As Boolean
        Return catalog IsNot Nothing AndAlso
            catalog.Vaults IsNot Nothing AndAlso
            catalog.Artifacts IsNot Nothing
    End Function

    Public Shared Sub EnsureVaultFolders(vaultRootPath As String)
        If String.IsNullOrWhiteSpace(vaultRootPath) Then
            Return
        End If

        Directory.CreateDirectory(vaultRootPath)
        For Each child In {"items", "catalog", "quarantine", "exports", "thumbnails", "extracted-text"}
            Directory.CreateDirectory(Path.Combine(vaultRootPath, child))
        Next
    End Sub

    Private Function CreateEmptyCatalog() As CatalogData
        Dim root = DefaultVaultRootPath
        EnsureVaultFolders(root)

        Return New CatalogData With {
            .SchemaVersion = 1,
            .CurrentVaultId = "main",
            .VaultRootPath = root,
            .DefaultIngestMode = "Move",
            .DuplicatePolicy = "Rename",
            .Vaults = New List(Of VaultModel) From {
                New VaultModel With {.Id = "main", .Name = "MainVault", .Path = root, .IsSelected = True}
            },
            .Activities = New List(Of ActivityEntryModel) From {
                New ActivityEntryModel With {
                    .ActionText = "Created local vault",
                    .DetailText = $"{DateTime.Now:yyyy-MM-dd HH:mm}  •  {root}",
                    .Icon = ""
                }
            }
        }
    End Function

End Class
