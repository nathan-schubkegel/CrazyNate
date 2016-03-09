using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace CrazyNateManaged
{
  [DataContract]
  public class DllExport
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public DbgHelp.SymbolInfo Info { get; set; }
  }
}
