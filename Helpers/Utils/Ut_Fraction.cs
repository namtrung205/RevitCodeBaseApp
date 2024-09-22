using System ;
using System.Globalization ;

// Sample
// DragInFactorString = new FractionUtils(0.15).ToString()

namespace Helpers.Utils ;

public class UtFraction
{
  private long _mIDenominator ;

  public UtFraction()
  {
    Initialize( 0, 1 ) ;
  }

  public UtFraction( long iWholeNumber )
  {
    Initialize( iWholeNumber, 1 ) ;
  }

  public UtFraction( double dDecimalValue )
  {
    var temp = ToFraction( dDecimalValue ) ;
    Initialize( temp.Numerator, temp.Denominator ) ;
  }

  public UtFraction( string strValue )
  {
    var temp = ToFraction( strValue ) ;
    Initialize( temp.Numerator, temp.Denominator ) ;
  }

  public UtFraction( long iNumerator, long iDenominator )
  {
    Initialize( iNumerator, iDenominator ) ;
  }

  private long Denominator
  {
    get => _mIDenominator ;
    set
    {
      if ( value != 0 )
        _mIDenominator = value ;
      else
        LoggerCit.Instance.LogError(null,string.Empty, "Denominator cannot be assigned a ZERO Value" );  
    }
  }

  private long Numerator { get ; set ; }

  public long Value
  {
    set
    {
      Numerator = value ;
      _mIDenominator = 1 ;
    }
  }

  private void Initialize( long iNumerator, long iDenominator )
  {
    Numerator = iNumerator ;
    Denominator = iDenominator ;
    ReduceFraction( this ) ;
  }

  private double ToDouble()
  {
    return (double) Numerator / Denominator ;
  }

  public override string ToString()
  {
    string str ;
    if ( Denominator == 1 )
      str = Numerator.ToString() ;
    else
      str = Numerator + "/" + Denominator ;
    return str ;
  }

  private static UtFraction ToFraction( string strValue )
  {
    int i ;
    for ( i = 0 ; i < strValue.Length ; i++ )
      if ( strValue[ i ] == '/' )
        break ;

    if ( i == strValue.Length ) // if string is not in the form of a fraction
      // then it is double or integer
      return Convert.ToDouble( strValue ) ;
    //return ( ToFraction( Convert.ToDouble(strValue) ) );

    // else string is in the form of Numerator/Denominator
    var iNumerator = Convert.ToInt64( strValue.Substring( 0, i ) ) ;
    var iDenominator = Convert.ToInt64( strValue.Substring( i + 1 ) ) ;
    return new UtFraction( iNumerator, iDenominator ) ;
  }

  private static UtFraction ToFraction( double dValue )
  {
    UtFraction frac = new UtFraction();
    try {
      checked {
        if ( dValue % 1 == 0 ) // if whole number
        {
          frac = new UtFraction( (long) dValue ) ;
        }
        else {
          var dTemp = dValue ;
          long iMultiple = 1 ;
          var strTemp = dValue.ToString( CultureInfo.InvariantCulture ) ;
          while ( strTemp.IndexOf( "E", StringComparison.Ordinal ) > 0 ) // if in the form like 12E-9
          {
            dTemp *= 10 ;
            iMultiple *= 10 ;
            strTemp = dTemp.ToString( CultureInfo.InvariantCulture ) ;
          }

          var i = 0 ;
          while ( strTemp[ i ] != '.' )
            i++ ;
          var iDigitsAfterDecimal = strTemp.Length - i - 1 ;
          while ( iDigitsAfterDecimal > 0 ) {
            dTemp *= 10 ;
            iMultiple *= 10 ;
            iDigitsAfterDecimal-- ;
          }

          frac = new UtFraction( (int) Math.Round( dTemp ), iMultiple ) ;
        }

        return frac ;
      }
    }
    catch ( System.Exception ) {
      LoggerCit.Instance.LogError(null,string.Empty, "Conversion not possible due to overflow" ) ;
    }
    return frac ;
  }

  public UtFraction Duplicate()
  {
    var frac = new UtFraction { Numerator = Numerator, Denominator = Denominator } ;
    return frac ;
  }

  private static UtFraction Inverse( UtFraction frac1 )
  {
    if ( frac1.Numerator == 0 )
      LoggerCit.Instance.LogError(null,string.Empty, "Operation not possible (Denominator cannot be assigned a ZERO Value)" ) ;

    var iNumerator = frac1.Denominator ;
    var iDenominator = frac1.Numerator ;
    return new UtFraction( iNumerator, iDenominator ) ;
  }

  public static UtFraction operator -( UtFraction frac1 )
  {
    return Negate( frac1 ) ;
  }

  public static UtFraction operator +( UtFraction frac1, UtFraction frac2 )
  {
    return Add( frac1, frac2 ) ;
  }

  public static UtFraction operator +( int iNo, UtFraction frac1 )
  {
    return Add( frac1, new UtFraction( iNo ) ) ;
  }

  public static UtFraction operator +( UtFraction frac1, int iNo )
  {
    return Add( frac1, new UtFraction( iNo ) ) ;
  }

  public static UtFraction operator +( double dbl, UtFraction frac1 )
  {
    return Add( frac1, ToFraction( dbl ) ) ;
  }

  public static UtFraction operator +( UtFraction frac1, double dbl )
  {
    return Add( frac1, ToFraction( dbl ) ) ;
  }

  public static UtFraction operator -( UtFraction frac1, UtFraction frac2 )
  {
    return Add( frac1, -frac2 ) ;
  }

  public static UtFraction operator -( int iNo, UtFraction frac1 )
  {
    return Add( -frac1, new UtFraction( iNo ) ) ;
  }

  public static UtFraction operator -( UtFraction frac1, int iNo )
  {
    return Add( frac1, -new UtFraction( iNo ) ) ;
  }

  public static UtFraction operator -( double dbl, UtFraction frac1 )
  {
    return Add( -frac1, ToFraction( dbl ) ) ;
  }

  public static UtFraction operator -( UtFraction frac1, double dbl )
  {
    return Add( frac1, -ToFraction( dbl ) ) ;
  }

  public static UtFraction operator *( UtFraction frac1, UtFraction frac2 )
  {
    return Multiply( frac1, frac2 ) ;
  }

  public static UtFraction operator *( int iNo, UtFraction frac1 )
  {
    return Multiply( frac1, new UtFraction( iNo ) ) ;
  }

  public static UtFraction operator *( UtFraction frac1, int iNo )
  {
    return Multiply( frac1, new UtFraction( iNo ) ) ;
  }

  public static UtFraction operator *( double dbl, UtFraction frac1 )
  {
    return Multiply( frac1, ToFraction( dbl ) ) ;
  }

  public static UtFraction operator *( UtFraction frac1, double dbl )
  {
    return Multiply( frac1, ToFraction( dbl ) ) ;
  }

  public static UtFraction operator /( UtFraction frac1, UtFraction frac2 )
  {
    return Multiply( frac1, Inverse( frac2 ) ) ;
  }

  public static UtFraction operator /( int iNo, UtFraction frac1 )
  {
    return Multiply( Inverse( frac1 ), new UtFraction( iNo ) ) ;
  }

  public static UtFraction operator /( UtFraction frac1, int iNo )
  {
    return Multiply( frac1, Inverse( new UtFraction( iNo ) ) ) ;
  }

  public static UtFraction operator /( double dbl, UtFraction frac1 )
  {
    return Multiply( Inverse( frac1 ), ToFraction( dbl ) ) ;
  }

  public static UtFraction operator /( UtFraction frac1, double dbl )
  {
    return Multiply( frac1, Inverse( ToFraction( dbl ) ) ) ;
  }

  public static bool operator ==( UtFraction frac1, UtFraction frac2 )
  {
    return frac1.Equals( frac2 ) ;
  }

  public static bool operator !=( UtFraction frac1, UtFraction frac2 )
  {
    return ! frac1.Equals( frac2 ) ;
  }

  public static bool operator ==( UtFraction frac1, int iNo )
  {
    return frac1.Equals( new UtFraction( iNo ) ) ;
  }

  public static bool operator !=( UtFraction frac1, int iNo )
  {
    return ! frac1.Equals( new UtFraction( iNo ) ) ;
  }

  public static bool operator ==( UtFraction frac1, double dbl )
  {
    return frac1.Equals( new UtFraction( dbl ) ) ;
  }

  public static bool operator !=( UtFraction frac1, double dbl )
  {
    return ! frac1.Equals( new UtFraction( dbl ) ) ;
  }

  public static bool operator <( UtFraction frac1, UtFraction frac2 )
  {
    return frac1.Numerator * frac2.Denominator < frac2.Numerator * frac1.Denominator ;
  }

  public static bool operator >( UtFraction frac1, UtFraction frac2 )
  {
    return frac1.Numerator * frac2.Denominator > frac2.Numerator * frac1.Denominator ;
  }

  public static bool operator <=( UtFraction frac1, UtFraction frac2 )
  {
    return frac1.Numerator * frac2.Denominator <= frac2.Numerator * frac1.Denominator ;
  }

  public static bool operator >=( UtFraction frac1, UtFraction frac2 )
  {
    return frac1.Numerator * frac2.Denominator >= frac2.Numerator * frac1.Denominator ;
  }


  public static implicit operator UtFraction( long lNo )
  {
    return new UtFraction( lNo ) ;
  }

  public static implicit operator UtFraction( double dNo )
  {
    return new UtFraction( dNo ) ;
  }

  public static implicit operator UtFraction( string strNo )
  {
    return new UtFraction( strNo ) ;
  }

  public static explicit operator double( UtFraction frac )
  {
    return frac.ToDouble() ;
  }

  public static implicit operator string( UtFraction frac )
  {
    return frac.ToString() ;
  }

  public override bool Equals( object? obj )
  {
    if ( obj == null ) return false ;
    var frac = (UtFraction) obj ;
    return Numerator == frac.Numerator && Denominator == frac.Denominator ;
  }

  public override int GetHashCode()
  {
    return Convert.ToInt32( ( Numerator ^ Denominator ) & 0xFFFFFFFF ) ;
  }

  private static UtFraction Negate( UtFraction frac1 )
  {
    var iNumerator = -frac1.Numerator ;
    var iDenominator = frac1.Denominator ;
    return new UtFraction( iNumerator, iDenominator ) ;
  }

  private static UtFraction Add( UtFraction frac1, UtFraction frac2 )
  {
    try {
      checked {
        var iNumerator = frac1.Numerator * frac2.Denominator + frac2.Numerator * frac1.Denominator ;
        var iDenominator = frac1.Denominator * frac2.Denominator ;
        return new UtFraction( iNumerator, iDenominator ) ;
      }
    }
    catch ( System.Exception ) {
      LoggerCit.Instance.LogError( null,string.Empty,"An error occurred while performing" ) ;
    }
    return new UtFraction() ;
  }

  private static UtFraction Multiply( UtFraction frac1, UtFraction frac2 )
  {
    try {
      checked {
        var iNumerator = frac1.Numerator * frac2.Numerator ;
        var iDenominator = frac1.Denominator * frac2.Denominator ;
        return new UtFraction( iNumerator, iDenominator ) ;
      }
    }
    catch (System.Exception ) {
      LoggerCit.Instance.LogError( null,string.Empty, "An error occurred while performing " ) ;
    }
    return new UtFraction() ;
  }

  private static long GCD( long iNo1, long iNo2 )
  {
    if ( iNo1 < 0 ) iNo1 = -iNo1 ;
    if ( iNo2 < 0 ) iNo2 = -iNo2 ;
    do {
      if ( iNo1 < iNo2 ) ( iNo1, iNo2 ) = ( iNo2, iNo1 ) ;
      iNo1 = iNo1 % iNo2 ;
    } while ( iNo1 != 0 ) ;

    return iNo2 ;
  }

  private static void ReduceFraction( UtFraction frac )
  {
    try {
      if ( frac.Numerator == 0 ) {
        frac.Denominator = 1 ;
        return ;
      }

      var iGcd = GCD( frac.Numerator, frac.Denominator ) ;
      frac.Numerator /= iGcd ;
      frac.Denominator /= iGcd ;

      if ( frac.Denominator < 0 ) {
        frac.Numerator *= -1 ;
        frac.Denominator *= -1 ;
      }
    }
    catch ( System.Exception exp ) {
      LoggerCit.Instance.LogError( null,string.Empty, "Cannot reduce Fraction: " + exp.Message ) ;
    }
  }
}
