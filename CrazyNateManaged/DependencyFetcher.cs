using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyNateManaged
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

    public static IEnumerable<string> GetPathsToThisAssemblyAndDependencies()
    {
      yield return CrazyNateDll.GetAssemblyPath();
      yield return CrazyNateSharpDisasm.DependencyFetcher.GetPathToThisAssembly();
      yield return GetPathToThisAssembly();
    }

    public static IEnumerable<DllInjector.DllInfo> GetDllInjectionInfo()
    {
      // return CrazyNate.dll
      yield return new DllInjector.CrazyNateDllInfo();

      // return CrazyNateSharpDisasm.dll
      yield return new DllInjector.ManagedDllInfo
        {
          FileNameOrPath = CrazyNateSharpDisasm.DependencyFetcher.GetPathToThisAssembly(),
          EntryTypeName = CrazyNateSharpDisasm.ManagedEntryPoint.EntryTypeName,
          EntryMethodName = CrazyNateSharpDisasm.ManagedEntryPoint.EntryMethodName,
          EntryArgument = CrazyNateSharpDisasm.ManagedEntryPoint.EntryArgument,
        };

      // return CrazyNateManaged.dll
      yield return new DllInjector.ManagedDllInfo
        {
          FileNameOrPath = DependencyFetcher.GetPathToThisAssembly(),
          EntryTypeName = ManagedEntryPoint.EntryTypeName,
          EntryMethodName = ManagedEntryPoint.EnterMethodName,
          EntryArgument = "hello",
        };
    }
  }
}
