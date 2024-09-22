using System ;
using System.Collections.Generic ;
using System.Linq ;

namespace Helpers.Utils ;

public static class UtStringExtent
{
  public static string ReplaceListString( this string text, List<string> listString, string replaceTo )
  {
    var result = text ;
    if ( ! listString.Any() ) return result ;
    foreach ( var s in listString ) result = result.Replace( s, replaceTo ) ;
    return result ;
  }

  public static string RemoveDigitalFromFirst( this string text )
  {
    if ( string.IsNullOrWhiteSpace( text ) ) return text ;
    var stringOut = string.Empty ;
    foreach ( var c in text.Trim() ) {
      if ( char.IsDigit( c ) ) continue ;
      stringOut += c ;
    }

    return stringOut ;
  }

  public static string RemoveDigitalFromLast( this string text )
  {
    if ( string.IsNullOrWhiteSpace( text ) ) return text ;
    var stringOut = string.Empty ;
    var textReverse = string.Empty ;
    foreach ( var c in text.Trim().Reverse() ) {
      if ( char.IsDigit( c ) ) continue ;
      textReverse += c ;
    }

    foreach ( var c in textReverse.Reverse() ) stringOut += c ;
    return stringOut ;
  }

  public static List<int> ListIndexOfStr( this string text, string textSearch )
  {
    var foundIndexes = new List<int>() ;
    for ( var i = text.IndexOf( textSearch, StringComparison.Ordinal ) ;
         i > -1 ;
         i = text.IndexOf( textSearch, i + 1, StringComparison.Ordinal ) ) foundIndexes.Add( i ) ;
    return foundIndexes ;
  }

  public static List<string> ListStringBetween( string text, string textSearch )
  {
    var listIndexSearch = text.ListIndexOfStr( textSearch ) ;
    if ( ! listIndexSearch.Any() ) return new List<string>() ;
    var listString = new List<string>() ;

    if ( listIndexSearch.Count == 1 ) {
      var subTextFirst = text.Substring( 0, listIndexSearch[ 0 ] ) ;
      var subTextSecond = text.Substring( listIndexSearch[ 0 ], text.Length - listIndexSearch[ 0 ] ) ;
      listString.Add( subTextFirst ) ;
      listString.Add( subTextSecond ) ;
      return listString ;
    }

    for ( var i = 0 ; i < listIndexSearch.Count ; i++ ) {
      if ( i == 0 ) {
        var subTextFirst = text.Substring( 0, listIndexSearch[ 0 ] ) ;
        listString.Add( subTextFirst ) ;
        continue ;
      }

      if ( i == listIndexSearch.Count - 1 ) {
        var subTextFirst = text.Substring( listIndexSearch[ listIndexSearch.Count - 1 ], text.Length - listIndexSearch[ listIndexSearch.Count - 1 ] ) ;
        listString.Add( subTextFirst ) ;
        continue ;
      }

      var subText = text.Substring( listIndexSearch[ i ], listIndexSearch[ i + 1 ] - listIndexSearch[ i ] ) ;
      listString.Add( subText ) ;
    }

    return listString ;
  }
}