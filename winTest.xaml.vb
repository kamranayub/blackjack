Partial Public Class winTest

    Private Sub Button_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Dim pc As New PlayingCard()
        pc.ActualCard = New Card(Card.RankType.Ace, Card.SuitType.Spades)
        cardHolder.Children.Add(pc)
        'pc.Opacity = 0.5

        pc.PlayDrawCardAnim(0)
    End Sub
End Class
