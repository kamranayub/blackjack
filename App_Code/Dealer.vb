Imports System.Linq.Enumerable

''' <summary>
''' Represents the Dealer, which inherits the <see cref="Hand" /> class. 
''' </summary>
''' <remarks></remarks>
Public Class Dealer
    Inherits Character

#Region " Properties "

    Private _mShowHole As Boolean

    ''' <summary>
    ''' Shortcut to get the Dealer's Hand
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private ReadOnly Property Hand() As Hand
        Get
            Return Hands(0)
        End Get
    End Property

    ''' <summary>
    ''' Shortcut to get the dealer's Hand
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides ReadOnly Property CurrentHand() As Hand
        Get
            Return Hand
        End Get
    End Property

    ''' <summary>
    ''' Gets the Dealer's total
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overrides ReadOnly Property Total() As Integer
        Get
            Return Hand.Total
        End Get
    End Property

    ''' <summary>
    ''' Returns the true Total if we can show the Hole card,
    ''' otherwise it returns Total - Hole value
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property FaceTotal() As Integer
        Get
            Dim myTotal As Integer = Total

            ' No total?
            If myTotal <= 0 Then
                Return 0
            End If

            ' If we have a hole card, subtract that value
            Dim holeValue As Integer

            For Each Card In Hand.Cards
                If Card.IsHole Then
                    holeValue = Card.Value
                End If
            Next

            Return myTotal - holeValue
        End Get
    End Property


    ''' <summary>
    ''' Whether or not to show the Hole card
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ShowHole() As Boolean
        Get
            Return _mShowHole
        End Get
        Set(ByVal value As Boolean)
            _mShowHole = value

            If Hand.Cards.Count > 1 Then
                Hand.Cards(0).IsHole = Not value
            End If

            If UIControl.DrawingSurface.Children.Count > 0 Then
                Dim firstCard As PlayingCard = CType(UIControl.DrawingSurface.Children(0), PlayingCard)
                firstCard.ShowActualCard(value)
            End If

            ' Update the total to
            ' show the face total
            RefreshTotal()
        End Set
    End Property

    Protected Overrides ReadOnly Property LastCard() As Card
        Get
            Return Last(Hand.Cards)
        End Get
    End Property

#End Region

    ''' <summary>
    ''' Constructor
    ''' </summary>
    ''' <param name="useDeck"></param>
    ''' <param name="myCtrl"></param>
    ''' <remarks></remarks>
    Public Sub New(ByRef useDeck As Shoe, ByRef myCtrl As UCCharacter)
        MyBase.New(useDeck, myCtrl)
    End Sub

    ''' <summary>
    ''' Special Hit routine for the Dealer
    ''' Takes into account "Hit on Soft 17", etc.
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub PerformHit()

        ' Dealer draws
        Do Until Hand.Total >= 17
            Hand.Add()
            OnHit(Me, New HitEventArgs(LastCard, False))
        Loop

        ' Game is over if dealer's total exceeds 17
        If Hand.Total > 17 Then
            PerformStand()
            Exit Sub
        End If

        ' Dealer must hit on Soft 17
        If Hand.Total = 17 And Hand.Soft Then
            ' Dealer can hit one or more times
            Hand.Add()
            OnHit(Me, New HitEventArgs(LastCard, False))

            Do Until Hand.Total >= 17
                Hand.Add()
                OnHit(Me, New HitEventArgs(LastCard, False))
            Loop
        End If

        ' Check if we have a game over, it's the final turn
        PerformStand()

    End Sub

    ''' <summary>
    ''' Gets the Dealer's outcome relative
    ''' to himself.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Protected Overrides Function GetOutcome() As BlackJack.OutcomeType
        If IsBlackJack Then
            Return BlackJack.OutcomeType.BlackJack
        End If

        If Total > 21 Then
            Return BlackJack.OutcomeType.Bust
        End If

        Return BlackJack.OutcomeType.None
    End Function

    ''' <summary>
    ''' Refreshes the UI total
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub RefreshTotal()
        UIControl.SetTotal(FaceTotal)
    End Sub
End Class
