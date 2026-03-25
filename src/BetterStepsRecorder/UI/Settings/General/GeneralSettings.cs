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
            
            // Auto-save when checkbox changes
            chkMinimizeOnStart.CheckedChanged += Checkbox_CheckedChanged;
        }

        private void LoadSettings()
        {
            var settings = BSRSettings.Load();
            chkMinimizeOnStart.Checked = settings.MinimizeOnStartRecording;
        }

        private void Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            var settings = BSRSettings.Load();
            settings.MinimizeOnStartRecording = chkMinimizeOnStart.Checked;
            settings.Save();

        }
    }
}
