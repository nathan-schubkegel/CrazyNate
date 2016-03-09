using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace CrazyNateManaged
{
  public static class DbgHelp
  {
    public const UInt32 SYMOPT_DEFERRED_LOADS = 0x00000004;
    public const UInt32 SYMOPT_FAIL_CRITICAL_ERRORS = 0x00000200;
    public const UInt32 SYMOPT_NO_UNQUALIFIED_LOADS = 0x00000100;
    public const UInt32 SYMOPT_PUBLICS_ONLY = 0x00004000;

    [DllImport("DbgHelp.dll", SetLastError = true)]
    public static extern UInt32 SymSetOptions(
      UInt32 SymOptions);

    [DllImport("DbgHelp.dll", SetLastError = true)]
    public static extern bool SymInitializeW(
      IntPtr hProcess,
      [MarshalAs(UnmanagedType.LPWStr)] string UserSearchPath,
      bool fInvadeProcess);

    [DllImport("DbgHelp.dll", SetLastError = true)]
    public static extern bool SymCleanup(
      IntPtr hProcess);

    [DllImport("DbgHelp.dll", SetLastError = true)]
    public static extern bool SymEnumSymbolsW(
      IntPtr hProcess,
      Int64 BaseOfDll,
      [MarshalAs(UnmanagedType.LPWStr)] string Mask,
      EnumerateSymbolsCallback EnumSymbolsCallback,
      IntPtr UserContext);

    public delegate bool EnumerateSymbolsCallback(
      IntPtr pSymInfo,
      UInt32 SymbolSize,
      IntPtr UserContext);

    [DataContract]
    [StructLayout(LayoutKind.Sequential)]
    public struct SymbolInfo 
    {
      [DataMember]
      public UInt32 SizeOfStruct;
      [DataMember]
      public UInt32 TypeIndex;
      [DataMember]
      public UInt64 Reserved1;
      [DataMember]
      public UInt64 Reserved2;
      //24

      [DataMember]
      public UInt32 Index;
      [DataMember]
      public UInt32 Size;
      [DataMember]
      public UInt64 ModBase;
      [DataMember]
      public UInt32 Flags;
      // 20 (44)

      [DataMember]
      public UInt64 Value;
      [DataMember]
      public UInt64 Address;
      [DataMember]
      public UInt32 Register;
      [DataMember]
      public UInt32 Scope;
      // 24 (68)

      [DataMember]
      public UInt32 Tag;
      [DataMember]
      public UInt32 NameLen;
      [DataMember]
      public UInt32 MaxNameLen;
      // 12 (80)

      [DataMember]
      public char NameFirstChar;
    }

    public const int SymbolInfoNameOffset = 80;
  }
}
