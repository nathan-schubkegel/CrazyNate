using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrazyNateManaged
{
  public static class ManagedEntryPoint
  {
    public static int Enter(string argument)
    {
      MessageBox.Show("ok, entered. " + argument + " AppDomain: " + AppDomain.CurrentDomain.Id.ToString());
      return 1337;
    }
  }
}
