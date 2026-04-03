using System;
using System.Drawing;
using System.Windows.Forms;

namespace BetterStepsRecorder
{
    public partial class MainForm
    {
        /// <summary>
        /// Initializes the step text formatting toolbar.
        /// </summary>
        private void InitializeStepTextToolbar()
        {
            // Update toolbar state when selection changes
            richTextBox_stepText.SelectionChanged += richTextBox_stepText_SelectionChanged;
        }

        /// <summary>
        /// Updates the formatting toolbar to reflect the current selection state.
        /// </summary>
        private void richTextBox_stepText_SelectionChanged(object? sender, EventArgs e)
        {
            UpdateFormattingToolbarState();
        }

        /// <summary>
        /// Syncs toolbar button checked states with the current selection font.
        /// </summary>
        private void UpdateFormattingToolbarState()
        {
            var font = richTextBox_stepText.SelectionFont;
            if (font != null)
            {
                boldButton.Checked = font.Bold;
                italicButton.Checked = font.Italic;
                underlineButton.Checked = font.Underline;
                strikethroughButton.Checked = font.Strikeout;
            }
        }

        /// <summary>
        /// Toggles a font style on the current selection.
        /// </summary>
        private void ToggleSelectionStyle(FontStyle style)
        {
            var font = richTextBox_stepText.SelectionFont;
            if (font == null)
            {
                font = richTextBox_stepText.Font;
            }

            FontStyle newStyle;
            if (font.Style.HasFlag(style))
                newStyle = font.Style & ~style;
            else
                newStyle = font.Style | style;

            richTextBox_stepText.SelectionFont = new Font(font.FontFamily, font.Size, newStyle);
            richTextBox_stepText.Focus();
        }

        private void boldButton_Click(object sender, EventArgs e) => ToggleSelectionStyle(FontStyle.Bold);
        private void italicButton_Click(object sender, EventArgs e) => ToggleSelectionStyle(FontStyle.Italic);
        private void underlineButton_Click(object sender, EventArgs e) => ToggleSelectionStyle(FontStyle.Underline);
        private void strikethroughButton_Click(object sender, EventArgs e) => ToggleSelectionStyle(FontStyle.Strikeout);

        private void fontColorButton_Click(object sender, EventArgs e)
        {
            using var dlg = new ColorDialog();
            dlg.Color = richTextBox_stepText.SelectionColor;
            dlg.FullOpen = true;
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                richTextBox_stepText.SelectionColor = dlg.Color;
                fontColorButton.ForeColor = dlg.Color;
            }
            richTextBox_stepText.Focus();
        }

        private void highlightColorButton_Click(object sender, EventArgs e)
        {
            using var dlg = new ColorDialog();
            dlg.Color = richTextBox_stepText.SelectionBackColor;
            dlg.FullOpen = true;
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                richTextBox_stepText.SelectionBackColor = dlg.Color;
                highlightColorButton.BackColor = dlg.Color;
            }
            richTextBox_stepText.Focus();
        }
    }
}
