Public Class Debugger

    Private Shared _mInstance As Debugger
    Private _mMsgsEnabled As Boolean

    Shared Sub New()
        _mInstance = New Debugger
    End Sub

    Private Sub New()
        ' Disallow creation of other instances
    End Sub

    Public Shared ReadOnly Property Instance() As Debugger
        Get
            Return _mInstance
        End Get
    End Property

    Public Property MessagesEnabled() As Boolean
        Get
            Return _mMsgsEnabled
        End Get
        Set(ByVal value As Boolean)
            _mMsgsEnabled = value
        End Set
    End Property

    Public Sub ShowMessage(ByVal sender As Object, ByVal msg As String)

#If DEBUG Then

        If Instance.MessagesEnabled Then

            Dim ID As String
            Dim MsgDate As String = Now.ToString("hh:mm:ss.fff")

            ID = sender.GetType.ToString

            If TypeOf sender Is Character Then
                ID &= String.Format("[{0}]", CType(sender, Character).UIControl.Name)
            End If

            Debug.Print(String.Format("[{0}] {1} says: {2}", MsgDate, ID, msg))

        End If
#End If

    End Sub

End Class
