''' <summary>
''' Keeps a list of rules that we are using throughout the game.
''' </summary>
''' <remarks>
''' The rules that affect strategy [most] are:
''' <list>
''' <item>Number of Decks</item>
''' <item>Dealer Hits/Stands on Soft 17</item>
''' <item>Double Down After Split</item>
''' </list>
'''</remarks>
Public Class Rules

    Private _mNumDecks As Integer
    Private _mDealer As DealerRuleType
    Private _mDDRestrict As DoubleDownType
    Private _mDAS As Boolean
    Private _mSurrender As SurrenderType

    Public Property DealerRules() As DealerRuleType
        Get
            Return _mDealer
        End Get
        Set(ByVal value As DealerRuleType)
            _mDealer = value
        End Set
    End Property
    Public Property NumberOfDecks() As Integer
        Get
            Return _mNumDecks
        End Get
        Set(ByVal value As Integer)
            If value = 1 Or value = 2 Or value = 4 Or value = 6 Then
                _mNumDecks = value
            Else
                ' Default is 4 decks
                _mNumDecks = 4
            End If
        End Set
    End Property
    Public Property DoubleDownRestrictions() As DoubleDownType
        Get
            Return _mDDRestrict
        End Get
        Set(ByVal value As DoubleDownType)
            _mDDRestrict = value
        End Set
    End Property
    Public Property DoubleAfterSplit() As Boolean
        Get
            Return _mDAS
        End Get
        Set(ByVal value As Boolean)
            _mDAS = value
        End Set
    End Property
    Public Property Surrender() As SurrenderType
        Get
            Return _mSurrender
        End Get
        Set(ByVal value As SurrenderType)
            _mSurrender = value
        End Set
    End Property

    ''' <summary>
    ''' The types of actions the Dealer should take
    ''' on a Soft 17
    ''' </summary>
    ''' <remarks></remarks>
    Public Enum DealerRuleType As Integer
        H17 = 0
        S17 = 1
    End Enum

    Public Enum DoubleDownType As Integer
        OnAny = 0
        OnNineTenEleven = 1
        OnTenEleven = 2
    End Enum

    Public Enum SurrenderType As Integer
        None = 0
        Late = 1
        Early = 2
    End Enum

    Public Sub New()

        ' Initialize defaults
        NumberOfDecks = 4
        DealerRules = DealerRuleType.H17
        DoubleAfterSplit = True
        Surrender = SurrenderType.None
        DoubleDownRestrictions = DoubleDownType.OnAny

    End Sub

End Class
