using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace CrazyNateManaged
{
  public static class DllExports
  {
    public static List<DllExport> GetExports(IntPtr dllHModule)
    {
      List<DllExport> results = new List<DllExport>();
      
      // MSDN rules on this are weird. So I'm using a weird technique.
      var processHandle = Win32.OpenProcess(Win32.PROCESS_QUERY_INFORMATION, false, System.Diagnostics.Process.GetCurrentProcess().Id);
      if (processHandle == IntPtr.Zero) throw new Exception("couldn't look at my own process or something");
      try
      {
        IntPtr callerId = processHandle; // new IntPtr(processHandle.ToInt64() + 1);

        // make an empty temp dir for the search path
        string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("D") + ".CrazyNate.DllExports");
        Directory.CreateDirectory(tempDir);
        try
        {
          var a = DbgHelp.SymSetOptions(
            DbgHelp.SYMOPT_DEFERRED_LOADS | 
            DbgHelp.SYMOPT_FAIL_CRITICAL_ERRORS | 
            DbgHelp.SYMOPT_NO_UNQUALIFIED_LOADS |
            DbgHelp.SYMOPT_PUBLICS_ONLY);

          var b = DbgHelp.SymInitializeW(callerId, tempDir, false);
          try
          {
            staticResults = results;
            try
            {
              var c = DbgHelp.SymEnumSymbolsW(callerId, dllHModule.ToInt64(), "*", GetExportCallback, IntPtr.Zero);
            }
            finally
            {
              staticResults = null;
            }
          }
          finally
          {
            var d = DbgHelp.SymCleanup(callerId);
          }
        }
        finally
        {
          Directory.Delete(tempDir, true);
        }
      }
      finally
      {
        Win32.CloseHandle(processHandle);
      }

      return results;
    }

    private static List<DllExport> staticResults;

    private static bool GetExportCallback(IntPtr pSymInfo, uint SymbolSize, IntPtr UserContext)
    {
      DbgHelp.SymbolInfo symbolInfo = (DbgHelp.SymbolInfo)Marshal.PtrToStructure(pSymInfo, typeof(DbgHelp.SymbolInfo));

      // TODO: what is SymbolSize good for?

      string name = Marshal.PtrToStringUni(new IntPtr(pSymInfo.ToInt64() + DbgHelp.SymbolInfoNameOffset), (int)symbolInfo.NameLen);
      DllExport export = new DllExport()
      {
        Name = name,
        Info = symbolInfo,
      };

      staticResults.Add(export);

      return true;
    }
  }
}
