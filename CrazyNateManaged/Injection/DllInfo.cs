using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyNateManaged.Injection
{
  public abstract class DllInfo
  {
    public string FileNameOrPath { get; set; }
    public DllLoadBehavior LoadBehavior { get; set; }
    public DllCopyBehavior CopyBehavior { get; set; }
    public UInt32 WaitTimeMs { get; set; } // use Win32.INFINITE for infinite

    public DllInfo()
    {
      WaitTimeMs = Win32.INFINITE;
    }
  }
}
