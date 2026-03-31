using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BetterStepsRecorder.Core.ImageOperations;

namespace BetterStepsRecorder
{
    public partial class MainForm
    {
        // ── Tool mode ────────────────────────────────────────────────────────────
        private enum ImageTool { None, Blur, Highlight, Text, Arrow, Crop }
        private ImageTool _activeTool = ImageTool.None;

        // Shared drawing state (rect-based tools)
        private bool    _toolDrawing  = false;
        private bool    _applyingOperation = false; // Guard against re-entry during operation application
        private Point   _toolStart;
        private Point   _toolCurrent;
        private Rectangle _toolRect;

        // Selection highlight state (marching ants)
        private int _selectedOperationIndex = -1;
        private Rectangle _selectedOperationBounds = Rectangle.Empty;
        private Point[] _selectedOperationPoints = null;
        private float _marchingAntsOffset = 0f;
        private System.Windows.Forms.Timer _selectionFlashTimer;

        // Arrow tool: two endpoints
        private Point _arrowStart;
        private Point _arrowEnd;

        // Highlight colour (user-configurable via toolbar button)
        public static Color HighlightColor { get; set; } = Color.FromArgb(160, 255, 255, 0);

        // Arrow colour (user-configurable via toolbar button)
        public static Color ArrowColor { get; set; } = Color.FromArgb(255, 255, 0, 255);

        // Undo stack: keyed by RecordEvent.ID, stores previous Screenshotb64 values
        private readonly Dictionary<Guid, Stack<string>> _undoStacks = new();

        // Undo list: keyed by RecordEvent.ID, stores list of edits for non-linear undo
        private readonly Dictionary<Guid, List<UndoItem>> _undoLists = new();

        private class UndoItem
        {
            public string Description { get; set; } = "";
            public string ImageState { get; set; } = "";
        }

        /// <summary>
        /// Initializes the marching ants timer for the edit selection highlight
        /// </summary>
        private void InitializeSelectionFlashTimer()
        {
            _selectionFlashTimer = new System.Windows.Forms.Timer();
            _selectionFlashTimer.Interval = 100; // Update every 100ms for smooth animation
            _selectionFlashTimer.Tick += (s, e) =>
            {
                _marchingAntsOffset += 2f;
                if (_marchingAntsOffset > 8f) _marchingAntsOffset = 0f;
                if (_selectedOperationIndex >= 0)
                {
                    pictureBox1.Invalidate();
                }
            };
        }


        /// <summary>
        /// Handles selection change in the edits listbox to show selection highlight
        /// </summary>
        private void listBox_Edits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox_Edits.SelectedIndex < 0)
            {
                ClearSelectionHighlight();
                return;
            }

            if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;

            int index = listBox_Edits.SelectedIndex;
            if (index >= selectedEvent.ImageOperations.Count)
            {
                ClearSelectionHighlight();
                return;
            }

            _selectedOperationIndex = index;
            UpdateSelectionHighlightBounds(selectedEvent, index);

            // Start the marching ants animation
            _marchingAntsOffset = 0f;
            _selectionFlashTimer?.Start();
            pictureBox1.Invalidate();
        }

        /// <summary>
        /// Updates the selection highlight bounds based on the selected operation
        /// </summary>
        private void UpdateSelectionHighlightBounds(RecordEvent selectedEvent, int operationIndex)
        {
            _selectedOperationBounds = Rectangle.Empty;
            _selectedOperationPoints = null;

            var operation = selectedEvent.ImageOperations.Operations[operationIndex];

            // Get the bounds based on operation type
            switch (operation)
            {
                case BlurOperation blur:
                    _selectedOperationBounds = GetAdjustedBounds(blur.Region, selectedEvent, operationIndex);
                    break;
                case HighlightOperation highlight:
                    _selectedOperationBounds = GetAdjustedBounds(highlight.Region, selectedEvent, operationIndex);
                    break;
                case TextLabelOperation text:
                    _selectedOperationBounds = GetAdjustedBounds(text.Region, selectedEvent, operationIndex);
                    break;
                case CropOperation crop:
                    _selectedOperationBounds = GetAdjustedBounds(crop.Region, selectedEvent, operationIndex);
                    break;
                case ArrowOperation arrow:
                    _selectedOperationPoints = GetAdjustedPoints(new[] { arrow.StartPoint, arrow.EndPoint }, selectedEvent, operationIndex);
                    break;
                case ClickIndicatorOperation click:
                    // Create a small bounds around the click point
                    var adjustedClick = GetAdjustedPoints(new[] { click.CursorPosition }, selectedEvent, operationIndex);
                    if (adjustedClick != null && adjustedClick.Length > 0)
                    {
                        int radius = 30;
                        _selectedOperationBounds = new Rectangle(
                            adjustedClick[0].X - radius, adjustedClick[0].Y - radius,
                            radius * 2, radius * 2);
                    }
                    break;
                case DragIndicatorOperation drag:
                    _selectedOperationPoints = GetAdjustedPoints(new[] { drag.StartPoint, drag.EndPoint }, selectedEvent, operationIndex);
                    break;
            }
        }

        /// <summary>
        /// Gets bounds adjusted for any prior crop operations
        /// </summary>
        private Rectangle GetAdjustedBounds(Rectangle originalBounds, RecordEvent selectedEvent, int upToIndex)
        {
            // Account for any crop operations that occurred before this operation
            Rectangle adjusted = originalBounds;
            for (int i = 0; i < upToIndex; i++)
            {
                if (selectedEvent.ImageOperations.Operations[i] is CropOperation cropOp)
                {
                    // Adjust coordinates relative to the crop origin
                    adjusted = new Rectangle(
                        adjusted.X - cropOp.Region.X,
                        adjusted.Y - cropOp.Region.Y,
                        adjusted.Width,
                        adjusted.Height);
                }
            }
            return adjusted;
        }

        /// <summary>
        /// Gets points adjusted for any prior crop operations
        /// </summary>
        private Point[] GetAdjustedPoints(Point[] originalPoints, RecordEvent selectedEvent, int upToIndex)
        {
            Point[] adjusted = (Point[])originalPoints.Clone();
            for (int i = 0; i < upToIndex; i++)
            {
                if (selectedEvent.ImageOperations.Operations[i] is CropOperation cropOp)
                {
                    for (int j = 0; j < adjusted.Length; j++)
                    {
                        adjusted[j] = new Point(
                            adjusted[j].X - cropOp.Region.X,
                            adjusted[j].Y - cropOp.Region.Y);
                    }
                }
            }
            return adjusted;
        }

        /// <summary>
        /// Clears the selection highlight
        /// </summary>
        private void ClearSelectionHighlight()
        {
            _selectedOperationIndex = -1;
            _selectedOperationBounds = Rectangle.Empty;
            _selectedOperationPoints = null;
            _marchingAntsOffset = 0f;
            _selectionFlashTimer?.Stop();
            pictureBox1.Invalidate();
        }

        /// <summary>
        /// Converts an image-space rectangle to control-space rectangle for the PictureBox
        /// </summary>
        private Rectangle ImageRectToControlRect(Rectangle imageRect)
        {
            if (pictureBox1.Image == null) return Rectangle.Empty;

            Rectangle imageDrawRect = GetImageRectInZoomMode(pictureBox1);
            if (imageDrawRect.Width == 0 || imageDrawRect.Height == 0) return Rectangle.Empty;

            double scaleX = (double)imageDrawRect.Width / pictureBox1.Image.Width;
            double scaleY = (double)imageDrawRect.Height / pictureBox1.Image.Height;

            int x = (int)Math.Round(imageRect.X * scaleX + imageDrawRect.X);
            int y = (int)Math.Round(imageRect.Y * scaleY + imageDrawRect.Y);
            int w = (int)Math.Round(imageRect.Width * scaleX);
            int h = (int)Math.Round(imageRect.Height * scaleY);

            return new Rectangle(x, y, w, h);
        }

        /// <summary>
        /// Converts an image-space point to control-space point for the PictureBox
        /// </summary>
        private Point ImagePointToControlPoint(Point imagePoint)
        {
            if (pictureBox1.Image == null) return imagePoint;

            Rectangle imageDrawRect = GetImageRectInZoomMode(pictureBox1);
            if (imageDrawRect.Width == 0 || imageDrawRect.Height == 0) return imagePoint;

            double scaleX = (double)imageDrawRect.Width / pictureBox1.Image.Width;
            double scaleY = (double)imageDrawRect.Height / pictureBox1.Image.Height;

            int x = (int)Math.Round(imagePoint.X * scaleX + imageDrawRect.X);
            int y = (int)Math.Round(imagePoint.Y * scaleY + imageDrawRect.Y);

            return new Point(x, y);
        }

        private void undoToolStripButton_Click(object sender, EventArgs e)
        {
            if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;

            // Remove the last operation
            int lastIndex = selectedEvent.ImageOperations.Count - 1;
            if (lastIndex >= 0)
            {
                selectedEvent.ImageOperations.RemoveOperationAt(lastIndex);
                RebuildImageFromOperations(selectedEvent);
                RefreshOperationsListBox();
            }

            undoToolStripButton.Enabled = selectedEvent.ImageOperations.Count > 0;
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

            // Reset ALL tool state when switching tools
            _toolDrawing = false;
            _toolRect = Rectangle.Empty;
            _toolStart = Point.Empty;
            _toolCurrent = Point.Empty;
            _arrowStart = Point.Empty;
            _arrowEnd = Point.Empty;

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
            RefreshUndoListBox();
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
            if (!_toolDrawing || _applyingOperation) return;

            // Capture current tool before any state changes
            var currentTool = _activeTool;

            _toolDrawing = false;
            _toolCurrent = e.Location;
            _arrowEnd = e.Location;
            _toolRect = RectFromPoints(_toolStart, _toolCurrent);

            // Store the rect locally and clear it immediately to prevent stale state
            var operationRect = _toolRect;
            _toolRect = Rectangle.Empty;

            bool applied = false;
            switch (currentTool)
            {
                case ImageTool.Blur:
                    if (operationRect.Width >= 4 && operationRect.Height >= 4)
                    {
                        var blurOp = new BlurOperation(ControlRectToImageRect(operationRect));
                        ApplyOperation(blurOp);
                        applied = true;
                    }
                    break;

                case ImageTool.Highlight:
                    if (operationRect.Width >= 4 && operationRect.Height >= 4)
                    {
                        var highlightOp = new HighlightOperation(ControlRectToImageRect(operationRect), HighlightColor);
                        ApplyOperation(highlightOp);
                        applied = true;
                    }
                    break;

                case ImageTool.Text:
                    if (operationRect.Width >= 4 || operationRect.Height >= 4)
                    {
                        _toolRect = operationRect; // Temporarily restore for dialog
                        ShowTextInputDialog(operationRect);
                        _toolRect = Rectangle.Empty;
                        // ShowTextInputDialog handles applying and saving
                    }
                    break;

                case ImageTool.Arrow:
                {
                    Point imgStart = ControlPointToImagePoint(_arrowStart);
                    Point imgEnd   = ControlPointToImagePoint(_arrowEnd);
                    if (Math.Abs(imgEnd.X - imgStart.X) >= 4 || Math.Abs(imgEnd.Y - imgStart.Y) >= 4)
                    {
                        var arrowOp = new ArrowOperation(imgStart, imgEnd, ArrowColor);
                        ApplyOperation(arrowOp);
                        applied = true;
                    }
                    break;
                }

                case ImageTool.Crop:
                    if (operationRect.Width >= 16 && operationRect.Height >= 16)
                    {
                        var cropOp = new CropOperation(ControlRectToImageRect(operationRect));
                        ApplyOperation(cropOp);
                        applied = true;
                    }
                    break;
            }

            // Reset all tool state
            _toolStart = Point.Empty;
            _toolCurrent = Point.Empty;
            _arrowStart = Point.Empty;
            _arrowEnd = Point.Empty;

            pictureBox1.Invalidate();

            // Stay in tool mode so the user can apply multiple times (except crop)
            if (applied && currentTool == ImageTool.Crop)
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

                /// <summary>
                /// Paint handler for drawing marching ants selection highlight on selected operation
                /// </summary>
                private void SelectionHighlight_Paint(object sender, PaintEventArgs e)
                {
                    if (_selectedOperationIndex < 0) return;

                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    // Classic marching ants: black background line + white dashed line with animated offset
                    using var blackPen = new Pen(Color.Black, 1f);
                    using var whitePen = new Pen(Color.White, 1f)
                    {
                        DashStyle = System.Drawing.Drawing2D.DashStyle.Dash,
                        DashPattern = new float[] { 4f, 4f },
                        DashOffset = _marchingAntsOffset
                    };

                    // Draw bounds for rectangle-based operations
                    if (!_selectedOperationBounds.IsEmpty)
                    {
                        Rectangle controlRect = ImageRectToControlRect(_selectedOperationBounds);
                        if (controlRect.Width > 0 && controlRect.Height > 0)
                        {
                            // Inflate slightly to make the border visible outside the operation
                            controlRect.Inflate(2, 2);
                            // Draw black line first (background), then white dashed line (marching ants)
                            e.Graphics.DrawRectangle(blackPen, controlRect);
                            e.Graphics.DrawRectangle(whitePen, controlRect);
                        }
                    }

                    // Draw for point-based operations (arrows, drags)
                    if (_selectedOperationPoints != null && _selectedOperationPoints.Length >= 2)
                    {
                        Point start = ImagePointToControlPoint(_selectedOperationPoints[0]);
                        Point end = ImagePointToControlPoint(_selectedOperationPoints[1]);

                        // Draw a bounding box around the line
                        int minX = Math.Min(start.X, end.X) - 10;
                        int minY = Math.Min(start.Y, end.Y) - 10;
                        int maxX = Math.Max(start.X, end.X) + 10;
                        int maxY = Math.Max(start.Y, end.Y) + 10;
                        var lineRect = new Rectangle(minX, minY, maxX - minX, maxY - minY);

                        e.Graphics.DrawRectangle(blackPen, lineRect);
                        e.Graphics.DrawRectangle(whitePen, lineRect);

                        // Also draw circles at the endpoints for clarity
                        int circleRadius = 8;
                        var startCircle = new Rectangle(start.X - circleRadius, start.Y - circleRadius, circleRadius * 2, circleRadius * 2);
                        var endCircle = new Rectangle(end.X - circleRadius, end.Y - circleRadius, circleRadius * 2, circleRadius * 2);

                        e.Graphics.DrawEllipse(blackPen, startCircle);
                        e.Graphics.DrawEllipse(whitePen, startCircle);
                        e.Graphics.DrawEllipse(blackPen, endCircle);
                        e.Graphics.DrawEllipse(whitePen, endCircle);
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

            var textOp = new TextLabelOperation(text, imgRect);
            ApplyOperation(textOp);
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
        private void ApplyToImage(Action<Bitmap> mutate, string actionDescription = "Edit")
        {
            if (pictureBox1.Image == null) return;
            if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;

            using var bmp = new Bitmap(pictureBox1.Image);
            mutate(bmp);
            CommitBitmap(new Bitmap(bmp), selectedEvent, actionDescription);
        }

        /// <summary>
        /// Replaces pictureBox1.Image with the new bitmap and saves b64 back to the event.
        /// </summary>
        private void CommitBitmap(Bitmap newBmp, RecordEvent evt, string actionDescription = "Edit")
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

            // Add to undo list for non-linear undo
            if (!_undoLists.TryGetValue(evt.ID, out var undoList))
            {
                undoList = new List<UndoItem>();
                _undoLists[evt.ID] = undoList;
            }
            undoList.Add(new UndoItem
            {
                Description = actionDescription,
                ImageState = previousState
            });
            RefreshUndoListBox();

            var oldImage = pictureBox1.Image;
            pictureBox1.Image = newBmp;
            oldImage?.Dispose();

            using var ms = new MemoryStream();
            newBmp.Save(ms, ImageFormat.Png);
            evt.Screenshotb64 = Convert.ToBase64String(ms.ToArray());

            activityTimer.Stop();
            activityTimer.Start();
        }

        /// <summary>
        /// Applies an operation to the current image and updates the display
        /// </summary>
        private void ApplyOperation(ImageOperation operation)
        {
            if (_applyingOperation) return; // Prevent re-entry
            if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;

            _applyingOperation = true;
            try
            {
                // Add the operation to the event's operations list
                selectedEvent.ImageOperations.AddOperation(operation);

                // Rebuild the image from base + all operations
                RebuildImageFromOperations(selectedEvent);

                // Refresh the undo list to show the new operation
                RefreshOperationsListBox();

                activityTimer.Stop();
                activityTimer.Start();
            }
            finally
            {
                _applyingOperation = false;
            }
        }

        /// <summary>
        /// Rebuilds the image from the base screenshot and all operations
        /// </summary>
        private void RebuildImageFromOperations(RecordEvent selectedEvent)
        {
            // Get the base screenshot
            byte[]? baseBytes = Program.GetBaseScreenshotBytes(selectedEvent);
            if (baseBytes == null || baseBytes.Length == 0)
            {
                // If no base screenshot, try to get the current screenshot
                baseBytes = Program.GetScreenshotBytes(selectedEvent);
                if (baseBytes == null || baseBytes.Length == 0)
                {
                    pictureBox1.Image?.Dispose();
                    pictureBox1.Image = null;
                    return;
                }
            }

            // Load the base image
            using var baseMs = new MemoryStream(baseBytes);
            using var baseImage = new Bitmap(baseMs);

            // Apply all operations to create the final image
            Bitmap finalImage = selectedEvent.ImageOperations.ApplyOperationsToImage(baseImage);

            // Update the display
            var oldImage = pictureBox1.Image;
            pictureBox1.Image = finalImage;
            oldImage?.Dispose();

            // Force the PictureBox to recalculate its layout immediately
            // This ensures GetImageRectInZoomMode returns correct values for subsequent operations
            pictureBox1.Refresh();

            // Save the result back to the event
            using var resultMs = new MemoryStream();
            finalImage.Save(resultMs, ImageFormat.Png);
            selectedEvent.Screenshotb64 = Convert.ToBase64String(resultMs.ToArray());

            // Enable undo button if there are operations
            undoToolStripButton.Enabled = selectedEvent.ImageOperations.Count > 0;
        }

        /// <summary>Maps a control-space rectangle to image pixel space.</summary>
        private Rectangle ControlRectToImageRect(Rectangle controlRect)
        {
            if (pictureBox1.Image == null) return Rectangle.Empty;

            Rectangle imageDrawRect = GetImageRectInZoomMode(pictureBox1);
            if (imageDrawRect.Width == 0 || imageDrawRect.Height == 0) return Rectangle.Empty;

            // Use double precision for better accuracy
            double scaleX = (double)pictureBox1.Image.Width / imageDrawRect.Width;
            double scaleY = (double)pictureBox1.Image.Height / imageDrawRect.Height;

            int x = (int)Math.Round((controlRect.X - imageDrawRect.X) * scaleX);
            int y = (int)Math.Round((controlRect.Y - imageDrawRect.Y) * scaleY);
            int w = (int)Math.Round(controlRect.Width * scaleX);
            int h = (int)Math.Round(controlRect.Height * scaleY);

            // Clamp to image bounds
            x = Math.Max(0, Math.Min(x, pictureBox1.Image.Width - 1));
            y = Math.Max(0, Math.Min(y, pictureBox1.Image.Height - 1));
            w = Math.Max(1, Math.Min(w, pictureBox1.Image.Width - x));
            h = Math.Max(1, Math.Min(h, pictureBox1.Image.Height - y));

            return new Rectangle(x, y, w, h);
        }

        /// <summary>Maps a single control-space point to image pixel space.</summary>
        private Point ControlPointToImagePoint(Point controlPt)
        {
            if (pictureBox1.Image == null) return controlPt;

            Rectangle imageDrawRect = GetImageRectInZoomMode(pictureBox1);
            if (imageDrawRect.Width == 0 || imageDrawRect.Height == 0) return controlPt;

            double scaleX = (double)pictureBox1.Image.Width / imageDrawRect.Width;
            double scaleY = (double)pictureBox1.Image.Height / imageDrawRect.Height;

            int x = (int)Math.Round((controlPt.X - imageDrawRect.X) * scaleX);
            int y = (int)Math.Round((controlPt.Y - imageDrawRect.Y) * scaleY);

            // Clamp to image bounds
            x = Math.Max(0, Math.Min(x, pictureBox1.Image.Width - 1));
            y = Math.Max(0, Math.Min(y, pictureBox1.Image.Height - 1));

            return new Point(x, y);
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

        // ── Undo ListBox Management ──────────────────────────────────────────

        /// <summary>
        /// Refreshes the operations listbox with the current event's operations.
        /// </summary>
        private void RefreshOperationsListBox()
        {
            listBox_Edits.Items.Clear();

            if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;
            if (selectedEvent.ImageOperations.Count == 0) return;

            foreach (var operation in selectedEvent.ImageOperations.Operations)
            {
                listBox_Edits.Items.Add(operation.Description);
            }
        }

        /// <summary>
        /// Refreshes the undo listbox with the current event's edit history.
        /// NOTE: This is the old system, kept for backward compatibility during transition
        /// </summary>
        private void RefreshUndoListBox()
        {
            // Use the new operations-based system instead
            RefreshOperationsListBox();
        }

        /// <summary>
        /// Handles double-click on operations listbox to restore to that specific state.
        /// </summary>
        private void listBox_Edits_DoubleClick(object sender, EventArgs e)
        {
            if (listBox_Edits.SelectedIndex < 0) return;
            if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;

            int selectedIndex = listBox_Edits.SelectedIndex;
            if (selectedIndex >= selectedEvent.ImageOperations.Count) return;

            // Remove all operations after the selected one
            int operationsToRemove = selectedEvent.ImageOperations.Count - selectedIndex;
            for (int i = 0; i < operationsToRemove; i++)
            {
                selectedEvent.ImageOperations.RemoveOperationAt(selectedEvent.ImageOperations.Count - 1);
            }

            // Rebuild the image with the remaining operations
            RebuildImageFromOperations(selectedEvent);
            RefreshOperationsListBox();

            // Select the item that was double-clicked (if it still exists)
            if (selectedIndex < listBox_Edits.Items.Count)
            {
                listBox_Edits.SelectedIndex = selectedIndex;
            }

            activityTimer.Stop();
            activityTimer.Start();
        }
    }
}
