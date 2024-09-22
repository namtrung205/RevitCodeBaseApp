using System ;
using System.Collections ;
using System.Collections.Generic ;
using System.Collections.ObjectModel ;
using System.Diagnostics ;
using System.Linq ;
using System.Windows ;
using System.Windows.Controls ;
using System.Windows.Data ;
using System.Windows.Media ;
using Helpers.Utils ;
using static System.Windows.Controls.TextBlock ;

namespace Helpers.Utils_Wpf ;

public static class DataGridUtils
{
  public static void SetHeaderCheckBoxDataGrid ( DataGrid dataGrid,int indexColumnCheckBox)
  {
    try {
      if ( dataGrid.Columns[ indexColumnCheckBox ].Header is not CheckBox checkBoxHeader ) {
        Debug.Assert( false,"Header is not CheckBox, check again" );
        return ;
      }
      var itemSource = dataGrid.ItemsSource ;
      var enumerable = itemSource.Cast<object>().ToList() ;
      checkBoxHeader.IsChecked = enumerable.All( x => x.GetValuePropertyByNames( "IsSelect" ) != null && x.GetValuePropertyByNames( "IsSelect" ) is true) ? true :
        enumerable.All( x => x.GetValuePropertyByNames( "IsSelect" ) != null && x.GetValuePropertyByNames( "IsSelect" ) is false ) ? false : null ;
    }
    catch ( Exception e ) {
      LoggerCit.Instance.LogError( e );
    }
  }
  public static void SetBorderDataGridCell( DataGrid dataGrid, int row, int column, bool isError )
  {
    if ( dataGrid.ItemContainerGenerator.ContainerFromIndex( row ) is DataGridRow &&
         dataGrid.Columns[ column ].GetCellContent( row )?.Parent is DataGridCell dataGridCell ) {
      if ( isError ) {
        dataGridCell.BorderThickness = new Thickness( 1, 1, 1, 1 ) ;
        dataGridCell.BorderBrush = Brushes.Red ;
      }
      else {
        dataGridCell.BorderThickness = new Thickness( 0 ) ;
        dataGridCell.BorderBrush = new SolidColorBrush( Colors.Transparent ) ;
      }
    }
  }

  public static TextBox? GetTextBoxInDataGrid( DataGrid dataGrid, int row, int column )
  {
    if ( dataGrid.ItemContainerGenerator.ContainerFromIndex( row ) is DataGridRow dgRow
         && dataGrid.Columns[ column ].GetCellContent( dgRow )?.Parent is DataGridCell cell
         && FindVisualUtils.FindVisualChild<TextBox>( cell ) is { } textBox )
      return textBox ;
    return null ;
  }

  public static bool DataGridError( DataGrid dataGrid )
  {
    for ( var i = 0 ; i < dataGrid.Items.Count ; i++ )
    for ( var j = 0 ; j < dataGrid.Columns.Count ; j++ ) {
      var textBox = GetTextBoxInDataGrid( dataGrid, i, j ) ;
      if ( textBox == null ) continue ;
      if ( textBox.Background.ToString() == new SolidColorBrush( InputBehaviourTextBox.ColorError ).ToString() ) return true ;
    }

    return false ;
  }

  public static void AddRowAndFocusTextBox<T>( DataGrid dataGrid, ObservableCollection<T> listItemSources, T itemAdd, int positionColumn )
  {
    listItemSources.Add( itemAdd ) ;
    dataGrid.SelectedItem = itemAdd ;
    if ( dataGrid.SelectedItem != null ) dataGrid.ScrollIntoView( dataGrid.SelectedItem ) ;
    if ( GetTextBoxInDataGrid( dataGrid, dataGrid.SelectedIndex, positionColumn ) is { } textBox ) {
      textBox.Focus() ;
      textBox.Select( 0, textBox.Text.Length ) ;
    }
  }

  public static void AddRow<T>( DataGrid dataGrid, ObservableCollection<T> listItemSources, T itemAdd )
  {
    listItemSources.Add( itemAdd ) ;
    dataGrid.SelectedItem = itemAdd ;
    if ( dataGrid.SelectedItem != null ) dataGrid.ScrollIntoView( dataGrid.SelectedItem ) ;
  }

  public static void DeleteSingleRow<T>( DataGrid dataGrid, ObservableCollection<T> listItemSources )
  {
    if ( ! listItemSources.Any() ) return ;
    var index = dataGrid.SelectedIndex ;
    if ( index == -1 ) return ;
    listItemSources.RemoveAt( index ) ;
    if ( ! listItemSources.Any() ) return ;
    if ( index == 0 ) {
      dataGrid.SelectedIndex = index ;
      return ;
    }

    dataGrid.SelectedIndex = index - 1 ;
    dataGrid.ScrollIntoView( dataGrid.SelectedItem ) ;
  }

  public static void DeleteMultiRow<T>( DataGrid dataGrid, ObservableCollection<T> listItemSources )
  {
    if ( ! listItemSources.Any() ) return ;
    var selectedItems = dataGrid.SelectedItems.Cast<T>().ToList() ;
    if ( ! selectedItems.Any() ) return ;
    if ( selectedItems.Count == 1 ) {
      DeleteSingleRow( dataGrid, listItemSources ) ;
      return ;
    }

    foreach ( var itemDelete in selectedItems ) {
      var index = listItemSources.IndexOf( itemDelete ) ;
      if ( index == -1 ) continue ;
      listItemSources.RemoveAt( index ) ;
    }
  }

  public static void MoveSingleRowUp<T>( DataGrid dataGrid, ObservableCollection<T> listItemSources )
  {
    if ( ! listItemSources.Any() ) return ;
    var selectedItem = (T) dataGrid.SelectedItem ;
    if ( selectedItem == null ) return ;
    var index = listItemSources.IndexOf( selectedItem ) ;
    if ( index == -1 || index == 0 ) return ;

    listItemSources.RemoveAt( index ) ;
    listItemSources.Insert( index - 1, selectedItem ) ;
    dataGrid.SelectedItem = listItemSources[ index - 1 ] ;
  }

  public static void MoveSingleRowDown<T>( DataGrid dataGrid, ObservableCollection<T> listItemSources )
  {
    if ( ! listItemSources.Any() ) return ;
    var selectedItem = (T) dataGrid.SelectedItem ;
    if ( selectedItem == null ) return ;
    var index = listItemSources.IndexOf( selectedItem ) ;
    if ( index == -1 || index == listItemSources.Count - 1 ) return ;

    listItemSources.RemoveAt( index ) ;
    listItemSources.Insert( index + 1, selectedItem ) ;
    dataGrid.SelectedItem = listItemSources[ index + 1 ] ;
  }

  public static void CheckValueIsExitedInDataGrid<T>( DataGrid dataGrid, ObservableCollection<T> listItemSources, int columnCheck )
  {
    var selectedItem = (T) dataGrid.SelectedItem ;
    var selectedIndex = dataGrid.SelectedIndex ;
    if ( selectedItem == null ) return ;
    var textBoxInput = GetTextBoxInDataGrid( dataGrid, selectedIndex, columnCheck ) ;
    if ( textBoxInput == null ) return ;
    var stringInput = textBoxInput.Text.Trim() ;
    var isFailFormat = false ;
    if ( string.IsNullOrWhiteSpace( stringInput ) ) {
      isFailFormat = true ;
      textBoxInput.Background = new SolidColorBrush( Color.FromRgb( 255, 216, 255 ) ) ;
    }

    for ( var i = 0 ; i < listItemSources.Count ; i++ ) {
      if ( i == selectedIndex ) continue ;
      var textBox = GetTextBoxInDataGrid( dataGrid, i, columnCheck ) ;
      if ( textBox == null ) continue ;
      if ( string.IsNullOrWhiteSpace( textBox.Text ) ) {
        textBoxInput.Background = new SolidColorBrush( Color.FromRgb( 255, 216, 255 ) ) ;
        continue ;
      }

      if ( textBox.Text.Trim() == stringInput ) {
        textBox.Background = new SolidColorBrush( Color.FromRgb( 255, 216, 255 ) ) ;
        textBoxInput.Background = new SolidColorBrush( Color.FromRgb( 255, 216, 255 ) ) ;
        isFailFormat = true ;
        continue ;
      }

      textBox.Background = new SolidColorBrush( Colors.Transparent ) ;
    }

    if ( ! isFailFormat ) textBoxInput.Background = new SolidColorBrush( Colors.Transparent ) ;
  }

  public static void InitColumnDataGrid( DataGrid dataGrid, List<string> listNameHeader, string columnHeaderStyle, string cellStyle, double minWidth )
  {
    dataGrid.Columns.Clear() ;
    var styleHeader = dataGrid.FindResource( columnHeaderStyle ) as Style ;
    var styleCell = dataGrid.FindResource( cellStyle ) as Style ;

    foreach ( var header in listNameHeader ) {
      DataGridTemplateColumn column = new()
      {
        Header = header,
        MinWidth = minWidth,
        Width = DataGridLength.SizeToCells,
        HeaderStyle = styleHeader,
        CellStyle = styleCell
      } ;
      dataGrid.Columns.Add( column ) ;
    }
  }

  public static HierarchicalDataTemplate GetTemplate()
  {
    //create the data template
    var dataTemplate = new HierarchicalDataTemplate() ;

    //create stack pane;
    var stackPanel = new FrameworkElementFactory( typeof( StackPanel ) ) ;
    stackPanel.Name = "parentStackpanel" ;
    stackPanel.SetValue( StackPanel.OrientationProperty, Orientation.Horizontal ) ;

    ////// Create check box
    var checkBox = new FrameworkElementFactory( typeof( CheckBox ) ) { Name = "chk" } ;
    checkBox.SetValue( FrameworkElement.NameProperty, "chk" ) ;
    checkBox.SetValue( FrameworkElement.TagProperty, new Binding() ) ;
    checkBox.SetValue( FrameworkElement.MarginProperty, new Thickness( 2 ) ) ;
    checkBox.SetValue( FrameworkElement.TagProperty, new Binding { Path = new PropertyPath( "Name" ) } ) ;
    stackPanel.AppendChild( checkBox ) ;


    // create text
    var label = new FrameworkElementFactory( typeof( TextBlock ) ) ;
    label.SetBinding( TextProperty, new Binding { Path = new PropertyPath( "Name" ) } ) ;
    label.SetValue( FrameworkElement.MarginProperty, new Thickness( 2 ) ) ;
    label.SetValue( FontWeightProperty, FontWeights.Bold ) ;
    label.SetValue( FrameworkElement.ToolTipProperty, new Binding() ) ;

    stackPanel.AppendChild( label ) ;

    dataTemplate.ItemsSource = new Binding( "Elements" ) ;

    //set the visual tree of the data template
    dataTemplate.VisualTree = stackPanel ;

    return dataTemplate ;
  }
}