using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using BetterStepsRecorder.Core.ImageOperations;

namespace BetterStepsRecorder.Core
{
    /// <summary>
    /// Manages the list of image operations for a RecordEvent
    /// </summary>
    public class ImageOperationsManager
    {
        private readonly List<ImageOperation> _operations = new List<ImageOperation>();

        /// <summary>
        /// Gets the read-only list of operations
        /// </summary>
        public IReadOnlyList<ImageOperation> Operations => _operations.AsReadOnly();

        /// <summary>
        /// Adds an operation to the list
        /// </summary>
        public void AddOperation(ImageOperation operation)
        {
            _operations.Add(operation);
        }

        /// <summary>
        /// Removes an operation at the specified index
        /// </summary>
        public void RemoveOperationAt(int index)
        {
            if (index >= 0 && index < _operations.Count)
            {
                _operations.RemoveAt(index);
            }
        }

        /// <summary>
        /// Removes an operation by its ID
        /// </summary>
        public bool RemoveOperation(Guid operationId)
        {
            var op = _operations.FirstOrDefault(o => o.Id == operationId);
            if (op != null)
            {
                _operations.Remove(op);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Moves an operation from one index to another
        /// </summary>
        public void MoveOperation(int fromIndex, int toIndex)
        {
            if (fromIndex >= 0 && fromIndex < _operations.Count &&
                toIndex >= 0 && toIndex < _operations.Count &&
                fromIndex != toIndex)
            {
                var operation = _operations[fromIndex];
                _operations.RemoveAt(fromIndex);
                _operations.Insert(toIndex, operation);
            }
        }

        /// <summary>
        /// Swaps two operations
        /// </summary>
        public void SwapOperations(int index1, int index2)
        {
            if (index1 >= 0 && index1 < _operations.Count &&
                index2 >= 0 && index2 < _operations.Count &&
                index1 != index2)
            {
                var temp = _operations[index1];
                _operations[index1] = _operations[index2];
                _operations[index2] = temp;
            }
        }

        /// <summary>
        /// Clears all operations
        /// </summary>
        public void Clear()
        {
            _operations.Clear();
        }

        /// <summary>
        /// Gets the count of operations
        /// </summary>
        public int Count => _operations.Count;

        /// <summary>
        /// Applies all operations to a base image and returns the result
        /// </summary>
        /// <param name="baseImage">The base image to apply operations to</param>
        /// <returns>A new bitmap with all operations applied</returns>
        public Bitmap ApplyOperationsToImage(Bitmap baseImage)
        {
            if (baseImage == null)
                throw new ArgumentNullException(nameof(baseImage));

            Bitmap result = new Bitmap(baseImage);

            try
            {
                foreach (var operation in _operations)
                {
                    if (operation is CropOperation cropOp)
                    {
                        // Handle crop specially as it changes dimensions
                        // Clamp crop region to current bitmap bounds to prevent invalid input errors
                        var region = Rectangle.Intersect(cropOp.Region, new Rectangle(0, 0, result.Width, result.Height));
                        if (region.Width <= 0 || region.Height <= 0)
                            continue; // Skip invalid crop

                        var cropped = result.Clone(region, result.PixelFormat);
                        result.Dispose();
                        result = cropped;
                    }
                    else
                    {
                        operation.Apply(result);
                    }
                }

                return result;
            }
            catch
            {
                result.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Applies operations up to a specific index and returns the result
        /// </summary>
        /// <param name="baseImage">The base image to apply operations to</param>
        /// <param name="upToIndex">The index to apply up to (inclusive)</param>
        /// <returns>A new bitmap with operations applied up to the specified index</returns>
        public Bitmap ApplyOperationsToImage(Bitmap baseImage, int upToIndex)
        {
            if (baseImage == null)
                throw new ArgumentNullException(nameof(baseImage));

            if (upToIndex < 0)
                return new Bitmap(baseImage);

            Bitmap result = new Bitmap(baseImage);

            try
            {
                int endIndex = Math.Min(upToIndex, _operations.Count - 1);
                for (int i = 0; i <= endIndex; i++)
                {
                    var operation = _operations[i];
                    if (operation is CropOperation cropOp)
                    {
                        // Clamp crop region to current bitmap bounds to prevent invalid input errors
                        var region = Rectangle.Intersect(cropOp.Region, new Rectangle(0, 0, result.Width, result.Height));
                        if (region.Width <= 0 || region.Height <= 0)
                            continue; // Skip invalid crop

                        var cropped = result.Clone(region, result.PixelFormat);
                        result.Dispose();
                        result = cropped;
                    }
                    else
                    {
                        operation.Apply(result);
                    }
                }

                return result;
            }
            catch
            {
                result.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Creates a deep copy of this manager
        /// </summary>
        public ImageOperationsManager Clone()
        {
            var clone = new ImageOperationsManager();
            foreach (var operation in _operations)
            {
                clone.AddOperation(operation.Clone());
            }
            return clone;
        }
    }
}
