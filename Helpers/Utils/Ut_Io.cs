using System ;
using System.IO ;

namespace Helpers.Utils ;

public class UtIo
{
  //Check file Locked
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

  public static bool IsFileExist( string filePath )
  {
    try {
      if ( File.Exists( filePath ) ) return true ;
    }
    catch {
      // ignored
    }

    return false ;
  }

  public static string GetTempFolderWidthDoubleSplash()
  {
    var path = Path.GetTempPath() ;
    //path = path.Replace("\\", "\\\\");
    return path ;
  }

  public static string? CloneFileWindowWithGuidName( string filePath )
  {
    try {
      // Check if the file exists
      if ( ! File.Exists( filePath ) ) {
        Console.WriteLine( "The specified file does not exist." ) ;
        return null ; // Exit the program if the file does not exist
      }

      // Get the directory of the original file
      var directoryPath = Path.GetDirectoryName( filePath ) ;

      // Generate a new GUID for the new file name
      var newFileName = Guid.NewGuid() + Path.GetExtension( filePath ) ;

      // Combine the directory path and the new file name to create the full path
      if ( directoryPath != null ) {
        var newFilePath = Path.Combine( directoryPath, newFileName ) ;
        // Copy the original file to the new file path
        File.Copy( filePath, newFilePath ) ;
        return newFilePath ;
      }

      return null ;
    }
    catch ( System.Exception e ) {
      Console.WriteLine( e ) ;
      return null ;
    }
  }

  public static string? CloneFileWindowWithGuidNameToTempFolder( string filePath )
  {
    try {
      // Check if the file exists
      if ( ! File.Exists( filePath ) ) {
        Console.WriteLine( "The specified file does not exist." ) ;
        return null ; // Exit the program if the file does not exist
      }

      // Get the directory of the original file
      var directoryPath = GetTempFolderWidthDoubleSplash() ;

      // Generate a new GUID for the new file name
      var newFileName = Guid.NewGuid() + Path.GetExtension( filePath ) ;

      // Combine the directory path and the new file name to create the full path
      {
        var newFilePath = Path.Combine( directoryPath, newFileName ) ;
        // Copy the original file to the new file path
        File.Copy( filePath, newFilePath ) ;
        return newFilePath ;
      }
    }
    catch ( System.Exception e ) {
      Console.WriteLine( e ) ;
      return null ;
    }
  }


  public static string? CreateTempFilePath( string filePath )
  {
    try {
      // Check if the file exists
      if ( ! File.Exists( filePath ) ) {
        Console.WriteLine( "The specified file does not exist." ) ;
        return null ; // Exit the program if the file does not exist
      }

      // Get the directory of the original file
      var directoryPath = Path.GetDirectoryName( filePath ) ;

      // Generate a new GUID for the new file name
      var newFileName = Guid.NewGuid() + Path.GetExtension( filePath ) ;

      // Combine the directory path and the new file name to create the full path
      if ( directoryPath != null ) {
        var newFilePath = Path.Combine( directoryPath, newFileName ) ;
        return newFilePath ;
      }

      return null ;
    }
    catch ( System.Exception e ) {
      Console.WriteLine( e ) ;
      return null ;
    }
  }


  public static void RemoveFileInWindow( string filePath )
  {
    try {
      if ( File.Exists( filePath ) ) {
        File.Delete( filePath ) ;
        Console.WriteLine( "File deleted successfully." ) ;
      }
      else {
        Console.WriteLine( "File not found." ) ;
      }
    }
    catch ( System.Exception ex ) {
      Console.WriteLine( $"An error occurred: {ex.Message}" ) ;
    }
  }
}