
#include <windows.h>
//#include <objbase.h>
//#include <INITGUID.H>
#include <metahost.h>
#include <OleAuto.h>
//#include "mscoree.h"
//#include <Strsafe.h>
#include <stdio.h>
//#import "C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\mscorlib.tlb" no_namespace, raw_interfaces_only

#include "MyMscorlib.h"

struct __declspec(uuid("aaaaaaaa-2f1a-384a-bc52-fde93c215c5b"))
IFoo : IDispatch
{
  virtual HRESULT __stdcall DoStuff() = 0;
};

HRESULT result;
#define CHECK(a) { result = (a); if (!SUCCEEDED(result)) { printf("Error: %d\n", HRESULT_CODE(result)); return 1; } }

class MyHostControl : public IHostControl
{
  virtual HRESULT STDMETHODCALLTYPE QueryInterface(
    /* [in] */ REFIID riid,
    /* [iid_is][out] */ __RPC__deref_out void __RPC_FAR *__RPC_FAR *ppvObject)
  {
    if (ppvObject == 0) return E_POINTER;
    if (IsEqualIID(riid, IID_IUnknown) || IsEqualIID(riid, IID_IHostControl))
    {
      *ppvObject = this;
      return S_OK;
    }
    *ppvObject = 0;
    return E_NOINTERFACE;
  }

  virtual ULONG STDMETHODCALLTYPE AddRef(void)
  {
    return 1;
  }

  virtual ULONG STDMETHODCALLTYPE Release(void)
  {
    return 1;
  }

  virtual HRESULT STDMETHODCALLTYPE GetHostManager(
    /* [in] */ REFIID riid,
    /* [out] */ void **ppObject)
  {
    if (ppObject == 0) return E_POINTER;
    *ppObject = 0;
    return E_NOINTERFACE;
  }

  virtual HRESULT STDMETHODCALLTYPE SetAppDomainManager(
    /* [in] */ DWORD dwAppDomainID,
    /* [in] */ IUnknown *pUnkAppDomainManager)
  {
    return S_OK;
  }
};

int CALLBACK WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow)
{
  ICLRMetaHost * pMetaHost = NULL;
  ICLRRuntimeInfo * pRuntimeInfo = NULL;
  ICLRRuntimeHost * pRuntime = NULL;
  ICorRuntimeHost * pCorRuntime = NULL;
  IHostControl * pHostControl = new MyHostControl();
  IUnknown * pDefaultDomainUnkown = NULL;
  _AppDomain * pAppDomain = NULL;
  _Assembly * pAssembly = NULL;
  DWORD returnValue = 0;
  DWORD appDomainId = 0;
  BSTR assemblyName = SysAllocString(L"FizzBuzz");// L"C:\\Users\\nathschu\\Desktop\\CrazyNate\\Debug\\FizzBuzz.dll";
  BSTR typeName = SysAllocString(L"FizzBuzz.FooBar");
  VARIANT fooBar;
  IFoo * foo = NULL;

  CHECK(CLRCreateInstance(CLSID_CLRMetaHost, IID_ICLRMetaHost, (LPVOID*)&pMetaHost));
  CHECK(pMetaHost->GetRuntime(L"v4.0.30319", IID_ICLRRuntimeInfo, (LPVOID*)&pRuntimeInfo));
  CHECK(pRuntimeInfo->GetInterface(CLSID_CLRRuntimeHost, IID_ICLRRuntimeHost, (LPVOID*)&pRuntime));
  CHECK(pRuntime->SetHostControl(pHostControl));
  CHECK(pRuntime->Start());
  CHECK(pRuntime->GetCurrentAppDomainId(&appDomainId));

  CHECK(CorBindToRuntimeEx(L"v4.0.30319", 0, 0, CLSID_CorRuntimeHost, IID_ICorRuntimeHost, (LPVOID*)&pCorRuntime));
  CHECK(pCorRuntime->GetDefaultDomain(&pDefaultDomainUnkown));
  CHECK(pDefaultDomainUnkown->QueryInterface(&pAppDomain));
  CHECK(pAppDomain->Load_2(assemblyName, &pAssembly));
  CHECK(pAssembly->CreateInstance(typeName, &fooBar));
  CHECK(fooBar.pdispVal->QueryInterface(&foo));
  CHECK(foo->DoStuff());

  //CHECK(pRuntime->lpVtbl->ExecuteInDefaultAppDomain(pRuntime,
  //  L"FizzBuzz.dll", // assembly path
  //  L"FizzBuzz.FooBar", // type name
  //  L"DoStuff", // method name
  //  L"and I mean it!", // argument
  //  &returnValue));

  return 0;
}
