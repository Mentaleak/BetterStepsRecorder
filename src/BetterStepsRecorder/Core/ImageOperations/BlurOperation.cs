using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace BetterStepsRecorder.Core.ImageOperations
{
    /// <summary>
    /// Operation that blurs a rectangular region
    /// </summary>
    [Serializable]
    public class BlurOperation : ImageOperation
    {
        public Rectangle Region { get; set; }

        public override string Description => "Blur";

        public BlurOperation() { }

        public BlurOperation(Rectangle region)
        {
            Region = region;
        }

        public override void Apply(Bitmap bitmap)
        {
            if (Region.Width <= 0 || Region.Height <= 0) return;

            // Clamp region to bitmap bounds
            var rect = Rectangle.Intersect(Region, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
            if (rect.Width <= 0 || rect.Height <= 0) return;

            BitmapData data = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            try
            {
                int pixelSize = 4;
                int stride = data.Stride;
                IntPtr scan0 = data.Scan0;
                int w = rect.Width;
                int h = rect.Height;

                byte[] pixels = new byte[stride * h];
                Marshal.Copy(scan0, pixels, 0, pixels.Length);

                // Simple box blur with pixelation effect
                int blockSize = Math.Max(8, Math.Min(w, h) / 20);
                for (int y = 0; y < h; y += blockSize)
                {
                    for (int x = 0; x < w; x += blockSize)
                    {
                        int r = 0, g = 0, b = 0, count = 0;
                        int endY = Math.Min(y + blockSize, h);
                        int endX = Math.Min(x + blockSize, w);

                        // Calculate average color for the block
                        for (int by = y; by < endY; by++)
                        {
                            for (int bx = x; bx < endX; bx++)
                            {
                                int idx = by * stride + bx * pixelSize;
                                b += pixels[idx];
                                g += pixels[idx + 1];
                                r += pixels[idx + 2];
                                count++;
                            }
                        }

                        if (count > 0)
                        {
                            byte avgB = (byte)(b / count);
                            byte avgG = (byte)(g / count);
                            byte avgR = (byte)(r / count);

                            // Apply average color to entire block
                            for (int by = y; by < endY; by++)
                            {
                                for (int bx = x; bx < endX; bx++)
                                {
                                    int idx = by * stride + bx * pixelSize;
                                    pixels[idx] = avgB;
                                    pixels[idx + 1] = avgG;
                                    pixels[idx + 2] = avgR;
                                }
                            }
                        }
                    }
                }

                Marshal.Copy(pixels, 0, scan0, pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(data);
            }
        }

        public override ImageOperation Clone()
        {
            return new BlurOperation(Region) { Id = this.Id, CreatedAt = this.CreatedAt };
        }
    }
}
