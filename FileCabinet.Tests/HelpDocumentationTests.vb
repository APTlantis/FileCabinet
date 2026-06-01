Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace FileCabinet.Tests
    <TestClass>
    Public Class HelpDocumentationTests
        <TestMethod>
        Sub ResolveDocumentationPathFindsReadmeFromDevelopmentTree()
            Dim path = Global.FileCabinet.MainViewModel.ResolveDocumentationPath("README.md")

            Assert.IsFalse(String.IsNullOrWhiteSpace(path))
            Assert.IsTrue(IO.File.Exists(path))
            Assert.AreEqual("README.md", IO.Path.GetFileName(path))
        End Sub

        <TestMethod>
        Sub ResolveDocumentationPathReturnsEmptyForMissingDocument()
            Dim path = Global.FileCabinet.MainViewModel.ResolveDocumentationPath("docs\missing-help-document.md")

            Assert.AreEqual("", path)
        End Sub
    End Class
End Namespace
