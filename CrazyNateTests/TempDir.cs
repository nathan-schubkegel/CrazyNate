using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CrazyNateTests
{
  public class TempDir : IDisposable
  {
    public string Path { get; private set; }

    public TempDir()
    {
      int attemptsLeft = 5; ;
      string tempPath = System.IO.Path.GetTempPath();
      string dirPath;
      while (true)
      {
        if (attemptsLeft == 0) throw new Exception("Failed to create temp directory");
        attemptsLeft--;

        string tempName = "CrazyNateTempDir_" + Guid.NewGuid().ToString("D");
        dirPath = System.IO.Path.Combine(tempPath, tempName);
        try
        {
          if (File.Exists(dirPath) || Directory.Exists(dirPath))
          {
            // try again with new dir name
            continue;
          }
          else
          {
            Directory.CreateDirectory(dirPath);
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

      Path = dirPath;
    }

    public void Dispose()
    {
      if (Path != null)
      {
        try
        {
          // first set directory and all contents to "not read only"
          ReadOnlyAttribute.TryClearAllRecursive(Path);

          // then attempt to remove directory and all contents
          Directory.Delete(Path, true);

          // wipe _dirPath when that's finally successful
          // (maybe if it fails during explicit disposal, it'll work better during finalization)
          Path = null;
        }
        catch
        {
        }
      }
    }

    ~TempDir()
    {
      Dispose();
    }

    public override string ToString()
    {
      return Path;
    }
  }
}
