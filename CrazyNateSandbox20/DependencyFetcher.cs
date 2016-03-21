using System;
using System.Collections.Generic;
using System.Text;

namespace CrazyNateSandbox20
{
  public static class DependencyFetcher
  {
    public static string GetPathToThisAssembly()
    {
      // Get the full path of this dll
      string thisDllUri = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
      string thisDllPath = (new Uri(thisDllUri)).LocalPath;
      return thisDllPath;
    }
  }
}
