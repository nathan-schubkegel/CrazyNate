using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Pipes;
using System.Threading.Tasks;
using System.Threading;

namespace CrazyNateManaged
{
  public class Communicator : IDisposable
  {
    private readonly object _lock = new object();
    private bool _disposed;
    private PipeStream _stream;
    private Queue<byte> _readBuffer = new Queue<byte>();
    private bool _reading;

    private Communicator()
    {
    }

    public static Communicator CreateHost()
    {
      Communicator c = new Communicator();

      string exMessage = null;
      for (int i = 0; i < 5; i++)
      {
        try
        {
          c._stream = new NamedPipeServerStream(@"\\.\pipe\CrazyNate-" + Guid.NewGuid().ToString("D"));
          break;
        }
        catch (Exception ex)
        {
          c._stream = null;
          if (i == 4)
          {
            exMessage = ex.GetType().Name + ": " + ex.Message;
          }
        }
      }
      if (c._stream == null)
      {
        throw new Exception(exMessage);
      }

      return c;
    }

    public static Communicator CreateClient(string name)
    {
      Communicator c = new Communicator();
      c._stream = new NamedPipeClientStream(name);
      return c;
    }

    public void Dispose()
    {
      Dispose(true);
    }

    private void Dispose(bool disposing)
    {
      _disposed = true;
      if (disposing)
      {
        if (_stream != null)
        {
          _stream.Dispose();
          _stream = null;
        }
      }
    }

    ~Communicator()
    {
      Dispose(false);
    }

    public async Task<string> ReadMessageAsync()
    {
      Task t = new Task(() =>
        {
          lock (_lock)
          {
            while (_reading && !_disposed)
            {
              Monitor.Wait(_lock);
            }
            if (_disposed) return;
            _reading = true;
          }
        });

      Task t2 = t.ContinueWith(unused =>
        {
          // a message starts with int32 indicating byte count after that int32
          
        });
      Monitor.Enter(_readingLock);
      try
      {
        while (_reading && !_disposed)
        {
          Monitor.Wait(_readingLock);
        }
        if (_disposed)
        {
          throw new ObjectDisposedException("Communicator");
        }
        _reading = true;
      }
      finally
      {
        Monitor.Exit(_readingLock);
      }

      //await ReadUntil(4, () => _readBuffer.Count >= 4, () => _readBuffer.Dequeue)

    }

    private void ReadUntil(int bufferSize, Func<bool> condition)
    {
      Monitor.Enter(_lock);
      try
      {
        byte[] buffer = new byte[bufferSize];
        while (!_disposed && !condition())
        {
          int numBytesAcquired;
          Monitor.Exit(_lock);
          try
          {
            // TODO: assuming this is non-blocking... if it's blocking, then we use await with cancellation token
            numBytesAcquired = _stream.Read(buffer, 0, bufferSize);
          }
          finally
          {
            Monitor.Enter(_lock);
          }
          for (int i = 0; i < numBytesAcquired; i++)
          {
            _readBuffer.Enqueue(buffer[i]);
          }
        }

        if (!_disposed)
        {
          completion();
        }
      }
      finally
      {
        Monitor.Exit(_readBuffer);
      }
    }
  }
}
