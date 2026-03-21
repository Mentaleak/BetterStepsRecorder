using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BetterStepsRecorder.UI.Settings
{
    public partial class IndicatorColor : UserControl
    {
        public IndicatorColor()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            var settings = RecordingSettings.Load();
            UpdateColorDisplay(settings.ArrowColor);
        }

        private void UpdateColorDisplay(Color color)
        {
            panelColorPreview.BackColor = color;
            lblColorValue.Text = $"RGB({color.R}, {color.G}, {color.B})";
        }

        private void btnChooseColor_Click(object sender, EventArgs e)
        {
            using (var dlg = new ColorDialog())
            {
                var settings = RecordingSettings.Load();
                dlg.Color = settings.ArrowColor;
                dlg.FullOpen = true;
                
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    SaveColor(dlg.Color);
                    UpdateColorDisplay(dlg.Color);
                }
            }
        }

        private void SaveColor(Color color)
        {
            Program.ArrowColor = color;
            RecordingSettings.SaveCurrent();
        }
    }
}
