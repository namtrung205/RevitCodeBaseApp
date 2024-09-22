using System.IO ;
using System.Reflection ;

namespace Helpers.Utils ;

public static class ConstantsPathFolder
{
  private static readonly string ResourcesFolder = "GeneralResources" ;

  public static readonly string PathJsonCraneInfo =
    Path.Combine( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location )!, ResourcesFolder, "CraneInfo.json" ) ;
}