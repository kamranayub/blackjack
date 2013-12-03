Imports System.Windows.Media.Animation

Partial Public Class UCCharacter

    Public ReadOnly Property DrawingSurface() As CardStacker
        Get
            Return cardStack
        End Get
    End Property

    Public ReadOnly Property LabelCardCount() As TextBlock
        Get
            Return lblCardCount
        End Get
    End Property

    Public Sub New()

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

    End Sub

    Public Sub AddCard(ByVal card As Card)



    End Sub

    Public Sub SetTotal(ByVal val As Integer)

        'lblTotal.Text = String.Format("Tot: {0}", val)
        SetOutcome(val.ToString, True)

    End Sub
    Public Sub SetOutcome(ByVal val As String, ByVal show As Boolean)

        lblOutcome.Text = val
        If show Then
            bdrOutcome.Visibility = Windows.Visibility.Visible
        Else
            bdrOutcome.Visibility = Windows.Visibility.Hidden
        End If

    End Sub
    Public Sub PlayTurnEnd()
        Try
            Dim TurnEndAnim As Storyboard = CType(FindResource("stbdTurnEnd"), Storyboard)

            TurnEndAnim.Begin(Me)
        Catch ex As Exception
            ' Animation didn't work, do it manually
            cardStack.Opacity = 0.4

            Debugger.Instance.ShowMessage(Me, ex.Message)

        End Try
    End Sub

    Public Sub PlayTurnStart()
        Try
            Dim TurnEndAnim As Storyboard = CType(FindResource("stbdTurnStart"), Storyboard)

            TurnEndAnim.Begin(Me)
        Catch ex As Exception
            cardStack.Opacity = 1.0

            Debugger.Instance.ShowMessage(Me, ex.Message)

        End Try
    End Sub

End Class
