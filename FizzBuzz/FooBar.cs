using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FizzBuzz
{
  [ComVisible(true)]
  public class FooBar : IFoo
  {
    public void DoStuff()
    {
      MessageBox.Show("hahaha");
    }
  }
}
