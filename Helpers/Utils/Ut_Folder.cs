using System.Collections.Generic ;
using System.IO ;
using System.Linq ;
using Microsoft.WindowsAPICodePack.Dialogs ;

namespace Helpers.Utils ;

public static class UtFolder
{
  public static string? GetFolder()
  {
    CommonOpenFileDialog dialog = new() { IsFolderPicker = true, EnsurePathExists = true } ;
    var result = dialog.ShowDialog() ;
    if ( result != CommonFileDialogResult.Ok ) return null ;
    return dialog.FileName ;
  }

  public static List<string> GetListPathFromFolder( List<string> listExtentAllow )
  {
    var pathFolder = GetFolder() ;
    if ( pathFolder == null ) return new List<string>() ;
    var allFile = Directory.GetFiles( pathFolder, "*.*", SearchOption.TopDirectoryOnly )
      .Where( s => listExtentAllow.Contains( Path.GetExtension( s ).ToLower() ) ).Distinct().ToList() ;
    if ( ! allFile.Any() ) return new List<string>() ;
    return allFile ;
  }
}