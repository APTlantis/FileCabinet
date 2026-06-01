Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.Reflection

Namespace FileCabinet.Tests
    <TestClass>
    Public Class CliParserTests
        <TestMethod>
        Sub ParseIngestCommandWithGlobalOptionsAndMode()
            Dim command = Global.FileCabinet.Cli.CliParser.Parse({"ingest", "--copy", "--catalog", "C:\Temp\catalog.json", "--vault", "K:\Vault", "C:\Temp\a.txt"})

            Assert.IsTrue(command.IsValid)
            Assert.AreEqual("ingest", command.CommandName)
            Assert.AreEqual(Global.FileCabinet.IngestMode.Copy, command.Mode.Value)
            Assert.AreEqual("C:\Temp\catalog.json", command.CatalogPath)
            Assert.AreEqual("K:\Vault", command.VaultRootPath)
            Assert.AreEqual(1, command.Paths.Count)
        End Sub

        <TestMethod>
        Sub ParseSearchCommandCollectsQueryAndFilters()
            Dim command = Global.FileCabinet.Cli.CliParser.Parse({"search", "firmware", "manifest", "--scope", "missing-preview", "--tag", "release", "--limit", "5", "--json"})

            Assert.IsTrue(command.IsValid)
            Assert.AreEqual("search", command.CommandName)
            Assert.AreEqual("firmware manifest", command.Query)
            Assert.AreEqual("missing-preview", command.Scope)
            Assert.AreEqual("release", command.Tag)
            Assert.AreEqual(5, command.Limit)
            Assert.IsTrue(command.Json)
        End Sub

        <TestMethod>
        Sub ParseUnknownCommandReportsInvalidCommand()
            Dim command = Global.FileCabinet.Cli.CliParser.Parse({"launch", "rockets"})

            Assert.IsFalse(command.IsValid)
            Assert.IsTrue(command.Errors.Any(Function(message) message.Contains("Unknown command")))
        End Sub

        <TestMethod>
        Sub ParseMutatingCommandRequiresApplyAndYesTogether()
            Dim missingApproval = Global.FileCabinet.Cli.CliParser.Parse({"repair", "--apply"})
            Dim approved = Global.FileCabinet.Cli.CliParser.Parse({"repair", "--apply", "--yes"})

            Assert.IsFalse(missingApproval.IsValid)
            Assert.IsTrue(missingApproval.Errors.Any(Function(message) message.Contains("--yes")))
            Assert.IsTrue(approved.IsValid)
            Assert.IsTrue(approved.Apply)
            Assert.IsTrue(approved.Yes)
        End Sub

        <TestMethod>
        Sub ParseHelpVersionQuietAndZipSwitches()
            Dim help = Global.FileCabinet.Cli.CliParser.Parse({"-h"})
            Dim version = Global.FileCabinet.Cli.CliParser.Parse({"--version"})
            Dim package = Global.FileCabinet.Cli.CliParser.Parse({"package", "--quiet", "--zip"})

            Assert.IsTrue(help.Help)
            Assert.IsTrue(help.IsValid)
            Assert.IsTrue(version.Version)
            Assert.IsTrue(version.IsValid)
            Assert.IsTrue(package.Quiet)
            Assert.IsTrue(package.Zip)
            Assert.IsTrue(package.IsValid)
        End Sub

        <TestMethod>
        Sub ParseValueOptionRequiresValue()
            Dim command = Global.FileCabinet.Cli.CliParser.Parse({"search", "firmware", "--limit"})

            Assert.IsFalse(command.IsValid)
            Assert.IsTrue(command.Errors.Any(Function(message) message.Contains("--limit requires a value.")))
        End Sub

        <TestMethod>
        Sub ApplyValueOptionReportsUnexpectedOption()
            Dim command As New Global.FileCabinet.Cli.CliCommand()
            Dim method = GetType(Global.FileCabinet.Cli.CliParser).GetMethod("ApplyValueOption", BindingFlags.NonPublic Or BindingFlags.Static)

            method.Invoke(Nothing, {command, "--mystery", "value"})

            Assert.IsFalse(command.IsValid)
            Assert.IsTrue(command.Errors.Any(Function(message) message.Contains("Unknown option: --mystery")))
        End Sub
    End Class
End Namespace
