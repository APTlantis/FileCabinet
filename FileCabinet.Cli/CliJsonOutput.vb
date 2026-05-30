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
End Class
