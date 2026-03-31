using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace BetterStepsRecorder.Core.ImageOperations
{
    /// <summary>
    /// Operation that draws a drag indicator showing the path from start to end point.
    /// Displays a curved arrow with numbered start (1) and end (2) circles.
    /// </summary>
    [Serializable]
    public class DragIndicatorOperation : ImageOperation
    {
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }
        public Color IndicatorColor { get; set; }

        public override string Description => "Drag Indicator";

        public DragIndicatorOperation() { }

        public DragIndicatorOperation(Point startPoint, Point endPoint, Color indicatorColor)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            IndicatorColor = indicatorColor;
        }

        public override void Apply(Bitmap bitmap)
        {
            using var g = Graphics.FromImage(bitmap);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.SetClip(new Rectangle(0, 0, bitmap.Width, bitmap.Height));

            int width = bitmap.Width;
            int height = bitmap.Height;
            int sx = StartPoint.X;
            int sy = StartPoint.Y;
            int ex = EndPoint.X;
            int ey = EndPoint.Y;

            // Bezier control points: bow the curve out perpendicular to the drag direction
            float midX = (sx + ex) / 2f;
            float midY = (sy + ey) / 2f;
            float dx = ex - sx;
            float dy = ey - sy;
            float len = (float)Math.Sqrt(dx * dx + dy * dy);
            float bow = Math.Min(Math.Max(len * 0.35f, 40f), 120f); // proportional but capped

            // Perpendicular unit vector (rotated 90° clockwise)
            float perpX = len > 0 ? dy / len : 1f;
            float perpY = len > 0 ? -dx / len : 0f;
            float c1x = sx + dx * 0.25f + perpX * bow;
            float c1y = sy + dy * 0.25f + perpY * bow;
            float c2x = sx + dx * 0.75f + perpX * bow;
            float c2y = sy + dy * 0.75f + perpY * bow;

            // If the control points bow outside the image, flip the curve to bow inward instead
            if (c1x < 0 || c1x > width || c1y < 0 || c1y > height ||
                c2x < 0 || c2x > width || c2y < 0 || c2y > height)
            {
                perpX = -perpX;
                perpY = -perpY;
                c1x = sx + dx * 0.25f + perpX * bow;
                c1y = sy + dy * 0.25f + perpY * bow;
                c2x = sx + dx * 0.75f + perpX * bow;
                c2y = sy + dy * 0.75f + perpY * bow;
            }

            // Compute the blended arrowhead direction first so we can shorten the line to match
            float _ltx = ex - c2x, _lty = ey - c2y;
            float _ltLen = (float)Math.Sqrt(_ltx * _ltx + _lty * _lty);
            if (_ltLen > 0) { _ltx /= _ltLen; _lty /= _ltLen; }
            float _gtx = len > 0 ? dx / len : 1f, _gty = len > 0 ? dy / len : 0f;
            if (_ltx * _gtx + _lty * _gty < 0) { _ltx = -_ltx; _lty = -_lty; }
            float tx = _ltx * 0.5f + _gtx * 0.5f, ty = _lty * 0.5f + _gty * 0.5f;
            float tLen2 = (float)Math.Sqrt(tx * tx + ty * ty);
            if (tLen2 > 0) { tx /= tLen2; ty /= tLen2; }

            const float cr = 10f; // circle radius — must match below
            const float ah = 16f; // arrowhead height
            const float aw = 9f;  // arrowhead half-width

            // Tip sits exactly on the circle edge; base is ah pixels back along the tangent
            float tipX = ex - tx * cr;
            float tipY = ey - ty * cr;
            float baseX = tipX - tx * ah;
            float baseY = tipY - ty * ah;

            // Draw the Bezier line shortened to the arrowhead base so nothing pokes through
            using (var path = new GraphicsPath())
            {
                path.AddBezier(sx, sy, c1x, c1y, c2x, c2y, baseX, baseY);
                using (var pen = new Pen(IndicatorColor, 4))
                    g.DrawPath(pen, path);
            }

            // Draw arrowhead triangle: tip on circle edge, base back along tangent
            {
                float px = -ty, py = tx;
                PointF tip = new PointF(tipX, tipY);
                PointF base1 = new PointF(baseX + px * aw, baseY + py * aw);
                PointF base2 = new PointF(baseX - px * aw, baseY - py * aw);
                using (var brush = new SolidBrush(IndicatorColor))
                    g.FillPolygon(brush, new[] { tip, base1, base2 });
            }

            // Clamp a circle center so it stays fully inside the bitmap
            int r = 10;
            int ClampX(int x) => Math.Max(r, Math.Min(width - r, x));
            int ClampY(int y) => Math.Max(r, Math.Min(height - r, y));

            // START: semi-transparent circle with a white "1" label
            int scx = ClampX(sx), scy = ClampY(sy);
            using (var fill = new SolidBrush(Color.FromArgb(160, IndicatorColor)))
                g.FillEllipse(fill, scx - r, scy - r, r * 2, r * 2);
            using (var border = new Pen(Color.White, 2f))
                g.DrawEllipse(border, scx - r, scy - r, r * 2, r * 2);
            using (var font = new Font("Arial", 8f, FontStyle.Bold))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            using (var textBrush = new SolidBrush(Color.White))
                g.DrawString("1", font, textBrush, new RectangleF(scx - r, scy - r, r * 2, r * 2), sf);

            // END: filled circle with a white "2" label
            int ecx = ClampX(ex), ecy = ClampY(ey);
            using (var fill = new SolidBrush(Color.FromArgb(220, IndicatorColor)))
                g.FillEllipse(fill, ecx - r, ecy - r, r * 2, r * 2);
            using (var border = new Pen(Color.White, 2f))
                g.DrawEllipse(border, ecx - r, ecy - r, r * 2, r * 2);
            using (var font = new Font("Arial", 8f, FontStyle.Bold))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            using (var textBrush = new SolidBrush(Color.White))
                g.DrawString("2", font, textBrush, new RectangleF(ecx - r, ecy - r, r * 2, r * 2), sf);
        }

        public override ImageOperation Clone()
        {
            return new DragIndicatorOperation(StartPoint, EndPoint, IndicatorColor) 
            { 
                Id = this.Id, 
                CreatedAt = this.CreatedAt 
            };
        }
    }
}
