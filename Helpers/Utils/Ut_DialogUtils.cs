using System ;
using System.Collections.Generic ;
using System.Diagnostics ;
using System.IO ;
using System.Linq ;
using System.Windows ;
using System.Windows.Forms ;
using MessageBox = System.Windows.Forms.MessageBox ;

namespace Helpers.Utils ;

public static class UtDialogUtils
{
  private const string FileOpeningError = "別のプログラムがこのファイルを開いているので挿入出来ません" ;
  private const string FileSaveEngError = "An error occurred while trying to save the file." ;
  private const string FileOpeningEngError = "An error occurred while trying to open the file." ;

  // Can Pick Folder or File
  public static string? GetPath( string title, List<string> listExtent )
  {
    var dialog = new OpenFileDialog() ;
    var stringDefault = "Select Folder." ;
    dialog.ValidateNames = false ;
    dialog.Title = title ;
    dialog.CheckFileExists = false ;
    dialog.CheckPathExists = true ;
    dialog.FileName = stringDefault ;
    dialog.Filter = GetFilter( title, listExtent ) ;

    if ( dialog.ShowDialog() != DialogResult.OK ) return null ;
    if ( ! dialog.FileName.Contains( stringDefault ) ) {
      if ( File.Exists( dialog.FileName ) ) return dialog.FileName ;
      return null ;
    }

    return Path.GetDirectoryName( dialog.FileName ) ;
  }

  public static string? GetFileExcel( string title )
  {
    var chooseDialog = new OpenFileDialog() ;
    chooseDialog.Filter = $"{title}|*.xls;*.xlsx;*.xlsm" ;
    chooseDialog.Title = title ;
    chooseDialog.FilterIndex = 1 ;

    if ( chooseDialog.ShowDialog() == DialogResult.OK ) {
      if ( IsFileLocked( new FileInfo( chooseDialog.FileName ) ) ) {
        MessageBox.Show( FileOpeningError ) ;
        return null ;
      }

      return chooseDialog.FileName ;
    }

    return null ;
  }

  public static string? GetCsv( string title )
  {
    OpenFileDialog openFileDialog = new() ;
    openFileDialog.Filter = $"{title} (*csv)|*csv" ;
    openFileDialog.Title = title ;
    openFileDialog.FilterIndex = 1 ;

    if ( openFileDialog.ShowDialog() == DialogResult.OK ) return openFileDialog.FileName ;
    return null ;
  }

  public static string? GetImage( string title, List<string> listExtent )
  {
    var chooseDialog = new OpenFileDialog() ;
    chooseDialog.Title = title ;
    chooseDialog.Filter = GetFilter( title, listExtent ) ;
    chooseDialog.FilterIndex = 1 ;

    if ( chooseDialog.ShowDialog() == DialogResult.OK ) {
      if ( IsFileLocked( new FileInfo( chooseDialog.FileName ) ) ) {
        MessageBox.Show( FileOpeningError ) ;
        return null ;
      }

      return chooseDialog.FileName ;
    }

    return null ;
  }

  public static string? GetFileDwg( string title, bool checkFileLocked = true )
  {
    var chooseDialog = new OpenFileDialog() ;
    chooseDialog.Filter = $"{title}(*.dwg)|*.dwg" ;
    chooseDialog.Title = title ;
    chooseDialog.FilterIndex = 1 ;

    if ( chooseDialog.ShowDialog() == DialogResult.OK ) {
      if ( checkFileLocked )
        if ( IsFileLocked( new FileInfo( chooseDialog.FileName ) ) ) {
          MessageBox.Show( FileOpeningError ) ;
          return null ;
        }

      return chooseDialog.FileName ;
    }

    return null ;
  }


  private static string? GetFilter( string? titleFilter, List<string>? listExtensionFilter )
  {
    if ( listExtensionFilter == null || ! listExtensionFilter.Any() || string.IsNullOrEmpty( titleFilter ) ) return null ;
    var output = $"{titleFilter}|" ;
    listExtensionFilter.ForEach( x => output += $"*{x};" ) ;
    return output.Remove( output.Length - 1 ) ;
  }

  private static bool IsFileLocked( FileInfo file )
  {
    try {
      if ( file.Exists ) {
        using var stream = file.Open( FileMode.Open, FileAccess.Read, FileShare.None ) ;
        stream.Close() ;
      }
    }
    catch(System.Exception) {
      return true ;
    }
    return false ;
  }


  public static string? SaveFileDwg( string title )
  {
    var saveFileDialog = new SaveFileDialog() ;
    saveFileDialog.Filter = $"{title}(*.dwg)|*.dwg" ;
    saveFileDialog.Title = title ;

    if ( saveFileDialog.ShowDialog() == DialogResult.OK ) {
      if ( File.Exists( saveFileDialog.FileName ) && IsFileLocked( new FileInfo( saveFileDialog.FileName ) ) ) {
        MessageBox.Show( FileOpeningError ) ;
        return "" ;
      }

      return saveFileDialog.FileName ;
    }

    return "" ;
  }

  public static void OpenFileByProcess( string? path, string mess )
  {
    try {
      if ( string.IsNullOrWhiteSpace( path ) || ! File.Exists( path ) ) return ;
      if ( UtilsMessageCad.Notification_YesNo( mess ) != MessageBoxResult.Yes ) return ;
      Process.Start( path ) ;
    }
    catch {
      var messError = FileOpeningEngError ;
      UtilsMessageCad.Error( messError ) ;
    }
  }

  #region Save File

  public static string? SaveFileExcel( string title, string nameDefault = "" )
  {
    string? fileSave = null ;
    SaveFileDialog saveFileDialog1 = new() ;
    saveFileDialog1.CheckFileExists = false ;
    saveFileDialog1.CheckPathExists = false ;
    saveFileDialog1.ValidateNames = false ;

    saveFileDialog1.Filter = "Excel Files|*.xlsx;*.xlsm;*.xls" ;
    saveFileDialog1.Title = title ;
    if ( ! string.IsNullOrEmpty( nameDefault ) ) saveFileDialog1.FileName = $"{DateTime.Now:yyyyMMdd}_{nameDefault}" ;

    if ( saveFileDialog1.ShowDialog() == DialogResult.OK ) {
      if ( IsFileLocked( new FileInfo( saveFileDialog1.FileName ) ) ) {
        var messError = FileSaveEngError ;
        UtilsMessageCad.Warning( messError ) ;
        return null ;
      }

      fileSave = saveFileDialog1.FileName ;
    }

    return fileSave ;
  }

  public static string? SaveFileDwg( string title, string nameDwg )
  {
    string? fileSave = null ;
    SaveFileDialog saveFileDialog1 = new() ;
    saveFileDialog1.CheckFileExists = false ;
    saveFileDialog1.CheckPathExists = false ;
    saveFileDialog1.ValidateNames = false ;

    saveFileDialog1.Filter = "Autocad files (*.dwg)|*.dwg" ;
    saveFileDialog1.Title = title ;
    saveFileDialog1.FileName = nameDwg ;

    if ( saveFileDialog1.ShowDialog() == DialogResult.OK ) fileSave = saveFileDialog1.FileName ;
    return fileSave ;
  }

  #endregion
}