using System ;
using System.Windows ;
using System.Windows.Interop ;
using System.Windows.Media ;
using Autodesk.Revit.UI ;

namespace LibraryCIT.WpfUtils
{
  public static class ViewUtils 
  {
    public static void OnTopMostView( this Window view )
    {
      var isMaximum = view.WindowState == WindowState.Maximized ;
      view.WindowState = isMaximum ? WindowState.Maximized : WindowState.Normal ;
      view.Topmost = true ;
      view.Topmost = false ;
    }
    
    // public static T HandleNavisworks<T>() where T : new()
    // {
    //   var inPtrHandle = Autodesk.Navisworks.Api.Application.Gui.MainWindow.Handle ;
    //   var ui = new T() ;
    //   if ( ui is Window window ) {
    //     new WindowInteropHelper( window ).Owner = inPtrHandle ;
    //   }
    //   return ui ;
    // } 
    //
    // public static T HandleAutocad<T>() where T : new()
    // {
    //   var inPtrHandle = Autodesk.AutoCAD.ApplicationServices.Core.Application.MainWindow.Handle ;
    //   var ui = new T() ;
    //   if ( ui is Window window ) {
    //     new WindowInteropHelper( window ).Owner = inPtrHandle ;
    //   }
    //   return ui ;
    // } 
    
    // public static T? CreateWindowOwnerNaVisWorks<T>(params object[]? args) where T : Window
    // {
    //   var inPtrHandle = Autodesk.Navisworks.Api.Application.Gui.MainWindow.Handle ;
    //   var ui =  args  == null ? Activator.CreateInstance(typeof( T )) : Activator.CreateInstance( typeof( T ), args ) ;
    //   if ( ui is Window window ) {
    //     new WindowInteropHelper( window ).Owner = inPtrHandle ;
    //   }
    //   return ui as T;
    // }
    
    public static T? CreateWindowOwnerAutocad<T>(params object[]? args) where T : Window
    {
      var inPtrHandle = Autodesk.AutoCAD.ApplicationServices.Core.Application.MainWindow.Handle ;
      var ui =  args  == null ? Activator.CreateInstance(typeof( T )) : Activator.CreateInstance( typeof( T ), args ) ;
      if ( ui is Window window ) {
        new WindowInteropHelper( window ).Owner = inPtrHandle ;
      }
      return ui as T;
    }
    
    public static T? CreateWindowOwnerRevit<T>( this UIApplication uiApplication, params object[]? args ) where T : Window
    {
      var inPtrHandle = uiApplication.MainWindowHandle ;
      var ui = args  == null ? Activator.CreateInstance(typeof( T )) : Activator.CreateInstance( typeof( T ), args ) ;
      if ( ui is Window window ) {
        new WindowInteropHelper( window ).Owner = inPtrHandle ;
      }
      return ui as T ;
    }
    
    public static Brush ConvertStrHexToBrush( string hex )
    {
      return new BrushConverter().ConvertFromString( $"#{hex}" ) as Brush ?? 
             new BrushConverter().ConvertFromString( hex ) as Brush ??
             Brushes.Transparent ;
    }
    
    private const string FontName = "Segoe UI" ;
    
    public static void ShowExtensionB(this Window window)
    {
      window.FontFamily = new FontFamily(FontName) ;
      var color = new BrushConverter().ConvertFrom( "#f0f0f0" ) as Brush ;
      window.Background = color ;
      window.WindowStartupLocation = WindowStartupLocation.CenterScreen ;
      window.Show();
    }

    public static bool? ShowDialogExtensionB(this Window window)
    {
      window.FontFamily = new FontFamily(FontName) ;
      var color = new BrushConverter().ConvertFrom( "#f0f0f0" ) as Brush ;
      window.Background = color ;
      window.WindowStartupLocation = WindowStartupLocation.CenterScreen ;
      return window.ShowDialog();
    } 
  }
}