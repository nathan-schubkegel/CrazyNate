using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrazyNateManaged;

namespace CrazyNateSelfLauncher
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      StringBuilder builder = new StringBuilder((int)CrazyNateDll.GetInputOutputBufferCharCount());
      builder.Append("CrazyNate.dll");
      builder.Append((char)0);
      builder.Append("CrazyNateManaged.dll");
      builder.Append((char)0);
      builder.Append("CrazyNateManaged.ManagedEntryPoint");
      builder.Append((char)0);
      builder.Append("Enter");
      builder.Append((char)0);
      builder.Append("and I mean it!");
      builder.Append((char)0);
      CrazyNateDll.LaunchCrazyNateManaged(builder);
    }
  }
}
