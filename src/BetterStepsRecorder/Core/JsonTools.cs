using System;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BetterStepsRecorder
{
    /// <summary>
    /// Utility class for custom JSON converters and serialization helpers.
    /// </summary>
    public static class JsonTools
    {
        /// <summary>
        /// JsonConverter for ARGB color values stored as hex strings (e.g., #FFFF00FF).
        /// Provides backward compatibility with integer format.
        /// </summary>
        public class ArgbHexConverter : JsonConverter<int>
        {
            public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    string? hex = reader.GetString();
                    if (!string.IsNullOrEmpty(hex) && hex.StartsWith("#") && hex.Length == 9)
                    {
                        return (int)Convert.ToUInt32(hex.Substring(1), 16);
                    }
                }
                else if (reader.TokenType == JsonTokenType.Number)
                {
                    // Backward compatibility: read old integer format
                    return reader.GetInt32();
                }
                return Color.Magenta.ToArgb(); // Fallback to default
            }

            public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
            {
                writer.WriteStringValue($"#{value:X8}");
            }
        }
    }
}
