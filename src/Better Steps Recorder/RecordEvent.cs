using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Automation;


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
        /*
        public string ElementName {
            get { return this.UIElement.Current.Name; }
        }

        public string ElementType
        {
            get { return this.UIElement.Current.LocalizedControlType; }
        }
        */
        public string? ElementName { get; set; }
        public string? ElementType { get; set; }
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