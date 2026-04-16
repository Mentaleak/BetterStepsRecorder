using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BetterStepsRecorder.UI.Settings
{
    public partial class ExportObsidian : UserControl
    {
        public ExportObsidian()
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
            var obsidianSettings = BSRSettings.Current.ExportOptions.Obsidian;

            chkSummary.Checked = obsidianSettings.ShowSummary;
            chkGeneratedDate.Checked = obsidianSettings.ShowGeneratedDate;
            chkTableOfContents.Checked = obsidianSettings.ShowTableOfContents;
            chkStepTimestamps.Checked = obsidianSettings.ShowStepTimestamps;
            chkAction.Checked = obsidianSettings.ShowAction;
            chkApplication.Checked = obsidianSettings.ShowApplication;
            chkWindow.Checked = obsidianSettings.ShowWindow;
            chkElement.Checked = obsidianSettings.ShowElement;
            chkElementType.Checked = obsidianSettings.ShowElementType;
            chkMousePosition.Checked = obsidianSettings.ShowMousePosition;
        }

        private void Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            var obsidianSettings = BSRSettings.Current.ExportOptions.Obsidian;

            obsidianSettings.ShowSummary = chkSummary.Checked;
            obsidianSettings.ShowGeneratedDate = chkGeneratedDate.Checked;
            obsidianSettings.ShowTableOfContents = chkTableOfContents.Checked;
            obsidianSettings.ShowStepTimestamps = chkStepTimestamps.Checked;
            obsidianSettings.ShowAction = chkAction.Checked;
            obsidianSettings.ShowApplication = chkApplication.Checked;
            obsidianSettings.ShowWindow = chkWindow.Checked;
            obsidianSettings.ShowElement = chkElement.Checked;
            obsidianSettings.ShowElementType = chkElementType.Checked;
            obsidianSettings.ShowMousePosition = chkMousePosition.Checked;

            BSRSettings.Current.Save();
        }
    }
}
