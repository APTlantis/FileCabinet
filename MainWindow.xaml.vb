Imports System.Windows
Imports System.Windows.Input
Imports Microsoft.Win32

Class MainWindow
    Public Sub New()
        InitializeComponent()
        Dim viewModel = New MainViewModel()
        DataContext = viewModel

        Dim shellRequest = ShellIngestRequest.Parse(Environment.GetCommandLineArgs().Skip(1))
        If shellRequest.HasPaths Then
            AddHandler Loaded, Async Sub()
                                   Await viewModel.IngestPathsAsync(shellRequest.Paths, shellRequest.Mode)
                               End Sub
        End If
    End Sub

    Private Sub TitleBar_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
        If e.ClickCount = 2 Then
            ToggleWindowState()
            Return
        End If

        DragMove()
    End Sub

    Private Sub MinimizeButton_Click(sender As Object, e As RoutedEventArgs)
        WindowState = WindowState.Minimized
    End Sub

    Private Sub MaximizeButton_Click(sender As Object, e As RoutedEventArgs)
        ToggleWindowState()
    End Sub

    Private Sub CloseButton_Click(sender As Object, e As RoutedEventArgs)
        Close()
    End Sub

    Private Sub DropZone_Drop(sender As Object, e As DragEventArgs)
        If Not e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Return
        End If

        Dim paths = TryCast(e.Data.GetData(DataFormats.FileDrop), String())
        IngestPathsAsync(paths)
    End Sub

    Private Sub DropZone_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        Dim dialog As New OpenFileDialog With {
            .Multiselect = True,
            .Title = "Select files to ingest into FileCabinet"
        }

        If dialog.ShowDialog(Me) = True Then
            IngestPathsAsync(dialog.FileNames)
        End If
    End Sub

    Private Sub ChooseVaultButton_Click(sender As Object, e As RoutedEventArgs)
        Dim viewModel = TryCast(DataContext, MainViewModel)

        If viewModel Is Nothing Then
            Return
        End If

        Dim dialog As New OpenFolderDialog With {
            .Title = "Choose FileCabinet vault folder",
            .Multiselect = False,
            .InitialDirectory = viewModel.VaultRootPath
        }

        If dialog.ShowDialog(Me) = True Then
            viewModel.SetVaultRoot(dialog.FolderName)
        End If
    End Sub

    Private Sub RestoreArtifactButton_Click(sender As Object, e As RoutedEventArgs)
        Dim viewModel = TryCast(DataContext, MainViewModel)

        If viewModel Is Nothing OrElse Not viewModel.RestoreArtifactCommand.CanExecute(Nothing) Then
            Return
        End If

        Dim dialog As New OpenFolderDialog With {
            .Title = "Choose folder to restore a copy",
            .Multiselect = False,
            .InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
        }

        If dialog.ShowDialog(Me) = True Then
            viewModel.RestoreArtifactCommand.Execute(dialog.FolderName)
        End If
    End Sub

    Private Sub DeleteArtifactButton_Click(sender As Object, e As RoutedEventArgs)
        Dim viewModel = TryCast(DataContext, MainViewModel)

        If viewModel Is Nothing OrElse Not viewModel.PermanentlyDeleteArtifactCommand.CanExecute(Nothing) Then
            Return
        End If

        Dim artifactName = viewModel.GetSelectedArtifactName()
        Dim result = MessageBox.Show(
            Me,
            $"Permanently delete '{artifactName}' from the vault and catalog? This cannot be undone.",
            "Delete artifact permanently",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No)

        If result = MessageBoxResult.Yes Then
            viewModel.PermanentlyDeleteArtifactCommand.Execute(Nothing)
        End If
    End Sub

    Private Sub ApplyRepairCandidatesButton_Click(sender As Object, e As RoutedEventArgs)
        Dim viewModel = TryCast(DataContext, MainViewModel)

        If viewModel Is Nothing OrElse Not viewModel.ApplySelectedRepairCandidatesCommand.CanExecute(Nothing) Then
            Return
        End If

        Dim result = MessageBox.Show(
            Me,
            viewModel.GetSelectedRepairCandidateSummary(),
            "Apply selected repairs",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No)

        If result = MessageBoxResult.Yes Then
            viewModel.ApplySelectedRepairCandidatesCommand.Execute(Nothing)
        End If
    End Sub

    Private Sub ToggleWindowState()
        If WindowState = WindowState.Maximized Then
            WindowState = WindowState.Normal
        Else
            WindowState = WindowState.Maximized
        End If
    End Sub

    Private Async Sub IngestPathsAsync(paths As IEnumerable(Of String))
        Dim viewModel = TryCast(DataContext, MainViewModel)

        If viewModel Is Nothing Then
            Return
        End If

        Await viewModel.IngestPathsAsync(paths)
    End Sub
End Class
