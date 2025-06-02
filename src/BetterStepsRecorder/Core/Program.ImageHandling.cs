using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Debug = System.Diagnostics.Debug;
using static BetterStepsRecorder.WindowHelper;

namespace BetterStepsRecorder
{
    internal static partial class Program
    {
        /// <summary>
        /// Captures a screenshot of a specific region of the screen and returns it as a Base64 string
        /// </summary>
        /// <param name="x">X coordinate of the top-left corner</param>
        /// <param name="y">Y coordinate of the top-left corner</param>
        /// <param name="width">Width of the region to capture</param>
        /// <param name="height">Height of the region to capture</param>
        /// <param name="eventId">ID of the associated record event</param>
        /// <returns>Base64 string representation of the screenshot, or null if capture failed</returns>
        public static string? SaveScreenRegionScreenshot(int x, int y, int width, int height, Guid eventId)
        {
            try
            {
                // Create a bitmap of the specified size
                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                // Create graphics object from the bitmap
                using (Graphics gfx = Graphics.FromImage(bmp))
                {
                    // Copy the specified screen area to the bitmap
                    gfx.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(width, height), CopyPixelOperation.SourceCopy);

                    // Draw an arrow pointing at the cursor
                    DrawArrowAtCursor(gfx, width, height, x, y);
                }

                // Convert the bitmap to a memory stream
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Png);
                    byte[] imageBytes = ms.ToArray();

                    // Convert byte array to Base64 string
                    string base64String = Convert.ToBase64String(imageBytes);

                    // Dispose of the bitmap
                    bmp.Dispose();

                    // Return the Base64 string
                    return base64String;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to capture screenshot: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Draws an arrow pointing to the current cursor position on the given graphics object
        /// </summary>
        /// <param name="gfx">Graphics object to draw on</param>
        /// <param name="width">Width of the bitmap</param>
        /// <param name="height">Height of the bitmap</param>
        /// <param name="offsetX">X offset of the bitmap</param>
        /// <param name="offsetY">Y offset of the bitmap</param>
        private static void DrawArrowAtCursor(Graphics gfx, int width, int height, int offsetX, int offsetY)
        {
            // Define the arrow properties
            Pen arrowPen = new Pen(Color.Magenta, 5);
            arrowPen.EndCap = System.Drawing.Drawing2D.LineCap.Custom;
            arrowPen.CustomEndCap = new System.Drawing.Drawing2D.AdjustableArrowCap(5, 5); // Bigger arrow head

            // Define the length of the arrow
            int arrowLength = 200;

            // Get the current cursor position
            POINT cursorPos;
            GetCursorPos(out cursorPos);

            // Convert the screen coordinates to bitmap coordinates
            int cursorX = cursorPos.X - offsetX;
            int cursorY = cursorPos.Y - offsetY;

            // Determine arrow direction: down if in top half, up if in bottom half
            int endX, endY;
            if (cursorY < height / 2)
            {
                // Cursor is in the top half, arrow points down
                endX = cursorX;
                endY = cursorY + arrowLength;
            }
            else
            {
                // Cursor is in the bottom half, arrow points up
                endX = cursorX;
                endY = cursorY - arrowLength;
            }

            // Draw the arrow
            gfx.DrawLine(arrowPen, endX, endY, cursorX, cursorY);
        }

        /// <summary>
        /// Converts a Base64 string to an Image
        /// </summary>
        /// <param name="base64String">Base64 string representation of the image</param>
        /// <returns>Image object</returns>
        public static Image Base64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                ms.Write(imageBytes, 0, imageBytes.Length);
                return Image.FromStream(ms, true);
            }
        }

        /// <summary>
        /// Converts an Image to a Base64 string
        /// </summary>
        /// <param name="image">Image to convert</param>
        /// <param name="format">Image format to use</param>
        /// <returns>Base64 string representation of the image</returns>
        public static string ImageToBase64(Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                return Convert.ToBase64String(imageBytes);
            }
        }
    }
}