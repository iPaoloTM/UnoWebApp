using System.Text.Json;
using System.Text.Json.Serialization;

namespace Entities;

//This is a CUSTOM CONVERTER to serialize properly the card lists
//It allows for serializing the classes that inherit from card properly,
//Saving their respective unique attributes Number and Effect
public class JsonConverterUno : JsonConverter<Card>
{
    public override Card? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var root = doc.RootElement;
            if (root.TryGetProperty("Number", out var _))
            {
                return JsonSerializer.Deserialize<NumericCard>(root.GetRawText());
            }
            else if (root.TryGetProperty("Effect", out var _))
            {
                return JsonSerializer.Deserialize<SpecialCard>(root.GetRawText());
            }
            throw new JsonException();
        }
    }
    
    public override void Write(Utf8JsonWriter writer, Card value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, (dynamic)value, options);
    }
}