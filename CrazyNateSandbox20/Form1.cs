using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CrazyNateSandbox20
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

    protected override bool ShowWithoutActivation
    {
      get
      {
        return true;
      }
    }
  }
}
