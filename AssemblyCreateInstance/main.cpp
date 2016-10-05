
#include <windows.h>
#include <metahost.h>
#include <OleAuto.h>
#include <process.h>
#include <stdio.h>

//#import "C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\mscorlib.tlb" no_namespace, raw_interfaces_only
// that conflicted with 'metahost.h' on IUnknown and other stuff, so I copied the auto-generated parts I needed into my own header
#include "MyMscorlib.h"

// IFoo interface
struct __declspec(uuid("aaaaaaaa-2f1a-384a-bc52-fde93c215c5b"))
IFoo : IDispatch
{
  virtual HRESULT __stdcall DoStuff() = 0;
};

// IBar interface
struct __declspec(uuid("D4599D30-9C4F-4A62-95DE-94ABF02E3C5E"))
IBar : IDispatch
{
  virtual HRESULT __stdcall DoSomethingElse() = 0;
};

// HRESULT inspector
HRESULT result;
#define CHECK(a) { result = (a); if (!SUCCEEDED(result)) { printf("Error: %d\n", HRESULT_CODE(result)); return 1; } }

// global variables
VARIANT fooBar;
IFoo * foo = NULL;

unsigned int __stdcall ThreadProc(void * p);

int CALLBACK WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow)
{
  ICLRMetaHost * pMetaHost = NULL;
  ICLRRuntimeInfo * pRuntimeInfo = NULL;
  ICLRRuntimeHost * pRuntime = NULL;
  ICorRuntimeHost * pCorRuntime = NULL;
  IUnknown * pDefaultDomainUnkown = NULL;
  _AppDomain * pAppDomain = NULL;
  _Assembly * pAssembly = NULL;
  DWORD returnValue = 0;
  DWORD appDomainId = 0;
  BSTR assemblyName = SysAllocString(L"FizzBuzz"); // FizzBuzz.dll
  BSTR typeName = SysAllocString(L"FizzBuzz.FooBar");
  MSG msg;

  // make sure the thread's message queue exists
  BOOL peekResult = PeekMessage(&msg, 0, WM_USER, WM_USER, PM_NOREMOVE);

  // make sure this is an STA thread
  CHECK(CoInitializeEx(0, COINIT_APARTMENTTHREADED));

  // load and start a CLR
  CHECK(CLRCreateInstance(CLSID_CLRMetaHost, IID_ICLRMetaHost, (LPVOID*)&pMetaHost));
  CHECK(pMetaHost->GetRuntime(L"v4.0.30319", IID_ICLRRuntimeInfo, (LPVOID*)&pRuntimeInfo));
  CHECK(pRuntimeInfo->GetInterface(CLSID_CLRRuntimeHost, IID_ICLRRuntimeHost, (LPVOID*)&pRuntime));
  CHECK(pRuntime->Start());
  CHECK(pRuntime->GetCurrentAppDomainId(&appDomainId));

  // load FizzBuzz.dll and create a FooBar instance
  CHECK(CorBindToRuntimeEx(L"v4.0.30319", 0, 0, CLSID_CorRuntimeHost, IID_ICorRuntimeHost, (LPVOID*)&pCorRuntime));
  CHECK(pCorRuntime->GetDefaultDomain(&pDefaultDomainUnkown));
  CHECK(pDefaultDomainUnkown->QueryInterface(&pAppDomain));
  CHECK(pAppDomain->Load_2(assemblyName, &pAssembly));
  CHECK(pAssembly->CreateInstance(typeName, &fooBar));
  CHECK(fooBar.pdispVal->QueryInterface(&foo));
  
  // prove that it works
  CHECK(foo->DoStuff());

  // launch a thread to use foo
  uintptr_t threadHandle = _beginthreadex(0, 0, ThreadProc, 0, 0, 0);

  while (true)
  {
    Sleep(1);

    // TODO: if we wanted to pump messages between sleeps, this is how we'd do it
    //while (GetMessage(&msg, NULL, 0, 0))
    //{
    //  TranslateMessage(&msg);
    //  DispatchMessage(&msg);
    //}
  }

  return 0;
}

unsigned int __stdcall ThreadProc(void * p)
{
  IBar * bar = NULL;

  // cast to another interface type
  // (If 'foo' was not free-threaded, this would block indefinitely 
  CHECK(foo->QueryInterface(&bar));
  bar->DoSomethingElse();
}