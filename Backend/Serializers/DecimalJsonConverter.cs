using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend.Serializers
{
    public class DecimalJsonConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) => decimal.Round(reader.GetDecimal(), 2, MidpointRounding.AwayFromZero);

        public override void Write(Utf8JsonWriter writer,
        decimal value,
        JsonSerializerOptions options) => writer.WriteNumberValue(decimal.Round(value, 2, MidpointRounding.AwayFromZero));
    }
}