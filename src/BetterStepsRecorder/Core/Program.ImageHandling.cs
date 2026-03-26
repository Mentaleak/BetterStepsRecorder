using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Debug = System.Diagnostics.Debug;
using static BetterStepsRecorder.WindowHelper;

namespace BetterStepsRecorder
{
    internal static partial class Program
    {
        /// <summary>
        /// Captures a screenshot of a specific region of the screen and returns it as a Base64 string
        /// </summary>
        /// <param name="x">X coordinate of the top-left corner</param>
        /// <param name="y">Y coordinate of the top-left corner</param>
        /// <param name="width">Width of the region to capture</param>
        /// <param name="height">Height of the region to capture</param>
        /// <param name="eventId">ID of the associated record event</param>
        /// <returns>Base64 string representation of the screenshot, or null if capture failed</returns>
        public static string? SaveScreenRegionScreenshot(int x, int y, int width, int height, Guid eventId, POINT cursorPos)
        {
            try
            {
                // Create a bitmap of the specified size
                using (Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb))
                {
                    // Create graphics object from the bitmap
                    using (Graphics gfx = Graphics.FromImage(bmp))
                    {
                        // Copy the specified screen area to the bitmap
                        gfx.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(width, height), CopyPixelOperation.SourceCopy);

                        // Draw an arrow pointing at the click position (use snapshotted coords, not live cursor)
                        DrawArrowAtCursor(gfx, width, height, x, y, cursorPos);
                    }

                    // Convert the bitmap to a memory stream
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bmp.Save(ms, ImageFormat.Png);

                        // Convert byte array to Base64 string
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to capture screenshot: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Draws an arrow pointing to the current cursor position on the given graphics object
        /// </summary>
        /// <param name="gfx">Graphics object to draw on</param>
        /// <param name="width">Width of the bitmap</param>
        /// <param name="height">Height of the bitmap</param>
        /// <param name="offsetX">X offset of the bitmap</param>
        /// <param name="offsetY">Y offset of the bitmap</param>
        private static void DrawArrowAtCursor(Graphics gfx, int width, int height, int offsetX, int offsetY, POINT cursorPos)
        {
            int cursorX = cursorPos.X - offsetX;
            int cursorY = cursorPos.Y - offsetY;

            switch (BSRSettings.Current.IndicatorStyle)
            {
                case ClickIndicatorStyle.Circle:
                    DrawCircleIndicator(gfx, cursorX, cursorY);
                    break;
                case ClickIndicatorStyle.Cursor:
                    DrawCursorIndicator(gfx, cursorX, cursorY);
                    break;
                default:
                    DrawArrowIndicator(gfx, width, height, cursorX, cursorY);
                    break;
            }
        }

        private static void DrawArrowIndicator(Graphics gfx, int width, int height, int cursorX, int cursorY)
        {
            int arrowLength = 200;
            int endX = cursorX;
            int endY = cursorY < height / 2 ? cursorY + arrowLength : cursorY - arrowLength;

            using (var arrowCap = new System.Drawing.Drawing2D.AdjustableArrowCap(5, 5))
            using (var arrowPen = new Pen(BSRSettings.Current.IndicatorColor, 5))
            {
                arrowPen.EndCap = System.Drawing.Drawing2D.LineCap.Custom;
                arrowPen.CustomEndCap = arrowCap;
                gfx.DrawLine(arrowPen, endX, endY, cursorX, cursorY);
            }
        }

        private static void DrawCircleIndicator(Graphics gfx, int cursorX, int cursorY)
        {
            int radius = 28;
            gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Semi-transparent filled inner circle
            using (var fill = new SolidBrush(Color.FromArgb(60, BSRSettings.Current.IndicatorColor)))
                gfx.FillEllipse(fill, cursorX - radius, cursorY - radius, radius * 2, radius * 2);

            // Solid border ring
            using (var border = new Pen(BSRSettings.Current.IndicatorColor, 3.5f))
                gfx.DrawEllipse(border, cursorX - radius, cursorY - radius, radius * 2, radius * 2);

            // Small solid centre dot
            int dot = 5;
            using (var dotBrush = new SolidBrush(BSRSettings.Current.IndicatorColor))
                gfx.FillEllipse(dotBrush, cursorX - dot, cursorY - dot, dot * 2, dot * 2);
        }

        /// <summary>
        /// Draws a drag arrow from <paramref name="dragStart"/> to <paramref name="dragEnd"/>
        /// on the screenshot, showing both the origin and destination of a drag action.
        /// </summary>
        internal static void DrawDragArrow(Graphics gfx, int width, int height, int offsetX, int offsetY, POINT dragStart, POINT dragEnd)
        {
            int sx = dragStart.X - offsetX;
            int sy = dragStart.Y - offsetY;
            int ex = dragEnd.X   - offsetX;
            int ey = dragEnd.Y   - offsetY;

            gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Bezier control points: bow the curve out perpendicular to the drag direction
            // so the arc clears the content it is drawn over.
            float midX = (sx + ex) / 2f;
            float midY = (sy + ey) / 2f;
            float dx   = ex - sx;
            float dy   = ey - sy;
            float len  = (float)Math.Sqrt(dx * dx + dy * dy);
            float bow  = Math.Min(Math.Max(len * 0.35f, 40f), 120f); // proportional but capped
            // Perpendicular unit vector (rotated 90° clockwise)
            float perpX = len > 0 ?  dy / len : 1f;
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
            const float aw =  9f; // arrowhead half-width

            // Tip sits exactly on the circle edge; base is ah pixels back along the tangent
            float tipX  = ex - tx * cr;
            float tipY  = ey - ty * cr;
            float baseX = tipX - tx * ah;
            float baseY = tipY - ty * ah;

            // Draw the Bezier line shortened to the arrowhead base so nothing pokes through
            using (var path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                path.AddBezier(sx, sy, c1x, c1y, c2x, c2y, baseX, baseY);
                using (var pen = new Pen(BSRSettings.Current.IndicatorColor, 4))
                    gfx.DrawPath(pen, path);
            }

            // Draw arrowhead triangle: tip on circle edge, base back along tangent
            {
                float px = -ty, py = tx;
                PointF tip   = new PointF(tipX, tipY);
                PointF base1 = new PointF(baseX + px * aw, baseY + py * aw);
                PointF base2 = new PointF(baseX - px * aw, baseY - py * aw);
                using (var brush = new SolidBrush(BSRSettings.Current.IndicatorColor))
                    gfx.FillPolygon(brush, new[] { tip, base1, base2 });
            }

            // Clamp a circle center so it stays fully inside the bitmap
            int r = 10;
            int ClampX(int x) => Math.Max(r, Math.Min(width  - r, x));
            int ClampY(int y) => Math.Max(r, Math.Min(height - r, y));

            // START: semi-transparent circle with a white "1" label
            int scx = ClampX(sx), scy = ClampY(sy);
            using (var fill = new SolidBrush(Color.FromArgb(160, BSRSettings.Current.IndicatorColor)))
                gfx.FillEllipse(fill, scx - r, scy - r, r * 2, r * 2);
            using (var border = new Pen(Color.White, 2f))
                gfx.DrawEllipse(border, scx - r, scy - r, r * 2, r * 2);
            using (var font = new Font("Arial", 8f, FontStyle.Bold))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            using (var textBrush = new SolidBrush(Color.White))
                gfx.DrawString("1", font, textBrush, new RectangleF(scx - r, scy - r, r * 2, r * 2), sf);

            // END: filled circle with a white "2" label
            int ecx = ClampX(ex), ecy = ClampY(ey);
            using (var fill = new SolidBrush(Color.FromArgb(220, BSRSettings.Current.IndicatorColor)))
                gfx.FillEllipse(fill, ecx - r, ecy - r, r * 2, r * 2);
            using (var border = new Pen(Color.White, 2f))
                gfx.DrawEllipse(border, ecx - r, ecy - r, r * 2, r * 2);
            using (var font = new Font("Arial", 8f, FontStyle.Bold))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            using (var textBrush = new SolidBrush(Color.White))
                gfx.DrawString("2", font, textBrush, new RectangleF(ecx - r, ecy - r, r * 2, r * 2), sf);
        }

        private static void DrawCursorIndicator(Graphics gfx, int cursorX, int cursorY)
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

            gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // White outline for contrast
            using (var outline = new Pen(Color.White, 3f))
            {
                outline.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;
                gfx.DrawPolygon(outline, cursorPoly);
            }

            // Filled with indicator colour
            using (var fill = new SolidBrush(BSRSettings.Current.IndicatorColor))
                gfx.FillPolygon(fill, cursorPoly);
        }

        /// <summary>
        /// Writes PNG bytes to the session spool directory and returns the file path.
        /// Returns null if the write fails (caller should fall back to keeping bytes in RAM).
        /// </summary>
        public static string? SpoolScreenshot(byte[] pngBytes, Guid eventId)
        {
            try
            {
                string filePath = Path.Combine(SessionSpoolDir, $"{eventId:N}.png");
                File.WriteAllBytes(filePath, pngBytes);
                return filePath;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Spool write failed, keeping screenshot in RAM: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Reads raw PNG bytes for a RecordEvent, whether stored in RAM (Screenshotb64)
        /// or on disk (ScreenshotSpoolPath). Returns null if neither is available.
        /// </summary>
        public static byte[]? GetScreenshotBytes(RecordEvent recordEvent)
        {
            if (!string.IsNullOrEmpty(recordEvent.Screenshotb64))
                return Convert.FromBase64String(recordEvent.Screenshotb64);

            if (!string.IsNullOrEmpty(recordEvent.ScreenshotSpoolPath) &&
                File.Exists(recordEvent.ScreenshotSpoolPath))
                return File.ReadAllBytes(recordEvent.ScreenshotSpoolPath);

            return null;
        }

        /// <summary>
        /// Converts a Base64 string to an Image
        /// </summary>
        /// <param name="base64String">Base64 string representation of the image</param>
        /// <returns>Image object</returns>
        public static Image Base64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (var ms = new MemoryStream(imageBytes))
            {
                // Return a Bitmap (stream-independent copy) so the stream can be safely disposed
                return new Bitmap(ms);
            }
        }

        /// <summary>
        /// Converts an Image to a Base64 string
        /// </summary>
        /// <param name="image">Image to convert</param>
        /// <param name="format">Image format to use</param>
        /// <returns>Base64 string representation of the image</returns>
        public static string ImageToBase64(Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                return Convert.ToBase64String(imageBytes);
            }
        }
    }
}