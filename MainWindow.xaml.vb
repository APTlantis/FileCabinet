Imports System.Windows
Imports System.Windows.Input

Class MainWindow
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

    Private Sub ToggleWindowState()
        If WindowState = WindowState.Maximized Then
            WindowState = WindowState.Normal
        Else
            WindowState = WindowState.Maximized
        End If
    End Sub
End Class
