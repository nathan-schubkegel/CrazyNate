using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyNateManaged.Injection
{
  public enum DllLoadBehavior
  {
    /// <summary>
    /// The target process will be asked to load the DLL by full file path.
    /// Relative DLL file paths will be resolved to full paths in the 'injecting' process first.
    /// NOTE: I *think* this gets the DLL into the "LoadFrom" context... if that matters to you. Google "LoadFromContext" and look for StackOverflow discussions for more info.
    /// NOTE: The target process will hold locks on the DLL until it quits!
    /// </summary>
    LoadFromFullPath = 0,

    /// <summary>
    /// The target process will be asked to load the DLL by filename (without path info).
    /// This is the "traditional" way DLLs are loaded into a process.
    /// Path info provided with the DLLs will be stripped in the 'injecting' process first.
    /// Choose an appropriate <c>CopyBehavior</c> to ensure the DLL will be found successfully by the target process.
    /// NOTE: I *think* this gets the DLL into the "Load" context... if that matters to you. Google "LoadFromContext" and look for StackOverflow discussions for more info.
    /// NOTE: The target process will hold locks on the DLL until it quits!
    /// </summary>
    LoadFromFileName = 1,

    /// <summary>
    /// The target process will be asked to load the DLL using the provided string.
    /// </summary>
    LoadAsProvided = 2,

    /// <summary>
    /// The target process will not be explicitly asked to load the DLL.
    /// (this option could be used in combination with a CopyBehavior to get
    /// dependency dlls copied to the destination process folder)
    /// </summary>
    None = 3,
  }
}
