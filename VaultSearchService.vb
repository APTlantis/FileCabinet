Imports System.IO

Public Class VaultSearchOptions
    Public Property Query As String = ""
    Public Property Scope As String = "All"
    Public Property Category As String = ""
    Public Property Tag As String = ""
    Public Property Limit As Integer = 50
End Class

Public Class VaultSearchService
    Public Function Search(artifacts As IEnumerable(Of ArtifactModel), vaultRootPath As String, options As VaultSearchOptions) As List(Of ArtifactModel)
        Dim artifactList = If(artifacts, Enumerable.Empty(Of ArtifactModel)()).Where(Function(artifact) artifact IsNot Nothing).ToList()
        Dim normalizedOptions = If(options, New VaultSearchOptions())
        Dim limit = If(normalizedOptions.Limit <= 0, 50, normalizedOptions.Limit)

        Return artifactList.
            Where(Function(artifact) MatchesFilters(artifact, artifactList, vaultRootPath, normalizedOptions)).
            Take(limit).
            ToList()
    End Function

    Private Shared Function MatchesFilters(artifact As ArtifactModel, artifacts As IEnumerable(Of ArtifactModel), vaultRootPath As String, options As VaultSearchOptions) As Boolean
        If Not MainViewModel.ArtifactMatchesDiscoveryScope(artifact, NormalizeScope(options.Scope), artifacts, vaultRootPath) Then
            Return False
        End If

        If Not String.IsNullOrWhiteSpace(options.Category) AndAlso
            Not String.Equals(artifact.Category, options.Category, StringComparison.OrdinalIgnoreCase) Then
            Return False
        End If

        If Not String.IsNullOrWhiteSpace(options.Tag) Then
            If artifact.Tags Is Nothing OrElse Not artifact.Tags.Any(Function(tag) String.Equals(tag, options.Tag, StringComparison.OrdinalIgnoreCase)) Then
                Return False
            End If
        End If

        If String.IsNullOrWhiteSpace(options.Query) Then
            Return True
        End If

        Dim needle = options.Query.Trim()
        Return ContainsText(artifact.Name, needle) OrElse
            ContainsText(artifact.Type, needle) OrElse
            ContainsText(artifact.TypeFamily, needle) OrElse
            ContainsText(artifact.Category, needle) OrElse
            ContainsText(artifact.Path, needle) OrElse
            ContainsText(artifact.OriginalPath, needle) OrElse
            ContainsText(artifact.Notes, needle) OrElse
            ContainsText(artifact.RetentionReason, needle) OrElse
            ContainsText(artifact.WhyThisMatters, needle) OrElse
            ContainsText(artifact.SourceProvenance, needle) OrElse
            ContainsText(artifact.AcquisitionMethod, needle) OrElse
            ContainsText(artifact.TrustClassification, needle) OrElse
            ContainsText(artifact.RetentionPriority, needle) OrElse
            ContainsText(artifact.ArchiveStatus, needle) OrElse
            ContainsText(artifact.TagsText, needle) OrElse
            ContainsAnyHash(artifact, needle) OrElse
            ContainsExtractedText(artifact, vaultRootPath, needle)
    End Function

    Private Shared Function NormalizeScope(scope As String) As String
        Select Case If(scope, "").Trim().ToLowerInvariant()
            Case "", "all"
                Return "All"
            Case "missing-preview"
                Return "Missing preview"
            Case "repair-needed"
                Return "Repair needed"
            Case "duplicate-candidates"
                Return "Duplicate candidates"
            Case "same-source-batch"
                Return "Same source batch"
            Case "large-artifacts"
                Return "Large artifacts"
            Case Else
                Return scope
        End Select
    End Function

    Private Shared Function ContainsText(value As String, needle As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(value) AndAlso value.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0
    End Function

    Private Shared Function ContainsAnyHash(artifact As ArtifactModel, needle As String) As Boolean
        Return HashRegistry.Options.Any(Function(optionItem) ContainsText(HashRegistry.GetArtifactHashValue(artifact, optionItem.Id), needle))
    End Function

    Private Shared Function ContainsExtractedText(artifact As ArtifactModel, vaultRootPath As String, needle As String) As Boolean
        If artifact Is Nothing OrElse String.IsNullOrWhiteSpace(artifact.ExtractedTextRelativePath) OrElse String.IsNullOrWhiteSpace(vaultRootPath) Then
            Return False
        End If

        Try
            Dim extractedPath = If(Path.IsPathRooted(artifact.ExtractedTextRelativePath), artifact.ExtractedTextRelativePath, Path.Combine(vaultRootPath, artifact.ExtractedTextRelativePath))
            If Not File.Exists(extractedPath) Then
                Return False
            End If

            Return File.ReadLines(extractedPath).Any(Function(line) line.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0)
        Catch
            Return False
        End Try
    End Function
End Class
