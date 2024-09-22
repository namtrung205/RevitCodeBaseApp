using Autodesk.Revit.Attributes ;
using Nice3point.Revit.Toolkit.External ;
using RevitAddinApp._02_Views ;
using RevitAddinApp._03_ViewModel ;

namespace RevitAddinApp.Commands
{
  /// <summary>
  ///     External command entry point invoked from the Revit interface
  /// </summary>
  [UsedImplicitly]
  [Transaction( TransactionMode.Manual )]
  public class ExportWallTypeCommand : ExternalCommand
  {
    public override void Execute()
    {
      Helpers.Utils.LoggerCit.Instance.LogInformation( "Execute StartupCommand" ) ;
      ExportWallTypeVm exportWallTypeVm = new ExportWallTypeVm() ;
      UiExportWallType ui = new UiExportWallType(exportWallTypeVm) ;
      ui.Show( UiApplication.MainWindowHandle ) ;
    }
  }
}