using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrazyNateManaged;
using System.Runtime.Serialization;
using System.IO;
using System.Diagnostics;

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
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      // This part explores BPL exports
      Application.Run(new Form1());
    }
  }
}
