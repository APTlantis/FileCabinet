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

Public Class AsyncRelayCommand
    Implements ICommand

    Private ReadOnly _execute As Func(Of Object, Task)
    Private ReadOnly _canExecute As Predicate(Of Object)
    Private _isExecuting As Boolean

    Public Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged

    Public Sub New(execute As Func(Of Object, Task), Optional canExecute As Predicate(Of Object) = Nothing)
        _execute = execute
        _canExecute = canExecute
    End Sub

    Public ReadOnly Property IsExecuting As Boolean
        Get
            Return _isExecuting
        End Get
    End Property

    Public Function CanExecute(parameter As Object) As Boolean Implements ICommand.CanExecute
        Return Not _isExecuting AndAlso (_canExecute Is Nothing OrElse _canExecute(parameter))
    End Function

    Public Async Sub Execute(parameter As Object) Implements ICommand.Execute
        Await ExecuteAsync(parameter)
    End Sub

    Public Async Function ExecuteAsync(parameter As Object) As Task
        If Not CanExecute(parameter) Then
            Return
        End If

        Try
            _isExecuting = True
            RaiseCanExecuteChanged()
            Await _execute(parameter)
        Finally
            _isExecuting = False
            RaiseCanExecuteChanged()
        End Try
    End Function

    Public Sub RaiseCanExecuteChanged()
        RaiseEvent CanExecuteChanged(Me, EventArgs.Empty)
    End Sub
End Class
