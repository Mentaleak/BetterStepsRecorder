using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FlaUI.Core.AutomationElements;
using BetterStepsRecorder.Core;
using BetterStepsRecorder.Core.ImageOperations;

namespace BetterStepsRecorder
{
    public class RecordEvent
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        private DateTime _CreationTime = DateTime.Now;

        [TypeConverter(typeof(DateTimeWithSecondsConverter))]
        public DateTime CreationTime 
        {
            get { return _CreationTime; }
            set { _CreationTime = value; }
        }

        public string? WindowTitle { get; set; }
        public string? ApplicationName { get; set; }
        public WindowHelper.RECT WindowCoordinates { get; set; }
        public WindowHelper.Size WindowSize { get; set; }
        public WindowHelper.RECT UICoordinates { get; set; }
        public WindowHelper.Size UISize { get; set; }
        public int Step { get; set; }

        [JsonIgnore] 
        public AutomationElement? UIElement { get; set; }

        public WindowHelper.POINT MouseCoordinates { get; set; }
        public WindowHelper.POINT? DragStartCoordinates { get; set; }
        public WindowHelper.POINT? DragEndCoordinates { get; set; }
        public string? EventType { get; set; }

        /// <summary>
        /// Base screenshot without any operations applied (used for save files).
        /// This is serialized to JSON and represents the clean screenshot.
        /// </summary>
        public string? BaseScreenshotb64 { get; set; }

        /// <summary>
        /// Legacy: Annotated screenshot with all operations applied.
        /// Kept for backward compatibility with older save files.
        /// New saves will only use BaseScreenshotb64 + Operations.
        /// </summary>
        public string? Screenshotb64 { get; set; }

        /// <summary>
        /// Serialized list of image operations to apply to the base screenshot.
        /// </summary>
        public List<ImageOperationDto>? Operations { get; set; }

        public string? _StepText { get; set; }

        /// <summary>
        /// RTF-formatted step text for rich text editing. When set, this is used to
        /// populate the RichTextBox. _StepText is kept in sync as the plain text version
        /// for export and backward compatibility.
        /// </summary>
        public string? _StepRtf { get; set; }

        /// <summary>
        /// Alt text for the screenshot image, used for accessibility in exports.
        /// This provides a text description of the image for screen readers.
        /// </summary>
        public string? AltText { get; set; }

        /// <summary>
        /// Path to the spooled PNG on disk. When set, Screenshotb64 is null and the image
        /// is read from this path on demand instead of being held in RAM.
        /// </summary>
        [JsonIgnore]
        public string? ScreenshotSpoolPath { get; set; }

        /// <summary>
        /// Base screenshot without any indicators, stored for undo functionality.
        /// When set, this allows users to undo the initial indicator drawing.
        /// </summary>
        [JsonIgnore]
        public string? BaseScreenshotSpoolPath { get; set; }

        /// <summary>
        /// Manages the list of image operations applied to the base screenshot
        /// </summary>
        [JsonIgnore]
        public ImageOperationsManager ImageOperations { get; set; } = new ImageOperationsManager();

        /// <summary>True when a screenshot is available either in RAM or via a spool file on disk.</summary>
        [JsonIgnore]
        public bool HasScreenshot => !string.IsNullOrEmpty(Screenshotb64) || 
                                      !string.IsNullOrEmpty(BaseScreenshotb64) ||
                                      !string.IsNullOrEmpty(ScreenshotSpoolPath) ||
                                      !string.IsNullOrEmpty(BaseScreenshotSpoolPath);

        /// <summary>Returns the first 8 hex characters of the ID for use in filenames.</summary>
        public string ShortId => ID.ToString("N")[..8];

        public override string ToString()
        {
            // Use only the first line of step text for display in the ListBox
            string? displayText = _StepText;
            if (!string.IsNullOrEmpty(displayText))
            {
                int breakIndex = displayText.IndexOfAny(['\r', '\n', '\v']);
                if (breakIndex >= 0)
                    displayText = displayText[..breakIndex];
            }
            return $"{Step}: {displayText}";
        }

        public string? ElementName { get; set; }
        public string? ElementType { get; set; }

        /// <summary>
        /// Prepares the event for serialization by converting runtime operations to DTOs
        /// and setting up the base screenshot for saving.
        /// </summary>
        public void PrepareForSave()
        {
            // Convert runtime ImageOperations to serializable DTOs
            if (ImageOperations.Count > 0)
            {
                Operations = ImageOperationDto.FromOperations(ImageOperations.Operations);
            }
            else
            {
                Operations = null;
            }
        }

        /// <summary>
        /// Restores runtime state after deserialization by converting DTOs back to operations.
        /// </summary>
        public void RestoreFromLoad()
        {
            // Convert serialized DTOs back to runtime ImageOperations
            ImageOperations = new ImageOperationsManager();
            if (Operations != null && Operations.Count > 0)
            {
                foreach (var dto in Operations)
                {
                    try
                    {
                        ImageOperations.AddOperation(dto.ToOperation());
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to restore operation: {ex.Message}");
                    }
                }
            }
        }

        // Helper methods to get element properties using FlaUI
        public static string? GetDetailedElementDescription(AutomationElement element)
        {
            if (element == null) return null;
            
            var sb = new StringBuilder();
            sb.AppendLine($"Name: {element.Name}");
            sb.AppendLine($"ControlType: {element.ControlType}");
            sb.AppendLine($"AutomationId: {element.AutomationId}");
            sb.AppendLine($"ClassName: {element.ClassName}");
            
            if (element.Properties.HelpText.IsSupported)
                sb.AppendLine($"HelpText: {element.Properties.HelpText.Value}");
            
            if (element.Properties.AcceleratorKey.IsSupported)
                sb.AppendLine($"AcceleratorKey: {element.Properties.AcceleratorKey.Value}");
            
            if (element.Properties.AccessKey.IsSupported)
                sb.AppendLine($"AccessKey: {element.Properties.AccessKey.Value}");
            
            return sb.ToString();
        }
        
        public static string? GetElementPath(AutomationElement element)
        {
            if (element == null) return null;
            
            var path = new List<string>();
            var current = element;
            
            while (current != null)
            {
                string elementInfo = !string.IsNullOrEmpty(current.Name) 
                    ? $"{current.ControlType}:{current.Name}" 
                    : current.ControlType.ToString();
                
                path.Add(elementInfo);
                current = current.Parent;
            }
            
            path.Reverse();
            return string.Join(" > ", path);
        }
        
        public static string? GetAcceleratorKey(AutomationElement element)
        {
            return element?.Properties.AcceleratorKey.IsSupported == true 
                ? element.Properties.AcceleratorKey.Value 
                : null;
        }
        
        public static string? GetAccessKey(AutomationElement element)
        {
            return element?.Properties.AccessKey.IsSupported == true 
                ? element.Properties.AccessKey.Value 
                : null;
        }
        
        public static string? GetAutomationId(AutomationElement element)
        {
            try
            {
                if (element == null) return null;
                
                // Get the raw automation ID
                string? automationId = element.AutomationId;
                
                // Check if it's in a special format like [#3011]
                if (string.IsNullOrEmpty(automationId) && element.Properties.AutomationId.IsSupported)
                {
                    // Try to get the automation ID through properties
                    automationId = element.Properties.AutomationId.Value;
                }
                
                return automationId;
            }
            catch (Exception)
            {
                // If there's any exception accessing the automation ID, return null
                return null;
            }
        }
        
        public static string? GetClassName(AutomationElement element)
        {
            return element?.ClassName;
        }
        
        public static string? GetHelpText(AutomationElement element)
        {
            return element?.Properties.HelpText.IsSupported == true 
                ? element.Properties.HelpText.Value 
                : null;
        }
    }

    public class DateTimeWithSecondsConverter : DateTimeConverter
    {
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is DateTime dt)
            {
                return dt.ToString("yyyy-MM-dd HH:mm:ss", culture);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string s)
            {
                if (DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm:ss", culture, DateTimeStyles.None, out DateTime dt))
                {
                    return dt;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}