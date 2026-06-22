using System.Text.Json.Serialization;

namespace FUNewsManagement.Web.Models.Dtos;

public class ODataResponse<T>
{
    [JsonPropertyName("value")]
    public List<T> Value { get; set; } = new();
}
