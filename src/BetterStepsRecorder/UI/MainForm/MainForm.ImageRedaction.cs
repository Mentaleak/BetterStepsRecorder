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
        private bool    _completingTextInput = false; // Guard against re-entry during text input completion
        private Point   _toolStart;
        private Point   _toolCurrent;
        private Rectangle _toolRect;

        // Selection highlight state (marching ants)
        private int _selectedOperationIndex = -1;
        private Rectangle _selectedOperationBounds = Rectangle.Empty;
        private Point[] _selectedOperationPoints = null;
        private float _marchingAntsOffset = 0f;
        private System.Windows.Forms.Timer _selectionFlashTimer;

        // Operation dragging state
        private bool _isDraggingOperation = false;
        private int _dragOperationIndex = -1;
        private Point _dragStartImagePoint;
        private Point _dragCurrentImagePoint;

        // Operation resizing state
        private bool _isResizingOperation = false;
        private int _resizeOperationIndex = -1;
        private ResizeHandle _activeResizeHandle = ResizeHandle.None;
        private Rectangle _resizeOriginalBounds = Rectangle.Empty;
        private Point _resizeStartPoint = Point.Empty;

        private enum ResizeHandle
        {
            None,
            TopLeft,
            TopCenter,
            TopRight,
            MiddleRight,
            BottomRight,
            BottomCenter,
            BottomLeft,
            MiddleLeft
        }

        // Arrow tool: two endpoints
        private Point _arrowStart;
        private Point _arrowEnd;

        // Text input state (on-canvas editing)
        private TextBox _canvasTextBox = null;
        private Rectangle _textInputControlRect = Rectangle.Empty;
        private Rectangle _textInputImageRect = Rectangle.Empty;
        private int _editingTextOperationIndex = -1; // -1 for new, >= 0 for editing existing

        // Highlight colour (user-configurable via toolbar button)
        public static Color HighlightColor { get; set; } = Color.FromArgb(160, 255, 255, 0);

        // Arrow colour (user-configurable via toolbar button)
        public static Color ArrowColor { get; set; } = Color.FromArgb(255, 255, 0, 255);

        // Text colours (user-configurable via toolbar buttons)
        public static Color TextInnerColor { get; set; } = Color.White;
        public static Color TextOuterColor { get; set; } = Color.Black;

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
            // Deselect any active markup tool when selecting an edit
            if (_activeTool != ImageTool.None)
            {
                DetachToolHandlers();
                _activeTool = ImageTool.None;
                UncheckAllToolButtons();
            }

            if (listBox_Edits.SelectedIndex < 0)
            {
                ClearSelectionHighlight();
                return;
            }

            if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;

            int visualIndex = listBox_Edits.SelectedIndex;
            int operationIndex = VisualIndexToOperationIndex(visualIndex, selectedEvent.ImageOperations.Count);
            if (operationIndex < 0 || operationIndex >= selectedEvent.ImageOperations.Count)
            {
                ClearSelectionHighlight();
                return;
            }

            _selectedOperationIndex = operationIndex;
            UpdateSelectionHighlightBounds(selectedEvent, operationIndex);

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
        /// Finds the operation at the given control-space point (click location on pictureBox)
        /// Returns the index of the topmost (last applied) operation at that point, or -1 if none found.
        /// </summary>
        private int FindOperationAtPoint(Point controlPoint, RecordEvent selectedEvent)
        {
            if (selectedEvent == null || pictureBox1.Image == null) return -1;

            // Convert control point to image point
            Point imagePoint = ControlPointToImagePoint(controlPoint);

            // Search from last to first (topmost operation takes priority)
            for (int i = selectedEvent.ImageOperations.Count - 1; i >= 0; i--)
            {
                var operation = selectedEvent.ImageOperations.Operations[i];
                Rectangle bounds = GetOperationBoundsInImageSpace(operation, selectedEvent, i);

                if (!bounds.IsEmpty && bounds.Contains(imagePoint))
                {
                    return i;
                }

                // For line-based operations, check proximity to the line
                Point[]? points = GetOperationPointsInImageSpace(operation, selectedEvent, i);
                if (points != null && points.Length >= 2)
                {
                    if (IsPointNearLine(imagePoint, points[0], points[1], tolerance: 15))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the bounds of an operation in image space, adjusted for prior crops
        /// </summary>
        private Rectangle GetOperationBoundsInImageSpace(ImageOperation operation, RecordEvent selectedEvent, int operationIndex)
        {
            Rectangle bounds = Rectangle.Empty;

            switch (operation)
            {
                case BlurOperation blur:
                    bounds = blur.Region;
                    break;
                case HighlightOperation highlight:
                    bounds = highlight.Region;
                    break;
                case TextLabelOperation text:
                    bounds = text.Region;
                    break;
                case CropOperation crop:
                    bounds = crop.Region;
                    break;
                case ClickIndicatorOperation click:
                    // Create a clickable area around the indicator
                    int radius = 30;
                    bounds = new Rectangle(click.CursorPosition.X - radius, click.CursorPosition.Y - radius, radius * 2, radius * 2);
                    break;
            }

            if (!bounds.IsEmpty)
            {
                return GetAdjustedBounds(bounds, selectedEvent, operationIndex);
            }

            return Rectangle.Empty;
        }

        /// <summary>
        /// Gets the points of a line-based operation in image space, adjusted for prior crops
        /// </summary>
        private Point[]? GetOperationPointsInImageSpace(ImageOperation operation, RecordEvent selectedEvent, int operationIndex)
        {
            Point[]? points = null;

            switch (operation)
            {
                case ArrowOperation arrow:
                    points = new[] { arrow.StartPoint, arrow.EndPoint };
                    break;
                case DragIndicatorOperation drag:
                    points = new[] { drag.StartPoint, drag.EndPoint };
                    break;
            }

            if (points != null)
            {
                return GetAdjustedPoints(points, selectedEvent, operationIndex);
            }

            return null;
        }

        /// <summary>
        /// Checks if a point is near a line segment
        /// </summary>
        private static bool IsPointNearLine(Point point, Point lineStart, Point lineEnd, int tolerance)
        {
            // Calculate distance from point to line segment
            double dx = lineEnd.X - lineStart.X;
            double dy = lineEnd.Y - lineStart.Y;
            double lengthSquared = dx * dx + dy * dy;

            if (lengthSquared == 0)
            {
                // Line is a point
                double dist = Math.Sqrt(Math.Pow(point.X - lineStart.X, 2) + Math.Pow(point.Y - lineStart.Y, 2));
                return dist <= tolerance;
            }

            // Project point onto line, clamped to segment
            double t = Math.Max(0, Math.Min(1, ((point.X - lineStart.X) * dx + (point.Y - lineStart.Y) * dy) / lengthSquared));
            double projX = lineStart.X + t * dx;
            double projY = lineStart.Y + t * dy;

            double distance = Math.Sqrt(Math.Pow(point.X - projX, 2) + Math.Pow(point.Y - projY, 2));
            return distance <= tolerance;
        }

        /// <summary>
        /// Detects which resize handle (if any) is at the given control-space point for the selected operation
        /// </summary>
        private ResizeHandle GetResizeHandleAtPoint(Point controlPoint, Rectangle controlBounds)
        {
            const int handleSize = 8;
            const int handleTolerance = 4;
            int totalSize = handleSize + handleTolerance;

            // Get handle positions
            var handles = GetResizeHandleRectangles(controlBounds, handleSize);

            // Check each handle
            foreach (var kvp in handles)
            {
                Rectangle expandedHandle = kvp.Value;
                expandedHandle.Inflate(handleTolerance, handleTolerance);
                if (expandedHandle.Contains(controlPoint))
                {
                    return kvp.Key;
                }
            }

            return ResizeHandle.None;
        }

        /// <summary>
        /// Gets the rectangles for all resize handles around a control-space bounding rectangle
        /// </summary>
        private Dictionary<ResizeHandle, Rectangle> GetResizeHandleRectangles(Rectangle controlBounds, int handleSize)
        {
            int half = handleSize / 2;
            var handles = new Dictionary<ResizeHandle, Rectangle>();

            // Corner handles (always included)
            handles[ResizeHandle.TopLeft] = new Rectangle(controlBounds.Left - half, controlBounds.Top - half, handleSize, handleSize);
            handles[ResizeHandle.TopRight] = new Rectangle(controlBounds.Right - half, controlBounds.Top - half, handleSize, handleSize);
            handles[ResizeHandle.BottomLeft] = new Rectangle(controlBounds.Left - half, controlBounds.Bottom - half, handleSize, handleSize);
            handles[ResizeHandle.BottomRight] = new Rectangle(controlBounds.Right - half, controlBounds.Bottom - half, handleSize, handleSize);

            // Edge handles (only for non-text operations)
            // Check if the currently selected operation is a text operation
            bool isTextOperation = false;
            if (Listbox_Events.SelectedItem is RecordEvent selectedEvent)
            {
                if (_selectedOperationIndex >= 0 && _selectedOperationIndex < selectedEvent.ImageOperations.Count)
                {
                    isTextOperation = selectedEvent.ImageOperations.Operations[_selectedOperationIndex] is TextLabelOperation;
                }
            }

            // Only add edge handles for non-text operations
            if (!isTextOperation)
            {
                handles[ResizeHandle.TopCenter] = new Rectangle(controlBounds.Left + controlBounds.Width / 2 - half, controlBounds.Top - half, handleSize, handleSize);
                handles[ResizeHandle.BottomCenter] = new Rectangle(controlBounds.Left + controlBounds.Width / 2 - half, controlBounds.Bottom - half, handleSize, handleSize);
                handles[ResizeHandle.MiddleLeft] = new Rectangle(controlBounds.Left - half, controlBounds.Top + controlBounds.Height / 2 - half, handleSize, handleSize);
                handles[ResizeHandle.MiddleRight] = new Rectangle(controlBounds.Right - half, controlBounds.Top + controlBounds.Height / 2 - half, handleSize, handleSize);
            }

            return handles;
        }

        /// <summary>
        /// Gets the appropriate cursor for a resize handle
        /// </summary>
        private Cursor GetCursorForResizeHandle(ResizeHandle handle)
        {
            return handle switch
            {
                ResizeHandle.TopLeft => Cursors.SizeNWSE,
                ResizeHandle.TopRight => Cursors.SizeNESW,
                ResizeHandle.BottomLeft => Cursors.SizeNESW,
                ResizeHandle.BottomRight => Cursors.SizeNWSE,
                ResizeHandle.TopCenter => Cursors.SizeNS,
                ResizeHandle.BottomCenter => Cursors.SizeNS,
                ResizeHandle.MiddleLeft => Cursors.SizeWE,
                ResizeHandle.MiddleRight => Cursors.SizeWE,
                _ => Cursors.Default
            };
        }

        /// <summary>
        /// Checks if an operation supports resizing (rectangle-based operations)
        /// </summary>
        private bool OperationSupportsResize(ImageOperation operation)
        {
            return operation is BlurOperation
                || operation is HighlightOperation
                || operation is TextLabelOperation
                || operation is CropOperation;
        }

        /// <summary>
        /// Handles mouse down on pictureBox to select or start dragging operations when no tool is active
        /// </summary>
        private void PictureBox_SelectionMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            if (_activeTool != ImageTool.None) return; // Don't interfere with tools
            if (pictureBox1.Image == null) return;
            if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;

            // First, check if clicking on a resize handle of the currently selected operation
            if (_selectedOperationIndex >= 0 && !_selectedOperationBounds.IsEmpty)
            {
                Rectangle controlBounds = ImageRectToControlRect(_selectedOperationBounds);
                controlBounds.Inflate(2, 2); // Match the selection highlight inflation
                ResizeHandle handle = GetResizeHandleAtPoint(e.Location, controlBounds);

                if (handle != ResizeHandle.None)
                {
                    var operation = selectedEvent.ImageOperations.Operations[_selectedOperationIndex];
                    if (OperationSupportsResize(operation))
                    {
                        // Start resizing
                        _isResizingOperation = true;
                        _resizeOperationIndex = _selectedOperationIndex;
                        _activeResizeHandle = handle;
                        _resizeOriginalBounds = _selectedOperationBounds;
                        _resizeStartPoint = ControlPointToImagePoint(e.Location);
                        pictureBox1.Cursor = GetCursorForResizeHandle(handle);
                        return;
                    }
                }
            }

            int operationIndex = FindOperationAtPoint(e.Location, selectedEvent);

            if (operationIndex >= 0 && operationIndex < selectedEvent.ImageOperations.Count)
            {
                // Convert operation index to visual index for listbox selection
                int visualIndex = OperationIndexToVisualIndex(operationIndex, selectedEvent.ImageOperations.Count);
                if (visualIndex >= 0 && visualIndex < listBox_Edits.Items.Count)
                {
                    listBox_Edits.SelectedIndex = visualIndex;
                }

                // Start dragging
                _isDraggingOperation = true;
                _dragOperationIndex = operationIndex;
                _dragStartImagePoint = ControlPointToImagePoint(e.Location);
                _dragCurrentImagePoint = _dragStartImagePoint;
                pictureBox1.Cursor = Cursors.SizeAll;
            }
            else
            {
                // Clicked on empty space - deselect
                listBox_Edits.ClearSelected();
                ClearSelectionHighlight();
            }
        }

        /// <summary>
        /// Handles mouse move on pictureBox for dragging operations and cursor changes
        /// </summary>
        private void PictureBox_SelectionMouseMove(object sender, MouseEventArgs e)
        {
            if (_activeTool != ImageTool.None) return;
            if (pictureBox1.Image == null) return;
            if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;

            if (_isResizingOperation && _resizeOperationIndex >= 0)
            {
                // Update resize
                Point currentImagePoint = ControlPointToImagePoint(e.Location);
                UpdateOperationSizeDuringResize(selectedEvent, _resizeOperationIndex, _activeResizeHandle, 
                    _resizeOriginalBounds, _resizeStartPoint, currentImagePoint);

                // Rebuild image to show the resized operation
                RebuildImageFromOperations(selectedEvent);

                // Update selection highlight bounds
                UpdateSelectionHighlightBounds(selectedEvent, _resizeOperationIndex);
                pictureBox1.Invalidate();
            }
            else if (_isDraggingOperation && _dragOperationIndex >= 0)
            {
                // Update drag position
                _dragCurrentImagePoint = ControlPointToImagePoint(e.Location);

                // Update the operation position in real-time for visual feedback
                UpdateOperationPositionDuringDrag(selectedEvent, _dragOperationIndex, _dragStartImagePoint, _dragCurrentImagePoint);

                // Rebuild image to show the dragged position
                RebuildImageFromOperations(selectedEvent);

                // Update selection highlight bounds
                UpdateSelectionHighlightBounds(selectedEvent, _dragOperationIndex);
                pictureBox1.Invalidate();
            }
            else
            {
                // Not dragging or resizing - update cursor based on what's under the mouse
                // First check for resize handles on selected operation
                if (_selectedOperationIndex >= 0 && !_selectedOperationBounds.IsEmpty)
                {
                    var operation = selectedEvent.ImageOperations.Operations[_selectedOperationIndex];
                    if (OperationSupportsResize(operation))
                    {
                        Rectangle controlBounds = ImageRectToControlRect(_selectedOperationBounds);
                        controlBounds.Inflate(2, 2);
                        ResizeHandle handle = GetResizeHandleAtPoint(e.Location, controlBounds);

                        if (handle != ResizeHandle.None)
                        {
                            pictureBox1.Cursor = GetCursorForResizeHandle(handle);
                            return;
                        }
                    }
                }

                // Check if over an operation
                int operationIndex = FindOperationAtPoint(e.Location, selectedEvent);
                pictureBox1.Cursor = operationIndex >= 0 ? Cursors.SizeAll : Cursors.Default;
            }
        }

        /// <summary>
        /// Handles mouse up on pictureBox to finish dragging or resizing operations
        /// </summary>
        private void PictureBox_SelectionMouseUp(object sender, MouseEventArgs e)
        {
            if (!_isDraggingOperation && !_isResizingOperation) return;
            if (_activeTool != ImageTool.None) return;
            if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;

            if (_isResizingOperation)
            {
                // Finalize resize
                _isResizingOperation = false;
                _resizeOperationIndex = -1;
                _activeResizeHandle = ResizeHandle.None;
                _resizeOriginalBounds = Rectangle.Empty;
                _resizeStartPoint = Point.Empty;
            }
            else if (_isDraggingOperation)
            {
                _dragCurrentImagePoint = ControlPointToImagePoint(e.Location);

                // Finalize the position (already updated during drag)
                // The operation position was already modified in MouseMove

                // Reset drag state
                _isDraggingOperation = false;
                _dragOperationIndex = -1;
                _dragStartImagePoint = Point.Empty;
                _dragCurrentImagePoint = Point.Empty;
            }

            // Update cursor based on what's under mouse
            if (_selectedOperationIndex >= 0 && !_selectedOperationBounds.IsEmpty)
            {
                var operation = selectedEvent.ImageOperations.Operations[_selectedOperationIndex];
                if (OperationSupportsResize(operation))
                {
                    Rectangle controlBounds = ImageRectToControlRect(_selectedOperationBounds);
                    controlBounds.Inflate(2, 2);
                    ResizeHandle handle = GetResizeHandleAtPoint(e.Location, controlBounds);
                    if (handle != ResizeHandle.None)
                    {
                        pictureBox1.Cursor = GetCursorForResizeHandle(handle);
                    }
                    else
                    {
                        int operationIndex = FindOperationAtPoint(e.Location, selectedEvent);
                        pictureBox1.Cursor = operationIndex >= 0 ? Cursors.SizeAll : Cursors.Default;
                    }
                }
                else
                {
                    int operationIndex = FindOperationAtPoint(e.Location, selectedEvent);
                    pictureBox1.Cursor = operationIndex >= 0 ? Cursors.SizeAll : Cursors.Default;
                }
            }
            else
            {
                int operationIndex = FindOperationAtPoint(e.Location, selectedEvent);
                pictureBox1.Cursor = operationIndex >= 0 ? Cursors.SizeAll : Cursors.Default;
            }

            // Refresh UI
            RefreshOperationsListBox();
            if (listBox_Edits.SelectedIndex >= 0)
            {
                int opIndex = VisualIndexToOperationIndex(listBox_Edits.SelectedIndex, selectedEvent.ImageOperations.Count);
                if (opIndex >= 0)
                {
                    UpdateSelectionHighlightBounds(selectedEvent, opIndex);
                }
            }
            pictureBox1.Invalidate();

            activityTimer.Stop();
            activityTimer.Start();
        }

                    /// <summary>
                    /// Handles mouse leave to reset cursor
                    /// </summary>
                    private void PictureBox_SelectionMouseLeave(object sender, EventArgs e)
                    {
                        if (_activeTool == ImageTool.None && !_isDraggingOperation && !_isResizingOperation)
                        {
                            pictureBox1.Cursor = Cursors.Default;
                        }
                    }

                    /// <summary>
                    /// Handles right-click on pictureBox to show context menu for selected operation
                    /// </summary>
                    private void PictureBox_MouseUp_ContextMenu(object sender, MouseEventArgs e)
                    {
                        if (e.Button != MouseButtons.Right) return;
                        if (_activeTool != ImageTool.None) return;
                        if (pictureBox1.Image == null) return;
                        if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;

                                    // Find operation at click point
                                    int operationIndex = FindOperationAtPoint(e.Location, selectedEvent);

                                    if (operationIndex >= 0 && operationIndex < selectedEvent.ImageOperations.Count)
                                    {
                                        // Convert operation index to visual index for listbox selection
                                        int visualIndex = OperationIndexToVisualIndex(operationIndex, selectedEvent.ImageOperations.Count);
                                        if (visualIndex >= 0 && visualIndex < listBox_Edits.Items.Count)
                                        {
                                            listBox_Edits.SelectedIndex = visualIndex;
                                        }

                                        // Show context menu
                                        contextMenu_PictureBox.Show(pictureBox1, e.Location);
                                    }
                                }

                                /// <summary>
                                /// Handles the Opening event of the PictureBox context menu to show/hide "Edit Text" item
                                /// </summary>
                                private void contextMenu_PictureBox_Opening(object sender, System.ComponentModel.CancelEventArgs e)
                                {
                                    // Hide "Edit Text" by default
                                    editTextPictureBoxMenuItem.Visible = false;

                                    if (_selectedOperationIndex < 0) return;
                                    if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;

                                    if (_selectedOperationIndex >= 0 && _selectedOperationIndex < selectedEvent.ImageOperations.Count)
                                    {
                                        var operation = selectedEvent.ImageOperations.Operations[_selectedOperationIndex];
                                        // Show "Edit Text" only for TextLabelOperation
                                        editTextPictureBoxMenuItem.Visible = operation is TextLabelOperation;
                                    }
                                }

                                /// <summary>
                                /// Handles the "Edit Text" context menu item click for text label edits on the PictureBox
                                /// </summary>
                                private void editTextPictureBoxMenuItem_Click(object sender, EventArgs e)
                                {
                                    if (_selectedOperationIndex < 0) return;
                                    if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;

                                    if (_selectedOperationIndex >= 0 && _selectedOperationIndex < selectedEvent.ImageOperations.Count)
                                    {
                                        var operation = selectedEvent.ImageOperations.Operations[_selectedOperationIndex];
                                        if (operation is TextLabelOperation)
                                        {
                                            // Use the existing on-canvas text editing functionality
                                            EditExistingTextOperation(_selectedOperationIndex);
                                        }
                                    }
                                }

                                /// <summary>
                                /// Handles key down on pictureBox to delete selected operation
                                /// </summary>
                                private void PictureBox_KeyDown(object sender, KeyEventArgs e)
                                {
                                    if (e.KeyCode == Keys.Delete && _selectedOperationIndex >= 0)
                                    {
                                        DeleteSelectedEdit();
                                        e.Handled = true;
                                    }
                                }

                                /// <summary>
                                /// Handles double-click on pictureBox to edit text operations
                                /// </summary>
                                private void PictureBox_SelectionDoubleClick(object sender, EventArgs e)
                                {
                                    if (_activeTool != ImageTool.None) return;
                                    if (pictureBox1.Image == null) return;
                                    if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;

                                    // Get mouse position from the last known location
                                    Point mousePos = pictureBox1.PointToClient(Cursor.Position);
                                    int operationIndex = FindOperationAtPoint(mousePos, selectedEvent);

                                    if (operationIndex >= 0 && operationIndex < selectedEvent.ImageOperations.Count)
                                    {
                                        var operation = selectedEvent.ImageOperations.Operations[operationIndex];

                                        // If it's a text operation, enter edit mode
                                        if (operation is TextLabelOperation)
                                        {
                                            EditExistingTextOperation(operationIndex);
                                        }
                                    }
                                }

                                /// <summary>
                                /// Updates an operation's position during drag
                                /// </summary>
                                private void UpdateOperationPositionDuringDrag(RecordEvent selectedEvent, int operationIndex, Point startImagePoint, Point currentImagePoint)
        {
            if (operationIndex < 0 || operationIndex >= selectedEvent.ImageOperations.Count) return;

            int deltaX = currentImagePoint.X - startImagePoint.X;
            int deltaY = currentImagePoint.Y - startImagePoint.Y;

            if (deltaX == 0 && deltaY == 0) return;

            var operation = selectedEvent.ImageOperations.Operations[operationIndex];

            // Update position based on operation type
            switch (operation)
            {
                case BlurOperation blur:
                    blur.Region = new Rectangle(blur.Region.X + deltaX, blur.Region.Y + deltaY, blur.Region.Width, blur.Region.Height);
                    break;
                case HighlightOperation highlight:
                    highlight.Region = new Rectangle(highlight.Region.X + deltaX, highlight.Region.Y + deltaY, highlight.Region.Width, highlight.Region.Height);
                    break;
                case TextLabelOperation text:
                    text.Region = new Rectangle(text.Region.X + deltaX, text.Region.Y + deltaY, text.Region.Width, text.Region.Height);
                    break;
                case CropOperation crop:
                    crop.Region = new Rectangle(crop.Region.X + deltaX, crop.Region.Y + deltaY, crop.Region.Width, crop.Region.Height);
                    break;
                case ArrowOperation arrow:
                    arrow.StartPoint = new Point(arrow.StartPoint.X + deltaX, arrow.StartPoint.Y + deltaY);
                    arrow.EndPoint = new Point(arrow.EndPoint.X + deltaX, arrow.EndPoint.Y + deltaY);
                    break;
                case ClickIndicatorOperation click:
                    click.CursorPosition = new Point(click.CursorPosition.X + deltaX, click.CursorPosition.Y + deltaY);
                    break;
                case DragIndicatorOperation drag:
                    drag.StartPoint = new Point(drag.StartPoint.X + deltaX, drag.StartPoint.Y + deltaY);
                    drag.EndPoint = new Point(drag.EndPoint.X + deltaX, drag.EndPoint.Y + deltaY);
                    break;
            }

            // Update the start point for the next delta calculation
            _dragStartImagePoint = currentImagePoint;
        }

        /// <summary>
        /// Updates an operation's size during resize
        /// </summary>
        private void UpdateOperationSizeDuringResize(RecordEvent selectedEvent, int operationIndex, ResizeHandle handle,
            Rectangle originalBounds, Point startPoint, Point currentPoint)
        {
            if (operationIndex < 0 || operationIndex >= selectedEvent.ImageOperations.Count) return;

            int deltaX = currentPoint.X - startPoint.X;
            int deltaY = currentPoint.Y - startPoint.Y;

            var operation = selectedEvent.ImageOperations.Operations[operationIndex];

            // Calculate new bounds based on which handle is being dragged
            Rectangle newBounds = originalBounds;
            const int minSize = 10; // Minimum size for operations

            switch (handle)
            {
                case ResizeHandle.TopLeft:
                    newBounds.X = originalBounds.X + deltaX;
                    newBounds.Y = originalBounds.Y + deltaY;
                    newBounds.Width = originalBounds.Width - deltaX;
                    newBounds.Height = originalBounds.Height - deltaY;
                    break;

                case ResizeHandle.TopCenter:
                    newBounds.Y = originalBounds.Y + deltaY;
                    newBounds.Height = originalBounds.Height - deltaY;
                    break;

                case ResizeHandle.TopRight:
                    newBounds.Y = originalBounds.Y + deltaY;
                    newBounds.Width = originalBounds.Width + deltaX;
                    newBounds.Height = originalBounds.Height - deltaY;
                    break;

                case ResizeHandle.MiddleRight:
                    newBounds.Width = originalBounds.Width + deltaX;
                    break;

                case ResizeHandle.BottomRight:
                    newBounds.Width = originalBounds.Width + deltaX;
                    newBounds.Height = originalBounds.Height + deltaY;
                    break;

                case ResizeHandle.BottomCenter:
                    newBounds.Height = originalBounds.Height + deltaY;
                    break;

                case ResizeHandle.BottomLeft:
                    newBounds.X = originalBounds.X + deltaX;
                    newBounds.Width = originalBounds.Width - deltaX;
                    newBounds.Height = originalBounds.Height + deltaY;
                    break;

                case ResizeHandle.MiddleLeft:
                    newBounds.X = originalBounds.X + deltaX;
                    newBounds.Width = originalBounds.Width - deltaX;
                    break;
            }

            // Enforce minimum size
            if (newBounds.Width < minSize)
            {
                if (handle == ResizeHandle.TopLeft || handle == ResizeHandle.MiddleLeft || handle == ResizeHandle.BottomLeft)
                {
                    newBounds.X = newBounds.Right - minSize;
                }
                newBounds.Width = minSize;
            }

            if (newBounds.Height < minSize)
            {
                if (handle == ResizeHandle.TopLeft || handle == ResizeHandle.TopCenter || handle == ResizeHandle.TopRight)
                {
                    newBounds.Y = newBounds.Bottom - minSize;
                }
                newBounds.Height = minSize;
            }

            // Update the operation's region
            switch (operation)
            {
                case BlurOperation blur:
                    blur.Region = newBounds;
                    break;
                case HighlightOperation highlight:
                    highlight.Region = newBounds;
                    break;
                case TextLabelOperation text:
                    text.Region = newBounds;
                    break;
                case CropOperation crop:
                    crop.Region = newBounds;
                    break;
            }
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
            {
                HighlightColor = Color.FromArgb(160, dlg.Color.R, dlg.Color.G, dlg.Color.B);

                // Update selected highlight if one is selected
                if (_selectedOperationIndex >= 0 && Listbox_Events.SelectedItem is RecordEvent selectedEvent)
                {
                    if (_selectedOperationIndex < selectedEvent.ImageOperations.Count)
                    {
                        var operation = selectedEvent.ImageOperations.Operations[_selectedOperationIndex];
                        if (operation is HighlightOperation highlightOp)
                        {
                            highlightOp.Color = Color.FromArgb(160, dlg.Color.R, dlg.Color.G, dlg.Color.B);
                            RebuildImageFromOperations(selectedEvent);
                            RefreshOperationsListBox();
                            activityTimer.Stop();
                            activityTimer.Start();
                        }
                    }
                }
            }
        }

        private void arrowColourToolStripButton_Click(object sender, EventArgs e)
        {
            using var dlg = new ColorDialog { Color = ArrowColor, FullOpen = true };
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                ArrowColor = dlg.Color;

                // Update selected arrow if one is selected
                if (_selectedOperationIndex >= 0 && Listbox_Events.SelectedItem is RecordEvent selectedEvent)
                {
                    if (_selectedOperationIndex < selectedEvent.ImageOperations.Count)
                    {
                        var operation = selectedEvent.ImageOperations.Operations[_selectedOperationIndex];
                        if (operation is ArrowOperation arrowOp)
                        {
                            arrowOp.Color = dlg.Color;
                            RebuildImageFromOperations(selectedEvent);
                            RefreshOperationsListBox();
                            activityTimer.Stop();
                            activityTimer.Start();
                        }
                    }
                }
            }
        }

        private void textLabelToolStripButton_Click(object sender, EventArgs e)
            => ActivateTool(textLabelToolStripButton.Checked ? ImageTool.Text : ImageTool.None);

        private void textInnerColourToolStripButton_Click(object sender, EventArgs e)
        {
            using var dlg = new ColorDialog { Color = TextInnerColor, FullOpen = true };
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                TextInnerColor = dlg.Color;

                // Update selected text label if one is selected
                if (_selectedOperationIndex >= 0 && Listbox_Events.SelectedItem is RecordEvent selectedEvent)
                {
                    if (_selectedOperationIndex < selectedEvent.ImageOperations.Count)
                    {
                        var operation = selectedEvent.ImageOperations.Operations[_selectedOperationIndex];
                        if (operation is TextLabelOperation textOp)
                        {
                            textOp.InnerColor = dlg.Color;
                            RebuildImageFromOperations(selectedEvent);
                            RefreshOperationsListBox();
                            activityTimer.Stop();
                            activityTimer.Start();
                        }
                    }
                }
            }
        }

        private void textOuterColourToolStripButton_Click(object sender, EventArgs e)
        {
            using var dlg = new ColorDialog { Color = TextOuterColor, FullOpen = true };
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                TextOuterColor = dlg.Color;

                // Update selected text label if one is selected
                if (_selectedOperationIndex >= 0 && Listbox_Events.SelectedItem is RecordEvent selectedEvent)
                {
                    if (_selectedOperationIndex < selectedEvent.ImageOperations.Count)
                    {
                        var operation = selectedEvent.ImageOperations.Operations[_selectedOperationIndex];
                        if (operation is TextLabelOperation textOp)
                        {
                            textOp.OuterColor = dlg.Color;
                            RebuildImageFromOperations(selectedEvent);
                            RefreshOperationsListBox();
                            activityTimer.Stop();
                            activityTimer.Start();
                        }
                    }
                }
            }
        }

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

                    // Cancel any ongoing drag operation
                    if (_isDraggingOperation)
                    {
                        _isDraggingOperation = false;
                        _dragOperationIndex = -1;
                        _dragStartImagePoint = Point.Empty;
                        _dragCurrentImagePoint = Point.Empty;
                    }

                    // Cancel any ongoing resize operation
                    if (_isResizingOperation)
                    {
                        _isResizingOperation = false;
                        _resizeOperationIndex = -1;
                        _activeResizeHandle = ResizeHandle.None;
                        _resizeOriginalBounds = Rectangle.Empty;
                        _resizeStartPoint = Point.Empty;
                    }

                    // Cancel any ongoing canvas text input
                    if (_canvasTextBox != null)
                    {
                        CancelCanvasTextInput();
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

            // Cancel any ongoing canvas text input
            if (_canvasTextBox != null)
            {
                CancelCanvasTextInput();
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

                            // Draw resize handles if this operation supports resizing
                            if (_selectedOperationIndex >= 0 && _selectedOperationIndex < ((RecordEvent)Listbox_Events.SelectedItem).ImageOperations.Count)
                            {
                                var operation = ((RecordEvent)Listbox_Events.SelectedItem).ImageOperations.Operations[_selectedOperationIndex];
                                if (OperationSupportsResize(operation))
                                {
                                    DrawResizeHandles(e.Graphics, controlRect);
                                }
                            }
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

        /// <summary>
        /// Draws resize handles around a rectangle
        /// </summary>
        private void DrawResizeHandles(Graphics g, Rectangle controlBounds)
        {
            const int handleSize = 8;
            var handles = GetResizeHandleRectangles(controlBounds, handleSize);

            using var handleBrush = new SolidBrush(Color.White);
            using var handlePen = new Pen(Color.Black, 1f);

            foreach (var handle in handles.Values)
            {
                g.FillRectangle(handleBrush, handle);
                g.DrawRectangle(handlePen, handle);
            }
        }

        // ── Tool implementations ──────────────────────────────────────────────

                private void ShowTextInputDialog(Rectangle controlRect)
                {
                    // Complete any existing canvas text input first to avoid null reference
                    // when the focus shift triggers LostFocus on the previous text box
                    if (_canvasTextBox != null)
                    {
                        // Temporarily remove the LostFocus handler to prevent re-entry issues
                        _canvasTextBox.LostFocus -= CanvasTextBox_LostFocus;
                        CompleteCanvasTextInput();
                    }

                    // Store the rectangles for later use
                    _textInputControlRect = controlRect;
                    _textInputImageRect = ControlRectToImageRect(controlRect);
                    _editingTextOperationIndex = -1; // Creating new text

                    // Calculate font size based on the drawn box height
                    float estimatedFontSize = Math.Clamp(controlRect.Height * 0.3f, 10f, 72f);

                    // Use the drawn box width as initial width (or minimum 100)
                    int initialWidth = Math.Max(controlRect.Width, 100);

                    // Create a TextBox positioned at the drawn rectangle
                    _canvasTextBox = new TextBox
                    {
                        Location = new Point(controlRect.X, controlRect.Y),
                        Size = new Size(initialWidth, Math.Max(controlRect.Height, 30)),
                        Font = new Font("Segoe UI", estimatedFontSize, FontStyle.Bold),
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.FromArgb(255, 50, 50, 50),
                        ForeColor = TextInnerColor,
                        Multiline = false,
                        Text = "",
                        Tag = "CanvasTextInput" // Tag to identify it
                    };

                    // Add event handlers
                    _canvasTextBox.KeyDown += CanvasTextBox_KeyDown;
                    _canvasTextBox.LostFocus += CanvasTextBox_LostFocus;
                    _canvasTextBox.TextChanged += CanvasTextBox_TextChanged;

                    // Add to pictureBox and focus it
                    pictureBox1.Controls.Add(_canvasTextBox);
                    _canvasTextBox.BringToFront();
                    _canvasTextBox.Focus();
                    _canvasTextBox.SelectAll();
                }

        /// <summary>
        /// Starts editing an existing text operation
        /// </summary>
        private void EditExistingTextOperation(int operationIndex)
        {
            if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;
            if (operationIndex < 0 || operationIndex >= selectedEvent.ImageOperations.Count) return;

            var operation = selectedEvent.ImageOperations.Operations[operationIndex];
            if (!(operation is TextLabelOperation textOp)) return;

            // Complete any existing canvas text input first to avoid null reference
            // when the focus shift triggers LostFocus on the previous text box
            if (_canvasTextBox != null)
            {
                // Temporarily remove the LostFocus handler to prevent re-entry issues
                _canvasTextBox.LostFocus -= CanvasTextBox_LostFocus;
                CompleteCanvasTextInput();
            }

            // Store the operation we're editing
            _editingTextOperationIndex = operationIndex;

            // Get the adjusted bounds in image space
            Rectangle imageBounds = GetAdjustedBounds(textOp.Region, selectedEvent, operationIndex);

            // Convert to control space
            Rectangle controlBounds = ImageRectToControlRect(imageBounds);
            _textInputControlRect = controlBounds;
            _textInputImageRect = imageBounds;

            // Calculate font size based on the control bounds height
            float estimatedFontSize = Math.Clamp(controlBounds.Height * 0.3f, 10f, 72f);

            // Use the operation's width as initial width (or minimum 100)
            int initialWidth = Math.Max(controlBounds.Width, 100);

            // Create a TextBox positioned at the operation's location
            _canvasTextBox = new TextBox
            {
                Location = new Point(controlBounds.X, controlBounds.Y),
                Size = new Size(initialWidth, Math.Max(controlBounds.Height, 30)),
                Font = new Font("Segoe UI", estimatedFontSize, FontStyle.Bold),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(255, 50, 50, 50),
                ForeColor = TextInnerColor,
                Multiline = false,
                Text = textOp.Text, // Pre-populate with existing text
                Tag = "CanvasTextInput"
            };

            // Add event handlers
            _canvasTextBox.KeyDown += CanvasTextBox_KeyDown;
            _canvasTextBox.LostFocus += CanvasTextBox_LostFocus;
            _canvasTextBox.TextChanged += CanvasTextBox_TextChanged;

            // Add to pictureBox and focus it
            pictureBox1.Controls.Add(_canvasTextBox);
            _canvasTextBox.BringToFront();
            _canvasTextBox.Focus();
            _canvasTextBox.SelectAll();

            // Trigger TextChanged to size the box for existing text
            CanvasTextBox_TextChanged(_canvasTextBox, EventArgs.Empty);
        }

        private void CanvasTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                CompleteCanvasTextInput();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                CancelCanvasTextInput();
            }
        }

        private void CanvasTextBox_LostFocus(object sender, EventArgs e)
        {
            // Complete input when clicking away
            CompleteCanvasTextInput();
        }

        private void CanvasTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_canvasTextBox == null) return;

            // Keep width constrained to the originally drawn rectangle
            // Text should fit within the drawn bounds, not expand beyond them
            int fixedWidth = Math.Max(_textInputControlRect.Width, 100);
            _canvasTextBox.Width = fixedWidth;
        }

        private void CompleteCanvasTextInput()
        {
            if (_canvasTextBox == null) return;
            if (_completingTextInput) return; // Prevent re-entry

            _completingTextInput = true;
            try
            {
                string text = _canvasTextBox.Text;

                // Use the original drawn rectangle (not the TextBox size)
                // to ensure the operation region matches what was drawn
                Rectangle finalImageRect = _textInputImageRect;

                // Remove the textbox
                pictureBox1.Controls.Remove(_canvasTextBox);
                _canvasTextBox.Dispose();
                _canvasTextBox = null;

                // Apply the text operation if text was entered
                if (!string.IsNullOrWhiteSpace(text))
                {
                    if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;

                    if (_editingTextOperationIndex >= 0 && _editingTextOperationIndex < selectedEvent.ImageOperations.Count)
                    {
                        // Editing existing text - update text but keep the original region
                        var operation = selectedEvent.ImageOperations.Operations[_editingTextOperationIndex];
                        if (operation is TextLabelOperation textOp)
                        {
                            textOp.Text = text;
                            // Keep the original region - don't update it
                            RebuildImageFromOperations(selectedEvent);
                            RefreshOperationsListBox();
                        }
                    }
                    else
                    {
                        // Creating new text - use the originally drawn rectangle
                        var textOp = new TextLabelOperation(text, finalImageRect, TextInnerColor, TextOuterColor);
                        ApplyOperation(textOp);
                    }
                }
                else if (_editingTextOperationIndex >= 0)
                {
                    // If editing and text is empty, remove the operation
                    if (Listbox_Events.SelectedItem is RecordEvent selectedEvent)
                    {
                        if (_editingTextOperationIndex < selectedEvent.ImageOperations.Count)
                        {
                            selectedEvent.ImageOperations.RemoveOperationAt(_editingTextOperationIndex);
                            RebuildImageFromOperations(selectedEvent);
                            RefreshOperationsListBox();
                        }
                    }
                }

                // Clear state
                _textInputControlRect = Rectangle.Empty;
                _textInputImageRect = Rectangle.Empty;
                _editingTextOperationIndex = -1;

                // Refocus the picture box
                pictureBox1.Focus();
            }
            finally
            {
                _completingTextInput = false;
            }
        }

        private void CancelCanvasTextInput()
        {
            if (_canvasTextBox == null) return;

            // Remove the textbox without applying
            pictureBox1.Controls.Remove(_canvasTextBox);
            _canvasTextBox.Dispose();
            _canvasTextBox = null;

            // Clear state
            _textInputControlRect = Rectangle.Empty;
            _textInputImageRect = Rectangle.Empty;
            _editingTextOperationIndex = -1;

            // Refocus the picture box
            pictureBox1.Focus();
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
                /// Operations are displayed in reverse order (topmost/front at top of list).
                /// </summary>
                private void RefreshOperationsListBox()
                {
                    listBox_Edits.Items.Clear();

                    if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;
                    if (selectedEvent.ImageOperations.Count == 0) return;

                    // Add items in reverse order so the topmost (last applied) appears at the top
                    for (int i = selectedEvent.ImageOperations.Count - 1; i >= 0; i--)
                    {
                        listBox_Edits.Items.Add(selectedEvent.ImageOperations.Operations[i].Description);
                    }
                }

                /// <summary>
                /// Converts a visual listbox index to the actual operation index.
                /// Since the list is displayed in reverse, we need to convert.
                /// </summary>
                private int VisualIndexToOperationIndex(int visualIndex, int operationCount)
                {
                    if (visualIndex < 0 || operationCount <= 0) return -1;
                    return operationCount - 1 - visualIndex;
                }

                /// <summary>
                /// Converts an operation index to a visual listbox index.
                /// Since the list is displayed in reverse, we need to convert.
                /// </summary>
                private int OperationIndexToVisualIndex(int operationIndex, int operationCount)
                {
                    if (operationIndex < 0 || operationCount <= 0) return -1;
                    return operationCount - 1 - operationIndex;
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
        /// Handles double-click on operations listbox to restore to that specific state or edit text operations.
        /// For text operations: opens edit mode
        /// For other operations: removes all operations ABOVE the selected one (newer operations).
        /// </summary>
        private void listBox_Edits_DoubleClick(object sender, EventArgs e)
        {
            if (listBox_Edits.SelectedIndex < 0) return;
            if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;

            int visualIndex = listBox_Edits.SelectedIndex;
            int operationIndex = VisualIndexToOperationIndex(visualIndex, selectedEvent.ImageOperations.Count);
            if (operationIndex < 0 || operationIndex >= selectedEvent.ImageOperations.Count) return;

            var operation = selectedEvent.ImageOperations.Operations[operationIndex];

            // If it's a text operation, enter edit mode
            if (operation is TextLabelOperation)
            {
                EditExistingTextOperation(operationIndex);
                return;
            }

            // For non-text operations, remove all operations after the selected one (visually above = newer = higher index)
            int operationsToRemove = selectedEvent.ImageOperations.Count - 1 - operationIndex;
            for (int i = 0; i < operationsToRemove; i++)
            {
                selectedEvent.ImageOperations.RemoveOperationAt(selectedEvent.ImageOperations.Count - 1);
            }

            // Rebuild the image with the remaining operations
            RebuildImageFromOperations(selectedEvent);
            RefreshOperationsListBox();

            // Select the first item (which is now the topmost remaining operation)
            if (listBox_Edits.Items.Count > 0)
            {
                listBox_Edits.SelectedIndex = 0;
            }

            activityTimer.Stop();
            activityTimer.Start();
        }
    }
}
