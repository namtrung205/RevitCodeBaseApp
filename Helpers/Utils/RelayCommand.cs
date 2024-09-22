using System ;
using System.Windows.Input ;

namespace Helpers.Utils ;

public class RelayCommand<T>( Predicate<T> canExecute, Action<T> execute ) : ICommand
{
  private readonly Action<T> _execute = execute ?? throw new SystemException(nameof( execute ) ) ;

  public bool CanExecute( object parameter )
  {
    try {
      return canExecute( (T) parameter ) ;
    }
    catch {
      return true ;
    }
  }

  public void Execute( object parameter )
  {
    _execute( (T) parameter ) ;
  }

  public event EventHandler CanExecuteChanged
  {
    add => CommandManager.RequerySuggested += value ;
    remove => CommandManager.RequerySuggested -= value ;
  }
}