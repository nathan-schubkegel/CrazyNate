using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyNateTests
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
