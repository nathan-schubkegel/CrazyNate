using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using CrazyNateManaged;

namespace CrazyNateSandboxInjector
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      // find running process CrazyNateSandbox.exe
      Process[] processes = Process.GetProcesses();

      bool foundWhatLookedLikeIt = false;
      int? pid = null;
      foreach (Process p in processes)
      {
        if (p.ProcessName == "CrazyNateSandbox")
        {
          foundWhatLookedLikeIt = true;
        }

        string processPath;
        try
        {
          processPath = ProcessInspector.GetProcessPath(p.Id);
          if (processPath == null)
          {
            // this occurs when the process dies while we're enumerating it
            continue;
          }
        }
        catch
        {
          // obviously not a process we could inject into, then
          continue;
        }

        string fileName = Path.GetFileName(processPath);

        // take the standalone process if we can get it
        if ("CrazyNateSandbox.exe".Equals(fileName, StringComparison.OrdinalIgnoreCase))
        {
          pid = p.Id;
          break;
        }

        // take the 'vshost' process if that's all we can find
        if ("CrazyNateSandbox.vshost.exe".Equals(fileName, StringComparison.OrdinalIgnoreCase))
        {
          pid = p.Id;
        }
      }

      if (pid == null)
      {
        if (foundWhatLookedLikeIt) MessageBox.Show("Could not find CrazyNateSandbox.exe (saw CrazyNateSandbox but that wasn't it)");
        else MessageBox.Show("Could not find CrazyNateSandbox.exe");
      }
      else
      {
        // inject my dll into it
        try
        {
          DllInjector.InjectIntoProcess(pid.Value, "CrazyNateManaged.dll", "CrazyNateManaged.ManagedEntryPoint", "Enter", "and I mean it!");
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.ToString());
        }
      }
    }
  }
}
