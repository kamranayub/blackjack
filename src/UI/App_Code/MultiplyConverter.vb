Public Class MultiplyConverter
    Implements System.Windows.Data.IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Return AsDouble(value) * AsDouble(parameter)
    End Function
    Private Function AsDouble(ByVal value As Object) As Double
        Dim valueText = TryCast(value, String)
        If valueText IsNot Nothing Then
            Return Double.Parse(valueText)
        Else
            Return CDbl(value)
        End If
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As System.Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New System.NotSupportedException()
    End Function
End Class

