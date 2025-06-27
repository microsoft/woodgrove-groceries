
using System.Text.Json;
using System.Text.Json.Serialization;

public class VerifyCodeResponse
{
    public VerifyCodeResponse()
    {

    }

    public VerifyCodeResponse(string error)
    {
        this.Error = error;
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Error { get; set; }

    public bool ValidationPassed { get; set; } = false;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AuthValue { get; set; }

    public AuthMethodType AuthType { get; set; }

    /// <summary>
    /// Deserialize a JSON string into Status object
    /// </summary>
    /// <param name="JsonString">The JSON string to be loaded</param>
    /// <returns></returns>
    public static VerifyCodeResponse Parse(string JsonString)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var result = JsonSerializer.Deserialize<VerifyCodeResponse>(JsonString, options);
        if (result == null)
        {
            throw new JsonException("Failed to deserialize VerifyCodeResponse from the provided JSON string.");
        }
        return result;
    }
}

