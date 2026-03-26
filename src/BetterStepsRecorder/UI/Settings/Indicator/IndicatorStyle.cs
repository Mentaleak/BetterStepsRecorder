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
            var settings = BSRSettings.Current;

            rdoArrow.Checked = settings.Indicator.Style == ClickIndicatorStyle.Arrow;
            rdoCircle.Checked = settings.Indicator.Style == ClickIndicatorStyle.Circle;
            rdoCursor.Checked = settings.Indicator.Style == ClickIndicatorStyle.Cursor;
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton { Checked: true, Tag: ClickIndicatorStyle style })
            {
                BSRSettings.Current.Indicator.Style = style;
                BSRSettings.Current.Save();
            }
        }
    }
}
