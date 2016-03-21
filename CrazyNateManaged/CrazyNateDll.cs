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
  public static class CrazyNateDll
  {
    [DllImport("CrazyNate.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern IntPtr GetCrazyNateHModule();

    [DllImport("CrazyNate.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern IntPtr GetLaunchCrazyNateManagedAddress();

    [DllImport("CrazyNate.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern UInt32 GetInputOutputBufferCharCount();

    [DllImport("CrazyNate.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern UInt32 LaunchCrazyNateManaged([MarshalAs(UnmanagedType.LPWStr)] StringBuilder inputOutputBuffer);

    public static string GetAssemblyPath()
    {
      return Win32.GetModuleFileNameW(GetCrazyNateHModule());
    }
  }
}