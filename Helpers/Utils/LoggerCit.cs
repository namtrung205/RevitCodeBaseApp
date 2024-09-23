using System ;
using System.IO ;
using System.Runtime.CompilerServices ;
using Serilog ;

namespace Helpers.Utils ;

public sealed class LoggerCit
{
  private static Lazy<LoggerCit> _instance = new(() => new LoggerCit()) ;

  private readonly ILogger _serilog ;

  public static string LogFolder { get ; set ; } = "" ;

  // Private constructor to prevent external instantiation
  private LoggerCit()
  {
    var logPath = Path.Combine( Path.GetTempPath(), "logs" , LogFolder, "log-.txt" ) ;
    _serilog = new LoggerConfiguration()
      .MinimumLevel.Debug()
      .WriteTo.File(
        logPath,
        rollingInterval: RollingInterval.Day,
        fileSizeLimitBytes: 10_000_000, // 10 MB
        rollOnFileSizeLimit: true,
        retainedFileCountLimit: 7,
        shared: true,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{System.Exception}"
      )
      .CreateLogger() ;
  }

  public static LoggerCit Instance => _instance.Value ;

  public ILogger GetLogger()
  {
    return _serilog ;
  }
  
  public void LogInformation( string message,
    [CallerMemberName] string memberName = "",
    [CallerFilePath] string filePath = "",
    [CallerLineNumber] int lineNumber = 0 )
  {
    _serilog.Information( $"[{Path.GetFileName( filePath )}:{lineNumber} - {memberName}] {message}" ) ;
#if DEBUG
    Console.WriteLine($"[{Path.GetFileName( filePath )}:{lineNumber} - {memberName}] {message}");
#endif
  }

  public void LogError( Exception? ex = null,
    [CallerMemberName] string memberName = "",
    string message = "")
  {
    _serilog.Error( ex, $"{ex?.StackTrace}]" ) ;
#if DEBUG
    Console.WriteLine( string.IsNullOrEmpty( message ) ? $"[{memberName}-{ex?.Message}\n{ex?.StackTrace}]" : $"{message}" ) ;
#endif
  }
  
  public void LogWarning( string message )
  {
    _serilog.Warning( message ) ;
  }
  
  public void LogDebug( string message )
  {
    _serilog.Debug( message ) ;
  }
}