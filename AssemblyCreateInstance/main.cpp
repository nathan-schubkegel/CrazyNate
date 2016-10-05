
#include <windows.h>
#include <OleAuto.h>
#include <process.h>
#include <stdio.h>
#include <mscoree.h>

//#import "C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\mscorlib.tlb" no_namespace, raw_interfaces_only
// mscorlib.tlb has conflicts with 'metahost.h' and 'mscoree.h' so I copied the auto-generated parts I needed into my own header
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
  ICorRuntimeHost * pCorRuntime = NULL;
  IUnknown * pAppDomainSetupUnknown = NULL;
  IAppDomainSetup * pAppDomainSetup = NULL;
  IUnknown * pAppDomainUnknown = NULL;
  _AppDomain * pAppDomain = NULL;
  _Assembly * pAssembly = NULL;
  BSTR appDomainName = SysAllocString(L"My Default AppDomain");
  BSTR assemblyName = SysAllocString(L"FizzBuzz"); // FizzBuzz.dll
  BSTR typeName = SysAllocString(L"FizzBuzz.FooBar");
  MSG msg;

  // make sure the thread's message queue exists
  PeekMessage(&msg, 0, WM_USER, WM_USER, PM_NOREMOVE);

  // make sure this is an STA thread
  CHECK(CoInitializeEx(0, COINIT_APARTMENTTHREADED));

  // create the default AppDomain
  CHECK(CorBindToRuntimeEx(L"v4.0.30319", L"svr", 0, CLSID_CorRuntimeHost, IID_ICorRuntimeHost, (LPVOID*)&pCorRuntime));
  CHECK(pCorRuntime->CreateDomainSetup(&pAppDomainSetupUnknown));
  CHECK(pAppDomainSetupUnknown->QueryInterface(&pAppDomainSetup));
  CHECK(pAppDomainSetup->put_ApplicationName(appDomainName));
  CHECK(pCorRuntime->CreateDomainEx(L"My Default AppDomain", pAppDomainSetup, 0, &pAppDomainUnknown));
  CHECK(pAppDomainUnknown->QueryInterface(&pAppDomain));

  // load FizzBuzz.dll and create a FooBar instance
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