Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace FilingCabinet.Tests
    <TestClass>
    Public Class HelpDocumentationTests
        <TestMethod>
        Sub ResolveDocumentationPathFindsReadmeFromDevelopmentTree()
            Dim path = Global.FilingCabinet.MainViewModel.ResolveDocumentationPath("README.md")

            Assert.IsFalse(String.IsNullOrWhiteSpace(path))
            Assert.IsTrue(IO.File.Exists(path))
            Assert.AreEqual("README.md", IO.Path.GetFileName(path))
        End Sub

        <TestMethod>
        Sub ResolveDocumentationPathReturnsEmptyForMissingDocument()
            Dim path = Global.FilingCabinet.MainViewModel.ResolveDocumentationPath("docs\missing-help-document.md")

            Assert.AreEqual("", path)
        End Sub
    End Class
End Namespace

