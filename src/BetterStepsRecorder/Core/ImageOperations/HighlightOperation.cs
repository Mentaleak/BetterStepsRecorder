using System;
using System.Drawing;

namespace BetterStepsRecorder.Core.ImageOperations
{
    /// <summary>
    /// Operation that draws a semi-transparent colored rectangle
    /// </summary>
    [Serializable]
    public class HighlightOperation : ImageOperation
    {
        public Rectangle Region { get; set; }
        public Color Color { get; set; }

        public override string Description => "Highlight";

        public HighlightOperation() { }

        public HighlightOperation(Rectangle region, Color color)
        {
            Region = region;
            Color = color;
        }

        public override void Apply(Bitmap bitmap)
        {
            if (Region.Width <= 0 || Region.Height <= 0) return;

            using var g = Graphics.FromImage(bitmap);
            using var brush = new SolidBrush(Color);
            g.FillRectangle(brush, Region);
        }

        public override ImageOperation Clone()
        {
            return new HighlightOperation(Region, Color) { Id = this.Id, CreatedAt = this.CreatedAt };
        }
    }
}
