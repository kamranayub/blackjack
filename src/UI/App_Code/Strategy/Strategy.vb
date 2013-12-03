''' <summary>
''' Summary description for Strategy.
''' </summary>
<Serializable()> _
Public MustInherit Class Strategy


    Public Const S As Integer = 100     ' Stand
    Public Const H As Integer = 200     ' Hit
    Public Const D As Integer = 300     ' Double Down
    Public Const P As Integer = 400     ' Split
    Public Const N As Integer = -100    ' None
    Public Const L As Integer = -200    ' Null

    Public Enum AdviceType
        None = N
        Hit = H
        Stand = S
        DoubleDown = D
        Split = P
        Null = L
    End Enum

    <NonSerialized()> _
    Protected Pairs As Integer(,)
    <NonSerialized()> _
    Protected Aces As Integer(,)
    <NonSerialized()> _
    Protected DoubleH As Integer(,)
    <NonSerialized()> _
    Protected DoubleS As Integer(,)
    <NonSerialized()> _
    Protected Hand As Integer(,)

    Protected Sub New()
    End Sub
    Public MustOverride Function GetAdvice(ByVal h As Hand, ByVal c As Card, ByVal b As Boolean, ByVal cc As Double) As AdviceType
    Public MustOverride Function GetInsuranceAdvice(ByVal count As Double, ByVal cardCount As Double, ByVal decks As Integer) As Boolean
    Public Overridable ReadOnly Property StrategyName() As String
        Get
            Return ""
        End Get
    End Property

    Public Shared Function GetStrategies() As ArrayList
        Dim strategies As New ArrayList()

        strategies.Add("Basic Single Deck")
        strategies.Add("Basic Multi Deck")
        strategies.Add("Aggressive Multi Deck")
        strategies.Add("Smart Multi Deck")
        strategies.Add("High Low Multi Deck")

        Return strategies
    End Function
    Public Shared Function NewStrategy(ByVal strategyName As String) As Strategy
        Dim returnValue As Strategy = Nothing

        Select Case strategyName.ToLower()
            Case "basic single deck"
                returnValue = New BasicSingleDeck()
                Exit Select
            Case "basic multi deck"
                'returnValue = New BasicMultiDeck()
                Exit Select
            Case "aggressive multi deck"
                'returnValue = New AggresiveMultiDeck()
                Exit Select
            Case "smart multi deck"
                'returnValue = New SmartMultiDeck()
                Exit Select
            Case "high low multi deck"
                'returnValue = New HighLowMultiDeck()
                Exit Select
            Case Else
                returnValue = Nothing
                Exit Select
        End Select
        Return returnValue
    End Function
End Class

