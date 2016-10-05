//#include <windows.h>
//#include <objbase.h>
//#include <INITGUID.H>
//#include <metahost.h>
//#include <Strsafe.h>
//#include <stdio.h>
#import "C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\mscorlib.tlb" no_namespace, raw_interfaces_only

HRESULT STDMETHODCALLTYPE HostControl_QueryInterface(IHostControl * This, REFIID riid, __RPC__deref_out  void **ppvObject)
{
  if (ppvObject == 0)
  {
    return E_POINTER;
  }

  if (IsEqualIID(riid, &IID_IUnknown) || IsEqualIID(riid, &IID_IHostControl))
  {
    This->lpVtbl->AddRef(This);
    *ppvObject = This;
    return S_OK;
  }

  *ppvObject = 0;
  return E_NOINTERFACE;
}

ULONG STDMETHODCALLTYPE HostControl_AddRef(IHostControl * This)
{
  return 1;
}

ULONG STDMETHODCALLTYPE HostControl_Release(IHostControl * This)
{
  return 1;
}

HRESULT STDMETHODCALLTYPE HostControl_GetHostManager(IHostControl * This, REFIID riid, void **ppObject)
{
  if (ppObject == 0) return E_POINTER;
  *ppObject = 0;
  return E_NOINTERFACE;
}

HRESULT STDMETHODCALLTYPE HostControl_SetAppDomainManager(IHostControl * This, DWORD dwAppDomainID, IUnknown *pUnkAppDomainManager)
{
  return S_OK;
}

IHostControlVtbl vtblHostControl =
{
  HostControl_QueryInterface,
  HostControl_AddRef,
  HostControl_Release,
  HostControl_GetHostManager,
  HostControl_SetAppDomainManager,
};

IHostControl hostControl =
{
  &vtblHostControl
};