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
        private HtmlExportSettings _settings;

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
            _settings = HtmlExportSettings.Load();
            
            chkSummary.Checked = _settings.ShowSummary;
            chkGeneratedDate.Checked = _settings.ShowGeneratedDate;
            chkStepTimestamps.Checked = _settings.ShowStepTimestamps;
            chkAction.Checked = _settings.ShowAction;
            chkApplication.Checked = _settings.ShowApplication;
            chkWindow.Checked = _settings.ShowWindow;
            chkElement.Checked = _settings.ShowElement;
            chkElementType.Checked = _settings.ShowElementType;
            chkMousePosition.Checked = _settings.ShowMousePosition;
        }

        private void Checkbox_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void SaveSettings()
        {
            _settings.ShowSummary = chkSummary.Checked;
            _settings.ShowGeneratedDate = chkGeneratedDate.Checked;
            _settings.ShowStepTimestamps = chkStepTimestamps.Checked;
            _settings.ShowAction = chkAction.Checked;
            _settings.ShowApplication = chkApplication.Checked;
            _settings.ShowWindow = chkWindow.Checked;
            _settings.ShowElement = chkElement.Checked;
            _settings.ShowElementType = chkElementType.Checked;
            _settings.ShowMousePosition = chkMousePosition.Checked;
            _settings.Save();
        }
    }
}
