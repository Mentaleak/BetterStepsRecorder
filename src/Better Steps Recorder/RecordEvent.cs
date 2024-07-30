using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Better_Steps_Recorder
{
    public class RecordEvent
    {
        

        public int ID { get; set; }
        public string WindowTitle { get; set; }
        public (int Left, int Top, int Right, int Bottom) WindowCoordinates { get; set; }
        public (int Width, int Height) WindowSize { get; set; }
        public string UIElement { get; set; }
        public (int X, int Y) MouseCoordinates { get; set; }
        public string EventType { get; set; }
        public string ScreenshotPath { get; set; }

    }
}
