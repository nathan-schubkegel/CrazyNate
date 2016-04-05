using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using CrazyNateManaged;
using CrazyNateManaged.Injection;

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
      string[] exeNames = new string[] {"CrazyNateSandbox451.exe", "CrazyNateSandbox20.exe"};
      int? pid = exeNames.Select(exeName => ProcessInspector.FindProcess(exeName)).First(x => x != null);
      if (pid == null)
      {
        MessageBox.Show("Could not find any of these processes:" + Environment.NewLine +
          string.Join(Environment.NewLine, exeNames));
        return;
      }

      // get path of its exe
      string processPath = ProcessInspector.GetProcessPath(pid.Value);
      if (processPath == null)
      {
        // this can occur when the process dies while we're enumerating it
        MessageBox.Show("Could not get process path");
        return;
      }
      
      // inject my dll into it
      try
      {
        DllInfo[] dlls = CrazyNateManaged.DependencyFetcher.GetDllInjectionInfo().ToArray();
        var a = (ManagedDllInfo)dlls.Last();
        if (a.FileNameOrPath != CrazyNateManaged.DependencyFetcher.GetPathToThisAssembly())
        {
          throw new Exception("expected last assembly to be CrazyNateManaged.dll");
        }
        a.EntryTypeName = ManagedEntryPoint.EntryTypeName;
        a.EntryMethodName = ManagedEntryPoint.ShowMessageBoxMethodName;
        a.EntryArgument = "and I mean it!";
        DllInjector.InjectIntoProcess(pid.Value, dlls);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.ToString());
      }
    }
  }
}
