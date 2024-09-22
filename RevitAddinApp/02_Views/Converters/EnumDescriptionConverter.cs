using System ;
using System.ComponentModel ;
using System.Globalization ;
using System.Windows.Data ;

namespace RevitAddinApp._02_Views.Converters ;

public class EnumDescriptionConverter : IValueConverter
{
  public object Convert( object? value, Type targetType, object? parameter, CultureInfo culture )
  {
    if ( value is not Enum enumValue ) return string.Empty ;
    var fi = enumValue.GetType().GetField( enumValue.ToString() ) ;
    if ( fi == null ) return string.Empty ;
    var attributes = (DescriptionAttribute[]) fi.GetCustomAttributes( typeof( DescriptionAttribute ), false ) ;
    if ( attributes.Length > 0 )
      return attributes[ 0 ].Description ;
    return string.Empty ;
  }

  public object? ConvertBack( object? value, Type targetType, object? parameter, CultureInfo culture )
  {
    return value ;
  }
}