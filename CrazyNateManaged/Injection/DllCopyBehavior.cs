using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyNateManaged.Injection
{
  public enum DllCopyBehavior
  {
    /// <summary>
    /// No DLL copying occurs before the injection.
    /// </summary>
    None = 0,

    /// <summary>
    /// The listed DLLs are copied to the same folder as the target process EXE.
    /// </summary>
    CopyToTargetProcessExeFolder = 1,

    /// <summary>
    /// The listed DLLs are copied to a folder named "CrazyNate" under the folder holding the target process EXE.
    /// </summary>
    CopyToFolderUnderTargetProcessExeFolder = 2,
  }
}
