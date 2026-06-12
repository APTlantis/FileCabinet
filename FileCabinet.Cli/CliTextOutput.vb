Imports System.Text
Imports FileCabinet

Namespace FileCabinet.Cli
    Public Class CliTextOutput
        Public Shared Function Help() As String
        Return String.Join(Environment.NewLine, {
            "FileCabinet CLI",
            "",
            "Commands:",
            "  ingest <path...> [--copy|--move] [--vault <path>] [--catalog <path>] [--json]",
            "  verify [--fail-on any|medium|high] [--vault <path>] [--catalog <path>] [--json]",
            "  search <query> [--scope <name>] [--category <name>] [--tag <tag>] [--limit <n>] [--json]",
            "  export [--output <folder>] [--catalog <path>] [--json]",
            "  report [--output <path>] [--format text|json] [--vault <path>] [--catalog <path>]",
            "  repair-preview [--json]",
            "  repair [--apply --yes] [--json]",
            "  rescan [--apply --yes] [--json]",
            "  rebuild-thumbnails [--apply --yes] [--json]",
            "  package [--output <folder>] [--zip] [--json]",
            "",
            "Global options: --help, --version, --catalog <path>, --vault <path>, --json, --quiet"
        })
        End Function

        Public Shared Function Version() As String
            Dim assemblyVersion = GetType(CliTextOutput).Assembly.GetName().Version
            Return $"FileCabinet.Cli {assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}"
        End Function

        Public Shared Function Ingest(result As HeadlessIngestResult) As String
        Dim builder As New StringBuilder()
        builder.AppendLine($"Ingested {result.IngestedArtifacts.Count:N0} file(s) using {result.Mode}.")
        For Each artifact In result.IngestedArtifacts
            builder.AppendLine($"- {artifact.Id}  {artifact.Name}")
        Next
        Return builder.ToString().TrimEnd()
        End Function

        Public Shared Function Verify(result As HeadlessVerifyResult) As String
        Dim builder As New StringBuilder()
        builder.AppendLine(result.HealthReport.Summary)
        builder.AppendLine(result.RepairReport.Summary)
        For Each finding In result.HealthReport.Findings.Take(20)
            builder.AppendLine($"- [{finding.RiskLevel}] {finding.FindingType}: {finding.Subject}")
        Next
        Return builder.ToString().TrimEnd()
        End Function

        Public Shared Function Search(results As List(Of ArtifactModel)) As String
        Dim builder As New StringBuilder()
        builder.AppendLine($"Found {results.Count:N0} artifact(s).")
        For Each artifact In results
            builder.AppendLine($"- {artifact.Id}  {artifact.Name}  [{artifact.Category}]")
        Next
        Return builder.ToString().TrimEnd()
        End Function

        Public Shared Function ExportSnapshot(result As HeadlessExportResult) As String
        Return If(result.Validation.IsValid,
            $"Catalog backup created and validated: {result.Validation.BackupPath}",
            $"Catalog backup validation failed: {result.Validation.Detail}")
        End Function

        Public Shared Function Report(result As HeadlessReportResult) As String
        Return $"Report written: {result.OutputPath}"
        End Function

        Public Shared Function RepairPreview(result As HeadlessRepairPreviewResult) As String
        Dim builder As New StringBuilder()
        builder.AppendLine(result.HealthReport.Summary)
        builder.AppendLine($"{result.RepairCandidates.Count:N0} repair candidate(s).")
        For Each candidate In result.RepairCandidates.Take(20)
            builder.AppendLine($"- {candidate.ActionType}: {candidate.Subject} ({candidate.FindingType})")
        Next
        Return builder.ToString().TrimEnd()
        End Function

        Public Shared Function Repair(result As HeadlessRepairApplyResult) As String
        Return $"Repair {If(result.Applied, "applied", "preview")}: {result.AppliedCount:N0} applied, {result.FailedCount:N0} failed, {result.SkippedCount:N0} skipped."
        End Function

        Public Shared Function Rescan(result As HeadlessRescanResult) As String
        Return $"Rescan {If(result.Applied, "applied", "preview")}: {result.OrphanFiles.Count:N0} orphan file(s), {result.AdoptedArtifacts.Count:N0} adopted."
        End Function

        Public Shared Function RebuildThumbnails(result As HeadlessThumbnailRebuildResult) As String
        Return $"Thumbnail rebuild {If(result.Applied, "applied", "preview")}: {result.CandidateCount:N0} candidate(s), {result.RebuiltCount:N0} rebuilt, {result.FailedCount:N0} failed."
        End Function

        Public Shared Function Package(result As HeadlessPackageResult) As String
        Return $"Package written: {result.OutputPath}"
        End Function
    End Class
End Namespace
