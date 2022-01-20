using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend.Serializers
{
    public class DateJsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            try
            {
                return DateTime.ParseExact(reader.GetString(),
                    "MM/dd/yyyy", CultureInfo.InvariantCulture);
            }
            catch (FormatException e)
            {
                throw new JsonException(e.Message);
            }
        }

        public override void Write(
            Utf8JsonWriter writer,
            DateTime dateTimeValue,
            JsonSerializerOptions options) =>
                writer.WriteStringValue(dateTimeValue.ToString(
                   "MM/dd/yyyy", CultureInfo.InvariantCulture));

    }

}