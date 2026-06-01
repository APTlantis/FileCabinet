Imports System.IO

Namespace FileCabinet.Tests
    Public NotInheritable Class TestWorkspace
        Implements IDisposable

        Public Sub New()
            Root = Path.Combine(Path.GetTempPath(), "FileCabinetTests", Guid.NewGuid().ToString("N"))
            SourceRoot = Path.Combine(Root, "source")
            VaultRoot = Path.Combine(Root, "vault")
            CatalogPath = Path.Combine(Root, "appdata", "catalog.json")

            Directory.CreateDirectory(SourceRoot)
            Directory.CreateDirectory(Path.GetDirectoryName(CatalogPath))
            Global.FileCabinet.CatalogService.EnsureVaultFolders(VaultRoot)
        End Sub

        Public ReadOnly Property Root As String
        Public ReadOnly Property SourceRoot As String
        Public ReadOnly Property VaultRoot As String
        Public ReadOnly Property CatalogPath As String

        Public Function SourcePath(fileName As String) As String
            Return Path.Combine(SourceRoot, fileName)
        End Function

        Public Function VaultItemPath(fileName As String) As String
            Return Path.Combine(VaultRoot, "items", fileName)
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            If Directory.Exists(Root) Then
                Directory.Delete(Root, recursive:=True)
            End If
        End Sub
    End Class
End Namespace
