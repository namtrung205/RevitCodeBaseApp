using System.Globalization ;
using System.Windows ;

namespace Helpers.Utils ;

public static class UtilsMessage
{
  private static string _notificationCap = "通知" ;
  private static string _warningCap = "警告" ;
  private static string _errorCap = "エラーメッセージ" ;

  private static bool IsJapanese()
  {
    CultureInfo ci = CultureInfo.InstalledUICulture ;
    if ( ci.Name == "ja-JP" ) return true ;
    return false ;
  }
  
  public static MessageBoxResult Notification( string content )
  {
    if ( ! IsJapanese() ) _notificationCap = "Notification" ;
    return MessageBox.Show( content, _notificationCap, MessageBoxButton.OK, MessageBoxImage.Information ) ;
  }

  public static MessageBoxResult Notification_YesNo( string content )
  {
    if ( ! IsJapanese() ) _notificationCap = "Notification" ;
    return MessageBox.Show( content, _notificationCap, MessageBoxButton.YesNo, MessageBoxImage.Information ) ;
  }

  public static MessageBoxResult Notification_OKCancel( string content )
  {
    if ( ! IsJapanese() ) _notificationCap = "Notification" ;
    return MessageBox.Show( content, _notificationCap, MessageBoxButton.OKCancel, MessageBoxImage.Information ) ;
  }

  public static MessageBoxResult Warning( string content )
  {
    if ( ! IsJapanese() ) _warningCap = "Warning" ;
    return MessageBox.Show( content, _warningCap, MessageBoxButton.OK, MessageBoxImage.Warning ) ;
  }

  public static MessageBoxResult Warning_YesNo( string content )
  {
    if ( ! IsJapanese() ) _warningCap = "Warning" ;
    return MessageBox.Show( content, _warningCap, MessageBoxButton.YesNo, MessageBoxImage.Warning ) ;
  }

  public static MessageBoxResult Warning_OKCancel( string content )
  {
    if ( ! IsJapanese() ) _warningCap = "Warning" ;
    return MessageBox.Show( content, _warningCap, MessageBoxButton.OKCancel, MessageBoxImage.Warning ) ;
  }

  public static MessageBoxResult Error( string content )
  {
    if ( ! IsJapanese() ) _errorCap = "Error" ;
    return MessageBox.Show( content, _errorCap, MessageBoxButton.OK, MessageBoxImage.Error ) ;
  }

  public static MessageBoxResult Error_YesNo( string content )
  {
    if ( ! IsJapanese() ) _errorCap = "Error" ;
    return MessageBox.Show( content, _errorCap, MessageBoxButton.YesNo, MessageBoxImage.Error ) ;
  }

  public static MessageBoxResult Error_OKCancel( string content )
  {
    if ( ! IsJapanese() ) _errorCap = "Error" ;
    return MessageBox.Show( content, _errorCap, MessageBoxButton.OKCancel, MessageBoxImage.Error ) ;
  }
}