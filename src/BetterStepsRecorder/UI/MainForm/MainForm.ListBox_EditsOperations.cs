using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BetterStepsRecorder
{
    public partial class MainForm
    {
        private Point _mouseDownLocationEdits;

        /// <summary>
        /// Handles key down events on the listBox_Edits
        /// </summary>
        private void listBox_Edits_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedEdit();
            }
        }

        /// <summary>
        /// Handles mouse down events on the listBox_Edits
        /// </summary>
        private void listBox_Edits_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseDownLocationEdits = e.Location;

            if (e.Button == MouseButtons.Right)
            {
                int index = listBox_Edits.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    listBox_Edits.SelectedIndex = index;
                }
            }
        }

        /// <summary>
        /// Handles mouse move events on the listBox_Edits for drag and drop
        /// </summary>
        private void listBox_Edits_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && listBox_Edits.SelectedIndex >= 0)
            {
                if (Math.Abs(e.X - _mouseDownLocationEdits.X) > SystemInformation.DragSize.Width ||
                    Math.Abs(e.Y - _mouseDownLocationEdits.Y) > SystemInformation.DragSize.Height)
                {
                    int index = listBox_Edits.IndexFromPoint(_mouseDownLocationEdits);
                    if (index != ListBox.NoMatches)
                    {
                        listBox_Edits.DoDragDrop(new EditDragData { Index = index, Text = listBox_Edits.Items[index].ToString() }, DragDropEffects.Move);
                    }
                }
            }
        }

        /// <summary>
        /// Handles drag enter events on the listBox_Edits
        /// </summary>
        private void listBox_Edits_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(EditDragData)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// Handles drag over events on the listBox_Edits
        /// </summary>
        private void listBox_Edits_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(EditDragData)))
            {
                e.Effect = DragDropEffects.Move;

                Point point = listBox_Edits.PointToClient(new Point(e.X, e.Y));
                int scrollRegionHeight = 20;
                int scrollSpeed = 1;

                if (point.Y < scrollRegionHeight && listBox_Edits.TopIndex > 0)
                {
                    listBox_Edits.TopIndex = Math.Max(listBox_Edits.TopIndex - scrollSpeed, 0);
                }
                else if (point.Y > listBox_Edits.Height - scrollRegionHeight)
                {
                    int maxTopIndex = Math.Max(0, listBox_Edits.Items.Count - 1);
                    listBox_Edits.TopIndex = Math.Min(listBox_Edits.TopIndex + scrollSpeed, maxTopIndex);
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// Handles drag drop events on the listBox_Edits
        /// </summary>
        private void listBox_Edits_DragDrop(object sender, DragEventArgs e)
        {
            if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;

            var dragData = e.Data.GetData(typeof(EditDragData)) as EditDragData;
            if (dragData == null) return;

            Point point = listBox_Edits.PointToClient(new Point(e.X, e.Y));
            int targetVisualIndex = listBox_Edits.IndexFromPoint(point);

            if (targetVisualIndex < 0) targetVisualIndex = listBox_Edits.Items.Count - 1;

            int sourceVisualIndex = dragData.Index;

            // Convert visual indices to operation indices
            int sourceOperationIndex = VisualIndexToOperationIndex(sourceVisualIndex, selectedEvent.ImageOperations.Count);
            int targetOperationIndex = VisualIndexToOperationIndex(targetVisualIndex, selectedEvent.ImageOperations.Count);

            if (sourceOperationIndex != targetOperationIndex && 
                sourceOperationIndex >= 0 && sourceOperationIndex < selectedEvent.ImageOperations.Count && 
                targetOperationIndex >= 0 && targetOperationIndex < selectedEvent.ImageOperations.Count)
            {
                // Move the operation
                selectedEvent.ImageOperations.MoveOperation(sourceOperationIndex, targetOperationIndex);

                // Rebuild the image with the reordered operations
                RebuildImageFromOperations(selectedEvent);

                // Refresh the listbox
                RefreshOperationsListBox();
                listBox_Edits.SelectedIndex = targetVisualIndex;
            }
        }

        /// <summary>
        /// Handles the move up context menu item click for edits.
        /// Since the list is displayed in reverse (front at top), "Move Up" visually 
        /// means moving to a higher layer (later in the operations list).
        /// </summary>
        private void moveUpEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox_Edits.SelectedIndex <= 0) return;
            if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;

            int visualIndex = listBox_Edits.SelectedIndex;
            int operationIndex = VisualIndexToOperationIndex(visualIndex, selectedEvent.ImageOperations.Count);

            // Move up visually = move to higher index (later in list = more on top)
            if (operationIndex >= 0 && operationIndex < selectedEvent.ImageOperations.Count - 1)
            {
                selectedEvent.ImageOperations.SwapOperations(operationIndex, operationIndex + 1);

                // Rebuild the image
                RebuildImageFromOperations(selectedEvent);

                RefreshOperationsListBox();
                listBox_Edits.SelectedIndex = visualIndex - 1; // Visual index goes up (decreases)
            }
        }

        /// <summary>
        /// Handles the move down context menu item click for edits.
        /// Since the list is displayed in reverse (front at top), "Move Down" visually 
        /// means moving to a lower layer (earlier in the operations list).
        /// </summary>
        private void moveDownEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox_Edits.SelectedIndex < 0) return;
            if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;

            int visualIndex = listBox_Edits.SelectedIndex;
            int operationIndex = VisualIndexToOperationIndex(visualIndex, selectedEvent.ImageOperations.Count);

            // Move down visually = move to lower index (earlier in list = more behind)
            if (operationIndex > 0 && operationIndex < selectedEvent.ImageOperations.Count)
            {
                selectedEvent.ImageOperations.SwapOperations(operationIndex, operationIndex - 1);

                // Rebuild the image
                RebuildImageFromOperations(selectedEvent);

                RefreshOperationsListBox();
                listBox_Edits.SelectedIndex = visualIndex + 1; // Visual index goes down (increases)
            }
        }

        /// <summary>
        /// Handles the move to front context menu item click for edits (move to end of operations list = top layer = top of visual list)
        /// </summary>
        private void moveToFrontEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox_Edits.SelectedIndex < 0) return;
            if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;

            int visualIndex = listBox_Edits.SelectedIndex;
            int operationIndex = VisualIndexToOperationIndex(visualIndex, selectedEvent.ImageOperations.Count);
            int lastIndex = selectedEvent.ImageOperations.Count - 1;

            if (operationIndex >= 0 && operationIndex < lastIndex)
            {
                // Move the operation to the end (front/top layer)
                selectedEvent.ImageOperations.MoveOperation(operationIndex, lastIndex);

                // Rebuild the image
                RebuildImageFromOperations(selectedEvent);

                RefreshOperationsListBox();
                listBox_Edits.SelectedIndex = 0; // Top of visual list
            }
        }

        /// <summary>
        /// Handles the send to back context menu item click for edits (move to start of operations list = bottom layer = bottom of visual list)
        /// </summary>
        private void sendToBackEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox_Edits.SelectedIndex < 0) return;
            if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;

            int visualIndex = listBox_Edits.SelectedIndex;
            int operationIndex = VisualIndexToOperationIndex(visualIndex, selectedEvent.ImageOperations.Count);

            if (operationIndex > 0 && operationIndex < selectedEvent.ImageOperations.Count)
            {
                // Move the operation to the start (back/bottom layer)
                selectedEvent.ImageOperations.MoveOperation(operationIndex, 0);

                // Rebuild the image
                RebuildImageFromOperations(selectedEvent);

                RefreshOperationsListBox();
                listBox_Edits.SelectedIndex = listBox_Edits.Items.Count - 1; // Bottom of visual list
            }
        }

        /// <summary>
        /// Handles the delete context menu item click for edits
        /// </summary>
        private void deleteEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedEdit();
        }

        /// <summary>
        /// Deletes the selected edit from the edit history
        /// </summary>
        private void DeleteSelectedEdit()
        {
            if (listBox_Edits.SelectedIndex < 0) return;
            if (!(Listbox_Events.SelectedItem is RecordEvent selectedEvent)) return;

            int visualIndex = listBox_Edits.SelectedIndex;
            int operationIndex = VisualIndexToOperationIndex(visualIndex, selectedEvent.ImageOperations.Count);

            if (operationIndex >= 0 && operationIndex < selectedEvent.ImageOperations.Count)
            {
                // Remove only this operation
                selectedEvent.ImageOperations.RemoveOperationAt(operationIndex);

                // Clear selection highlight since operation is deleted
                ClearSelectionHighlight();

                // Rebuild the image with the remaining operations
                RebuildImageFromOperations(selectedEvent);
                RefreshOperationsListBox();

                // Select the same visual position or adjust if at end
                if (listBox_Edits.Items.Count > 0)
                {
                    if (visualIndex >= listBox_Edits.Items.Count)
                        listBox_Edits.SelectedIndex = listBox_Edits.Items.Count - 1;
                    else
                        listBox_Edits.SelectedIndex = visualIndex;
                }

                // Update undo button state
                undoToolStripButton.Enabled = selectedEvent.ImageOperations.Count > 0;
            }
        }

        /// <summary>
        /// Restores the image to a specific state
        /// </summary>
        private void RestoreImageState(RecordEvent selectedEvent, string imageState)
        {
            if (!string.IsNullOrEmpty(imageState))
            {
                byte[] bytes = Convert.FromBase64String(imageState);
                using var ms = new MemoryStream(bytes);
                var oldImage = pictureBox1.Image;
                pictureBox1.Image = new Bitmap(ms);
                oldImage?.Dispose();

                using var saveMs = new MemoryStream();
                pictureBox1.Image.Save(saveMs, ImageFormat.Png);
                selectedEvent.Screenshotb64 = Convert.ToBase64String(saveMs.ToArray());
            }
            else
            {
                pictureBox1.Image?.Dispose();
                pictureBox1.Image = null;
                selectedEvent.Screenshotb64 = "";
            }
        }

        /// <summary>
        /// Helper class for drag-and-drop data
        /// </summary>
        private class EditDragData
        {
            public int Index { get; set; }
            public string Text { get; set; }
        }
    }
}
