using System.ComponentModel ;
using System.Windows ;
using Microsoft.Xaml.Behaviors ;

namespace Helpers.Utils_Wpf.Behavior ;

public class BaseControlBehavior<TControl, TView, TViewModel> : Behavior<TControl>
  where TControl : DependencyObject
  where TView : Window
  where TViewModel : INotifyPropertyChanged
{
  protected static TView? UiWindow ;
  protected static TViewModel? ViewModel ;

  private TView? GetWindow()
  {
    if ( AssociatedObject == null ) return default ;
    if ( AssociatedObject is TView window ) return window ;
    if ( Window.GetWindow( AssociatedObject ) is not TView windowGet ) return default ;
    return windowGet ;
  }

  private TViewModel? GetDataContext()
  {
    if ( UiWindow == null ) return default ;
    if ( UiWindow.DataContext is not TViewModel viewModel ) return default ;
    return viewModel ;
  }

  protected void InitWindow()
  {
    UiWindow = GetWindow() ;
    if ( UiWindow == null ) return ;
    ViewModel = GetDataContext() ;
    UiWindow.DataContextChanged += DataContextChanged ;
  }

  protected void DisposeWindow()
  {
    if ( UiWindow == null ) return ;
    UiWindow.DataContextChanged -= DataContextChanged ;
    UiWindow = null ;
    ViewModel = default ;
  }

  private void DataContextChanged( object sender, DependencyPropertyChangedEventArgs e )
  {
    ViewModel = GetDataContext() ;
  }
}