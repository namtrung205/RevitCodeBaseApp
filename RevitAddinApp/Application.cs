using System.Reflection ;
using Helpers.Utils ;
using JetBrains.Annotations ;
using Nice3point.Revit.Extensions ;
using Nice3point.Revit.Toolkit.External ;
using RevitAddinApp.Commands ;


namespace RevitAddinApp
{
  /// <summary>
  ///     Application entry point
  /// </summary>
  [UsedImplicitly]
  public class Application : ExternalApplication
  {
    public override void OnStartup()
    {
      var logFolder = Assembly.GetExecutingAssembly().GetName().Name ;
      if ( logFolder != null ) LoggerCit.LogFolder = logFolder ;

      CreateRibbon() ;
    }

    public override void OnShutdown()
    {
    }

    private void CreateRibbon()
    {
      var panel = Application.CreatePanel( "Commands", "RevitAddinApp" ) ;
      panel.AddPushButton<ExportWallTypeCommand>( "Execute" ).SetImage( "/RevitAddinApp;component/Resources/Icons/RibbonIcon16.png" ).SetLargeImage( "/RevitAddinApp;component/Resources/Icons/RibbonIcon16.png" ) ;
    }
  }
}