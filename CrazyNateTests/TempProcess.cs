using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CrazyNateTests
{
  public class TempProcess : IDisposable
  {
    public Process Process { get; private set; }

    public TempProcess(Process process)
    {
      Process = process;
      if (!Process.Start()) throw new Exception("Failed to start process");
    }

    public void Dispose()
    {
      if (Process != null)
      {
        try
        {
          if (!Process.HasExited)
          {
            int id = Process.Id;

            IntPtr handle = Process.Handle;

            // may already be dead, even if we check first, yay race conditions
            Process.Kill();

            // wait up to 5000ms for the process to finish
            CrazyNateManaged.Win32.WaitForSingleObject(handle, 5000);
          }
        }
        catch
        {
        }

        try
        {
          Process.Dispose();
        }
        catch
        {
        }

        Process = null;
      }
    }

    ~TempProcess()
    {
      Dispose();
    }

    public override string ToString()
    {
      try
      {
        if (Process != null)
        {
          return Process.Id.ToString();
        }
      }
      catch
      {
      }
      return null;
    }
  }
}
