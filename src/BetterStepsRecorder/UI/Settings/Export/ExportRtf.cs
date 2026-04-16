using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BetterStepsRecorder.UI.Settings
{
    public partial class ExportRtf : UserControl
    {
        public ExportRtf()
        {
            InitializeComponent();
            LoadSettings();

            // Auto-save when any checkbox changes
            chkSummary.CheckedChanged += Checkbox_CheckedChanged;
            chkGeneratedDate.CheckedChanged += Checkbox_CheckedChanged;
            chkTableOfContents.CheckedChanged += Checkbox_CheckedChanged;
            chkStepTimestamps.CheckedChanged += Checkbox_CheckedChanged;
            chkAction.CheckedChanged += Checkbox_CheckedChanged;
            chkApplication.CheckedChanged += Checkbox_CheckedChanged;
            chkWindow.CheckedChanged += Checkbox_CheckedChanged;
            chkElement.CheckedChanged += Checkbox_CheckedChanged;
            chkElementType.CheckedChanged += Checkbox_CheckedChanged;
            chkMousePosition.CheckedChanged += Checkbox_CheckedChanged;
        }

        private void LoadSettings()
        {
            var rtfSettings = BSRSettings.Current.ExportOptions.Rtf;

            chkSummary.Checked = rtfSettings.ShowSummary;
            chkGeneratedDate.Checked = rtfSettings.ShowGeneratedDate;
            chkTableOfContents.Checked = rtfSettings.ShowTableOfContents;
            chkStepTimestamps.Checked = rtfSettings.ShowStepTimestamps;
            chkAction.Checked = rtfSettings.ShowAction;
            chkApplication.Checked = rtfSettings.ShowApplication;
            chkWindow.Checked = rtfSettings.ShowWindow;
            chkElement.Checked = rtfSettings.ShowElement;
            chkElementType.Checked = rtfSettings.ShowElementType;
            chkMousePosition.Checked = rtfSettings.ShowMousePosition;
        }

        private void Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            var rtfSettings = BSRSettings.Current.ExportOptions.Rtf;

            rtfSettings.ShowSummary = chkSummary.Checked;
            rtfSettings.ShowGeneratedDate = chkGeneratedDate.Checked;
            rtfSettings.ShowTableOfContents = chkTableOfContents.Checked;
            rtfSettings.ShowStepTimestamps = chkStepTimestamps.Checked;
            rtfSettings.ShowAction = chkAction.Checked;
            rtfSettings.ShowApplication = chkApplication.Checked;
            rtfSettings.ShowWindow = chkWindow.Checked;
            rtfSettings.ShowElement = chkElement.Checked;
            rtfSettings.ShowElementType = chkElementType.Checked;
            rtfSettings.ShowMousePosition = chkMousePosition.Checked;

            BSRSettings.Current.Save();
        }
    }
}
