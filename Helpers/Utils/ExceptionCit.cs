using System ;

namespace Helpers.Utils ;

public class Exception : Exception
{
  public Exception()
  {}
  public Exception(string exception) : base(exception)
  {}
  public Exception (string message, Exception innerException)
    : base (message, innerException)
  {} 
}