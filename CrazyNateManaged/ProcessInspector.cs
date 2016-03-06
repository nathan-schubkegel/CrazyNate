using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Reflection;
using System.IO;

namespace CrazyNateManaged
{
  public static class ProcessInspector
  {
    public static string GetProcessPath(int processPid)
    {
      // Retrieve a HANDLE to the remote process (OpenProcess).
      IntPtr processHandle = Win32.OpenProcess(
        Win32.PROCESS_QUERY_INFORMATION,
        false, // inherithandles = no
        processPid);

      if (processHandle == IntPtr.Zero)
      {
        throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to open handle to target process");
      }

      try
      {
        // Get the full path of the target process
        StringBuilder processPathBuffer = new StringBuilder(Win32.MAX_PATH);
        int numChars = Win32.MAX_PATH;
        if (!Win32.QueryFullProcessImageNameW(processHandle, 0, processPathBuffer, ref numChars))
        {
          throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to get full executable path of target process");
        }
        processPathBuffer.Length = numChars;
        string processPath = processPathBuffer.ToString();
        return processPath;
      }
      finally
      {
        Win32.CloseHandle(processHandle);
      }
    }

  }
}
