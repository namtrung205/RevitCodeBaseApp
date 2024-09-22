using System.Diagnostics ;
using System.IO ;

namespace Helpers.Utils ;

public class UtOpenPdf
{
  public static void OpenPagePdfByMicrosoftEdge( string pathInput, int? page = null )
  {
    var pageNotnull = page ?? 0 ;
    if ( string.IsNullOrEmpty( pathInput ) ) return ;
    if ( ! File.Exists( pathInput ) ) return ;

    var pathEdgeExe = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe" ;
    if ( File.Exists( pathEdgeExe ) ) {
      var process = new Process() ;
      var startInfo = new ProcessStartInfo() ;
      process.StartInfo = startInfo ;
      startInfo.FileName = pathEdgeExe ;
      var path = pathInput.Replace( " ", "%20" ) ;
      startInfo.Arguments = $" file://{path}#page={pageNotnull}" ;
      process.Start() ;
      return ;
    }

    Process.Start( pathInput ) ;
  }
}