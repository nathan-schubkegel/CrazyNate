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
    public static List<string> GetExports(string dllFilePath)
    {
      // This was stolen from http://stackoverflow.com/questions/4353116/listing-the-exported-functions-of-a-dll

      List<string> results = new List<string>();

      ImageHlp.LOADED_IMAGE LoadedImage = new ImageHlp.LOADED_IMAGE();
      if (ImageHlp.MapAndLoad(
        Path.GetFileName(dllFilePath), //dllFilePath, 
        Path.GetDirectoryName(dllFilePath),
        ref LoadedImage, true, true))
      {
        try
        {
          IntPtr foundHeader = IntPtr.Zero;
          UInt32 cDirSize = 0;
          IntPtr ImageExportDirectoryPtr = ImageHlp.ImageDirectoryEntryToDataEx(
            LoadedImage.MappedAddress, 0, ImageHlp.IMAGE_DIRECTORY_ENTRY_EXPORT, ref cDirSize, ref foundHeader);

          if (ImageExportDirectoryPtr != IntPtr.Zero)
          {
            ImageHlp.IMAGE_EXPORT_DIRECTORY ImageExportDirectory = (ImageHlp.IMAGE_EXPORT_DIRECTORY)
              Marshal.PtrToStructure(ImageExportDirectoryPtr, typeof(ImageHlp.IMAGE_EXPORT_DIRECTORY));

            IntPtr lastRvaSection = IntPtr.Zero;
            IntPtr dNameRVAs = ImageHlp.ImageRvaToVa(
              LoadedImage.FileHeader,
              LoadedImage.MappedAddress,
              ImageExportDirectory.AddressOfNames, 
              ref lastRvaSection);

            for (int i = 0; i < ImageExportDirectory.NumberOfNames; i++)
            {
              UInt32 dNameRVA = (UInt32)Marshal.ReadInt32(new IntPtr(dNameRVAs.ToInt64() + i * sizeof(UInt32)));
              IntPtr lastRvaSection2 = IntPtr.Zero;
              IntPtr sName = ImageHlp.ImageRvaToVa(
                LoadedImage.FileHeader,
                LoadedImage.MappedAddress,
                dNameRVA, 
                ref lastRvaSection2);

              string sName2 = Marshal.PtrToStringAnsi(sName);

              results.Add(sName2);
            }
          }
        }
        finally
        {
          ImageHlp.UnMapAndLoad(ref LoadedImage);
        }
      }

      return results;
    }
  }
}
