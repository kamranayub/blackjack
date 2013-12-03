Imports System.Linq.Enumerable

Public Class Player
    Inherits Character

#Region " Vars "

    Private _mLblCardCount As TextBlock

    Private _mDealer As Dealer
    Private _mIsHuman As Boolean
    Private _mIsTakingTurn As Boolean

    Private _mCurrentHandIndex As Integer = 0
    Private _mNumHands As Integer

    Private _mWins As Integer
    Private _mLosses As Integer
    Private _mBank As Double

    Private _mStrategy As Strategy
    Private _mCardCountMethod As CountMethod

#End Region

    Protected Overrides Sub Character_OnHit(ByVal sender As Object, ByVal e As HitEventArgs)
        MyBase.Character_OnHit(sender, e)

        If TypeOf sender Is Player Then

            ' Count the card if it's a player
            CountCard(e.Card)

        ElseIf TypeOf sender Is Dealer Then

            ' Only count the card if it's not the Hole
            If Not e.Card.IsHole Then
                CountCard(e.Card)
            End If
        End If

        If sender Is Me Then
            ' DEBUG:
            Debugger.Instance.ShowMessage(Me, "Hit")

            ' On a hit, the only way to end the turn is to bust
            ' and reach 21 (even though you could still hit if you were silly)
            If Total >= 21 Then

                ' If the computer busted, it will know the next time
                ' it gets advice, in which case it will know to Stand
                If Me.IsHuman Then

                    ' Stand
                    PerformStand()

                End If
            End If
        End If
    End Sub

    Protected Overrides Sub Deck_OnDeckChanged(ByVal sender As Object, ByVal e As BlackJackEvents.DeckChangedEventArgs)
        MyBase.Deck_OnDeckChanged(sender, e)

        ' TODO: When we have a Shoe, make sure
        ' to tell count methods how many decks
        ' there are
        If e.TypeOfChange = Shoe.ChangeType.Shuffle Then
            ResetCount(1)
            Debugger.Instance.ShowMessage(Me, "Reset card count: " & UsingCountMethod.Count)
        End If

    End Sub

    Protected Overrides Sub Game_OnRoundEnd(ByVal sender As Object, ByVal e As EventArgs)
        MyBase.Game_OnRoundEnd(sender, e)

        ' Count dealer's Hole card
        CountCard(Dealer.CurrentHand.Cards(0))

    End Sub

#Region " Properties "

    Public Property Bank() As Double
        Get
            Return _mBank
        End Get
        Set(ByVal value As Double)
            _mBank = value
        End Set
    End Property

    Public Property IsHuman() As Boolean
        Get
            Return _mIsHuman
        End Get
        Set(ByVal value As Boolean)
            _mIsHuman = value
        End Set
    End Property

    Public Property IsTakingTurn() As Boolean
        Get
            Return _mIsTakingTurn
        End Get
        Set(ByVal value As Boolean)
            _mIsTakingTurn = value
        End Set
    End Property

    Public ReadOnly Property Wins() As Integer
        Get
            Return _mWins
        End Get
    End Property

    Public ReadOnly Property Losses() As Integer
        Get
            Return _mLosses
        End Get
    End Property

    Public ReadOnly Property Ratio() As Double
        Get
            Return Math.Round(CType(Wins, Double) / CType(Losses, Double))
        End Get
    End Property

    Private Property LabelCardCount() As TextBlock
        Get
            Return _mLblCardCount
        End Get
        Set(ByVal value As TextBlock)
            _mLblCardCount = value
        End Set
    End Property

    Public Overrides ReadOnly Property CurrentHand() As Hand
        Get
            Return Hands(_mCurrentHandIndex)
        End Get
    End Property

    Private ReadOnly Property Dealer() As Dealer
        Get
            Return _mDealer
        End Get
    End Property

    Private ReadOnly Property NumberOfHands() As Integer
        Get
            Return _mNumHands
        End Get
    End Property

    Protected Overrides ReadOnly Property LastCard() As Card
        Get
            Return Last(CurrentHand.Cards)
        End Get
    End Property

    Public Property UsingStrategy() As Strategy
        Get
            Return _mStrategy
        End Get
        Set(ByVal value As Strategy)
            _mStrategy = value
        End Set
    End Property
    Public Property UsingCountMethod() As CountMethod
        Get
            Return _mCardCountMethod
        End Get
        Set(ByVal value As CountMethod)
            _mCardCountMethod = value
        End Set
    End Property

#End Region

    ''' <summary>
    ''' Constructor. Creates a new Player.
    ''' </summary>
    ''' <param name="useDeck">The deck we are using</param>
    ''' <param name="myCtrl">The associated user control</param>
    ''' <remarks></remarks>
    Public Sub New(ByRef useDeck As Shoe, ByVal myCtrl As UCCharacter)
        MyBase.New(useDeck, myCtrl)

        LabelCardCount = UIControl.LabelCardCount

        _mNumHands = 1
    End Sub

    ''' <summary>
    ''' Sets the current game's dealer
    ''' </summary>
    ''' <param name="theDealer"></param>
    ''' <remarks></remarks>
    Public Sub SetDealer(ByRef theDealer As Dealer)
        _mDealer = theDealer
    End Sub

    ''' <summary>
    ''' Advances to the next available Hand
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub NextHand()
        _mCurrentHandIndex += 1
    End Sub

    Private Sub ResetCount(ByVal numDecks As Integer)
        If UsingCountMethod IsNot Nothing Then
            UsingCountMethod.Reset(numDecks)

            RefreshCount()
        End If
    End Sub

    Private Sub CountCard(ByVal newCard As Card)
        If UsingCountMethod IsNot Nothing Then
            UsingCountMethod.CountCard(newCard)

            RefreshCount()

            ' DEBUG:
            Debugger.Instance.ShowMessage(Me, "Counted card " & newCard.IdentifyCard & " (Running Count: " & UsingCountMethod.Count & ")")
        End If
    End Sub

    Public Overrides Sub PerformHit()

        CurrentHand.Add()

        OnHit(Me, New HitEventArgs(LastCard, False))
    End Sub


    Protected Overrides Function GetOutcome() As BlackJack.OutcomeType

        Dim returnValue As BlackJack.OutcomeType = BlackJack.OutcomeType.None
        Dim dealerBJ As Boolean

        If Total() > 0 Then
            dealerBJ = Dealer.IsBlackJack

            If Total() > 21 Then
                returnValue = BlackJack.OutcomeType.Bust
            ElseIf IsBlackJack AndAlso Not dealerBJ AndAlso NumberOfHands = 1 Then
                returnValue = BlackJack.OutcomeType.BlackJack
            ElseIf Dealer.Total > 21 Then
                returnValue = BlackJack.OutcomeType.Won
            ElseIf Total < Dealer.Total Then
                returnValue = BlackJack.OutcomeType.Lost
            ElseIf Total > Dealer.Total Then
                returnValue = BlackJack.OutcomeType.Won
            ElseIf Total = Dealer.Total Then
                returnValue = BlackJack.OutcomeType.Push
            Else
                returnValue = BlackJack.OutcomeType.None
            End If

        End If

        Return returnValue
    End Function

    Public Function GetAdvice() As Strategy.AdviceType
        ' If this hand is bust, we should stand
        ' Note: This is useful because AI players
        ' will get advice if they hit and bust.
        If GetOutcome() = BlackJack.OutcomeType.Bust Then
            Return Strategy.AdviceType.Stand
        End If

        Dim dealerUpCard As Card = Dealer.CurrentHand.Cards(1)
        Dim playerStrategy As Strategy
        Dim cardCount As Double

        If UsingStrategy IsNot Nothing Then
            playerStrategy = UsingStrategy
        Else
            playerStrategy = Nothing
        End If

        If UsingCountMethod IsNot Nothing Then
            cardCount = UsingCountMethod.Count
        Else
            cardCount = 0
        End If

        Dim allowSplit As Boolean = True
        IIf(Not NumberOfHands = 2, allowSplit = True, allowSplit = False)

        Return CurrentHand.GetAdvice(dealerUpCard, _
                                     playerStrategy, _
                                     allowSplit, _
                                     cardCount)
    End Function

    ''' <summary>
    ''' Resets internal vars
    ''' </summary>
    ''' <remarks></remarks>
    Protected Overrides Sub Reset()
        MyBase.Reset()

        ' Current hand
        _mCurrentHandIndex = 0
        _mNumHands = 1

        ' Fade out
        UIControl.PlayTurnEnd()
    End Sub

    Private Sub RefreshCount()
        If UsingCountMethod IsNot Nothing Then
            If UsingCountMethod.Count = 0 Then
                LabelCardCount.Visibility = Visibility.Hidden
                LabelCardCount.Text = "0"
            Else
                LabelCardCount.Visibility = Visibility.Visible
                LabelCardCount.Text = FormatNumber(UsingCountMethod.Count, 2)
            End If

        End If
    End Sub

    ''' <summary>
    ''' Gets whether or not the Player is on their last hand
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsLastHand() As Boolean
        Return _mCurrentHandIndex + 1 = NumberOfHands
    End Function
End Class
