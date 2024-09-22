using System ;
using System.Globalization ;
using System.Windows ;
using System.Windows.Data ;

namespace RevitAddinApp._02_Views.Converters ;

public class EnumToBooleanConvertParameter : IValueConverter
{
  public object Convert( object? value, Type targetType, object? parameter, CultureInfo culture )
  {
    var parameterString = parameter?.ToString() ;
    if ( parameterString == null )
      return DependencyProperty.UnsetValue ;
    if ( value != null && Enum.IsDefined( value.GetType(), value ) == false )
      return DependencyProperty.UnsetValue ;
    if ( value?.GetType() == null )
      return DependencyProperty.UnsetValue ;
    var parameterValue = Enum.Parse( value.GetType(), parameterString ) ;
    return parameterValue.Equals( value ) ;
  }

  public object? ConvertBack( object? value, Type targetType, object? parameter, CultureInfo culture )
  {
    if ( parameter == null ) return DependencyProperty.UnsetValue ;
    var parameterString = parameter.ToString() ;
    var enumValue = Enum.Parse( targetType, parameterString ) ;
    if ( enumValue.GetType() == targetType ) return enumValue ;
    return null ;
  }
}