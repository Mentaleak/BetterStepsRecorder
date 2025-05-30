using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FlaUI.Core.AutomationElements;

namespace Better_Steps_Recorder
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
        public string? EventType { get; set; }
        public string? Screenshotb64 { get; set; }

        public string? _StepText { get; set; }

        public override string ToString()
        {
            // Customize the string representation for display in the ListBox
            return $"{Step}: {_StepText}";
        }

        public string? ElementName { get; set; }
        public string? ElementType { get; set; }
        
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