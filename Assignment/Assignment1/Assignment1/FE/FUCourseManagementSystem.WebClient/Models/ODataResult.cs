using System.Text.Json;
using System.Text.Json.Serialization;

namespace FUCourseManagementSystem.WebClient.Models;

public class ODataResult<T>
{
    public List<T>? Value { get; set; }
}

public class ODataResultConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert.IsGenericType &&
           typeToConvert.GetGenericTypeDefinition() == typeof(ODataResult<>);

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var elementType = typeToConvert.GetGenericArguments()[0];
        return (JsonConverter)Activator.CreateInstance(
            typeof(ODataResultConverter<>).MakeGenericType(elementType))!;
    }
}

internal class ODataResultConverter<T> : JsonConverter<ODataResult<T>>
{
    public override ODataResult<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Plain array: [...]
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            var list = JsonSerializer.Deserialize<List<T>>(ref reader, options);
            return new ODataResult<T> { Value = list };
        }

        // OData object: {"value": [...], "@odata.context": "..."}
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            List<T>? value = null;
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propName = reader.GetString();
                    reader.Read();
                    if (string.Equals(propName, "value", StringComparison.OrdinalIgnoreCase))
                        value = JsonSerializer.Deserialize<List<T>>(ref reader, options);
                    else
                        reader.Skip();
                }
            }
            return new ODataResult<T> { Value = value };
        }

        return null;
    }

    public override void Write(Utf8JsonWriter writer, ODataResult<T> value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value.Value, options);
}
