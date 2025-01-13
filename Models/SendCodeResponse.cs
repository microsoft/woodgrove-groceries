
using System.Text.Json;
using System.Text.Json.Serialization;

public class SendCodeResponse
{
    public SendCodeResponse()
    {

    }

    public SendCodeResponse(string error)
    {
        this.Error = error;
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Error { get; set; }

    /// <summary>
    /// Deserialize a JSON string into Status object
    /// </summary>
    /// <param name="JsonString">The JSON string to be loaded</param>
    /// <returns></returns>
    public static SendCodeResponse Parse(string JsonString)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<SendCodeResponse>(JsonString, options);
    }
}

