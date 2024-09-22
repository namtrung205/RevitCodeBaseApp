using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Windows ;
using System.Windows.Controls ;
using System.Windows.Controls.Primitives ;
using System.Windows.Input ;
using System.Windows.Media ;

namespace Helpers.Utils_Wpf ;

public static class InputBehaviourComboboxEdit
{
  public static readonly DependencyProperty ModeProperty = DependencyProperty.RegisterAttached( "Mode", typeof( InputBehaviourModesComboboxEdit? ),
    typeof( InputBehaviourComboboxEdit ), new UIPropertyMetadata( null, OnValueChanged ) ) ;

  private static readonly List<string> ListHandleAutocad =
  [
    @"\",
    "/",
    "'",
    "*",
    ":",
    "|",
    ";",
    "<",
    ">",
    "?",
    "`",
    ",",
    "=",
    "\""
  ] ;

  private static Brush? ColorInit { get ; set ; }
  private static Color ColorError { get ; } = Color.FromRgb( 255, 216, 255 ) ;

  private static InputBehaviourModesComboboxEdit? GetMode( DependencyObject o )
  {
    return (InputBehaviourModesComboboxEdit?) o.GetValue( ModeProperty ) ;
  }

  public static void SetMode( DependencyObject o, InputBehaviourModesComboboxEdit? value )
  {
    o.SetValue( ModeProperty, value ) ;
  }

  private static void OnValueChanged( DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e )
  {
    var uiElement = dependencyObject as ComboBox ;

    if ( uiElement == null ) return ;
    if ( ! uiElement.IsEditable ) return ;
    if ( e.NewValue is InputBehaviourModesComboboxEdit ) {
      uiElement.PreviewTextInput += OnTextInput ;
      uiElement.PreviewKeyDown += OnPreviewKeyDown ;
      uiElement.AddHandler( TextBoxBase.TextChangedEvent,
        new TextChangedEventHandler( UiElementTextChanged ) ) ;
      DataObject.AddPastingHandler( uiElement, OnPaste ) ;
      uiElement.LostFocus += UiElementLostFocus ;
    }
    else {
      uiElement.PreviewTextInput -= OnTextInput ;
      uiElement.PreviewKeyDown -= OnPreviewKeyDown ;
      uiElement.RemoveHandler( TextBoxBase.TextChangedEvent,
        new TextChangedEventHandler( UiElementTextChanged ) ) ;
      DataObject.RemovePastingHandler( uiElement, OnPaste ) ;
      uiElement.LostFocus -= UiElementLostFocus ;
    }
  }

  private static void UiElementLostFocus( object sender, RoutedEventArgs e )
  {
    if ( sender is not ComboBox comboBox ) return ;
    comboBox.Text = comboBox.Text.Trim() ;
    if ( sender is not DependencyObject dependencyObject ) return ;

    var nameProperty = comboBox.GetBindingExpression( ComboBox.TextProperty )?.ParentBinding.Path.Path ;
    if ( nameProperty == null ) return ;
    var isNullAble = comboBox.DataContext.IsChildNullAble( nameProperty ) ;
    var mode = GetMode( dependencyObject ) ;
    if ( isNullAble == false && ( mode == InputBehaviourModesComboboxEdit.Decimal || mode == InputBehaviourModesComboboxEdit.PositiveDecimal ) )
      if ( string.IsNullOrWhiteSpace( comboBox.Text ) ) {
        comboBox.Text = "0" ;
        return ;
      }

    if ( isNullAble == false && ( mode == InputBehaviourModesComboboxEdit.WholeNumber || mode == InputBehaviourModesComboboxEdit.PositiveWholeNumber ) )
      if ( string.IsNullOrWhiteSpace( comboBox.Text ) )
        comboBox.Text = "0" ;
  }

  private static void UiElementTextChanged( object sender, RoutedEventArgs e )
  {
    if ( sender is not ComboBox combobox ) return ;
    var textBox = combobox.Template.FindName( "PART_EditableTextBox", combobox ) as TextBox ;
    var toggleButton = combobox.Template.FindName( "toggleButton", combobox ) as ToggleButton ;
    if ( textBox == null || toggleButton == null ) return ;
    ColorInit ??= textBox.Background ;

    if ( sender is not DependencyObject dependencyObject ) return ;

    var mode = GetMode( dependencyObject ) ;

    if ( mode == InputBehaviourModesComboboxEdit.TextNoEmpty ) {
      if ( string.IsNullOrWhiteSpace( combobox.Text ) ) {
        textBox.Background = new SolidColorBrush( ColorError ) ;
        toggleButton.Background = new SolidColorBrush( ColorError ) ;
        return ;
      }

      var parent = GetDataGridCell( combobox ) ;
      combobox.Background = parent == null ? ColorInit : new SolidColorBrush( Colors.Transparent ) ;
    }

    if ( mode == InputBehaviourModesComboboxEdit.PositiveWholeNumberNoZeroNoEmpty || mode == InputBehaviourModesComboboxEdit.WholeNumberNoZeroNoEmpty ) {
      if ( string.IsNullOrWhiteSpace( combobox.Text ) || combobox.Text == "-" ) {
        textBox.Background = new SolidColorBrush( ColorError ) ;
        toggleButton.Background = new SolidColorBrush( ColorError ) ;
        return ;
      }

      int.TryParse( combobox.Text, out var intValue ) ;
      var parent = GetDataGridCell( combobox ) ;
      if ( intValue == 0 ) {
        textBox.Background = new SolidColorBrush( ColorError ) ;
        toggleButton.Background = new SolidColorBrush( ColorError ) ;
        return ;
      }

      if ( parent == null ) {
        textBox.Background = ColorInit ;
        toggleButton.Background = ColorInit ;
        return ;
      }

      textBox.Background = new SolidColorBrush( Colors.Transparent ) ;
      toggleButton.Background = new SolidColorBrush( Colors.Transparent ) ;
    }

    if ( mode == InputBehaviourModesComboboxEdit.WholeNumberNoEmpty || mode == InputBehaviourModesComboboxEdit.PositiveWholeNumberNoEmpty ) {
      if ( string.IsNullOrWhiteSpace( combobox.Text ) || combobox.Text == "-" ) {
        combobox.Background = new SolidColorBrush( ColorError ) ;
        textBox.Background = new SolidColorBrush( ColorError ) ;
        toggleButton.Background = new SolidColorBrush( ColorError ) ;
        return ;
      }

      var parent = GetDataGridCell( combobox ) ;
      if ( parent == null ) {
        textBox.Background = ColorInit ;
        toggleButton.Background = ColorInit ;
        return ;
      }

      textBox.Background = new SolidColorBrush( Colors.Transparent ) ;
      toggleButton.Background = new SolidColorBrush( Colors.Transparent ) ;
    }

    if ( mode == InputBehaviourModesComboboxEdit.DecimalNoEmpty || mode == InputBehaviourModesComboboxEdit.PositiveDecimalNoEmpty ) {
      if ( string.IsNullOrWhiteSpace( combobox.Text ) || combobox.Text == "-" ) {
        combobox.Background = new SolidColorBrush( ColorError ) ;
        return ;
      }

      var parent = GetDataGridCell( combobox ) ;
      if ( parent == null ) {
        textBox.Background = ColorInit ;
        toggleButton.Background = ColorInit ;
        return ;
      }

      textBox.Background = new SolidColorBrush( Colors.Transparent ) ;
      toggleButton.Background = new SolidColorBrush( Colors.Transparent ) ;
    }

    if ( mode == InputBehaviourModesComboboxEdit.PositiveDecimalNoZeroNoEmpty || mode == InputBehaviourModesComboboxEdit.DecimalNoZeroNoEmpty ) {
      if ( string.IsNullOrWhiteSpace( combobox.Text ) || combobox.Text == "-" ) {
        combobox.Background = new SolidColorBrush( ColorError ) ;
        return ;
      }

      double.TryParse( combobox.Text, out var doubleValue ) ;
      var parent = GetDataGridCell( combobox ) ;
      if ( doubleValue == 0 ) {
        textBox.Background = new SolidColorBrush( ColorError ) ;
        toggleButton.Background = new SolidColorBrush( ColorError ) ;
        return ;
      }

      if ( parent == null ) {
        textBox.Background = ColorInit ;
        toggleButton.Background = ColorInit ;
        return ;
      }

      textBox.Background = new SolidColorBrush( Colors.Transparent ) ;
      toggleButton.Background = new SolidColorBrush( Colors.Transparent ) ;
    }
  }

  private static void OnTextInput( object sender, TextCompositionEventArgs e )
  {
    var combobox = sender as ComboBox ;
    if ( combobox == null ) return ;
    var txtBox = combobox.Template.FindName( "PART_EditableTextBox", combobox ) as TextBox ;

    if ( sender is DependencyObject dependencyObject ) {
      var mode = txtBox == null ? InputBehaviourModesComboboxEdit.PositiveWholeNumber : GetMode( dependencyObject ) ;

      if ( mode == InputBehaviourModesComboboxEdit.Decimal
           || mode == InputBehaviourModesComboboxEdit.DecimalNoZeroNoEmpty
           || mode == InputBehaviourModesComboboxEdit.DecimalNoEmpty ) {
        if ( txtBox == null ) return ;
        var selectionStart = txtBox.SelectionStart ;
        var brushSelectDecimal = txtBox.SelectionLength ;
        if ( brushSelectDecimal > 1 ) {
          var selectionStartNew = txtBox.SelectionStart + brushSelectDecimal ;
          selectionStart = selectionStartNew > txtBox.Text.Length ? txtBox.Text.Length : selectionStartNew ;
        }

        var textNew = txtBox.Text ;
        foreach ( var c in e.Text ) {
          if ( ! char.IsDigit( c ) && c != '-' && c != '.' ) {
            e.Handled = true ;
            break ;
          }

          if ( textNew != null && textNew.Contains( "-" ) && c == '-' && brushSelectDecimal != textNew.Length ) {
            e.Handled = true ;
            break ;
          }

          if ( textNew != null && selectionStart > textNew.Length ) return ;
          if ( double.TryParse( textNew?.Insert( selectionStart, c.ToString() ), out var _ ) == false ) {
            if ( brushSelectDecimal == textNew?.Length ) break ;
            if ( ( textNew != "" && c != '-' ) || ( textNew != "" && selectionStart != 0 && c == '-' ) || ( textNew == "" && c == '.' ) ) {
              e.Handled = true ;
              break ;
            }
          }
        }

        HandleDecimalPoint() ;
      }

      if ( mode == InputBehaviourModesComboboxEdit.PositiveDecimal
           || mode == InputBehaviourModesComboboxEdit.PositiveDecimalNoZeroNoEmpty
           || mode == InputBehaviourModesComboboxEdit.PositiveDecimalNoEmpty ) {
        if ( txtBox == null ) return ;
        var selectionStartPositiveDecimal = txtBox.SelectionStart ;
        var brushSelectPositiveDecimal = txtBox.SelectionLength ;
        if ( brushSelectPositiveDecimal > 1 ) {
          var selectionStartNew = txtBox.SelectionStart + brushSelectPositiveDecimal ;
          selectionStartPositiveDecimal = selectionStartNew > txtBox.Text.Length ? txtBox.Text.Length : selectionStartNew ;
        }

        var textNewPositiveDecimal = txtBox.Text ;
        foreach ( var c in e.Text ) {
          if ( ! char.IsDigit( c ) && c != '.' ) {
            e.Handled = true ;
            break ;
          }

          if ( textNewPositiveDecimal == "" && c == '.' ) {
            e.Handled = true ;
            break ;
          }

          if ( textNewPositiveDecimal != null && selectionStartPositiveDecimal > textNewPositiveDecimal.Length ) return ;
          if ( double.TryParse( textNewPositiveDecimal?.Insert( selectionStartPositiveDecimal, c.ToString() ), out var _ ) == false ) {
            e.Handled = true ;
            break ;
          }
        }

        HandleDecimalPoint() ;
      }

      if ( mode == InputBehaviourModesComboboxEdit.PositiveWholeNumber
           || mode == InputBehaviourModesComboboxEdit.PositiveWholeNumberNoZeroNoEmpty
           || mode == InputBehaviourModesComboboxEdit.PositiveWholeNumberNoEmpty )
        if ( e.Text.Any( c => ! char.IsDigit( c ) ) )
          e.Handled = true ;

      if ( mode == InputBehaviourModesComboboxEdit.WholeNumber
           || mode == InputBehaviourModesComboboxEdit.WholeNumberNoZeroNoEmpty
           || mode == InputBehaviourModesComboboxEdit.WholeNumberNoEmpty ) {
        if ( txtBox == null ) return ;
        var tbChanged2 = (ComboBox) e.Source ;
        var selectionStartWholeNumber = txtBox.SelectionStart ;
        var brushSelectWholeNumber = txtBox.SelectionLength ;
        if ( brushSelectWholeNumber > 1 ) {
          var selectionStartNew = txtBox.SelectionStart + brushSelectWholeNumber ;
          selectionStartWholeNumber = selectionStartNew > txtBox.Text.Length ? txtBox.Text.Length : selectionStartNew ;
        }

        var textNew2 = tbChanged2.Text ;
        foreach ( var c in e.Text ) {
          if ( ! char.IsDigit( c ) && c != '-' ) {
            e.Handled = true ;
            break ;
          }

          if ( textNew2 != null && textNew2.Contains( "-" ) && c == '-' && brushSelectWholeNumber != textNew2.Length ) {
            e.Handled = true ;
            break ;
          }

          if ( textNew2 != null && selectionStartWholeNumber > textNew2.Length ) return ;
          if ( double.TryParse( textNew2?.Insert( selectionStartWholeNumber, c.ToString() ), out var _ ) == false ) {
            if ( brushSelectWholeNumber == textNew2?.Length ) break ;
            if ( ( textNew2 != "" && c != '-' ) || ( textNew2 != "" && selectionStartWholeNumber != 0 && c == '-' ) ) {
              e.Handled = true ;
              break ;
            }
          }
        }
      }

      if ( mode == InputBehaviourModesComboboxEdit.TextHandleAutocad )
        foreach ( var c in e.Text )
          if ( ListHandleAutocad.Any( x => x == c.ToString() ) ) {
            e.Handled = true ;
            break ;
          }
    }

    void HandleDecimalPoint()
    {
      if ( string.IsNullOrEmpty( e.Text ) ) return ;
      if ( e.Text[ 0 ] == '.' ) {
        var nonSelectedTest = GetNonSelectedTest( txtBox ) ;
        e.Handled = nonSelectedTest.Contains( "." ) ;
      }
    }
  }

  private static string GetNonSelectedTest( TextBox txtBox )
  {
    var startText = txtBox.SelectionStart == 0 ? string.Empty : txtBox.Text.Substring( 0, txtBox.SelectionStart ) ;
    var endText = txtBox.SelectionStart + txtBox.SelectionLength == txtBox.Text.Length
      ? string.Empty
      : txtBox.Text.Substring( txtBox.SelectionStart + txtBox.SelectionLength ) ;
    return startText + endText ;
  }

  private static void OnPreviewKeyDown( object sender, KeyEventArgs e )
  {
    if ( e.Key == Key.Space ) e.Handled = true ;
  }

  private static void OnPaste( object sender, DataObjectPastingEventArgs e )
  {
    if ( sender is not DependencyObject dependencyObject ) return ;
    var mode = GetMode( dependencyObject ) ;
    if ( e.DataObject.GetDataPresent( DataFormats.Text ) ) {
      var text = Convert.ToString( e.DataObject.GetData( DataFormats.Text ) ).Trim() ;
      if ( mode == InputBehaviourModesComboboxEdit.Text ) return ;
      if ( mode == InputBehaviourModesComboboxEdit.TextHandleAutocad ) {
        if ( text.Any( c => ListHandleAutocad.Any( x => x == c.ToString() ) ) ) e.CancelCommand() ;
        return ;
      }

      if ( text.Any( c => ! char.IsDigit( c ) ) ) e.CancelCommand() ;
    }
    else {
      e.CancelCommand() ;
    }
  }

  private static DataGridCell? GetDataGridCell( DependencyObject dependencyObject )
  {
    var parent = VisualTreeHelper.GetParent( dependencyObject ) ;
    while ( parent is not DataGridCell )
      if ( parent != null ) {
        parent = VisualTreeHelper.GetParent( parent ) ;
        if ( parent == null ) break ;
      }

    return parent as DataGridCell ;
  }

  private static bool? IsChildNullAble( this object? obj, string nameProperty )
  {
    if ( obj == null ) return null ;
    var listNameProperty = nameProperty.Split( '.' ) ;
    if ( listNameProperty.Length == 1 ) {
      foreach ( var propertyInfo in obj.GetType().GetProperties() )
        if ( propertyInfo.Name == nameProperty ) {
          if ( Nullable.GetUnderlyingType( propertyInfo.PropertyType ) != null ) return true ;
          if ( propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof( Nullable<> ) ) return true ;
          return false ;
        }

      return null ;
    }

    var valueProp = obj.GetType().GetProperty( listNameProperty[ 0 ] )?.GetValue( obj ) ;
    for ( var i = 1 ; i < listNameProperty.Length ; i++ ) {
      if ( i == listNameProperty.Length - 1 )
        if ( valueProp != null ) {
          foreach ( var propertyInfo in valueProp.GetType().GetProperties() )
            if ( propertyInfo.Name == listNameProperty[ listNameProperty.Length - 1 ] ) {
              if ( Nullable.GetUnderlyingType( propertyInfo.PropertyType ) != null ) return true ;
              if ( propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof( Nullable<> ) ) return true ;
            }

          return false ;
        }

      valueProp = valueProp?.GetType().GetProperty( listNameProperty[ i ] ) ;
    }

    return null ;
  }
}

public enum InputBehaviourModesComboboxEdit
{
  Text,
  TextNoEmpty,
  TextHandleAutocad,

  WholeNumber,
  WholeNumberNoZeroNoEmpty,
  WholeNumberNoEmpty,

  PositiveWholeNumber,
  PositiveWholeNumberNoZeroNoEmpty,
  PositiveWholeNumberNoEmpty,

  Decimal,
  DecimalNoZeroNoEmpty,
  DecimalNoEmpty,

  PositiveDecimal,
  PositiveDecimalNoZeroNoEmpty,
  PositiveDecimalNoEmpty
}