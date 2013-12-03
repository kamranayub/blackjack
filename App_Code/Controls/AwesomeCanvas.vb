Public Class AwesomeCanvas
    Inherits Canvas

    Protected Overrides Sub OnRenderSizeChanged(ByVal sizeInfo As System.Windows.SizeChangedInfo)

        Dim NewWidth As Double = sizeInfo.NewSize.Width
        Dim NewHeight As Double = sizeInfo.NewSize.Height
        Dim OldWidth As Double = sizeInfo.PreviousSize.Width
        Dim OldHeight As Double = sizeInfo.PreviousSize.Height

        Dim RatioW As Double
        Dim RatioH As Double

        RatioW = (NewWidth - OldWidth) / OldWidth
        RatioH = (NewHeight - OldHeight) / OldHeight

        If Double.IsInfinity(RatioW) Then
            Exit Sub
        ElseIf Double.IsInfinity(RatioH) Then
            Exit Sub
        End If

        ' Relatively position children
        For Each c As Control In Me.Children
            Dim currentX As Double = CDbl(c.GetValue(Canvas.LeftProperty))
            Dim currentY As Double = CDbl(c.GetValue(Canvas.TopProperty))
            Dim originX As Double = currentX + CInt(c.RenderTransformOrigin.X)
            Dim originY As Double = currentY + CInt(c.RenderTransformOrigin.Y)

            Dim moveX As Double = (originX * RatioW) + currentX
            Dim moveY As Double = (originY * RatioH) + currentY

            c.SetValue(Canvas.LeftProperty, CDbl(moveX))
            c.SetValue(Canvas.TopProperty, CDbl(moveY))
        Next
    End Sub
End Class
