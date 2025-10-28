using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StockTracker.CrossCutting.Converters;

public class DateTimeJsonConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateTimeFromJson = reader.GetString()!;
        var standarized = $"{dateTimeFromJson.Split("+")[0]}Z";
        var parsedDate = DateTime.TryParse(standarized, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal,
            out DateTime result);
        return result;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}