using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace AAM.Helpers.Common
{
    /// <summary>
    /// Supports converting from Base64 String into JsonByteArray, and vice versa.
    /// </summary>
    public class JsonByteArrayConverter : JsonConverter<byte[]?>
    {
        /// <summary>
        /// Converts base64 encoded string to byte[].
        /// </summary>
        /// <param name="reader">The Utf8JsonReader object.</param>
        /// <param name="typeToConvert">The target type.</param>
        /// <param name="options">JsonSerializer options object.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override byte[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TryGetBytesFromBase64(out byte[]? result))
            {
                throw new Exception("The value (" + reader.GetString() + ") couldn't be encoded successfully.");
            }
            return result;
        }

        /// <summary>
        /// Converts byte[] to base64 encoded string.
        /// </summary>
        /// <param name="writer">The Utf8JsonReader object.</param>
        /// <param name="value">The value.</param>
        /// <param name="options">JsonSerializer options object.</param>
        public override void Write(Utf8JsonWriter writer, byte[]? value, JsonSerializerOptions options)
        {
            writer.WriteBase64StringValue(value);
        }
    }
}
