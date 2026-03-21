using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BetterStepsRecorder.UI.Settings
{
    public partial class IndicatorStyle : UserControl
    {
        public IndicatorStyle()
        {
            InitializeComponent();
            LoadSettings();

            // Auto-save when selection changes
            rdoArrow.CheckedChanged += RadioButton_CheckedChanged;
            rdoCircle.CheckedChanged += RadioButton_CheckedChanged;
            rdoCursor.CheckedChanged += RadioButton_CheckedChanged;
        }

        private void LoadSettings()
        {
            var settings = RecordingSettings.Load();

            rdoArrow.Checked = settings.IndicatorStyle == ClickIndicatorStyle.Arrow;
            rdoCircle.Checked = settings.IndicatorStyle == ClickIndicatorStyle.Circle;
            rdoCursor.Checked = settings.IndicatorStyle == ClickIndicatorStyle.Cursor;
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // Only save when a radio button is checked (not when unchecked)
            if (sender is RadioButton rb && rb.Checked)
            {
                SaveSettings();
            }
        }

        private void SaveSettings()
        {
            ClickIndicatorStyle selectedStyle;

            if (rdoCircle.Checked)
                selectedStyle = ClickIndicatorStyle.Circle;
            else if (rdoCursor.Checked)
                selectedStyle = ClickIndicatorStyle.Cursor;
            else
                selectedStyle = ClickIndicatorStyle.Arrow;

            Program.IndicatorStyle = selectedStyle;
            RecordingSettings.SaveCurrent();
        }
    }
}
