﻿using System;
using System.Collections.Generic;
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
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
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
  }
}
