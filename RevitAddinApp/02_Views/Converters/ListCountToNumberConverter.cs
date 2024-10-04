using System ;
using System.Collections ;
using System.Globalization ;
using System.Windows.Data ;

namespace RevitAddinApp._02_Views.Converters ;

public class ListCountToNumberConverter : IValueConverter
{
  public object Convert( object? value, Type targetType, object? parameter, CultureInfo culture )
  {
    if ( value is not IList list ) return 0 ;
    return list.Count ;
  }

  public object ConvertBack( object? value, Type targetType, object? parameter, CultureInfo culture )
  {
   return System.Windows.Data.Binding.DoNothing ;
  }
}