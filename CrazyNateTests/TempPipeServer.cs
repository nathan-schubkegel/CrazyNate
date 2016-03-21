using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;

namespace CrazyNateTests
{
  public class TempPipeServer : IDisposable
  {
    public string Path { get; private set; }

    public NamedPipeServerStream Stream { get; private set; }

    public TempPipeServer()
    {
      int attemptsLeft = 5; ;
      string pipePath;
      NamedPipeServerStream pipeStream;
      while (true)
      {
        if (attemptsLeft == 0) throw new Exception("Failed to create temp pipe");
        attemptsLeft--;

        pipePath = @"\\.\pipe\CrazyNateTempPipe_" + Guid.NewGuid().ToString("D");

        try
        {
          pipeStream = new NamedPipeServerStream(pipePath, PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
        }
        catch
        {
          // eat the exception and try again with new pipe name
          continue;
        }

        // yay suceeded
        break;
      }

      Path = pipePath;
      Stream = pipeStream;
    }

    public void Dispose()
    {
      if (Stream != null)
      {
        try
        {
          Stream.Dispose();
        }
        catch
        {
        }
        Stream = null;
      }
    }

    ~TempPipeServer()
    {
      Dispose();
    }

    public override string ToString()
    {
      return Path;
    }
  }
}
