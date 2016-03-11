using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace CrazyNateManaged
{
  public static class ImageHlp
  {
    [DllImport("Imagehlp.dll", SetLastError = true)]
    public static extern bool MapAndLoad(
      [MarshalAs(UnmanagedType.LPStr)] string ImageName,
      [MarshalAs(UnmanagedType.LPStr)] string DllPath,
      ref LOADED_IMAGE LoadedImage,
      bool DotDll,
      bool ReadOnly);

    [StructLayout(LayoutKind.Sequential)]
    public struct LOADED_IMAGE
    {
      public IntPtr ModuleName; // ptr to string
      public IntPtr hFile; // handle
      public IntPtr MappedAddress; // puchar?
      public IntPtr FileHeader; // PIMAGE_NT_HEADERS32
      public IntPtr LastRvaSection; // PIMAGE_SECTION_HEADER
      public UInt32 NumberOfSections;
      public IntPtr Sections; // PIMAGE_SECTION_HEADER
      public UInt32 Characteristics;
      public byte fSystemImage; // BOOLEAN
      public byte fDOSImage; // BOOLEAN
      public byte fReadOnly; // BOOLEAN
      public byte Version; // UCHAR
      public LIST_ENTRY Links; // LIST_ENTRY
      public UInt32 SizeOfImage;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct LIST_ENTRY
    {
      public IntPtr Flink; // LIST_ENTRY *
      public IntPtr Blink; // LIST_ENTRY *
    }

    [DllImport("Dbghelp.dll", SetLastError = true)]
    public static extern IntPtr ImageDirectoryEntryToDataEx(
      IntPtr Base,
      byte MappedAsImage,
      UInt16 DirectoryEntry,
      ref UInt32 Size,
      ref IntPtr FoundHeader); // PIMAGE_SECTION_HEADER *

    [StructLayout(LayoutKind.Sequential)]
    public struct IMAGE_SECTION_HEADER_NAME
    {
      public byte a,b,c,d,e,f,g,h;

      public override string ToString()
      {
        string meh = string.Empty + (char)a + (char)b + (char)c + (char)d + (char)e + (char)f + (char)g + (char)h;
        int zeroIndex = meh.IndexOf((char)0);
        if (zeroIndex >= 0)
        {
          meh = meh.Substring(0, zeroIndex);
        }
        return meh;
      }
    }

    public const UInt16 IMAGE_DIRECTORY_ENTRY_EXPORT = 0;

    [DllImport("Dbghelp.dll", SetLastError = true)]
    public static extern IntPtr ImageRvaToVa(
      IntPtr NtHeaders, // PIMAGE_NT_HEADERS
      IntPtr Base,
      UInt32 Rva,
      ref IntPtr LastRvaSection); // PIMAGE_SECTION_HEADER *

    [StructLayout(LayoutKind.Sequential)]
    public struct IMAGE_EXPORT_DIRECTORY
    {
      public UInt32 Characteristics;
      public UInt32 TimeDateStamp;
      public UInt16 MajorVersion;
      public UInt16 MinorVersion;
      public UInt32 Name;
      public UInt32 Base;
      public UInt32 NumberOfFunctions;
      public UInt32 NumberOfNames;
      public UInt32 AddressOfFunctions;     // RVA from base of image
      public UInt32 AddressOfNames;     // RVA from base of image
      public UInt32 AddressOfNameOrdinals;  // RVA from base of image
    }

    [DllImport("Imagehlp.dll", SetLastError = true)]
    public static extern bool UnMapAndLoad(
      ref LOADED_IMAGE LoadedImage);
  }
}
