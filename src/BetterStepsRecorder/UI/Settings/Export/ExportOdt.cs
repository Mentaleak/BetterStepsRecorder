using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BetterStepsRecorder.UI.Settings
{
    public partial class ExportOdt : UserControl
    {
        public ExportOdt()
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
            var odtSettings = BSRSettings.Current.ExportOptions.Odt;

            chkSummary.Checked = odtSettings.ShowSummary;
            chkGeneratedDate.Checked = odtSettings.ShowGeneratedDate;
            chkTableOfContents.Checked = odtSettings.ShowTableOfContents;
            chkStepTimestamps.Checked = odtSettings.ShowStepTimestamps;
            chkAction.Checked = odtSettings.ShowAction;
            chkApplication.Checked = odtSettings.ShowApplication;
            chkWindow.Checked = odtSettings.ShowWindow;
            chkElement.Checked = odtSettings.ShowElement;
            chkElementType.Checked = odtSettings.ShowElementType;
            chkMousePosition.Checked = odtSettings.ShowMousePosition;
        }

        private void Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            var odtSettings = BSRSettings.Current.ExportOptions.Odt;

            odtSettings.ShowSummary = chkSummary.Checked;
            odtSettings.ShowGeneratedDate = chkGeneratedDate.Checked;
            odtSettings.ShowTableOfContents = chkTableOfContents.Checked;
            odtSettings.ShowStepTimestamps = chkStepTimestamps.Checked;
            odtSettings.ShowAction = chkAction.Checked;
            odtSettings.ShowApplication = chkApplication.Checked;
            odtSettings.ShowWindow = chkWindow.Checked;
            odtSettings.ShowElement = chkElement.Checked;
            odtSettings.ShowElementType = chkElementType.Checked;
            odtSettings.ShowMousePosition = chkMousePosition.Checked;

            BSRSettings.Current.Save();
        }
    }
}
