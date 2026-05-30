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
    End Class
End Namespace
