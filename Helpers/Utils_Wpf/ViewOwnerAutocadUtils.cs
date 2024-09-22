using System ;
using System.Collections.Generic ;
using System.ComponentModel ;
using System.Linq ;
using System.Windows ;
using System.Windows.Interop ;
using System.Windows.Media ;

namespace LibraryCIT.WpfUtils
{
  public static class ViewOwnerAutocadUtils
  {
    private static Dictionary<string,Window> Windows { get ; set ; } = new() ;
    
    public static void OnTopMostView( this Window view )
    {
      var isMaximum = view.WindowState == WindowState.Maximized ;
      view.WindowState = isMaximum ? WindowState.Maximized : WindowState.Normal ;
      view.Topmost = true ;
      view.Topmost = false ;
    }
    
    public static T? ShowWindowOwnerAutocad<T>(params object[]? args) where T : Window
    {
      var type = typeof( T ).FullName ;
      var inPtrHandle = Autodesk.AutoCAD.ApplicationServices.Core.Application.MainWindow.Handle ;
      var ui =  args  == null ? Activator.CreateInstance(typeof( T )) : Activator.CreateInstance( typeof( T ), args ) ;
      if ( ui is not Window window ) return null;
      if ( Windows.Any(x =>x.Key == type) ) return Windows[type] as T;
      new WindowInteropHelper( window ).Owner = inPtrHandle ;
      Windows.Add(type,window);
      ShowExtensionAutocad( window ) ;
      window.Closing += WindowOnClosing;
      return ui as T ;
    }
    private static void WindowOnClosing( object sender, CancelEventArgs e )
    {
      if ( sender is not Window window) return;
      var type = sender.GetType().FullName ;
      if (Windows.Any(x =>x.Key == type) ) {
        Windows.Remove( type ) ;
      }
      window.Closing -= WindowOnClosing;
    }
    

    public static T? ShowDialogWindowOwnerAutocad<T>(params object[]? args) where T : Window
    {
      var type = typeof( T ).FullName ;
      var inPtrHandle = Autodesk.AutoCAD.ApplicationServices.Core.Application.MainWindow.Handle ;
      var ui =  args  == null ? Activator.CreateInstance(typeof( T )) : Activator.CreateInstance( typeof( T ), args ) ;
      if ( ui is not Window window ) return default;
      if ( Windows.Any(x =>x.Key == type) ) return default;
      new WindowInteropHelper( window ).Owner = inPtrHandle ;
      Windows.Add(type,window);
      window.Closing += WindowDialogOnClosing;
      window.ShowDialogExtensionAutocad() ;
      return ui as T ;
    }
    
    private static void WindowDialogOnClosing( object sender, CancelEventArgs e )
    {
      if ( sender is not Window window) return;
      var type = sender.GetType().FullName ;
      if (Windows.Any(x =>x.Key == type) ) {
        Windows.Remove( type ) ;
      }
      window.Closing -= WindowDialogOnClosing;
    }
    
    
    public static Brush ConvertStrHexToBrush( string hex )
    {
      return new BrushConverter().ConvertFromString( $"#{hex}" ) as Brush ?? 
             new BrushConverter().ConvertFromString( hex ) as Brush ??
             Brushes.Transparent ;
    }
    
    private const string FontName = "Segoe UI" ;
    
    public static void ShowExtensionAutocad(this Window window)
    {
      window.FontFamily = new FontFamily(FontName) ;
      var color = new BrushConverter().ConvertFrom( "#f0f0f0" ) as Brush ;
      window.Background = color ;
      window.WindowStartupLocation = WindowStartupLocation.CenterScreen ;
      window.Show();
    }

    public static bool? ShowDialogExtensionAutocad(this Window window)
    {
      window.FontFamily = new FontFamily(FontName) ;
      var color = new BrushConverter().ConvertFrom( "#f0f0f0" ) as Brush ;
      window.Background = color ;
      window.WindowStartupLocation = WindowStartupLocation.CenterScreen ;
      return window.ShowDialog();
    } 
  }
}