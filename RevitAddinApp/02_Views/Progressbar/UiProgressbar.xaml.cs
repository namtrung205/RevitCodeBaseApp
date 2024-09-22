using System ;
using System.Windows ;
using System.Windows.Controls.Primitives ;
using System.Windows.Input ;
using System.Windows.Media.Imaging ;
using System.Windows.Threading ;

namespace RevitAddinApp._02_Views.Progressbar ;

public partial class UiProgressbar : Window
{
  public UiProgressbar()
  {
    InitializeComponent() ;
  }

  public void UpdateProgressbar( double value )
  {
    Dispatcher.Invoke( new Action<DependencyProperty, object>( ( dp, valueDp ) =>
      {
        Mouse.OverrideCursor = Cursors.Wait ;
        ProgressBar.SetValue( dp, valueDp ) ;
      } ),
      DispatcherPriority.Background,
      RangeBase.ValueProperty, value ) ;
  }

  public void SetIcon( string path )
  {
    IconImage.Source = new BitmapImage( new Uri( path ) ) ;
  }
}