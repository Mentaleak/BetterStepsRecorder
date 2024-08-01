using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Better_Steps_Recorder
{
    internal class PictureBoxTools
    {
        private bool isDrawing;
        private Rectangle blurRectangle;
        private Point startPoint;

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            isDrawing = true;
            startPoint = e.Location;
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                Point endPoint = e.Location;
                blurRectangle = new Rectangle(
                    Math.Min(startPoint.X, endPoint.X),
                    Math.Min(startPoint.Y, endPoint.Y),
                    Math.Abs(startPoint.X - endPoint.X),
                    Math.Abs(startPoint.Y - endPoint.Y));

                pictureBox.Invalidate(); // Refresh the PictureBox to draw the selection rectangle
            }
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                isDrawing = false;
                ApplyBlur(blurRectangle);
            }
        }

        private void ApplyBlur(Rectangle rect)
        {
            if (pictureBox.Image == null)
                return;

            Bitmap originalBitmap = new Bitmap(pictureBox.Image);
            Bitmap blurredBitmap = new Bitmap(originalBitmap);

            // Simple box blur
            int blurSize = 10;
            for (int x = rect.X; x < rect.Right; x += blurSize)
            {
                for (int y = rect.Y; y < rect.Bottom; y += blurSize)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int blurPixelCount = 0;

                    // Average color in the blur region
                    for (int xx = x; xx < x + blurSize && xx < originalBitmap.Width; xx++)
                    {
                        for (int yy = y; yy < y + blurSize && yy < originalBitmap.Height; yy++)
                        {
                            Color pixelColor = originalBitmap.GetPixel(xx, yy);
                            avgR += pixelColor.R;
                            avgG += pixelColor.G;
                            avgB += pixelColor.B;
                            blurPixelCount++;
                        }
                    }

                    // Calculate the average color
                    avgR /= blurPixelCount;
                    avgG /= blurPixelCount;
                    avgB /= blurPixelCount;

                    // Set the color of the blur region
                    for (int xx = x; xx < x + blurSize && xx < originalBitmap.Width; xx++)
                    {
                        for (int yy = y; yy < y + blurSize && yy < originalBitmap.Height; yy++)
                        {
                            blurredBitmap.SetPixel(xx, yy, Color.FromArgb(avgR, avgG, avgB));
                        }
                    }
                }
            }

            // Update PictureBox with blurred image
            pictureBox.Image = blurredBitmap;
        }

    }
}
