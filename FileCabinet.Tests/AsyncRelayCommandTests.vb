Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.Threading.Tasks

Namespace FileCabinet.Tests
    <TestClass>
    Public Class AsyncRelayCommandTests
        <TestMethod>
        Public Async Function ExecuteAsyncBlocksReentrantExecutionWhileRunning() As Task
            Dim started = New TaskCompletionSource(Of Boolean)()
            Dim release = New TaskCompletionSource(Of Boolean)()
            Dim runCount = 0
            Dim command = New Global.FileCabinet.AsyncRelayCommand(
                Async Function(parameter)
                    runCount += 1
                    started.SetResult(True)
                    Await release.Task
                End Function)

            Dim firstRun = command.ExecuteAsync(Nothing)
            Await started.Task

            Assert.IsFalse(command.CanExecute(Nothing))
            Await command.ExecuteAsync(Nothing)
            Assert.AreEqual(1, runCount)

            release.SetResult(True)
            Await firstRun

            Assert.IsTrue(command.CanExecute(Nothing))
        End Function

        <TestMethod>
        Public Async Function ExecuteAsyncCapturesExceptionsAndRestoresCanExecute() As Task
            Dim observed As Exception = Nothing
            Dim command = New Global.FileCabinet.AsyncRelayCommand(
                Function(parameter) Task.FromException(New InvalidOperationException("boom")),
                onException:=Sub(ex) observed = ex)

            Await command.ExecuteAsync(Nothing)

            Assert.IsTrue(command.CanExecute(Nothing))
            Assert.IsNotNull(command.LastException)
            Assert.IsNotNull(observed)
            Assert.AreEqual("boom", observed.Message)
        End Function
    End Class
End Namespace
