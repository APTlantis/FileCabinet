Imports System.IO

Namespace FileCabinet.Tests
    Public NotInheritable Class TestVaultFixtures
        Private Sub New()
        End Sub

        Public Shared Function EmptyVault(workspace As TestWorkspace) As Global.FileCabinet.CatalogData
            Dim service As New Global.FileCabinet.CatalogService(workspace.CatalogPath, workspace.VaultRoot)
            Return service.LoadOrCreate()
        End Function

        Public Shared Function TinyNormalVault(workspace As TestWorkspace) As Global.FileCabinet.CatalogData
            Dim service As New Global.FileCabinet.CatalogService(workspace.CatalogPath, workspace.VaultRoot)
            Dim catalog = service.LoadOrCreate()
            Dim storedPath = workspace.VaultItemPath("manifest.json")
            File.WriteAllText(storedPath, "{""name"":""fixture""}")

            catalog.Artifacts.Add(CompleteArtifact(
                "artifact-001",
                "manifest.json",
                storedPath,
                workspace.VaultRoot,
                "source\manifest.json"))
            service.Save(catalog)
            Return catalog
        End Function

        Public Shared Function DuplicateFileVault(workspace As TestWorkspace) As Global.FileCabinet.CatalogData
            Dim service As New Global.FileCabinet.CatalogService(workspace.CatalogPath, workspace.VaultRoot)
            Dim catalog = service.LoadOrCreate()
            Dim firstPath = workspace.VaultItemPath("duplicate-a.txt")
            Dim secondPath = workspace.VaultItemPath("duplicate-b.txt")
            File.WriteAllText(firstPath, "same retained content")
            File.WriteAllText(secondPath, "same retained content")

            Dim first = CompleteArtifact("artifact-duplicate-a", "duplicate-a.txt", firstPath, workspace.VaultRoot, "source\duplicate-a.txt")
            Dim second = CompleteArtifact("artifact-duplicate-b", "duplicate-b.txt", secondPath, workspace.VaultRoot, "source\duplicate-b.txt")
            second.Sha256 = first.Sha256
            second.Blake3 = first.Blake3
            catalog.Artifacts.Add(first)
            catalog.Artifacts.Add(second)
            service.Save(catalog)
            Return catalog
        End Function

        Public Shared Function MissingFileVault(workspace As TestWorkspace) As Global.FileCabinet.CatalogData
            Dim service As New Global.FileCabinet.CatalogService(workspace.CatalogPath, workspace.VaultRoot)
            Dim catalog = service.LoadOrCreate()
            Dim missingPath = workspace.VaultItemPath("missing.iso")

            catalog.Artifacts.Add(CompleteArtifact(
                "artifact-missing",
                "missing.iso",
                missingPath,
                workspace.VaultRoot,
                "source\missing.iso"))
            service.Save(catalog)
            Return catalog
        End Function

        Public Shared Function CompleteArtifact(id As String, name As String, storedPath As String, vaultRoot As String, originalRelativePath As String) As Global.FileCabinet.ArtifactModel
            Dim hashes As New Global.FileCabinet.FileHashes With {
                .Blake3 = "fixture-blake3",
                .Sha256 = "fixture-sha256"
            }

            If File.Exists(storedPath) Then
                hashes = New Global.FileCabinet.HashService().ComputeHashes(storedPath)
            End If

            Dim sizeBytes As Long = If(File.Exists(storedPath), New FileInfo(storedPath).Length, 1)
            Dim timestamp = "2026-05-23 10:00"

            Return New Global.FileCabinet.ArtifactModel With {
                .Id = id,
                .Name = name,
                .Path = storedPath,
                .RelativePath = Path.GetRelativePath(vaultRoot, storedPath),
                .OriginalPath = Path.Combine(Path.GetDirectoryName(vaultRoot), originalRelativePath),
                .Type = "Text File",
                .TypeFamily = "Text",
                .Category = "Documents",
                .SizeBytes = sizeBytes,
                .Size = $"{sizeBytes} B",
                .Created = timestamp,
                .DateModified = timestamp,
                .IngestedAt = timestamp,
                .Blake3 = hashes.Blake3,
                .Sha256 = hashes.Sha256,
                .HashStatus = "Verified",
                .ThumbnailStatus = Global.FileCabinet.ThumbnailService.NotApplicableStatus,
                .ExtractedTextStatus = "Not extractable",
                .RetentionReason = "Fixture retention reason",
                .WhyThisMatters = "Fixture context",
                .SourceProvenance = "Generated test fixture",
                .AcquisitionMethod = "Generated",
                .TrustClassification = "Trusted",
                .RetentionPriority = "Normal",
                .ArchiveStatus = "Active"
            }
        End Function
    End Class
End Namespace
