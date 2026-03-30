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
            var behavior = BSRSettings.Current.General.MinimizeOnStartRecording;
            rbDoNotMinimize.Checked = behavior == MinimizeBehavior.DoNotMinimize;
            rbMinimizeToTaskbar.Checked = behavior == MinimizeBehavior.MinimizeToTaskbar;
            rbMinimizeToSystemTray.Checked = behavior == MinimizeBehavior.MinimizeToSystemTray;
            chkAllowRecordSelf.Checked = BSRSettings.Current.General.AllowRecordSelf;
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton rb && rb.Checked)
            {
                if (rb == rbDoNotMinimize)
                    BSRSettings.Current.General.MinimizeOnStartRecording = MinimizeBehavior.DoNotMinimize;
                else if (rb == rbMinimizeToTaskbar)
                    BSRSettings.Current.General.MinimizeOnStartRecording = MinimizeBehavior.MinimizeToTaskbar;
                else if (rb == rbMinimizeToSystemTray)
                    BSRSettings.Current.General.MinimizeOnStartRecording = MinimizeBehavior.MinimizeToSystemTray;

                BSRSettings.Current.Save();
            }
        }

        private void Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            BSRSettings.Current.General.AllowRecordSelf = chkAllowRecordSelf.Checked;
            BSRSettings.Current.Save();
        }
    }
}
