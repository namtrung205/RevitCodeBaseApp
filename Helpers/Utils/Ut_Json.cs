using System ;
using System.Collections.Generic ;
using System.IO ;
using Newtonsoft.Json ;

namespace Helpers.Utils ;

public static class UtJson
{
  public static void SaveIListJson<T>( IList<T> listSave, string pathSave )
  {
    try {
      var stringListSave = JsonConvert.SerializeObject( listSave ) ;
      File.WriteAllText( pathSave, stringListSave ) ;
    }
    catch ( System.Exception e ) {
     LoggerCit.Instance.LogError( e ) ;
    }
  }

  public static void SaveJson<T>( T objectSave, string pathSave )
  {
    try {
      var stringListSave = JsonConvert.SerializeObject( objectSave ) ;
      File.WriteAllText( pathSave, stringListSave ) ;
    }
    catch ( System.Exception e ) {
      LoggerCit.Instance.LogError( e ) ;
    }
  }

  public static List<T> GetListOut<T>( string pathSaveJson )
  {
    if ( ! File.Exists( pathSaveJson ) ) return new List<T>() ;
    try {
      return JsonConvert.DeserializeObject<List<T>>( File.ReadAllText( pathSaveJson ) ) ?? new List<T>() ;
    }
    catch ( System.Exception e ) {
      LoggerCit.Instance.LogError( e ) ;
      return new List<T>() ;
    }
  }

  public static T? GetOut<T>( string pathSaveJson )
  {
    if ( ! File.Exists( pathSaveJson ) ) return default ;
    try {
      return JsonConvert.DeserializeObject<T>( File.ReadAllText( pathSaveJson ) ) ?? default ;
    }
    catch ( System.Exception e ) {
      LoggerCit.Instance.LogError( e ) ;
      return default ;
    }
  }
}