using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyNateManaged.Injection
{
  public class ManagedDllInfo : DllInfo
  {
    // these must be populated, or the managed DLL can't be injected
    public string EntryTypeName { get; set; }
    public string EntryMethodName { get; set; }
    public string EntryArgument { get; set; }
    public int ReturnValue { get; set; }
  }
}
