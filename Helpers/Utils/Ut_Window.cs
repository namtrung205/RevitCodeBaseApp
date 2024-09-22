using System ;
using System.Windows ;
using System.Windows.Forms ;
using System.Windows.Interop ;

namespace Helpers.Utils ;

public static class UtWindow
{
  public static (double Top, double Left) GetPosition( IntPtr handle )
  {
    // System.Windows.Forms.Screen currentScreen = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper( this ).Handle);
    var currentScreen = Screen.FromHandle( handle ) ;
    return ( currentScreen.Bounds.Top, currentScreen.Bounds.Left ) ;
  }

  public static bool OnPrimaryMonitor( IntPtr handle )
  {
    var currentScreen = Screen.FromHandle( handle ) ;
    if ( currentScreen.Primary ) return true ;
    return false ;
  }

  public static IntPtr Handle( this Window window )
  {
    return new WindowInteropHelper( window ).Handle ;
  }

  public static void SetStateWindows( this Window? window, WindowState state )
  {
    if ( window == null ) return ;
    window.WindowState = state ;
    foreach ( Window windowChild in window.OwnedWindows ) windowChild.WindowState = state ;
  }

  public static void SetStateChildWindows( this Window? window, WindowState state )
  {
    if ( window == null ) return ;
    foreach ( Window windowChild in window.OwnedWindows ) windowChild.WindowState = state ;
  }
}