using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrazyNateManaged;
using System.Runtime.Serialization;
using System.IO;
using System.Diagnostics;

namespace CrazyNateSelfLauncher
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void button2_Click(object sender, System.EventArgs e)
    {
      // This part tests CrazyNate.LaunchCrazyNateManaged
      StringBuilder builder = new StringBuilder((int)CrazyNateDll.GetInputOutputBufferCharCount());
      builder.Append("CrazyNate.dll");
      builder.Append((char)0);
      builder.Append("CrazyNateManaged.dll");
      builder.Append((char)0);
      builder.Append("CrazyNateManaged.ManagedEntryPoint");
      builder.Append((char)0);
      builder.Append("Enter");
      builder.Append((char)0);
      builder.Append("and I mean it!");
      builder.Append((char)0);
      CrazyNateDll.LaunchCrazyNateManaged(builder);
    }

    private void button1_Click(object sender, EventArgs e)
    {
      // This part tests acquisition of unmanaged DLL exports
      List<string> exports = CrazyNateManaged.DllExports.GetExports("CrazyNate.dll");
      // write to a local xml file
      DataContractSerializer serializer = new DataContractSerializer(typeof(List<string>));
      using (FileStream fs = new FileStream("CrazyNate_Exports.xml", FileMode.Create))
      {
        serializer.WriteObject(fs, exports);
      }
      // open the xml file
      using (Process p = new Process())
      {
        p.StartInfo = new ProcessStartInfo("CrazyNate_Exports.xml");
        p.Start();
      }
    }

    private void button3_Click(object sender, System.EventArgs e)
    {
      // Test acquisition of Delphi BPL exports
      using (OpenFileDialog dialog = new OpenFileDialog())
      {
        dialog.Filter = "Delphi BPLs (*.bpl)|*.bpl|All files|*";
        if (dialog.ShowDialog() == DialogResult.OK)
        {
          List<string> exports = DllExports.GetExports(dialog.FileName);
          exports.Sort();

          // write to a local xml file
          DataContractSerializer serializer = new DataContractSerializer(typeof(List<string>));
          using (FileStream fs = new FileStream("DelphiBplExports.xml", FileMode.Create))
          {
            serializer.WriteObject(fs, exports);
          }
          // open the xml file
          using (Process p = new Process())
          {
            p.StartInfo = new ProcessStartInfo("DelphiBplExports.xml");
            p.Start();
          }

        }
      }
    }


  }
}
