using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BetterStepsRecorder.Core.ImageOperations
{
    /// <summary>
    /// Operation that draws a click indicator at the specified position.
    /// Supports multiple indicator styles: Circle, Arrow, or Cursor.
    /// </summary>
    [Serializable]
    public class ClickIndicatorOperation : ImageOperation
    {
        public Point CursorPosition { get; set; }
        public Color IndicatorColor { get; set; }
        public ClickIndicatorStyle Style { get; set; }

        public override string Description => $"Click Indicator ({Style})";

        public ClickIndicatorOperation() { }

        public ClickIndicatorOperation(Point cursorPosition, Color indicatorColor, ClickIndicatorStyle style)
        {
            CursorPosition = cursorPosition;
            IndicatorColor = indicatorColor;
            Style = style;
        }

        public override void Apply(Bitmap bitmap)
        {
            using var g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.SetClip(new Rectangle(0, 0, bitmap.Width, bitmap.Height));

            int cursorX = CursorPosition.X;
            int cursorY = CursorPosition.Y;

            switch (Style)
            {
                case ClickIndicatorStyle.Circle:
                    DrawCircleIndicator(g, cursorX, cursorY);
                    break;
                case ClickIndicatorStyle.Cursor:
                    DrawCursorIndicator(g, cursorX, cursorY);
                    break;
                default:
                    DrawArrowIndicator(g, bitmap.Width, bitmap.Height, cursorX, cursorY);
                    break;
            }
        }

        private void DrawCircleIndicator(Graphics gfx, int cursorX, int cursorY)
        {
            int radius = 28;

            // Semi-transparent filled inner circle
            using (var fill = new SolidBrush(Color.FromArgb(60, IndicatorColor)))
                gfx.FillEllipse(fill, cursorX - radius, cursorY - radius, radius * 2, radius * 2);

            // Solid border ring
            using (var border = new Pen(IndicatorColor, 3.5f))
                gfx.DrawEllipse(border, cursorX - radius, cursorY - radius, radius * 2, radius * 2);

            // Small solid centre dot
            int dot = 5;
            using (var dotBrush = new SolidBrush(IndicatorColor))
                gfx.FillEllipse(dotBrush, cursorX - dot, cursorY - dot, dot * 2, dot * 2);
        }

        private void DrawArrowIndicator(Graphics gfx, int width, int height, int cursorX, int cursorY)
        {
            int arrowLength = 200;
            int endX = cursorX;
            int endY = cursorY < height / 2 ? cursorY + arrowLength : cursorY - arrowLength;

            using (var arrowCap = new AdjustableArrowCap(5, 5))
            using (var arrowPen = new Pen(IndicatorColor, 5))
            {
                arrowPen.EndCap = LineCap.Custom;
                arrowPen.CustomEndCap = arrowCap;
                gfx.DrawLine(arrowPen, endX, endY, cursorX, cursorY);
            }
        }

        private void DrawCursorIndicator(Graphics gfx, int cursorX, int cursorY)
        {
            // Classic arrow cursor polygon (pointing up-left)
            int s = 28; // scale
            PointF[] cursorPoly = new PointF[]
            {
                new PointF(cursorX,          cursorY),
                new PointF(cursorX,          cursorY + s * 0.85f),
                new PointF(cursorX + s * 0.25f, cursorY + s * 0.62f),
                new PointF(cursorX + s * 0.42f, cursorY + s * 0.98f),
                new PointF(cursorX + s * 0.54f, cursorY + s * 0.93f),
                new PointF(cursorX + s * 0.37f, cursorY + s * 0.57f),
                new PointF(cursorX + s * 0.65f, cursorY + s * 0.57f),
            };

            // White outline for contrast
            using (var outline = new Pen(Color.White, 3f))
            {
                outline.LineJoin = LineJoin.Round;
                gfx.DrawPolygon(outline, cursorPoly);
            }

            // Filled with indicator colour
            using (var fill = new SolidBrush(IndicatorColor))
                gfx.FillPolygon(fill, cursorPoly);
        }

        public override ImageOperation Clone()
        {
            return new ClickIndicatorOperation(CursorPosition, IndicatorColor, Style) 
            { 
                Id = this.Id, 
                CreatedAt = this.CreatedAt 
            };
        }
    }
}
