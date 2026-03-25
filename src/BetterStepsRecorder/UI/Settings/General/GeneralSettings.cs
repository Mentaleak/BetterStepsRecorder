using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BetterStepsRecorder.UI.Settings
{
    public partial class GeneralSettings : UserControl
    {
        public GeneralSettings()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            chkMinimizeOnStart.Checked = BSRSettings.Current.MinimizeOnStartRecording;
        }

        private void Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            BSRSettings.Current.MinimizeOnStartRecording = chkMinimizeOnStart.Checked;
            BSRSettings.Current.Save();
        }
    }
}
