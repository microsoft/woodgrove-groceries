public class DemoDataParam
{
    /// <summary>
    /// The name of the parameter to pass in the authentication request.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// The fixed value of the parameter.
    /// </summary>
    public string? FixedValue { get; set; }

    /// <summary>
    /// The identifier of the query string parameter from which the value is to be retrieved.
    /// </summary>
    public string? QueryStringName { get; set; }
}