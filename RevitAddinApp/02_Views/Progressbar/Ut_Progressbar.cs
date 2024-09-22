using System.ComponentModel ;
using System.Windows ;
using System.Windows.Input ;
using System.Windows.Media.Imaging ;
using Visibility = System.Windows.Visibility ;

namespace RevitAddinApp._02_Views.Progressbar ;

public static class UtProgressbar
{
  public static UiProgressbar? UiProgressbar ;
  public static bool StopProgress ;
  public static void InvokeProgressbarWithIndex<T>( T objectInvoke, IList<T> listObject, string titleWindow, string pathIcon ,bool allowClose = false)
  {
    if ( UiProgressbar == null ) {
      StopProgress = false ;
      
      UiProgressbar = new UiProgressbar() ;
      UiProgressbar.Topmost = true ;
      if ( allowClose ) {
        UiProgressbar.Icon =new BitmapImage(new Uri(pathIcon)) ;
        UiProgressbar.Title = titleWindow ;
        UiProgressbar.CustomWindow.Visibility = Visibility.Collapsed ;
        UiProgressbar.WindowStyle = WindowStyle.ThreeDBorderWindow ;
      }
      else {
        UiProgressbar.SetIcon( pathIcon ) ;
        UiProgressbar.TextTitleWindow.Text = titleWindow ;
      }
      UiProgressbar.ProgressBar.Maximum = listObject.Count ;
      UiProgressbar.Closing += UiProgressbarOnClosing;
    }

    Mouse.OverrideCursor = Cursors.Wait ;
    var indexProgress = listObject.IndexOf( objectInvoke ) ;
    UiProgressbar.UpdateProgressbar( indexProgress ) ;
    UiProgressbar.TextProgress.Text = $"{indexProgress + 1}/{listObject.Count}" ;
  }
  public static void InvokeProgressbarWithText<T>( string textProgress,T objectInvoke, IList<T> listObject , string titleWindow, string pathIcon ,bool allowClose = false)
  {
    if ( UiProgressbar == null ) {
      StopProgress = false ;
      
      UiProgressbar = new UiProgressbar() ;
      UiProgressbar.Topmost = true ;
      if ( allowClose ) {
        UiProgressbar.Icon =new BitmapImage(new Uri(pathIcon)) ;
        UiProgressbar.Title = titleWindow ;
        UiProgressbar.CustomWindow.Visibility = Visibility.Collapsed ;
        UiProgressbar.WindowStyle = WindowStyle.ThreeDBorderWindow ;
      }
      else {
        UiProgressbar.SetIcon( pathIcon ) ;
        UiProgressbar.TextTitleWindow.Text = titleWindow ;
      }
      UiProgressbar.ProgressBar.Maximum =  listObject.Count ;
      UiProgressbar.Closing += UiProgressbarOnClosing;
    }
    Mouse.OverrideCursor = Cursors.Wait ;
    var indexProgress = listObject.IndexOf( objectInvoke ) ;
    UiProgressbar.UpdateProgressbar( indexProgress ) ;
    UiProgressbar.TextProgress.Text = $"{textProgress}" ;
  }

  public static void InvokeProgressbarWithPercent( double value, string titleWindow, string pathIcon ,bool allowClose = false)
  {
    if ( UiProgressbar == null ) {
      StopProgress = false ;
      
      UiProgressbar = new UiProgressbar() ;
      UiProgressbar.Topmost = true ;
      
      if ( allowClose ) {
        UiProgressbar.Icon =new BitmapImage(new Uri(pathIcon)) ;
        UiProgressbar.Title = titleWindow ;
        UiProgressbar.CustomWindow.Visibility = Visibility.Collapsed ;
        UiProgressbar.WindowStyle = WindowStyle.ThreeDBorderWindow ;
      }
      else {
        UiProgressbar.SetIcon( pathIcon ) ;
        UiProgressbar.TextTitleWindow.Text = titleWindow ;
      }
      UiProgressbar.ProgressBar.Minimum = 0 ;
      UiProgressbar.ProgressBar.Maximum = 100 ;
      UiProgressbar.Closing += UiProgressbarOnClosing;
    }

    Mouse.OverrideCursor = Cursors.Wait ;
    UiProgressbar.UpdateProgressbar( value ) ;
    UiProgressbar.TextProgress.Text = $"{value}/{100}" ;
  }

  private static void UiProgressbarOnClosing( object sender, CancelEventArgs e )
  {
    StopProgress = true ;
  }
  public static void DisPoseProgressbar()
  {
    UiProgressbar?.Close() ;
    UiProgressbar = null ;
    Mouse.OverrideCursor = null ;
  }
}