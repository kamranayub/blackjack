Imports System.Windows.Media.Animation

Public Class CardStacker
    Inherits Panel

    ''' <summary>
    ''' Dependency property that controls the width of the child elements
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared ReadOnly CardWidthProperty As DependencyProperty = DependencyProperty.RegisterAttached("CardWidth", GetType(Double), GetType(CardStacker), New FrameworkPropertyMetadata(1.0R, FrameworkPropertyMetadataOptions.AffectsMeasure Or FrameworkPropertyMetadataOptions.AffectsArrange))

    ''' <summary>
    ''' Dependency property that controls the height of the child elements
    ''' </summary>
    ''' <remarks></remarks>
    Public Shared ReadOnly CardHeightProperty As DependencyProperty = DependencyProperty.RegisterAttached("CardHeight", GetType(Double), GetType(CardStacker), New FrameworkPropertyMetadata(1.0R, FrameworkPropertyMetadataOptions.AffectsMeasure Or FrameworkPropertyMetadataOptions.AffectsArrange))

    ''' <summary>
    ''' Accessor for the card width dependency property
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CardWidth() As Double
        Get
            Return CDbl(GetValue(CardWidthProperty))
        End Get
        Set(ByVal value As Double)
            SetValue(CardWidthProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Accessor for the card height dependency property
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property CardHeight() As Double
        Get
            Return CDbl(GetValue(CardHeightProperty))
        End Get
        Set(ByVal value As Double)
            SetValue(CardHeightProperty, value)
        End Set
    End Property

    ''' <summary>
    ''' Gets a private Size structure for card size
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private ReadOnly Property CardSize() As Size
        Get
            If CardHeight <= 1.0 OrElse CardWidth <= 1.0 Then
                Return New Size(90, 135)
            Else
                Return New Size(CardWidth, CardHeight)
            End If
        End Get
    End Property

    Private m_OffsetX As Double = CardSize.Width * 0.25
    Private m_OffsetY As Double = CardSize.Height * 0.25

    ''' <summary>
    ''' Measures the size of the panel
    ''' </summary>
    ''' <param name="availableSize"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function MeasureOverride(ByVal availableSize As System.Windows.Size) As System.Windows.Size

        Dim resultSize As New Size(0, 0)

        Dim childCount As Integer = Me.Children.Count

        If Me.Children Is Nothing OrElse childCount = 0 Then
            Return resultSize
        End If

        Dim widthOffset, heightOffset As Double
        widthOffset = 0
        heightOffset = 0

        For Each child As UIElement In Children
            child.Measure(CardSize)
        Next

        ' Since we want to arrange elements like a stack of cards, we need
        ' to subtract the total offset
        resultSize.Width = CardSize.Width + (m_OffsetX * (childCount - 1))
        resultSize.Height = CardSize.Height

        Return resultSize

    End Function

    ''' <summary>
    ''' Arranges and animates cards to their positions.
    ''' Smoothly centers cards when the layout is affected.
    ''' </summary>
    ''' <param name="finalSize"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function ArrangeOverride(ByVal finalSize As System.Windows.Size) As System.Windows.Size
        If Me.Children Is Nothing OrElse Me.Children.Count = 0 Then
            Return finalSize
        End If

        Dim childCount As Integer = Me.Children.Count
        Dim trans As TranslateTransform = Nothing

        For i As Integer = 0 To childCount - 1
            Dim child As UIElement = Me.Children(i)

            ' New offset
            Dim newOffset As New Point(i * m_OffsetX, 0)

            trans = TryCast(child.RenderTransform, TranslateTransform)

            If trans Is Nothing Then
                child.RenderTransformOrigin = New Point(0, 0)
                trans = New TranslateTransform()
                child.RenderTransform = trans
            End If

            child.Arrange(New Rect(newOffset, CardSize))

            If i = 0 And childCount = 1 Then
                Exit For
            End If

            If Me.HorizontalAlignment = Windows.HorizontalAlignment.Center Then

                ' Animate from Offset / 2, because the panel is centered
                trans.BeginAnimation(TranslateTransform.XProperty, MakeAnimation(m_OffsetX / 2))
            End If

        Next

        Return finalSize
    End Function

    ''' <summary>
    ''' Create the animation for centering the card.
    ''' </summary>
    ''' <param name="start">The start position</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function MakeAnimation(ByVal start As Double) As DoubleAnimation
        Dim animation As New DoubleAnimation(start, 0.0R, New Duration(TimeSpan.FromMilliseconds(700)))
        animation.DecelerationRatio = 0.8
        Return animation
    End Function

    Protected Overrides Function GetLayoutClip(ByVal layoutSlotSize As Size) As Geometry

        Return If(ClipToBounds, MyBase.GetLayoutClip(layoutSlotSize), Nothing)
    End Function
End Class
