﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Reflection;
using System.IO;

namespace CrazyNateManaged
{
  public static class DllInjector
  {
    public static void InjectIntoProcess(int processPid)
    {
      // Retrieve a HANDLE to the remote process (OpenProcess).
      IntPtr processHandle = Win32.OpenProcess(
        Win32.PROCESS_CREATE_THREAD | Win32.PROCESS_QUERY_INFORMATION | Win32.PROCESS_VM_OPERATION | Win32.PROCESS_VM_WRITE | Win32.PROCESS_VM_READ,
        false, // inherithandles = no
        processPid);

      if (processHandle == IntPtr.Zero)
      {
        throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to open handle to target process");
      }

      try
      {
        // Copy all our DLLs to the same directory as the exe
        // (because theoretically that's where they're easiest to load from)
        CopyDllsToProcessDirectory(processHandle);

        // Allocate memory for the DLL name in the remote process (VirtualAllocEx).
        // (allocate alot, since we'll reuse this memory later for reporting error messages back from CrazyNate.dll)
        UInt32 requestedCharsInErrorMessageBuffer = CrazyNateDll.GetExpectedErrorMessageBufferSize();
        IntPtr remoteMemorySize = new IntPtr(Math.Max(Win32.MAX_PATH, requestedCharsInErrorMessageBuffer) * sizeof(char));
        IntPtr pRemoteMemory = Win32.VirtualAllocEx(
          processHandle,
          IntPtr.Zero, // let the function determine where to allocate the memory
          remoteMemorySize, // size
          Win32.MEM_COMMIT,
          Win32.PAGE_READWRITE);

        if (pRemoteMemory == IntPtr.Zero)
        {
          throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to allocate memory in target process");
        }

        try
        {
          // Write the DLL name (without path) to the allocated memory (WriteProcessMemory).
          byte[] dllNameBytes = Encoding.Unicode.GetBytes("CrazyNate.dll" + (char)0);

          if (dllNameBytes.Length > remoteMemorySize.ToInt32())
          {
            throw new Win32Exception("Insufficient memory allocated in target process for dll name");
          }

          IntPtr numberOfBytesWritten = IntPtr.Zero;
          if (!Win32.WriteProcessMemory(
            processHandle,
            pRemoteMemory,
            dllNameBytes,
            new IntPtr(dllNameBytes.Length),
            ref numberOfBytesWritten))
          {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to write memory in target process");
          }

          // Map your DLL to the remote process via CreateRemoteThread & LoadLibrary.
          // (note: this technically gets a local proc address, but it works in the remote process
          //        because Kernel32 is mapped to the same location in all processes. Thanks win32)
          IntPtr remoteProcAddress = Win32.GetProcAddress(Win32.GetModuleHandleW("Kernel32"), "LoadLibraryW");
          int remoteThreadId = 0;
          IntPtr hRemoteThread = Win32.CreateRemoteThread(
            processHandle,
            IntPtr.Zero, // thread attributes (zero means default)
            IntPtr.Zero, // stack size (zero means default)
            remoteProcAddress,
            pRemoteMemory, // procedure parameter
            0, // creation flags (zero means thread runs immediately)
            ref remoteThreadId);

          if (hRemoteThread == IntPtr.Zero)
          {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to create thread in target process");
          }

          Int32 remoteLoadLibraryResult = 0;
          try
          {
            // Wait until the remote thread terminates (WaitForSingleObject);
            // this is until the call to LoadLibrary returns. Put another way, the
            // thread will terminate as soon as our DllMain (called with reason DLL_PROCESS_ATTACH) returns.
            if (Win32.WAIT_OBJECT_0 != Win32.WaitForSingleObject(hRemoteThread, Win32.INFINITE))
            {
              throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to wait for thread in target process to finish");
            }

            // Retrieve the exit code of the remote thread (GetExitCodeThread).
            // Note that this is the value returned by LoadLibrary, thus the base address (HMODULE) of our mapped DLL.
            if (!Win32.GetExitCodeThread(hRemoteThread, ref remoteLoadLibraryResult))
            {
              throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to get exit code of thread in target process");
            }

            if (remoteLoadLibraryResult == 0)
            {
              throw new Win32Exception("LoadLibraryW failed in target process");
              // JOKE: too bad I can't fire up another thread to call GetLastError in the target process! lol!
            }
          }
          finally
          {
            Win32.CloseHandle(hRemoteThread);
          }

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
              throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to enumerate dlls in target process");
            }
#endif
          }

          try
          {
            // determine the offset address of the 'LaunchCrazyNateManaged' function in the remote mapped DLL
            // by comparing locally loaded method address to local base dll address
            IntPtr localCrazyNateHModule = CrazyNateDll.GetCrazyNateHModule();
            IntPtr localLaunchCrazyNateManagedAddress = CrazyNateDll.GetLaunchCrazyNateManagedAddress();
            IntPtr remoteLaunchCrazyNateManagedAddress = new IntPtr(
              remoteCrazyNateHModule.ToInt64() + 
              (localLaunchCrazyNateManagedAddress.ToInt64() - localCrazyNateHModule.ToInt64()));

            // Perform 'LaunchCrazyNateManaged' in remote process via CreateRemoteThread
            remoteThreadId = 0;
            hRemoteThread = Win32.CreateRemoteThread(
              processHandle,
              IntPtr.Zero, // thread attributes (zero means default)
              IntPtr.Zero, // stack size (zero means default)
              remoteLaunchCrazyNateManagedAddress,
              pRemoteMemory, // procedure parameter
              0, // creation flags (zero means thread runs immediately)
              ref remoteThreadId);

            if (hRemoteThread == IntPtr.Zero)
            {
              throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to create thread 2 in target process");
            }

            try
            {
              // Wait until the remote thread terminates (WaitForSingleObject);
              if (Win32.WAIT_OBJECT_0 != Win32.WaitForSingleObject(hRemoteThread, Win32.INFINITE))
              {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to wait for thread 2 in target process to finish");
              }

              // Retrieve the exit code of the remote thread (GetExitCodeThread).
              // Note that this is the value returned by LaunchCrazyNateManaged, thus indicates whether it thinks it succeeded.
              Int32 remoteLaunchCrazyNateManagedResult = 0;
              if (!Win32.GetExitCodeThread(hRemoteThread, ref remoteLaunchCrazyNateManagedResult))
              {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to get exit code of thread 2 in target process");
              }

              if (remoteLaunchCrazyNateManagedResult == 0)
              {
                // read remote process memory for error message about why it failed
                string errorMessage = string.Empty;
                StringBuilder errorMessageBuilder = new StringBuilder(remoteMemorySize.ToInt32() / sizeof(char));
                IntPtr errorMessageBytesRead = IntPtr.Zero;
                if (Win32.ReadProcessMemory(processHandle, pRemoteMemory, errorMessageBuilder, remoteMemorySize, ref errorMessageBytesRead))
                {
                  errorMessage = errorMessageBuilder.ToString();
                  int nullTerminatorIndex = errorMessage.IndexOf((char)0);
                  if (nullTerminatorIndex >= 0)
                  {
                    errorMessage = errorMessage.Substring(0, nullTerminatorIndex);
                  }
                }
                else
                {
                  errorMessage = "Failed to get error message from target process: " + (new Win32Exception(Marshal.GetLastWin32Error())).Message;
                }

                throw new Win32Exception("Failed to launch managed code in target process\r\n" + errorMessage);
              }
            }
            finally
            {
              Win32.CloseHandle(hRemoteThread);
            }
          }
          finally
          {
            // meh, don't bother unloading the dll. Let's just leave a mess.
#if false
            // Unload the DLL from the remote process via CreateRemoteThread & FreeLibrary.
            // Pass the HMODULE handle retreived in Step #6 to FreeLibrary (via lpParameter in CreateRemoteThread).
            // Note: If your injected DLL spawns any new threads, be sure they are all terminated before unloading it.
            // (note: again, this works because Kernel32 is loaded at the same address in all processes)
            remoteProcAddress := cardinal(GetProcAddress( GetModuleHandle('Kernel32'), 'FreeLibrary'));
            hRemoteThread := cardinal(CreateRemoteThread(
              qsHandle,
              nil, // thread attributes
              0, // stack size
              Pointer(remoteProcAddress),
              Pointer(hRemoteLoadedDllHModule),
              0,
              {var} remoteThreadId));

            if hRemoteThread = 0 then
            begin
              ShowErrorMessage('Failed to start FreeLibrary thread in QuickSet.exe PID=' + IntToStr(qsPid));
            end
            else
            begin
              // Wait until the thread terminates (WaitForSingleObject).
              WaitForSingleObject(hRemoteThread, INFINITE);
            end;
#endif
          }
        }
        finally
        {
          // Free the memory allocated in Step #2 (VirtualFreeEx).
          Win32.VirtualFreeEx(processHandle, pRemoteMemory, IntPtr.Zero, Win32.MEM_RELEASE);
        }
      }
      finally
      {
        Win32.CloseHandle(processHandle);
      }
    }

    private static void CopyDllsToProcessDirectory(IntPtr processHandle)
    {
      // Get the full path of CrazyNateManaged.dll
      string thisDllUri = Assembly.GetExecutingAssembly().CodeBase;
      string thisDllPath = (new Uri(thisDllUri)).LocalPath;

      // Get the directory path of CrazyNateManaged.dll
      string thisDllDirPath = Path.GetDirectoryName(thisDllPath);

      // Get the full path of the target process
      StringBuilder processPathBuffer = new StringBuilder(Win32.MAX_PATH);
      int numChars = Win32.MAX_PATH;
      if (!Win32.QueryFullProcessImageNameW(processHandle, 0, processPathBuffer, ref numChars))
      {
        throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to get full executable path of target process");
      }
      processPathBuffer.Length = numChars;
      string processPath = processPathBuffer.ToString();

      // Get directory path of the target process
      string processDirPath = Path.GetDirectoryName(processPath);

      // Copy CrazyNate dlls to target process directory
      foreach (string fileName in new string[] { "CrazyNateManaged.dll", "CrazyNate.dll" })
      {
        string sourcePath = Path.Combine(thisDllDirPath, fileName);
        string destPath = Path.Combine(processDirPath, fileName);

        try
        {
          File.Copy(sourcePath, destPath, true);
        }
        catch (Exception ex)
        {
          // let it slide if target process already has dlls loaded
          if (!File.Exists(destPath))
          {
            throw new AggregateException("Unable to copy CrazyNate dlls from " + thisDllDirPath + " to target process directory " + processDirPath, ex);
          }
        }
      }
    }
  }
}