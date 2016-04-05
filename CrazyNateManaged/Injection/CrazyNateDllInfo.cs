using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyNateManaged.Injection
{
  public class CrazyNateDllInfo : DllInfo
  {
    public CrazyNateDllInfo()
    {
      FileNameOrPath = CrazyNateDll.GetAssemblyPath();
    }
  }
}
