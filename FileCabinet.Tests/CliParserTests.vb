Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace FileCabinet.Tests
    <TestClass>
    Public Class CliParserTests
        <TestMethod>
        Sub ParseIngestCommandWithGlobalOptionsAndMode()
            Dim command = Global.CliParser.Parse({"ingest", "--copy", "--catalog", "C:\Temp\catalog.json", "--vault", "K:\Vault", "C:\Temp\a.txt"})

            Assert.IsTrue(command.IsValid)
            Assert.AreEqual("ingest", command.CommandName)
            Assert.AreEqual(Global.FileCabinet.IngestMode.Copy, command.Mode.Value)
            Assert.AreEqual("C:\Temp\catalog.json", command.CatalogPath)
            Assert.AreEqual("K:\Vault", command.VaultRootPath)
            Assert.AreEqual(1, command.Paths.Count)
        End Sub

        <TestMethod>
        Sub ParseSearchCommandCollectsQueryAndFilters()
            Dim command = Global.CliParser.Parse({"search", "firmware", "manifest", "--scope", "missing-preview", "--tag", "release", "--limit", "5", "--json"})

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
            Dim command = Global.CliParser.Parse({"launch", "rockets"})

            Assert.IsFalse(command.IsValid)
            Assert.IsTrue(command.Errors.Any(Function(message) message.Contains("Unknown command")))
        End Sub

        <TestMethod>
        Sub ParseMutatingCommandRequiresApplyAndYesTogether()
            Dim missingApproval = Global.CliParser.Parse({"repair", "--apply"})
            Dim approved = Global.CliParser.Parse({"repair", "--apply", "--yes"})

            Assert.IsFalse(missingApproval.IsValid)
            Assert.IsTrue(missingApproval.Errors.Any(Function(message) message.Contains("--yes")))
            Assert.IsTrue(approved.IsValid)
            Assert.IsTrue(approved.Apply)
            Assert.IsTrue(approved.Yes)
        End Sub
    End Class
End Namespace
