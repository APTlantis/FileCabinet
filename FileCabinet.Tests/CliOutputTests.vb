Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace FileCabinet.Tests
    <TestClass>
    Public Class CliOutputTests
        <TestMethod>
        Sub TextOutputHelpAndVersionUseCliBranding()
            Dim help = Global.FileCabinet.Cli.CliTextOutput.Help()
            Dim version = Global.FileCabinet.Cli.CliTextOutput.Version()

            StringAssert.Contains(help, "FileCabinet CLI")
            StringAssert.Contains(help, "Global options")
            Assert.AreEqual("FileCabinet.Cli 1.4.1", version)
        End Sub

        <TestMethod>
        Sub JsonOutputSerializesIndentedCommandPayload()
            Dim json = Global.FileCabinet.Cli.CliJsonOutput.Search(New List(Of Global.FileCabinet.ArtifactModel) From {
                New Global.FileCabinet.ArtifactModel With {
                    .Id = "artifact-1",
                    .Name = "firmware.bin",
                    .Category = "Software / Installers",
                    .Type = "Binary"
                }
            })

            StringAssert.Contains(json, """command"": ""search""")
            StringAssert.Contains(json, """count"": 1")
            StringAssert.Contains(json, """name"": ""firmware.bin""")
        End Sub
    End Class
End Namespace
