using System.Text.Json;
using System.Text.Json.Serialization;
public class ActAsRequest
{
    public string ActAs { get; set; }

    /// <summary>
    /// Serialize this object into a string
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
