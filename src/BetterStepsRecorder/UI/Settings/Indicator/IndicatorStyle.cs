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
        }

        private void LoadSettings()
        {
            var settings = BSRSettings.Load();

            rdoArrow.Checked = settings.IndicatorStyle == ClickIndicatorStyle.Arrow;
            rdoCircle.Checked = settings.IndicatorStyle == ClickIndicatorStyle.Circle;
            rdoCursor.Checked = settings.IndicatorStyle == ClickIndicatorStyle.Cursor;
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton { Checked: true, Tag: ClickIndicatorStyle style })
            {
                Program.IndicatorStyle = style;
                BSRSettings.SaveCurrent();
            }
        }
    }
}
