using System.Text.Json.Serialization;

public class DemoData
{
    /// <summary>
    /// Unique ID of the demo data item.
    /// This ID is used to identify the demo in the application.
    /// </summary>
    public required string ID { get; set; }

    /// <summary>
    /// The demonstration list includes certain items that are non-clickable.
    /// For example, the search box and the headers.
    /// </summary>
    public bool IsSpecialListItem { get; set; } = false;

    /// <summary>
    /// Some of the demos involved two steps. For example, in the "step up" demo, a user first sign-in and only later they complete the MFA. 
    /// In this case, two records are needed. 
    ///   - The first one will be displayed on the list of demonstrations.
    ///   - While the second record is not displayed and is executed only on the server side.
    /// </summary>
    public bool ServerSideOnly { get; set; } = false;

    /// <summary>
    /// Indicates whether the demo should be hidden from the demo list.
    /// This is useful for demos that are not ready for public use or are intended for internal testing only.
    /// </summary>
    public bool IsHidden { get; set; } = false;

    /// <summary>
    /// The demo title, shown in the demo list and on the demo details card.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// In certain instances, the title displayed on the details card may differ from the title listed in the demos. 
    /// Use this attribute to provide an alternative title.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CardTitle { get; set; }

    /// <summary>
    /// The Bootstrap icon to be displayed in the demo list.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Icon { get; set; }

    /// <summary>
    /// The content of the demo details card.
    /// This content is displayed when the user clicks on the demo in the list.
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// The URL triggered when the user clicks "start the use case" button.
    /// This URL is used to start the demo and usually points to the identity controller.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ActionUrl { get; set; }

    /// <summary>
    /// Alternative text to display on the 'start the use case' button".
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ActionText { get; set; }

    /// <summary>
    /// Some demos involve a two-step process. First, the user needs to sign in, followed by another action, such as signing in with MFA. 
    /// This attribute specifies the URL to be executed when a logged-in user clicks on the button.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AuthorizedActionUrl { get; set; }

    /// <summary>
    /// Alternative text to display on the 'start the use case' button for authenticated users".
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? AuthorizedActionText { get; set; }

    /// <summary>
    /// The URL to the help page for this demo.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ConfigHelpUrl { get; set; }

    /// <summary>
    /// Upon successful sign-in, by default users are directed to the application's home page.
    /// However, there may be instances where it's preferable to redirect them to a specific page. 
    /// For example, if a user attempts to access the profile page but needs to sign in first, they should be redirected back to the profile page after completing the sign-in process.
    /// </summary>
    [JsonIgnore]
    public string? PostSignInRedirectUri { get; set; }

    /// <summary>
    /// Certain demonstrations will redirect the user to another application. 
    /// For instance, to illustrate the single sign-on functionality, the user will be redirected to the Woodgrove bank application. 
    /// When this attribute is specified, the application will stop processing the authentication request and redirect the user to the URL indicated in this attribute.
    /// 
    /// Note, the ActionUrl pointing to the identity controller will always be called.
    /// Then the identity controller will redirect the user to the URL specified in this attribute.
    /// This is done to ensure that the telemetry is tracked correctly.
    /// </summary>
    [JsonIgnore]
    public string? RedirectUri { get; set; }

    /// <summary>
    /// In most demonstrations, the SSO session should be invalidated, requiring the user to sign in even if they have already signed in. 
    /// However, there are instances where users can continue with an existing session. 
    /// For example, when editing a profile, if the user has already signed in, there is no need to prompt them to sign in again. 
    /// Since the objective is to showcase the “edit profile” functionality, rather than the sign-in process.
    /// </summary>
    [JsonIgnore]
    public bool Reauth { get; set; } = true;

    /// <summary>
    /// Collection of additional parameters that may be required for the demo.
    /// These parameters can be used to pass extra information needed during the demo execution.
    /// </summary>
    [JsonIgnore]
    public List<DemoDataParam>? ExtraParams { get; set; }

}




