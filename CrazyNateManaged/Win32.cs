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
  public static class Win32
  {
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr OpenProcess(
      UInt32 dwDesiredAccess,
      bool bInheritHandle,
      Int32 dwProcessId);

    public const UInt32 PROCESS_CREATE_THREAD = 0x02;
    public const UInt32 PROCESS_QUERY_INFORMATION = 0x0400;
    public const UInt32 PROCESS_VM_OPERATION = 0x0008;
    public const UInt32 PROCESS_VM_WRITE = 0x0020;
    public const UInt32 PROCESS_VM_READ = 0x0010;

    public const Int32 MAX_PATH = 260;
    //public const Int32 PROCESS_NAME_NATIVE = 1;

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool CloseHandle(IntPtr hObject);

    /*
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern Int32 GetModuleFileNameExW(
      IntPtr hProcess, 
      IntPtr hModule, 
      ref byte[] lpFilename,
      Int32 nSize);
     * */

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool QueryFullProcessImageNameW(
      IntPtr hProcess,
      Int32 dwFlags,
      [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpExeName,
      ref Int32 lpdwSize);

    public static string QueryFullProcessImageNameW(IntPtr processHandle)
    {
      // Get the full path of the target process
      StringBuilder processPathBuffer = new StringBuilder(2000);
      int numChars = processPathBuffer.Capacity;
      if (!Win32.QueryFullProcessImageNameW(processHandle, 0, processPathBuffer, ref numChars))
      {
        // this could probably happen if the process dies before we get at it
        return null;
      }
      processPathBuffer.Length = numChars;
      return processPathBuffer.ToString();
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr VirtualAllocEx(
      IntPtr hProcess,
      IntPtr lpAddress,
      IntPtr szSize,
      UInt32 flAllocationType,
      UInt32 flProtect);

    public const UInt32 MEM_COMMIT = 0x00001000;
    public const UInt32 PAGE_READWRITE = 0x04;
    public const UInt32 MEM_RELEASE = 0x8000;

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool VirtualFreeEx(
      IntPtr hProcess,
      IntPtr lpAddress,
      IntPtr dwSize,
      UInt32 dwFreeType);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool WriteProcessMemory(
      IntPtr hProcess,
      IntPtr lpBaseAddress,
      [MarshalAs(UnmanagedType.LPArray)] byte[] lpBuffer,
      IntPtr nSize,
      ref IntPtr lpNumberOfBytesWritten);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool WriteProcessMemory(
      IntPtr hProcess,
      IntPtr lpBaseAddress,
      [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpBuffer,
      IntPtr nSize,
      ref IntPtr lpNumberOfBytesWritten);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool ReadProcessMemory(
      IntPtr hProcess,
      IntPtr lpBaseAddress,
      IntPtr lpBuffer,
      IntPtr nSize,
      ref IntPtr lpNumberOfBytesRead
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool ReadProcessMemory(
      IntPtr hProcess,
      IntPtr lpBaseAddress,
      [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpBuffer,
      IntPtr nSize,
      ref IntPtr lpNumberOfBytesRead
    );

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetModuleHandleW(
      [MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr GetProcAddress(
      IntPtr hModule,
      [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr CreateRemoteThread(
      IntPtr hProcess,
      IntPtr lpThreadAttributes,
      IntPtr dwStackSize,
      IntPtr lpStartAddress,
      IntPtr lpParameter,
      Int32 dwCreationFlags,
      ref Int32 lpThreadId);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern UInt32 WaitForSingleObject(
      IntPtr hHandle,
      UInt32 dwMilliseconds);

    public const UInt32 INFINITE = 0xFFFFFFFF;
    public const UInt32 WAIT_OBJECT_0 = 0;

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool GetExitCodeThread(
      IntPtr hThread,
      ref Int32 lpExitCode);

    // this will be used for x64 injection someday
#if false
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool EnumProcessModules(
      IntPtr hProcess,
      ref IntPtr[] lphModule,
      UInt32 cb,
      ref UInt32 lpcbNeeded);
#endif

    public static string GetLastErrorMessage()
    {
      return (new Win32Exception(Marshal.GetLastWin32Error())).Message;
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr LoadLibraryW(
      [MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern UInt32 GetModuleFileNameW(
      IntPtr hModule,
      [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpFilename,
      UInt32 nSize);

    /// <summary>
    /// Returns full path of a DLL loaded in the current process.
    /// </summary>
    public static string GetModuleFileNameW(IntPtr hModule)
    {
      // TODO: do this without two allocations
      StringBuilder b = new StringBuilder(512);
      UInt32 numChars = (UInt32)b.Capacity;
      while (numChars == b.Capacity)
      {
        b.Capacity *= 2;
        numChars = GetModuleFileNameW(hModule, b, (UInt32)b.Capacity);
      }
      if (numChars == 0) return null;
      b.Length = (int)numChars;
      return b.ToString();
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern UInt32 GetLongPathName(
      [MarshalAs(UnmanagedType.LPWStr)] string lpszShortPath,
      [MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpszLongPath,
      UInt32 cchBuffer);

    public static string GetLongPathName(string shortPath)
    {
      UInt32 requiredChars = GetLongPathName(shortPath, null, 0);
      if (requiredChars == 0) return null;
      StringBuilder b = new StringBuilder((int)requiredChars);
      UInt32 numChars = GetLongPathName(shortPath, b, requiredChars);
      if (numChars == 0) return null;
      b.Length = (int)numChars;
      return b.ToString();
    }
  }
}
