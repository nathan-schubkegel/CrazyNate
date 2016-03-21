using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Pipes;
using System.IO;

namespace CrazyNateManaged
{
  public static class ManagedEntryPoint
  {
    public static string EntryTypeName { get { return "CrazyNateManaged.ManagedEntryPoint"; } }

    // alternative entry point
    public static string ShowMessageBoxMethodName { get { return "ShowMessageBox"; } }
    public static int ShowMessageBox(string message)
    {
      try
      {
        MessageBox.Show("ok, entered. " + message + " AppDomain: " + AppDomain.CurrentDomain.Id.ToString());
      }
      catch
      {
        // not sure what it would mean for this method to throw an exception 
        // that escapes into unmanaged code! App teardown perhaps?
      }
      return 1337;
    }

    // alternative entry point
    public static string WriteToPipeMethodName { get { return "WriteToPipe"; } }
    public static int WriteToPipe(string pipeName)
    {
      try
      {
        // write back on a Task so this method can return immediately
        Task t = new Task(() =>
        {
          try
          {
            using (NamedPipeClientStream clientPipe = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut))
            {
              clientPipe.Connect();
              // encode without writing ByteOrderMark
              UnicodeEncoding encoding = new UnicodeEncoding(false, false, true);
              using (StreamWriter writer = new StreamWriter(clientPipe, encoding))
              {
                // this message is checked by unit tests
                writer.Write("ok, entered. AppDomain: " + AppDomain.CurrentDomain.Id.ToString() + (char)0);
              }
              //clientPipe.WaitForPipeDrain();
            }
          }
          catch
          {
            // I guess keep eating exceptions
          }
        });
        t.Start();
      }
      catch
      {
        // not sure what it would mean for this method to throw an exception 
        // that escapes into unmanaged code! App teardown perhaps?
      }
      return 1337;
    }

    public static string EnterMethodName { get { return "Enter"; } }
    public static int Enter(string arg)
    {
      // This method doesn't really need to do anything on load
      return 1;
    }
  }
}
