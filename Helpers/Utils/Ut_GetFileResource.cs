using System.IO ;
using System.Reflection ;

namespace Helpers.Utils ;

public static class UtGetFileResource
{
  private const string Resources = "GeneralResources" ;

  public static string GetFileResource( string fileName )
  {
    var location = new FileInfo( Assembly.GetExecutingAssembly().Location ).DirectoryName ;
    return Path.Combine( location!, Resources, fileName ) ;
  }

  public static string GetResource()
  {
    var location = new FileInfo( Assembly.GetExecutingAssembly().Location ).DirectoryName ;
    return Path.Combine( location!, Resources ) ;
  }
}