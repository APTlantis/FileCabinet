Imports System.IO
Imports FileCabinet
Imports FileCabinet.Cli

Public Module Program
    Public Function Main(args As String()) As Integer
        Return Run(args, Console.Out, Console.Error)
    End Function

    Public Function Run(args As IEnumerable(Of String), output As TextWriter, errorOutput As TextWriter) As Integer
        Dim command = CliParser.Parse(args)

        If command.Version Then
            output.WriteLine(CliTextOutput.Version())
            Return 0
        End If

        If command.Help Then
            output.WriteLine(CliTextOutput.Help())
            Return 0
        End If

        If Not command.IsValid Then
            For Each message In command.Errors
                errorOutput.WriteLine(message)
            Next
            errorOutput.WriteLine("Use --help for usage.")
            Return 1
        End If

        Try
            Dim service = New VaultHeadlessService(New HeadlessOptions With {
                .CatalogPath = command.CatalogPath,
                .VaultRootPath = command.VaultRootPath
            })

            Return ExecuteCommand(command, service, output, errorOutput)
        Catch ex As Exception
            errorOutput.WriteLine(ex.Message)
            Return 1
        End Try
    End Function

    Private Function ExecuteCommand(command As CliCommand, service As VaultHeadlessService, output As TextWriter, errorOutput As TextWriter) As Integer
        Select Case command.CommandName
            Case "ingest"
                Dim result = service.Ingest(command.Paths, command.Mode)
                WriteOutput(command, output, CliJsonOutput.Ingest(result), CliTextOutput.Ingest(result))
                Return result.ExitCode
            Case "verify"
                Dim result = service.Verify(command.FailOn)
                WriteOutput(command, output, CliJsonOutput.Verify(result), CliTextOutput.Verify(result))
                Return result.ExitCode
            Case "search"
                Dim results = service.Search(New VaultSearchOptions With {
                    .Query = command.Query,
                    .Scope = command.Scope,
                    .Category = command.Category,
                    .Tag = command.Tag,
                    .Limit = command.Limit
                })
                WriteOutput(command, output, CliJsonOutput.Search(results), CliTextOutput.Search(results))
                Return 0
            Case "export"
                Dim result = service.ExportSnapshot(command.OutputPath)
                WriteOutput(command, output, CliJsonOutput.ExportSnapshot(result), CliTextOutput.ExportSnapshot(result))
                Return If(result.Validation.IsValid, 0, 1)
            Case "report"
                Dim result = service.GenerateReport(command.OutputPath, command.Format)
                WriteOutput(command, output, CliJsonOutput.Report(result), CliTextOutput.Report(result))
                Return 0
            Case "repair-preview"
                Dim result = service.RepairPreview()
                WriteOutput(command, output, CliJsonOutput.RepairPreview(result), CliTextOutput.RepairPreview(result))
                Return 0
            Case "repair"
                Dim result = service.Repair(command.Apply)
                WriteOutput(command, output, CliJsonOutput.Repair(result), CliTextOutput.Repair(result))
                Return If(result.FailedCount > 0, 3, 0)
            Case "rescan"
                Dim result = service.Rescan(command.Apply)
                WriteOutput(command, output, CliJsonOutput.Rescan(result), CliTextOutput.Rescan(result))
                Return 0
            Case "rebuild-thumbnails"
                Dim result = service.RebuildThumbnails(command.Apply)
                WriteOutput(command, output, CliJsonOutput.RebuildThumbnails(result), CliTextOutput.RebuildThumbnails(result))
                Return If(result.FailedCount > 0, 3, 0)
            Case "package"
                Dim result = service.CreatePackage(command.OutputPath, command.Zip)
                WriteOutput(command, output, CliJsonOutput.Package(result), CliTextOutput.Package(result))
                Return 0
            Case Else
                errorOutput.WriteLine($"Unknown command: {command.CommandName}")
                Return 1
        End Select
    End Function

    Private Sub WriteOutput(command As CliCommand, output As TextWriter, jsonText As String, text As String)
        If command.Quiet Then
            Return
        End If

        output.WriteLine(If(command.Json, jsonText, text))
    End Sub
End Module
