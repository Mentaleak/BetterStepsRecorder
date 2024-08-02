using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Better_Steps_Recorder
{
    public partial class HelpPopup : Form
    {
        public HelpPopup()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://github.com/Mentaleak/BetterStepsRecorder",
                UseShellExecute = true
            });
        }

        private void button_CloseHelp_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
