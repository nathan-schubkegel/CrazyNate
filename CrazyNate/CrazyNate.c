
// MSCorEE.lib;kernel32.lib;user32.lib;gdi32.lib;winspool.lib;comdlg32.lib;advapi32.lib;shell32.lib;ole32.lib;oleaut32.lib;uuid.lib;odbc32.lib;odbccp32.lib;%(AdditionalDependencies)

#include <windows.h>
#include <objbase.h>
#include <INITGUID.H>
#include <metahost.h>

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

__declspec(dllexport) DWORD STDMETHODCALLTYPE GetExpectedErrorMessageBufferSize()
{
  return ERROR_BUFFER_SIZE;
}

__declspec(dllexport) DWORD STDMETHODCALLTYPE LaunchCrazyNateManaged(LPVOID errorMessageBuffer)
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
  int i;

  // initialize error buffer completely to zeros so managed code doesn't pick up any scum when reading this data
  for (i = 0; i < ERROR_BUFFER_SIZE; i++)
  {
    ((wchar_t*)errorMessageBuffer)[i] = 0;
  }

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
        memset(versionBuffer, 0, sizeof(versionBuffer));
        if (S_OK == pRuntimeInfo->lpVtbl->GetVersionString(pRuntimeInfo, versionBuffer, &versionBufferLen))
        {
          versionBuffer[99] = 0;

          if (versionBuffer[0] == 'v' &&
            versionBuffer[1] == '4' &&
            versionBuffer[2] == '.' &&
            versionBuffer[3] == '0')
          {
            wcscat_s(errorMessageBuffer, ERROR_BUFFER_SIZE, L"found CLR, version is ");
            wcscat_s(errorMessageBuffer, ERROR_BUFFER_SIZE, versionBuffer);
            wcscat_s(errorMessageBuffer, ERROR_BUFFER_SIZE, L"\r\n");

            // load the CLR
            // (TODO: it should already be loaded! this is just going to get me a reference to it, right?)
            if (S_OK == pRuntimeInfo->lpVtbl->GetInterface(pRuntimeInfo,
              &CLSID_CLRRuntimeHost,
              &IID_ICLRRuntimeHost,
              &pRuntime))
            {
              if (S_OK == pRuntime->lpVtbl->ExecuteInDefaultAppDomain(pRuntime,
                L"CrazyNateManaged.dll", // assembly path
                L"CrazyNateManaged.ManagedEntryPoint", // type name
                L"Enter", // method name
                L"and I mean it!", // argument
                &returnValue)) // TODO: do I have any use for this?
              {
                // great. Can I have a doughnut?
                done = 1;
              }
              else
              {
                wcscat_s(errorMessageBuffer, ERROR_BUFFER_SIZE, L"ignoring CLR because ICLRRuntimeHost.ExecuteInDefaultAppDomain(CrazyNateManaged.ManagedEntryPoint.Enter) failed\r\n");
              }

              pRuntime->lpVtbl->Release(pRuntime);
            }
            else
            {
              wcscat_s(errorMessageBuffer, ERROR_BUFFER_SIZE, L"ignoring CLR because ICLRRuntimeInfo.GetInterface failed\r\n");
            }
          }
          else
          {
            wcscat_s(errorMessageBuffer, ERROR_BUFFER_SIZE, L"found CLR, but ignoring because version is ");
            wcscat_s(errorMessageBuffer, ERROR_BUFFER_SIZE, versionBuffer);
            wcscat_s(errorMessageBuffer, ERROR_BUFFER_SIZE, L"\r\n");
          }
        }
        else
        {
          wcscat_s(errorMessageBuffer, ERROR_BUFFER_SIZE, L"found CLR, but ignoring because ICLRRuntimeInfo.GetVersionString() failed\r\n");
        }
        pRuntimeInfo->lpVtbl->Release(pRuntimeInfo);
      }
      if (numRuntimesEnumerated == 0)
      {
        wcscat_s(errorMessageBuffer, ERROR_BUFFER_SIZE, L"no CLR runtimes were loaded\r\n");
      }
      pRuntimes->lpVtbl->Release(pRuntimes);
    }
    else
    {
      wcscat_s(errorMessageBuffer, ERROR_BUFFER_SIZE, L"EnumerateLoadedRuntimes() failed\r\n");
    }
    pMetaHost->lpVtbl->Release(pMetaHost);
  }
  else
  {
    wcscat_s(errorMessageBuffer, ERROR_BUFFER_SIZE, L"CLRCreateInstance() failed\r\n");
  }

  return done;
}

__declspec(dllexport) LPVOID STDMETHODCALLTYPE GetLaunchCrazyNateManagedAddress()
{
  return (LPVOID)&LaunchCrazyNateManaged;
}
