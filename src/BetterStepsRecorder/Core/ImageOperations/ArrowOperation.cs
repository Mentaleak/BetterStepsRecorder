using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BetterStepsRecorder.Core.ImageOperations
{
    /// <summary>
    /// Operation that draws an arrow from one point to another
    /// </summary>
    [Serializable]
    public class ArrowOperation : ImageOperation
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public Color Color { get; set; }
        public float Width { get; set; } = 4f;

        public override string Description => "Arrow";

        public ArrowOperation() { }

        public ArrowOperation(Point start, Point end, Color color, float width = 4f)
        {
            StartPoint = start;
            EndPoint = end;
            Color = color;
            Width = width;
        }

        public override void Apply(Bitmap bitmap)
        {
            using var g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var arrowCap = new AdjustableArrowCap(7, 7);
            using var pen = new Pen(Color, Width);
            pen.CustomEndCap = arrowCap;
            g.DrawLine(pen, StartPoint, EndPoint);
        }

        public override ImageOperation Clone()
        {
            return new ArrowOperation(StartPoint, EndPoint, Color, Width) { Id = this.Id, CreatedAt = this.CreatedAt };
        }
    }
}
