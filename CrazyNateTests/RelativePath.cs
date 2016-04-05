using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CrazyNateTests
{
  public static class RelativePath
  {

    /// <summary>
    /// Creates a relative path from one file or folder to another.
    /// code from http://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path
    /// </summary>
    /// <param name="baseDirPath">Contains the directory that defines the start of the relative path.</param>
    /// <param name="absolutePath">Contains the path that defines the endpoint of the relative path.</param>
    /// <returns>The relative path from <c>basePath</c> to <c>absolutePath</c>.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="UriFormatException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static string FromAbsolutePath(string baseDirPath, string absolutePath)
    {
      if (String.IsNullOrEmpty(baseDirPath)) throw new ArgumentNullException("fromPath");
      if (String.IsNullOrEmpty(absolutePath)) throw new ArgumentNullException("toPath");

      // basePath must end with a slash, else this method behaves wrong
      if (!baseDirPath.EndsWith(Path.DirectorySeparatorChar.ToString())) throw new ArgumentException("baseDirPath must end in slash");

      Uri fromUri = new Uri(baseDirPath);
      Uri toUri = new Uri(absolutePath);

      if (fromUri.Scheme != toUri.Scheme) { throw new InvalidOperationException("path can't be made relative"); }

      Uri relativeUri = fromUri.MakeRelativeUri(toUri);
      string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

      if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
      {
        relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
      }

      return relativePath;
    }

    /*
    ///<summary>
    ///This should handle few scenarios like
    ///uri and potential escaped characters in it, like
    ///  file:///C:/Test%20Project.exe -> C:\TEST PROJECT.EXE
    ///path segments specified by dots to denote current or parent directory
    ///  c:\aaa\bbb\..\ccc -> C:\AAA\CCC
    ///tilde shortened (long) paths
    ///  C:\Progra~1\ -> C:\PROGRAM FILES
    ///inconsistent directory delimiter character
    ///  C:/Documents\abc.txt -> C:\DOCUMENTS\ABC.TXT
    ///Other than those, it can ignore case, trailing \ directory delimiter character etc.
    ///from http://stackoverflow.com/questions/1266674/how-can-one-get-an-absolute-or-normalized-file-path-in-net/21058121#21058121
    ///</summary>
    private static string NormalizePath(string path)
    {
      return Path.GetFullPath(new Uri(path).LocalPath)
                 .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                 .ToUpperInvariant();
    }

    public static string FromAbsolutePath(string fromDir, string toPath)
    {
      fromDir = NormalizePath(fromDir);
      toPath = NormalizePath(toPath);
      
      if (!Path.IsPathRooted(fromDir)) throw new ArgumentException("fromDir must be an absolute path");
      if (!Path.IsPathRooted(toPath)) throw new ArgumentException("toPath must be an absolute path");
      
      string fromRoot = Path.GetPathRoot(fromDir);
      string toRoot = Path.GetPathRoot(toPath);
      if (fromRoot != toRoot) throw new ArgumentException("fromDir and toPath must have same root");

      List<string> fromParts = new List<string>();
      List<string> toParts = new List<string>();

      GetPathParts(fromDir, fromParts);
      GetPathParts(toPath, toParts);

      List<string> commonParts = new List<string>();
      for (int i = 0; i < fromParts.Count && i < toParts.Count; i++)
      {
        if (fromParts[i] == toParts[i])
        {
          commonParts.Add(fromParts[i]);
        }
        else
        {
          break;
        }
      }

      if (commonParts.Count == 0) throw new ArgumentException("no common parts! (probably an internal error)");
      string baseDir = Path.Combine(commonParts.ToArray());


    }

    private static void GetPathParts(string path, List<string> parts)
    {
      string root = Path.GetPathRoot(path);
      string dir = Path.GetDirectoryName(path);
      if (dir == null || root == null) throw new ArgumentException("can't get path parts");
      if (root == dir)
      {
        parts.Add(root);
      }
      else
      {
        string name = Path.GetFileName(path);
        if (name == null) throw new ArgumentException("can't get path parts");
        GetPathParts(dir, parts);
        parts.Add(name);
      }
    }
     * */
  }
}
