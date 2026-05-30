Imports System.IO
Imports System.Text
Imports System.Text.Json

Public Class HeadlessOptions
    Public Property CatalogPath As String = ""
    Public Property VaultRootPath As String = ""
End Class

Public Class HeadlessIngestResult
    Public Property IngestedArtifacts As New List(Of ArtifactModel)
    Public Property RequestedPathCount As Integer
    Public Property Mode As IngestMode
    Public ReadOnly Property ExitCode As Integer
        Get
            If RequestedPathCount > IngestedArtifacts.Count Then
                Return 3
            End If

            Return 0
        End Get
    End Property
End Class

Public Class HeadlessVerifyResult
    Public Property HealthReport As VaultHealthReport
    Public Property RepairReport As VaultRepairReport
    Public Property ExitCode As Integer
End Class

Public Class HeadlessExportResult
    Public Property Validation As CatalogBackupValidationResult
End Class

Public Class HeadlessReportResult
    Public Property OutputPath As String = ""
    Public Property HealthReport As VaultHealthReport
    Public Property RepairReport As VaultRepairReport
End Class

Public Class HeadlessRepairPreviewResult
    Public Property HealthReport As VaultHealthReport
    Public Property RepairCandidates As New List(Of RepairCandidate)
End Class

Public Class VaultHeadlessService
    Private ReadOnly _catalogService As CatalogService
    Private ReadOnly _ingestionService As IngestionService
    Private ReadOnly _searchService As VaultSearchService

    Public Sub New(Optional options As HeadlessOptions = Nothing)
        Dim effectiveOptions = If(options, New HeadlessOptions())
        _catalogService = New CatalogService(effectiveOptions.CatalogPath, effectiveOptions.VaultRootPath)
        _ingestionService = New IngestionService()
        _searchService = New VaultSearchService()
    End Sub

    Public ReadOnly Property CatalogPath As String
        Get
            Return _catalogService.CatalogPath
        End Get
    End Property

    Public Function LoadCatalog() As CatalogData
        Dim catalog = _catalogService.LoadOrCreate()
        CatalogService.EnsureVaultFolders(catalog.VaultRootPath)
        Return catalog
    End Function

    Public Function Ingest(paths As IEnumerable(Of String), Optional modeOverride As IngestMode? = Nothing) As HeadlessIngestResult
        Dim catalog = LoadCatalog()
        Dim requestedPaths = If(paths, Enumerable.Empty(Of String)()).Where(Function(path) Not String.IsNullOrWhiteSpace(path)).ToList()
        Dim mode = If(modeOverride.HasValue, modeOverride.Value, ParseIngestMode(catalog.DefaultIngestMode))
        Dim ingested = _ingestionService.Ingest(requestedPaths, catalog.VaultRootPath, Nothing, mode)

        If ingested.Count > 0 Then
            For Each artifact In ingested
                catalog.Artifacts.Insert(0, artifact)
            Next

            catalog.Activities.Insert(0, New ActivityEntryModel With {
                .ActionText = $"CLI ingested {ingested.Count:N0} file(s)",
                .DetailText = $"{DateTime.Now:yyyy-MM-dd HH:mm}  •  {mode.ToString().ToLowerInvariant()} into vault",
                .Icon = "CLI"
            })
            RefreshDerivedCatalogLists(catalog)
            _catalogService.Save(catalog)
        End If

        Return New HeadlessIngestResult With {
            .IngestedArtifacts = ingested,
            .RequestedPathCount = requestedPaths.Count,
            .Mode = mode
        }
    End Function

    Public Function Verify(Optional failOn As String = "any") As HeadlessVerifyResult
        Dim catalog = LoadCatalog()
        Dim healthReport = MainViewModel.BuildVaultHealthReport(catalog.Artifacts, catalog.VaultRootPath, New ThumbnailService(), New HashService())
        Dim repairReport = MainViewModel.BuildRepairReport(catalog.Artifacts, catalog.VaultRootPath, healthReport)
        Dim thresholdMet = FindingsMeetThreshold(healthReport, failOn)

        Return New HeadlessVerifyResult With {
            .HealthReport = healthReport,
            .RepairReport = repairReport,
            .ExitCode = If(thresholdMet, 2, 0)
        }
    End Function

    Public Function Search(options As VaultSearchOptions) As List(Of ArtifactModel)
        Dim catalog = LoadCatalog()
        Return _searchService.Search(catalog.Artifacts, catalog.VaultRootPath, options)
    End Function

    Public Function ExportSnapshot(outputFolder As String) As HeadlessExportResult
        Dim catalog = LoadCatalog()
        Dim exportRoot = If(String.IsNullOrWhiteSpace(outputFolder), Path.Combine(catalog.VaultRootPath, "exports"), outputFolder)
        Dim validation = _catalogService.ExportSnapshotWithValidation(catalog, exportRoot)
        Return New HeadlessExportResult With {.Validation = validation}
    End Function

    Public Function GenerateReport(outputPath As String, format As String) As HeadlessReportResult
        Dim catalog = LoadCatalog()
        Dim healthReport = MainViewModel.BuildVaultHealthReport(catalog.Artifacts, catalog.VaultRootPath, New ThumbnailService(), New HashService())
        Dim repairReport = MainViewModel.BuildRepairReport(catalog.Artifacts, catalog.VaultRootPath, healthReport)
        Dim normalizedFormat = If(format, "text").Trim().ToLowerInvariant()
        Dim destination = If(String.IsNullOrWhiteSpace(outputPath), BuildDefaultReportPath(catalog.VaultRootPath, normalizedFormat), outputPath)
        Dim directoryPath = Path.GetDirectoryName(destination)

        If Not String.IsNullOrWhiteSpace(directoryPath) Then
            Directory.CreateDirectory(directoryPath)
        End If

        If normalizedFormat = "json" Then
            File.WriteAllText(destination, JsonSerializer.Serialize(New With {
                .generatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                .vaultRoot = catalog.VaultRootPath,
                .summary = healthReport.Summary,
                .findings = healthReport.Findings,
                .repair = repairReport
            }, JsonOptions))
        Else
            File.WriteAllText(destination, BuildTextReport(catalog.VaultRootPath, healthReport, repairReport))
        End If

        Return New HeadlessReportResult With {
            .OutputPath = destination,
            .HealthReport = healthReport,
            .RepairReport = repairReport
        }
    End Function

    Public Function RepairPreview() As HeadlessRepairPreviewResult
        Dim catalog = LoadCatalog()
        Dim healthReport = MainViewModel.BuildVaultHealthReport(catalog.Artifacts, catalog.VaultRootPath, New ThumbnailService(), New HashService())
        Return New HeadlessRepairPreviewResult With {
            .HealthReport = healthReport,
            .RepairCandidates = healthReport.Findings.Select(Function(finding) MainViewModel.BuildRepairCandidate(finding)).ToList()
        }
    End Function

    Private Shared Function ParseIngestMode(value As String) As IngestMode
        Dim parsed As IngestMode
        If [Enum].TryParse(value, ignoreCase:=True, result:=parsed) Then
            Return parsed
        End If

        Return IngestMode.Move
    End Function

    Private Shared Function FindingsMeetThreshold(report As VaultHealthReport, failOn As String) As Boolean
        Dim normalized = If(failOn, "any").Trim().ToLowerInvariant()
        If report Is Nothing OrElse report.Findings.Count = 0 Then
            Return False
        End If

        Select Case normalized
            Case "high"
                Return report.Findings.Any(Function(finding) String.Equals(finding.RiskLevel, "High", StringComparison.OrdinalIgnoreCase))
            Case "medium"
                Return report.Findings.Any(Function(finding) String.Equals(finding.RiskLevel, "Medium", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(finding.RiskLevel, "High", StringComparison.OrdinalIgnoreCase))
            Case Else
                Return report.Findings.Count > 0
        End Select
    End Function

    Private Shared Sub RefreshDerivedCatalogLists(catalog As CatalogData)
        catalog.Categories = catalog.Artifacts.
            Where(Function(artifact) Not String.IsNullOrWhiteSpace(artifact.Category)).
            GroupBy(Function(artifact) artifact.Category, StringComparer.OrdinalIgnoreCase).
            Select(Function(group) New CategoryModel With {.Name = group.First().Category, .Count = group.Count().ToString("N0")}).
            OrderBy(Function(category) category.Name).
            ToList()

        catalog.Tags = catalog.Artifacts.
            Where(Function(artifact) artifact.Tags IsNot Nothing).
            SelectMany(Function(artifact) artifact.Tags).
            Where(Function(tag) Not String.IsNullOrWhiteSpace(tag)).
            Distinct(StringComparer.OrdinalIgnoreCase).
            OrderBy(Function(tag) tag).
            ToList()
    End Sub

    Private Shared Function BuildDefaultReportPath(vaultRootPath As String, format As String) As String
        Dim extension = If(String.Equals(format, "json", StringComparison.OrdinalIgnoreCase), "json", "txt")
        Return Path.Combine(vaultRootPath, "exports", $"vault-health-{DateTime.Now:yyyyMMdd-HHmmss}.{extension}")
    End Function

    Private Shared Function BuildTextReport(vaultRootPath As String, healthReport As VaultHealthReport, repairReport As VaultRepairReport) As String
        Dim builder As New StringBuilder()
        builder.AppendLine("FileCabinet Vault Health Report")
        builder.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}")
        builder.AppendLine($"Vault: {vaultRootPath}")
        builder.AppendLine(healthReport.Summary)
        builder.AppendLine(repairReport.Summary)

        If Not String.IsNullOrWhiteSpace(healthReport.Detail) Then
            builder.AppendLine(healthReport.Detail)
        End If

        For Each finding In healthReport.Findings
            builder.AppendLine($"- [{finding.RiskLevel}] {finding.FindingType}: {finding.Subject} - {finding.ProposedAction}")
        Next

        Return builder.ToString()
    End Function

    Private Shared ReadOnly JsonOptions As New JsonSerializerOptions With {
        .WriteIndented = True
    }
End Class
