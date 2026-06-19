Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace FileCabinet.Tests
    <TestClass>
    Public Class CliOutputTests
        <TestMethod>
        Sub TextOutputHelpAndVersionUseCliBranding()
            Dim help = Global.FileCabinet.Cli.CliTextOutput.Help()
            Dim version = Global.FileCabinet.Cli.CliTextOutput.Version()

            Assert.Contains("FileCabinet CLI", help)
            Assert.Contains("Global options", help)
            Assert.AreEqual("FileCabinet.Cli 1.7.3", version)
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

            Assert.Contains("""command"": ""search""", json)
            Assert.Contains("""count"": 1", json)
            Assert.Contains("""name"": ""firmware.bin""", json)
        End Sub
    End Class
End Namespace
