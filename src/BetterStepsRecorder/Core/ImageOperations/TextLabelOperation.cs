using System;
using System.Drawing;

namespace BetterStepsRecorder.Core.ImageOperations
{
    /// <summary>
    /// Operation that draws a text label with background
    /// </summary>
    [Serializable]
    public class TextLabelOperation : ImageOperation
    {
        public string Text { get; set; } = "";
        public Rectangle Region { get; set; }
        public Color BackgroundColor { get; set; } = Color.FromArgb(200, 0, 120, 215);
        public Color TextColor { get; set; } = Color.White;
        public float FontSize { get; set; } = 16f;
        public string FontFamily { get; set; } = "Segoe UI";

        public override string Description => "Text Label";

        public TextLabelOperation() { }

        public TextLabelOperation(string text, Rectangle region, Color? backgroundColor = null, Color? textColor = null)
        {
            Text = text;
            Region = region;
            if (backgroundColor.HasValue) BackgroundColor = backgroundColor.Value;
            if (textColor.HasValue) TextColor = textColor.Value;
        }

        public override void Apply(Bitmap bitmap)
        {
            if (string.IsNullOrWhiteSpace(Text) || Region.Width <= 0 || Region.Height <= 0) return;

            using var g = Graphics.FromImage(bitmap);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            // Draw background
            using (var bgBrush = new SolidBrush(BackgroundColor))
            {
                g.FillRectangle(bgBrush, Region);
            }

            // Draw text
            using (var font = new Font(FontFamily, FontSize, FontStyle.Regular))
            using (var textBrush = new SolidBrush(TextColor))
            {
                g.DrawString(Text, font, textBrush, Region.X + 6, Region.Y + 3);
            }
        }

        public override ImageOperation Clone()
        {
            return new TextLabelOperation(Text, Region, BackgroundColor, TextColor)
            {
                Id = this.Id,
                CreatedAt = this.CreatedAt,
                FontSize = this.FontSize,
                FontFamily = this.FontFamily
            };
        }
    }
}
