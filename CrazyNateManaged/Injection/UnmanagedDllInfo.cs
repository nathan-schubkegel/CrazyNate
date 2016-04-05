using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyNateManaged.Injection
{
  public class UnmanagedDllInfo : DllInfo
  {
    // these may be empty if not needed
    public string EntryProcName { get; set; }
    public StringBuilder InputOutputBuffer { get; set; }
    public int ReturnValue { get; set; }
  }
}
