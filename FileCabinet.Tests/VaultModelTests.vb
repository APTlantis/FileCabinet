Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.ComponentModel

Namespace FileCabinet.Tests
    <TestClass>
    Public Class VaultModelTests
        <TestMethod>
        Sub ChangingPathRaisesDisplayNameNotification()
            Dim vault As New Global.FileCabinet.VaultModel With {
                .Name = "ArchiveVault",
                .Path = "D:\"
            }
            Dim changedProperties As New List(Of String)
            AddHandler vault.PropertyChanged,
                Sub(sender As Object, args As PropertyChangedEventArgs)
                    changedProperties.Add(args.PropertyName)
                End Sub

            vault.Path = "K:\"

            Assert.AreEqual("ArchiveVault (K:\)", vault.DisplayName)
            CollectionAssert.Contains(changedProperties, NameOf(Global.FileCabinet.VaultModel.Path))
            CollectionAssert.Contains(changedProperties, NameOf(Global.FileCabinet.VaultModel.DisplayName))
        End Sub
    End Class
End Namespace
