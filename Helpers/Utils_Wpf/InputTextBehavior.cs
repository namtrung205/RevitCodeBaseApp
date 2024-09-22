using System ;
using System.Collections.Generic ;
using System.Linq ;
using System.Windows ;
using System.Windows.Input ;
using System.Windows.Media ;
using TextBox = System.Windows.Controls.TextBox ;

namespace Helpers.Utils_Wpf ;

public static class InputTextBehaviour
{
  public static readonly DependencyProperty ModeProperty = DependencyProperty.RegisterAttached( "Mode", typeof( TextInputBehaviourModes? ),
    typeof( InputTextBehaviour ), new UIPropertyMetadata( null, OnValueChanged ) ) ;

  private static readonly List<string> ListHandleAutocad = new()
  {
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
  } ;

  private static Color ColorError { get ; } = Color.FromRgb( 255, 216, 255 ) ;

  public static List<string> ListString { get ; set ; } = new() ;

  public static TextInputBehaviourModes? GetMode( DependencyObject o )
  {
    return (TextInputBehaviourModes?) o.GetValue( ModeProperty ) ;
  }

  public static void SetMode( DependencyObject o, TextInputBehaviourModes? value )
  {
    o.SetValue( ModeProperty, value ) ;
  }

  private static void OnValueChanged( DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e )
  {
    var uiElement = dependencyObject as TextBox ;
    if ( uiElement == null ) return ;
    if ( e.NewValue is TextInputBehaviourModes ) {
      uiElement.PreviewTextInput += OnTextInput ;
      uiElement.TextChanged += UiElementTextChanged ;
      DataObject.AddPastingHandler( uiElement, OnPaste ) ;
    }
    else {
      uiElement.PreviewTextInput -= OnTextInput ;
      uiElement.TextChanged -= UiElementTextChanged ;
      DataObject.RemovePastingHandler( uiElement, OnPaste ) ;
    }
  }

  private static void UiElementTextChanged( object sender, RoutedEventArgs e )
  {
    if ( sender is not TextBox textBox ) return ;
    if ( string.IsNullOrWhiteSpace( textBox.Text ) ) textBox.Background = new SolidColorBrush( ColorError ) ;
  }

  private static void OnTextInput( object sender, TextCompositionEventArgs e )
  {
    if ( sender is not TextBox txtBox ) return ;
    if ( sender is not DependencyObject dependencyObject ) return ;
    var mode = GetMode( dependencyObject ) ;

    switch ( mode ) {
      case TextInputBehaviourModes.TextHandleAutoCad :
        foreach ( var c in e.Text )
          if ( ListHandleAutocad.Any( x => x == c.ToString() ) ) {
            e.Handled = true ;
            break ;
          }

        break ;
    }
  }

  private static void OnPaste( object sender, DataObjectPastingEventArgs e )
  {
    if ( sender is not DependencyObject dependencyObject ) return ;
    var mode = GetMode( dependencyObject ) ;

    if ( e.DataObject.GetDataPresent( DataFormats.Text ) ) {
      var text = Convert.ToString( e.DataObject.GetData( DataFormats.Text ) ).Trim() ;
      switch ( mode ) {
        case TextInputBehaviourModes.TextHandleAutoCad :
          if ( text.Any( c => ListHandleAutocad.Any( x => x == c.ToString() ) ) ) e.CancelCommand() ;
          break ;
      }
    }
    else {
      e.CancelCommand() ;
    }
  }
}

public enum TextInputBehaviourModes
{
  TextNoneNull,
  TextHandleAutoCad
}