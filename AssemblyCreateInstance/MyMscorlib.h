// Created by Microsoft (R) C/C++ Compiler Version 12.00.31101.0 (adddd687).
//
// c:\users\bratface\desktop\crap\crazynate\assemblycreateinstance\debug\mscorlib.tlh
//
// C++ source equivalent of Win32 type library C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319\\mscorlib.tlb
// compiler-generated file created 10/04/16 at 23:37:44 - DO NOT EDIT!

#pragma once
#pragma pack(push, 8)

struct __declspec(uuid("27fff232-a7a8-40dd-8d4a-734ad59fcd41"))
IAppDomainSetup : IUnknown
{
  //
  // Raw methods provided by interface
  //

  virtual HRESULT __stdcall get_ApplicationBase(
    /*[out,retval]*/ BSTR * pRetVal) = 0;
  virtual HRESULT __stdcall put_ApplicationBase(
    /*[in]*/ BSTR pRetVal) = 0;
  virtual HRESULT __stdcall get_ApplicationName(
    /*[out,retval]*/ BSTR * pRetVal) = 0;
  virtual HRESULT __stdcall put_ApplicationName(
    /*[in]*/ BSTR pRetVal) = 0;
  virtual HRESULT __stdcall get_CachePath(
    /*[out,retval]*/ BSTR * pRetVal) = 0;
  virtual HRESULT __stdcall put_CachePath(
    /*[in]*/ BSTR pRetVal) = 0;
  virtual HRESULT __stdcall get_ConfigurationFile(
    /*[out,retval]*/ BSTR * pRetVal) = 0;
  virtual HRESULT __stdcall put_ConfigurationFile(
    /*[in]*/ BSTR pRetVal) = 0;
  virtual HRESULT __stdcall get_DynamicBase(
    /*[out,retval]*/ BSTR * pRetVal) = 0;
  virtual HRESULT __stdcall put_DynamicBase(
    /*[in]*/ BSTR pRetVal) = 0;
  virtual HRESULT __stdcall get_LicenseFile(
    /*[out,retval]*/ BSTR * pRetVal) = 0;
  virtual HRESULT __stdcall put_LicenseFile(
    /*[in]*/ BSTR pRetVal) = 0;
  virtual HRESULT __stdcall get_PrivateBinPath(
    /*[out,retval]*/ BSTR * pRetVal) = 0;
  virtual HRESULT __stdcall put_PrivateBinPath(
    /*[in]*/ BSTR pRetVal) = 0;
  virtual HRESULT __stdcall get_PrivateBinPathProbe(
    /*[out,retval]*/ BSTR * pRetVal) = 0;
  virtual HRESULT __stdcall put_PrivateBinPathProbe(
    /*[in]*/ BSTR pRetVal) = 0;
  virtual HRESULT __stdcall get_ShadowCopyDirectories(
    /*[out,retval]*/ BSTR * pRetVal) = 0;
  virtual HRESULT __stdcall put_ShadowCopyDirectories(
    /*[in]*/ BSTR pRetVal) = 0;
  virtual HRESULT __stdcall get_ShadowCopyFiles(
    /*[out,retval]*/ BSTR * pRetVal) = 0;
  virtual HRESULT __stdcall put_ShadowCopyFiles(
    /*[in]*/ BSTR pRetVal) = 0;
};

struct __declspec(uuid("17156360-2f1a-384a-bc52-fde93c215c5b"))
_Assembly : IDispatch
{
  //
  // Raw methods provided by interface
  //

  virtual HRESULT __stdcall get_ToString(
    /*[out,retval]*/ BSTR * pRetVal) = 0;
  virtual HRESULT __stdcall Equals(
    /*[in]*/ VARIANT other,
    /*[out,retval]*/ VARIANT_BOOL * pRetVal) = 0;
  virtual HRESULT __stdcall GetHashCode(
    /*[out,retval]*/ long * pRetVal) = 0;
  virtual HRESULT __stdcall GetType(
  /*[out,retval]*/ struct _Type * * pRetVal) = 0;
  virtual HRESULT __stdcall get_CodeBase(
    /*[out,retval]*/ BSTR * pRetVal) = 0;
  virtual HRESULT __stdcall get_EscapedCodeBase(
    /*[out,retval]*/ BSTR * pRetVal) = 0;
  virtual HRESULT __stdcall GetName(
  /*[out,retval]*/ struct _AssemblyName * * pRetVal) = 0;
  virtual HRESULT __stdcall GetName_2(
    /*[in]*/ VARIANT_BOOL copiedName,
  /*[out,retval]*/ struct _AssemblyName * * pRetVal) = 0;
  virtual HRESULT __stdcall get_FullName(
    /*[out,retval]*/ BSTR * pRetVal) = 0;
  virtual HRESULT __stdcall get_EntryPoint(
  /*[out,retval]*/ struct _MethodInfo * * pRetVal) = 0;
  virtual HRESULT __stdcall GetType_2(
    /*[in]*/ BSTR name,
  /*[out,retval]*/ struct _Type * * pRetVal) = 0;
  virtual HRESULT __stdcall GetType_3(
    /*[in]*/ BSTR name,
    /*[in]*/ VARIANT_BOOL throwOnError,
  /*[out,retval]*/ struct _Type * * pRetVal) = 0;
  virtual HRESULT __stdcall GetExportedTypes(
    /*[out,retval]*/ SAFEARRAY * * pRetVal) = 0;
  virtual HRESULT __stdcall GetTypes(
    /*[out,retval]*/ SAFEARRAY * * pRetVal) = 0;
  virtual HRESULT __stdcall GetManifestResourceStream(
  /*[in]*/ struct _Type * Type,
    /*[in]*/ BSTR name,
  /*[out,retval]*/ struct _Stream * * pRetVal) = 0;
  virtual HRESULT __stdcall GetManifestResourceStream_2(
    /*[in]*/ BSTR name,
  /*[out,retval]*/ struct _Stream * * pRetVal) = 0;
  virtual HRESULT __stdcall GetFile(
    /*[in]*/ BSTR name,
  /*[out,retval]*/ struct _FileStream * * pRetVal) = 0;
  virtual HRESULT __stdcall GetFiles(
    /*[out,retval]*/ SAFEARRAY * * pRetVal) = 0;
  virtual HRESULT __stdcall GetFiles_2(
    /*[in]*/ VARIANT_BOOL getResourceModules,
    /*[out,retval]*/ SAFEARRAY * * pRetVal) = 0;
  virtual HRESULT __stdcall GetManifestResourceNames(
    /*[out,retval]*/ SAFEARRAY * * pRetVal) = 0;
  virtual HRESULT __stdcall GetManifestResourceInfo(
    /*[in]*/ BSTR resourceName,
  /*[out,retval]*/ struct _ManifestResourceInfo * * pRetVal) = 0;
  virtual HRESULT __stdcall get_Location(
    /*[out,retval]*/ BSTR * pRetVal) = 0;
  virtual HRESULT __stdcall get_Evidence(
  /*[out,retval]*/ struct _Evidence * * pRetVal) = 0;
  virtual HRESULT __stdcall GetCustomAttributes(
  /*[in]*/ struct _Type * attributeType,
    /*[in]*/ VARIANT_BOOL inherit,
    /*[out,retval]*/ SAFEARRAY * * pRetVal) = 0;
  virtual HRESULT __stdcall GetCustomAttributes_2(
    /*[in]*/ VARIANT_BOOL inherit,
    /*[out,retval]*/ SAFEARRAY * * pRetVal) = 0;
  virtual HRESULT __stdcall IsDefined(
  /*[in]*/ struct _Type * attributeType,
    /*[in]*/ VARIANT_BOOL inherit,
    /*[out,retval]*/ VARIANT_BOOL * pRetVal) = 0;
  virtual HRESULT __stdcall GetObjectData(
  /*[in]*/ struct _SerializationInfo * info,
  /*[in]*/ struct StreamingContext Context) = 0;
  virtual HRESULT __stdcall add_ModuleResolve(
  /*[in]*/ struct _ModuleResolveEventHandler * value) = 0;
  virtual HRESULT __stdcall remove_ModuleResolve(
  /*[in]*/ struct _ModuleResolveEventHandler * value) = 0;
  virtual HRESULT __stdcall GetType_4(
    /*[in]*/ BSTR name,
    /*[in]*/ VARIANT_BOOL throwOnError,
    /*[in]*/ VARIANT_BOOL ignoreCase,
  /*[out,retval]*/ struct _Type * * pRetVal) = 0;
  virtual HRESULT __stdcall GetSatelliteAssembly(
  /*[in]*/ struct _CultureInfo * culture,
  /*[out,retval]*/ struct _Assembly * * pRetVal) = 0;
  virtual HRESULT __stdcall GetSatelliteAssembly_2(
  /*[in]*/ struct _CultureInfo * culture,
  /*[in]*/ struct _Version * Version,
  /*[out,retval]*/ struct _Assembly * * pRetVal) = 0;
  virtual HRESULT __stdcall LoadModule(
    /*[in]*/ BSTR moduleName,
    /*[in]*/ SAFEARRAY * rawModule,
  /*[out,retval]*/ struct _Module * * pRetVal) = 0;
  virtual HRESULT __stdcall LoadModule_2(
    /*[in]*/ BSTR moduleName,
    /*[in]*/ SAFEARRAY * rawModule,
    /*[in]*/ SAFEARRAY * rawSymbolStore,
  /*[out,retval]*/ struct _Module * * pRetVal) = 0;
  virtual HRESULT __stdcall CreateInstance(
    /*[in]*/ BSTR typeName,
    /*[out,retval]*/ VARIANT * pRetVal) = 0;
  virtual HRESULT __stdcall CreateInstance_2(
    /*[in]*/ BSTR typeName,
    /*[in]*/ VARIANT_BOOL ignoreCase,
    /*[out,retval]*/ VARIANT * pRetVal) = 0;
  virtual HRESULT __stdcall CreateInstance_3(
    /*[in]*/ BSTR typeName,
    /*[in]*/ VARIANT_BOOL ignoreCase,
  /*[in]*/ enum BindingFlags bindingAttr,
  /*[in]*/ struct _Binder * Binder,
    /*[in]*/ SAFEARRAY * args,
  /*[in]*/ struct _CultureInfo * culture,
    /*[in]*/ SAFEARRAY * activationAttributes,
    /*[out,retval]*/ VARIANT * pRetVal) = 0;
  virtual HRESULT __stdcall GetLoadedModules(
    /*[out,retval]*/ SAFEARRAY * * pRetVal) = 0;
  virtual HRESULT __stdcall GetLoadedModules_2(
    /*[in]*/ VARIANT_BOOL getResourceModules,
    /*[out,retval]*/ SAFEARRAY * * pRetVal) = 0;
  virtual HRESULT __stdcall GetModules(
    /*[out,retval]*/ SAFEARRAY * * pRetVal) = 0;
  virtual HRESULT __stdcall GetModules_2(
    /*[in]*/ VARIANT_BOOL getResourceModules,
    /*[out,retval]*/ SAFEARRAY * * pRetVal) = 0;
  virtual HRESULT __stdcall GetModule(
    /*[in]*/ BSTR name,
  /*[out,retval]*/ struct _Module * * pRetVal) = 0;
  virtual HRESULT __stdcall GetReferencedAssemblies(
    /*[out,retval]*/ SAFEARRAY * * pRetVal) = 0;
  virtual HRESULT __stdcall get_GlobalAssemblyCache(
    /*[out,retval]*/ VARIANT_BOOL * pRetVal) = 0;
};


struct __declspec(uuid("05f696dc-2b29-3663-ad8b-c4389cf2a713"))
_AppDomain : IUnknown
{
    //
    // Raw methods provided by interface
    //

      virtual HRESULT __stdcall GetTypeInfoCount (
        /*[out]*/ unsigned long * pcTInfo ) = 0;
      virtual HRESULT __stdcall GetTypeInfo (
        /*[in]*/ unsigned long iTInfo,
        /*[in]*/ unsigned long lcid,
        /*[in]*/ long ppTInfo ) = 0;
      virtual HRESULT __stdcall GetIDsOfNames (
        /*[in]*/ GUID * riid,
        /*[in]*/ long rgszNames,
        /*[in]*/ unsigned long cNames,
        /*[in]*/ unsigned long lcid,
        /*[in]*/ long rgDispId ) = 0;
      virtual HRESULT __stdcall Invoke (
        /*[in]*/ unsigned long dispIdMember,
        /*[in]*/ GUID * riid,
        /*[in]*/ unsigned long lcid,
        /*[in]*/ short wFlags,
        /*[in]*/ long pDispParams,
        /*[in]*/ long pVarResult,
        /*[in]*/ long pExcepInfo,
        /*[in]*/ long puArgErr ) = 0;
      virtual HRESULT __stdcall get_ToString (
        /*[out,retval]*/ BSTR * pRetVal ) = 0;
      virtual HRESULT __stdcall Equals (
        /*[in]*/ VARIANT other,
        /*[out,retval]*/ VARIANT_BOOL * pRetVal ) = 0;
      virtual HRESULT __stdcall GetHashCode (
        /*[out,retval]*/ long * pRetVal ) = 0;
      virtual HRESULT __stdcall GetType (
        /*[out,retval]*/ struct _Type * * pRetVal ) = 0;
      virtual HRESULT __stdcall InitializeLifetimeService (
        /*[out,retval]*/ VARIANT * pRetVal ) = 0;
      virtual HRESULT __stdcall GetLifetimeService (
        /*[out,retval]*/ VARIANT * pRetVal ) = 0;
      virtual HRESULT __stdcall get_Evidence (
        /*[out,retval]*/ struct _Evidence * * pRetVal ) = 0;
      virtual HRESULT __stdcall add_DomainUnload (
        /*[in]*/ struct _EventHandler * value ) = 0;
      virtual HRESULT __stdcall remove_DomainUnload (
        /*[in]*/ struct _EventHandler * value ) = 0;
      virtual HRESULT __stdcall add_AssemblyLoad (
        /*[in]*/ struct _AssemblyLoadEventHandler * value ) = 0;
      virtual HRESULT __stdcall remove_AssemblyLoad (
        /*[in]*/ struct _AssemblyLoadEventHandler * value ) = 0;
      virtual HRESULT __stdcall add_ProcessExit (
        /*[in]*/ struct _EventHandler * value ) = 0;
      virtual HRESULT __stdcall remove_ProcessExit (
        /*[in]*/ struct _EventHandler * value ) = 0;
      virtual HRESULT __stdcall add_TypeResolve (
        /*[in]*/ struct _ResolveEventHandler * value ) = 0;
      virtual HRESULT __stdcall remove_TypeResolve (
        /*[in]*/ struct _ResolveEventHandler * value ) = 0;
      virtual HRESULT __stdcall add_ResourceResolve (
        /*[in]*/ struct _ResolveEventHandler * value ) = 0;
      virtual HRESULT __stdcall remove_ResourceResolve (
        /*[in]*/ struct _ResolveEventHandler * value ) = 0;
      virtual HRESULT __stdcall add_AssemblyResolve (
        /*[in]*/ struct _ResolveEventHandler * value ) = 0;
      virtual HRESULT __stdcall remove_AssemblyResolve (
        /*[in]*/ struct _ResolveEventHandler * value ) = 0;
      virtual HRESULT __stdcall add_UnhandledException (
        /*[in]*/ struct _UnhandledExceptionEventHandler * value ) = 0;
      virtual HRESULT __stdcall remove_UnhandledException (
        /*[in]*/ struct _UnhandledExceptionEventHandler * value ) = 0;
      virtual HRESULT __stdcall DefineDynamicAssembly (
        /*[in]*/ struct _AssemblyName * name,
        /*[in]*/ enum AssemblyBuilderAccess access,
        /*[out,retval]*/ struct _AssemblyBuilder * * pRetVal ) = 0;
      virtual HRESULT __stdcall DefineDynamicAssembly_2 (
        /*[in]*/ struct _AssemblyName * name,
        /*[in]*/ enum AssemblyBuilderAccess access,
        /*[in]*/ BSTR dir,
        /*[out,retval]*/ struct _AssemblyBuilder * * pRetVal ) = 0;
      virtual HRESULT __stdcall DefineDynamicAssembly_3 (
        /*[in]*/ struct _AssemblyName * name,
        /*[in]*/ enum AssemblyBuilderAccess access,
        /*[in]*/ struct _Evidence * Evidence,
        /*[out,retval]*/ struct _AssemblyBuilder * * pRetVal ) = 0;
      virtual HRESULT __stdcall DefineDynamicAssembly_4 (
        /*[in]*/ struct _AssemblyName * name,
        /*[in]*/ enum AssemblyBuilderAccess access,
        /*[in]*/ struct _PermissionSet * requiredPermissions,
        /*[in]*/ struct _PermissionSet * optionalPermissions,
        /*[in]*/ struct _PermissionSet * refusedPermissions,
        /*[out,retval]*/ struct _AssemblyBuilder * * pRetVal ) = 0;
      virtual HRESULT __stdcall DefineDynamicAssembly_5 (
        /*[in]*/ struct _AssemblyName * name,
        /*[in]*/ enum AssemblyBuilderAccess access,
        /*[in]*/ BSTR dir,
        /*[in]*/ struct _Evidence * Evidence,
        /*[out,retval]*/ struct _AssemblyBuilder * * pRetVal ) = 0;
      virtual HRESULT __stdcall DefineDynamicAssembly_6 (
        /*[in]*/ struct _AssemblyName * name,
        /*[in]*/ enum AssemblyBuilderAccess access,
        /*[in]*/ BSTR dir,
        /*[in]*/ struct _PermissionSet * requiredPermissions,
        /*[in]*/ struct _PermissionSet * optionalPermissions,
        /*[in]*/ struct _PermissionSet * refusedPermissions,
        /*[out,retval]*/ struct _AssemblyBuilder * * pRetVal ) = 0;
      virtual HRESULT __stdcall DefineDynamicAssembly_7 (
        /*[in]*/ struct _AssemblyName * name,
        /*[in]*/ enum AssemblyBuilderAccess access,
        /*[in]*/ struct _Evidence * Evidence,
        /*[in]*/ struct _PermissionSet * requiredPermissions,
        /*[in]*/ struct _PermissionSet * optionalPermissions,
        /*[in]*/ struct _PermissionSet * refusedPermissions,
        /*[out,retval]*/ struct _AssemblyBuilder * * pRetVal ) = 0;
      virtual HRESULT __stdcall DefineDynamicAssembly_8 (
        /*[in]*/ struct _AssemblyName * name,
        /*[in]*/ enum AssemblyBuilderAccess access,
        /*[in]*/ BSTR dir,
        /*[in]*/ struct _Evidence * Evidence,
        /*[in]*/ struct _PermissionSet * requiredPermissions,
        /*[in]*/ struct _PermissionSet * optionalPermissions,
        /*[in]*/ struct _PermissionSet * refusedPermissions,
        /*[out,retval]*/ struct _AssemblyBuilder * * pRetVal ) = 0;
      virtual HRESULT __stdcall DefineDynamicAssembly_9 (
        /*[in]*/ struct _AssemblyName * name,
        /*[in]*/ enum AssemblyBuilderAccess access,
        /*[in]*/ BSTR dir,
        /*[in]*/ struct _Evidence * Evidence,
        /*[in]*/ struct _PermissionSet * requiredPermissions,
        /*[in]*/ struct _PermissionSet * optionalPermissions,
        /*[in]*/ struct _PermissionSet * refusedPermissions,
        /*[in]*/ VARIANT_BOOL IsSynchronized,
        /*[out,retval]*/ struct _AssemblyBuilder * * pRetVal ) = 0;
      virtual HRESULT __stdcall CreateInstance (
        /*[in]*/ BSTR AssemblyName,
        /*[in]*/ BSTR typeName,
        /*[out,retval]*/ struct _ObjectHandle * * pRetVal ) = 0;
      virtual HRESULT __stdcall CreateInstanceFrom (
        /*[in]*/ BSTR assemblyFile,
        /*[in]*/ BSTR typeName,
        /*[out,retval]*/ struct _ObjectHandle * * pRetVal ) = 0;
      virtual HRESULT __stdcall CreateInstance_2 (
        /*[in]*/ BSTR AssemblyName,
        /*[in]*/ BSTR typeName,
        /*[in]*/ SAFEARRAY * activationAttributes,
        /*[out,retval]*/ struct _ObjectHandle * * pRetVal ) = 0;
      virtual HRESULT __stdcall CreateInstanceFrom_2 (
        /*[in]*/ BSTR assemblyFile,
        /*[in]*/ BSTR typeName,
        /*[in]*/ SAFEARRAY * activationAttributes,
        /*[out,retval]*/ struct _ObjectHandle * * pRetVal ) = 0;
      virtual HRESULT __stdcall CreateInstance_3 (
        /*[in]*/ BSTR AssemblyName,
        /*[in]*/ BSTR typeName,
        /*[in]*/ VARIANT_BOOL ignoreCase,
        /*[in]*/ enum BindingFlags bindingAttr,
        /*[in]*/ struct _Binder * Binder,
        /*[in]*/ SAFEARRAY * args,
        /*[in]*/ struct _CultureInfo * culture,
        /*[in]*/ SAFEARRAY * activationAttributes,
        /*[in]*/ struct _Evidence * securityAttributes,
        /*[out,retval]*/ struct _ObjectHandle * * pRetVal ) = 0;
      virtual HRESULT __stdcall CreateInstanceFrom_3 (
        /*[in]*/ BSTR assemblyFile,
        /*[in]*/ BSTR typeName,
        /*[in]*/ VARIANT_BOOL ignoreCase,
        /*[in]*/ enum BindingFlags bindingAttr,
        /*[in]*/ struct _Binder * Binder,
        /*[in]*/ SAFEARRAY * args,
        /*[in]*/ struct _CultureInfo * culture,
        /*[in]*/ SAFEARRAY * activationAttributes,
        /*[in]*/ struct _Evidence * securityAttributes,
        /*[out,retval]*/ struct _ObjectHandle * * pRetVal ) = 0;
      virtual HRESULT __stdcall Load (
        /*[in]*/ struct _AssemblyName * assemblyRef,
        /*[out,retval]*/ struct _Assembly * * pRetVal ) = 0;
      virtual HRESULT __stdcall Load_2 (
        /*[in]*/ BSTR assemblyString,
        /*[out,retval]*/ struct _Assembly * * pRetVal ) = 0;
      virtual HRESULT __stdcall Load_3 (
        /*[in]*/ SAFEARRAY * rawAssembly,
        /*[out,retval]*/ struct _Assembly * * pRetVal ) = 0;
      virtual HRESULT __stdcall Load_4 (
        /*[in]*/ SAFEARRAY * rawAssembly,
        /*[in]*/ SAFEARRAY * rawSymbolStore,
        /*[out,retval]*/ struct _Assembly * * pRetVal ) = 0;
      virtual HRESULT __stdcall Load_5 (
        /*[in]*/ SAFEARRAY * rawAssembly,
        /*[in]*/ SAFEARRAY * rawSymbolStore,
        /*[in]*/ struct _Evidence * securityEvidence,
        /*[out,retval]*/ struct _Assembly * * pRetVal ) = 0;
      virtual HRESULT __stdcall Load_6 (
        /*[in]*/ struct _AssemblyName * assemblyRef,
        /*[in]*/ struct _Evidence * assemblySecurity,
        /*[out,retval]*/ struct _Assembly * * pRetVal ) = 0;
      virtual HRESULT __stdcall Load_7 (
        /*[in]*/ BSTR assemblyString,
        /*[in]*/ struct _Evidence * assemblySecurity,
        /*[out,retval]*/ struct _Assembly * * pRetVal ) = 0;
      virtual HRESULT __stdcall ExecuteAssembly (
        /*[in]*/ BSTR assemblyFile,
        /*[in]*/ struct _Evidence * assemblySecurity,
        /*[out,retval]*/ long * pRetVal ) = 0;
      virtual HRESULT __stdcall ExecuteAssembly_2 (
        /*[in]*/ BSTR assemblyFile,
        /*[out,retval]*/ long * pRetVal ) = 0;
      virtual HRESULT __stdcall ExecuteAssembly_3 (
        /*[in]*/ BSTR assemblyFile,
        /*[in]*/ struct _Evidence * assemblySecurity,
        /*[in]*/ SAFEARRAY * args,
        /*[out,retval]*/ long * pRetVal ) = 0;
      virtual HRESULT __stdcall get_FriendlyName (
        /*[out,retval]*/ BSTR * pRetVal ) = 0;
      virtual HRESULT __stdcall get_BaseDirectory (
        /*[out,retval]*/ BSTR * pRetVal ) = 0;
      virtual HRESULT __stdcall get_RelativeSearchPath (
        /*[out,retval]*/ BSTR * pRetVal ) = 0;
      virtual HRESULT __stdcall get_ShadowCopyFiles (
        /*[out,retval]*/ VARIANT_BOOL * pRetVal ) = 0;
      virtual HRESULT __stdcall GetAssemblies (
        /*[out,retval]*/ SAFEARRAY * * pRetVal ) = 0;
      virtual HRESULT __stdcall AppendPrivatePath (
        /*[in]*/ BSTR Path ) = 0;
      virtual HRESULT __stdcall ClearPrivatePath ( ) = 0;
      virtual HRESULT __stdcall SetShadowCopyPath (
        /*[in]*/ BSTR s ) = 0;
      virtual HRESULT __stdcall ClearShadowCopyPath ( ) = 0;
      virtual HRESULT __stdcall SetCachePath (
        /*[in]*/ BSTR s ) = 0;
      virtual HRESULT __stdcall SetData (
        /*[in]*/ BSTR name,
        /*[in]*/ VARIANT data ) = 0;
      virtual HRESULT __stdcall GetData (
        /*[in]*/ BSTR name,
        /*[out,retval]*/ VARIANT * pRetVal ) = 0;
      virtual HRESULT __stdcall SetAppDomainPolicy (
        /*[in]*/ struct _PolicyLevel * domainPolicy ) = 0;
      virtual HRESULT __stdcall SetThreadPrincipal (
        /*[in]*/ struct IPrincipal * principal ) = 0;
      virtual HRESULT __stdcall SetPrincipalPolicy (
        /*[in]*/ enum PrincipalPolicy policy ) = 0;
      virtual HRESULT __stdcall DoCallBack (
        /*[in]*/ struct _CrossAppDomainDelegate * theDelegate ) = 0;
      virtual HRESULT __stdcall get_DynamicDirectory (
        /*[out,retval]*/ BSTR * pRetVal ) = 0;
};


#pragma pack(pop)
