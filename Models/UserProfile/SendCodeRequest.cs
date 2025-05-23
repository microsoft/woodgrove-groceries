
using System.Text.Json;
using System.Text.Json.Serialization;

public class SendCodeRequest
{
    public required string AuthValue { get; set; }
    public required AuthMethodType AuthType { get; set; }

    /// <summary>
    /// Serialize this object into a string
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}

