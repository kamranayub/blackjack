Imports System
Imports System.Collections.Generic
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation
Imports System.Windows.Shapes

Partial Public Class PlayingCard

    Private m_Image As ImageBrush
    Private Property CardImage() As ImageBrush
        Get
            Return m_Image
        End Get
        Set(ByVal value As ImageBrush)
            m_Image = value
        End Set
    End Property

    Private m_Card As Card
    Public Property ActualCard() As Card
        Get
            Return m_Card
        End Get
        Set(ByVal value As Card)
            m_Card = value

            rectCard.Fill = m_Card.Image
        End Set
    End Property

    Private Sub Control_Loaded(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Loaded

        Dim myWidth As Double = Me.ActualWidth

    End Sub

    Public Sub ShowActualCard(ByVal bShowCard As Boolean)
        If bShowCard Then
            PlayShowHole()
        Else
            rectCard.Fill = CType(FindResource("EndDeck"), Brush)
        End If
    End Sub

    Public Sub PlayDrawCardAnim(ByVal delay As Integer)
        Try
            Dim createCardAnim As Storyboard = CType(FindResource("stbdCreateCard"), Storyboard)

            ' Delay the anim
            createCardAnim.BeginTime = TimeSpan.FromMilliseconds(delay)

            createCardAnim.Begin(Me)
        Catch ex As Exception
            Me.Opacity = 1.0
            Debugger.Instance.ShowMessage(Me, ex.Message)
        End Try
        
    End Sub

    Public Sub PlayShowHole()
        Try
            Dim showHoleAnim As Storyboard = CType(FindResource("stbdShowHole"), Storyboard)

            ' Let's create a new object animation
            Dim FillCard As ObjectAnimationUsingKeyFrames = New ObjectAnimationUsingKeyFrames

            ' We are targeting the rectangle's Fill property
            Storyboard.SetTarget(FillCard, Me.rectCard)
            Storyboard.SetTargetProperty(FillCard, New PropertyPath("(Rectangle.Fill)"))

            ' Tell the animation when to change the Fill
            ' and what image to use
            FillCard.KeyFrames.Add(New DiscreteObjectKeyFrame(ActualCard.Image, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.25))))

            showHoleAnim.Children.Add(FillCard)
            showHoleAnim.Begin(Me)
        Catch ex As Exception
            rectCard.Fill = ActualCard.Image

            Debugger.Instance.ShowMessage(Me, ex.Message)

        End Try
    End Sub
End Class
