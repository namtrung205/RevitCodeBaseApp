using System.ComponentModel ;

namespace RevitAddinApp._01_Model.Enums ;

public enum StatusObject
{
  [Description( "-" )]
  None,

  [Description( "OK" )]
  Success,

  [Description( "NG" )]
  Error
}