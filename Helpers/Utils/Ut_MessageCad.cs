using System.Windows ;
namespace Helpers.Utils ;

public static class UtilsMessageCad
{
  private static string _notificationCap = "通知" ;
  private static string _warningCap = "警告" ;
  private static string _errorCap = "エラーメッセージ" ;

  public static MessageBoxResult Notification( string content )
  {
    _notificationCap = "Notification" ;
    return MessageBox.Show( content, _notificationCap, MessageBoxButton.OK, MessageBoxImage.Information ) ;
  }

  public static MessageBoxResult Notification_YesNo( string content )
  {
    _notificationCap = "Notification" ;
    return MessageBox.Show( content, _notificationCap, MessageBoxButton.YesNo, MessageBoxImage.Information ) ;
  }

  public static MessageBoxResult Notification_OKCancel( string content )
  {
    _notificationCap = "Notification" ;
    return MessageBox.Show( content, _notificationCap, MessageBoxButton.OKCancel, MessageBoxImage.Information ) ;
  }

  public static MessageBoxResult Warning( string content )
  {
    _warningCap = "Warning" ;
    return MessageBox.Show( content, _warningCap, MessageBoxButton.OK, MessageBoxImage.Warning ) ;
  }

  public static MessageBoxResult Warning_YesNo( string content )
  {
    _warningCap = "Warning" ;
    return MessageBox.Show( content, _warningCap, MessageBoxButton.YesNo, MessageBoxImage.Warning ) ;
  }

  public static MessageBoxResult Warning_OKCancel( string content )
  {
    _warningCap = "Warning" ;
    return MessageBox.Show( content, _warningCap, MessageBoxButton.OKCancel, MessageBoxImage.Warning ) ;
  }

  public static MessageBoxResult Error( string content )
  {
    _errorCap = "Error";
    return MessageBox.Show( content, _errorCap, MessageBoxButton.OK, MessageBoxImage.Error ) ;
  }

  public static MessageBoxResult Error_YesNo( string content )
  {
    _errorCap = "Error" ;
    return MessageBox.Show( content, _errorCap, MessageBoxButton.YesNo, MessageBoxImage.Error ) ;
  }

  public static MessageBoxResult Error_OKCancel( string content )
  {
     _errorCap = "Error" ;
    return MessageBox.Show( content, _errorCap, MessageBoxButton.OKCancel, MessageBoxImage.Error ) ;
  }
}