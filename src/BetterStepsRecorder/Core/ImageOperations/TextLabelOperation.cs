using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BetterStepsRecorder.Core.ImageOperations
{
    /// <summary>
    /// Operation that draws a text label with outer border/glow
    /// </summary>
    [Serializable]
    public class TextLabelOperation : ImageOperation
    {
        public string Text { get; set; } = "";
        public Rectangle Region { get; set; }
        public Color InnerColor { get; set; } = Color.White;
        public Color OuterColor { get; set; } = Color.Black;
        public float FontSize { get; set; } = 16f;
        public string FontFamily { get; set; } = "Segoe UI";
        public float OutlineWidth { get; set; } = 3f;

        public override string Description => "Text Label";

        public TextLabelOperation() { }

        public TextLabelOperation(string text, Rectangle region, Color? innerColor = null, Color? outerColor = null)
        {
            Text = text;
            Region = region;
            if (innerColor.HasValue) InnerColor = innerColor.Value;
            if (outerColor.HasValue) OuterColor = outerColor.Value;
        }

        public override void Apply(Bitmap bitmap)
        {
            if (string.IsNullOrWhiteSpace(Text)) return;

            using var g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;

            using (var font = new Font(FontFamily, FontSize, FontStyle.Bold))
            using (var path = new GraphicsPath())
            {
                path.AddString(Text, font.FontFamily, (int)FontStyle.Bold, 
                    g.DpiY * FontSize / 72, new PointF(Region.X + 6, Region.Y + 3), 
                    StringFormat.GenericDefault);

                // Draw outer border/glow
                using (var outlinePen = new Pen(OuterColor, OutlineWidth))
                {
                    outlinePen.LineJoin = LineJoin.Round;
                    g.DrawPath(outlinePen, path);
                }

                // Draw inner fill
                using (var innerBrush = new SolidBrush(InnerColor))
                {
                    g.FillPath(innerBrush, path);
                }
            }
        }

        public override ImageOperation Clone()
        {
            return new TextLabelOperation(Text, Region, InnerColor, OuterColor)
            {
                Id = this.Id,
                CreatedAt = this.CreatedAt,
                FontSize = this.FontSize,
                FontFamily = this.FontFamily,
                OutlineWidth = this.OutlineWidth
            };
        }
    }
}
