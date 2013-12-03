''' <summary>
''' A Basic Strategy for a single deck of cards.
''' Based on Wizard of Odds charts @ http://wizardofodds.com/blackjack/strategy/1deck.html
''' </summary>
''' <remarks></remarks>
Public Class BasicSingleDeck
    Inherits Strategy

#Region " Vars "

    Private _mName As String = "Basic Single Deck"

#End Region

    Public Overrides ReadOnly Property StrategyName() As String
        Get
            Return _mName
        End Get
    End Property

    Public Sub New()
        ' Player has a pair of cards
        '			Dealer Card	    A, 2, 3, 4, 5, 6, 7, 8, 9, 10
        '                                                           A   P
        '                                                           2   l
        '                                                           3   a
        '                                                           4   y
        '                                                           5   e
        '                                                           6   r
        '                                                           7   P
        '                                                           8   a
        '                                                           9   i
        '                                                           10  r
        Pairs = New Integer(9, 9) {{P, P, P, P, P, P, P, P, P, P}, _
                                   {H, H, P, P, P, P, P, H, H, H}, _
                                   {H, H, H, P, P, P, P, H, H, H}, _
                                   {H, H, H, H, D, D, H, H, H, H}, _
                                   {H, D, D, D, D, D, D, D, D, H}, _
                                   {H, P, P, P, P, P, H, H, H, H}, _
                                   {H, P, P, P, P, P, P, H, H, H}, _
                                   {P, P, P, P, P, P, P, P, P, P}, _
                                   {S, P, P, P, P, P, S, P, P, S}, _
                                   {S, S, S, S, S, S, S, S, S, S}}

        ' Player has an ace and something else
        '		Dealer Card	     A,2,3,4,5,6,7,8,9,10
        ' 2   P
        ' 3   l
        ' 4   a
        ' 5   y
        ' 6   e
        ' 7   r
        ' 8   
        ' 9   
        ' 10   
        Aces = New Integer(8, 9) {{H, H, H, D, D, D, H, H, H, H}, _
                                  {H, H, H, D, D, D, H, H, H, H}, _
                                  {H, H, H, D, D, D, H, H, H, H}, _
                                  {H, H, H, D, D, D, H, H, H, H}, _
                                  {H, D, D, D, D, D, H, H, H, H}, _
                                  {S, D, D, D, D, S, S, H, H, H}, _
                                  {S, S, S, S, S, S, S, S, S, S}, _
                                  {S, S, S, S, S, S, S, S, S, S}, _
                                  {S, S, S, S, S, S, S, S, S, S}}

        ' Player has a hard hand without an ace
        '      Dealer Card	     A,2,3,4,5,6,7,8,9,10
        ' 3   P
        ' 4   l
        ' 5   a
        ' 6   y
        ' 7   e
        ' 8   r
        ' 9   s
        ' 10   
        ' 11  T 
        ' 12  o
        ' 13  t
        ' 14  a
        ' 15  l
        ' 16
        Hand = New Integer(13, 9) {{H, H, H, H, H, H, H, H, H, H}, _
                                   {H, H, H, H, H, H, H, H, H, H}, _
                                   {H, H, H, H, H, H, H, H, H, H}, _
                                   {H, H, H, H, H, H, H, H, H, H}, _
                                   {H, H, H, H, H, H, H, H, H, H}, _
                                   {H, H, H, H, H, H, H, H, H, H}, _
                                   {H, D, D, D, D, D, H, H, H, H}, _
                                   {H, D, D, D, D, D, D, D, D, H}, _
                                   {D, D, D, D, D, D, D, D, D, D}, _
                                   {H, H, H, S, S, S, H, H, H, H}, _
                                   {H, S, S, S, S, S, H, H, H, H}, _
                                   {H, S, S, S, S, S, H, H, H, H}, _
                                   {H, S, S, S, S, S, H, H, H, H}, _
                                   {H, S, S, S, S, S, H, H, H, H}}

    End Sub


    Public Overloads Overrides Function GetInsuranceAdvice(ByVal count As Double, ByVal cardCount As Double, ByVal decks As Integer) As Boolean
        Return cardCount >= 3
    End Function
    Public Overloads Overrides Function GetAdvice(ByVal playerHand As Hand, ByVal dealerCard As Card, ByVal allowSplit As Boolean, ByVal cardCount As Double) As AdviceType
        Dim Advice As AdviceType = AdviceType.Null

        Try
            If dealerCard IsNot Nothing AndAlso playerHand.Cards.Count > 0 Then

                ' Pairs or Ace/#
                If playerHand.Cards.Count = 2 AndAlso allowSplit Then
                    If playerHand.Cards(0).Rank = playerHand.Cards(1).Rank Then
                        Advice = DirectCast(Pairs(CInt(playerHand.Cards(0).TrueValue), CInt(dealerCard.TrueValue)), AdviceType)
                    ElseIf playerHand.Cards(0).Rank = Card.RankType.Ace Then
                        Advice = DirectCast(Aces(CInt(playerHand.Cards(1).TrueValue) - 1, CInt(dealerCard.TrueValue)), AdviceType)
                    ElseIf playerHand.Cards(1).Rank = Card.RankType.Ace Then
                        Advice = DirectCast(Aces(CInt(playerHand.Cards(0).TrueValue) - 1, CInt(dealerCard.TrueValue)), AdviceType)
                    ElseIf playerHand.Total() >= 17 Then
                        Advice = AdviceType.Stand
                    Else
                        ' 16 or lower (Total - 3)
                        Advice = DirectCast(Hand(playerHand.Total() - 3, CInt(dealerCard.TrueValue)), AdviceType)
                    End If
                ElseIf playerHand.Total() >= 17 Then
                    Advice = AdviceType.Stand
                Else
                    Advice = DirectCast(Hand(playerHand.Total() - 3, CInt(dealerCard.TrueValue)), AdviceType)
                    If Advice = AdviceType.DoubleDown Then
                        Advice = AdviceType.Hit
                    End If
                End If
            End If
        Catch Ex As Exception
            Debugger.Instance.ShowMessage(Me, Ex.Message)
        End Try

        Return Advice
    End Function
End Class

