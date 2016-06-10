using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CrazyNateManaged;
using CrazyNateSharpDisasm;
using System.Runtime.Serialization;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Xml;

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
        // ask user to pick a BPL
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

    private void button4_Click(object sender, EventArgs e)
    {
      // Test acquisition of Delphi BPL exports
      using (OpenFileDialog dialog = new OpenFileDialog())
      {
        // ask user to pick a BPL
        dialog.Filter = "Delphi BPLs (*.bpl)|*.bpl|All files|*";
        if (dialog.ShowDialog() == DialogResult.OK)
        {
          List<string> exports = DllExports.GetExports(dialog.FileName);
          
          // ask user to pick an exported function
          string selectedExportName = null;
          using (ExportPickerForm form = new ExportPickerForm())
          {
            form.Setup(exports.OrderBy(x => x));
            DialogResult r2 = form.ShowDialog(this);
            if (r2 == System.Windows.Forms.DialogResult.OK)
            {
              selectedExportName = form.GetSelectedExport();
            }
          }

          if (selectedExportName != null)
          {
            // find desired export
            int index = exports.FindIndex(x => x == selectedExportName);
            string export = exports[index];

            // find next export (that's where we'll stop reading bytes)
            if (index == exports.Count - 1 || index == 0)
            {
              MessageBox.Show("first and last export not supported");
              return;
            }

            string prevExport = exports[index - 1];
            string nextExport = exports[index + 1];

            // Load the bpl
            Environment.CurrentDirectory = Path.GetDirectoryName(dialog.FileName);
            IntPtr hModule = Win32.LoadLibraryW(dialog.FileName);
            if (hModule == IntPtr.Zero)
            {
              MessageBox.Show("failed to load " + dialog.FileName);
              return;
            }
            IntPtr exportAddress = Win32.GetProcAddress(hModule, export);
            IntPtr nextExportAddress = Win32.GetProcAddress(hModule, nextExport);
            IntPtr prevExportAddress = Win32.GetProcAddress(hModule, prevExport);
            if (exportAddress == IntPtr.Zero ||
                nextExportAddress == IntPtr.Zero ||
                prevExportAddress == IntPtr.Zero)
            {
              MessageBox.Show("failed to GetProcAddress");
              return;
            }

            int numBytes = Math.Max(
              (int)(nextExportAddress.ToInt64() - exportAddress.ToInt64()),
              (int)(prevExportAddress.ToInt64() - exportAddress.ToInt64()));

            if (numBytes < 0)
            {
              MessageBox.Show("exports not ordered");
            }

            if (numBytes > 1000000)
            {
              MessageBox.Show("method too long");
              return;
            }

            byte[] bytes = new byte[numBytes];
            for (int i = 0; i < numBytes; i++)
            {
              bytes[i] = Marshal.ReadByte(exportAddress, i);
            }
            List<Instruction> instructions = OpCodeReader.Decompile(bytes);
            var disassemblyLines = instructions.ToPrintableStrings();

            // write to a local xml file
            DataContractSerializer serializer = new DataContractSerializer(disassemblyLines.GetType());
            var xmlSettings = new XmlWriterSettings { Indent = true };
            using (var xmlWriter = XmlWriter.Create("DelphiExportOpcodes.xml", xmlSettings))
            {
              serializer.WriteObject(xmlWriter, disassemblyLines);
            }

            // open the xml file
            using (Process p = new Process())
            {
              p.StartInfo = new ProcessStartInfo("DelphiExportOpcodes.xml");
              p.Start();
            }
          }
        }
      }
    }
  }
}
