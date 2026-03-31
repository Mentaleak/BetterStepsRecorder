using System;
using System.Drawing;

namespace BetterStepsRecorder.Core.ImageOperations
{
    /// <summary>
    /// Operation that crops the image to a specific region
    /// NOTE: Crop operations fundamentally change the image dimensions,
    /// so subsequent operations need to account for the new coordinate system.
    /// </summary>
    [Serializable]
    public class CropOperation : ImageOperation
    {
        public Rectangle Region { get; set; }

        public override string Description => "Crop";

        public CropOperation() { }

        public CropOperation(Rectangle region)
        {
            Region = region;
        }

        public override void Apply(Bitmap bitmap)
        {
            // Note: Crop is handled specially in the ApplyOperations method
            // because it changes the bitmap dimensions
            throw new NotSupportedException("Crop operations must be applied using the special ApplyOperationsToImage method");
        }

        public override ImageOperation Clone()
        {
            return new CropOperation(Region) { Id = this.Id, CreatedAt = this.CreatedAt };
        }
    }
}
