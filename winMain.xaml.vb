Imports System.Threading
Imports System.Media
Imports System.IO

Class winMain


#Region " Properties and Vars "

    Private WithEvents m_Game As BlackJack

    ''' <summary>
    ''' Gets or sets the current BlackJack game
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property Game() As BlackJack
        Get
            Return m_Game
        End Get
        Set(ByVal value As BlackJack)
            m_Game = value
        End Set
    End Property

#End Region


    Private Sub winMain_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles MyBase.Loaded
        ' Start a new game
        Game = BlackJack.Current

        ' Add players
        Game.AddDealer(playerDealer)
        Game.AddPlayer(playerHuman, True)

        ' Test add players
        Game.AddPlayer(AIPlayer1, False)
        Game.AddPlayer(AIPlayer2, False)
        Game.AddPlayer(AIPlayer3, False)
        Game.AddPlayer(AIPlayer4, False)

        ' Focus
        btnNewHand.Focus()
        btnHit.IsEnabled = False
        btnStand.IsEnabled = False

        ' Set amount of money player has
        lblBet.Text = String.Format("Bet: {0}", FormatCurrency(Game.GetHumanPlayer.Bank, 0))
    End Sub

#Region " Game Events "

    ''' <summary>
    ''' A new betting round has started
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub BlackJack_RoundStart(ByVal sender As Object, ByVal e As EventArgs) Handles m_Game.RoundStart

        ' Cheats
        Dim Cheats As New BlackJack.CheatTypes
        Cheats.DealSoft17 = menuCheatsSoft17.IsChecked
        Cheats.AlwaysShowHole = menuCheatsShowHole.IsChecked
        Cheats.DealBlackJackAll = menuCheatsBJAll.IsChecked
        Game.Cheats = Cheats

        ' Reset vars
        progressShuffle.Minimum = 1
        progressShuffle.Maximum = Game.ShufflePasses

        ' Buttons
        btnHit.IsEnabled = False
        btnStand.IsEnabled = False
        btnNewHand.IsEnabled = False
    End Sub

    ''' <summary>
    ''' Resets the form for a game over
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub BlackJack_OnRoundEnd(ByVal sender As Object, ByVal e As EventArgs) Handles m_Game.RoundEnd

        ' Find and determine the outcomes
        GetOutcomes()

        ' Update stats
        lblWins.Text = String.Format("Wins: {0}", Game.GetHumanPlayer.Wins.ToString)
        lblLosses.Text = String.Format("Losses: {0}", Game.GetHumanPlayer.Losses.ToString)
        lblRatio.Text = String.Format("Ratio: {0}", FormatNumber(Game.GetHumanPlayer.Ratio, 2))
        lblBet.Text = String.Format("Bet: {0}", FormatCurrency(Game.GetHumanPlayer.Bank, 0))

        ' Buttons
        btnStand.IsEnabled = False
        btnHit.IsEnabled = False
        btnNewHand.IsEnabled = True
        btnNewHand.Focus()
    End Sub

    ''' <summary>
    ''' Performs UI functions on a player's start turn
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub BlackJack_OnPlayerTurnStart(ByVal sender As Object, ByVal e As PlayerTurnEventArgs) Handles m_Game.PlayerTurnStart
        ' DEBUG:
        Debugger.Instance.ShowMessage(sender, "Starting a turn")

        If e.Player.IsHuman Then
            btnStand.IsEnabled = True
            btnHit.IsEnabled = True
            btnNewHand.IsEnabled = False
        End If

        e.Player.UIControl.PlayTurnStart()
    End Sub

    ''' <summary>
    ''' Performs UI functions on a player's end turn
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub BlackJack_OnPlayerTurnEnd(ByVal sender As Object, ByVal e As PlayerTurnEventArgs) Handles m_Game.PlayerTurnEnd
        ' DEBUG:
        Debugger.Instance.ShowMessage(sender, "Ending a turn")

        If e.Player.IsHuman Then
            ' Disable all buttons
            ' while we wait for everyone to finish
            btnStand.IsEnabled = False
            btnHit.IsEnabled = False
            btnNewHand.IsEnabled = False
        End If

        e.Player.UIControl.PlayTurnEnd()
    End Sub

#End Region

#Region " Game Routines "

    ''' <summary>
    ''' Displays the message to send to the user
    ''' based on the outcome of the game.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetOutcomes()

        ' Now, get the human player's result
        Select Case Game.GetHumanPlayer.Outcome()
            Case BlackJack.OutcomeType.BlackJack : PlayerWin()
            Case BlackJack.OutcomeType.Won : PlayerWin()
            Case BlackJack.OutcomeType.Bust : PlayerLose()
            Case BlackJack.OutcomeType.Lost : PlayerLose()
            Case BlackJack.OutcomeType.Push : PlayerPush()
        End Select
    End Sub

    ''' <summary>
    ''' Perform a Player win scenario
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PlayerWin()
        PlaySound("Win", True)
    End Sub

    ''' <summary>
    ''' Perform a Dealer win scenario
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PlayerLose()
        PlaySound("Lose", True)
    End Sub

    ''' <summary>
    ''' Perform a Push outcome
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PlayerPush()
        PlaySound("Push", True)
    End Sub
#End Region

#Region " Control Event Handlers "

    Private Sub btnNewHand_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)

        ' Shuffle passes
        ' TODO: VALIDATE
        Game.ShufflePasses = CInt(txtNoOfShuffles.Text)

        ' Audio 
        PlaySound("Shuffle", True)

        ' Fade out players
        ' Game.FadeOutAllPlayers()

        ' Start the round
        Game.StartRound()
    End Sub

    Private Sub btnHit_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnHit.Click
        ' Don't hit unless game has started
        If Not Game Is Nothing Then

            ' Play draw card sound
            PlaySound("Draw", False)

            ' Player draws one card
            Game.GetHumanPlayer.PerformHit()
        End If
    End Sub

    Private Sub btnStand_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnStand.Click
        ' Don't hit unless game has started
        If Not Game Is Nothing Then
            ' Disable Hit button, even though
            ' the outcome is decided too quick
            btnHit.IsEnabled = False

            ' Player stands
            Game.GetHumanPlayer.PerformStand()
        End If
    End Sub

    Private Sub menuShowDeck_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Game.ShowDeckDialog(Me)
    End Sub

    Private Sub menuExit_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.Close()
    End Sub

#End Region

    Private myMediaPlayer As MediaPlayer


    ''' <summary>
    ''' Play a specified sound clip
    ''' </summary>
    ''' <param name="Sound">The Resource Sound to play</param>
    ''' <param name="ASync">Whether or not to play the sound Async</param>
    ''' <remarks></remarks>
    Private Sub PlaySound(ByVal Sound As String, ByVal ASync As Boolean)
        If Not chkMute.IsChecked Then
            Dim soundLoc As String = "Library\Sounds\" & Sound & ".wav"

            If File.Exists(soundLoc) Then
                myMediaPlayer = New MediaPlayer
                myMediaPlayer.Open(New Uri(soundLoc, UriKind.Relative))
                myMediaPlayer.Play()

            End If
        End If
    End Sub

    ''' <summary>
    ''' Enables and disables showing Debug messages
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub menuDebugEnableMsgs_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
#If DEBUG Then

        ' Only allow this in Debug mode
        Debugger.Instance.MessagesEnabled = menuDebugEnableMsgs.IsChecked

#End If
    End Sub

    Private Sub NumberOfDecks_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)

        Dim itemClicked As MenuItem = CType(sender, MenuItem)

        For Each Item As MenuItem In menuNumberOfDecks.Items
            If Item IsNot itemClicked Then
                Item.IsChecked = False
            End If
        Next

    End Sub
End Class
