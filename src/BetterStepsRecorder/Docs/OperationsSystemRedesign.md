# Image Operations System Redesign

## Overview
The image editing system has been completely redesigned to use an **operation-based architecture** instead of storing image states. This fundamental change provides:

1. **True non-linear editing**: Operations can be reordered, deleted, or modified without losing subsequent edits
2. **Smaller data footprint**: Store operations (a few bytes) instead of full image states (kilobytes/megabytes)
3. **Future-proof architecture**: Operations can be serialized and saved with the base image for later editing
4. **Reproducible results**: The final image is always reconstructed from base + operations

## Architecture

### Core Components

#### 1. **ImageOperation (Base Class)**
- Abstract base class for all editing operations
- Properties:
  - `Id`: Unique identifier for each operation
  - `CreatedAt`: Timestamp when operation was created
  - `Description`: Human-readable description
- Methods:
  - `Apply(Bitmap)`: Applies the operation to a bitmap
  - `Clone()`: Creates a deep copy

#### 2. **Concrete Operation Classes**
Located in `Core/ImageOperations/`:

- **BlurOperation**: Pixelates/blurs a rectangular region
  - Stores: Region (Rectangle)
  - Implements: Box blur with pixelation effect

- **HighlightOperation**: Draws a semi-transparent colored rectangle
  - Stores: Region (Rectangle), Color
  - Implements: Filled rectangle with custom color

- **ArrowOperation**: Draws an arrow from point A to point B
  - Stores: StartPoint, EndPoint, Color, Width
  - Implements: Anti-aliased arrow with adjustable cap

- **TextLabelOperation**: Adds a text label with background
  - Stores: Text, Region, BackgroundColor, TextColor, FontSize, FontFamily
  - Implements: Text rendering with custom styling

- **CropOperation**: Crops the image to a specific region
  - Stores: Region (Rectangle)
  - Special handling: Changes image dimensions, handled separately in ApplyOperations

#### 3. **ImageOperationsManager**
Located in `Core/ImageOperationsManager.cs`

Manages the list of operations for each RecordEvent:
- `AddOperation(operation)`: Adds a new operation
- `RemoveOperationAt(index)`: Removes operation by index
- `MoveOperation(from, to)`: Reorders operations
- `SwapOperations(i, j)`: Swaps two operations
- `ApplyOperationsToImage(baseImage)`: Reconstructs final image from base + all operations
- `ApplyOperationsToImage(baseImage, upToIndex)`: Reconstructs image up to a specific operation

### Data Flow

#### Before (State-Based):
```
User draws blur
  → Apply blur to current image
  → Save entire image state for undo
  → Display new image

User draws arrow
  → Apply arrow to current image
  → Save entire image state for undo
  → Display new image
```

**Problem**: Can't reorder or selectively delete edits

#### After (Operation-Based):
```
User draws blur
  → Create BlurOperation(region)
  → Add to operations list
  → Rebuild: base + all operations → final image
  → Display final image

User draws arrow
  → Create ArrowOperation(start, end, color)
  → Add to operations list
  → Rebuild: base + all operations → final image
  → Display final image
```

**Benefits**: 
- Reordering: Swap operations, rebuild image
- Deletion: Remove operation, rebuild image
- Undo: Remove last operation, rebuild image

## Integration Points

### RecordEvent Class
Added property:
```csharp
[JsonIgnore]
public ImageOperationsManager ImageOperations { get; set; } = new ImageOperationsManager();
```

### MainForm.ImageRedaction.cs
Key method changes:

1. **Tool application** (MouseUp):
   - Old: `ApplyToImage(bmp => ApplyBlur(...), "Blur")`
   - New: `ApplyOperation(new BlurOperation(region))`

2. **Image reconstruction**:
   ```csharp
   private void RebuildImageFromOperations(RecordEvent evt)
   {
       // Get base screenshot
       byte[]? baseBytes = Program.GetBaseScreenshotBytes(evt);
       using var baseImage = new Bitmap(baseMs);
       
       // Apply all operations
       Bitmap final = evt.ImageOperations.ApplyOperationsToImage(baseImage);
       
       // Update display and save
       pictureBox1.Image = final;
       evt.Screenshotb64 = Convert.ToBase64String(finalBytes);
   }
   ```

3. **Undo**:
   - Old: Pop image state from stack, restore
   - New: Remove last operation, rebuild image

### MainForm.ListBox_EditsOperations.cs
Enhanced edit list functionality:

1. **Delete**: Removes operation(s) and rebuilds
2. **Move Up/Down**: Swaps operations and rebuilds
3. **Drag-Drop**: Reorders operations and rebuilds

**No more destructive warnings!** Operations can be freely reordered.

## Migration Notes

### Backward Compatibility
The old undo system (`_undoStacks`, `_undoLists`) is still present but no longer used for new edits. Operations created with the new system will use the operations-based workflow.

### Data Persistence
Currently, operations are `[JsonIgnore]` and not serialized. To enable save/load:

1. Remove `[JsonIgnore]` from `RecordEvent.ImageOperations`
2. Ensure all operation classes are `[Serializable]`
3. Add JSON converters if needed for complex types (Color, Point, Rectangle)
4. Consider storing base screenshot separately from operations

Example save format:
```json
{
  "ID": "guid",
  "BaseScreenshotPath": "path/to/base.png",
  "ImageOperations": [
    {
      "$type": "BlurOperation",
      "Region": { "X": 10, "Y": 20, "Width": 100, "Height": 50 }
    },
    {
      "$type": "ArrowOperation",
      "StartPoint": { "X": 50, "Y": 60 },
      "EndPoint": { "X": 150, "Y": 160 },
      "Color": "#FF0000FF",
      "Width": 4.0
    }
  ]
}
```

## Benefits

### For Users
- ✅ **Reorder edits** without losing work
- ✅ **Delete any edit** (not just the last one or all subsequent)
- ✅ **Experiment freely** - easy to undo/redo specific changes
- ✅ **Better edit history** - see exactly what operations were applied

### For Developers
- ✅ **Cleaner code** - operation classes are self-contained
- ✅ **Easier testing** - test operations independently
- ✅ **Extensible** - add new operation types easily
- ✅ **Smaller memory footprint** - operations vs. full images

### For Data
- ✅ **Smaller save files** - operations are tiny compared to images
- ✅ **Portable** - operations can be applied to different base images
- ✅ **Editable** - modify operation parameters after creation (future feature)
- ✅ **Version-friendly** - easier to add new operation types without breaking old saves

## Future Enhancements

### Immediate Possibilities
1. **Edit operation parameters** after creation
   - Double-click blur to change region
   - Click arrow to change color/width
   - Edit text labels

2. **Copy/paste operations** between events

3. **Operation presets/templates**
   - Save common operation sequences
   - Apply preset blur sizes, arrow styles, etc.

### Long-term Possibilities
1. **Conditional operations**
   - "Blur all faces" (auto-detect)
   - "Highlight all buttons" (auto-detect)

2. **Parametric operations**
   - Operations that adapt to image size
   - Relative positioning instead of absolute

3. **Smart operations**
   - "Blur all personal information" using OCR
   - "Enhance screenshot" with auto-adjustments

4. **Collaborative editing**
   - Share operations, not images
   - Merge operations from multiple editors

5. **Animation/History playback**
   - Show how edits were applied step-by-step
   - Export to animated GIF/video

## Testing Recommendations

1. **Verify operation correctness**:
   - Create each operation type
   - Verify it appears in the edit list
   - Rebuild image and verify appearance

2. **Test reordering**:
   - Create blur → arrow → highlight
   - Reorder to arrow → blur → highlight
   - Verify visual result changes correctly

3. **Test deletion**:
   - Create multiple operations
   - Delete middle operation
   - Verify subsequent operations still work

4. **Test undo/redo**:
   - Add operations
   - Undo all
   - Verify returns to base image

5. **Performance testing**:
   - Add many operations (50+)
   - Verify rebuild performance
   - Test with large images

## Code Quality Notes

All operation classes are:
- ✅ **Immutable** after creation (except Id/CreatedAt)
- ✅ **Serializable** for future save/load
- ✅ **Self-contained** - no external dependencies
- ✅ **Well-documented** with XML comments
- ✅ **Cloneable** for copy/paste functionality

The ImageOperationsManager:
- ✅ **Thread-safe ready** (can add locks if needed)
- ✅ **Efficient** - rebuilds only when necessary
- ✅ **Flexible** - supports partial application
- ✅ **Robust** - handles crop operations specially

## Summary

This redesign transforms the image editing system from a **destructive, state-based** approach to a **non-destructive, operation-based** approach. This is a significant architectural improvement that enables powerful editing features, reduces memory usage, and makes the data portable and editable in the future.

The system is now ready for advanced features like operation parameter editing, copy/paste, presets, and most importantly: **saving and loading edited screenshots with full edit history**.

---

## Save/Load Implementation (Added)

### New Save Format

The BSR zip file now stores:
1. **Base screenshot** (`BaseScreenshotb64`) - The original screenshot without any operations applied
2. **Operations list** (`Operations`) - Serialized list of all image operations (indicators, edits, etc.)

This approach provides:
- **Smaller file sizes** - Base image stored once, operations are just JSON
- **Full editability** - Operations can be modified, reordered, or deleted after reopening
- **Backward compatibility** - Old files with `Screenshotb64` are still supported

### RecordEvent JSON Structure

```json
{
  "ID": "a1b2c3d4-...",
  "Step": 1,
  "EventType": "Left Click",
  "BaseScreenshotb64": "iVBORw0KGgo...",
  "Operations": [
    {
      "Type": "ClickIndicator",
      "Id": "...",
      "CreatedAt": "2024-01-15T10:30:00",
      "Parameters": {
        "X": 150,
        "Y": 200,
        "Color": -65281,
        "Style": "Circle"
      }
    },
    {
      "Type": "Blur",
      "Id": "...",
      "CreatedAt": "2024-01-15T10:31:00",
      "Parameters": {
        "X": 50,
        "Y": 100,
        "Width": 200,
        "Height": 50
      }
    }
  ],
  ...other fields...
}
```

### Supported Operation Types

| Type | Parameters |
|------|------------|
| `Blur` | X, Y, Width, Height |
| `Highlight` | X, Y, Width, Height, Color (ARGB int) |
| `Arrow` | StartX, StartY, EndX, EndY, Color, Width |
| `TextLabel` | Text, X, Y, Width, Height, FontFamily, FontSize, TextColor, BackgroundColor |
| `Crop` | X, Y, Width, Height |
| `ClickIndicator` | X, Y, Color, Style ("Arrow", "Circle", "Cursor") |
| `DragIndicator` | StartX, StartY, EndX, EndY, Color |

### Key Components

#### ImageOperationDto
Located in `Core/ImageOperations/ImageOperationDto.cs`

Handles serialization/deserialization of operations:
- `FromOperation(operation)` - Converts runtime operation to DTO
- `ToOperation()` - Converts DTO back to runtime operation
- Uses JSON-based `Parameters` property for operation-specific data

#### RecordEvent Methods
- `PrepareForSave()` - Converts `ImageOperations` to serializable `Operations` list
- `RestoreFromLoad()` - Reconstructs `ImageOperations` from serialized `Operations`

#### ZipFileHandler.SaveToZip()
Updated workflow:
1. Snapshot all events
2. For each event:
   - Call `PrepareForSave()` to serialize operations
   - Load base screenshot from `BaseScreenshotSpoolPath` → `BaseScreenshotb64`
   - Serialize to JSON and add to zip

#### LoadRecordEventsFromFile()
Updated workflow:
1. For each event in zip:
   - Deserialize JSON
   - Call `RestoreFromLoad()` to reconstruct operations
   - If `BaseScreenshotb64` exists (new format):
     - Spool base screenshot
     - Apply operations to generate annotated version for display
   - Else if `Screenshotb64` exists (legacy format):
     - Spool as annotated screenshot (backward compatible)

### Export Behavior

Exports **always render the final flattened image** by applying all operations:

```csharp
// ExporterBase.GetRenderedImage()
1. Try to get base screenshot
2. If base exists and operations exist:
   - Apply all operations to base
   - Return rendered result
3. Else fall back to pre-rendered screenshot
```

This ensures exports contain the complete, final image with all edits applied, regardless of the internal storage format.

### Migration Notes

#### Old Files (Pre-Operations)
- Store only `Screenshotb64` (annotated image)
- No `Operations` array
- Loaded as-is, displayed with existing annotations
- Operations list will be empty
- New edits will use operations system

#### New Files (With Operations)
- Store `BaseScreenshotb64` (clean image) + `Operations`
- `Screenshotb64` is null
- On load: base + operations → annotated display
- Full edit capability restored

#### Hybrid Scenario
If user loads old file and adds new edits:
- Old image becomes the "base" (with its annotations baked in)
- New operations are added on top
- Save will store the current state as new base + new operations
