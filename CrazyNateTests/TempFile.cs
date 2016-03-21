using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CrazyNateTests
{
  public class TempFile : IDisposable
  {
    public string Path { get; private set; }

    public TempFile()
    {
      int attemptsLeft = 5; ;
      string tempPath = System.IO.Path.GetTempPath();
      string filePath;
      while (true)
      {
        if (attemptsLeft == 0) throw new Exception("Failed to create temp file");
        attemptsLeft--;

        string tempName = "CrazyNateTempFile_" + Guid.NewGuid().ToString("D");
        filePath = System.IO.Path.Combine(tempPath, tempName);
        try
        {
          if (File.Exists(filePath) || Directory.Exists(filePath))
          {
            // try again with new dir name
            continue;
          }
          else
          {
            File.WriteAllBytes(filePath, new byte[0]);
          }
        }
        catch
        {
          // eat the exception and try again with new dir name
          continue;
        }

        // yay suceeded
        break;
      }

      Path = filePath;
    }

    public void Dispose()
    {
      if (Path != null)
      {
        try
        {
          // first set file to "not read only"
          ReadOnlyAttribute.TryClearAllRecursive(Path);

          // then attempt to remove file and all contents
          File.Delete(Path);

          // wipe _filePath when that's finally successful
          // (maybe if it fails during explicit disposal, it'll work better during finalization)
          Path = null;
        }
        catch
        {
        }
      }
    }

    ~TempFile()
    {
      Dispose();
    }

    public override string ToString()
    {
      return Path;
    }
  }
}
