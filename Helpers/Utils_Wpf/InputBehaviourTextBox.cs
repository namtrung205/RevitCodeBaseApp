using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Reflection ;
using System.Windows ;
using System.Windows.Controls ;
using System.Windows.Data ;
using System.Windows.Input ;
using System.Windows.Media ;
using Helpers.Utils ;

namespace Helpers.Utils_Wpf ;

public static class InputBehaviourTextBox
{
  public static readonly DependencyProperty ModeProperty = DependencyProperty.RegisterAttached( "Mode", typeof( InputBehaviourModesTextBox? ),
    typeof( InputBehaviourTextBox ), new UIPropertyMetadata( null, OnValueChanged ) ) ;

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

  private static Brush? ColorInit { get ; } = Brushes.White ;
  private static BindingMode BindingModeSave { get ; set ; } = BindingMode.Default ;
  private static string? StringFormatSave { get ; set ; }
  private static UpdateSourceTrigger UpdateSourceTriggerSave { get ; set ; } = UpdateSourceTrigger.Default ;
  public static Color ColorError { get ; set ; } = Color.FromRgb( 255, 216, 255 ) ;

  public static InputBehaviourModesTextBox? GetMode( DependencyObject o )
  {
    return (InputBehaviourModesTextBox?) o.GetValue( ModeProperty ) ;
  }

  public static void SetMode( DependencyObject o, InputBehaviourModesTextBox? value )
  {
    o.SetValue( ModeProperty, value ) ;
  }

  private static void OnValueChanged( DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e )
  {
    var uiElement = dependencyObject as TextBox ;
    if ( uiElement == null ) return ;
    if ( e.NewValue is InputBehaviourModesTextBox ) {
      uiElement.PreviewTextInput += OnTextInput ;
      uiElement.PreviewKeyDown += OnPreviewKeyDown ;
      uiElement.TextChanged += UiElementTextChanged ;
      uiElement.GotFocus += UiElementOnGotFocus ;
      uiElement.LostFocus += UiElementLostFocus ;
      DataObject.AddPastingHandler( uiElement, OnPaste ) ;
    }
    else {
      uiElement.GotFocus += UiElementOnGotFocus ;
      uiElement.PreviewTextInput -= OnTextInput ;
      uiElement.PreviewKeyDown -= OnPreviewKeyDown ;
      uiElement.TextChanged -= UiElementTextChanged ;
      uiElement.LostFocus -= UiElementLostFocus ;
      DataObject.RemovePastingHandler( uiElement, OnPaste ) ;
    }
  }

  private static void UiElementOnGotFocus( object sender, RoutedEventArgs e )
  {
    if ( sender is not TextBox textBox ) return ;
    if ( sender is not DependencyObject dependencyObject ) return ;
    var mode = GetMode( dependencyObject ) ;

    var nameProperty = textBox.GetBindingExpression( TextBox.TextProperty )?.ParentBinding.Path.Path ;
    if ( nameProperty == null ) return ;

    if ( textBox.GetBindingExpression( TextBox.TextProperty ) != null ) {
      BindingModeSave = textBox.GetBindingExpression( TextBox.TextProperty )!.ParentBinding.Mode ;
      UpdateSourceTriggerSave = textBox.GetBindingExpression( TextBox.TextProperty )!.ParentBinding.UpdateSourceTrigger ;
      StringFormatSave = textBox.GetBindingExpression( TextBox.TextProperty )!.ParentBinding.StringFormat ;
    }

    if ( mode == InputBehaviourModesTextBox.Decimal
         || mode == InputBehaviourModesTextBox.DecimalNoEmpty
         || mode == InputBehaviourModesTextBox.DecimalNoZeroNoEmpty
         || mode == InputBehaviourModesTextBox.PositiveDecimal
         || mode == InputBehaviourModesTextBox.PositiveDecimalNoEmpty
         || mode == InputBehaviourModesTextBox.PositiveDecimalNoZeroNoEmpty )
      textBox.SetBinding( TextBox.TextProperty,
        new Binding { Path = new PropertyPath( nameProperty ), Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.LostFocus } ) ;
  }

  private static void UiElementLostFocus( object sender, RoutedEventArgs e )
  {
    if ( sender is not TextBox textBox ) return ;
    textBox.Text = textBox.Text.Trim() ;
    if ( sender is not DependencyObject dependencyObject ) return ;
    var mode = GetMode( dependencyObject ) ;
    var nameProperty = textBox.GetBindingExpression( TextBox.TextProperty )?.ParentBinding.Path.Path ;
    if ( nameProperty == null ) return ;
    if ( mode == InputBehaviourModesTextBox.Decimal
         || mode == InputBehaviourModesTextBox.DecimalNoEmpty
         || mode == InputBehaviourModesTextBox.DecimalNoZeroNoEmpty
         || mode == InputBehaviourModesTextBox.PositiveDecimal
         || mode == InputBehaviourModesTextBox.PositiveDecimalNoEmpty
         || mode == InputBehaviourModesTextBox.PositiveDecimalNoZeroNoEmpty )
      textBox.SetBinding( TextBox.TextProperty,
        new Binding
        {
          Path = new PropertyPath( nameProperty ), Mode = BindingModeSave, UpdateSourceTrigger = UpdateSourceTriggerSave, StringFormat = StringFormatSave
        } ) ;
  }

  private static void UiElementTextChanged( object sender, RoutedEventArgs e )
  {
    if ( sender is not TextBox tb ) return ;
    if ( sender is not DependencyObject dependencyObject ) return ;
    var textNew = tb.Text ;
    var selectionStart = tb.SelectionStart ;
    IgnoreTextChangedEvent( tb, textBox =>
    {
      var mode = GetMode( dependencyObject ) ;
      var nameProperty = textBox.GetBindingExpression( TextBox.TextProperty )?.ParentBinding.Path.Path ;
      if ( nameProperty == null ) return ;

      object? valueTextBox = null ;
      var valueOld = textBox.DataContext.GetValueObjectPropertyByListName( nameProperty ) ;
      if ( mode == InputBehaviourModesTextBox.Decimal
           || mode == InputBehaviourModesTextBox.DecimalNoEmpty
           || mode == InputBehaviourModesTextBox.DecimalNoZeroNoEmpty
           || mode == InputBehaviourModesTextBox.PositiveDecimal
           || mode == InputBehaviourModesTextBox.PositiveDecimalNoEmpty
           || mode == InputBehaviourModesTextBox.PositiveDecimalNoZeroNoEmpty ) {
        valueTextBox = CanConvertToDouble( textBox.Text ) ;

        if ( textBox.DataContext.IsChildNullAble( nameProperty ) == true ) {
          if ( $"{valueTextBox:0.000000}" != $"{valueOld:0.000000}" ) {
            textBox.DataContext.SetValuePropertyByNames( nameProperty, valueTextBox ) ;
            textBox.Text = textNew ;
            textBox.SelectionStart = selectionStart ;
          }
        }
        else {
          if ( valueTextBox == null ) {
            textBox.DataContext.SetValuePropertyByNames( nameProperty, 0 ) ;
            textBox.Text = textNew ;
            textBox.SelectionStart = selectionStart ;
          }

          if ( $"{valueTextBox:0.000000}" != $"{valueOld:0.000000}" && valueTextBox != null ) {
            textBox.DataContext.SetValuePropertyByNames( nameProperty, valueTextBox ) ;
            textBox.Text = textNew ;
            textBox.SelectionStart = selectionStart ;
          }
        }
      }

      if ( mode == InputBehaviourModesTextBox.WholeNumber
           || mode == InputBehaviourModesTextBox.WholeNumberNoEmpty
           || mode == InputBehaviourModesTextBox.WholeNumberNoZeroNoEmpty
           || mode == InputBehaviourModesTextBox.PositiveWholeNumber
           || mode == InputBehaviourModesTextBox.PositiveWholeNumberNoEmpty
           || mode == InputBehaviourModesTextBox.PositiveDecimalNoZeroNoEmpty ) {
        valueTextBox = CanConvertToInt( textBox.Text ) ;
        if ( textBox.DataContext.IsChildNullAble( nameProperty ) == true ) {
          if ( valueTextBox != valueOld ) {
            textBox.DataContext.SetValuePropertyByNames( nameProperty, valueTextBox ) ;
            textBox.Text = textNew ;
            textBox.SelectionStart = selectionStart ;
          }
        }
        else {
          if ( valueTextBox == null ) {
            textBox.DataContext.SetValuePropertyByNames( nameProperty, 0 ) ;
            textBox.Text = textNew ;
            textBox.SelectionStart = selectionStart ;
          }
          else {
            if ( valueTextBox != valueOld ) {
              textBox.DataContext.SetValuePropertyByNames( nameProperty, valueTextBox ) ;
              textBox.Text = textNew ;
              textBox.SelectionStart = selectionStart ;
            }
          }
        }
      }

      if ( mode == InputBehaviourModesTextBox.TextNoEmpty ) {
        if ( string.IsNullOrWhiteSpace( textBox.Text ) ) {
          textBox.Background = new SolidColorBrush( ColorError ) ;
          return ;
        }

        var parent = GetDataGridCell( textBox ) ;
        textBox.Background = parent == null ? ColorInit : new SolidColorBrush( Colors.Transparent ) ;
      }

      if ( mode == InputBehaviourModesTextBox.Decimal || mode == InputBehaviourModesTextBox.WholeNumber ) {
        if ( CheckTextInvalid( textBox.Text ) ) {
          textBox.Background = new SolidColorBrush( ColorError ) ;
          return ;
        }

        var parent = GetDataGridCell( textBox ) ;
        textBox.Background = parent == null ? ColorInit : new SolidColorBrush( Colors.Transparent ) ;
        return ;
      }

      if ( mode == InputBehaviourModesTextBox.PositiveWholeNumberNoZeroNoEmpty || mode == InputBehaviourModesTextBox.WholeNumberNoZeroNoEmpty ) {
        if ( valueTextBox == null || valueTextBox.CanConvertToInt() == 0 || CheckTextInvalid( textBox.Text ) ) {
          textBox.Background = new SolidColorBrush( ColorError ) ;
          return ;
        }

        var parent = GetDataGridCell( textBox ) ;
        textBox.Background = parent == null ? ColorInit : new SolidColorBrush( Colors.Transparent ) ;
        return ;
      }

      if ( mode == InputBehaviourModesTextBox.WholeNumberNoEmpty || mode == InputBehaviourModesTextBox.PositiveWholeNumberNoEmpty ) {
        if ( string.IsNullOrWhiteSpace( textBox.Text ) || textBox.Text == "-" || CheckTextInvalid( textBox.Text ) ) {
          textBox.Background = new SolidColorBrush( ColorError ) ;
          return ;
        }

        var parent = GetDataGridCell( textBox ) ;
        textBox.Background = parent == null ? ColorInit : new SolidColorBrush( Colors.Transparent ) ;
      }

      if ( mode == InputBehaviourModesTextBox.DecimalNoEmpty || mode == InputBehaviourModesTextBox.PositiveDecimalNoEmpty ) {
        if ( string.IsNullOrWhiteSpace( textBox.Text ) || textBox.Text == "-" || CheckTextInvalid( textBox.Text ) ) {
          textBox.Background = new SolidColorBrush( ColorError ) ;
          return ;
        }

        var parent = GetDataGridCell( textBox ) ;
        textBox.Background = parent == null ? ColorInit : new SolidColorBrush( Colors.Transparent ) ;
      }

      if ( mode == InputBehaviourModesTextBox.PositiveDecimalNoZeroNoEmpty || mode == InputBehaviourModesTextBox.DecimalNoZeroNoEmpty ) {
        if ( valueTextBox == null || valueTextBox.CanConvertToDouble() == 0 || CheckTextInvalid( textBox.Text ) ) {
          textBox.Background = new SolidColorBrush( ColorError ) ;
          return ;
        }

        var parent = GetDataGridCell( textBox ) ;
        textBox.Background = parent == null ? ColorInit : new SolidColorBrush( Colors.Transparent ) ;
      }
    } ) ;
  }

  private static void IgnoreTextChangedEvent( TextBox textBox, Action<TextBox> action )
  {
    textBox.TextChanged -= UiElementTextChanged ;
    action.Invoke( textBox ) ;
    textBox.TextChanged += UiElementTextChanged ;
  }

  private static void OnTextInput( object sender, TextCompositionEventArgs e )
  {
    var txtBox = sender as TextBox ;
    if ( sender is DependencyObject dependencyObject ) {
      var mode = txtBox == null ? InputBehaviourModesTextBox.PositiveWholeNumber : GetMode( dependencyObject ) ;
      if ( txtBox == null ) return ;
      var selectionStart = txtBox.SelectionStart ;
      var brushSelectLength = txtBox.SelectionLength ;
      var textSelection = txtBox.SelectedText ;
      if ( brushSelectLength > 1 ) {
        var selectionStartNew = txtBox.SelectionStart + brushSelectLength ;
        selectionStart = selectionStartNew > txtBox.Text.Length ? txtBox.Text.Length : selectionStartNew ;
      }

      var textNew = txtBox.Text ;

      if ( mode.ToString().Contains( "Decimal" ) )
        if ( ( e.Text == "." && ! txtBox.Text.Contains( "." ) && txtBox.Text != "-" && brushSelectLength != 0 &&
               ! textSelection.Equals( textNew.Substring( 0, textSelection.Length ) ) && ! string.IsNullOrEmpty( textSelection ) )
             || ( e.Text == "." && ! txtBox.Text.Contains( "." ) && txtBox.Text != "-" && brushSelectLength == 0 && selectionStart != 0 ) )
          txtBox.SelectionStart = txtBox.SelectionStart == 0 && txtBox.Text.Length != 0 ? txtBox.Text.Length - 1 : txtBox.SelectionStart ;
      if ( mode == InputBehaviourModesTextBox.Decimal
           || mode == InputBehaviourModesTextBox.DecimalNoZeroNoEmpty
           || mode == InputBehaviourModesTextBox.DecimalNoEmpty ) {
        foreach ( var c in e.Text ) {
          if ( ! char.IsDigit( c ) && c != '-' && c != '.' ) {
            e.Handled = true ;
            break ;
          }

          if ( textNew != null && textNew.Contains( "-" ) && c == '-' && textSelection.Equals( textNew.Substring( 0, textSelection.Length ) ) &&
               string.IsNullOrEmpty( textSelection ) ) {
            e.Handled = true ;
            break ;
          }

          if ( textNew != null && selectionStart > textNew.Length ) return ;
          if ( double.TryParse( textNew?.Insert( selectionStart, c.ToString() ), out var _ ) == false ) {
            if ( textSelection.Equals( textNew?.Substring( 0, textSelection.Length ) ) && ! string.IsNullOrEmpty( textSelection ) ) break ;
            if ( ( textNew != "" && c != '-' ) || ( textNew != "" && selectionStart != 0 && c == '-' ) || ( textNew == "" && c == '.' ) ) {
              e.Handled = true ;
              break ;
            }
          }
        }

        HandleDecimalPoint() ;
      }

      if ( mode == InputBehaviourModesTextBox.PositiveDecimal
           || mode == InputBehaviourModesTextBox.PositiveDecimalNoZeroNoEmpty
           || mode == InputBehaviourModesTextBox.PositiveDecimalNoEmpty ) {
        foreach ( var c in e.Text ) {
          if ( ! char.IsDigit( c ) && c != '.' ) {
            e.Handled = true ;
            break ;
          }

          if ( textNew == "" && c == '.' ) {
            e.Handled = true ;
            break ;
          }

          if ( textNew != null && selectionStart > textNew.Length ) return ;
          if ( double.TryParse( textNew?.Insert( selectionStart, c.ToString() ), out var _ ) == false ) {
            e.Handled = true ;
            break ;
          }
        }

        HandleDecimalPoint() ;
      }

      if ( mode == InputBehaviourModesTextBox.PositiveWholeNumber
           || mode == InputBehaviourModesTextBox.PositiveWholeNumberNoZeroNoEmpty
           || mode == InputBehaviourModesTextBox.PositiveWholeNumberNoEmpty )
        if ( e.Text.Any( c => ! char.IsDigit( c ) ) )
          e.Handled = true ;

      if ( mode == InputBehaviourModesTextBox.WholeNumber
           || mode == InputBehaviourModesTextBox.WholeNumberNoZeroNoEmpty
           || mode == InputBehaviourModesTextBox.WholeNumberNoEmpty )
        foreach ( var c in e.Text ) {
          if ( ! char.IsDigit( c ) && c != '-' ) {
            e.Handled = true ;
            break ;
          }

          if ( textNew != null && textNew.Contains( "-" ) && c == '-' && textSelection.Equals( textNew.Substring( 0, textSelection.Length ) ) &&
               string.IsNullOrEmpty( textSelection ) ) {
            e.Handled = true ;
            break ;
          }

          if ( textNew != null && selectionStart > textNew.Length ) return ;
          if ( double.TryParse( textNew?.Insert( selectionStart, c.ToString() ), out var _ ) == false ) {
            if ( textSelection.Equals( textNew?.Substring( 0, textSelection.Length ) ) && ! string.IsNullOrEmpty( textSelection ) ) break ;

            if ( ( textNew != "" && c != '-' ) || ( textNew != "" && selectionStart != 0 && c == '-' ) ) {
              e.Handled = true ;
              break ;
            }
          }
        }

      if ( mode == InputBehaviourModesTextBox.TextHandleAutocad )
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
      if ( mode == InputBehaviourModesTextBox.Text ) return ;
      if ( mode == InputBehaviourModesTextBox.TextHandleAutocad ) {
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

  private static object? GetValueObjectPropertyByListName<T>( this T obj, string nameProperty )
  {
    object? valueResult = null ;
    var listNameProperty = nameProperty.Split( '.' ) ;
    if ( listNameProperty.Length == 1 ) return obj?.GetType().GetProperty( listNameProperty[ 0 ] )?.GetValue( obj ) ;
    var valueProp = obj?.GetType().GetProperty( listNameProperty[ 0 ] )?.GetValue( obj ) ;
    for ( var i = 1 ; i < listNameProperty.Length ; i++ ) {
      valueProp = valueProp?.GetType().GetProperty( listNameProperty[ i ] )?.GetValue( valueProp ) ;
      if ( i == listNameProperty.Length - 1 ) valueResult = valueProp ;
    }

    return valueResult ;
  }

  private static void SetValuePropertyByNames<T>( this T obj, string nameProperty, object? valueSet )
  {
    var listNameProperty = nameProperty.Split( '.' ) ;
    var valueProp = obj?.GetType().GetProperty( listNameProperty[ 0 ] )?.GetValue( obj ) ;
    if ( listNameProperty.Length == 1 ) {
      obj.SetValuePropertyByName( listNameProperty[ 0 ], valueSet ) ;
      return ;
    }

    for ( var i = 1 ; i < listNameProperty.Length ; i++ ) {
      if ( i == listNameProperty.Length - 1 ) {
        valueProp.SetValuePropertyByName( listNameProperty[ listNameProperty.Length - 1 ], valueSet ) ;
        return ;
      }

      valueProp = valueProp?.GetType().GetProperty( listNameProperty[ i ] )?.GetValue( valueProp ) ;
    }
  }

  private static void SetValuePropertyByName<TParent>( this TParent obj, string nameProperty, object? value )
  {
    try {
      if ( obj == null ) return ;
      PropertyInfo? property = null ;
      foreach ( var propertyInfo in obj.GetType().GetProperties() )
        if ( propertyInfo.Name == nameProperty ) {
          property = propertyInfo ;
          break ;
        }

      if ( property == null ) return ;
      if ( value == null && obj.IsChildNullAble( nameProperty ) == true ) {
        property.SetValue( obj, null ) ;
        return ;
      }

      var safeValue = obj.IsChildNullAble( nameProperty ) == true ? Convert.ChangeType( value, Nullable.GetUnderlyingType( property.PropertyType )! ) : value ;
      property.SetValue( obj, safeValue ) ;
    }
#if RELEASE
      catch{
#endif
#if DEBUG
    catch ( System.Exception e ) {
      MessageBox.Show( $"{e.Message}\n{e.StackTrace}" ) ;
#endif
    }
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

  private static bool CheckTextInvalid( string? text )
  {
    if ( text == null ) return true ;
    foreach ( var c in text )
      if ( ! char.IsDigit( c ) && c != '.' && c != '-' )
        return true ;
    return false ;
  }

  private static double? CanConvertToDouble( this object? text )
  {
    if ( text == null ) return null ;
    var textConvert = text.ToString() ;
    if ( string.IsNullOrEmpty( textConvert ) ) return null ;
    if ( double.TryParse( textConvert, out var doubleValue ) ) return doubleValue ;
    return null ;
  }

  private static int? CanConvertToInt( this object? text )
  {
    if ( text == null ) return null ;
    var textConvert = text.ToString() ;
    if ( string.IsNullOrEmpty( textConvert ) ) return null ;
    if ( int.TryParse( textConvert, out var intValue ) ) return intValue ;
    return null ;
  }
}

public enum InputBehaviourModesTextBox
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