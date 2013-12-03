Imports System.Windows.Controls.Primitives
Imports System.Linq.Enumerable

''' <summary>
''' Represents a deck of cards.
''' Provides functionality to draw a new card, shuffle,
''' and create a new deck.
''' </summary>
''' <remarks></remarks>
Public Class Shoe

#Region " Events "

    ''' <summary>
    ''' Raised whenever the deck has been changed.
    ''' </summary>
    ''' <remarks></remarks>
    Public Event DeckChanged As DeckChangedEventHandler

    Protected Overridable Sub OnDeckChanged(ByVal sender As Object, ByVal e As DeckChangedEventArgs)

        RaiseEvent DeckChanged(Me, e)

    End Sub

#End Region

#Region " Vars "

    Private m_Deck As New List(Of Card)
    Private m_currCardIndex As Integer

    Private m_Passes As Integer

    Public Const DEFAULT_SHUFFLE_PASSES As Integer = 500

    Public Enum ChangeType As Integer
        Hit = 0
        Shuffle = 1
    End Enum
#End Region

#Region " Properties "

    ''' <summary>
    ''' Gets or sets the current card that can be drawn next from the array.
    ''' </summary>
    ''' <value>Sets the current card index</value>
    ''' <returns>Returns the current card index</returns>
    ''' <remarks></remarks>
    Private Property CurrentCardIndex() As Integer
        Get
            Return m_currCardIndex
        End Get
        Set(ByVal value As Integer)
            m_currCardIndex = value
        End Set
    End Property

    ''' <summary>
    ''' How many passes to perform shuffling
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ShufflePasses() As Integer
        Get
            Return m_Passes
        End Get
        Set(ByVal value As Integer)
            m_Passes = value
        End Set
    End Property

    ''' <summary>
    ''' Holds the array of cards in the deck
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Property Cards() As List(Of Card)
        Get
            Return m_Deck
        End Get
        Set(ByVal value As List(Of Card))
            m_Deck = value
        End Set
    End Property
#End Region

#Region " Constructor Routines "

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()

        Reset()

        ' Shuffle passes
        ShufflePasses = DEFAULT_SHUFFLE_PASSES
    End Sub

    Public Sub Reset()

        CreateDeck()

        CurrentCardIndex = 0

    End Sub

    ''' <summary>
    ''' Creates the deck. (From textbook, pg. 571)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CreateDeck()

        ' Remove previous cards
        Cards.Clear()

        ' Make the first thirteen cards hearts, the
        ' next thirteen cards diamonds, and so on.
        For i As Integer = 1 To BlackJack.Current.Rules.NumberOfDecks
            For suit As Card.SuitType = Card.SuitType.Hearts To Card.SuitType.Spades
                ' Now fill the ranks
                For rank As Card.RankType = Card.RankType.Ace To Card.RankType.King
                    Cards.Add(New Card(rank, suit))
                Next
            Next
        Next
    End Sub

    ''' <summary>
    ''' Swaps a card. (From textbook, pg. 571)
    ''' </summary>
    ''' <param name="i">The first card that we are swapping</param>
    ''' <param name="j">The second card that we are swapping</param>
    ''' <remarks>Swap the ith and jth card in the deck.</remarks>
    Private Sub Swap(ByVal i As Integer, ByVal j As Integer)
        Dim tempCard As Card
        tempCard = Cards(i)
        Cards(i) = m_Deck(j)
        Cards(j) = tempCard
    End Sub

#End Region


    ''' <summary>
    ''' <para>Shuffles the deck.</para>
    ''' <para>Does x passes through the deck. On each pass, swap each card with a randomly selected card.</para>
    ''' </summary>
    ''' <remarks>
    '''</remarks>
    Public Sub Shuffle()
        Dim index As Integer
        Dim randomNum As New Random
        Dim timeBefore As DateTime = Now

        For i As Integer = 1 To ShufflePasses

            For k As Integer = 0 To Cards.Count - 1
                index = randomNum.Next(0, Cards.Count) ' randomly select number
                Swap(k, index)
            Next
        Next

        OnDeckChanged(Me, New DeckChangedEventArgs(ChangeType.Shuffle))
    End Sub

    ''' <summary>
    ''' Draws card from deck, updates index
    ''' </summary>
    ''' <returns>Returns the next card in the deck that we can draw.</returns>
    ''' <remarks></remarks>
    Public Function NextCard() As Card

        ' The new card is the next card we can draw
        Dim newCard As Card = Cards(CurrentCardIndex)

        ' Update the current card
        CurrentCardIndex += 1

        ' DEBUG
        Debugger.Instance.ShowMessage(Me, "Cards Left: " & (m_Deck.Count - CurrentCardIndex).ToString)

        OnDeckChanged(Me, New DeckChangedEventArgs(ChangeType.Hit))

        Return newCard
    End Function

    ''' <summary>
    ''' Replaces a card with a new card at a specified index of the array
    ''' </summary>
    ''' <param name="index">The index to insert at</param>
    ''' <param name="newCard">The new card to insert</param>
    ''' <remarks></remarks>
    Public Sub Replace(ByVal index As Integer, ByVal newCard As Card)
        If Not Cards Is Nothing Then Cards(index) = newCard
    End Sub

    Public Sub FillGrid(ByRef myGrid As UniformGrid)

        myGrid.Children.Clear()

        ' Fill the grid with PlayingCard controls
        For Each Card In Cards
            Dim pc As New PlayingCard()

            pc.ActualCard = Card

            ' Set the opacity to 1
            pc.Opacity = 1.0

            myGrid.Children.Add(pc)
        Next
    End Sub
End Class