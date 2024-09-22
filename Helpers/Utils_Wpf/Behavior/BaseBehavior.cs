using System.Windows ;
using Microsoft.Xaml.Behaviors ;
namespace Helpers.Utils_Wpf.Behavior ;

public class BaseBehavior<TControl> : Behavior<TControl>
  where TControl : DependencyObject
{
  protected static Window? UiWindow ;
  protected static object? ViewModel ;
  
  private Window? GetWindow()
  {
    if ( AssociatedObject == null ) return default ;
    if ( AssociatedObject is Window window ) return window ;
    if ( Window.GetWindow( AssociatedObject ) is not { } windowGet ) return default ;
    return windowGet ;
  }

  private object? GetDataContext()
  {
    if ( UiWindow == null ) return default ;
    if ( UiWindow.DataContext is { } viewModel ) {
      return viewModel ;
    }
    return default ;
  }
  
  protected void InitWindow()
  {
    UiWindow = GetWindow() ;
    if ( UiWindow == null ) return ;
    ViewModel = GetDataContext() ;
    UiWindow.DataContextChanged += DataContextChanged ;
  }
  
  private void DataContextChanged( object sender, DependencyPropertyChangedEventArgs e )
  {
    ViewModel = GetDataContext() ;
  }
}