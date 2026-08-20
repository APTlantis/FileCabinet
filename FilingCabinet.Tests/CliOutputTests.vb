Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace FilingCabinet.Tests
    <TestClass>
    Public Class CliOutputTests
        <TestMethod>
        Sub TextOutputHelpAndVersionUseCliBranding()
            Dim help = Global.FilingCabinet.Cli.CliTextOutput.Help()
            Dim version = Global.FilingCabinet.Cli.CliTextOutput.Version()

            Assert.Contains("FilingCabinet CLI", help)
            Assert.Contains("Global options", help)
        Assert.AreEqual("FilingCabinet.Cli 0.1.1", version)
        End Sub

        <TestMethod>
        Sub JsonOutputSerializesIndentedCommandPayload()
            Dim json = Global.FilingCabinet.Cli.CliJsonOutput.Search(New List(Of Global.FilingCabinet.ArtifactModel) From {
                New Global.FilingCabinet.ArtifactModel With {
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

