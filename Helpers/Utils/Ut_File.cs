using System.IO ;

namespace Helpers.Utils ;

public static class UtFile
{
  public static bool IsFileLocked( FileInfo file )
  {
    try {
      using var stream = file.Open( FileMode.Open, FileAccess.Read, FileShare.None ) ;
      stream.Close() ;
    }
    catch {
      return true ;
    }

    return false ;
  }
}