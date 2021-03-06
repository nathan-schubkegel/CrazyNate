﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace CrazyNateManaged
{
  public static class ProcessInspector
  {
    public static int? FindProcess(string exeName)
    {
      int? pid = null;
      try
      {
        exeName = exeName ?? string.Empty;

        // look through all running processes
        Process[] processes = Process.GetProcesses();
        foreach (Process p in processes)
        {
          string processPath = ProcessInspector.GetProcessPath(p.Id);
          if (processPath == null)
          {
            // this occurs when the process dies while we're enumerating it
            continue;
          }

          string fileName = Path.GetFileName(processPath);

          if (exeName.Equals(fileName, StringComparison.OrdinalIgnoreCase))
          {
            pid = p.Id;
            break;
          }

          p.Dispose();
        }
      }
      catch
      {
        pid = null;
      }
      return pid;
    }

    public static string GetProcessPath(int processPid)
    {
      // Retrieve a HANDLE to the remote process (OpenProcess).
      IntPtr processHandle = Win32.OpenProcess(
        Win32.PROCESS_QUERY_INFORMATION,
        false, // inherithandles = no
        processPid);

      if (processHandle == IntPtr.Zero)
      {
        //throw new Exception("Unable to open handle to target process: " + Win32.GetLastErrorMessage());
        // this can happen if the process dies before we get at it
        return null;
      }

      try
      {
        // Get the full path of the target process
        return Win32.QueryFullProcessImageNameW(processHandle);
      }
      finally
      {
        Win32.CloseHandle(processHandle);
      }
    }
  }
}
