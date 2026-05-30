Imports FileCabinet

Public Class CliParser
    Public Shared Function Parse(args As IEnumerable(Of String)) As CliCommand
        Dim command As New CliCommand()
        Dim tokens = If(args, Enumerable.Empty(Of String)()).ToList()
        Dim index = 0

        While index < tokens.Count
            Dim token = tokens(index)

            If String.Equals(token, "--help", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(token, "-h", StringComparison.OrdinalIgnoreCase) Then
                command.Help = True
                index += 1
            ElseIf String.Equals(token, "--version", StringComparison.OrdinalIgnoreCase) Then
                command.Version = True
                index += 1
            ElseIf String.Equals(token, "--json", StringComparison.OrdinalIgnoreCase) Then
                command.Json = True
                index += 1
            ElseIf String.Equals(token, "--quiet", StringComparison.OrdinalIgnoreCase) Then
                command.Quiet = True
                index += 1
            ElseIf String.Equals(token, "--apply", StringComparison.OrdinalIgnoreCase) Then
                command.Apply = True
                index += 1
            ElseIf String.Equals(token, "--yes", StringComparison.OrdinalIgnoreCase) Then
                command.Yes = True
                index += 1
            ElseIf String.Equals(token, "--zip", StringComparison.OrdinalIgnoreCase) Then
                command.Zip = True
                index += 1
            ElseIf IsValueOption(token) Then
                ApplyValueOption(command, token, ReadValue(tokens, index, command))
                index += 2
            ElseIf token.StartsWith("-") Then
                ApplyFlagOption(command, token)
                index += 1
            ElseIf String.IsNullOrWhiteSpace(command.CommandName) Then
                command.CommandName = token.Trim().ToLowerInvariant()
                index += 1
            Else
                ApplyPositional(command, token)
                index += 1
            End If
        End While

        Validate(command)
        Return command
    End Function

    Private Shared Function IsValueOption(token As String) As Boolean
        Select Case If(token, "").Trim().ToLowerInvariant()
            Case "--catalog", "--vault", "--output", "--format", "--scope", "--category", "--tag", "--limit", "--fail-on"
                Return True
            Case Else
                Return False
        End Select
    End Function

    Private Shared Function ReadValue(tokens As List(Of String), index As Integer, command As CliCommand) As String
        If index + 1 >= tokens.Count OrElse tokens(index + 1).StartsWith("-") Then
            command.Errors.Add($"{tokens(index)} requires a value.")
            Return ""
        End If

        Return tokens(index + 1)
    End Function

    Private Shared Sub ApplyValueOption(command As CliCommand, optionName As String, value As String)
        Select Case optionName.Trim().ToLowerInvariant()
            Case "--catalog"
                command.CatalogPath = value
            Case "--vault"
                command.VaultRootPath = value
            Case "--output"
                command.OutputPath = value
            Case "--format"
                command.Format = value
            Case "--scope"
                command.Scope = value
            Case "--category"
                command.Category = value
            Case "--tag"
                command.Tag = value
            Case "--limit"
                Dim parsed As Integer
                If Integer.TryParse(value, parsed) Then
                    command.Limit = parsed
                Else
                    command.Errors.Add("--limit must be a number.")
                End If
            Case "--fail-on"
                command.FailOn = value
        End Select
    End Sub

    Private Shared Sub ApplyFlagOption(command As CliCommand, token As String)
        Select Case token.Trim().ToLowerInvariant()
            Case "--copy"
                command.Mode = IngestMode.Copy
            Case "--move"
                command.Mode = IngestMode.Move
            Case Else
                command.Errors.Add($"Unknown option: {token}")
        End Select
    End Sub

    Private Shared Sub ApplyPositional(command As CliCommand, token As String)
        Select Case command.CommandName
            Case "ingest"
                command.Paths.Add(token)
            Case "search"
                If String.IsNullOrWhiteSpace(command.Query) Then
                    command.Query = token
                Else
                    command.Query &= " " & token
                End If
            Case Else
                command.Paths.Add(token)
        End Select
    End Sub

    Private Shared Sub Validate(command As CliCommand)
        If command.Help OrElse command.Version Then
            Return
        End If

        If String.IsNullOrWhiteSpace(command.CommandName) Then
            command.Errors.Add("Command is required.")
            Return
        End If

        Select Case command.CommandName
            Case "ingest"
                If command.Paths.Count = 0 Then
                    command.Errors.Add("ingest requires at least one path.")
                End If
            Case "verify"
                If Not {"any", "medium", "high"}.Contains(If(command.FailOn, "").ToLowerInvariant()) Then
                    command.Errors.Add("--fail-on must be any, medium, or high.")
                End If
            Case "search"
                If String.IsNullOrWhiteSpace(command.Query) Then
                    command.Errors.Add("search requires a query.")
                End If
            Case "export", "report", "repair-preview", "repair", "rescan", "rebuild-thumbnails", "package"
            Case Else
                command.Errors.Add($"Unknown command: {command.CommandName}")
        End Select

        If command.CommandName = "report" AndAlso Not {"text", "json"}.Contains(If(command.Format, "").ToLowerInvariant()) Then
            command.Errors.Add("--format must be text or json.")
        End If

        If {"repair", "rescan", "rebuild-thumbnails"}.Contains(command.CommandName) AndAlso command.Apply AndAlso Not command.Yes Then
            command.Errors.Add($"{command.CommandName} --apply requires --yes.")
        End If
    End Sub
End Class
