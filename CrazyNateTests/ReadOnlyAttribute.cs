using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CrazyNateTests
{
  class ReadOnlyAttribute
  {
    public static void TryClearAllRecursive(string path)
    {
      try
      {
        if (File.Exists(path))
        {
          var fi = new FileInfo(path);
          if (fi.Attributes.HasFlag(FileAttributes.ReadOnly))
          {
            fi.Attributes &= ~FileAttributes.ReadOnly;
          }
        }
        else if (Directory.Exists(path))
        {
          var di = new DirectoryInfo(path);
          if (di.Attributes.HasFlag(FileAttributes.ReadOnly))
          {
            di.Attributes &= ~FileAttributes.ReadOnly;
          }
          foreach (FileSystemInfo subPath in di.EnumerateFileSystemInfos())
          {
            TryClearAllRecursive(subPath.FullName);
          }
        }
      }
      catch
      {
      }
    }
  }
}
