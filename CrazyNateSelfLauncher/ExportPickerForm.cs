using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrazyNateSelfLauncher
{
  public partial class ExportPickerForm : Form
  {
    public ExportPickerForm()
    {
      InitializeComponent();
    }

    public void Setup(IEnumerable<string> dllExportNames)
    {
      listBox1.Items.Clear();
      listBox1.Items.AddRange(dllExportNames.Cast<object>().ToArray());
      listBox1.SelectedIndex = listBox1.Items.Count >= 0 ? 0 : -1;
      ActiveControl = listBox1;
    }

    public string GetSelectedExport()
    {
      return (string)(listBox1.SelectedIndex >= 0 ? listBox1.Items[listBox1.SelectedIndex] : null);
    }

    private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      int i = listBox1.IndexFromPoint(e.Location);
      listBox1.SelectedIndex = i;
      DialogResult = DialogResult.OK;
    }
  }
}
