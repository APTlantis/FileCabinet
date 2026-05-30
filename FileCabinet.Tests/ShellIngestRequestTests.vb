Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace FileCabinet.Tests
    <TestClass>
    Public Class ShellIngestRequestTests
        <TestMethod>
        Sub ParseCopyShellVerbKeepsSelectedPaths()
            Dim request = Global.FileCabinet.ShellIngestRequest.Parse({"--copy", "C:\Temp\artifact.txt"})

            Assert.IsTrue(request.Mode.HasValue)
            Assert.AreEqual(Global.FileCabinet.IngestMode.Copy, request.Mode.Value)
            Assert.AreEqual(1, request.Paths.Count)
            Assert.AreEqual("C:\Temp\artifact.txt", request.Paths(0))
        End Sub

        <TestMethod>
        Sub ParseMoveShellVerbKeepsFolderPath()
            Dim request = Global.FileCabinet.ShellIngestRequest.Parse({"/move", "C:\Temp\batch"})

            Assert.IsTrue(request.Mode.HasValue)
            Assert.AreEqual(Global.FileCabinet.IngestMode.Move, request.Mode.Value)
            Assert.AreEqual("C:\Temp\batch", request.Paths(0))
        End Sub

        <TestMethod>
        Sub ParseWithoutModeLeavesModeUnsetForDefaultIntake()
            Dim request = Global.FileCabinet.ShellIngestRequest.Parse({"C:\Temp\artifact.txt"})

            Assert.IsFalse(request.Mode.HasValue)
            Assert.IsTrue(request.HasPaths)
        End Sub
    End Class
End Namespace
