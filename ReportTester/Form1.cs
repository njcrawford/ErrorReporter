using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReportTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnCrash_Click(object sender, EventArgs e)
        {
            int x = 100;
            int y = 0;

            // Do bad stuff!
            int z = x / y;
        }

        private void btnManualReport_Click(object sender, EventArgs e)
        {
            Exception test = new Exception("A test exception");
            NJCrawford.ErrorReporter.ReportError(test, false);
        }
    }
}
