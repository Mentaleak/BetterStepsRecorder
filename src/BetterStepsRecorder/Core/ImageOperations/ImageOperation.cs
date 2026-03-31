using System;
using System.Drawing;

namespace BetterStepsRecorder.Core.ImageOperations
{
    /// <summary>
    /// Base class for all image editing operations
    /// </summary>
    [Serializable]
    public abstract class ImageOperation
    {
        /// <summary>
        /// Unique identifier for this operation
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Timestamp when the operation was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Human-readable description of the operation
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Apply this operation to the given bitmap
        /// </summary>
        /// <param name="bitmap">The bitmap to modify</param>
        public abstract void Apply(Bitmap bitmap);

        /// <summary>
        /// Creates a deep copy of this operation
        /// </summary>
        public abstract ImageOperation Clone();
    }
}
