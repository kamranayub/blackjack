''' <summary>
''' The Base Class for any character in the game
''' </summary>
''' <remarks>
''' TODO:
''' - Have Reset subscribe to a BlackJack event OnRoundStart()
''' </remarks>
Public MustInherit Class Character

    Private _mDeck As Shoe
    Private _mHands() As Hand

    ' UI
    Private _mControl As UCCharacter

    ''' <summary>
    ''' Constructor. Sets up the player for first time use.
    ''' </summary>
    ''' <param name="theDeck">The current Deck in play</param>
    ''' <param name="myCtrl">The UCCharacter control to associate this Player with</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal theDeck As Shoe, ByVal myCtrl As UCCharacter)

        Dim theGame As BlackJack = BlackJack.Current

        UIControl = myCtrl

        _mDeck = theDeck

        Reset()

        ' Subsribe to events
        AddHandler theDeck.DeckChanged, AddressOf Deck_OnDeckChanged
        AddHandler theGame.PlayerTurnStart, AddressOf Player_OnTurnStart
        AddHandler theGame.PlayerTurnEnd, AddressOf Player_OnTurnEnd
        AddHandler theGame.RoundStart, AddressOf Game_OnRoundStart
        AddHandler theGame.RoundEnd, AddressOf Game_OnRoundEnd
    End Sub

    ''' <summary>
    ''' Attach ourselves to other player's events, so we know
    ''' what's going on.
    ''' </summary>
    ''' <param name="dealer"></param>
    ''' <param name="players"></param>
    ''' <remarks></remarks>
    Public Sub Listen(ByVal dealer As Dealer, ByVal players As List(Of Player))

        For Each aPlayer In players
            ' First remove a handler if we have one already
            RemoveHandler aPlayer.Hit, AddressOf Character_OnHit
            AddHandler aPlayer.Hit, AddressOf Character_OnHit
        Next

        RemoveHandler dealer.Hit, AddressOf Character_OnHit
        AddHandler dealer.Hit, AddressOf Character_OnHit

    End Sub

#Region " Properties "

    ''' <summary>
    ''' The Character's current hands
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Property Hands() As Hand()
        Get
            Return _mHands
        End Get
        Set(ByVal value As Hand())
            _mHands = value
        End Set
    End Property

    ''' <summary>
    ''' The UI <see cref="UCCharacter" /> control associated with this
    ''' Character
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property UIControl() As UCCharacter
        Get
            Return _mControl
        End Get
        Set(ByVal value As UCCharacter)
            _mControl = value
        End Set
    End Property

    ''' <summary>
    ''' Gets whether or not the Character has a BlackJack
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsBlackJack() As Boolean
        Get
            ' Whether or not the hand is a BlackJack
            ' Must be two cards
            ' Ace + Face/10
            If CurrentHand.Total > 0 Then
                If CurrentHand.Cards.Count = 2 And CurrentHand.Total = 21 Then
                    Return True
                End If
            End If

            Return False
        End Get
    End Property

    ''' <summary>
    ''' Gets the current running total of the Character's hands
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable ReadOnly Property Total() As Integer
        Get
            'TODO: Add value of hands?
            Return Hands(0).Total
        End Get
    End Property

    Public ReadOnly Property Outcome() As BlackJack.OutcomeType
        Get
            Return GetOutcome()
        End Get
    End Property

    Protected MustOverride ReadOnly Property LastCard() As Card
    Public MustOverride ReadOnly Property CurrentHand() As Hand

#End Region


#Region " Events "

    Public Event Hit As HitEventHandler
    Public Event Stand As StandEventHandler

    Protected Overridable Sub OnHit(ByVal sender As Object, ByVal e As HitEventArgs)

        RaiseEvent Hit(Me, e)

        RefreshTotal()
    End Sub

    Protected Overridable Sub OnStand(ByVal sender As Object, ByVal e As EventArgs)

        RaiseEvent Stand(Me, e)

    End Sub

#End Region

    Protected MustOverride Function GetOutcome() As BlackJack.OutcomeType
    Public MustOverride Sub PerformHit()

    ''' <summary>
    ''' Resets the Character for a new round.
    ''' </summary>
    ''' <remarks></remarks>
    Protected Overridable Sub Reset()
        Hands = New Hand(1) {}

        Hands(0) = New Hand(Me, _mDeck, UIControl.DrawingSurface)
        Hands(1) = New Hand(Me, _mDeck, UIControl.DrawingSurface)

        RefreshTotal()
        RefreshOutcome()

        UIControl.DrawingSurface.Children.Clear()
    End Sub

    Public Sub PerformHit(ByVal IsDeal As Boolean)

        If IsDeal Then
            CurrentHand.Add()
            RefreshTotal()
        Else

            PerformHit()

        End If

    End Sub
    ''' <summary>
    ''' Makes the player Stand, raises OnStand
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub PerformStand()
        OnStand(Me, Nothing)
    End Sub

    ''' <summary>
    ''' Refreshes the Character's outcome
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub RefreshOutcome()
        If Outcome = BlackJack.OutcomeType.None AndAlso Total <= 0 Then
            UIControl.SetOutcome("", False)
        ElseIf Outcome = BlackJack.OutcomeType.None Then
            UIControl.SetTotal(Total)
        Else
            UIControl.SetOutcome([Enum].GetName(GetType(BlackJack.OutcomeType), GetOutcome), True)
        End If
    End Sub

    ''' <summary>
    ''' Refreshes the Character's total hand value
    ''' </summary>
    ''' <remarks></remarks>
    Public Overridable Sub RefreshTotal()
        UIControl.SetTotal(Total)
    End Sub

#Region " Event Handlers  "

    ''' <summary>
    ''' Listen for the start of a Player's turn
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub Player_OnTurnStart(ByVal sender As Object, ByVal e As PlayerTurnEventArgs)

        e.Player.IsTakingTurn = True

    End Sub

    ''' <summary>
    ''' Listen for the end of a Player's turn
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub Player_OnTurnEnd(ByVal sender As Object, ByVal e As PlayerTurnEventArgs)

        e.Player.IsTakingTurn = False

    End Sub

    ''' <summary>
    ''' Listen for other character's Hits.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Overridable Sub Character_OnHit(ByVal sender As Object, ByVal e As HitEventArgs)

        Debugger.Instance.ShowMessage(Me, "I heard a hit from " & sender.GetType.ToString)

    End Sub

    ''' <summary>
    ''' Listen for stands.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Overridable Sub Player_OnStand(ByVal sender As Object, ByVal e As EventArgs)



    End Sub

    Protected Overridable Sub Deck_OnDeckChanged(ByVal sender As Object, ByVal e As DeckChangedEventArgs)

        ' DeckChanged is raised on hits and shuffles.

    End Sub

    Protected Overridable Sub Game_OnRoundStart(ByVal sender As Object, ByVal e As EventArgs)

        Reset()

    End Sub

    Protected Overridable Sub Game_OnRoundEnd(ByVal sender As Object, ByVal e As EventArgs)

        ' Refresh outcomes
        RefreshOutcome()

    End Sub

#End Region
End Class
