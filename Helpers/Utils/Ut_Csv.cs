using System ;
using System.Collections.Generic ;
using System.Diagnostics ;
using System.IO ;
using System.Linq ;
using System.Text ;
using System.Windows ;
using Csv ;

namespace Helpers.Utils ;

public static class UtCsv
{
  public static List<T> ReadCsv<T>( string pathCsv, HeaderMode headerMode, List<string> listPropertyName, out bool isError ) where T : new()
  {
    isError = false ;
    var listOut = new List<T>() ;
    if ( ! File.Exists( pathCsv ) || ! listPropertyName.Any() ) {
      isError = true ;
      return listOut ;
    }

    var csvString = ReadCloneFileCsv( pathCsv ) ;
    var options = headerMode == HeaderMode.HeaderAbsent
      ? new CsvOptions { HeaderMode = HeaderMode.HeaderAbsent }
      : new CsvOptions { HeaderMode = HeaderMode.HeaderPresent } ;
    foreach ( var line in CsvReader.ReadFromText( csvString, options ).ToList() ) {
      var row = new T() ;
      var listValueObject = new List<string>() ;
      for ( var i = 0 ; i < listPropertyName.Count ; i++ ) {
        var value = (object) line[ i ] ;
        listValueObject.Add( value.ToString() ) ;
        row.SetValuePropertyBaseByName( listPropertyName[ i ], value ) ;
      }

      if ( listValueObject.All( string.IsNullOrEmpty ) ) break ;
      listOut.Add( row ) ;
    }

    if ( ! listOut.Any() ) isError = true ;
    return listOut ;
  }

  public static void ExportCsv<T>( string pathSave, List<string> listStringHeader, List<T> listPropertyExport, List<string> listNamePropertyExport )
  {
    var listWrite = new List<string>() ;
    if ( ! listStringHeader.Any() || ! listNamePropertyExport.Any() ) return ;
    var header = string.Empty ;
    foreach ( var s in listStringHeader ) {
      if ( s == listStringHeader.Last() ) header += $"{s}" ;
      header += $"{s};" ;
    }

    listWrite.Add( header ) ;

    foreach ( var property in listPropertyExport ) {
      var row = string.Empty ;
      foreach ( var nameProperty in listNamePropertyExport ) row += property.GetValuePropertyByNames( nameProperty ) ?? string.Empty ;
      listWrite.Add( row ) ;
    }

    var dataWrite = "" ;
    listWrite.ForEach( l => dataWrite += l + "\n" ) ;
    File.WriteAllText( pathSave, dataWrite, Encoding.UTF8 ) ;
  }

  public static void OpenFile( string path )
  {
    if ( UtilsMessageCad.Notification_YesNo( "CSV出力は完了です。開きますか？" ) != MessageBoxResult.Yes ) return ;
    var p = new Process() ;
    p.StartInfo.UseShellExecute = true ;
    p.StartInfo.FileName = path ;
    p.Start() ;
  }

  private static string ReadCloneFileCsv( string filePath )
  {
    string csvString ;
    try {
      csvString = File.ReadAllText( filePath ) ;
    }
    catch {
      var fileCopy = Path.Combine( Path.GetDirectoryName( filePath )!, $"{Guid.NewGuid()}.csv" ) ;
      File.Copy( filePath, fileCopy ) ;
      csvString = File.ReadAllText( fileCopy ) ;
      File.Delete( fileCopy ) ;
    }

    return csvString ;
  }
}