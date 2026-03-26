using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BetterStepsRecorder.UI.Settings
{
    public partial class ExportHtml : UserControl
    {
        public ExportHtml()
        {
            InitializeComponent();
            LoadSettings();

            // Auto-save when any checkbox changes
            chkSummary.CheckedChanged += Checkbox_CheckedChanged;
            chkGeneratedDate.CheckedChanged += Checkbox_CheckedChanged;
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
            var htmlSettings = BSRSettings.Current.ExportOptions.Html;

            chkSummary.Checked = htmlSettings.ShowSummary;
            chkGeneratedDate.Checked = htmlSettings.ShowGeneratedDate;
            chkStepTimestamps.Checked = htmlSettings.ShowStepTimestamps;
            chkAction.Checked = htmlSettings.ShowAction;
            chkApplication.Checked = htmlSettings.ShowApplication;
            chkWindow.Checked = htmlSettings.ShowWindow;
            chkElement.Checked = htmlSettings.ShowElement;
            chkElementType.Checked = htmlSettings.ShowElementType;
            chkMousePosition.Checked = htmlSettings.ShowMousePosition;
        }

        private void Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            var htmlSettings = BSRSettings.Current.ExportOptions.Html;

            htmlSettings.ShowSummary = chkSummary.Checked;
            htmlSettings.ShowGeneratedDate = chkGeneratedDate.Checked;
            htmlSettings.ShowStepTimestamps = chkStepTimestamps.Checked;
            htmlSettings.ShowAction = chkAction.Checked;
            htmlSettings.ShowApplication = chkApplication.Checked;
            htmlSettings.ShowWindow = chkWindow.Checked;
            htmlSettings.ShowElement = chkElement.Checked;
            htmlSettings.ShowElementType = chkElementType.Checked;
            htmlSettings.ShowMousePosition = chkMousePosition.Checked;

            BSRSettings.Current.Save();
        }
    }
}
