using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BetterStepsRecorder
{
    public partial class MainForm
    {
        // ── Tool mode ────────────────────────────────────────────────────────────
        private enum ImageTool { None, Blur, Highlight, Text, Arrow, Crop }
        private ImageTool _activeTool = ImageTool.None;

        // Shared drawing state (rect-based tools)
        private bool    _toolDrawing  = false;
        private Point   _toolStart;
        private Point   _toolCurrent;
        private Rectangle _toolRect;

        // Arrow tool: two endpoints
        private Point _arrowStart;
        private Point _arrowEnd;

        // Highlight colour (user-configurable via toolbar button)
        public static Color HighlightColor { get; set; } = Color.FromArgb(160, 255, 255, 0);

        // Arrow colour (user-configurable via toolbar button)
        public static Color ArrowColor { get; set; } = Color.FromArgb(255, 255, 0, 255);

        // Undo stack: keyed by RecordEvent.ID, stores previous Screenshotb64 values
        private readonly Dictionary<Guid, Stack<string>> _undoStacks = new();

        private void undoToolStripButton_Click(object sender, EventArgs e)
        {
            if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;
            if (!_undoStacks.TryGetValue(selectedEvent.ID, out var stack) || stack.Count == 0)
            {
                undoToolStripButton.Enabled = false;
                return;
            }

            string previous = stack.Pop();
            selectedEvent.Screenshotb64 = previous;

            // Refresh picture box
            if (!string.IsNullOrEmpty(previous))
            {
                byte[] bytes = Convert.FromBase64String(previous);
                using var ms = new MemoryStream(bytes);
                var oldImage = pictureBox1.Image;
                pictureBox1.Image = new Bitmap(ms);
                oldImage?.Dispose();
            }
            else
            {
                pictureBox1.Image?.Dispose();
                pictureBox1.Image = null;
            }

            undoToolStripButton.Enabled = stack.Count > 0;
            activityTimer.Stop();
            activityTimer.Start();
        }

        // ── Toolbar button click handlers ─────────────────────────────────────

        private void blurRegionToolStripButton_Click(object sender, EventArgs e)
            => ActivateTool(blurRegionToolStripButton.Checked ? ImageTool.Blur : ImageTool.None);

        private void highlightToolStripButton_Click(object sender, EventArgs e)
            => ActivateTool(highlightToolStripButton.Checked ? ImageTool.Highlight : ImageTool.None);

        private void highlightColourToolStripButton_Click(object sender, EventArgs e)
        {
            using var dlg = new ColorDialog { Color = Color.FromArgb(255, HighlightColor), FullOpen = true };
            if (dlg.ShowDialog(this) == DialogResult.OK)
                HighlightColor = Color.FromArgb(160, dlg.Color.R, dlg.Color.G, dlg.Color.B);
        }

        private void arrowColourToolStripButton_Click(object sender, EventArgs e)
        {
            using var dlg = new ColorDialog { Color = ArrowColor, FullOpen = true };
            if (dlg.ShowDialog(this) == DialogResult.OK)
                ArrowColor = dlg.Color;
        }

        private void textLabelToolStripButton_Click(object sender, EventArgs e)
            => ActivateTool(textLabelToolStripButton.Checked ? ImageTool.Text : ImageTool.None);

        private void arrowToolStripButton_Click(object sender, EventArgs e)
            => ActivateTool(arrowToolStripButton.Checked ? ImageTool.Arrow : ImageTool.None);

        private void cropToolStripButton_Click(object sender, EventArgs e)
            => ActivateTool(cropToolStripButton.Checked ? ImageTool.Crop : ImageTool.None);

        // ── Tool activation ───────────────────────────────────────────────────

        private void ActivateTool(ImageTool tool)
        {
            if (tool != ImageTool.None && pictureBox1.Image == null)
            {
                UncheckAllToolButtons();
                return;
            }

            // Detach if already active
            if (_activeTool != ImageTool.None)
                DetachToolHandlers();

            _activeTool = tool;
            _toolDrawing = false;
            _toolRect = Rectangle.Empty;

            UncheckAllToolButtons();

            if (tool == ImageTool.None)
            {
                pictureBox1.Cursor = Cursors.Default;
                pictureBox1.Invalidate();
                return;
            }

            // Sync the correct button checked state
            switch (tool)
            {
                case ImageTool.Blur:      blurRegionToolStripButton.Checked      = true; break;
                case ImageTool.Highlight: highlightToolStripButton.Checked       = true; break;
                case ImageTool.Text:      textLabelToolStripButton.Checked       = true; break;
                case ImageTool.Arrow:     arrowToolStripButton.Checked           = true; break;
                case ImageTool.Crop:      cropToolStripButton.Checked            = true; break;
            }

            pictureBox1.Cursor = (tool == ImageTool.Text) ? Cursors.IBeam : Cursors.Cross;
            AttachToolHandlers();
        }

        private void AttachToolHandlers()
        {
            pictureBox1.MouseDown += Tool_MouseDown;
            pictureBox1.MouseMove += Tool_MouseMove;
            pictureBox1.MouseUp   += Tool_MouseUp;
            pictureBox1.Paint     += Tool_Paint;
        }

        private void DetachToolHandlers()
        {
            pictureBox1.MouseDown -= Tool_MouseDown;
            pictureBox1.MouseMove -= Tool_MouseMove;
            pictureBox1.MouseUp   -= Tool_MouseUp;
            pictureBox1.Paint     -= Tool_Paint;
            pictureBox1.Cursor = Cursors.Default;
            pictureBox1.Invalidate();
        }

        private void UncheckAllToolButtons()
        {
            blurRegionToolStripButton.Checked      = false;
            highlightToolStripButton.Checked       = false;
            textLabelToolStripButton.Checked       = false;
            arrowToolStripButton.Checked           = false;
            cropToolStripButton.Checked            = false;
        }

        /// <summary>Called when the selected step changes — resets any active tool and undo state.</summary>
        private void ResetImageTools()
        {
            if (_activeTool != ImageTool.None)
            {
                DetachToolHandlers();
                _activeTool = ImageTool.None;
                UncheckAllToolButtons();
            }
            undoToolStripButton.Enabled = false;
        }

        // ── Shared mouse handlers ─────────────────────────────────────────────

        private void Tool_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || pictureBox1.Image == null) return;
            _toolDrawing = true;
            _toolStart = _toolCurrent = e.Location;
            _arrowStart = e.Location;
            _toolRect = Rectangle.Empty;
        }

        private void Tool_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_toolDrawing) return;
            _toolCurrent = e.Location;
            _arrowEnd = e.Location;
            _toolRect = RectFromPoints(_toolStart, _toolCurrent);
            pictureBox1.Invalidate();
        }

        private void Tool_MouseUp(object sender, MouseEventArgs e)
        {
            if (!_toolDrawing) return;
            _toolDrawing = false;
            _toolCurrent = e.Location;
            _arrowEnd = e.Location;
            _toolRect = RectFromPoints(_toolStart, _toolCurrent);

            bool applied = false;
            switch (_activeTool)
            {
                case ImageTool.Blur:
                    if (_toolRect.Width >= 4 && _toolRect.Height >= 4)
                    {
                        ApplyToImage(bmp => ApplyBoxBlur(bmp, ControlRectToImageRect(_toolRect)));
                        applied = true;
                    }
                    break;

                case ImageTool.Highlight:
                    if (_toolRect.Width >= 4 && _toolRect.Height >= 4)
                    {
                        ApplyToImage(bmp => ApplyHighlight(bmp, ControlRectToImageRect(_toolRect)));
                        applied = true;
                    }
                    break;

                case ImageTool.Text:
                    if (_toolRect.Width >= 4 || _toolRect.Height >= 4)
                    {
                        ShowTextInputDialog(_toolRect);
                        // ShowTextInputDialog handles applying and saving
                    }
                    break;

                case ImageTool.Arrow:
                {
                    Point imgStart = ControlPointToImagePoint(_arrowStart);
                    Point imgEnd   = ControlPointToImagePoint(_arrowEnd);
                    if (Math.Abs(imgEnd.X - imgStart.X) >= 4 || Math.Abs(imgEnd.Y - imgStart.Y) >= 4)
                    {
                        ApplyToImage(bmp => DrawArrowOnBitmap(bmp, imgStart, imgEnd));
                        applied = true;
                    }
                    break;
                }

                case ImageTool.Crop:
                    if (_toolRect.Width >= 16 && _toolRect.Height >= 16)
                    {
                        ApplyCrop(ControlRectToImageRect(_toolRect));
                        applied = true;
                    }
                    break;
            }

            _toolRect = Rectangle.Empty;
            pictureBox1.Invalidate();

            // Stay in tool mode so the user can apply multiple times (except crop)
            if (applied && _activeTool == ImageTool.Crop)
                ActivateTool(ImageTool.None);
        }

        // ── Overlay paint ─────────────────────────────────────────────────────

        private void Tool_Paint(object sender, PaintEventArgs e)
        {
            if (!_toolDrawing) return;

            switch (_activeTool)
            {
                case ImageTool.Blur:
                    if (_toolRect.Width > 0 && _toolRect.Height > 0)
                    {
                        using var b = new SolidBrush(Color.FromArgb(80, 30, 30, 30));
                        e.Graphics.FillRectangle(b, _toolRect);
                        using var p = new Pen(Color.FromArgb(200, 80, 80, 80), 1.5f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
                        e.Graphics.DrawRectangle(p, _toolRect);
                    }
                    break;

                case ImageTool.Highlight:
                    if (_toolRect.Width > 0 && _toolRect.Height > 0)
                    {
                        using var b = new SolidBrush(HighlightColor);
                        e.Graphics.FillRectangle(b, _toolRect);
                        using var p = new Pen(Color.FromArgb(200, HighlightColor.R, HighlightColor.G, HighlightColor.B), 1.5f);
                        e.Graphics.DrawRectangle(p, _toolRect);
                    }
                    break;

                case ImageTool.Text:
                    if (_toolRect.Width > 0 && _toolRect.Height > 0)
                    {
                        using var p = new Pen(Color.FromArgb(200, 0, 120, 215), 1.5f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
                        e.Graphics.DrawRectangle(p, _toolRect);
                    }
                    break;

                case ImageTool.Arrow:
                {
                    using var arrowCap = new System.Drawing.Drawing2D.AdjustableArrowCap(6, 6);
                    using var pen = new Pen(ArrowColor, 3f);
                    pen.CustomEndCap = arrowCap;
                    e.Graphics.DrawLine(pen, _arrowStart, _toolCurrent);
                    break;
                }

                case ImageTool.Crop:
                    if (_toolRect.Width > 0 && _toolRect.Height > 0)
                    {
                        // Dim outside the crop rect
                        using var dim = new SolidBrush(Color.FromArgb(120, 0, 0, 0));
                        var outer = new Region(new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));
                        outer.Exclude(_toolRect);
                        e.Graphics.FillRegion(dim, outer);
                        using var p = new Pen(Color.White, 1.5f);
                        e.Graphics.DrawRectangle(p, _toolRect);
                    }
                    break;
            }
        }

        // ── Tool implementations ──────────────────────────────────────────────

        private void ShowTextInputDialog(Rectangle controlRect)
        {
            using var dlg = new Form
            {
                Text = "Add Text Label",
                Size = new Size(340, 160),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false, MinimizeBox = false
            };
            var tb = new TextBox { Dock = DockStyle.Top, Margin = new Padding(8), Font = new Font("Segoe UI", 11) };
            var ok = new Button { Text = "OK", DialogResult = DialogResult.OK, Dock = DockStyle.Bottom };
            var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Dock = DockStyle.Bottom };
            dlg.Controls.AddRange(new Control[] { ok, cancel, tb });
            dlg.AcceptButton = ok;
            dlg.CancelButton = cancel;

            if (dlg.ShowDialog(this) != DialogResult.OK || string.IsNullOrWhiteSpace(tb.Text)) return;

            string text = tb.Text;
            Rectangle imgRect = ControlRectToImageRect(controlRect);

            ApplyToImage(bmp =>
            {
                using var g = Graphics.FromImage(bmp);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                float fontSize = Math.Max(12f, imgRect.Height * 0.45f);
                using var font = new Font("Segoe UI", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);

                // Background pill
                SizeF textSize = g.MeasureString(text, font);
                var bgRect = new RectangleF(imgRect.X, imgRect.Y, textSize.Width + 12, textSize.Height + 6);
                using var bgBrush = new SolidBrush(Color.FromArgb(210, 30, 30, 30));
                using var path = RoundedRect(bgRect, 6);
                g.FillPath(bgBrush, path);

                // Text
                using var textBrush = new SolidBrush(Color.White);
                g.DrawString(text, font, textBrush, bgRect.X + 6, bgRect.Y + 3);
            });
        }

        private static void ApplyHighlight(Bitmap bmp, Rectangle rect)
        {
            using var g = Graphics.FromImage(bmp);
            using var b = new SolidBrush(MainForm_GetHighlightColor());
            g.FillRectangle(b, rect);
        }

        // Static accessor for use in static lambda context
        private static Color MainForm_GetHighlightColor() => HighlightColor;

        // Static accessor for arrow color in static lambda context
        private static Color MainForm_GetArrowColor() => ArrowColor;

        private static void DrawArrowOnBitmap(Bitmap bmp, Point start, Point end)
        {
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var arrowCap = new System.Drawing.Drawing2D.AdjustableArrowCap(7, 7);
            using var pen = new Pen(MainForm_GetArrowColor(), 4f);
            pen.CustomEndCap = arrowCap;
            g.DrawLine(pen, start, end);
        }

        private void ApplyCrop(Rectangle imgRect)
        {
            if (imgRect.Width < 16 || imgRect.Height < 16) return;
            if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;
            if (pictureBox1.Image == null) return;

            using var src = new Bitmap(pictureBox1.Image);
            using var cropped = src.Clone(imgRect, src.PixelFormat);
            CommitBitmap(new Bitmap(cropped), selectedEvent);
        }

        // ── Shared helpers ────────────────────────────────────────────────────

        /// <summary>
        /// Applies a bitmap mutation function and commits the result.
        /// </summary>
        private void ApplyToImage(Action<Bitmap> mutate)
        {
            if (pictureBox1.Image == null) return;
            if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;

            using var bmp = new Bitmap(pictureBox1.Image);
            mutate(bmp);
            CommitBitmap(new Bitmap(bmp), selectedEvent);
        }

        /// <summary>
        /// Replaces pictureBox1.Image with the new bitmap and saves b64 back to the event.
        /// </summary>
        private void CommitBitmap(Bitmap newBmp, RecordEvent evt)
        {
            // Push current state onto the undo stack before overwriting
            if (!_undoStacks.TryGetValue(evt.ID, out var stack))
            {
                stack = new Stack<string>();
                _undoStacks[evt.ID] = stack;
            }
            // Capture the current image bytes (RAM or spool) so undo can restore it correctly
            byte[]? currentBytes = Program.GetScreenshotBytes(evt);
            string previousState = currentBytes != null ? Convert.ToBase64String(currentBytes) : string.Empty;
            stack.Push(previousState);
            undoToolStripButton.Enabled = true;

            var oldImage = pictureBox1.Image;
            pictureBox1.Image = newBmp;
            oldImage?.Dispose();

            using var ms = new MemoryStream();
            newBmp.Save(ms, ImageFormat.Png);
            evt.Screenshotb64 = Convert.ToBase64String(ms.ToArray());

            activityTimer.Stop();
            activityTimer.Start();
        }

        /// <summary>Maps a control-space rectangle to image pixel space.</summary>
        private Rectangle ControlRectToImageRect(Rectangle controlRect)
        {
            Rectangle imageDrawRect = GetImageRectInZoomMode(pictureBox1);
            if (imageDrawRect.Width == 0 || imageDrawRect.Height == 0) return Rectangle.Empty;

            float scaleX = (float)pictureBox1.Image.Width  / imageDrawRect.Width;
            float scaleY = (float)pictureBox1.Image.Height / imageDrawRect.Height;

            int x = (int)((controlRect.X - imageDrawRect.X) * scaleX);
            int y = (int)((controlRect.Y - imageDrawRect.Y) * scaleY);
            int w = (int)(controlRect.Width  * scaleX);
            int h = (int)(controlRect.Height * scaleY);

            // Clamp
            x = Math.Max(0, x);
            y = Math.Max(0, y);
            w = Math.Min(w, pictureBox1.Image.Width  - x);
            h = Math.Min(h, pictureBox1.Image.Height - y);

            return new Rectangle(x, y, Math.Max(1, w), Math.Max(1, h));
        }

        /// <summary>Maps a single control-space point to image pixel space.</summary>
        private Point ControlPointToImagePoint(Point controlPt)
        {
            Rectangle imageDrawRect = GetImageRectInZoomMode(pictureBox1);
            if (imageDrawRect.Width == 0 || imageDrawRect.Height == 0) return controlPt;

            float scaleX = (float)pictureBox1.Image.Width  / imageDrawRect.Width;
            float scaleY = (float)pictureBox1.Image.Height / imageDrawRect.Height;

            return new Point(
                (int)((controlPt.X - imageDrawRect.X) * scaleX),
                (int)((controlPt.Y - imageDrawRect.Y) * scaleY));
        }

        private static void ApplyBoxBlur(Bitmap bmp, Rectangle rect)
        {
            if (rect.Width <= 0 || rect.Height <= 0) return;

            BitmapData data = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);

            int stride    = data.Stride;
            int byteCount = stride * bmp.Height;
            byte[] pixels = new byte[byteCount];
            Marshal.Copy(data.Scan0, pixels, 0, byteCount);

            int blurSize = Math.Max(10, Math.Min(rect.Width, rect.Height) / 6);
            int xMin = rect.X, yMin = rect.Y, xMax = rect.Right, yMax = rect.Bottom;

            for (int x = xMin; x < xMax; x += blurSize)
            {
                for (int y = yMin; y < yMax; y += blurSize)
                {
                    int sumB = 0, sumG = 0, sumR = 0, count = 0;
                    int bxMax = Math.Min(x + blurSize, xMax);
                    int byMax = Math.Min(y + blurSize, yMax);

                    for (int xx = x; xx < bxMax; xx++)
                        for (int yy = y; yy < byMax; yy++)
                        {
                            int idx = yy * stride + xx * 4;
                            sumB += pixels[idx]; sumG += pixels[idx + 1]; sumR += pixels[idx + 2];
                            count++;
                        }

                    if (count == 0) continue;
                    byte avgB = (byte)(sumB / count), avgG = (byte)(sumG / count), avgR = (byte)(sumR / count);

                    for (int xx = x; xx < bxMax; xx++)
                        for (int yy = y; yy < byMax; yy++)
                        {
                            int idx = yy * stride + xx * 4;
                            pixels[idx] = avgB; pixels[idx + 1] = avgG; pixels[idx + 2] = avgR;
                        }
                }
            }

            Marshal.Copy(pixels, 0, data.Scan0, byteCount);
            bmp.UnlockBits(data);
        }

        private static Rectangle GetImageRectInZoomMode(PictureBox pb)
        {
            if (pb.Image == null) return Rectangle.Empty;
            float imgAspect = (float)pb.Image.Width / pb.Image.Height;
            float ctlAspect = (float)pb.Width / pb.Height;

            int drawW, drawH;
            if (imgAspect > ctlAspect) { drawW = pb.Width;  drawH = (int)(pb.Width  / imgAspect); }
            else                        { drawH = pb.Height; drawW = (int)(pb.Height * imgAspect); }

            return new Rectangle((pb.Width - drawW) / 2, (pb.Height - drawH) / 2, drawW, drawH);
        }

        private static Rectangle RectFromPoints(Point a, Point b) =>
            new Rectangle(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));

        private static System.Drawing.Drawing2D.GraphicsPath RoundedRect(RectangleF r, float radius)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(r.X, r.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(r.Right - radius * 2, r.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(r.Right - radius * 2, r.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(r.X, r.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
