using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace BetterStepsRecorder
{
    public partial class MainForm
    {
        /// <summary>
        /// Handles key down events on the ListBox
        /// </summary>
        private void ListBox1_KeyDown(object? sender, KeyEventArgs e)
        {
            // Check if the Delete key was pressed
            if (e.KeyCode == Keys.Delete)
            {
                deleteToolStripMenuItem_Click(sender, e);
            }
        }

        /// <summary>
        /// Handles selection change in the ListBox
        /// </summary>
        private void Listbox_Events_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetImageTools();
            ClearSelectionHighlight(); // Clear edit selection highlight when changing events

            if (Listbox_Events.SelectedItem is RecordEvent selectedEvent)
            {
                propertyGrid_RecordEvent.SelectedObject = selectedEvent;

                // If there are operations, we need to rebuild from base image + operations
                // to ensure operations are properly displayed
                if (selectedEvent.ImageOperations.Count > 0)
                {
                    // Load the base (un-annotated) image first, then apply operations
                    byte[]? baseBytes = Program.GetBaseScreenshotBytes(selectedEvent);
                    if (baseBytes != null && baseBytes.Length > 0)
                    {
                        try
                        {
                            using (MemoryStream ms = new MemoryStream(baseBytes))
                            {
                                var oldImage = pictureBox1.Image;
                                pictureBox1.Image = new Bitmap(ms);
                                oldImage?.Dispose();
                            }
                            // Rebuild the image with all operations applied
                            // This ensures operations are always rendered on top of the base image
                            RebuildImageFromOperations(selectedEvent);
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Failed to load base screenshot: {ex.Message}");
                            // Fall back to loading the edited screenshot
                            LoadEditedScreenshot(selectedEvent);
                        }
                    }
                    else
                    {
                        // No base image available, load the edited screenshot directly
                        LoadEditedScreenshot(selectedEvent);
                    }
                }
                else
                {
                    // No operations - just load the screenshot directly
                    LoadEditedScreenshot(selectedEvent);
                }

                // Set the step text - use RTF if available for formatted text
                if (!string.IsNullOrEmpty(selectedEvent._StepRtf))
                {
                    richTextBox_stepText.Rtf = selectedEvent._StepRtf;
                }
                else
                {
                    richTextBox_stepText.Text = selectedEvent._StepText;
                }

                // Set the alt text
                textBoxAltText.Text = selectedEvent.AltText ?? string.Empty;

                // Refresh the operations/edits list to show any existing operations
                RefreshOperationsListBox();

                // Update undo button state based on operations
                undoToolStripButton.Enabled = selectedEvent.ImageOperations.Count > 0;

                // Enable reset indicator button - we can always recreate from stored coordinates
                resetIndicatorToolStripButton.Enabled = true;

                // Note: Undo stack is initialized lazily in CommitBitmap when an edit is made,
                // not here on selection, to avoid loading large images into memory unnecessarily.
            }
            else
            {
                // Clear the edits list when no event is selected
                listBox_Edits.Items.Clear();
                // Disable reset indicator when no event selected
                resetIndicatorToolStripButton.Enabled = false;
            }
        }

        /// <summary>
        /// Loads the edited screenshot (Screenshotb64 or spool file) into the picture box
        /// </summary>
        private void LoadEditedScreenshot(RecordEvent selectedEvent)
        {
            byte[]? imgBytes = Program.GetScreenshotBytes(selectedEvent);
            if (imgBytes != null)
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream(imgBytes))
                    {
                        var oldImage = pictureBox1.Image;
                        pictureBox1.Image = new Bitmap(ms);
                        oldImage?.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load screenshot: {ex.Message}");
                    pictureBox1.Image?.Dispose();
                    pictureBox1.Image = null;
                }
            }
            else
            {
                pictureBox1.Image?.Dispose();
                pictureBox1.Image = null;
            }
        }

        /// <summary>
        /// Handles mouse down events on the ListBox
        /// </summary>
        private void Listbox_Events_MouseDown(object sender, MouseEventArgs e)
        {
            // Store the mouse down location for potential dragging
            _mouseDownLocation = e.Location;

            if (e.Button == MouseButtons.Right)
            {
                // Get the index of the item under the mouse cursor
                int index = Listbox_Events.IndexFromPoint(e.Location);
                if (index != ListBox.NoMatches)
                {
                    // Select the item
                    Listbox_Events.SelectedIndex = index;

                    // Show the context menu at the mouse position
                    contextMenu_ListBox_Events.Show(Listbox_Events, e.Location);
                }
            }
        }

        /// <summary>
        /// Handles mouse move events on the ListBox for drag and drop
        /// </summary>
        private void Listbox_Events_MouseMove(object sender, MouseEventArgs e)
        {
            // Start the drag-and-drop operation if the left button is held and the mouse moved
            if (e.Button == MouseButtons.Left)
            {
                int index = Listbox_Events.IndexFromPoint(_mouseDownLocation);
                if (index != ListBox.NoMatches)
                {
                    // Determine if the drag threshold has been met
                    if (Math.Abs(e.X - _mouseDownLocation.X) > SystemInformation.DragSize.Width ||
                        Math.Abs(e.Y - _mouseDownLocation.Y) > SystemInformation.DragSize.Height)
                    {
                        // Start the drag-and-drop operation
                        Listbox_Events.DoDragDrop(Listbox_Events.Items[index], DragDropEffects.Move);
                    }
                }
            }
        }

        /// <summary>
        /// Handles drag over events on the ListBox
        /// </summary>
        private void Listbox_Events_DragOver(object sender, DragEventArgs e)
        {
            // Get the mouse position relative to the ListBox
            Point point = Listbox_Events.PointToClient(new Point(e.X, e.Y));

            // Determine the height of the ListBox and the threshold for scrolling
            int scrollRegionHeight = 20; // Adjust this value as needed for sensitivity
            int scrollSpeed = 1; // Number of items to scroll at a time

            // Scroll up if the mouse is near the top of the ListBox
            if (point.Y < scrollRegionHeight)
            {
                int newTopIndex = Math.Max(Listbox_Events.TopIndex - scrollSpeed, 0);
                Listbox_Events.TopIndex = newTopIndex;
            }
            // Scroll down if the mouse is near the bottom of the ListBox
            else if (point.Y > Listbox_Events.Height - scrollRegionHeight)
            {
                int newTopIndex = Math.Min(Listbox_Events.TopIndex + scrollSpeed, Listbox_Events.Items.Count - 1);
                Listbox_Events.TopIndex = newTopIndex;
            }

            // Set the drag effect to indicate a move operation
            e.Effect = DragDropEffects.Move;
        }

        /// <summary>
        /// Handles drag enter events on the ListBox
        /// </summary>
        private void Listbox_Events_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(RecordEvent)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// Handles drag drop events on the ListBox
        /// </summary>
        private void Listbox_Events_DragDrop(object sender, DragEventArgs e)
        {
            // Get the index of the item where the drop occurred
            Point point = Listbox_Events.PointToClient(new Point(e.X, e.Y));
            int targetIndex = Listbox_Events.IndexFromPoint(point);

            if (targetIndex < 0) targetIndex = Listbox_Events.Items.Count - 1;

            // Get the dragged item from the data
            var draggedEvent = e.Data.GetData(typeof(RecordEvent)) as RecordEvent;

            if (draggedEvent != null)
            {
                // Find the original index of the dragged item
                int originalIndex = Listbox_Events.Items.IndexOf(draggedEvent);

                // Check if the item was dragged to a new position
                if (originalIndex != targetIndex)
                {
                    // Remove the item from its original position in the data source
                    Program._recordEvents.RemoveAt(originalIndex);

                    // Insert the item into the new position in the data source
                    Program._recordEvents.Insert(targetIndex, draggedEvent);

                    // Remove the item from the ListBox
                    Listbox_Events.Items.RemoveAt(originalIndex);

                    // Insert the item into the new position in the ListBox
                    Listbox_Events.Items.Insert(targetIndex, draggedEvent);

                    // Set the new selected index
                    Listbox_Events.SelectedIndex = targetIndex;

                    // Update the step numbers and other necessary UI elements
                    UpdateListItems();
                }
            }
        }

        /// <summary>
        /// Handles the delete menu item click
        /// </summary>
        private void deleteToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (Listbox_Events.SelectedItems.Count > 0)
            {
                // Create a list to store the selected events to remove them safely
                List<RecordEvent> selectedEvents = new List<RecordEvent>();

                // Collect all selected events
                foreach (var item in Listbox_Events.SelectedItems)
                {
                    if (item is RecordEvent selectedEvent)
                    {
                        selectedEvents.Add(selectedEvent);
                    }
                }

                // Remove each selected event
                foreach (var selectedEvent in selectedEvents)
                {
                    Listbox_Events.Items.Remove(selectedEvent);

                    var recordEvent = Program._recordEvents.Find(e => e.ID == selectedEvent.ID);
                    if (recordEvent != null)
                    {
                        Program._recordEvents.Remove(recordEvent);
                    }
                    else
                    {
                        // Handle the case where the event is not found, if necessary
                        MessageBox.Show("One or more selected events were not found in the record events list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                // Update the display of list items after removal
                UpdateListItems();
            }
        }

        /// <summary>
        /// Handles the move up menu item click
        /// </summary>
        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get the selected indices and sort them
            var selectedIndices = Listbox_Events.SelectedIndices.Cast<int>().ToList();
            selectedIndices.Sort();

            // Move selected items up
            foreach (int index in selectedIndices)
            {
                if (index > 0) // Ensure there's an item above to swap with
                {
                    // Swap the selected item with the one above it
                    var temp = Program._recordEvents[index];
                    Program._recordEvents[index] = Program._recordEvents[index - 1];
                    Program._recordEvents[index - 1] = temp;

                    // Update the ListBox
                    Listbox_Events.Items[index] = Program._recordEvents[index];
                    Listbox_Events.Items[index - 1] = Program._recordEvents[index - 1];
                }
            }

            // Update the selected indices
            Listbox_Events.ClearSelected();
            foreach (var index in selectedIndices)
            {
                if (index > 0)
                {
                    Listbox_Events.SetSelected(index - 1, true);
                }
            }

            // Update the display
            UpdateListItems();
        }

        /// <summary>
        /// Handles the move down menu item click
        /// </summary>
        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get the selected indices and sort them in descending order
            var selectedIndices = Listbox_Events.SelectedIndices.Cast<int>().ToList();
            selectedIndices.Sort((x, y) => y.CompareTo(x));

            // Move selected items down
            foreach (int index in selectedIndices)
            {
                if (index < Program._recordEvents.Count - 1) // Ensure there's an item below to swap with
                {
                    // Swap the selected item with the one below it
                    var temp = Program._recordEvents[index];
                    Program._recordEvents[index] = Program._recordEvents[index + 1];
                    Program._recordEvents[index + 1] = temp;

                    // Update the ListBox
                    Listbox_Events.Items[index] = Program._recordEvents[index];
                    Listbox_Events.Items[index + 1] = Program._recordEvents[index + 1];
                }
            }

            // Update the selected indices
            Listbox_Events.ClearSelected();
            foreach (var index in selectedIndices)
            {
                if (index < Program._recordEvents.Count - 1)
                {
                    Listbox_Events.SetSelected(index + 1, true);
                }
            }

            // Update the display
            UpdateListItems();
        }
    }
}