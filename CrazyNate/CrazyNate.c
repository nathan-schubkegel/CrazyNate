
#include <windows.h>
#include <objbase.h>
#include <INITGUID.H>
#include <metahost.h>
#include <Strsafe.h>

// see if I can steal the .NET CRT implementation of memcpy
// like suggested at http://www.drdobbs.com/avoiding-the-visual-c-runtime-library/184416623
#pragma intrinsic(memcpy)

// weirdness to get intrinsic memset working...
// stolen from http://stackoverflow.com/questions/2938966/how-to-use-vc-intrinsic-functions-w-o-run-time-library
void * __cdecl memset(void *, int, size_t);
#pragma intrinsic(memset)
#pragma function(memset)
void * __cdecl memset(void *pTarget, int value, size_t cbTarget) {
  unsigned char *p = (unsigned char*)(pTarget);
  while (cbTarget-- > 0) {
    *p++ = (unsigned char)(value);
  }
  return pTarget;
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
  WCHAR * dllPath;
  WCHAR * typeName;
  WCHAR * methodName;
  WCHAR * argument;
  WCHAR * errorMessageBuffer;
  size_t lengthArg;
  size_t lengthTotal;

  while (InterlockedExchange(&inputBufferLock, 3) != 0)
  {
    // spin wait
  }

  // copy inputs to separate buffer, so 'inputOutputBuffer' can be used for output
  memcpy(inputBufferCopy, inputOutputBuffer, ERROR_BUFFER_SIZE * sizeof(WCHAR));
  errorMessageBuffer = (WCHAR*)inputOutputBuffer;
  memset(errorMessageBuffer, 0, ERROR_BUFFER_SIZE * sizeof(WCHAR));

  // prepare to find the individual strings in 'inputBufferCopy'
  lengthTotal = 0;
#define GetArg(varName) \
  if (lengthTotal < ERROR_BUFFER_SIZE) \
  { \
    if (S_OK == StringCchLength(&inputBufferCopy[lengthTotal], ERROR_BUFFER_SIZE - lengthTotal, &lengthArg)) \
    { \
      varName = &inputBufferCopy[lengthTotal]; \
      lengthTotal += lengthArg + 1; \
    } \
    else \
    { \
      lengthTotal = ERROR_BUFFER_SIZE + 1; \
    } \
  }

  // arguments are managed dll path, mananged type name, method name, and argument
  GetArg(dllPath);
  GetArg(typeName);
  GetArg(methodName);
  GetArg(argument);
  
  // complain if inputs were too large
  if (lengthTotal > ERROR_BUFFER_SIZE)
  {
    StringCchCat(errorMessageBuffer, ERROR_BUFFER_SIZE, L"inputs were too large");
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
          memset((void*)versionBuffer, (int)0, (size_t)sizeof(versionBuffer));
          if (S_OK == pRuntimeInfo->lpVtbl->GetVersionString(pRuntimeInfo, versionBuffer, &versionBufferLen))
          {
            versionBuffer[99] = 0;

            if (versionBuffer[0] == 'v' &&
              versionBuffer[1] == '4' &&
              versionBuffer[2] == '.' &&
              versionBuffer[3] == '0')
            {
              StringCchCat(errorMessageBuffer, ERROR_BUFFER_SIZE, L"found CLR, version is ");
              StringCchCat(errorMessageBuffer, ERROR_BUFFER_SIZE, versionBuffer);
              StringCchCat(errorMessageBuffer, ERROR_BUFFER_SIZE, L"\r\n");

              // load the CLR
              // (TODO: it should already be loaded! this is just going to get me a reference to it, right?)
              if (S_OK == pRuntimeInfo->lpVtbl->GetInterface(pRuntimeInfo,
                &CLSID_CLRRuntimeHost,
                &IID_ICLRRuntimeHost,
                &pRuntime))
              {
                if (S_OK == pRuntime->lpVtbl->ExecuteInDefaultAppDomain(pRuntime,
                  dllPath, // L"CrazyNateManaged.dll", // assembly path
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
                  StringCchCat(errorMessageBuffer, ERROR_BUFFER_SIZE, L"ignoring CLR because ICLRRuntimeHost.ExecuteInDefaultAppDomain(CrazyNateManaged.ManagedEntryPoint.Enter) failed\r\n");
                }

                pRuntime->lpVtbl->Release(pRuntime);
              }
              else
              {
                StringCchCat(errorMessageBuffer, ERROR_BUFFER_SIZE, L"ignoring CLR because ICLRRuntimeInfo.GetInterface failed\r\n");
              }
            }
            else
            {
              StringCchCat(errorMessageBuffer, ERROR_BUFFER_SIZE, L"found CLR, but ignoring because version is ");
              StringCchCat(errorMessageBuffer, ERROR_BUFFER_SIZE, versionBuffer);
              StringCchCat(errorMessageBuffer, ERROR_BUFFER_SIZE, L"\r\n");
            }
          }
          else
          {
            StringCchCat(errorMessageBuffer, ERROR_BUFFER_SIZE, L"found CLR, but ignoring because ICLRRuntimeInfo.GetVersionString() failed\r\n");
          }
          pRuntimeInfo->lpVtbl->Release(pRuntimeInfo);
        }
        if (numRuntimesEnumerated == 0)
        {
          StringCchCat(errorMessageBuffer, ERROR_BUFFER_SIZE, L"no CLR runtimes were loaded\r\n");
        }
        pRuntimes->lpVtbl->Release(pRuntimes);
      }
      else
      {
        StringCchCat(errorMessageBuffer, ERROR_BUFFER_SIZE, L"EnumerateLoadedRuntimes() failed\r\n");
      }
      pMetaHost->lpVtbl->Release(pMetaHost);
    }
    else
    {
      StringCchCat(errorMessageBuffer, ERROR_BUFFER_SIZE, L"CLRCreateInstance() failed\r\n");
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