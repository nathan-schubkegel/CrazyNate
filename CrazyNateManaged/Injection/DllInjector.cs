using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Reflection;
using System.IO;

namespace CrazyNateManaged.Injection
{
  public static class DllInjector
  {
    public static void InjectIntoProcess(int processPid, IEnumerable<DllInfo> dlls)
    {
      List<DllInfo> dllsCopy = new List<DllInfo>(dlls);

      // Retrieve a HANDLE to the remote process (OpenProcess).
      IntPtr processHandle = GetHandleToTargetProcess(processPid);
      try
      {
        // get target process dir path
        string processDirPath = GetTargetProcessDirPath(processHandle);

        // require CrazyNate.dll to be in the list if there are any managed dlls
        // because that one enables managed DLL injection
        CrazyNateDllInfo crazyNateDll = CheckForCrazyNateDllInfo(dllsCopy);

        // copy DLLs to destination folder first, if appropriate
        CopyDlls(dllsCopy, processDirPath);

        // For each DLL, figure out whether file name or full path will be used to load it
        Dictionary<DllInfo, string> dllLoadNames = GetDllLoadNames(dllsCopy);

        IntPtr crazyNateLoadManagedDllRemoteProcAddress = IntPtr.Zero;
        if (crazyNateDll != null)
        {
          // load CrazyNate.dll in the remote process
          IntPtr remoteCrazyNateDllHModule = LoadUnmanagedDllIntoTargetProcess(
            processHandle, crazyNateDll, dllLoadNames[crazyNateDll]);

          // determine the offset address of the 'LaunchCrazyNateManaged' function in the remote mapped DLL
          // by comparing the locally loaded method address to local base dll address
          IntPtr localCrazyNateHModule = CrazyNateDll.GetCrazyNateHModule();
          IntPtr localLaunchCrazyNateManagedAddress = CrazyNateDll.GetLaunchCrazyNateManagedAddress();
          crazyNateLoadManagedDllRemoteProcAddress = new IntPtr(
            remoteCrazyNateDllHModule.ToInt64() +
            (localLaunchCrazyNateManagedAddress.ToInt64() - localCrazyNateHModule.ToInt64()));
        }
        
        foreach (DllInfo dll in dllsCopy.Where(dll2 => !(dll2 is CrazyNateDllInfo)))
        {
          if (dll is ManagedDllInfo)
          {
            ManagedDllInfo managedDll = (ManagedDllInfo)dll;
            LoadManagedDllInRemoteProcess(processHandle, managedDll, dllLoadNames[dll], crazyNateLoadManagedDllRemoteProcAddress);
          }
          else if (dll is UnmanagedDllInfo)
          {
            UnmanagedDllInfo unmanagedDll = (UnmanagedDllInfo)dll;
            IntPtr hRemoteDllHModule = LoadUnmanagedDllIntoTargetProcess(processHandle, dll, dllLoadNames[dll]);
            try
            {
              // only invoke an unmanaged proc if requested
              if (unmanagedDll.EntryProcName != null)
              {
                ExecuteUnmanagedDllProcInRemoteProcess(processHandle, unmanagedDll);
              }
            }
            finally
            {
              Win32.CloseHandle(hRemoteDllHModule);
            }
            throw new NotImplementedException("support for UnmanagedDllInfo");
          }
          else
          {
            throw new Exception("Unexpected dll type");
          }
        }
      }
      finally
      {
        Win32.CloseHandle(processHandle);
      }
    }

    private static void ExecuteUnmanagedDllProcInRemoteProcess(IntPtr processHandle, UnmanagedDllInfo unmanagedDll)
    {
      // allocate memory in the remote process for the proc name
      AllocatedMemory remoteMemory = AllocateMemoryInRemoteProcess(processHandle, new IntPtr(unmanagedDll.EntryProcName.Length + 1));
      try
      {
        // write the proc name in the remote process
        WriteToMemoryInRemoteProcess(processHandle, remoteMemory, unmanagedDll.EntryProcName);

        // find the address of the proc in the remote process
        throw new NotImplementedException("this will take some extra work...");
      }
      finally
      {
        // Free the memory allocated in Step #2 (VirtualFreeEx).
        Win32.VirtualFreeEx(processHandle, remoteMemory.Address, IntPtr.Zero, Win32.MEM_RELEASE);
      }
    }

    private static void LoadManagedDllInRemoteProcess(IntPtr processHandle, ManagedDllInfo managedDll, string dllLoadName, IntPtr crazyNateLoadManagedDllRemoteProcAddress)
    {
      // These parameters will be sent to the target process as 1 string
      UInt32 inputOutputBufferCharCount = CrazyNateDll.GetInputOutputBufferCharCount();
      StringBuilder dllInfo = new StringBuilder((int)inputOutputBufferCharCount);
      dllInfo.Append(dllLoadName);
      dllInfo.Append((char)0);
      dllInfo.Append(managedDll.EntryTypeName);
      dllInfo.Append((char)0);
      dllInfo.Append(managedDll.EntryMethodName);
      dllInfo.Append((char)0);
      dllInfo.Append(managedDll.EntryArgument);
      dllInfo.Append((char)0);
      while (dllInfo.Length < inputOutputBufferCharCount) dllInfo.Append((char)0);
      if (dllInfo.Length > inputOutputBufferCharCount)
      {
        throw new Exception("Unable to store inputs (" + dllInfo.Length + " chars) in buffer (" + inputOutputBufferCharCount + " chars)");
      }

      // Allocate memory for the input/output buffer in the remote process
      IntPtr remoteMemorySize = new IntPtr(inputOutputBufferCharCount * sizeof(char));
      AllocatedMemory remoteMemory = AllocateMemoryInRemoteProcess(processHandle, remoteMemorySize);
      try
      {
        // Write the input args to the remote process memory
        WriteToMemoryInRemoteProcess(processHandle, remoteMemory, dllInfo.ToString());

        // Invoke the CrazyNate.dll method that will load the managed dll in the remote process
        IntPtr hRemoteThread = CreateRemoteThread(processHandle, crazyNateLoadManagedDllRemoteProcAddress, remoteMemory.Address);
        try
        {
          // Wait for it to return, so we know whether it thinks it succeeded
          Int32 remoteLaunchCrazyNateManagedResult = WaitForRemoteThreadResult(hRemoteThread, managedDll.WaitTimeMs);

          // if not success
          if (remoteLaunchCrazyNateManagedResult != 1)
          {
            // read remote process memory for error message about why it failed
            string errorMessage = ReadRemoteProcessMemory(processHandle, remoteMemory)
              ?? ("(failed to get error message from target process: " + Win32.GetLastErrorMessage() + ")");

            throw new Exception("Failed to load managed dll in target process: " + errorMessage);
          }
        }
        finally
        {
          Win32.CloseHandle(hRemoteThread);
        }
      }
      finally
      {
        // Free the memory allocated in Step #2 (VirtualFreeEx).
        Win32.VirtualFreeEx(processHandle, remoteMemory.Address, IntPtr.Zero, Win32.MEM_RELEASE);
      }
    }

    private static IntPtr GetHandleToTargetProcess(int processPid)
    {
      IntPtr processHandle = Win32.OpenProcess(
        Win32.PROCESS_CREATE_THREAD | Win32.PROCESS_QUERY_INFORMATION | Win32.PROCESS_VM_OPERATION | Win32.PROCESS_VM_WRITE | Win32.PROCESS_VM_READ,
        false, // inherithandles = no
        processPid);

      if (processHandle == IntPtr.Zero)
      {
        throw new Exception("Unable to open handle to target process: " + Win32.GetLastErrorMessage());
      }
      return processHandle;
    }

    private static IntPtr LoadUnmanagedDllIntoTargetProcess(IntPtr processHandle, DllInfo dll, string dllLoadName)
    {
      // Allocate memory for the argument to LoadLibraryW (it's the dll name/path) in the remote process
      IntPtr remoteMemorySize = new IntPtr((dllLoadName.Length + 1) * sizeof(char));
      AllocatedMemory remoteMemory = AllocateMemoryInRemoteProcess(processHandle, remoteMemorySize);
      try
      {
        // Write the DLL name/path to the allocated memory in the remote process
        WriteToMemoryInRemoteProcess(processHandle, remoteMemory, dllLoadName);

        // Get the address of LoadLibraryW() in Kernel32.dll in the local process
        // (LoadLibraryW() lives at the same address in the remote process
        //  because Kernel32 is mapped to the same location in all processes! Thanks win32!)
        IntPtr remoteProcAddress = Win32.GetProcAddress(Win32.GetModuleHandleW("Kernel32"), "LoadLibraryW");
        if (remoteProcAddress == IntPtr.Zero)
        {
          throw new Exception("Unable to get address of LoadLibraryW()");
        }

        // Load the DLL in the remote process via CreateRemoteThread & LoadLibraryW.
        IntPtr hRemoteThread = CreateRemoteThread(processHandle, remoteProcAddress, remoteMemory.Address);
        Int32 remoteLoadLibraryResult = 0;
        try
        {
          // Wait the remote thread to finish, which is when LoadLibraryW() returns.
          // Note that LoadLibraryW() executes the DLL's DllMain function (called with reason DLL_PROCESS_ATTACH).
          // Also retrieve the exit code of the remote thread, which is the value returned by LoadLibraryW, 
          // which is the base address (HMODULE) of our mapped DLL in the remote process (if running as x86).
          remoteLoadLibraryResult = WaitForRemoteThreadResult(hRemoteThread, dll.WaitTimeMs);
          if (remoteLoadLibraryResult == 0)
          {
            throw new Exception("LoadLibraryW failed in target process");
            // JOKE: too bad I can't fire up another thread to call GetLastError in the target process! lol!
          }
        }
        finally
        {
          Win32.CloseHandle(hRemoteThread);
        }

        return GetRemoteDllHModule(remoteLoadLibraryResult);
      }
      finally
      {
        // Free the memory allocated in Step #2 (VirtualFreeEx).
        Win32.VirtualFreeEx(processHandle, remoteMemory.Address, IntPtr.Zero, Win32.MEM_RELEASE);
      }
    }

    private static IntPtr GetRemoteDllHModule(Int32 remoteLoadLibraryResult)
    {
      IntPtr remoteCrazyNateHModule;

      // If running as x86
      if (IntPtr.Size == 4)
      {
        // the result of LoadLibrary is the remote HModule
        remoteCrazyNateHModule = new IntPtr(remoteLoadLibraryResult);
      }
      else // running as x64
      {
        throw new NotImplementedException("x64 injection not yet supported");

        // this part not implemented/tested yet
#if false
                // Enumerate to find the HMODULE of CrazyNate.dll in the remote process
                IntPtr[] remoteHModules = new IntPtr[5000]; // way more than anyone ever needs
                UInt32 remoteHModuleRequiredBytes = 0;
                if (!EnumProcessModules(processHandle, 
                  ref remoteHModules, 
                  (UInt32)(remoteHModules.Length * Marshal.SizeOf(typeof(IntPtr))), 
                  ref remoteHModuleRequiredBytes))
                {
                  throw new Exception(Marshal.GetLastWin32Error(), "Unable to enumerate dlls in target process");
                }
#endif
      }
      return remoteCrazyNateHModule;
    }

    private static int WaitForRemoteThreadResult(IntPtr hRemoteThread, UInt32 waitTimeMs)
    {
      int remoteThreadReturnValue = 0;

      // Wait until the remote thread terminates, or the user-provided timeout expires
      if (Win32.WAIT_OBJECT_0 != Win32.WaitForSingleObject(hRemoteThread, waitTimeMs))
      {
        throw new Exception("Failed to wait for thread in target process to finish: " + Win32.GetLastErrorMessage());
      }

      // Retrieve the exit code of the remote thread (GetExitCodeThread).
      if (!Win32.GetExitCodeThread(hRemoteThread, ref remoteThreadReturnValue))
      {
        throw new Exception("Unable to get exit code of thread in target process: " + Win32.GetLastErrorMessage());
      }

      return remoteThreadReturnValue;
    }

    private static IntPtr CreateRemoteThread(IntPtr processHandle, IntPtr remoteProcAddress, IntPtr remoteProcParameter)
    {
      IntPtr hRemoteThread;
      int remoteThreadId;

      remoteThreadId = 0;
      hRemoteThread = Win32.CreateRemoteThread(
        processHandle,
        IntPtr.Zero, // thread attributes (zero means default)
        IntPtr.Zero, // stack size (zero means default)
        remoteProcAddress,
        remoteProcParameter, // procedure parameter
        0, // creation flags (zero means thread runs immediately)
        ref remoteThreadId);

      if (hRemoteThread == IntPtr.Zero)
      {
        throw new Exception("Unable to create thread in target process: " + Win32.GetLastErrorMessage());
      }

      return hRemoteThread;
    }

    private class AllocatedMemory
    {
      public IntPtr Address;
      public IntPtr Length;

      public override string ToString()
      {
        return "Address: " + Address.ToString() + ", Length: " + Length.ToString();
      }
    }

    private static AllocatedMemory AllocateMemoryInRemoteProcess(IntPtr processHandle, IntPtr remoteMemorySize)
    {
      AllocatedMemory remoteMemory = new AllocatedMemory();

      // Allocate memory for the DLL name in the remote process (VirtualAllocEx).
      remoteMemory.Address = Win32.VirtualAllocEx(
        processHandle,
        IntPtr.Zero, // let the function determine where to allocate the memory
        remoteMemorySize, // size
        Win32.MEM_COMMIT,
        Win32.PAGE_READWRITE);

      if (remoteMemory.Address == IntPtr.Zero)
      {
        throw new Exception("Unable to allocate memory in target process: " + Win32.GetLastErrorMessage());
      }

      remoteMemory.Length = remoteMemorySize;

      return remoteMemory;
    }

    private static void WriteToMemoryInRemoteProcess(IntPtr processHandle, AllocatedMemory remoteMemory, string data)
    {
      StringBuilder buffer = new StringBuilder();
      buffer.Append(data);

      // Ensure that a null string terminator character is at the end of the buffer
      // I think StringBuilder marshals this way automatically, but I want to make sure
      // it's included in the remote memory size check
      if (buffer.Length == 0 || buffer[buffer.Length - 1] != (char)0)
      {
        buffer.Append((char)0);
      }

      IntPtr numberOfBytesToWrite = new IntPtr(buffer.Length * sizeof(char));

      if (numberOfBytesToWrite.ToInt64() > remoteMemory.Length.ToInt64())
      {
        throw new Exception("Insufficient space");
      }

      IntPtr numberOfBytesWritten = IntPtr.Zero;
      if (!Win32.WriteProcessMemory(
        processHandle,
        remoteMemory.Address,
        buffer,
        numberOfBytesToWrite,
        ref numberOfBytesWritten))
      {
        throw new Exception("Unable to write memory in target process: " + Win32.GetLastErrorMessage());
      }

      if (numberOfBytesWritten != numberOfBytesToWrite)
      {
        throw new Exception("Unable to write correct number of bytes in target process memory (" + numberOfBytesWritten.ToInt64() + " written, " + numberOfBytesToWrite.ToInt64() + " attempted)");
      }
    }

    private static string ReadRemoteProcessMemory(IntPtr processHandle, AllocatedMemory remoteMemory)
    {
      string message;
      StringBuilder messageBuilder = new StringBuilder((remoteMemory.Length.ToInt32() + 1) / sizeof(char));
      IntPtr bytesAcquired = IntPtr.Zero;
      if (Win32.ReadProcessMemory(processHandle, remoteMemory.Address, messageBuilder, remoteMemory.Length, ref bytesAcquired))
      {
        // bytesAcquired should never mismatch the requested amount...
        // but let's tolerate a mismatch anyway
        if (bytesAcquired.ToInt32() < messageBuilder.Length)
        {
          messageBuilder.Length = bytesAcquired.ToInt32();
        }

        message = messageBuilder.ToString();

        // it seems marshalling sets stringbuilder length to null terminator automatically
        // but this code helps make sure
        int nullTerminatorIndex = message.IndexOf((char)0);
        if (nullTerminatorIndex >= 0)
        {
          message = message.Substring(0, nullTerminatorIndex);
        }
      }
      else
      {
        message = null;
      }
      return message;
    }

    private static string GetTargetProcessDirPath(IntPtr processHandle)
    {
      string processExePath = Win32.QueryFullProcessImageNameW(processHandle);
      if (processExePath == null)
      {
        throw new Exception("Unable to get exe path of target process: " + Win32.GetLastErrorMessage());
      }

      string processDirPath = Path.GetDirectoryName(processExePath);
      if (processDirPath == null)
      {
        throw new Exception("Unable to get dir path of target process: " + Win32.GetLastErrorMessage());
      }

      return processDirPath;
    }

    private static CrazyNateDllInfo CheckForCrazyNateDllInfo(List<DllInfo> dllsCopy)
    {
      CrazyNateDllInfo[] crazyNateDlls = dllsCopy.Where(dll => dll is CrazyNateDllInfo).Cast<CrazyNateDllInfo>().ToArray();
      CrazyNateDllInfo crazyNateDll = crazyNateDlls.FirstOrDefault();
      if (dllsCopy.Any(dll => dll is ManagedDllInfo))
      {
        if (crazyNateDll == null || crazyNateDlls.Length > 1)
        {
          throw new Exception("Must provide one CrazyNateDllInfo when injecting managed dlls");
        }
        if (crazyNateDll.LoadBehavior == DllLoadBehavior.None)
        {
          throw new Exception("When providing CrazyNateDllInfo, must use a LoadBehavior");
        }
      }
      else
      {
        if (crazyNateDll != null)
        {
          throw new Exception("Must provide zero CrazyNateDllInfo when not injecting managed dlls");
        }
      }

      return crazyNateDll;
    }

    private static void CopyDlls(List<DllInfo> dllsCopy, string processDirPath)
    {
      foreach (DllInfo dll in dllsCopy)
      {
        string destDirPath;
        switch (dll.CopyBehavior)
        {
          case DllCopyBehavior.CopyToFolderUnderTargetProcessExeFolder:
            destDirPath = Path.Combine(processDirPath, "CrazyNate");
            break;

          case DllCopyBehavior.CopyToTargetProcessExeFolder:
            destDirPath = processDirPath;
            break;

          case DllCopyBehavior.None:
            destDirPath = null;
            break;

          default:
            throw new Exception("Unexpected CopyBehavior");
        }

        if (destDirPath != null)
        {
          string dllFileName = Path.GetFileName(dll.FileNameOrPath);
          if (dllFileName == null)
          {
            throw new Exception("Unable to get dll file name: " + dll.FileNameOrPath);
          }

          string destFilePath = Path.Combine(destDirPath, dllFileName);
          if (destDirPath == null)
          {
            throw new Exception("Unable to determine destination path for dll: " + dll.FileNameOrPath);
          }

          Directory.CreateDirectory(destDirPath);

          string sourceFilePath = Path.GetFullPath(dll.FileNameOrPath);
          if (sourceFilePath == null)
          {
            throw new Exception("Unable to determine full source path for dll: " + dll.FileNameOrPath);
          }

          try
          {
            File.Copy(sourceFilePath, destFilePath, true);
          }
          catch (Exception ex)
          {
            // on failure, it's usually because the DLL is already there and 
            // the process already has it loaded. Let's tolerate that.
            if (!File.Exists(destFilePath))
            {
              throw new AggregateException("Unable to copy DLL to destination folder", ex);
            }
          }
        }
      }
    }

    private static Dictionary<DllInfo, string> GetDllLoadNames(List<DllInfo> dllsCopy)
    {
      Dictionary<DllInfo, string> dllLoadNames = dllsCopy.ToDictionary(
        dll => dll, dll =>
        {
          switch (dll.LoadBehavior)
          {
            case DllLoadBehavior.None:
              return null;

            case DllLoadBehavior.LoadFromFullPath:
              string fullPath = Path.GetFullPath(dll.FileNameOrPath);
              if (fullPath == null)
              {
                throw new Exception("Unable to determine full load path for dll: " + dll.FileNameOrPath);
              }
              return fullPath;

            case DllLoadBehavior.LoadFromFileName:
              string fileName = Path.GetFileName(dll.FileNameOrPath);
              if (fileName == null)
              {
                throw new Exception("Unable to determine file name for dll: " + dll.FileNameOrPath);
              }
              return fileName;

            case DllLoadBehavior.LoadAsProvided:
              return dll.FileNameOrPath;

            default:
              throw new Exception("unexpected LoadBehavior");
          }
        });

      return dllLoadNames;
    }
  }
}