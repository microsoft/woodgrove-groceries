
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

public class VerifyCodeRequest
{
    public required string VerificationCode { get; set; }

    /// <summary>
    /// Serialize this object into a string
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}

