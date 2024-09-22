using System ;
using System.Diagnostics ;

namespace Helpers.Utils ;

public class UtProcesses
{
  public static void KillAllSilentProcesses( string processName )
  {
    try {
      // Get all processes named "EXCEL"
      Process[] excelProcesses = Process.GetProcessesByName( processName ) ;

      if ( excelProcesses.Length == 0 ) {
      }
      else {
        foreach ( var process in excelProcesses )
          // Check if the process is running without a UI (silent mode)
          if ( process.MainWindowHandle == IntPtr.Zero ) {
            process.Kill() ;
          }
      }
    }
    catch ( System.Exception ) {
    }
  }
}