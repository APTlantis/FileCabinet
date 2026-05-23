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

    Private Shared Function CreateSeedCatalog() As CatalogData
        Return New CatalogData With {
            .CurrentVaultId = "main",
            .VaultRootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "FileCabinetVault"),
            .Vaults = New List(Of VaultModel) From {
                New VaultModel With {.Id = "main", .Name = "MainVault", .Path = "H:", .IsSelected = True},
                New VaultModel With {.Id = "archive", .Name = "ArchiveVault", .Path = "D:"},
                New VaultModel With {.Id = "cold", .Name = "ColdStorage", .Path = "E:"}
            },
            .Stats = New List(Of StatCardModel) From {
                New StatCardModel With {.Label = "Total Items", .Value = "4,921", .Icon = "", .IconBrush = "#4DA3FF", .IconBackground = "#153C67"},
                New StatCardModel With {.Label = "Vault Size", .Value = "247.8 GB", .Icon = "", .IconBrush = "#9566FF", .IconBackground = "#2C2356"},
                New StatCardModel With {.Label = "Indexed", .Value = "4,712", .Icon = "", .IconBrush = "#55D680", .IconBackground = "#183C2B"},
                New StatCardModel With {.Label = "Large Objects", .Value = "38", .Icon = "", .IconBrush = "#F5A623", .IconBackground = "#4A3315"},
                New StatCardModel With {.Label = "In Quarantine", .Value = "2", .Icon = "", .IconBrush = "#F24F5F", .IconBackground = "#4A1F29"}
            },
            .Categories = New List(Of CategoryModel) From {
                New CategoryModel With {.Name = "Documents", .Count = "1,245"},
                New CategoryModel With {.Name = "Images", .Count = "1,102"},
                New CategoryModel With {.Name = "Audio", .Count = "152"},
                New CategoryModel With {.Name = "Video", .Count = "98"},
                New CategoryModel With {.Name = "Software / Installers", .Count = "374"},
                New CategoryModel With {.Name = "ISOs / Disk Images", .Count = "41"},
                New CategoryModel With {.Name = "Archives", .Count = "268"},
                New CategoryModel With {.Name = "Keys / Security", .Count = "33"},
                New CategoryModel With {.Name = "Manifests / Config", .Count = "213"},
                New CategoryModel With {.Name = "Other", .Count = "1,108"}
            },
            .Tags = New List(Of String) From {"ubuntu", "iso", "pgp", "installer", "toml", "backup", "security", "archive"},
            .Activities = New List(Of ActivityEntryModel) From {
                New ActivityEntryModel With {.ActionText = "Ingested  ubuntu-22.04.4-desktop-amd64.iso", .DetailText = "4.67 GB  •  Just now", .Icon = ""},
                New ActivityEntryModel With {.ActionText = "Indexed  neon-ink-architecture.png", .DetailText = "1.2 MB  •  1 min ago", .Icon = ""},
                New ActivityEntryModel With {.ActionText = "Added  ghostscript-signing-key.asc", .DetailText = "3.2 KB  •  2 min ago", .Icon = "", .IconBrush = "#FFFFFF", .IconBackground = "#61D181"},
                New ActivityEntryModel With {.ActionText = "Extracted text from Crates-Archive-Manifest.toml", .DetailText = "18.7 KB  •  3 min ago", .Icon = "", .IconBrush = "#746AC2", .IconBackground = "#DBD7FF"},
                New ActivityEntryModel With {.ActionText = "Generated thumbnails for 14 images", .DetailText = "4 min ago", .Icon = "", .IconBrush = "#69788D", .IconBackground = "#E8ECF4"}
            },
            .Artifacts = New List(Of ArtifactModel) From {
                New ArtifactModel With {.Name = "ubuntu-22.04.4-desktop-amd64.iso", .Type = "ISO Image", .Category = "ISOs / Disk Images", .Size = "4.67 GB", .DateModified = "2024-11-18 10:42", .Path = "H:\ISOs\ubuntu-22.04.4-desktop-amd64.iso", .Created = "2024-11-18 10:42", .Sha256 = "3f2c...9a7b", .Rating = 4, .IsStarred = True, .Notes = "Ubuntu 22.04.4 LTS Desktop (Jammy Jellyfish)" & vbCrLf & "Official release from ubuntu.com." & vbCrLf & "Used for WSL testing environment.", .Tags = New List(Of String) From {"ubuntu", "linux", "desktop", "os", "iso"}},
                New ArtifactModel With {.Name = "MongoDB-Compass-1.43.4.exe", .Type = "Installer", .Category = "Software / Installers", .Size = "148.3 MB", .DateModified = "2024-11-17 15:21", .Path = "H:\Installers\MongoDB-Compass-1.43.4.exe", .Created = "2024-11-17 15:21", .Sha256 = "8b2a...11de", .Rating = 3, .Notes = "MongoDB Compass installer retained for workstation rebuilds.", .Tags = New List(Of String) From {"mongodb", "compass", "nosql"}},
                New ArtifactModel With {.Name = "ghostscript-signing-key.asc", .Type = "PGP Key", .Category = "Keys / Security", .Size = "3.2 KB", .DateModified = "2024-11-16 09:11", .Path = "H:\Keys\ghostscript-signing-key.asc", .Created = "2024-11-16 09:11", .Sha256 = "cc91...72aa", .Rating = 5, .Notes = "Public signing key used to verify Ghostscript release artifacts.", .Tags = New List(Of String) From {"pgp", "signing", "ghostscript"}},
                New ArtifactModel With {.Name = "Crates-Archive-Manifest.toml", .Type = "TOML Document", .Category = "Manifests / Config", .Size = "18.7 KB", .DateModified = "2024-11-16 21:04", .Path = "H:\Manifests\Crates-Archive-Manifest.toml", .Created = "2024-11-16 21:04", .Sha256 = "a441...02bc", .Rating = 4, .Notes = "Manifest for retained crates archive snapshot.", .Tags = New List(Of String) From {"manifest", "crates", "archive"}},
                New ArtifactModel With {.Name = "neon-ink-panel-system.zip", .Type = "Archive", .Category = "Archives", .Size = "85.1 MB", .DateModified = "2024-11-15 18:33", .Path = "H:\Archives\neon-ink-panel-system.zip", .Created = "2024-11-15 18:33", .Sha256 = "984a...3eee", .Rating = 3, .Notes = "UI panel system archive and generated assets.", .Tags = New List(Of String) From {"neon-ink", "ui", "panels"}},
                New ArtifactModel With {.Name = "IA-Upload-Snapshot-2024-11-15.torrent", .Type = "Torrent", .Category = "Torrents", .Size = "28.6 KB", .DateModified = "2024-11-15 12:07", .Path = "H:\Torrents\IA-Upload-Snapshot-2024-11-15.torrent", .Created = "2024-11-15 12:07", .Sha256 = "53a0...b1cf", .Rating = 4, .Notes = "Torrent metadata for archived upload snapshot.", .Tags = New List(Of String) From {"archive", "internet-archive"}},
                New ArtifactModel With {.Name = "vault-schema-v1.2.json", .Type = "JSON Document", .Category = "Manifests / Config", .Size = "6.1 KB", .DateModified = "2024-11-14 22:19", .Path = "H:\Manifests\vault-schema-v1.2.json", .Created = "2024-11-14 22:19", .Sha256 = "014b...889c", .Rating = 4, .Notes = "Early vault schema draft for metadata experiments.", .Tags = New List(Of String) From {"schema", "vault", "json"}},
                New ArtifactModel With {.Name = "neon-ink-architecture.png", .Type = "Image (PNG)", .Category = "Images", .Size = "1.2 MB", .DateModified = "2024-11-14 17:02", .Path = "H:\Images\neon-ink-architecture.png", .Created = "2024-11-14 17:02", .Sha256 = "bb80...c201", .Rating = 5, .Notes = "Architecture diagram for neon ink panel work.", .Tags = New List(Of String) From {"architecture", "diagram", "ui"}},
                New ArtifactModel With {.Name = "setup.msix", .Type = "MSIX Installer", .Category = "Software / Installers", .Size = "63.4 MB", .DateModified = "2024-11-14 11:33", .Path = "H:\Installers\setup.msix", .Created = "2024-11-14 11:33", .Sha256 = "90ef...21ad", .Rating = 3, .Notes = "MSIX installer retained for rollback testing.", .Tags = New List(Of String) From {"msix", "installer"}},
                New ArtifactModel With {.Name = "private-key-backup.gpg", .Type = "GPG Encrypted File", .Category = "Keys / Security", .Size = "12.4 KB", .DateModified = "2024-11-13 16:44", .Path = "H:\Keys\private-key-backup.gpg", .Created = "2024-11-13 16:44", .Sha256 = "7f24...9d10", .Rating = 5, .Notes = "Encrypted private key backup. Keep offline and verify before use.", .Tags = New List(Of String) From {"pgp", "backup", "private"}}
            }
        }
    End Function
End Class
