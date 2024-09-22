using System ;
using System.ComponentModel ;

namespace Helpers.Utils ;

public static class UtEnum
{
  public static string Description<T>( this T value ) where T : Enum
  {
    var fi = value?.GetType().GetField( value.ToString() ) ;
    if ( fi == null ) return string.Empty ;
    var attributes = (DescriptionAttribute[]) fi.GetCustomAttributes( typeof( DescriptionAttribute ), false ) ;
    if ( attributes.Length > 0 )
      return attributes[ 0 ].Description ;
    return string.Empty ;
  }

  public static TEnum? TryConvertToEnum<TEnum>( this object? value ) where TEnum : struct, Enum
  {
    if ( value == null ) return default ;
    var text = value.ToString() ;
    if ( string.IsNullOrEmpty( text ) ) return default ;
    if ( Enum.TryParse( text, out TEnum result ) ) return result ;
    return default ;
  }
}