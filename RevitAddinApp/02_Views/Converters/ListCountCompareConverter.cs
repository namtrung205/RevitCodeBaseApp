using System ;
using System.Collections ;
using System.Globalization ;
using System.Windows.Data ;
using Binding = System.Windows.Data.Binding ;

namespace RevitAddinApp._02_Views.Converters ;

public class ListCountCompareConverter : IValueConverter
{
  public object Convert( object? value, Type targetType, object? parameter, CultureInfo culture )
  {
    if ( value is IList list ) {
      if ( parameter is double valueCompare ) return Math.Abs( list.Count - valueCompare ) < double.MinValue ;
      if ( parameter is int valueIntCompare ) return list.Count == valueIntCompare ;
    }

    if ( value is double doubleValue ) {
      if ( parameter is double valueCompare ) return Math.Abs( doubleValue - valueCompare ) < double.MinValue ;
      if ( parameter is int valueIntCompare ) return Math.Abs( doubleValue - valueIntCompare ) < double.MinValue ;
    }

    if ( value is int intValue ) {
      if ( parameter is double valueCompare ) return Math.Abs( intValue - valueCompare ) < double.MinValue ;
      if ( parameter is int valueIntCompare ) return intValue == valueIntCompare ;
    }

    return false ;
  }

  public object ConvertBack( object? value, Type targetType, object? parameter, CultureInfo culture )
  {
    return Binding.DoNothing ;
  }
}