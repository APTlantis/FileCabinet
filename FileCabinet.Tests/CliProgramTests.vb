Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.IO

Namespace FileCabinet.Tests
    <TestClass>
    Public Class CliProgramTests
        <TestMethod>
        Sub RunIngestReturnsZeroAndWritesTextOutput()
            Dim workspace = Path.Combine(Path.GetTempPath(), "FileCabinetCliTests", Guid.NewGuid().ToString("N"))
            Directory.CreateDirectory(workspace)
            Dim source = Path.Combine(workspace, "artifact.txt")
            File.WriteAllText(source, "cli entrypoint")
            Dim output As New StringWriter()
            Dim errors As New StringWriter()

            Try
                Dim exitCode = Global.Program.Run({
                    "ingest",
                    "--copy",
                    "--catalog", Path.Combine(workspace, "catalog.json"),
                    "--vault", Path.Combine(workspace, "vault"),
                    source
                }, output, errors)

                Assert.AreEqual(0, exitCode)
                StringAssert.Contains(output.ToString(), "Ingested 1 file")
                Assert.AreEqual("", errors.ToString())
            Finally
                If Directory.Exists(workspace) Then
                    Directory.Delete(workspace, recursive:=True)
                End If
            End Try
        End Sub

        <TestMethod>
        Sub RunInvalidCommandReturnsOneAndWritesError()
            Dim output As New StringWriter()
            Dim errors As New StringWriter()

            Dim exitCode = Global.Program.Run({"unknown"}, output, errors)

            Assert.AreEqual(1, exitCode)
            StringAssert.Contains(errors.ToString(), "Unknown command")
        End Sub
    End Class
End Namespace
