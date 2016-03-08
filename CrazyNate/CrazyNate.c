
// MSCorEE.lib;kernel32.lib;user32.lib;gdi32.lib;winspool.lib;comdlg32.lib;advapi32.lib;shell32.lib;ole32.lib;oleaut32.lib;uuid.lib;odbc32.lib;odbccp32.lib;%(AdditionalDependencies)

#include <windows.h>
#include <objbase.h>
#include <INITGUID.H>
#include <metahost.h>

// see if I can steal the .NET CRT implementation
// like suggested at http://www.drdobbs.com/avoiding-the-visual-c-runtime-library/184416623
#pragma intrinsic(memcpy)

void wcscat_s_nate(WCHAR * buffer, SIZE_T bufferSize, const WCHAR * source)
{
  SIZE_T i, j;
  for (i = 0; i < bufferSize; i++)
  {
    if (buffer[i] == 0)
    {
      for (j = 0; source[j] != 0 && i < bufferSize; j++, i++)
      {
        buffer[i] = source[j];
      }

      if (i < bufferSize)
      {
        buffer[i] = 0;
      }
      buffer[bufferSize - 1] = 0;

      break;
    }
  }
}

void memset_nate(void * _Dst, int _Val, size_t _Size)
{
  size_t i;
  for (i = 0; i < _Size; i++)
  {
    ((BYTE*)_Dst)[i] = (BYTE)_Val;
  }
}

__declspec(dllexport) HMODULE STDMETHODCALLTYPE GetCrazyNateHModule()
{
  HMODULE myHModule = NULL;
  if (!GetModuleHandleExW(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS | GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT,
    (LPCWSTR)&GetCrazyNateHModule,
    &myHModule))
  {
    myHModule = NULL;
  }
  return myHModule;
}

// ERROR_BUFFER_SIZE is in characters (not bytes), as required by wcscpy_s
#define ERROR_BUFFER_SIZE 2000

__declspec(dllexport) DWORD STDMETHODCALLTYPE GetInputOutputBufferCharCount()
{
  return ERROR_BUFFER_SIZE;
}

volatile LONG inputBufferLock = 0;
WCHAR inputBufferCopy[ERROR_BUFFER_SIZE];

__declspec(dllexport) DWORD STDMETHODCALLTYPE LaunchCrazyNateManaged(LPVOID inputOutputBuffer)
{
  IEnumUnknown * pRuntimes = NULL;
  ICLRMetaHost * pMetaHost = NULL;
  ICLRRuntimeInfo * pRuntimeInfo = NULL;
  ICLRRuntimeHost * pRuntime = NULL;
  DWORD returnValue = 0;
  DWORD done = 0;
  WCHAR versionBuffer[100];
  DWORD versionBufferLen = 100;
  DWORD numRuntimesEnumerated = 0;
  WCHAR * dllName;
  WCHAR * typeName;
  WCHAR * methodName;
  WCHAR * argument;
  WCHAR * errorMessageBuffer;

  while (InterlockedExchange(&inputBufferLock, 3) != 0)
  {
    // spin wait
  }

  // copy inputs to separate buffer, so 'inputOutputBuffer' can be used for output
  memcpy(inputBufferCopy, inputOutputBuffer, ERROR_BUFFER_SIZE * sizeof(WCHAR));
  // first element is CrazyNate.dll, which we already know!
  // next arguments are managed dll name, mananged type name, method name, and argument
  dllName = &inputBufferCopy[0] + lstrlenW(&inputBufferCopy[0]) + 1;
  typeName = dllName + lstrlenW(dllName) + 1;
  methodName = typeName + lstrlenW(typeName) + 1;
  argument = methodName + lstrlenW(methodName) + 1;

  // initialize error buffer completely to zeros so managed code doesn't pick up any scum when reading this data
  errorMessageBuffer = (WCHAR*)inputOutputBuffer;
  memset_nate(errorMessageBuffer, 0, ERROR_BUFFER_SIZE * sizeof(WCHAR));

  // complain if inputs are too large
  if (argument + lstrlenW(argument) >= &inputBufferCopy[0] + ERROR_BUFFER_SIZE)
  {
    wcscat_s_nate(errorMessageBuffer, ERROR_BUFFER_SIZE, L"inputs were too large");
  }
  else
  {
    // get CLR com object
    if (S_OK == CLRCreateInstance(&CLSID_CLRMetaHost, &IID_ICLRMetaHost, (LPVOID*)&pMetaHost))
    {
      if (S_OK == pMetaHost->lpVtbl->EnumerateLoadedRuntimes(pMetaHost,
        GetCurrentProcess(),
        &pRuntimes))
      {
        while (done == 0 &&
          S_OK == pRuntimes->lpVtbl->Next(pRuntimes,
          1,
          (IUnknown**)&pRuntimeInfo,
          NULL))
        {
          numRuntimesEnumerated++;

          // try to be smart about which runtime it is.
          // I want the .NET 4.5.1 runtime
          memset_nate(versionBuffer, 0, sizeof(versionBuffer));
          if (S_OK == pRuntimeInfo->lpVtbl->GetVersionString(pRuntimeInfo, versionBuffer, &versionBufferLen))
          {
            versionBuffer[99] = 0;

            if (versionBuffer[0] == 'v' &&
              versionBuffer[1] == '4' &&
              versionBuffer[2] == '.' &&
              versionBuffer[3] == '0')
            {
              wcscat_s_nate(errorMessageBuffer, ERROR_BUFFER_SIZE, L"found CLR, version is ");
              wcscat_s_nate(errorMessageBuffer, ERROR_BUFFER_SIZE, versionBuffer);
              wcscat_s_nate(errorMessageBuffer, ERROR_BUFFER_SIZE, L"\r\n");

              // load the CLR
              // (TODO: it should already be loaded! this is just going to get me a reference to it, right?)
              if (S_OK == pRuntimeInfo->lpVtbl->GetInterface(pRuntimeInfo,
                &CLSID_CLRRuntimeHost,
                &IID_ICLRRuntimeHost,
                &pRuntime))
              {
                if (S_OK == pRuntime->lpVtbl->ExecuteInDefaultAppDomain(pRuntime,
                  dllName, // L"CrazyNateManaged.dll", // assembly path
                  typeName, // L"CrazyNateManaged.ManagedEntryPoint", // type name
                  methodName, // L"Enter", // method name
                  argument, // L"and I mean it!", // argument
                  &returnValue)) // TODO: do I have any use for this?
                {
                  // great. Can I have a doughnut?
                  done = 1;
                }
                else
                {
                  wcscat_s_nate(errorMessageBuffer, ERROR_BUFFER_SIZE, L"ignoring CLR because ICLRRuntimeHost.ExecuteInDefaultAppDomain(CrazyNateManaged.ManagedEntryPoint.Enter) failed\r\n");
                }

                pRuntime->lpVtbl->Release(pRuntime);
              }
              else
              {
                wcscat_s_nate(errorMessageBuffer, ERROR_BUFFER_SIZE, L"ignoring CLR because ICLRRuntimeInfo.GetInterface failed\r\n");
              }
            }
            else
            {
              wcscat_s_nate(errorMessageBuffer, ERROR_BUFFER_SIZE, L"found CLR, but ignoring because version is ");
              wcscat_s_nate(errorMessageBuffer, ERROR_BUFFER_SIZE, versionBuffer);
              wcscat_s_nate(errorMessageBuffer, ERROR_BUFFER_SIZE, L"\r\n");
            }
          }
          else
          {
            wcscat_s_nate(errorMessageBuffer, ERROR_BUFFER_SIZE, L"found CLR, but ignoring because ICLRRuntimeInfo.GetVersionString() failed\r\n");
          }
          pRuntimeInfo->lpVtbl->Release(pRuntimeInfo);
        }
        if (numRuntimesEnumerated == 0)
        {
          wcscat_s_nate(errorMessageBuffer, ERROR_BUFFER_SIZE, L"no CLR runtimes were loaded\r\n");
        }
        pRuntimes->lpVtbl->Release(pRuntimes);
      }
      else
      {
        wcscat_s_nate(errorMessageBuffer, ERROR_BUFFER_SIZE, L"EnumerateLoadedRuntimes() failed\r\n");
      }
      pMetaHost->lpVtbl->Release(pMetaHost);
    }
    else
    {
      wcscat_s_nate(errorMessageBuffer, ERROR_BUFFER_SIZE, L"CLRCreateInstance() failed\r\n");
    }
  }

  // release spin lock
  InterlockedExchange(&inputBufferLock, 0);

  return done;
}

__declspec(dllexport) LPVOID STDMETHODCALLTYPE GetLaunchCrazyNateManagedAddress()
{
  return (LPVOID)&LaunchCrazyNateManaged;
}

BOOL STDMETHODCALLTYPE CrazyNateDllEntryPoint(
  HINSTANCE hinstDLL,
  DWORD     fdwReason,
  LPVOID    lpvReserved)
{
  if (fdwReason == DLL_PROCESS_ATTACH)
  {
    // TODO: was this not zero?
    inputBufferLock = 0;
    // TODO: initialize anything I guess?
    return TRUE; // indicate success
  }

  return 1337; // return value is ignored for other fdwReasons
}