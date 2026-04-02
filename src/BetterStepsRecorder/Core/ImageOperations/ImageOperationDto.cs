using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BetterStepsRecorder.Core.ImageOperations
{
    /// <summary>
    /// Data Transfer Object for serializing ImageOperation instances to/from JSON.
    /// Uses a discriminator pattern to handle polymorphic deserialization.
    /// </summary>
    public class ImageOperationDto
    {
        /// <summary>
        /// The type of operation (e.g., "Blur", "Highlight", "Arrow", etc.)
        /// </summary>
        public string Type { get; set; } = "";

        /// <summary>
        /// Unique identifier for this operation
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Timestamp when the operation was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Operation-specific parameters stored as JSON
        /// </summary>
        public JsonElement? Parameters { get; set; }

        /// <summary>
        /// Converts an ImageOperation to its DTO representation
        /// </summary>
        public static ImageOperationDto FromOperation(ImageOperation operation)
        {
            var dto = new ImageOperationDto
            {
                Id = operation.Id,
                CreatedAt = operation.CreatedAt
            };

            switch (operation)
            {
                case BlurOperation blur:
                    dto.Type = "Blur";
                    dto.Parameters = JsonSerializer.SerializeToElement(new
                    {
                        blur.Region.X,
                        blur.Region.Y,
                        blur.Region.Width,
                        blur.Region.Height
                    });
                    break;

                case HighlightOperation highlight:
                    dto.Type = "Highlight";
                    dto.Parameters = JsonSerializer.SerializeToElement(new
                    {
                        highlight.Region.X,
                        highlight.Region.Y,
                        highlight.Region.Width,
                        highlight.Region.Height,
                        Color = highlight.Color.ToArgb()
                    });
                    break;

                case ArrowOperation arrow:
                    dto.Type = "Arrow";
                    dto.Parameters = JsonSerializer.SerializeToElement(new
                    {
                        StartX = arrow.StartPoint.X,
                        StartY = arrow.StartPoint.Y,
                        EndX = arrow.EndPoint.X,
                        EndY = arrow.EndPoint.Y,
                        Color = arrow.Color.ToArgb(),
                        arrow.Width
                    });
                    break;

                case TextLabelOperation text:
                    dto.Type = "TextLabel";
                    dto.Parameters = JsonSerializer.SerializeToElement(new
                    {
                        text.Text,
                        text.Region.X,
                        text.Region.Y,
                        text.Region.Width,
                        text.Region.Height,
                        text.FontFamily,
                        text.FontSize,
                        InnerColor = text.InnerColor.ToArgb(),
                        OuterColor = text.OuterColor.ToArgb(),
                        text.OutlineWidth,
                        text.InitialRegionHeight
                    });
                    break;

                case CropOperation crop:
                    dto.Type = "Crop";
                    dto.Parameters = JsonSerializer.SerializeToElement(new
                    {
                        crop.Region.X,
                        crop.Region.Y,
                        crop.Region.Width,
                        crop.Region.Height
                    });
                    break;

                case ClickIndicatorOperation click:
                    dto.Type = "ClickIndicator";
                    dto.Parameters = JsonSerializer.SerializeToElement(new
                    {
                        click.CursorPosition.X,
                        click.CursorPosition.Y,
                        Color = click.IndicatorColor.ToArgb(),
                        Style = click.Style.ToString()
                    });
                    break;

                case DragIndicatorOperation drag:
                    dto.Type = "DragIndicator";
                    dto.Parameters = JsonSerializer.SerializeToElement(new
                    {
                        StartX = drag.StartPoint.X,
                        StartY = drag.StartPoint.Y,
                        EndX = drag.EndPoint.X,
                        EndY = drag.EndPoint.Y,
                        Color = drag.IndicatorColor.ToArgb()
                    });
                    break;

                default:
                    throw new NotSupportedException($"Operation type {operation.GetType().Name} is not supported for serialization.");
            }

            return dto;
        }

        /// <summary>
        /// Converts a DTO back to an ImageOperation instance
        /// </summary>
        public ImageOperation ToOperation()
        {
            if (Parameters == null)
                throw new InvalidOperationException("Parameters cannot be null");

            ImageOperation operation = Type switch
            {
                "Blur" => new BlurOperation
                {
                    Region = new Rectangle(
                        Parameters.Value.GetProperty("X").GetInt32(),
                        Parameters.Value.GetProperty("Y").GetInt32(),
                        Parameters.Value.GetProperty("Width").GetInt32(),
                        Parameters.Value.GetProperty("Height").GetInt32())
                },

                "Highlight" => new HighlightOperation
                {
                    Region = new Rectangle(
                        Parameters.Value.GetProperty("X").GetInt32(),
                        Parameters.Value.GetProperty("Y").GetInt32(),
                        Parameters.Value.GetProperty("Width").GetInt32(),
                        Parameters.Value.GetProperty("Height").GetInt32()),
                    Color = Color.FromArgb(Parameters.Value.GetProperty("Color").GetInt32())
                },

                "Arrow" => new ArrowOperation
                {
                    StartPoint = new Point(
                        Parameters.Value.GetProperty("StartX").GetInt32(),
                        Parameters.Value.GetProperty("StartY").GetInt32()),
                    EndPoint = new Point(
                        Parameters.Value.GetProperty("EndX").GetInt32(),
                        Parameters.Value.GetProperty("EndY").GetInt32()),
                    Color = Color.FromArgb(Parameters.Value.GetProperty("Color").GetInt32()),
                    Width = Parameters.Value.GetProperty("Width").GetSingle()
                },

                "TextLabel" => new TextLabelOperation
                {
                    Text = Parameters.Value.GetProperty("Text").GetString() ?? "",
                    Region = new Rectangle(
                        Parameters.Value.GetProperty("X").GetInt32(),
                        Parameters.Value.GetProperty("Y").GetInt32(),
                        Parameters.Value.GetProperty("Width").GetInt32(),
                        Parameters.Value.GetProperty("Height").GetInt32()),
                    FontFamily = Parameters.Value.GetProperty("FontFamily").GetString() ?? "Arial",
                    FontSize = Parameters.Value.GetProperty("FontSize").GetSingle(),
                    InnerColor = Parameters.Value.TryGetProperty("InnerColor", out var innerProp) 
                        ? Color.FromArgb(innerProp.GetInt32()) 
                        : (Parameters.Value.TryGetProperty("TextColor", out var textColorProp) 
                            ? Color.FromArgb(textColorProp.GetInt32()) 
                            : Color.White),
                    OuterColor = Parameters.Value.TryGetProperty("OuterColor", out var outerProp) 
                        ? Color.FromArgb(outerProp.GetInt32()) 
                        : (Parameters.Value.TryGetProperty("BackgroundColor", out var bgColorProp) 
                            ? Color.FromArgb(bgColorProp.GetInt32()) 
                            : Color.Black),
                    OutlineWidth = Parameters.Value.TryGetProperty("OutlineWidth", out var widthProp) 
                        ? widthProp.GetSingle() 
                        : 3f,
                    InitialRegionHeight = Parameters.Value.TryGetProperty("InitialRegionHeight", out var initialHeightProp)
                        ? initialHeightProp.GetInt32()
                        : Parameters.Value.GetProperty("Height").GetInt32() // Use current height as fallback for old files
                },

                "Crop" => new CropOperation
                {
                    Region = new Rectangle(
                        Parameters.Value.GetProperty("X").GetInt32(),
                        Parameters.Value.GetProperty("Y").GetInt32(),
                        Parameters.Value.GetProperty("Width").GetInt32(),
                        Parameters.Value.GetProperty("Height").GetInt32())
                },

                "ClickIndicator" => new ClickIndicatorOperation
                {
                    CursorPosition = new Point(
                        Parameters.Value.GetProperty("X").GetInt32(),
                        Parameters.Value.GetProperty("Y").GetInt32()),
                    IndicatorColor = Color.FromArgb(Parameters.Value.GetProperty("Color").GetInt32()),
                    Style = Enum.Parse<ClickIndicatorStyle>(Parameters.Value.GetProperty("Style").GetString() ?? "Arrow")
                },

                "DragIndicator" => new DragIndicatorOperation
                {
                    StartPoint = new Point(
                        Parameters.Value.GetProperty("StartX").GetInt32(),
                        Parameters.Value.GetProperty("StartY").GetInt32()),
                    EndPoint = new Point(
                        Parameters.Value.GetProperty("EndX").GetInt32(),
                        Parameters.Value.GetProperty("EndY").GetInt32()),
                    IndicatorColor = Color.FromArgb(Parameters.Value.GetProperty("Color").GetInt32())
                },

                _ => throw new NotSupportedException($"Operation type '{Type}' is not supported for deserialization.")
            };

            operation.Id = Id;
            operation.CreatedAt = CreatedAt;
            return operation;
        }

        /// <summary>
        /// Converts a list of operations to DTOs
        /// </summary>
        public static List<ImageOperationDto> FromOperations(IEnumerable<ImageOperation> operations)
        {
            var dtos = new List<ImageOperationDto>();
            foreach (var op in operations)
            {
                dtos.Add(FromOperation(op));
            }
            return dtos;
        }

        /// <summary>
        /// Converts a list of DTOs back to operations
        /// </summary>
        public static List<ImageOperation> ToOperations(IEnumerable<ImageOperationDto> dtos)
        {
            var operations = new List<ImageOperation>();
            foreach (var dto in dtos)
            {
                operations.Add(dto.ToOperation());
            }
            return operations;
        }
    }
}
