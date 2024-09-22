using System.Windows ;
using System.Windows.Controls ;
using RevitAddinApp._03_ViewModel ;

namespace RevitAddinApp._02_Views ;

public partial class UiExportWallType
{
  private readonly ExportWallTypeVm _exportExportWallTypeVm ;

  public UiExportWallType( ExportWallTypeVm exportWallTypeVm )
  {
    _exportExportWallTypeVm = exportWallTypeVm ;
    DataContext = exportWallTypeVm ;
    _exportExportWallTypeVm.UiExportWallType = this ;
    InitializeComponent() ;
  }

  private void ButtonBase_OnClick( object sender, RoutedEventArgs e )
  {
    Close() ;
  }

  private void CheckBoxAll_OnClick( object sender, RoutedEventArgs e )
  {
    var cb = e.Source as CheckBox ;
    if ( cb != null && ! cb.IsChecked.HasValue )
      cb.IsChecked = false ;
  }
}