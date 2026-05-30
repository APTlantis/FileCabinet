Imports System.Text.Json
Imports FileCabinet

Public Class CliJsonOutput
    Private Shared ReadOnly Options As New JsonSerializerOptions With {
        .WriteIndented = True
    }

    Public Shared Function Serialize(value As Object) As String
        Return JsonSerializer.Serialize(value, Options)
    End Function

    Public Shared Function Ingest(result As HeadlessIngestResult) As String
        Return Serialize(New With {
            .command = "ingest",
            .mode = result.Mode.ToString(),
            .ingestedCount = result.IngestedArtifacts.Count,
            .artifacts = result.IngestedArtifacts.Select(Function(artifact) New With {
                .id = artifact.Id,
                .name = artifact.Name,
                .path = artifact.Path,
                .relativePath = artifact.RelativePath,
                .sha256 = artifact.Sha256,
                .blake3 = artifact.Blake3
            }).ToList()
        })
    End Function

    Public Shared Function Verify(result As HeadlessVerifyResult) As String
        Return Serialize(New With {
            .command = "verify",
            .exitCode = result.ExitCode,
            .summary = result.HealthReport.Summary,
            .findings = result.HealthReport.Findings,
            .repair = result.RepairReport
        })
    End Function

    Public Shared Function Search(results As List(Of ArtifactModel)) As String
        Return Serialize(New With {
            .command = "search",
            .count = results.Count,
            .artifacts = results.Select(Function(artifact) New With {
                .id = artifact.Id,
                .name = artifact.Name,
                .category = artifact.Category,
                .type = artifact.Type,
                .path = artifact.Path,
                .relativePath = artifact.RelativePath,
                .tags = artifact.Tags,
                .sha256 = artifact.Sha256,
                .blake3 = artifact.Blake3
            }).ToList()
        })
    End Function

    Public Shared Function ExportSnapshot(result As HeadlessExportResult) As String
        Return Serialize(New With {
            .command = "export",
            .backupPath = result.Validation.BackupPath,
            .isValid = result.Validation.IsValid,
            .detail = result.Validation.Detail
        })
    End Function

    Public Shared Function Report(result As HeadlessReportResult) As String
        Return Serialize(New With {
            .command = "report",
            .outputPath = result.OutputPath,
            .summary = result.HealthReport.Summary,
            .findingCount = result.HealthReport.FindingCount
        })
    End Function

    Public Shared Function RepairPreview(result As HeadlessRepairPreviewResult) As String
        Return Serialize(New With {
            .command = "repair-preview",
            .summary = result.HealthReport.Summary,
            .candidates = result.RepairCandidates.Select(Function(candidate) New With {
                .findingType = candidate.FindingType,
                .subject = candidate.Subject,
                .actionType = candidate.ActionType,
                .canRepairAutomatically = candidate.CanRepairAutomatically,
                .requiresOperatorApproval = candidate.RequiresOperatorApproval,
                .proposedAction = candidate.ProposedAction
            }).ToList()
        })
    End Function

    Public Shared Function Repair(result As HeadlessRepairApplyResult) As String
        Return Serialize(New With {
            .command = "repair",
            .applied = result.Applied,
            .appliedCount = result.AppliedCount,
            .failedCount = result.FailedCount,
            .skippedCount = result.SkippedCount,
            .candidates = result.Candidates
        })
    End Function

    Public Shared Function Rescan(result As HeadlessRescanResult) As String
        Return Serialize(New With {
            .command = "rescan",
            .applied = result.Applied,
            .orphanCount = result.OrphanFiles.Count,
            .adoptedCount = result.AdoptedArtifacts.Count,
            .orphans = result.OrphanFiles,
            .adopted = result.AdoptedArtifacts.Select(Function(artifact) New With {.id = artifact.Id, .name = artifact.Name, .path = artifact.Path}).ToList()
        })
    End Function

    Public Shared Function RebuildThumbnails(result As HeadlessThumbnailRebuildResult) As String
        Return Serialize(New With {
            .command = "rebuild-thumbnails",
            .applied = result.Applied,
            .candidateCount = result.CandidateCount,
            .rebuiltCount = result.RebuiltCount,
            .failedCount = result.FailedCount
        })
    End Function

    Public Shared Function Package(result As HeadlessPackageResult) As String
        Return Serialize(New With {
            .command = "package",
            .outputPath = result.OutputPath,
            .artifactCount = result.ArtifactCount,
            .fileCount = result.FileCount,
            .isZip = result.IsZip
        })
    End Function
End Class
