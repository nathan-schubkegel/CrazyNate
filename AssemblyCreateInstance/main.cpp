
#include <windows.h>
//#include <objbase.h>
//#include <INITGUID.H>
//#include <metahost.h>
//#include "mscoree.h"
//#include <Strsafe.h>
#include <stdio.h>
#import "C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\mscorlib.tlb" no_namespace, raw_interfaces_only

#include "HostControl.h"

HRESULT result;
#define CHECK(a) { result = (a); if (!SUCCEEDED(result)) { printf("Error: %d\n", HRESULT_CODE(result)); return 1; } }

int CALLBACK WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow)
{
  ICLRMetaHost * pMetaHost = NULL;
  ICLRRuntimeInfo * pRuntimeInfo = NULL;
  ICLRRuntimeHost * pRuntime = NULL;
  ICorRuntimeHost * pCorRuntime = NULL;
  IHostControl * pHostControl = &hostControl;
  IUnknown * pDefaultDomainUnkown = NULL;
  _AppDomain * pAppDomain = NULL;
  DWORD returnValue = 0;
  DWORD appDomainId = 0;

  CHECK(CLRCreateInstance(&CLSID_CLRMetaHost, &IID_ICLRMetaHost, &pMetaHost));
  CHECK(pMetaHost->lpVtbl->GetRuntime(pMetaHost, L"v4.0.30319", &IID_ICLRRuntimeInfo, &pRuntimeInfo));
  CHECK(pRuntimeInfo->lpVtbl->GetInterface(pRuntimeInfo, &CLSID_CLRRuntimeHost, &IID_ICLRRuntimeHost, &pRuntime));
  CHECK(pRuntime->lpVtbl->SetHostControl(pRuntime, pHostControl));
  CHECK(pRuntime->lpVtbl->Start(pRuntime));
  CHECK(pRuntime->lpVtbl->GetCurrentAppDomainId(pRuntime, &appDomainId));

  CHECK(CorBindToRuntimeEx(L"v4.0.30319", 0, 0, &CLSID_CorRuntimeHost, &IID_ICorRuntimeHost, (void**)&pCorRuntime));
  
  hr = pRuntimeHost->GetDefaultDomain(&pCurrentDomain);
  pCurrentDomain.ExecuteAssembly(assemblyFilename);

  CHECK(pRuntime->lpVtbl->ExecuteInDefaultAppDomain(pRuntime,
    L"FizzBuzz.dll", // assembly path
    L"FizzBuzz.FooBar", // type name
    L"DoStuff", // method name
    L"and I mean it!", // argument
    &returnValue));

  return 0;
}
