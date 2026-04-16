using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BetterStepsRecorder.UI.Settings
{
    public partial class ExportMarkdown : UserControl
    {
        public ExportMarkdown()
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
            var mdSettings = BSRSettings.Current.ExportOptions.Markdown;

            chkSummary.Checked = mdSettings.ShowSummary;
            chkGeneratedDate.Checked = mdSettings.ShowGeneratedDate;
            chkTableOfContents.Checked = mdSettings.ShowTableOfContents;
            chkStepTimestamps.Checked = mdSettings.ShowStepTimestamps;
            chkAction.Checked = mdSettings.ShowAction;
            chkApplication.Checked = mdSettings.ShowApplication;
            chkWindow.Checked = mdSettings.ShowWindow;
            chkElement.Checked = mdSettings.ShowElement;
            chkElementType.Checked = mdSettings.ShowElementType;
            chkMousePosition.Checked = mdSettings.ShowMousePosition;
        }

        private void Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            var mdSettings = BSRSettings.Current.ExportOptions.Markdown;

            mdSettings.ShowSummary = chkSummary.Checked;
            mdSettings.ShowGeneratedDate = chkGeneratedDate.Checked;
            mdSettings.ShowTableOfContents = chkTableOfContents.Checked;
            mdSettings.ShowStepTimestamps = chkStepTimestamps.Checked;
            mdSettings.ShowAction = chkAction.Checked;
            mdSettings.ShowApplication = chkApplication.Checked;
            mdSettings.ShowWindow = chkWindow.Checked;
            mdSettings.ShowElement = chkElement.Checked;
            mdSettings.ShowElementType = chkElementType.Checked;
            mdSettings.ShowMousePosition = chkMousePosition.Checked;

            BSRSettings.Current.Save();
        }
    }
}
