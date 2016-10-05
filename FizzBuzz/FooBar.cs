using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FizzBuzz
{
  [ComVisible(true)]
  public class FooBar : IFoo, IBar
  {
    public void DoStuff()
    {
      MessageBox.Show("stuff");
    }

    public void DoSomethingElse()
    {
      MessageBox.Show("something else");
    }
  }
}
