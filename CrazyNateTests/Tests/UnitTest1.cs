using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CrazyNateManaged;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.Text;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace CrazyNateTests.Tests
{
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public void InjectIntoProcess_ByCopyingToApplicationFolder_Succeeds()
    {
      // make a new temp directory
      using (TempDir tempDir = new TempDir())
      {
        // copy "sandbox" exe (the one we're going to inject into) to that directory
        string sandbox451SourcePath = CrazyNateSandbox451.DependencyFetcher.GetPathToThisAssembly();
        string sandbox451DestPath = Path.Combine(tempDir.Path, Path.GetFileName(sandbox451SourcePath));
        File.Copy(sandbox451SourcePath, sandbox451DestPath, true);

        // start the sandbox process
        using (Process sandboxProcess = new Process() { StartInfo = new ProcessStartInfo(sandbox451DestPath) })
        using (TempProcess tempProcess = new TempProcess(sandboxProcess))
        {
          // create a named pipe to receive a message from the remote process
          using (TempPipeServer serverPipe = new TempPipeServer())
          {
            if (!sandboxProcess.WaitForInputIdle(5000))
            {
              throw new Exception("sandbox process did not enter idle state");
            }

            // inject my dlls into it
            DllInjector.DllInfo[] dlls = CrazyNateManaged.DependencyFetcher.GetDllInjectionInfo().ToArray();
            foreach (var dll in dlls)
            {
              // wait not more than 5 seconds for each dll to do its thing
              dll.WaitTimeMs = 5000;
            }
            var a = (DllInjector.ManagedDllInfo)dlls.Last();
            if (a.FileNameOrPath != CrazyNateManaged.DependencyFetcher.GetPathToThisAssembly())
            {
              throw new Exception("expected last assembly to be CrazyNateManaged.dll");
            }
            a.EntryTypeName = CrazyNateManaged.ManagedEntryPoint.EntryTypeName;
            a.EntryMethodName = CrazyNateManaged.ManagedEntryPoint.WriteToPipeMethodName;
            a.EntryArgument = serverPipe.Path;
            DllInjector.InjectIntoProcess(sandboxProcess.Id, dlls);

            // wait up to 5 seconds for the process to connect to the pipe
            Task task = Task.Factory.FromAsync(
              serverPipe.Stream.BeginWaitForConnection,
              serverPipe.Stream.EndWaitForConnection,
              null);
            if (!task.Wait(5000))
            {
              throw new Exception("sandbox process did not connect to the pipe");
            }
            // wait up to 5 seconds for the process to write a message to the pipe
            // that ends in a null terminator character
            List<byte> bytes = new List<byte>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int numAttempts = 0;
            while (bytes.Count % 2 != 0 || bytes.Count < 2 ||
              bytes[bytes.Count - 1] != 0 || bytes[bytes.Count - 2] != 0)
            {
              numAttempts++;
              if (stopwatch.ElapsedMilliseconds >= 5000 && numAttempts > 5)
              {
                throw new Exception("sanbox process did not write back fast enough");
              }
              byte[] buffer = new byte[1000];
              Task<int> read = serverPipe.Stream.ReadAsync(buffer, 0, 1000);
              if (!read.Wait(5000))
              {
                throw new Exception("sandbox process did not write back");
              }
              int numBytes = read.Result;
              for (int i = 0; i < numBytes; i++) bytes.Add(buffer[i]);
            }
            bytes.RemoveAt(bytes.Count - 2); // remove the null terminator
            bytes.RemoveAt(bytes.Count - 1);
            string message = Encoding.Unicode.GetString(bytes.ToArray());
            Assert.AreEqual("ok, entered. AppDomain: 1", message);
          }
        }
      }
    }
  }
}
