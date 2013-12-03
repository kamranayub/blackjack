''' <summary>
''' Represents a single playing Card
''' </summary>
''' <remarks></remarks>
Public Class Card

#Region " Vars "

    Private m_Rank As RankType
    Private m_Suit As SuitType
    Private m_bHole As Boolean
    Private m_Value As Integer
    Private m_Image As ImageBrush

    Private Const DRAW_OFFSET As Integer = 15    ' No. of pixels to offset each card
#End Region

#Region " Properties "

    ''' <summary>
    ''' The Rank of a card.
    ''' Ace, Two, King, etc.
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum RankType As Integer
        Ace = 0
        Two = 1
        Three = 2
        Four = 3
        Five = 4
        Six = 5
        Seven = 6
        Eight = 7
        Nine = 8
        Ten = 9
        Jack = 10
        Queen = 11
        King = 12
    End Enum

    ''' <summary>
    ''' The Suit of a card.
    ''' Spades, Hearts, etc.
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum SuitType As Integer
        Hearts = 0
        Diamonds = 1
        Clubs = 2
        Spades = 3
    End Enum

    ''' <summary>
    ''' Gets or sets the <see cref="RankType" /> of a card
    ''' </summary>
    ''' <value>Sets the <see cref="RankType" /></value>
    ''' <returns>Gets the <see cref="RankType" /></returns>
    ''' <remarks></remarks>
    Public Property Rank() As RankType
        Get
            Return m_Rank
        End Get
        Set(ByVal value As RankType)
            m_Rank = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the <see cref="SuitType" /> of a card
    ''' </summary>
    ''' <value>Sets the <see cref="SuitType" /></value>
    ''' <returns>Gets the <see cref="SuitType" /></returns>
    ''' <remarks></remarks>
    Public Property Suit() As SuitType
        Get
            Return m_Suit
        End Get
        Set(ByVal value As SuitType)
            m_Suit = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets whether or not a card is a Hole card
    ''' </summary>
    ''' <returns>Returns True if it is a hole, False if not</returns>
    ''' <remarks>A hole card is hidden from the player</remarks>
    Public Property IsHole() As Boolean
        Get
            Return m_bHole
        End Get
        Set(ByVal value As Boolean)
            m_bHole = value
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the numeric BlackJack value of the card.
    ''' </summary>
    ''' <value>Sets the value</value>
    ''' <returns>Gets the value</returns>
    ''' <remarks>Aces can be valued as either 1 or 11.</remarks>
    Public Property Value() As Integer
        Get
            Return m_Value
        End Get
        Set(ByVal value As Integer)
            m_Value = value
        End Set
    End Property

    ''' <summary>
    ''' Gets the "True" value of a card based on its
    ''' rank. Used for card counting and strategies.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property TrueValue() As Integer
        Get
            Dim rankValue As Integer = Rank

            ' If it's a Jack, Queen, or King
            ' it's really just a 10
            If rankValue > 9 Then
                Return 9
            End If

            Return rankValue
        End Get
    End Property

    ''' <summary>
    ''' Gets the card's friendly Rank name
    ''' </summary>
    ''' <returns>Ace, Two, Three, etc.</returns>
    ''' <remarks>Uses the Enum.GetName method</remarks>
    Public ReadOnly Property RankName() As String
        Get
            Return [Enum].GetName(GetType(RankType), Rank)
        End Get
    End Property

    ''' <summary>
    ''' Gets the card's friendly Suit name
    ''' </summary>
    ''' <returns>Hearts, Clubs, etc.</returns>
    ''' <remarks>Uses the Enum.GetName method</remarks>
    Public ReadOnly Property SuitName() As String
        Get
            Return [Enum].GetName(GetType(SuitType), Suit)
        End Get
    End Property


    ''' <summary>
    ''' Gets the image associated with the Card
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Image() As ImageBrush
        Get
            Return m_Image
        End Get
        Set(ByVal value As ImageBrush)
            m_Image = value
        End Set
    End Property

#End Region

    ''' <summary>
    ''' Constructor. Creates Card, assigns Rank and Suit.
    ''' </summary>
    ''' <param name="r">The Rank of the Card</param>
    ''' <param name="s">The Suit of the Card</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal r As RankType, ByVal s As SuitType)
        ' Set Rank and Suit
        Rank = r
        Suit = s
        Value = TrueValue + 1

        ' Image name
        ' _1Spades, etc.
        Dim imageId As String = CStr(CInt(r) + 1) & SuitName
        Image = GetImage(imageId)

        ' Null, couldn't find image
        If Image Is Nothing Then
            Image = GetImage("EndDeck")
        End If
    End Sub

    ''' <summary>
    ''' Returns a friendly identification of this card
    ''' </summary>
    ''' <returns>Returns a String value like "Ace of Spades"</returns>
    ''' <remarks></remarks>
    Public Function IdentifyCard() As String
        Dim cardRank As String = RankName

        If Rank > RankType.Ace And Rank <= RankType.Ten Then
            cardRank = CStr(Rank + 1)
        End If

        Return cardRank & " of " & SuitName
    End Function

    ''' <summary>
    ''' Draws a card onto a specified Drawing Surface
    ''' </summary>
    ''' <param name="drawingSurface">The surface on which to draw upon</param>
    ''' <remarks></remarks>
    Public Sub Draw(ByVal sender As Object, ByVal drawingSurface As CardStacker)
        Dim ImageResource As ImageBrush = Image
        Dim myHand As Hand = CType(sender, Hand)
        Dim numCards As Integer = drawingSurface.Children.Count

        ' Draw the card on the drawing surface
        Dim myCard As New PlayingCard()
        myCard.ActualCard = Me
        myCard.HorizontalAlignment = HorizontalAlignment.Left
        myCard.VerticalAlignment = VerticalAlignment.Top

        drawingSurface.Children.Add(myCard)

        ' Delay the animation, based on the number of new cards
        ' we are adding
        If numCards >= 2 Then
            ' If we are a human player, we don't need a delay
            If myHand.BelongsToHuman Or myHand.BelongsToComputer Then
                numCards = 0
            Else
                numCards -= 2
            End If

        End If

        myCard.PlayDrawCardAnim(numCards * 500)
    End Sub

    ''' <summary>
    ''' Gets an Image from the app's resources
    ''' </summary>
    ''' <param name="imageId">The image to retrieve</param>
    ''' <returns>Returns an Image object</returns>
    ''' <remarks></remarks>
    Private Function GetImage(ByVal imageId As String) As ImageBrush

        Try
            Return DirectCast(Application.Current.FindResource(imageId), ImageBrush)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
End Class




