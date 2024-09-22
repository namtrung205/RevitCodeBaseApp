using System.Collections.Generic ;
using System.ComponentModel ;
using System.Runtime.CompilerServices ;

namespace Helpers.Utils ;

public class ViewModelBase : INotifyPropertyChanged
{
  public event PropertyChangedEventHandler PropertyChanged = null! ;

  protected virtual void OnPropertyChanged( [CallerMemberName] string? propertyName = "" )
  {
    PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) ) ;
  }

  protected virtual bool SetProperty<T>( ref T member, T value, [CallerMemberName] string? propertyName = null )
  {
    if ( EqualityComparer<T>.Default.Equals( member, value ) ) return false ;

    member = value ;
    OnPropertyChanged( propertyName ) ;
    return true ;
  }
}