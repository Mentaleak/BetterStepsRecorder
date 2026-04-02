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

        // Store initial region height to maintain scaling ratio
        public int InitialRegionHeight { get; set; }

        public override string Description => "Text Label";

        public TextLabelOperation() { }

        public TextLabelOperation(string text, Rectangle region, Color? innerColor = null, Color? outerColor = null)
        {
            Text = text;
            Region = region;
            InitialRegionHeight = region.Height > 0 ? region.Height : 30; // Default to 30 if region height is 0

            // Calculate font size to fit the initial box height
            // Use approximately 50% of box height for font size to leave room for padding and outline
            // Minimum font size of 8pt, maximum of 200pt for reasonable bounds
            if (region.Height > 0)
            {
                FontSize = Math.Clamp(region.Height * 0.5f, 8f, 200f);
                // Scale outline width proportionally to font size (roughly 15-20% of font size)
                OutlineWidth = Math.Clamp(FontSize * 0.15f, 2f, 10f);
            }

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

            // Calculate scaled font size based on region height change
            float scaleFactor = InitialRegionHeight > 0 ? (float)Region.Height / InitialRegionHeight : 1f;
            float scaledFontSize = Math.Max(4f, FontSize * scaleFactor); // Minimum font size of 4

            // Also scale outline width proportionally
            float scaledOutlineWidth = Math.Max(1f, OutlineWidth * scaleFactor);

            // Calculate available width (with padding for outline and margins)
            float availableWidth = Region.Width - 12f; // 6px padding on left, 6px on right (approximate for outline)
            if (availableWidth < 10f) availableWidth = 10f; // Minimum reasonable width

            // Measure text at the calculated font size and scale down if it doesn't fit
            SizeF textSize;
            using (var testFont = new Font(FontFamily, scaledFontSize, FontStyle.Bold))
            {
                textSize = g.MeasureString(Text, testFont);

                // If text is too wide, scale down the font size proportionally
                if (textSize.Width > availableWidth)
                {
                    float widthScaleFactor = availableWidth / textSize.Width;
                    scaledFontSize = Math.Max(4f, scaledFontSize * widthScaleFactor);
                    scaledOutlineWidth = Math.Max(1f, scaledOutlineWidth * widthScaleFactor);
                    // Re-measure with the adjusted font size
                    using (var adjustedFont = new Font(FontFamily, scaledFontSize, FontStyle.Bold))
                    {
                        textSize = g.MeasureString(Text, adjustedFont);
                    }
                }
            }

            // Constrain region height to not exceed actual text height + padding
            int requiredHeight = (int)Math.Ceiling(textSize.Height + scaledOutlineWidth * 2 + 6);
            if (Region.Height > requiredHeight)
            {
                Region = new Rectangle(Region.X, Region.Y, Region.Width, requiredHeight);
            }

            using (var font = new Font(FontFamily, scaledFontSize, FontStyle.Bold))
            using (var path = new GraphicsPath())
            {
                path.AddString(Text, font.FontFamily, (int)FontStyle.Bold, 
                    g.DpiY * scaledFontSize / 72, new PointF(Region.X + 6, Region.Y + 3), 
                    StringFormat.GenericDefault);

                // Draw outer border/glow
                using (var outlinePen = new Pen(OuterColor, scaledOutlineWidth))
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
                OutlineWidth = this.OutlineWidth,
                InitialRegionHeight = this.InitialRegionHeight
            };
        }
    }
}
