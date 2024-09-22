using System.Collections.Generic ;

namespace Helpers.Utils ;

public static class UtAutoName
{
  public static string CheckAndIncreaseName( this string currentName, List<string> listTypeNames )
  {
    if ( ! listTypeNames.Exists( x => x == currentName ) ) return currentName ;
    var result = "" ;
    for ( var i = 1 ; i < 100000 ; i++ )
      if ( ! listTypeNames.Contains( currentName + " (" + i + ")" ) ) {
        result = currentName + " (" + i + ")" ;
        break ;
      }

    return result ;
  }

  public static string CheckAndIncreaseNameCsv( this string currentName, List<string> listTypeNames )
  {
    var result = "" ;
    var name = currentName.Replace( ".csv", string.Empty ) ;
    var x = 0 ;
    foreach ( var typeName in listTypeNames )
      if ( typeName.Contains( name ) )
        x += 1 ;
    for ( var i = 1 ; i < 100000 ; i++ )
      if ( x <= i && ! currentName.Contains( " (" + i + ")" ) ) {
        var index = currentName.Length - 4 ;
        if ( index < 0 ) return string.Empty ;
        result = currentName.Insert( index, " (" + i + ")" ) ;
        break ;
      }

    return result ;
  }
}