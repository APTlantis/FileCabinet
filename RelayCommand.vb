Imports System.Windows.Input

Public Class RelayCommand
    Implements ICommand

    Private ReadOnly _execute As Action(Of Object)
    Private ReadOnly _canExecute As Predicate(Of Object)

    Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

    Public Sub New(execute As Action(Of Object), Optional canExecute As Predicate(Of Object) = Nothing)
        _execute = execute
        _canExecute = canExecute
    End Sub

    Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
        Return _canExecute Is Nothing OrElse _canExecute(parameter)
    End Function

    Public Sub Execute(parameter As Object) Implements ICommand.Execute
        _execute(parameter)
    End Sub

    Public Sub RaiseCanExecuteChanged()
        RaiseEvent CanExecuteChanged(Me, EventArgs.Empty)
    End Sub
End Class
