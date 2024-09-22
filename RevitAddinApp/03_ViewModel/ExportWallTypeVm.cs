using System.Collections.ObjectModel ;
using System.Windows.Controls ;
using System.Windows.Input ;
using Autodesk.Revit.UI ;
using RevitAddinApp._01_Model ;
using RevitAddinApp._01_Model.Enums ;
using RevitAddinApp._02_Views ;
// using RevitAddinApp.Constants ;
// using RevitAddinApp.Services ;
using Helpers.Utils ;


namespace RevitAddinApp._03_ViewModel ;

public class ExportWallTypeVm : ViewModelBase
{
  private ExportWallToExcelInfo _exportWallToExcelInfo = new() ;

  private bool? _isAllItemChecked ;

  private bool _isEnableApply ;

  private ObservableCollection<ExportWallToExcelInfo> _listExportVolumeToExcelInfos = new() ;
  private List<Wall> _listSectionView = new() ;


  private string _numberViewSelected = "選択" ;

  private string _totalSelectedString = "選択横断数：0" ;

  private TypeConstruction _typeConstruction = TypeConstruction.TemporaryConstruction ;

  private ExternalCommandData _externalCommandData ;
  
  public ExportWallTypeVm(ExternalCommandData externalCommandData)
  {
    this._externalCommandData = externalCommandData ;
    IsAllItemChecked = true ;
    ListExportVolumeToExcelInfos = new ObservableCollection<ExportWallToExcelInfo>() ;
    SelectSectionViewCommand = new RelayCommand<object>( _ => true, SelectWallInvoke ) ;
    ApplyCommand = new RelayCommand<object>( _ => true, ApplyInvoke ) ;

    OnItemCheckedChangeCommand = new RelayCommand<object>( _ => true, _ =>
    {
      IsEnableApply = true ;
      var totalSelected = ListExportVolumeToExcelInfos.Where( x => x.IsSelected ).Count() ;
      TotalSelectedString = $"選択横断数：{totalSelected}" ;
      if ( totalSelected == 0 ) IsEnableApply = false ;
      UpdateSelectAllState() ;
      if ( UiExportWallType != null && UiExportWallType.DataGrid.Columns[ 0 ].Header is CheckBox checkBoxHeader ) checkBoxHeader.IsChecked = true ;
    } ) ;
  }

  public List<Wall> ListSectionView
  {
    get => _listSectionView ;
    set
    {
      _listSectionView = value ;
      OnPropertyChanged() ;
    }
  }

  public TypeConstruction TypeConstruction
  {
    get => _typeConstruction ;
    set
    {
      _typeConstruction = value ;
      OnPropertyChanged() ;
    }
  }

  public string TotalSelectedString
  {
    get => _totalSelectedString ;
    set
    {
      _totalSelectedString = value ;
      OnPropertyChanged() ;
    }
  }

  public ObservableCollection<ExportWallToExcelInfo> ListExportVolumeToExcelInfos
  {
    get => _listExportVolumeToExcelInfos ;
    set
    {
      _listExportVolumeToExcelInfos = value ;
      OnPropertyChanged() ;
    }
  }

  public ExportWallToExcelInfo ExportWallToExcelInfo
  {
    get => _exportWallToExcelInfo ;
    set
    {
      _exportWallToExcelInfo = value ;
      OnPropertyChanged() ;
    }
  }

  public bool? IsAllItemChecked
  {
    get => _isAllItemChecked ;
    set
    {
      _isAllItemChecked = value ;
      OnPropertyChanged() ;
    }
  }

  public bool IsEnableApply
  {
    get => _isEnableApply ;
    set
    {
      _isEnableApply = value ;
      OnPropertyChanged() ;
    }
  }

  public string NumberViewSelected
  {
    get => _numberViewSelected ;
    set
    {
      _numberViewSelected = value ;
      OnPropertyChanged() ;
    }
  }


  public UiExportWallType? UiExportWallType { get ; set ; }

  public ICommand SelectSectionViewCommand { get ; set ; }
  public ICommand ApplyCommand { get ; set ; }

  public ICommand OnItemCheckedChangeCommand { get ; set ; }

  private void SelectWallInvoke( object obj )
  {
    UIDocument uiDoc = _externalCommandData.Application.ActiveUIDocument;
    HelperRevit.UtSelection.SelectObjectByTypes( uiDoc ) ;

    // if ( UiExportWallType == null ) return ;
    // UiExportWallType.Hide() ;
    // // var listSectionView = UtSectionView.FilterCrossSectionViews( ConstantMessages.PromptSelectSectionView ) ;
    // UiExportWallType.Show() ;
    // if ( ! listSectionView.Any() ) return ;
    // ListSectionView = listSectionView ;
    // NumberViewSelected = ! ListSectionView.Any() ? "選択" : $"横断数：{ListSectionView.Count}" ;
    // Mouse.OverrideCursor = Cursors.Wait ;
    // //Fill data to gridView
    // ListExportVolumeToExcelInfos = new ObservableCollection<ExportWallToExcelInfo>( InitView() ) ;
    // Mouse.OverrideCursor = null ;
    // OnItemCheckedChangeCommand.Execute( null ) ;
  }

  private void UpdateSelectAllState()
  {
    var selectedCount = ListExportVolumeToExcelInfos.Count( x => x.IsSelected ) ;

    if ( selectedCount == 0 )
      IsAllItemChecked = false ;
    else if ( selectedCount == ListExportVolumeToExcelInfos.Count )
      IsAllItemChecked = true ;
    else
      IsAllItemChecked = null ; // Indeterminate state
  }


  private void ApplyInvoke( object obj )
  {
    // var doc = Application.DocumentManager.MdiActiveDocument ;
    // var path = UtDialogUtils.SaveFileExcel( "Excel Files" ) ;
    // if ( path == null ) return ;
    // ProgressMeter pm = new() ;
    // doc.TransactionStart( tran =>
    // {
    //   Mouse.OverrideCursor = Cursors.Wait ;
    //   var listAllStructure = new List<string>() ;
    //   var listSectionViewInfo = new List<SectionViewInfo>() ;
    //
    //   var listSectionViewSelected = ListExportVolumeToExcelInfos.Where( x => x.IsSelected ).Select( x => x.SectionView ).ToList() ;
    //
    //
    //   var tempLimit = listSectionViewSelected.Count ;
    //   pm.SetLimit( tempLimit ) ;
    //
    //   foreach ( var sectionView in listSectionViewSelected ) {
    //     var listMaterial = sectionView.GetListMaterialSectionHasValue( tran ) ;
    //     var sectionViewInfo = new SectionViewInfo { SectionView = sectionView, ListMaterialSectionHasValue = listMaterial } ;
    //     listSectionViewInfo.Add( sectionViewInfo ) ;
    //     pm.MeterProgress() ;
    //   }
    //
    //   //Get all list structure
    //   var listMaterialHasValue = new List<MaterialSection>() ;
    //   listSectionViewInfo.ForEach( x => listMaterialHasValue.AddRange( x.ListMaterialSectionHasValue ) ) ;
    //   var listMaterialSectionsHasValueDistinct = listMaterialHasValue.Distinct().ToList() ;
    //   if ( ! listMaterialSectionsHasValueDistinct.Any() ) {
    //     Mouse.OverrideCursor = null ;
    //     return true ;
    //   }
    //
    //   foreach ( var materialSection in listMaterialSectionsHasValueDistinct ) {
    //     var nameStructure = materialSection.GetStructureLayers() ;
    //     if ( listAllStructure.All( x => x != nameStructure ) && ! string.IsNullOrEmpty( nameStructure ) ) listAllStructure.Add( nameStructure ) ;
    //   }
    //
    //   listAllStructure = [..listAllStructure.Distinct()] ;
    //
    //   var listCut = CalculateVolume.DetectListMaterialCutOrFill( listMaterialHasValue ).ListMaterialCut ;
    //
    //   var listStructureName = listCut.Select( x => x.GetStructureLayers() ).Distinct().ToList() ;
    //
    //   if ( TypeConstruction == TypeConstruction.Construction )
    //     ExportVolumeToExcel.ExportConstructionVolumeToExcel( listSectionViewInfo, listAllStructure, tran, path ) ;
    //   else
    //     ExportVolumeToExcel.ExportTempConstructionVolumeToExcel( listSectionViewInfo, listStructureName, tran, path ) ;
    //
    //   return true ;
    // } ) ;
    // pm.Stop() ;
  }

  private List<ExportWallToExcelInfo> InitView()
  {
    // var listExportVolumeToExcelInfos = new List<ExportWallToExcelInfo>() ;
    // var doc = Application.DocumentManager.MdiActiveDocument ;
    // doc.TransactionStart( tran =>
    // {
    //   foreach ( var sectionView in ListSectionView ) {
    //     var station = sectionView.GetStation( tran ) ;
    //     var exportSecViewToExelInfo = new ExportWallToExcelInfo
    //     {
    //       SectionView = sectionView,
    //       IsSelected = true,
    //       StationString = $"{UtSectionView.StationToStationName( station!.Value )}",
    //       RatioDisplayName = $"\u00f7{sectionView.GetRatioVerticalExaggeration( tran )}"
    //     } ;
    //     listExportVolumeToExcelInfos.Add( exportSecViewToExelInfo ) ;
    //   }
    //
    //   return true ;
    // } ) ;
    //
    // var listExportVolumeToExcelInfosSorted =
    //   listExportVolumeToExcelInfos.OrderBy( x => x.StationString ).ToList() ;
    // // //Sort by Station Name
    // // for (int i = 0; i < listExportVolumeToExcelInfosSorted.Count; i++)
    // // {
    // //   listExportVolumeToExcelInfosSorted[i].Order = i + 1;
    // // }
    //
    // return listExportVolumeToExcelInfosSorted ;
    return new List<ExportWallToExcelInfo>() ;
  }
}