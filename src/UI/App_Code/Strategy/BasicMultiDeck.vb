<Serializable()> _
Public Class BasicMultiDeck
    Inherits Strategy
    Public Sub New()
        ' Player has a pair of cards
        '		Dealer Card	     A,2,3,4,5,6,7,8,9,10
        ' A   P
        ' 2   l
        ' 3   a
        ' 4   y
        ' 5   e
        ' 6   r
        ' 7   P
        ' 8   a
        ' 9   i
        Pairs = New Integer(9, 9) {{P, P, P, P, P, P, _
         P, P, P, P}, {H, H, H, P, P, P, _
         P, H, H, H}, {H, H, H, P, P, P, _
         P, H, H, H}, {H, H, H, H, H, H, _
         H, H, H, H}, {H, D, D, D, D, D, _
         D, D, D, H}, {H, H, P, P, P, P, _
         H, H, H, H}, _
         {H, P, P, P, P, P, _
         P, H, H, H}, {P, P, P, P, P, P, _
         P, P, P, P}, {S, P, P, P, P, P, _
         S, P, P, S}, {S, S, S, S, S, S, _
         S, S, S, S}}
        ' 10  r

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
        Aces = New Integer(8, 9) {{H, H, H, H, D, D, _
         H, H, H, H}, {H, H, H, H, D, D, _
         H, H, H, H}, {H, H, H, D, D, D, _
         H, H, H, H}, {H, H, H, D, D, D, _
         H, H, H, H}, {H, H, D, D, D, D, _
         H, H, H, H}, {H, S, D, D, D, D, _
         S, S, H, H}, _
         {S, S, S, S, S, S, _
         S, S, S, S}, {S, S, S, S, S, S, _
         S, S, S, S}, {S, S, S, S, S, S, _
         S, S, S, S}}
        ' 10  


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
        Hand = New Integer(13, 9) {{H, H, H, H, H, H, _
         H, H, H, H}, {H, H, H, H, H, H, _
         H, H, H, H}, {H, H, H, H, H, H, _
         H, H, H, H}, {H, H, H, H, H, H, _
         H, H, H, H}, {H, H, H, H, H, H, _
         H, H, H, H}, {H, H, H, H, H, H, _
         H, H, H, H}, _
         {H, H, D, D, D, D, _
         H, H, H, H}, {H, D, D, D, D, D, _
         D, D, D, H}, {H, D, D, D, D, D, _
         D, D, D, D}, {H, H, H, S, S, S, _
         H, H, H, H}, {H, S, S, S, S, S, _
         H, H, H, H}, {H, S, S, S, S, S, _
         H, H, H, H}, _
         {H, S, S, S, S, S, _
         H, H, H, H}, {H, S, S, S, S, S, _
         H, H, H, H}}
        ' 16

    End Sub

    Private name As String = "Basic Multi Deck"
    Public Overloads Overrides ReadOnly Property StrategyName() As String
        Get
            Return name
        End Get
    End Property
    Public Overloads Overrides Function GetInsuranceAdvice(ByVal count As Double, ByVal cardCount As Double, ByVal decks As Integer) As Boolean
        Return cardCount >= 3
    End Function
    Public Overloads Overrides Function GetAdvice(ByVal playerHand As Hand, ByVal dealerCard As Card, ByVal allowSplit As Boolean, ByVal cardCount As Double) As AdviceType
        Dim Advice As AdviceType = AdviceType.Null

        Try
            If dealerCard IsNot Nothing AndAlso playerHand.Cards.Count > 0 Then
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
            Dim foo As String = Ex.Message
        End Try

        Return Advice
    End Function
End Class

