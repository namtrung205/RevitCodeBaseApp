using System.Collections.Generic ;
using System.Linq ;
using System.Windows ;
using System.Windows.Controls ;
using System.Windows.Media ;

namespace Helpers.Utils_Wpf ;

public static class FindVisualUtils
{
  public static T? FindVisualChild<T>( DependencyObject obj ) where T : DependencyObject
  {
    for ( var i = 0 ; i < VisualTreeHelper.GetChildrenCount( obj ) ; i++ ) {
      var child = VisualTreeHelper.GetChild( obj, i ) ;
      if ( child is T dependencyObject ) return dependencyObject ;

      var childOfChild = FindVisualChild<T>( child ) ;
      if ( childOfChild != null )
        return childOfChild ;
    }

    return null ;
  }

  public static IEnumerable<T> FindVisualChildren<T>( DependencyObject? depObj ) where T : DependencyObject
  {
    if ( depObj == null ) yield return (T) Enumerable.Empty<T>() ;
    if ( depObj != null )
      for ( var i = 0 ; i < VisualTreeHelper.GetChildrenCount( depObj ) ; i++ ) {
        var ithChild = VisualTreeHelper.GetChild( depObj, i ) ;
        if ( ithChild is T t ) yield return t ;
        foreach ( var childOfChild in FindVisualChildren<T>( ithChild ) ) yield return childOfChild ;
      }
  }

  public static DataGridCell? GetDataGridCell( DependencyObject dependencyObject )
  {
    var parent = VisualTreeHelper.GetParent( dependencyObject ) ;
    while ( parent is not DataGridCell )
      if ( parent != null ) {
        parent = VisualTreeHelper.GetParent( parent ) ;
        if ( parent == null ) break ;
      }

    return parent as DataGridCell ;
  }

  public static Window? GetWindow( DependencyObject dependencyObject )
  {
    var parentWindow = Window.GetWindow( dependencyObject ) ;
    return parentWindow ;
  }

  public static TAncestor? GetAncestor<TAncestor>( this DependencyObject subElement )
    where TAncestor : DependencyObject
  {
    return subElement.GetAncestor<TAncestor>( null ) ;
  }

  private static TAncestor? GetAncestor<TAncestor>( this DependencyObject? subElement, UIElement? potentialAncestorToStopTheSearch )
    where TAncestor : DependencyObject
  {
    DependencyObject? parent ;
    for ( var subControl = subElement ; subControl != null ; subControl = parent ) {
      if ( subControl is TAncestor control ) return control ;

      if ( ReferenceEquals( subControl, potentialAncestorToStopTheSearch ) ) return null ;


      parent = VisualTreeHelper.GetParent( subControl ) ;
      if ( parent == null )
        if ( subControl is FrameworkElement element )
          parent = element.Parent ;
    }

    return null ;
  }
}