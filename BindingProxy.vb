Imports System.Windows

Public Class BindingProxy
    Inherits Freezable

    Public Shared ReadOnly DataProperty As DependencyProperty =
        DependencyProperty.Register(NameOf(Data), GetType(Object), GetType(BindingProxy), New UIPropertyMetadata(Nothing))

    Public Property Data As Object
        Get
            Return GetValue(DataProperty)
        End Get
        Set(value As Object)
            SetValue(DataProperty, value)
        End Set
    End Property

    Protected Overrides Function CreateInstanceCore() As Freezable
        Return New BindingProxy()
    End Function
End Class
