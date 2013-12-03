Imports System.Timers
Imports System.Windows.Threading
Imports System.Threading

''' <summary>
''' Encapsulating class for BlackJack.
''' Controls and manages a BlackJack game.
''' 
''' <para>TODO List:</para>
''' <para>* Create a Shoe to represent multiple Decks.</para>
''' <para>* Add support for Split and Double Down</para>
''' <para>* Add support for a "betting box" and player bets</para>
''' </summary>
''' <remarks></remarks>
Public Class BlackJack

#Region " Events "

    ''' <summary>
    ''' Raised when a new round starts
    ''' </summary>
    ''' <remarks></remarks>
    Public Event RoundStart As RoundStartEventHandler

    ''' <summary>
    ''' Raised when a round is over
    ''' </summary>
    ''' <remarks></remarks>
    Public Event RoundEnd As RoundEndEventHandler

    ''' <summary>
    ''' Raised when a <see cref="Player"/> starts their turn.
    ''' </summary>
    ''' <remarks></remarks>
    Public Event PlayerTurnStart As PlayerTurnStartEventHandler

    ''' <summary>
    ''' Raised when a <see cref="Player"/> ends their turn
    ''' </summary>
    ''' <remarks></remarks>
    Public Event PlayerTurnEnd As PlayerTurnEndEventHandler

    Protected Overridable Sub OnRoundStart(ByVal sender As Object, ByVal e As EventArgs)
        RaiseEvent RoundStart(Me, e)
    End Sub

    Protected Overridable Sub OnRoundEnd(ByVal sender As Object, ByVal e As EventArgs)
        RaiseEvent RoundEnd(Me, e)
    End Sub

    Protected Sub OnPlayerTurnStart(ByVal sender As Object, ByVal e As PlayerTurnEventArgs)
        RaiseEvent PlayerTurnStart(Me, e)
    End Sub

    Protected Sub OnPlayerTurnEnd(ByVal sender As Object, ByVal e As PlayerTurnEventArgs)
        RaiseEvent PlayerTurnEnd(Me, e)
    End Sub

#End Region

#Region " Properties "

    Private Shared _mInstance As BlackJack

    Private _mRules As Rules
    Private _mDeck As Shoe
    Private WithEvents _mDealer As Dealer
    Private WithEvents _mTurnTimer As DispatcherTimer
    Private WithEvents _mAIPerformTurnTimer As DispatcherTimer

    Private _mViewDeckWindow As winViewDeck
    Private _mCurrentPlayerIndex As Integer
    Private _mShufflePasses As Integer
    Private _mTicks As Integer = 0
    Private _mPlayers As List(Of Player)
    Private _mOutcome As OutcomeType
    Private _mCheats As CheatTypes

    Private Const INTERVAL_AI As Integer = 500     ' Millisecond interval between AI Player turns
    Private Const INTERVAL_GAME As Integer = 200    ' Millisecond interval between game state polling

    ''' <summary>
    ''' The game's outcome
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum OutcomeType As Integer
        None
        BlackJack
        Push
        Bust
        Won
        Lost
    End Enum

    ''' <summary>
    ''' The different types of cheats we can enable
    ''' </summary>
    ''' <remarks></remarks>
    Structure CheatTypes
        Dim DealSoft17 As Boolean
        Dim AlwaysShowHole As Boolean
        Dim DealBlackJackAll As Boolean
    End Structure

    ''' <summary>
    ''' Gets or sets the Rules for the game
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Rules() As Rules
        Get
            Return _mRules
        End Get
        Set(ByVal value As Rules)
            _mRules = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the cheats we are using
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Cheats() As CheatTypes
        Get
            Return _mCheats
        End Get
        Set(ByVal value As CheatTypes)
            _mCheats = value
        End Set
    End Property


    ''' <summary>
    ''' How many passes to make when shuffling cards.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ShufflePasses() As Integer
        Get
            Return _mShufflePasses
        End Get
        Set(ByVal value As Integer)
            _mShufflePasses = value
        End Set
    End Property

    ''' <summary>
    ''' For the UI, gets the human player object
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetHumanPlayer() As Player
        Get
            For Each thePlayer In Players
                If thePlayer.IsHuman Then
                    Return thePlayer
                End If
            Next

            Return Nothing
        End Get
    End Property

    ''' <summary>
    ''' Gets the <see cref="Dealer"/> object
    ''' </summary>
    ''' <returns>Returns the <see cref="Dealer"/> object</returns>
    ''' <remarks></remarks>
    Private ReadOnly Property Dealer() As Dealer
        Get
            Return _mDealer
        End Get
    End Property

    ''' <summary>
    ''' The deck of cards we are playing with
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property Deck() As Shoe
        Get
            Return _mDeck
        End Get
        Set(ByVal value As Shoe)
            _mDeck = value
        End Set
    End Property

    ''' <summary>
    ''' Gets the Player Hand
    ''' </summary>
    ''' <value></value>
    ''' <returns>Returns the <see cref="Hand"/> representing the player</returns>
    ''' <remarks></remarks>
    Private ReadOnly Property Players() As List(Of Player)
        Get
            Return _mPlayers
        End Get
    End Property

    ''' <summary>
    ''' Holds the current player who is taking their turn
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property CurrentPlayerIndex() As Integer
        Get
            Return _mCurrentPlayerIndex
        End Get
        Set(ByVal value As Integer)
            _mCurrentPlayerIndex = value
        End Set
    End Property

    ''' <summary>
    ''' A shortcut to get the current <see cref="Player"/> object
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private ReadOnly Property GetCurrentPlayer() As Player
        Get
            Return CType(Players(CurrentPlayerIndex), Player)
        End Get
    End Property

    ''' <summary>
    ''' Returns the number of players currently in play
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private ReadOnly Property NumberOfPlayers() As Integer
        Get
            Return Players.Count
        End Get
    End Property

    ''' <summary>
    ''' Gets the current and only instance of BlackJack
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared ReadOnly Property Current() As BlackJack
        Get
            Return _mInstance
        End Get
    End Property
#End Region

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Shared Sub New()
        ' Initialize the one and only instance of this class
        _mInstance = New BlackJack

        ' Initialize default rules
        Current.Rules = New Rules

        ' Initialize an array that can hold 5 players
        Current._mPlayers = New List(Of Player)

        ' Current player
        Current.CurrentPlayerIndex = 0

        ' Initialize reference to a Deck
        Current.Deck = New Shoe()

        ' Shuffle Passes
        Current.ShufflePasses = Shoe.DEFAULT_SHUFFLE_PASSES
    End Sub

    Private Sub New()

    End Sub

    ''' <summary>
    ''' Displays the Show Deck window to view
    ''' the current deck.
    ''' </summary>
    ''' <param name="parent"></param>
    ''' <remarks></remarks>
    Public Sub ShowDeckDialog(ByRef parent As Window)


        ' Show deck dialog
        If _mViewDeckWindow Is Nothing Then
            _mViewDeckWindow = New winViewDeck()

            ' Show it
            _mViewDeckWindow.Show()
            ' Update it
            UpdateDeckDialog()
        End If

        _mViewDeckWindow.Owner = parent
    End Sub

    ''' <summary>
    ''' Adds the new dealer to the game
    ''' </summary>
    ''' <param name="userControl"></param>
    ''' <remarks></remarks>
    Public Sub AddDealer(ByRef userControl As UCCharacter)

        Dim newDlr As Dealer = New Dealer(Deck, userControl)

        _mDealer = newDlr

    End Sub

    ''' <summary>
    ''' Adds a new player to the game
    ''' </summary>
    ''' <param name="userControl"></param>
    ''' <remarks></remarks>
    Public Function AddPlayer(ByRef userControl As UCCharacter, ByVal bIsHuman As Boolean) As Player

        ' Set up new player
        Dim newPlayer As New Player(Deck, userControl)

        ' Whether or not they're human
        newPlayer.IsHuman = bIsHuman

        ' The current dealer of the game
        newPlayer.SetDealer(Dealer)

        ' Set counting method and strategy
        newPlayer.UsingStrategy = New BasicSingleDeck()
        newPlayer.UsingCountMethod = New HiLo(Rules.NumberOfDecks)

        ' Set up event handlers
        AddHandler newPlayer.Stand, AddressOf Character_OnStand

        ' Adds them to the next available slot
        Players.Add(newPlayer)

        Return newPlayer
    End Function

    Public Sub RemovePlayer(ByVal ctrl As UCCharacter)
        For Each player In Players
            If player.UIControl Is ctrl Then
                ' Found the player
                ' Remove him
                Players.Remove(player)

                ' Hide the control
                ctrl.Visibility = Visibility.Hidden
                Exit For
            End If
        Next
    End Sub

    ''' <summary>
    ''' Raises the OnGameStart event
    ''' and Shuffles deck
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StartRound()

        ' Have all players listen to each other
        For Each player In Players
            player.Listen(Dealer, Players)
        Next

        ' Reset for new round
        Reset()

        OnRoundStart(Me, Nothing)

        ' Shuffle
        ' TODO: Move this to the Shoe
        Deck.Shuffle()

        ' Cheats
        OnBeforeDeal()

        ' Update deck dialog
        UpdateDeckDialog()

        ' Deal cards
        Deal()
    End Sub

    ''' <summary>
    ''' Resets the game for a new hand
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Reset()

        ' Current player
        CurrentPlayerIndex = 0

        ' TODO: Implement shoe that
        ' decides when to shuffle
        Deck.Reset()
        Deck.ShufflePasses = ShufflePasses

    End Sub

    ''' <summary>
    ''' Updates the Deck dialog, if it's open.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UpdateDeckDialog()

        ' Show deck dialog
        If _mViewDeckWindow IsNot Nothing Then
            ' Update grid
            If Deck Is Nothing Then Exit Sub

            _mViewDeckWindow.Deck = Deck
            _mViewDeckWindow.FillGrid(Players.Count)
        End If

    End Sub

    ''' <summary>
    ''' Raised before a deal. Used for enabling cheats.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub OnBeforeDeal()

        ' Always deal a soft 17 to the dealer
        If Cheats.DealSoft17 Then
            Dim aCard As Card
            Dim dealerIndex As Integer = 0

            dealerIndex = NumberOfPlayers
            aCard = New Card(Card.RankType.Ace, Card.SuitType.Hearts)
            Deck.Replace(dealerIndex, aCard)

            dealerIndex = (NumberOfPlayers * 2) + 1
            aCard = New Card(Card.RankType.Six, Card.SuitType.Hearts)
            Deck.Replace(dealerIndex, aCard)

        ElseIf Cheats.DealBlackJackAll Then
            Dim aCard As Card

            ' TODO: Add support for multiple players
            Dim dealerIndex As Integer = 0

            ' The dealer's first card index will be
            ' NumPlayers
            dealerIndex = NumberOfPlayers
            aCard = New Card(Card.RankType.Ace, Card.SuitType.Clubs)
            Deck.Replace(dealerIndex, aCard)

            dealerIndex = (NumberOfPlayers * 2) + 1
            aCard = New Card(Card.RankType.Ten, Card.SuitType.Hearts)
            Deck.Replace(dealerIndex, aCard)
        End If
    End Sub

    ''' <summary>
    ''' Initial deal of two cards to player and dealer
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    Private Sub Deal()
        ' Deal cards
        For i As Integer = 1 To 2
            For Each p In Players
                p.PerformHit(True)
            Next

            Dealer.PerformHit(True)
        Next

        ' This hides the Hole to if cheats aren't enabled
        Dealer.ShowHole = Cheats.AlwaysShowHole

        ' Dealer has a BlackJack? End the game!
        If Dealer.IsBlackJack Then
            OnRoundEnd(Me, Nothing)
            Exit Sub
        End If

        ' Let's start the game!
        _mTurnTimer = New DispatcherTimer()
        _mTurnTimer.Interval = TimeSpan.FromMilliseconds(INTERVAL_GAME)
        _mTurnTimer.Start()

        ' DEBUG:
        Debugger.Instance.ShowMessage(Me, "TurnTimer is ticking...")
    End Sub

    ''' <summary>
    ''' Advances the current players hand for Splits
    ''' and makes them Hit.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub NextHand()

        ' Advance to the player's next hand (for splits)
        GetCurrentPlayer.NextHand()

        ' They always get another card on a new, split hand
        GetCurrentPlayer.PerformHit()

    End Sub

    ''' <summary>
    ''' Advance the CurrentPlayerIndex
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub NextPlayer()
        CurrentPlayerIndex += 1
    End Sub

    ''' <summary>
    ''' Takes care of performing the Dealer's round logic.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PerformDealerTurn()

        ' Everyone has either stood or busted
        If Dealer.IsBlackJack Then
            Dealer.PerformStand()
            Exit Sub
        End If

        ' If everyone has busted, the dealer can just stand
        ' because he wins
        If Players.All(Function(p) p.Total > 21) Then
            Dealer.PerformStand()
            Exit Sub
        End If

        ' Not a BJ, and people still have playable hands
        ' so let's finish our turn
        Dealer.PerformHit()

    End Sub

    ''' <summary>
    ''' Takes care of AI and Human turns
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PerformPlayerTurn()

        ' Get the current player
        Dim thePlayer As Player = GetCurrentPlayer

        ' If it's a human, we don't need to do
        ' anything.
        If thePlayer.IsHuman Then

            ' Start turn
            ' Prevent event from firing each time the turnTimer ticks
            If Not thePlayer.IsTakingTurn Then
                RaiseEvent PlayerTurnStart(thePlayer, New PlayerTurnEventArgs(thePlayer))
            End If

            ' Don't do anything else
            Exit Sub
        End If


        If Not _mAIPerformTurnTimer Is Nothing AndAlso _mAIPerformTurnTimer.IsEnabled Then
            ' The AI is performing their turn
            ' Do nothing
            Exit Sub
        Else
            ' Start the AI's turn before we actually do it
            ' so that the UI can make it seem like
            ' something's happened
            If Not thePlayer.IsTakingTurn Then
                RaiseEvent PlayerTurnStart(thePlayer, New PlayerTurnEventArgs(thePlayer))
            End If

            ' Start their turn in one second
            _mAIPerformTurnTimer = New DispatcherTimer
            _mAIPerformTurnTimer.Interval = TimeSpan.FromMilliseconds(INTERVAL_AI)
            _mAIPerformTurnTimer.Start()
        End If
    End Sub

    Public Sub FadeInAllPlayers()
        For Each p In Players
            p.UIControl.PlayTurnStart()
        Next
    End Sub

    Public Sub FadeOutAllPlayers()
        For Each p In Players
            p.UIControl.PlayTurnEnd()
        Next
    End Sub

#Region " Game Event Handlers "

    ''' <summary>
    ''' Runs throughout a round.
    ''' Checks to see who is currently playing and takes action.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Game_OnTurnTimerElapsed(ByVal sender As Object, ByVal e As EventArgs) Handles _mTurnTimer.Tick

        ' CurrentPlayerIndex = no of players when everyone has performed
        ' their turn
        If NumberOfPlayers = CurrentPlayerIndex Then

            ' Stop the timer
            If Not _mTurnTimer Is Nothing AndAlso _mTurnTimer.IsEnabled Then _mTurnTimer.Stop()

            ' DEBUG
            Debugger.Instance.ShowMessage(Me, "TurnTimer stopped. " & _mTicks.ToString & " ticks elapsed.")

            ' Reset tick counter
            _mTicks = 0

            ' We've reached the end of the players
            ' Now we finish the game
            PerformDealerTurn()
        Else

            ' The round is still in progress, check players
            PerformPlayerTurn()
        End If

        ' DEBUG: Increment tick counter
        _mTicks += 1
    End Sub

    ''' <summary>
    ''' Raised when the AITurnTimer Tick event is fired.
    ''' Handles the logic for performing AI playing functions.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks>
    ''' <para>There are several important things to note about this routine:</para>
    ''' <list type="1">
    ''' <item>An AI player will perform a turn even if they have a Busted hand. The Player's GetAdvice
    ''' function will automatically return a Stand advice if it detects a Bust. This is to keep the
    ''' appearance of a smooth transition between players.</item>
    ''' <item>We stop the timer so that we only have one tick to perform a turn.</item>
    ''' </list>
    ''' </remarks>
    Private Sub Game_OnAIPerformTurn(ByVal sender As Object, ByVal e As EventArgs) Handles _mAIPerformTurnTimer.Tick

        Dim thePlayer As Player = GetCurrentPlayer
        Dim someAdvice As Strategy.AdviceType = Strategy.AdviceType.None

        ' Stop the timer, in case getting advice takes awhile and perform AI's turn
        If Not _mAIPerformTurnTimer Is Nothing AndAlso _mAIPerformTurnTimer.IsEnabled Then _mAIPerformTurnTimer.Stop()

        ' TODO: Make this its own worker thread? That will prevent longer
        ' strategies from holding the UI thread up.
        someAdvice = thePlayer.GetAdvice()

        ' DEBUG:
        Debugger.Instance.ShowMessage(thePlayer, "I was advised to " & someAdvice.ToString)

        Select Case someAdvice
            Case Strategy.AdviceType.Stand
                thePlayer.PerformStand()
            Case Strategy.AdviceType.Hit
                thePlayer.PerformHit()
            Case Else
                ' Right now don't worry about other kinds of advice
                thePlayer.PerformStand()
        End Select
    End Sub

    ''' <summary>
    ''' Handles <see cref="Character.OnStand"/> event.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Character_OnStand(ByVal sender As Object, ByVal e As EventArgs) Handles _mDealer.Stand

        ' If this is a player, move to the next player
        If TypeOf sender Is Player Then

            Dim thePlayer As Player = CType(sender, Player)

            ' Signal their turn is over
            OnPlayerTurnEnd(Me, New PlayerTurnEventArgs(thePlayer))

            NextPlayer()

        ElseIf TypeOf sender Is Dealer Then

            ' The dealer has stood, let's finish this.
            Dealer.ShowHole = True

            OnRoundEnd(Me, Nothing)
        End If

    End Sub

    Private Sub Game_OnRoundEnd() Handles Me.RoundEnd

        ' Iterate players, tell them to fade in their cards
        FadeInAllPlayers()

    End Sub

#End Region
End Class
