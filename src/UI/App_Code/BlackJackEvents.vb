''' <summary>
''' Holds delegate events
''' and event arguments.
''' Easier to manage events in a central repository.
''' This also makes it easier to convert the game to C#
''' </summary>
''' <remarks></remarks>
Public Module BlackJackEvents

#Region " Custom Event Handler Delegates "

    ' BlackJack Game
    Public Delegate Sub RoundStartEventHandler(ByVal sender As Object, ByVal e As EventArgs)
    Public Delegate Sub RoundEndEventHandler(ByVal sender As Object, ByVal e As EventArgs)
    Public Delegate Sub PlayerTurnStartEventHandler(ByVal sender As Object, ByVal e As PlayerTurnEventArgs)
    Public Delegate Sub PlayerTurnEndEventHandler(ByVal sender As Object, ByVal e As PlayerTurnEventArgs)

    ' Character
    Public Delegate Sub HitEventHandler(ByVal sender As Object, ByVal e As HitEventArgs)
    Public Delegate Sub StandEventHandler(ByVal sender As Object, ByVal e As EventArgs)
    Public Delegate Sub DoubleDownEventHandler()
    Public Delegate Sub SplitEventHandler()

    ' Shoe
    Public Delegate Sub DeckChangedEventHandler(ByVal sender As Object, ByVal e As DeckChangedEventArgs)

    ' Hand
    Public Delegate Sub CardAddedEventHandler()


#End Region

#Region " Custom Event Arguments "
    ''' <summary>
    ''' An event argument class that contains information about the Hit.
    ''' An instance of this is passed with every event.
    ''' </summary>
    ''' <remarks></remarks>
    <Serializable()> _
    Public Class HitEventArgs
        Inherits EventArgs

        Private _mCard As Card
        Private _mIsDeal As Boolean = False

        Public Sub New(ByVal newCard As Card, ByVal isDeal As Boolean)
            _mCard = newCard
            _mIsDeal = isDeal
        End Sub

        ''' <summary>
        ''' The new card we Hit
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Card() As Card
            Get
                Return _mCard
            End Get
        End Property

        Public ReadOnly Property IsDeal() As Boolean
            Get
                Return _mIsDeal
            End Get
        End Property
    End Class


    <Serializable()> _
    Class DeckChangedEventArgs

        Private _mChangeType As Shoe.ChangeType

        Public Sub New(ByVal type As Shoe.ChangeType)
            _mChangeType = type

        End Sub

        Public ReadOnly Property TypeOfChange() As Shoe.ChangeType
            Get
                Return _mChangeType
            End Get
        End Property

    End Class

    <Serializable()> _
    Class PlayerTurnEventArgs

        Private _mPlayer As Player

        Public Sub New(ByVal whosTurn As Player)
            _mPlayer = whosTurn
        End Sub

        Public ReadOnly Property Player() As Player
            Get
                Return _mPlayer
            End Get
        End Property
    End Class


#End Region

End Module