using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CrazyNateSandbox451
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      lblAppDomain.Text = AppDomain.CurrentDomain.Id.ToString();
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr LoadLibraryW(
      [MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

    public static string GetLastErrorMessage()
    {
      return (new Win32Exception(Marshal.GetLastWin32Error())).Message;
    }

    private void button1_Click(object sender, EventArgs e)
    {
      IntPtr hModule = LoadLibraryW(textBox1.Text);
      string errorMessage = GetLastErrorMessage();
      MessageBox.Show("hModule: " + hModule.ToString() + " - " + errorMessage);
    }

    protected override bool ShowWithoutActivation
    {
      get
      {
        return true;
      }
    }
  }
}
