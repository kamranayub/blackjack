''' <summary>
''' Summary description for CountMethod.
''' 
''' TODO: Clean this up!
''' </summary>
<Serializable()> _
Public MustInherit Class CountMethod
    Protected Sub New(ByVal numDecks As Integer)
        _mNumberOfDecks = numDecks
    End Sub

    Protected _mNumberOfDecks As Integer
    Protected _mRunningTotal As Double
    Protected _mCardCount As Integer
    Protected acesCount As Integer
    Protected twosCount As Integer
    Protected threesCount As Integer
    Protected foursCount As Integer
    Protected fivesCount As Integer
    Protected sixesCount As Integer
    Protected sevensCount As Integer
    Protected eightsCount As Integer
    Protected ninesCount As Integer
    Protected tensCount As Integer

    Protected Overridable Function GetCounts() As Double()
        Return Nothing
    End Function
    Protected Function SideCount(ByVal cardType As Card.RankType) As Integer
        Dim count As Integer = 0

        Select Case cardType
            Case Card.RankType.Ace
                count = acesCount
                Exit Select
            Case Card.RankType.Two
                count = twosCount
                Exit Select
            Case Card.RankType.Three
                count = threesCount
                Exit Select
            Case Card.RankType.Four
                count = foursCount
                Exit Select
            Case Card.RankType.Five
                count = fivesCount
                Exit Select
            Case Card.RankType.Six
                count = sixesCount
                Exit Select
            Case Card.RankType.Seven
                count = sevensCount
                Exit Select
            Case Card.RankType.Eight
                count = eightsCount
                Exit Select
            Case Card.RankType.Nine
                count = ninesCount
                Exit Select
            Case Card.RankType.Ten
                count = tensCount
                Exit Select
        End Select

        Return count
    End Function
    Public Overridable Function Insurance10Count() As Double
        Dim count As Double = acesCount + twosCount + threesCount + foursCount + fivesCount + sixesCount + sevensCount + eightsCount + ninesCount + (tensCount * -2)
        Return count
    End Function

    Public Overridable Function GetWager(ByVal normalBet As Double) As Double
        Dim wager As Double = 0
        Dim trueCount As Double = Count

        If trueCount > 0 Then
            wager = normalBet * trueCount
        ElseIf trueCount = 0 Then
            wager = normalBet
        ElseIf trueCount < 0 Then
            wager = normalBet * trueCount
        End If

        ' $10 table minimum :)  Also, round to nearest integer
        wager = CInt(Math.Max(10, wager))

        Return wager
    End Function

    Public Overridable ReadOnly Property MethodName() As String
        Get
            Return ""
        End Get
    End Property
    Public Overridable ReadOnly Property MethodLevel() As Integer
        Get
            Return 0
        End Get
    End Property
    Public Overridable Sub CountCard(ByVal aCard As Card)
        _mRunningTotal += GetCounts()(CInt(aCard.TrueValue))
        _mCardCount += 1

        ' Now keep side counts for all cards
        Select Case aCard.Rank
            Case Card.RankType.Ace
                acesCount += 1
                Exit Select
            Case Card.RankType.Two
                twosCount += 1
                Exit Select
            Case Card.RankType.Three
                threesCount += 1
                Exit Select
            Case Card.RankType.Four
                foursCount += 1
                Exit Select
            Case Card.RankType.Five
                fivesCount += 1
                Exit Select
            Case Card.RankType.Six
                sixesCount += 1
                Exit Select
            Case Card.RankType.Seven
                sevensCount += 1
                Exit Select
            Case Card.RankType.Eight
                eightsCount += 1
                Exit Select
            Case Card.RankType.Nine
                ninesCount += 1
                Exit Select
            Case Card.RankType.Ten
                tensCount += 1
                Exit Select
            Case Card.RankType.Jack
                tensCount += 1
                Exit Select
            Case Card.RankType.Queen
                tensCount += 1
                Exit Select
            Case Card.RankType.King
                tensCount += 1
                Exit Select
        End Select
    End Sub

    Public Overridable Sub Reset(ByVal numDecks As Integer)
        _mCardCount = 0
        _mRunningTotal = 0
        acesCount = 0
        twosCount = 0
        threesCount = 0
        foursCount = 0
        fivesCount = 0
        sixesCount = 0
        sevensCount = 0
        eightsCount = 0
        ninesCount = 0
        tensCount = 0
        _mNumberOfDecks = numDecks
    End Sub

    Public Overridable ReadOnly Property Count() As Double
        Get
            Return _mRunningTotal / ((_mNumberOfDecks * 52 - _mCardCount) / 52)
        End Get
    End Property

    Public Shared Function GetMethods() As ArrayList
        Dim methods As New ArrayList()

        methods.Add("Hi-Lo")
        methods.Add("High-Low")
        methods.Add("Hi Opt I")
        methods.Add("Hi Opt II")
        methods.Add("Silver Fox")
        methods.Add("Brh I")
        methods.Add("Brh II")
        methods.Add("Canfield Expert")
        methods.Add("Canfield Master")
        methods.Add("KO")
        methods.Add("Omega II")
        methods.Add("Red Seven")
        methods.Add("Revere Adv. Plus Minus")
        methods.Add("Revere Point Count")
        methods.Add("Unb. Zen 11")
        methods.Add("Uston Adv. Plus Minus")
        methods.Add("Uston APC")
        methods.Add("Uston SS")
        methods.Add("Wong Halves")
        methods.Add("Zen Count")
        methods.Add("Hi-Lo Ace Side Count")
        methods.Add("HiOptI Ace-Seven Side Count")

        Return methods
    End Function
    Public Shared Function NewMethod(ByVal methodName As String, ByVal n As Integer) As CountMethod
        Dim returnValue As CountMethod = Nothing

        Select Case methodName.ToLower()
            Case ("hi-lo")
                returnValue = New HiLo(n)
                Exit Select
            Case ("high-low")
                returnValue = New HighLow(n)
                Exit Select
            Case ("hi opt i")
                returnValue = New HiOptI(n)
                Exit Select
            Case ("hi opt ii")
                returnValue = New HiOptII(n)
                Exit Select
            Case ("silver fox")
                returnValue = New SilverFox(n)
                Exit Select
            Case ("brh i")
                returnValue = New BrhI(n)
                Exit Select
            Case ("brh ii")
                returnValue = New BrhII(n)
                Exit Select
            Case ("canfield expert")
                returnValue = New CanfieldExpert(n)
                Exit Select
            Case ("canfield master")
                returnValue = New CanfieldMaster(n)
                Exit Select
            Case ("ko")
                returnValue = New KO(n)
                Exit Select
            Case ("omega ii")
                returnValue = New OmegaII(n)
                Exit Select
            Case ("red seven")
                returnValue = New RedSeven(n)
                Exit Select
            Case ("revere adv. plus minus")
                returnValue = New RevereAdvPlusMinus(n)
                Exit Select
            Case ("revere point count")
                returnValue = New ReverePointCount(n)
                Exit Select
            Case ("unb. zen 11")
                returnValue = New UnbZen11(n)
                Exit Select
            Case ("uston adv. plus minus")
                returnValue = New UstonAdvPlusMinus(n)
                Exit Select
            Case ("uston apc")
                returnValue = New UstonApc(n)
                Exit Select
            Case ("uston ss")
                returnValue = New UstonSS(n)
                Exit Select
            Case ("wong halves")
                returnValue = New WongHalves(n)
                Exit Select
            Case ("zen count")
                returnValue = New ZenCount(n)
                Exit Select
            Case ("hi-lo ace side count")
                returnValue = New HiLoA(n)
                Exit Select
            Case ("hiopti ace-seven side count")
                returnValue = New HiOptIA7(n)
                Exit Select
            Case Else
                returnValue = Nothing
                Exit Select
        End Select

        Return returnValue
    End Function
End Class

<Serializable()> _
Public Class HiLo
    Inherits CountMethod
    Public Sub New(ByVal n As Integer)
        MyBase.New(n)
    End Sub

    Protected Overloads Overrides Function GetCounts() As Double()
        Return counts
    End Function
    ' Card counts                             A  2  3  4  5  6  7  8  9  T
    Protected counts As Double() = New Double() {0, 0, 1, 1, 1, 1, _
     0, 0, 0, -1}
    Private name As String = "Hi-Lo"
    Private level As Integer = 1
    Public Overloads Overrides ReadOnly Property MethodName() As String
        Get
            Return name
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property MethodLevel() As Integer
        Get
            Return level
        End Get
    End Property
End Class

<Serializable()> _
Public Class HighLow
    Inherits CountMethod
    Public Sub New(ByVal n As Integer)
        MyBase.New(n)
    End Sub

    Protected Overloads Overrides Function GetCounts() As Double()
        Return counts
    End Function
    ' Card counts                             A  2  3  4  5  6  7  8  9  T
    Protected counts As Double() = New Double() {-1, 1, 1, 1, 1, 1, _
     0, 0, 0, -1}
    Private name As String = "High-Low"
    Private level As Integer = 1
    Public Overloads Overrides ReadOnly Property MethodName() As String
        Get
            Return name
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property MethodLevel() As Integer
        Get
            Return level
        End Get
    End Property
End Class

<Serializable()> _
Public Class HiOptI
    Inherits CountMethod
    Public Sub New(ByVal n As Integer)
        MyBase.New(n)
    End Sub

    Protected Overloads Overrides Function GetCounts() As Double()
        Return counts
    End Function
    ' Card counts                             A  2  3  4  5  6  7  8  9  T
    Protected counts As Double() = New Double() {0, 0, 1, 1, 1, 1, _
     0, 0, 0, -1}
    Private name As String = "HiOptI"
    Private level As Integer = 1
    Public Overloads Overrides ReadOnly Property MethodName() As String
        Get
            Return name
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property MethodLevel() As Integer
        Get
            Return level
        End Get
    End Property
End Class

<Serializable()> _
Public Class HiOptII
    Inherits CountMethod
    Public Sub New(ByVal n As Integer)
        MyBase.New(n)
    End Sub

    Protected Overloads Overrides Function GetCounts() As Double()
        Return counts
    End Function
    ' Card counts                             A  2  3  4  5  6  7  8  9  T
    Protected counts As Double() = New Double() {0, 1, 1, 2, 2, 1, _
     1, 0, 0, -2}
    Private name As String = "HiOptII"
    Private level As Integer = 2
    Public Overloads Overrides ReadOnly Property MethodName() As String
        Get
            Return name
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property MethodLevel() As Integer
        Get
            Return level
        End Get
    End Property
End Class

<Serializable()> _
Public Class SilverFox
    Inherits CountMethod
    Public Sub New(ByVal n As Integer)
        MyBase.New(n)
    End Sub

    Protected Overloads Overrides Function GetCounts() As Double()
        Return counts
    End Function
    ' Card counts                             A  2  3  4  5  6  7  8  9  T
    Protected counts As Double() = New Double() {-1, 1, 1, 1, 1, 1, _
     1, 0, -1, -1}
    Private name As String = "Silver Fox"
    Private level As Integer = 1
    Public Overloads Overrides ReadOnly Property MethodName() As String
        Get
            Return name
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property MethodLevel() As Integer
        Get
            Return level
        End Get
    End Property
End Class

<Serializable()> _
Public Class BrhI
    Inherits CountMethod
    Public Sub New(ByVal n As Integer)
        MyBase.New(n)
    End Sub

    Protected Overloads Overrides Function GetCounts() As Double()
        Return counts
    End Function
    ' Card counts                             A  2  3  4  5  6  7  8  9  T
    Protected counts As Double() = New Double() {-2, 1, 2, 2, 3, 2, _
     1, 0, 0, -2}
    Private name As String = "BrhI"
    Private level As Integer = 3
    Public Overloads Overrides ReadOnly Property MethodName() As String
        Get
            Return name
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property MethodLevel() As Integer
        Get
            Return level
        End Get
    End Property
End Class

<Serializable()> _
Public Class BrhII
    Inherits CountMethod
    Public Sub New(ByVal n As Integer)
        MyBase.New(n)
    End Sub

    Protected Overloads Overrides Function GetCounts() As Double()
        Return counts
    End Function
    ' Card counts                             A  2  3  4  5  6  7  8  9  T
    Protected counts As Double() = New Double() {-2, 1, 1, 2, 2, 2, _
     1, 0, 0, -2}
    Private name As String = "BrhII"
    Private level As Integer = 2
    Public Overloads Overrides ReadOnly Property MethodName() As String
        Get
            Return name
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property MethodLevel() As Integer
        Get
            Return level
        End Get
    End Property
End Class

<Serializable()> _
Public Class CanfieldExpert
    Inherits CountMethod
    Public Sub New(ByVal n As Integer)
        MyBase.New(n)
    End Sub

    Protected Overloads Overrides Function GetCounts() As Double()
        Return counts
    End Function
    ' Card counts                             A  2  3  4  5  6  7  8  9  T
    Protected counts As Double() = New Double() {0, 0, 1, 1, 1, 1, _
     1, 0, -1, -1}
    Private name As String = "Canfield Expert"
    Private level As Integer = 1
    Public Overloads Overrides ReadOnly Property MethodName() As String
        Get
            Return name
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property MethodLevel() As Integer
        Get
            Return level
        End Get
    End Property
End Class

<Serializable()> _
Public Class CanfieldMaster
    Inherits CountMethod
    Public Sub New(ByVal n As Integer)
        MyBase.New(n)
    End Sub

    Protected Overloads Overrides Function GetCounts() As Double()
        Return counts
    End Function
    ' Card counts                             A  2  3  4  5  6  7  8  9  T
    Protected counts As Double() = New Double() {0, 1, 1, 2, 2, 2, _
     1, 0, -1, -2}
    Private name As String = "Canfield Master"
    Private level As Integer = 2
    Public Overloads Overrides ReadOnly Property MethodName() As String
        Get
            Return name
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property MethodLevel() As Integer
        Get
            Return level
        End Get
    End Property
End Class

<Serializable()> _
Public Class KO
    Inherits CountMethod
    Public Sub New(ByVal n As Integer)
        MyBase.New(n)
    End Sub

    Protected Overloads Overrides Function GetCounts() As Double()
        Return counts
    End Function
    ' Card counts                             A  2  3  4  5  6  7  8  9  T
    Protected counts As Double() = New Double() {-1, 1, 1, 1, 1, 1, _
     1, 0, 0, -1}
    Private name As String = "KO"
    Private level As Integer = 1
    Public Overloads Overrides ReadOnly Property MethodName() As String
        Get
            Return name
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property MethodLevel() As Integer
        Get
            Return level
        End Get
    End Property
End Class

<Serializable()> _
Public Class OmegaII
    Inherits CountMethod
    Public Sub New(ByVal n As Integer)
        MyBase.New(n)
    End Sub

    Protected Overloads Overrides Function GetCounts() As Double()
        Return counts
    End Function
    ' Card counts                             A  2  3  4  5  6  7  8  9  T
    Protected counts As Double() = New Double() {0, 0, 1, 1, 1, 1, _
     1, 0, -1, -1}
    Private name As String = "Omega II"
    Private level As Integer = 1
    Public Overloads Overrides ReadOnly Property MethodName() As String
        Get
            Return name
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property MethodLevel() As Integer
        Get
            Return level
        End Get
    End Property
End Class

<Serializable()> _
Public Class RedSeven
    Inherits CountMethod
    Public Sub New(ByVal n As Integer)
        MyBase.New(n)
    End Sub

    Protected Overloads Overrides Function GetCounts() As Double()
        Return counts
    End Function
    ' Card counts                             A  2  3  4  5  6  7  8  9  T
    Protected counts As Double() = New Double() {-1, 1, 1, 1, 1, 1, _
     0.5, 0, 0, -1}
    Private name As String = "Red Seven"
    Private level As Integer = 1
    Public Overloads Overrides ReadOnly Property MethodName() As String
        Get
            Return name
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property MethodLevel() As Integer
        Get
            Return level
        End Get
    End Property
End Class

<Serializable()> _
Public Class RevereAdvPlusMinus
    Inherits CountMethod
    Public Sub New(ByVal n As Integer)
        MyBase.New(n)
    End Sub

    Protected Overloads Overrides Function GetCounts() As Double()
        Return counts
    End Function
    ' Card counts                             A  2  3  4  5  6  7  8  9  T
    Protected counts As Double() = New Double() {0, 1, 1, 1, 1, 1, _
     0, 0, -1, -1}
    Private name As String = "Revere Adv. Plus Minus"
    Private level As Integer = 1
    Public Overloads Overrides ReadOnly Property MethodName() As String
        Get
            Return name
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property MethodLevel() As Integer
        Get
            Return level
        End Get
    End Property
End Class

<Serializable()> _
Public Class ReverePointCount
    Inherits CountMethod
    Public Sub New(ByVal n As Integer)
        MyBase.New(n)
    End Sub

    Protected Overloads Overrides Function GetCounts() As Double()
        Return counts
    End Function
    ' Card counts                             A  2  3  4  5  6  7  8  9  T
    Protected counts As Double() = New Double() {-2, 1, 2, 2, 2, 2, _
     1, 0, 0, -2}
    Private name As String = "Revere Point Count"
    Private level As Integer = 2
    Public Overloads Overrides ReadOnly Property MethodName() As String
        Get
            Return name
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property MethodLevel() As Integer
        Get
            Return level
        End Get
    End Property
End Class

<Serializable()> _
Public Class UnbZen11
    Inherits CountMethod
    Public Sub New(ByVal n As Integer)
        MyBase.New(n)
    End Sub

    Protected Overloads Overrides Function GetCounts() As Double()
        Return counts
    End Function
    ' Card counts                             A  2  3  4  5  6  7  8  9  T
    Protected counts As Double() = New Double() {-1, 1, 2, 2, 2, 2, _
     1, 0, 0, -2}
    Private name As String = "Unb. Zen 11"
    Private level As Integer = 2
    Public Overloads Overrides ReadOnly Property MethodName() As String
        Get
            Return name
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property MethodLevel() As Integer
        Get
            Return level
        End Get
    End Property
End Class

<Serializable()> _
Public Class UstonAdvPlusMinus
    Inherits CountMethod
    Public Sub New(ByVal n As Integer)
        MyBase.New(n)
    End Sub

    Protected Overloads Overrides Function GetCounts() As Double()
        Return counts
    End Function
    ' Card counts                             A  2  3  4  5  6  7  8  9  T
    Protected counts As Double() = New Double() {-1, 0, 1, 1, 1, 1, _
     1, 0, 0, -1}
    Private name As String = "Uston Adv. Plus Minus"
    Private level As Integer = 1
    Public Overloads Overrides ReadOnly Property MethodName() As String
        Get
            Return name
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property MethodLevel() As Integer
        Get
            Return level
        End Get
    End Property
End Class

<Serializable()> _
Public Class UstonApc
    Inherits CountMethod
    Public Sub New(ByVal n As Integer)
        MyBase.New(n)
    End Sub

    Protected Overloads Overrides Function GetCounts() As Double()
        Return counts
    End Function
    ' Card counts                             A  2  3  4  5  6  7  8  9  T
    Protected counts As Double() = New Double() {0, 1, 2, 2, 3, 2, _
     2, 1, -1, -3}
    Private name As String = "Uston APC"
    Private level As Integer = 3
    Public Overloads Overrides ReadOnly Property MethodName() As String
        Get
            Return name
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property MethodLevel() As Integer
        Get
            Return level
        End Get
    End Property
End Class

<Serializable()> _
Public Class UstonSS
    Inherits CountMethod
    Public Sub New(ByVal n As Integer)
        MyBase.New(n)
    End Sub

    Protected Overloads Overrides Function GetCounts() As Double()
        Return counts
    End Function
    ' Card counts                             A  2  3  4  5  6  7  8  9  T
    Protected counts As Double() = New Double() {-2, 2, 2, 2, 3, 2, _
     1, 0, -1, -2}
    Private name As String = "Uston SS"
    Private level As Integer = 2
    Public Overloads Overrides ReadOnly Property MethodName() As String
        Get
            Return name
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property MethodLevel() As Integer
        Get
            Return level
        End Get
    End Property
End Class

<Serializable()> _
Public Class WongHalves
    Inherits CountMethod
    Public Sub New(ByVal n As Integer)
        MyBase.New(n)
    End Sub

    Protected Overloads Overrides Function GetCounts() As Double()
        Return counts
    End Function
    ' Card counts                             A  2  3  4  5  6  7  8  9  T
    Protected counts As Double() = New Double() {-1, 0.5, 1, 1, 1.5, 1, _
     0.5, 0, -0.5, -1}
    Private name As String = "Wong Halves"
    Private level As Integer = 1
    Public Overloads Overrides ReadOnly Property MethodName() As String
        Get
            Return name
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property MethodLevel() As Integer
        Get
            Return level
        End Get
    End Property
End Class

<Serializable()> _
Public Class ZenCount
    Inherits CountMethod
    Public Sub New(ByVal n As Integer)
        MyBase.New(n)
    End Sub

    Protected Overloads Overrides Function GetCounts() As Double()
        Return counts
    End Function
    ' Card counts                             A  2  3  4  5  6  7  8  9  T
    Protected counts As Double() = New Double() {-1, 1, 1, 2, 2, 2, _
     1, 0, 0, -2}
    Private name As String = "Zen Count"
    Private level As Integer = 2
    Public Overloads Overrides ReadOnly Property MethodName() As String
        Get
            Return name
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property MethodLevel() As Integer
        Get
            Return level
        End Get
    End Property
End Class

<Serializable()> _
Public Class HiLoA
    Inherits CountMethod
    ' The standard HiLo method with an Ace side count
    Public Sub New(ByVal n As Integer)
        MyBase.New(n)
    End Sub

    Protected Overloads Overrides Function GetCounts() As Double()
        Return counts
    End Function
    ' Card counts                             A  2  3  4  5  6  7  8  9  T
    Protected counts As Double() = New Double() {0, 0, 1, 1, 1, 1, _
     0, 0, 0, -1}
    Public Overloads Overrides Function GetWager(ByVal normalBet As Double) As Double
        Dim wager As Double = 0
        Dim trueCount As Double = Count
        Dim aceCount As Integer = MyBase.SideCount(Card.RankType.Ace)

        Dim aceRatio As Double = ((_mCardCount / 13.0R) - aceCount) / ((_mNumberOfDecks * 52 - _mCardCount) / 52.0R)
        trueCount += aceRatio

        If trueCount > 0 Then
            wager = normalBet * trueCount
        ElseIf trueCount = 0 Then
            wager = normalBet
        ElseIf trueCount < 0 Then
            wager = normalBet * trueCount
        End If

        ' $10 table minimum :)  Also, round to nearest integer
        wager = CInt(Math.Max(10, wager))

        Return wager
    End Function

    Private name As String = "Hi-Lo Ace Side Count"
    Private level As Integer = 1
    Public Overloads Overrides ReadOnly Property MethodName() As String
        Get
            Return name
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property MethodLevel() As Integer
        Get
            Return level
        End Get
    End Property
End Class

<Serializable()> _
Public Class HiOptIA7
    Inherits CountMethod
    Public Sub New(ByVal n As Integer)
        MyBase.New(n)
    End Sub

    Protected Overloads Overrides Function GetCounts() As Double()
        Return counts
    End Function
    ' Card counts                             A  2  3  4  5  6  7  8  9  T
    Protected counts As Double() = New Double() {0, 0, 1, 1, 1, 1, _
     0, 0, 0, -1}
    Public Overloads Overrides Function GetWager(ByVal normalBet As Double) As Double
        Dim wager As Double = 0
        Dim trueCount As Double = Count
        Dim aceCount As Integer = MyBase.SideCount(Card.RankType.Ace)
        Dim sevenCount As Integer = MyBase.SideCount(Card.RankType.Seven)

        Dim aceRatio As Double = ((_mCardCount / 13.0R) - aceCount) / ((_mNumberOfDecks * 52 - _mCardCount) / 52.0R)
        Dim sevenRatio As Double = ((_mCardCount / 13.0R) - sevenCount) / ((_mNumberOfDecks * 52 - _mCardCount) / 52.0R)
        trueCount += aceRatio
        trueCount += sevenRatio

        If trueCount > 0 Then
            wager = normalBet * trueCount
        ElseIf trueCount = 0 Then
            wager = normalBet
        ElseIf trueCount < 0 Then
            wager = normalBet * trueCount
        End If

        ' $10 table minimum :)  Also, round to nearest integer
        wager = CInt(Math.Max(10, wager))

        Return wager
    End Function

    Private name As String = "Hi Opt I Ace Seven side counts"
    Private level As Integer = 1
    Public Overloads Overrides ReadOnly Property MethodName() As String
        Get
            Return name
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property MethodLevel() As Integer
        Get
            Return level
        End Get
    End Property
End Class