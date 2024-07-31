using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace Better_Steps_Recorder
{
    public class RecordEvent
    {


        public int ID { get; set; }
        public string? WindowTitle { get; set; }
        public string? ApplicationName { get; set; }
        public WindowHelper.RECT WindowCoordinates { get; set; }
        public WindowHelper.Size WindowSize { get; set; }
        public WindowHelper.RECT UICoordinates { get; set; }
        public WindowHelper.Size UISize { get; set; }
        public AutomationElement? UIElement { get; set; }
        public WindowHelper.POINT MouseCoordinates { get; set; }
        public string? EventType { get; set; }
        public string? ScreenshotPath { get; set; }

        public string _StepText { get; set; }

        public override string ToString()
        {
            // Customize the string representation for display in the ListBox
            return $"{ID}: {ApplicationName} {EventType} -  {ElementName}";
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
}