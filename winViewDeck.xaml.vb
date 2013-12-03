Partial Public Class winViewDeck

    Private _mDeck As Shoe
    Public Property Deck() As Shoe
        Get
            Return _mDeck
        End Get
        Set(ByVal value As Shoe)
            _mDeck = value
        End Set
    End Property

    Private Sub winViewDeck_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded

    End Sub

    Public Sub FillGrid(ByVal numPlayers As Integer)

        If numPlayers > 0 Then
            gridCards.Columns = numPlayers + 1
        End If

        Deck.FillGrid(gridCards)

        txtNumCards.Text = String.Format("Number of Cards: {0}", gridCards.Children.Count)
    End Sub
End Class
