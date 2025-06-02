# Functions List for Better Steps Recorder

This document provides a comprehensive list of functions in the Better Steps Recorder application, organized by file and class.

## Program.cs

| Function | Description |
|----------|-------------|
| `Main` | Entry point of the application that initializes the application and runs the main form |
| `HookMouseOperations` | Sets up the low-level mouse hook to begin recording user clicks |
| `UnHookMouseOperations` | Removes the low-level mouse hook to stop recording user clicks |
| `LoadRecordEventsFromFile` | Loads previously recorded events from a zip file |
| `SetHook` | Sets up the Windows hook for mouse events |
| `HookCallback` | Callback function that processes mouse events during recording |
| `SaveScreenRegionScreenshot` | Takes a screenshot of a specific region of the screen and returns it as base64 string |
| `ExportToRTF` | Exports the recorded steps to an RTF document format |
| `ExportToHTML` | Exports the recorded steps to an HTML document format |
| `GetRtfImage` | Converts an image to RTF format for inclusion in RTF documents |
| `DrawArrowAtCursor` | Draws an arrow pointing to the cursor position on a screenshot |
| `Base64ToImage` | Converts a base64 string to an Image object |
| `ImageToBase64` | Converts an Image object to a base64 string |

## WindowHelper.cs

| Function | Description |
|----------|-------------|
| `Cleanup` | Properly disposes of automation resources |
| `GetElementFromPoint` | Gets the UI automation element at a specific screen point |
| `GetElementFromCursor` | Gets the UI automation element under the current cursor position |
| `GetElementFromHandle` | Gets the UI automation element from a window handle |
| `FindElementByName` | Finds a UI element by its name |
| `FindElementByAutomationId` | Finds a UI element by its automation ID |
| `GetAllWindows` | Gets a list of all top-level windows |
| `GetWindowDetails` | Gets detailed information about a window |
| `GetWindowUnderCursor` | Gets the handle of the window currently under the cursor |
| `GetTopLevelWindowRect` | Gets the rectangle coordinates of a top-level window |
| `GetTopLevelWindowTitle` | Gets the title of a top-level window |
| `GetWindowText` | Gets the text of a window |
| `GetApplicationName` | Gets the name of the application that owns a window |

## Form1.cs

| Function | Description |
|----------|-------------|
| `DisableRecording` | Stops the recording process and updates UI |
| `EnableRecording` | Starts the recording process and updates UI |
| `EnableDisable_exportToolStripMenuItem` | Enables or disables export menu items based on current state |
| `Form1_FormClosing` | Handles cleanup when the form is closing |
| `Form1_Load` | Initializes the form when it's loaded |
| `ListBox1_KeyDown` | Handles keyboard events in the listbox |
| `Listbox_Events_DragDrop` | Handles drag and drop operations in the listbox |
| `Listbox_Events_DragEnter` | Handles the start of drag operations in the listbox |
| `Listbox_Events_DragOver` | Handles dragging over the listbox |
| `Listbox_Events_MouseDown` | Handles mouse down events in the listbox |
| `Listbox_Events_MouseMove` | Handles mouse movement in the listbox |
| `Listbox_Events_SelectedIndexChanged` | Updates UI when a different item is selected in the listbox |
| `ToolStripMenuItem_Recording_Click` | Toggles recording state when the recording menu item is clicked |
| `UpdateListItems` | Updates the items displayed in the listbox |
| `activityTimer_Tick` | Handles the timer tick event for user activity monitoring |
| `deleteToolStripMenuItem_Click` | Deletes the selected item when the delete menu option is clicked |
| `exportToolStripMenuItem_Click` | Handles the export menu item click |
| `helpToolStripMenuItem_Click` | Shows the help dialog when the help menu item is clicked |
| `moveDownToolStripMenuItem_Click` | Moves the selected item down in the list |
| `moveUpToolStripMenuItem_Click` | Moves the selected item up in the list |
| `newToolStripMenuItem_Click` | Creates a new recording when the new menu item is clicked |
| `openToolStripMenuItem_Click` | Opens an existing recording file |
| `richTextBox_stepText_Leave` | Updates step text when focus leaves the rich text box |
| `richTextBox_stepText_TextChanged` | Handles changes to the text in the rich text box |
| `toolStripMenuItem1_SaveAs_Click` | Saves the recording to a file |
| `AddRecordEventToListBox` | Adds a recorded event to the listbox |
| `ClearListBox` | Clears all items from the listbox |

## FileDialogHelper.cs

| Function | Description |
|----------|-------------|
| `ShowOpenFileDialog` | Displays an open file dialog and returns the selected file path |
| `ShowSaveFileDialog` | Displays a save file dialog and returns the selected file path |
| `SaveAs` | Shows a save dialog and saves the current recording |

## RecordEvent.cs

| Function | Description |
|----------|-------------|
| `ToString` | Converts a RecordEvent to a string representation |
| `GetDetailedElementDescription` | Gets a detailed description of the UI element |
| `GetElementPath` | Gets the path of an element in the UI hierarchy |
| `GetAcceleratorKey` | Gets the accelerator key for a UI element |
| `GetAccessKey` | Gets the access key for a UI element |
| `GetAutomationId` | Gets the automation ID of a UI element |
| `GetClassName` | Gets the class name of a UI element |
| `GetHelpText` | Gets the help text for a UI element |

## ExporterBase.cs

| Function | Description |
|----------|-------------|
| `Export` | Abstract method for exporting steps to a specific format |
| `SaveImageFromBase64` | Saves an image from a base64 string to a file |
| `EnsureDirectoryExists` | Ensures a directory exists, creating it if necessary |
| `ShowExportError` | Shows an error message for export failures |
| `ShowExportSuccess` | Shows a success message after export |

## OdtExporter.cs

| Function | Description |
|----------|-------------|
| `Export` | Exports the recorded steps to ODT format |
| `CreateManifestFile` | Creates the manifest file for the ODT document |
| `CreateContentFile` | Creates the content file for the ODT document |
| `CreateStylesFile` | Creates the styles file for the ODT document |
| `CreateMetaFile` | Creates the metadata file for the ODT document |
| `SaveImages` | Saves images from the recording to the ODT package |
| `GetImageDimensions` | Gets the dimensions of an image from a base64 string |

## ZipFileHandler.cs

| Function | Description |
|----------|-------------|
| `SaveToZip` | Saves recorded events to a zip file |

## PictureBoxTools.cs

| Function | Description |
|----------|-------------|
| `ApplyBlur` | Applies a blur effect to an image |
| `pictureBox_MouseDown` | Handles mouse down events in the picture box |
| `pictureBox_MouseMove` | Handles mouse movement in the picture box |
| `pictureBox_MouseUp` | Handles mouse up events in the picture box |

## ShellExecuteHelper.cs

| Function | Description |
|----------|-------------|
| `OpenWithDefaultProgram` | Opens a file with its default associated program |
| `ShowOpenWithDialog` | Shows the Windows "Open With" dialog for a file |

## HelpPopup.cs

| Function | Description |
|----------|-------------|
| `GetVersion` | Gets the application version |
| `HelpPopup_Load` | Initializes the help popup when loaded |
| `button_CloseHelp_Click` | Closes the help popup when the close button is clicked |
| `linkLabel1_LinkClicked` | Handles clicks on links in the help popup |

## UI/Dialogs/ExportDialogs.cs

| Function | Description |
|----------|-------------|
| `HandleHtmlExport` | Handles exporting to HTML format |
| `HandleObsidianExport` | Handles exporting to Obsidian format |
| `HandleOdtExport` | Handles exporting to ODT format |
| `HandleRtfExport` | Handles exporting to RTF format |
| `PromptForFileName` | Prompts the user for a filename |
| `SelectObsidianVault` | Allows selection of an Obsidian vault |
| `SelectSubfolder` | Allows selection of a subfolder |
| `ShowHtmlSaveDialog` | Shows a save dialog for HTML files |
| `ShowOdtSaveDialog` | Shows a save dialog for ODT files |
| `ShowRtfSaveDialog` | Shows a save dialog for RTF files |