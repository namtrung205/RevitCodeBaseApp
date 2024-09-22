using System ;
using System.Globalization ;
using System.IO ;
using System.Linq ;
using System.Windows.Data ;

namespace Helpers.Utils_Wpf ;

public class LongPathConverter : IValueConverter
{
  public object Convert( object? value, Type targetType, object? parameter, CultureInfo culture )
  {
    if ( value == null ) return string.Empty ;
    var path = (string) value ;
    if ( string.IsNullOrEmpty( path ) ) return string.Empty ;
    if ( parameter is not string stringPathLength ) return path ;
    int.TryParse( stringPathLength, out var pathLength ) ;
    if ( pathLength == 0 ) return path ;
    var directoryName = Path.GetDirectoryName( path ) ;
    if ( string.IsNullOrEmpty( directoryName ) ) return string.Empty ;
    var dirs = directoryName.Split( '\\' ).Reverse() ;
    var fileName = Path.GetFileName( path ) ;
    if ( string.IsNullOrEmpty( fileName ) ) return string.Empty ;
    var totalLength = fileName.Length ;
    if ( totalLength > pathLength ) return "..\\.." + fileName.Substring( totalLength - pathLength + 5 ) ;
    var index = -1 ;
    var lstDirIndex = dirs.ToList().Select( d =>
    {
      index += 1 ;
      totalLength += d.Length + 1 ;
      return new { index = index + 1, length = totalLength, dir = d } ;
    } ).ToList() ;
    if ( ! lstDirIndex.Any() || lstDirIndex.Last().length <= pathLength ) return path ;

    var shortPath = "..\\" ;
    var dirReverse = lstDirIndex.Where( x => x.length < pathLength ).Reverse() ;
    dirReverse.ToList().ForEach( x => shortPath += x.dir + "\\" ) ;
    shortPath += fileName ;

    return shortPath.Length > pathLength ? "..\\.." + shortPath.Substring( shortPath.Length - pathLength + 5 ) : shortPath ;
  }

  public object ConvertBack( object? value, Type targetType, object? parameter, CultureInfo culture )
  {
    return string.Empty ;
  }
}