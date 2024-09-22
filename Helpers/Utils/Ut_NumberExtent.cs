using System ;

namespace Helpers.Utils ;

public static class UtNumberExtent
{
  private const double Precision = 10e-6 ;

  public static bool IsAlmostEqualTo( this double left, double right )
  {
    return Math.Abs( left - right ) < Precision ;
  }

  public static bool IsAlmostEqualTo( this double left, double right, double precision )
  {
    return Math.Abs( left - right ) < precision ;
  }

  public static string StringFormat( this double value, int digits )
  {
    return value.ToString( $"F{digits}" ) ;
  }

  public static double? TryConvertToDouble( this object? value )
  {
    if ( value == null ) return null ;
    var text = value.ToString() ;
    if ( string.IsNullOrEmpty( text ) ) return null ;
    if ( double.TryParse( text, out var result ) ) return result ;
    return null ;
  }

  public static int? TryConvertToInt( this object? value )
  {
    if ( value == null ) return null ;
    var text = value.ToString() ;
    if ( string.IsNullOrEmpty( text ) ) return null ;
    if ( int.TryParse( text, out var result ) ) return result ;
    return null ;
  }
}