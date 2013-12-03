Imports System.Threading
Imports System.Linq.Enumerable


''' <summary>
''' Represents a generic hand of cards
''' </summary>
''' <remarks></remarks>
Public Class Hand

#Region " Vars "

    Private m_Cards As List(Of Card)       ' Cards in our hand, max in any hand is 12 cards
    Protected m_Deck As Shoe        ' Current deck we are using
    Protected m_Surface As CardStacker   ' Associated canvas to draw cards on

#End Region

#Region " Properties "

    ''' <summary>
    ''' Gets or sets an array of Card objects. Represents the current hand.
    ''' </summary>
    ''' <value>Replaces the old array of cards with a new one.</value>
    ''' <returns>Returns an array of Card objects</returns>
    ''' <remarks></remarks>
    Public Property Cards() As List(Of Card)
        Get
            Return m_Cards
        End Get
        Set(ByVal value As List(Of Card))
            m_Cards = value
        End Set
    End Property

    ''' <summary>
    ''' Gets whether the total is a Soft total
    ''' </summary>
    ''' <returns>Returns True if hand is a soft total, False if not</returns>
    ''' <remarks>A Soft hand has an Ace valued as 11. Example: Ace + 2 = 13</remarks>
    Public ReadOnly Property Soft() As Boolean
        Get

            ' Check if any cards are an Ace and valued at 11
            Return Cards.Any(Function(c) c.Rank = Card.RankType.Ace And c.Value = 11)

        End Get
    End Property

    ''' <summary>
    ''' Gets whether or not the current Hand is a BlackJack
    ''' </summary>
    ''' <returns>Returns True if Blackjack, False if not</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsBlackJack() As Boolean
        Get
            ' Whether or not the hand is a BlackJack
            ' Must be two cards
            ' Ace + Face/10
            If Cards.Count = 2 And Total() = 21 Then
                Return True
            End If

            Return False
        End Get
    End Property

    Public ReadOnly Property IsPair() As Boolean
        Get
            If Cards.Count = 2 Then
                If Cards(0).Rank = Cards(1).Rank Then
                    Return True
                Else
                    Return False
                End If
            End If

            Return False
        End Get
    End Property

    Public ReadOnly Property UISurface() As CardStacker
        Get
            Return m_Surface
        End Get
    End Property

    Private m_Owner As Character
    Public ReadOnly Property BelongsToHuman() As Boolean
        Get
            If TypeOf m_Owner Is Player Then
                If CType(m_Owner, Player).IsHuman Then
                    Return True
                End If
            End If
            Return False
        End Get
    End Property
    Public ReadOnly Property BelongsToComputer() As Boolean
        Get
            If TypeOf m_Owner Is Player Then
                If Not CType(m_Owner, Player).IsHuman Then
                    Return True
                End If
            End If

            Return False
        End Get
    End Property
    Public ReadOnly Property BelongsToDealer() As Boolean
        Get
            If TypeOf m_Owner Is Dealer Then
                Return True
            End If

            Return False
        End Get
    End Property

    ''' <summary>
    ''' Gets advice on what action to perform based on the
    ''' dealer's Up Card and the strategy the player is
    ''' using
    ''' </summary>
    ''' <param name="dealerCard"></param>
    ''' <param name="playerStrategy"></param>
    ''' <param name="allowSplit"></param>
    ''' <param name="cardCount"></param>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property GetAdvice(ByVal dealerCard As Card, ByVal playerStrategy As Strategy, ByVal allowSplit As Boolean, ByVal cardCount As Double) As Strategy.AdviceType
        Get
            If Not playerStrategy Is Nothing Then
                Return playerStrategy.GetAdvice(Me, dealerCard, allowSplit, cardCount)
            Else
                Return Strategy.AdviceType.None
            End If
        End Get
    End Property

#End Region

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="refDeck">The current deck in play</param>
    ''' <remarks></remarks>
    Public Sub New(ByRef owner As Character, ByRef refDeck As Shoe, ByRef surface As CardStacker)
        m_Deck = refDeck
        m_Surface = surface
        m_Owner = owner

        ' Initialize the list of cards
        Cards = New List(Of Card)
    End Sub

    ''' <summary>
    ''' Adds the next card from the deck to the current hand
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Add()
        Dim newCard As Card = m_Deck.NextCard
        Cards.Add(newCard)

        newCard.Draw(Me, UISurface)
    End Sub

    ''' <summary>
    ''' Re-calculates the total value of the hand
    ''' </summary>
    ''' <remarks></remarks>
    Public Function Total() As Integer
        Dim intTotal As Integer = 0

        ' Return 0 if we don't have cards
        If Not Cards.Count > 0 Then
            Return intTotal
        End If

        ' Calculate total for non-Ace cards
        ' PS. I <3 LINQ
        intTotal = Cards.Where(Function(c) c.Rank <> Card.RankType.Ace).Sum(Function(c) c.Value)

        ' Do we have any Aces? If so, find their total.
        If Cards.Any(Function(c) c.Rank = Card.RankType.Ace) Then

            ' Filter so we only get Aces
            For Each Ace In Cards.Where(Function(c) c.Rank = Card.RankType.Ace)

                ' Ace
                ' If total + 11 > 21 then it's a bust
                If (intTotal + 11) > 21 Then
                    ' Revert to a value of 1
                    Ace.Value = 1
                    intTotal += 1
                Else
                    Ace.Value = 11
                    intTotal += 11
                End If
            Next
        End If

        ' Set new total
        Return intTotal

    End Function
End Class
