public static class DemoDataList
{
    private static IConfiguration _configuration;
    public static List<DemoData> Demos { get; set; }
    const string GroupHeader = "<li><hr class='dropdown-divider'></li><li class='UseCaseHeader'><h6 class='dropdown-header' style='color: var(--color-very-dark);'>{0}</h6></li>";

    // static DemoDataList()
    // {
    //     Demos = new List<DemoData>();
    // }

    public static void Initialize(IConfiguration configuration)
    {
        _configuration = configuration;
        Demos = new List<DemoData>();

        Demos.Add(new DemoData
        {
            ID = "UseCaseSearch",
            Title = "Search",
            IsSpecialListItem = true,
            Content = @"<li class='UseCaseSearch no-close' id='UseCaseSearchContainer' style='padding-bottom: 3px; padding-top: 3px;  min-width: 450px;'><div class='dropdown-item'> <i class='bi bi-search' style='color: rgb(1, 121, 1)!important; font-weight: 400!important;'></i><input type='text' class='form-control' style='margin-left: 5px; width: 80%; display: inline; border-style: none;' placeholder='Search...' id='serachUseCase' onkeyup='filterUseCases()'></div></li>"
        });

        Demos.Add(new DemoData
        {
            ID = "Default",
            Title = "Home",
            CardTitle = "Woodgrove Groceries live demo",
            Icon = "bi bi-house-door",
            Content = @"
                <p>Microsoft Entra External ID offers solutions that let you quickly add
                    intuitive, user-friendly sign-up and sign-up experiences for your customer apps.</p>
                <p>The Woodgrove Groceries live demo illustrates several of the most common
                    authentication experiences that can be configured for your consumer-facing apps.</p>

                <p class='fs-5'>
                    From the above dropdown list, select a use-case and start the
                    demo.
                </p>

                <iframe style='width: 100%; margin-top: 30px;' height='315'
                    src='https://www.youtube.com/embed/ZRhD-1WLrh8?si=FejXcmO5VwooMmCL' title='YouTube video player'
                    frameborder='0' allow='accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share' referrerpolicy='strict-origin-when-cross-origin' allowfullscreen></iframe>
                <p>
                    Watch this video to learn more about the Woodgrove live demo.
                </p>"
        });

        Demos.Add(new DemoData
        {
            ID = "OnlineRetail",
            Title = "Online retail demo",
            Icon = "bi bi-shop",
            Content = @"
                <p>
                    The online retail use case is an end-to-end demonstration that illustrates
                    several of the most common authentication
                    experiences that can be configured for your customer-facing apps.
                    To run the use case, follow these steps:
                </p>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>From the sign-in page select <b>No account? Create one.</b></li>
                    <li>Enter your email address, which will be verified and becomes your login ID.</li>
                    <li>Open your mailbox and copy the verification code sent to you. Then, on the sign-in
                        page enter the verification code and select <b>next</b>.</li>
                    <li>After the email was verified, enter a password, and re-enter the password,
                        and enter your account details.</li>
                    <li>Select <b>next</b> to complete the registration.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=OnlineRetail",
            ConfigHelpUrl = "/help/EmailAndPassword"
        });

        Demos.Add(new DemoData
        {
            ID = "SignInGroup",
            Title = "Sign-in group",
            IsSpecialListItem = true,
            Content = string.Format(GroupHeader, "Sign-in")
        });

        Demos.Add(new DemoData
        {
            ID = "EmailAndPassword",
            Title = "Sign-up or sign-in with email and password",
            Icon = "bi-chat-right-dots",
            Content = @"
                <h5>Create a new Woodgrove account</h5>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>From the sign-in page select <b>No account? Create one.</b></li>
                    <li>Enter your email address, which will be verified and becomes your login ID.</li>
                    <li>Open your mailbox and copy the verification code sent to you. Then, on the sign-in
                        page enter the verification code and select <b>next</b>.</li>
                    <li>After the email was verified, enter a password, and re-enter the password,
                        and enter your account details.</li>
                    <li>Select <b>next</b> to complete the registration.</li>
                </ol>

                <h5>Sign-in with your Woodgrove account</h5>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>On the sign-in page, enter your email, and select <b>next</b>.</li>
                    <li>Enter your password and select <b>sign in</b>.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=EmailAndPassword",
            ConfigHelpUrl = "/help/EmailAndPassword"
        });

        Demos.Add(new DemoData
        {
            ID = "EmailOtp",
            Title = "Sign-up or sign-in with email and one-time passcode",
            Icon = "bi bi-at",
            Content = @"
                <h5>Create a new Woodgrove account</h5>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>From the sign-in page select <b>No account? Create one.</b></li>
                    <li>Enter your email address, which will be verified and becomes your login ID.</li>
                    <li>Open your mailbox and copy the verification code sent to you. Then, on the sign-in page enter the verification code and select <b>next</b>.</li>
                    <li>After the email was verified, and enter your account details. Notice, that password is not required.</li>
                    <li>Select <b>next</b> to complete the registration.</li>
                </ol>

                <h5>Sign-in with your Woodgrove account</h5>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>On the sign-in page, enter your email, and select <b>next</b>.</li>
                    <li>Open your mailbox and copy the verification code sent to you. Then, on the sign-in page enter the verification code and select <b>next</b>.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=EmailOtp",
            ConfigHelpUrl = ""
        });

        Demos.Add(new DemoData
        {
            ID = "SSPR",
            Title = "Self-service password reset",
            Icon = "bi bi-key",
            Content = @"
                <p>
                    Self-service password reset (SSPR) gives users the ability to change or reset their password, with no administrator or help desk involvement. If a user's account is locked
                    or they forget their password, they can follow prompts to unblock themselves and get
                    back to work. For more information, learn <a class='link-dark' href='https://learn.microsoft.com/en-us/entra/external-id/customers/how-to-enable-password-reset-customers'>how to enable self-service password reset.</a>
                    <br>&nbsp;<br> Before you start, make sure you've created an account with Woodgrove Groceries using the <a class='link-dark link-offset-2-hover link-underline link-underline-opacity-0 link-underline-opacity-75-hover' href='#usecase=EmailAndPassword'>Sign-up or sign-in with email and password</a> flow.
                </p>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>On the sign-in page, enter your email, and select <b>next</b>.</li>
                    <li>Select the <b>Forgot password?</b> link.</li>
                    <li>Open your mailbox and copy the verification code sent to you. Then, on the sign-in page enter the verification code and select <b>next</b>.</li>
                    <li>After your email was verified, enter a password, and re-enter the password and select <b>next</b> to update your password.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=SSPR",
            ConfigHelpUrl = "/help/sspr"
        });

        Demos.Add(new DemoData
        {
            ID = "LoginHint",
            Title = "Prepopulate the sign-in name",
            Icon = "bi bi-person-check",
            Content = @"
                <p>
                    During a sign-in an application may target a specific user. When targeting a user, an application
                    can specify, in the authorization request, the
                    'login_hint' query parameter with the user sign-in name. Microsoft Entra external ID automatically
                    populates the
                    sign-in name, while the user only needs to provide the password.
                </p>
                <ol>
                    <li>Enter a username: <input type='email' id='loginHint' placeholder='someone@woodgrovedemo.com'
                        oninput=""document.getElementById('LoginHint_start').href='/SignIn?handler=LoginHint&id=' + document.getElementById('loginHint').value"">
                        Make sure you enter an account that exists in the directory.</li>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>From the sign-in page, notice that the username already entered.</li>
                    <li>Enter your password and sign-in.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=LoginHint&id=someone@woodgrovedemo.com",
            ExtraParams = new List<DemoDataParam>
            {
                new DemoDataParam
                {
                    Name = "login_hint",
                    QueryStringName = "id"
                }
            },
        });

        Demos.Add(new DemoData
        {
            ID = "DomainHint",
            Title = "Direct sign-in to an external identity provider",
            Icon = "bi bi-signpost",
            IsHidden = true,
            Content = @"
                <p>
                    The domain_hint parameter is an optional query parameter that can be added to the authorization request URL. It indicates to Microsoft Entra external ID which domain the user should use for signing in. When included, the user will bypass the Microsoft Entra external ID sign-in page and proceed directly to the external identity provider's sign-in page. This feature is currently in preview and available only for Custom OpenID Connect IDPs
                </p>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>Make sure the user skips the Microsoft Entra external ID sign-in page.</li>
                    <li>Sign in with the external identity provider.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=DomainHint",
            ExtraParams = new List<DemoDataParam>
            {
                new DemoDataParam
                {
                    Name = "domain_hint",
                    FixedValue = "login.live.com"
                }
            },
        });

        Demos.Add(new DemoData
        {
            ID = "SignInGroup",
            Title = "Sign-in group",
            IsSpecialListItem = true,
            Content = string.Format(GroupHeader, "External identity providers")
        });

        Demos.Add(new DemoData
        {
            ID = "Social",
            Title = "Facebook",
            CardTitle = "Sign-in with social accounts",
            Icon = "bi bi-facebook",
            Content = @"
                <p>
                    Users can sign in with their existing social accounts, without having to create a
                    new account. For more information, learn how to add <a class='link-dark'
                        href='https://learn.microsoft.com/entra/external-id/customers/how-to-google-federation-customers'>Google</a>
                    and <a class='link-dark'
                        href='https://learn.microsoft.com/entra/external-id/customers/how-to-facebook-federation-customers'>Facebook</a>
                    identity providers.
                </p>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>From the sign-in page, select Google. Then you will be redirected to Google sign-in page.</li>
                    <li>If asked, consent to grant the permissions that Microsoft Entra external ID is requesting.</li>
                    <li>Upon first sign-in, complete the registration by entering your account details. </li>
                    <li>Select <b>next</b> to create the Woodgrove account.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=Social",
            ConfigHelpUrl = "/help/Social"
        });

        Demos.Add(new DemoData
        {
            ID = "Google",
            Title = "Google",
            IsSpecialListItem = true,
            Content = "<li><a class='dropdown-item' href='#usecase=Social'><i class='bi bi-google'></i>&nbsp; Google</a></li>"
        });

        Demos.Add(new DemoData
        {
            ID = "MSA",
            Title = "Microsoft personal account (live.com)",
            Icon = "bi bi-xbox",
            Content = @"
                <p>
                    Users can sign in with their existing Microsoft personal account accounts (live.com), without having to create a
                    new account. For more information, learn how to  <a class='link-dark'
                        href='https://learn.microsoft.com/en-us/entra/external-id/customers/how-to-custom-oidc-federation-customers'>Set up your OpenID Connect identity provider</a>.
                </p>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>From the sign-in page, select <b>Sign-in with Microsoft personal account</b>. Then you will be redirected to <b>live.com</b> sign-in page.</li>
                    <li>If asked, consent to grant the permissions that Microsoft Entra external ID is requesting.</li>
                    <li>Upon first sign-in, complete the registration by entering your account details. </li>
                    <li>Select <b>next</b> to create the Woodgrove account.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=MSA",
            ConfigHelpUrl = "/help/MSA"
        });

        Demos.Add(new DemoData
        {
            ID = "SignUpGroup",
            Title = "Sign-up group",
            IsSpecialListItem = true,
            Content = string.Format(GroupHeader, "Sign-up")
        });

        Demos.Add(new DemoData
        {
            ID = "CustomAttributes",
            Title = "Collect user attributes during sign-up",
            Icon = "bi bi-pencil-square",
            Content = @"
                <p>
                    User attributes are values collected from the user during self-service sign-up. In the user flow settings, you can select from a set of built-in user attributes you
                    want to collect from customers. You can also create custom user attributes and add them to your sign-up user flow. For more information, learn <a
                        class='link-dark' href='https://learn.microsoft.com/en-us/entra/external-id/customers/how-to-define-custom-attributes'>how to collect user attributes during sign-up.</a>

                    <br>&nbsp;<br>
                    On the sign-up page the user enters the information, and it's stored with their
                    profile in your directory. This demo shows the use of built-in attribute and custom attribute called <b>special diet</b>. To start the demo:
                </p>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>Sign-up with your email, or a social account. For more information, <a class='link-dark link-offset-2-hover link-underline link-underline-opacity-0 link-underline-opacity-75-hover' href='#usecase=EmailAndPassword'>sign-up or sign-in with email and password</a>. If you already have an account, <a class='link-dark link-offset-2-hover link-underline link-underline-opacity-0 link-underline-opacity-75-hover' href='#usecase=DeleteAccount'>delete it</a>.</li>
                    <li>After you validate your email, or sign-in with your social account, complete the registration by providing your details. The <b>special diet</b> is a custom attribute you can provide. For the demo enter, <b>Egg allergy</b> This attribute will be included in the security token that return to the Woodgrove application.</li>
                    <li>Select <b>next</b> to create a Woodgrove online identity. </li>
                    <li>After you successfully sign-in, in the home page, the <b>Eggs</b> product will show an allergy warning.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=CustomAttributes",
            ConfigHelpUrl = "/help/CustomAttributes"
        });

        Demos.Add(new DemoData
        {
            ID = "SignUpLink",
            Title = "Sign-up link",
            Icon = "bi bi-link",
            Content = @"
                <p>
                    Microsoft Entra external ID allows applications to start the authorization request with sign-up flow
                    (using the 'prompt=create' query parameter). You can also provide an email address (using the 'login_hint' query parameter). If provided, Microsoft Entra external ID automatically
                    populates the sign-up email address, while the user only needs to validate their email address and enter their profile attributes. Make sure there is no such account in the directory.
                </p>
                <ol>
                    <li>[Optimally] Enter an email: <input type='email' id='signUpLoginHint' oninput=""document.getElementById('SignUpLink_start').href='/SignIn?handler=SignUpLink&id=' + document.getElementById('signUpLoginHint').value"">.</li>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>Notice that the title of the page is 'Create account'</li>
                    <li>If email has been provided, notice that the email address already entered.</li>
                    <li>Continue with the sign-up flow.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=SignUpLink",
            ExtraParams = new List<DemoDataParam>
            {
                new DemoDataParam
                {
                    Name = "prompt",
                    FixedValue = "create"
                },
                new DemoDataParam
                {
                    Name = "login_hint",
                    QueryStringName = "id"
                }
            },
        });



        Demos.Add(new DemoData
        {
            ID = "PolicyAgreement",
            Title = "Links to terms of use and privacy policies",
            Icon = "bi bi-file-earmark-text",
            Content = @"
                <p>
                    Terms of use, also known as terms and conditions or terms of service, are rules, specifications, and requirements for the use of your app.
                    Microsoft Entra external ID allows you to add a custom attribute (type of Boolean) to the sign-up page.
                    Before completing the sign-up, users should read and accept your policies.
                    For more information, learn how to collect user attributes during sign-up <a class='link-dark'  href='https://learn.microsoft.com/entra/external-id/customers/how-to-define-custom-attributes#to-configure-a-single-select-checkbox-checkboxsingleselect'>and configure a single-select checkbox</a>.

                    <br>&nbsp;<br>
                    This demo shows to add links to <b>terms of use and privacy policies</b>. To start the demo:
                </p>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>Sign-up with your email, or a social account. For more information, <a class='link-dark link-offset-2-hover link-underline link-underline-opacity-0 link-underline-opacity-75-hover' href='#usecase=EmailAndPassword'>sign-up or sign-in with email and password</a>. If you already have an account, <a class='link-dark link-offset-2-hover link-underline link-underline-opacity-0 link-underline-opacity-75-hover' href='#usecase=DeleteAccount'>delete it</a>.</li>
                    <li>After you validate your email, or sign-in with your social account, complete the registration by providing your details.</li>
                    <li>Select the <b>terms of use and privacy policies</b> links which will be opened in a new browser tab. Then, close the tabs and go back to the sign-up page, and select the checkbox.</li>
                    <li>Select <b>next</b> to create a Woodgrove online identity. </li>
                </ol>",
            ActionUrl = "/SignIn?handler=PolicyAgreement",
            ConfigHelpUrl = "/help/PolicyAgreement"
        });

        Demos.Add(new DemoData
        {
            ID = "PreAttributeCollection",
            Title = "Prepopulate sign-up attributes (extension)",
            Icon = "bi bi-ui-checks",
            Content = @"
                <p>
                    The custom authentication extension supports the <a class='link-dark'  href='https://learn.microsoft.com/en-us/entra/identity-platform/custom-extension-attribute-collection?context=%2Fentra%2Fexternal-id%2Fcustomers%2Fcontext%2Fcustomers-context&tabs=start-continue%2Csubmit-continue'>on attribute collection start</a> event. This
                    event occurs at the beginning of the attribute collection step, before the attribute collection page
                    renders. You can add actions such as prefilling values and displaying a blocking error.
                    This demo shows how to prepopulate some of the values, including pre selecting the country attribute
                    with spain and generating and set the value of the promo code attribute.
                </p>
                To start the demo:
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>Sign-up with your email, or a social account. For more information, <a class='link-dark link-offset-2-hover link-underline link-underline-opacity-0 link-underline-opacity-75-hover' href='#usecase=EmailAndPassword'>sign-up or sign-in with email and password</a>. If you already have an account, <a class='link-dark link-offset-2-hover link-underline link-underline-opacity-0 link-underline-opacity-75-hover' href='#usecase=DeleteAccount'>delete it</a>.</li>
                    <li>After you validate your email, or sign-in first time with your social account, you will be taken to the sign-up page.</li>
                    <li>On the sign-up page notice that the <b>Spain</b> country was selected for you. Also at the  bottom of the page you can see that the <b>promo code</b> was generated and entered for you. Both values  were provided by a custom authentication extension.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=PreAttributeCollection",
            ConfigHelpUrl = "/help/PreAttributeCollection"
        });

        Demos.Add(new DemoData
        {
            ID = "PostAttributeCollection",
            Title = "Validate sign-up attributes (extension)",
            Icon = "bi bi-hand-thumbs-up",
            Content = @"
                <p>
                    The custom authentication extension supports the <a class='link-dark' href='https://learn.microsoft.com/en-us/entra/identity-platform/custom-extension-attribute-collection?context=%2Fentra%2Fexternal-id%2Fcustomers%2Fcontext%2Fcustomers-context&tabs=start-continue%2Csubmit-continue'>on
                    attribute collection submit</a> event. This event allows you to perform validation on attributes collected from the user during sign-up.
                    This demo validates the city name against a list of cities and countries compiled in the Woodgrove custom authentication extension REST API.
                </p>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>Sign-up with your email, or a social account. If you already have an account, <a class='link-dark link-offset-2-hover link-underline link-underline-opacity-0 link-underline-opacity-75-hover'  href='#usecase=DeleteAccount'>delete it</a>.</li>
                    <li>After you validate your email, or sign-in with your social account, complete the registration by providing your details. For the country, leave the <b>Spain</b> selected, and then for the city <b>Berlin</b> (Berlin is not a city in Spain).</li>
                    <li>Select <b>next</b> to create a Woodgrove online identity. And you should get an error message that Woodgrove doesn’t operate in this city. Because Berlin is a city in Germany, not in Spain.</li>
                    <li>Corrects the city name. For example, enter <b>Madrid</b> and try to complete the registration again. </li>
                </ol>",
            ActionUrl = "/SignIn?handler=PostAttributeCollection",
            ConfigHelpUrl = "/help/PostAttributeCollection"
        });

        Demos.Add(new DemoData
        {
            ID = "ModifyAttributeValues",
            Title = "Modify sign-up's attribute values (extension)",
            Icon = "bi bi-alphabet",
            Content = @"
                <p>
                    The custom authentication extension supports the on <a class='link-dark'  href='https://learn.microsoft.com/en-us/entra/identity-platform/custom-extension-attribute-collection?context=%2Fentra%2Fexternal-id%2Fcustomers%2Fcontext%2Fcustomers-context&tabs=start-continue%2Csubmit-continue'>
                    attribute collection submit</a> event. These event allows you to modify and override attributes provided by the user. This example shows how to modify the display name and the name of the city.
                </p>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>Sign-up with your email, or a social account. If you already have an account, <a class='link-dark link-offset-2-hover link-underline link-underline-opacity-0 link-underline-opacity-75-hover' href='#usecase=DeleteAccount'>delete it</a>. </li>
                    <li>After you validate your email, or sign-in with your social account, enter a <b>display name</b> in upper case. For example, DAVID</li>
                    <li>For the <b>city</b> attribute, enter <b>modify</b>.</li>
                    <li>Select <b>next</b> to try to create a Woodgrove online identity. At this time the custom authentication extension will <b>capitalize</b> the dispaly name (only first letter in upper case). The city will be modified to <b>Madrid</b>.</li>
                    <li>Select your name from the header, it shows the content of the access token.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=ModifyAttributeValues",
            ConfigHelpUrl = "/help/PostAttributeCollection"
        });

        Demos.Add(new DemoData
        {
            ID = "BlockSignUp",
            Title = "Stop the sign-up process (extension)",
            Icon = "bi bi-sign-stop",
            Content = @"
                <p>
                    The custom authentication extension supports the on <a class='link-dark' href='https://learn.microsoft.com/en-us/entra/identity-platform/custom-extension-attribute-collection?context=%2Fentra%2Fexternal-id%2Fcustomers%2Fcontext%2Fcustomers-context&tabs=start-continue%2Csubmit-continue'>attribute collection submit</a> event. These event allows you to block the user from continuing the sign-up process.
                    For example, you could use an identity verification service or external identity data source to verify the user's email address. This demo validates uses the <i>on attribute collection <b>submit</b> </i> even to check the  value of the city attribute and block the process.
                </p>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>Sign-up with your email, or a social account. If you already have an account, <a class='link-dark link-offset-2-hover link-underline link-underline-opacity-0 link-underline-opacity-75-hover' href='#usecase=DeleteAccount'>delete it</a>. </li>
                    <li>After you validate your email, or sign-in with your social account, complete the registration by providing your details.</li>
                    <li>For the <b>city</b> attribute, enter <b>block</b>.</li>
                    <li>Select <b>next</b> to try to create a Woodgrove online identity. At this time the sign-up process will be canceled all together. This is because the custom authentication extension checks the city value. If it contains <b>block</b>, it returns the <b>show block page</b> action.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=BlockSignUp",
            ConfigHelpUrl = "/help/PostAttributeCollection"
        });

        Demos.Add(new DemoData
        {
            ID = "ArkoseFraudProtection",
            Title = "Arkose fraud protection (private preview)",
            Icon = "bi bi-hand-index",
            Content = @"
                <p>
                    Arkose Labs' <a class='link-dark' href='https://www.arkoselabs.com/solutions/new-account-fraud/'>New Account Fraud Solution</a> is designed to combat the creation of fraudulent accounts to ensure the security of your application while maintaining a seamless onboarding experience for legitimate users.
                </p>
                <p>
                    After a user has entered their profile data and the account is ready to be created, an additional page will appear presenting an interactive CAPTCHA-style puzzle. These puzzles are designed to be easily solvable by humans yet challenging for “bots” or scripts. 
                </p>
                <p>
                    The Arkose solution is integrated within Microsoft Entra external ID (private preview) and offers organizations seamless deployment, and tools to combat fake account creation effectively. 
                </p>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>Sign-up with your email. If you already have an account, <a class='link-dark link-offset-2-hover link-underline link-underline-opacity-0 link-underline-opacity-75-hover' href='#usecase=DeleteAccount'>delete it</a>. </li>
                    <li>After you validate your email, complete the registration by providing your details.</li>
                    <li>Select <b>next</b> to try to create a Woodgrove online identity.</li>
                    <li>complete the Arkose puzzle to create the account.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=ArkoseFraudProtection",
            ConfigHelpUrl = "/help/ArkoseFraudProtection"
        });

        Demos.Add(new DemoData
        {
            ID = "UserProfileGroup",
            Title = "User profile group",
            IsSpecialListItem = true,
            Content = string.Format(GroupHeader, "User profile")
        });

        Demos.Add(new DemoData
        {
            ID = "ProfileEdit",
            Title = "Edit your account",
            Icon = "bi bi-pencil",
            Content = @"
                <p>
                    Profile editing lets you manage you profile attributes, like display name, surname, given name, city, and others.
                </p>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>After you update your profile, sign-in again to refresh the security token.</li>
                </ol>",
            ActionUrl = "/Profile",
            ConfigHelpUrl = "/help/Profile"
        });

        Demos.Add(new DemoData
        {
            ID = "DeleteAccount",
            Title = "Delete your account",
            Icon = "bi bi-person-x",
            Content = @"
                <p>
                    If you would like to delete your account and personal information, visit the delete my
                    account page. You won't be able to reactivate your account. In a couple of minutes you
                    will be able to sign-up again with the same credentials.
                </p>",
            ActionUrl = "/DeleteMyAccount",
            ConfigHelpUrl = "/help/DeleteAccount"
        });

        Demos.Add(new DemoData
        {
            ID = "DisableAccount",
            Title = "Disable an account",
            Icon = "bi bi-ban",
            Content = @"
                <p>
                    Disabling an account can be a critical step for businesses in managing their security and
                    operational efficiency. When an account is disabled, it prevents unauthorized access to your
                    application. This demo allows you to disable your account. Keep in mind that you will not be able to
                    sign-in and enabled your account. Therefore, <b>use a temporary email</b> for this use case.
                </p>
                <ol>
                    <li class='auth'>You already signed-in to the Woodgrove application, so no need to sign-in again.</li>
                    <li class='auth'>Go to the <a class='link-dark link-offset-1 link-underline' href='/Profile'>Profile page</a>.</li>

                    <li class='unauth'>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li class='unauth'>Sign-up or sign-in with your email, or a social account.</li>
                    <li class='unauth'>After you sign-in, you will be taken to the profile page.</li>

                    <li>Uncheck the <b>Enable</b> checkbox and select <b>Save</b>.</li>
                    <li>Wait for a couple of seconds and try to sign-in again.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=DisableAccount",
            ConfigHelpUrl = "/help/DisableAccount",
            PostSignInRedirectUri = "/Profile",
            Reauth = false
        });

        Demos.Add(new DemoData
        {
            ID = "SecurityGroup",
            Title = "Security group",
            IsSpecialListItem = true,
            Content = string.Format(GroupHeader, "Security")
        });

        Demos.Add(new DemoData
        {
            ID = "MFA",
            Title = "Multifactor authentication (MFA)",
            Icon = "bi bi-envelope-at",
            Content = @"
                <p>
                    Microsoft Entra Conditional Access brings signals together, to make decisions, and enforce security
                    policies.

                    Multifactor authentication (MFA) protects customers identity by prompting them for a second verification method. For more information,
                    learn <a class='link-dark'  href='https://learn.microsoft.com/entra/external-id/customers/how-to-multifactor-authentication-customers'>how to add MFA</a>.
                </p>
                <p>
                    In this demo a Conditional Access policy that's targeted to all users when the sign-in risk level is medium or high, prompts for MFA.
                </p>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>Sign-up or sign-in with your email, or a social account.</li>
                    <li>As a second factor authenticate, Microsoft Entra ID will send you a verification code to your email or phone that you need to complete.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=MFA",
            ConfigHelpUrl = "/help/MFA",
            ExtraParams = new List<DemoDataParam>
            {
                new DemoDataParam
                {
                    Name = "StepUp",
                    FixedValue = "true",
                }
            }
        });

        Demos.Add(new DemoData
        {
            ID = "CA",
            Title = "Conditional access (with MFA)",
            Icon = "bi bi-shield-lock",
            Content = @"
                <p>
                    Microsoft Entra Conditional Access brings signals together, to make decisions, and enforce security policies.

                    Multifactor authentication (MFA) protects customers identity by prompting  them for a second verification method. For more information,
                    learn <a class='link-dark' href='https://learn.microsoft.com/entra/external-id/customers/how-to-multifactor-authentication-customers'>how to add MFA</a>.
                </p>
                <p>
                    In this demo a Conditional Access policy that's targeted to all users when the sign-in risk level is medium or high, prompts for MFA.
                </p>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page. Sign-up or sign-in with your account.</li>
                    <li>After you signed-in, proceed to the next step.</li>
                    <li>For this demo, download and run the <a class='link-dark' href='https://www.torproject.org/' target='_blank'>Tor Browse</a>. It's an open-source web browser that helps people use the internet anonymously. Therefore, Microsoft Entra ID may consider your sign-in request as suspicious.</li>
                    <li>In the Tor browser Navigate to <b>https://woodgrovedemo.com</b></li>
                    <li>Select the <b>sign-in</b> button.</li>
                    <li>Sign-in with the same account.</li>
                    <li>This time you should complete the MFA.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=CA",
            ConfigHelpUrl = "/help/CA"
        });

        Demos.Add(new DemoData
        {
            ID = "StepUpIntro",
            Title = "This record is the first step of the 'StepUp' demo for unauthenticated users.",
            Content = "",
            ServerSideOnly = true,
            PostSignInRedirectUri = "/#usecase=StepUp"
        });

        Demos.Add(new DemoData
        {
            ID = "StepUp",
            Title = "Step-up authentication (with MFA)",
            Icon = "bi bi-arrow-up-right-square",
            Content = @"
                <p>
                    Use the Microsoft Entra Conditional Access engine's <a class='link-dark'  href='https://learn.microsoft.com/entra/identity-platform/developer-guide-conditional-access-authentication-context'>authentication context</a> to trigger a demand for step-up authentication from within your application.
                    <br>&nbsp;<br> This demo allows customer to access the app and purchase items. However, upon risky action, for example
                    When a Woodgrove customer finishes shopping and proceeds to the checkout. If the sum of the items in the shopping cart is higher than usual it requires the customer to sign-in with a strong factor authentication.
                </p>
                <ol>
                    <li class='unauth'>Select the <b>start the use case</b> button at the bottom of this page. Sign-in with your account. Note, if you create a new account, you fulfill the MFA requirement since the email  is already verified. Therefore, make sure you sign-in with an existing account.</li>
                    <li class='unauth'>After you singed-in, select this use-case again.</li>

                    <li class='auth-with-mfa'>You fulfilled the MFA requirement. No further action required.</li>
                    <li class='auth-with-mfa'>To start the demo again, select the <b>Start the use case</b> button.</li>

                    <li class='auth-without-mfa'>Looks like you already signed-in. Good! Now let's demonstrate a step-up with MFA.</li>
                    <li class='auth-without-mfa'>In the home page, add some items to the shopping cart, until the <b>total</b> is more that 50$.</li>
                    <li class='auth-without-mfa'>Then, select the <b>checkout</b> button. If you have not signed-in with MFA, you will be asked to do so.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=StepUpIntro",
            AuthorizedActionText = "Start over",
            ConfigHelpUrl = "/help/StepUp",
            PostSignInRedirectUri = "/#cmd=StepUpCompleted",
            Reauth = false
        });

        Demos.Add(new DemoData
        {
            ID = "CSA",
            Title = "Custom security attribute based conditional access",
            Icon = "bi bi-person-lock",
            Content = @"
                <p>
                    <a href='https://learn.microsoft.com/entra/identity/conditional-access/concept-filter-for-applications' class='link-dark' target='_blank'> Application filters for Conditional Access</a> allow you to tag your application with custom
                    attributes. These custom attributes are then added to their Conditional Access policies. Filters for applications are evaluated at token issuance runtime.

                    <br>&nbsp;<br> In this demo a conditional access block access to all applications tagged as <b>BlockGuestsUsers</b>.
                </p>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>Sign-up or sign-in with your email, or a social account.</li>
                    <li>Upon successful sign-in you will get an error message that you don't have access to the app.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=CSA",
            ConfigHelpUrl = "/help/CustomSecurityAttributes",
            RedirectUri = _configuration.GetSection("Demos:CustomSecurityAttributesURL").Value
        });

        Demos.Add(new DemoData
        {
            ID = "AccessManagementGroup",
            Title = "Access management group",
            IsSpecialListItem = true,
            Content = string.Format(GroupHeader, "Access management")
        });

        Demos.Add(new DemoData
        {
            ID = "RBAC",
            Title = "Role-based access control",
            Icon = "bi bi-person-badge-fill",
            Content = @"
                <p>
                    <a class='link-dark'  href='https://learn.microsoft.com/entra/external-id/customers/how-to-use-app-roles-customers'>Role-based access control</a> is a popular mechanism to enforce authorization in
                    applications. It helps you manage who has access to your application and what they can do in the application. In this demo, you assign yourself to application roles which are automatically approved.
                </p>
                To start the demo:
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>Sign-up or sign-in with your email, or a social account.</li>
                    <li>From the Woodgrove header, select the <b>profile</b> button.</li>
                    <li>In profile page add yourself the <b>Products.Contributor</b> and <b>Orders.Manager</b> roles.</li>
                    <li>To reflect the changes in the security token, sign-in again with the same account (you won't be asked to enter the credentials).</li>
                    <li>After you sign-in, the <b>Manage products</b> and <b>Orders</b> buttons appear in the header.</li>
                    <li>Select your name from the header to show the security token. It should contain the role claims you assigned to.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=RBAC",
            ConfigHelpUrl = "/help/RBAC"
        });

        Demos.Add(new DemoData
        {
            ID = "GBAC",
            Title = "Group-based access control",
            Icon = "bi bi-people",
            Content = @"
                <p>
                    <a class='link-dark'  href='https://learn.microsoft.com/entra/external-id/customers/how-to-use-app-roles-customers'>Group-based access control</a>
                    is a popular mechanism to enforce authorization in applications. It helps you manage who has access to your application and what they can do in the application. You can also alter the UI based on the user's membership.

                    <br>&nbsp;<br>In this demo, you add yourself to the <b>Commercial Accounts</b> security group and you will get a discount for some of the products.
                </p>
                To start the demo:
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>Sign-up or sign-in with your email, or a social account.</li>
                    <li>From the Woodgrove header, select the <b>profile</b> button.</li>
                    <li>In profile page, add yourself to the <b>Commercial Accounts</b> security group and <b>update your account</b>.</li>
                    <li>To reflect the changes in the security token, sign-in again with the same account (you won't be asked to enter the credentials).</li>
                    <li>Now that you are a member of the Commercial Accounts security group, you get a discount to some of the products.</li>
                </ol>
                <p>
                  Note, if you select your name from the header, it shows the content of the access token issued by Microsoft Entra External ID that was returned to the application. It should contain the groups claims. This demo application checks the claim’s value and gives you the discounts.
                </p>",
            ActionUrl = "/SignIn?handler=GBAC",
            ConfigHelpUrl = "/help/GBAC"
        });

        Demos.Add(new DemoData
        {
            ID = "AssignmentRequired",
            Title = "Restrict app to a set of users",
            Icon = "bi bi-slash-circle",
            Content = @"
                <p>
                    Applications registered in a Microsoft Entra tenant are, by default, available to all users of the tenant who
                    authenticate successfully. You can configure your application to be <a href='https://learn.microsoft.com/entra/identity-platform/howto-restrict-your-app-to-a-set-of-users' target='_blank' class='link-dark link-offset-2'>restricted to a certain set of users or apps</a>.
                </p>
                <p>
                    In this demo only selected users (Woodgrove partners) can sign-in to the <b>Woodgrove partners portal</b>. Other users are not allowed to sign-in.
                </p>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>Sign-up or sign-in with your email, or a social account.</li>
                    <li>After you completed the sign-in, you will get an error message that you can't sign-in to the <b>Woodgrove partners portal</b> </li>
                </ol>
                <p>
                    Note, in this demo you can't assign yourself to the <b>Woodgrove partners portal</b> app. If you are interested in app assignment, check out the <a class='link-dark link-offset-2-hover link-underline link-underline-opacity-0 link-underline-opacity-75-hover' href='#usecase=RBAC'>Role based access control demo</a>
                </p>",
            ActionUrl = "/SignIn?handler=AssignmentRequired",
            RedirectUri = _configuration.GetSection("Demos:AssignmentRequiredURL").Value,
            ConfigHelpUrl = "/help/AssignmentRequired"
        });


        Demos.Add(new DemoData
        {
            ID = "ActAs",
            Title = "Act as (delegation)",
            Icon = "bi bi-person-workspace",
            Content = @"
                <p>
                    In an 'act as' or 'delegation' scenario, a signed-in user (the <b>delegate</b>) acts on behalf of  another user
                    (the <b>principal</b>). For instance, in a corporate context, an executive assistant
                    (the agent) may need to approve expenses on behalf of the chief financial officer (the principal).
                    Another example is helpdesk personnel (the agent) performing actions on behalf of a customer (the principal).
                </p>
                <p>
                    In these cases, the <b>agent</b> is provided with a security token that permits them to act as the <b>principal</b>. To obtain this token, the principal must first approve it. Upon receiving
                    approval, the agent may request a new security token that includes the <b>act_as</b> claim with the value specifying the name or ID of the principal (the chief financial officer or customer).
                </p>
                <p>
                    The application uses the <b>act_as</b> claim to operate on behalf of the principal. To start the demo:
                </p>
                <ol>
                    <li class='auth'>Enter a name: <input type='text' id='ActAs' placeholder='Dave' maxlength='20' oninput=""document.getElementById('ActAs_start').href='/SignIn?handler=ActAs&id=' + document.getElementById('ActAs').value""><br>  Make sure you enter an account that exists in the directory.</li>
                    <li class='auth'>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li class='auth'>After you sign-in, you will be taken to the <b>token</b> page.</li>
                    <li class='auth'>In the token page, check the 'delegation' alert.</li>

                    <li class='unauth'>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li class='unauth'>Sign-up or sign-in with your email, or a social account.</li>
                    <li class='unauth'>After you sign-in, run this use case again.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=default",
            AuthorizedActionUrl = "/SignIn?handler=ActAs&id=Dave",
            PostSignInRedirectUri = "/Token",
            Reauth = false,
            ConfigHelpUrl = ""
        });

        Demos.Add(new DemoData
        {
            ID = "UserExperienceGroup",
            Title = "User experience group",
            IsSpecialListItem = true,
            Content = string.Format(GroupHeader, "User experience")
        });

        Demos.Add(new DemoData
        {
            ID = "CompanyBranding",
            Title = "Customize the look and feel",
            Icon = "bi bi-palette",
            Content = @"
                <p>
                    You can create a custom look and feel for users signing in to your apps. With these settings, you can add your own background images, colors, company logos, and text to
                    customize the sign-in experiences across your apps. So that the sign-in page blends seamlessly into Woodgrove applications’ look and feel.
                    For more information, learn <a class='link-dark' href='https://learn.microsoft.com/entra/external-id/customers/how-to-customize-branding-customers'>how to customize the neutral branding in your customer tenant</a>. <br>
                </p>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>On the sign-in page take a look on the header, the header logo, the banner logo, the title, buttons, and the background image which are all customized.</li>
                    <li>The sign-in text appears with some guidance for the users</li>
                    <li>The footer contains links to the term of use and privacy policies. Both the links and the text can be customized</li>
                    <li>Every text on the screen can be localized.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=CompanyBranding",
            ConfigHelpUrl = "/help/CompanyBranding"
        });


        Demos.Add(new DemoData
        {
            ID = "CustomDomain",
            Title = "Custom URL domain",
            Icon = "bi bi-globe2",
            Content = @"
                <p>
                    The <a class='link-dark' href='https://learn.microsoft.com/en-us/entra/external-id/customers/concept-custom-url-domain'>custom URL domain</a> provides a more seamless user experience. From the user's perspective, they
                    remain in your domain during  the sign in process rather than redirecting to the Microsoft Entra external ID default domain
                    <b>{tenant-name}</b>.ciamlogin.com. Note, this feature is currently limited to sign-in with local accounts.  Social accounts such as Google or Facebook are not yet supported. <br>
                </p>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>Take a look on the URL in the web browser address bar. It should be <b id='customDomain'></b></li>
                    <li>You can sign-up or sign-in with local account. Do NOT use the Facebook or Google options.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=CustomDomain",
            ExtraParams = new List<DemoDataParam>
            {
                new DemoDataParam
                {
                    Name = "domain",
                    FixedValue = _configuration.GetSection("Demos:CustomDomain").Value,
                }
            },
        });


        Demos.Add(new DemoData
        {
            ID = "CustomEmail",
            Title = "Custom email",
            Icon = "bi bi-envelope-at",
            Content = @"
                <p>
                    The <a class='link-dark' href='https://learn.microsoft.com/en-us/entra/identity-platform/custom-extension-email-otp-get-started?toc=%2Fentra%2Fexternal-id%2Ftoc.json&bc=%2Fentra%2Fexternal-id%2Fbreadcrumb%2Ftoc.json&tabs=azure-communication-services%2Cazure-portal'>custom email</a> allows you to send customized emails to users who sign up, reset their password,  sign-in with email and one-time passcode, or email multifactor authentication (MFA). <br>
                </p>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>From the sign-in page select <b>No account? Create one.</b></li>
                    <li>Enter your email address, which will be verified and becomes your login ID.</li>
                    <li>Check your mailbox and review the email. Pay attention to the sender, the subject line, and the content of the email, they are all customized to align with Woodgrove branding.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=CustomEmail",
            ConfigHelpUrl = ""
        });


        Demos.Add(new DemoData
        {
            ID = "Language",
            Title = "Customize the language",
            Icon = "bi bi-translate",
            Content = @"
                <p>
                    You can create a personalized sign-in experience for users who sign in using a specific browser language by customizing the branding elements for that browser language. This customization
                    overrides any configurations made to the <b>default</b> branding. For more information, learn <a class='link-dark'  href='https://learn.microsoft.com/entra/external-id/customers/how-to-customize-branding-customers#to-customize-user-attributes'>how to customize the language of the authentication experience</a>.
                </p>
                <ol>
                    <li>Make sure your browser is configure to any language other than German</li>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>On the sign-in page take a look on the login text that appears <a class='link-dark' href='images/help/login-text.png' target='_blank'>under the login button</a>.</li>
                    <li>The sign-in text appears with some guidance for the users in English. It will remain in English  for other languages as well.</li>
                    <li>Now, change your browser settings to German and refresh the page</li>
                    <li>The sign-in text now should appear in German. This is because we configured a special branding for the German language</li>
                </ol>",
            ActionUrl = "/SignIn?handler=Language",
            ConfigHelpUrl = "/help/CompanyBranding"
        });


        Demos.Add(new DemoData
        {
            ID = "PreSelectLanguage",
            Title = "Preselect a language",
            Icon = "bi bi-translate",
            Content = @"
                <p>
                    Duing the sign-up or sign-in flow, the user's language is dictated by their browser's settings.
                    Application can pass the <b>ui_locales</b> and <b>mkt</b> parameters with a specific language.
                </p>

                <br>
                Select one a language and start the demo:<br>

                <select id='preSelectLanguages' onchange=""document.getElementById('PreSelectLanguage_start').href='/SignIn?handler=PreSelectLanguage&ui_locales=' + document.getElementById('preSelectLanguages').options[document.getElementById('preSelectLanguages').selectedIndex].value"">
                    <option value='ar-SA'>Arabic (Saudi Arabia)</option>
                    <option value='eu-ES'>Basque (Basque)</option>
                    <option value='bg-BG'>Bulgarian (Bulgaria)</option>
                    <option value='ca-ES'>Catalan (Catalan)</option>
                    <option value='zh-CN'>Chinese (China)</option>
                    <option value='zh-HK'>Chinese (Hong Kong SAR)</option>
                    <option value='hr-HR'>Croatian (Croatia)</option>
                    <option value='cs-CZ'>Czech (Czechia)</option>
                    <option value='da-DK'>Danish (Denmark)</option>
                    <option value='nl-NL'>Dutch (Netherlands)</option>
                    <option value='en-US'>English (United States)</option>
                    <option value='et-EE'>Estonian (Estonia)</option>
                    <option value='fi-FI'>Finnish (Finland)</option>
                    <option value='fr-FR'>French (France)</option>
                    <option value='gl-ES'>Galician (Galician)</option>
                    <option value='de-DE'>German (Germany)</option>
                    <option value='el-GR'>Greek (Greece)</option>
                    <option value='he-IL'>Hebrew (Israel)</option>
                    <option value='hu-HU'>Hungarian (Hungary)</option>
                    <option value='it-IT'>Italian (Italy)</option>
                    <option value='ja-JP'>Japanese (Japan)</option>
                    <option value='kk-KZ'>Kazakh (Kazakhstan)</option>
                    <option value='ko-KR'>Korean (Korea)</option>
                    <option value='lv-LV'>Latvian (Latvia)</option>
                    <option value='lt-LT'>Lithuanian (Lithuania</option>
                    <option value='nb-NO'>Norwegian Bokmål (Norway)</option>
                    <option value='pl-PL'>Polish (Poland)</option>
                    <option value='pt-BR'>Portuguese (Brazil)</option>
                    <option value='pt-PT'>Portuguese (Portugal)</option>
                    <option value='ro-RO'>Romanian (Romania)</option>
                    <option value='ru-RU'>Russian (Russia)</option>
                    <option value='sr-Latn-RS'>Serbian (Latin, Serbia)</option>
                    <option value='sk-SK'>Slovak (Slovakia)</option>
                    <option value='sl-Sl'>Slovenian (Sierra Leone)</option>
                    <option value='es-ES'>Spanish (Spain)</option>
                    <option value='sv-SE'>Swedish (Sweden)</option>
                    <option value='th-TH'>Thai (Thailand)</option>
                    <option value='tr-TR'>Turkish (Turkey)</option>
                    <option value='uk-UA'>Ukrainian (Ukraine)</option>
                </select>
                <br>&nbsp;<br>",
            ActionUrl = "/SignIn?handler=PreSelectLanguage&ui_locales=ar-SA",
            ExtraParams = new List<DemoDataParam>
            {
                new DemoDataParam
                {
                    Name = "ui_locales",
                    QueryStringName = "ui_locales"
                }
            },
        });

        Demos.Add(new DemoData
        {
            ID = "TokenAndSessionGroup",
            Title = "Token and session group",
            IsSpecialListItem = true,
            Content = string.Format(GroupHeader, "Token and session")
        });

        Demos.Add(new DemoData
        {
            ID = "TokenClaims",
            Title = "Add directory claims",
            CardTitle = "Add directory claims to security tokens",
            Icon = "bi bi-file-text",
            Content = @"
                <p>
                    When users authenticate to your application with Microsoft Entra External ID, a security token is return to your application. The security token contains claims that are statements about the user, such as name, unique identifier, or application roles.
                    Beyond the default set of claims that are contained in the security token you can <a class='link-dark'  href='https://learn.microsoft.com/en-us/entra/external-id/customers/how-to-add-attributes-to-token'>add  more claims</a>.
                </p>
                <p>
                    This demo shows how to add additional attributes to the access and ID tokens.
                </p>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>Sign-up your email, or a social account.</li>
                    <li>After you validate your email, or sign-in with your social account, complete the registration by providing your details. The <b>special diet</b> is a custom attribute you can provide. For the demo enter, <b>Egg allergy</b> This attribute will be included in the security token that return to the Woodgrove application. </li>
                    <li>From the Woodgrove header, select your name, which will take you to the <b>security token page</b>. The security token page contains the claims that return by Microsoft Entra External ID. Look for the <b>special diet</b> claim</li>
                </ol>",
            ActionUrl = "/SignIn?handler=TokenClaims",
            ConfigHelpUrl = ""
        });


        Demos.Add(new DemoData
        {
            ID = "TokenAugmentation",
            Title = "Add claims from external systems (extension)",
            Icon = "bi bi-database-add",
            Content = @"
                <p>
                    When users authenticate to your application with Microsoft Entra External ID, a security token is return to your application. The security token contains claims that are statements about the user, such as name, unique identifier, or application roles.
                    <br>&nbsp;<br> Beyond the default set of claims that are contained in the security token you can add custom claims
                    from external systems using a REST API you develop. For more information, learn <a class='link-dark' href='https://learn.microsoft.com/en-us/entra/identity-platform/custom-extension-get-started?context=%2Fentra%2Fexternal-id%2Fcustomers%2Fcontext%2Fcustomers-context&tabs=entra-admin-center%2Chttp'>  how to configure a custom claim provider token issuance event</a>.
                </p>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>Sign-up or sign-in with your email, or a social account.</li>
                    <li>From the Woodgrove header, select your name, which will take you to the <b>security token page</b>.</li>
                    <li>The security token page contains the claims that return by Microsoft Entra External ID. Locate the loyaltyNumber, loyaltySince, and loyaltyTier claims and check their value. These claims were returned by a custom authentication extension REST API with some random values.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=TokenAugmentation",
            ConfigHelpUrl = "/help/TokenAugmentation"
        });

        Demos.Add(new DemoData
        {
            ID = "SSO1",
            Title = "This flow starts the SSO demo for unauthenticated users.",
            Content = "",
            ServerSideOnly = true
        });

        Demos.Add(new DemoData
        {
            ID = "SSO2",
            Title = "This flow is the second step the SSO demo for authenticated users.",
            Content = "",
            ServerSideOnly = true,
            RedirectUri = _configuration.GetSection("Demos:WoodgroveBankURL").Value + "/Auth/Login",
        });

        Demos.Add(new DemoData
        {
            ID = "SSO",
            Title = "Single sign-on (SSO)",
            Icon = "bi bi-arrow-left-right",
            Content = @"
                     
                <p>
                    Single sign-on (SSO) adds security and convenience when users sign-in across multiple applications in Microsoft Entra ID.
                    With single sign-on, users sign-in once with a single account and get access to multiple applications.
                    When the user initially signs-in to an application, Microsoft Entra ID initiates a single sign-on session.
                    Upon subsequent authentication requests, Microsoft Entra ID validates the session, and issues a security token without prompting the user to sign in again.
                </p>

                <ol class='auth'>
                    <li>You already signed-in to the Woodgrove application, so no need to sign-in again.</li>
                    <li>Instead, select the <b>start the use case</b> button at the bottom of this page which will take you to the Woodgrove Bank web application.</li>
                    <li>In the Woodgrove bank application, select the sign-in button.</li>
                    <li>Notice that you don't need to enter your credentials, and you will be authenticated without prompting to sign in again.</li>
                </ol>

                <div class='unauth'>Follow these steps to check out the SSO feature:</div>
                <ol class='unauth'>
                    <li>Start by signing-in to this application. You may need use the <a class='link-dark' href='https://support.microsoft.com/microsoft-edge/browse-inprivate-in-microsoft-edge-cd2c9a48-0bc4-b98e-5e46-ac40c84e27e2'>InPrivate mode in Microsoft Edge</a>.</li>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>After you sign-in, come back to this dialog page and follow the instructions.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=sso1",
            AuthorizedActionUrl = "/SignIn?handler=sso2",
            ConfigHelpUrl = ""
        });


        Demos.Add(new DemoData
        {
            ID = "TokenTTL",
            Title = "Token lifetime",
            Icon = "bi bi-stopwatch",
            Content = @"
                <p>
                    You can specify the lifetime of an access token, ID token, or SAML token issued by the Microsoft
                    Entra ID. You can set token lifetimes for all apps in your tenant, or for service principals. You cannot set token lifetime policies for refresh tokens and session tokens.
                </p>
                <ol>

                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>Sign-up or sign-in with your email, or a social account.</li>
                    <li>Select your name from the header to show the security token. Scroll down to check your token expiration.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=TokenTTL",
            ConfigHelpUrl = "/help/TokenLifeTime"
        });


        Demos.Add(new DemoData
        {
            ID = "ForceSignIn",
            Title = "Force sign-in",
            Icon = "bi bi-person-lock",
            Content = @"
                <p>
                    Single sign-on (SSO) adds security and convenience when users sign-in across multiple applications
                    in Microsoft Entra ID. With single sign-on, users sign-in once with a single account and get access to multiple
                    applications. When the user initially signs-in to an application, Microsoft Entra ID initiates a single sign-on
                    session. Upon subsequent authentication requests, Microsoft Entra ID validates the session, and issues a security token without prompting the user to sign in again.
                    <br>&nbsp;<br> You can force the user to enter their credentials on a sign-in request, negating single-sign on session.

                    To do so, select the <b>start the use case</b> button at the bottom of this page.
                </p>",
            ActionUrl = "/SignIn?handler=ForceSignIn",
            ConfigHelpUrl = "/help/ForceSignIn"
        });


        Demos.Add(new DemoData
        {
            ID = "OBO",
            Title = "OAuth 2.0 On-Behalf-Of flow",
            Icon = "bi bi-arrow-90deg-up",
            Content = @"
                <p>
                    The on-behalf-of (OBO) flow describes the scenario of a web API using an identity other than its own to call another downstream web API. For the middle-tier web API to make authenticated requests to
                    the downstream web API it needs a different audience and another set of scopes (permissions). For more information, <a class='link-dark' href='https://learn.microsoft.com/entra/identity-platform/v2-oauth2-on-behalf-of-flow'>Microsoft identity platform and OAuth 2.0 On-Behalf-Of flow</a>
                    <br>&nbsp;<br> This demo shows how the <a class='link-dark' href='https://github.com/microsoft/woodgrove-groceries-api'>Account web API</a> makes authenticated requests to a downstream <b>Payment web API</b>.
                    To call the Payment web API, the Account web API acquires an access token for the Payment web API (audience or aud claim) and another set of scopes (permissions) that require by the Payment web API.
                </p>
                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>Sign-up or sign-in with your email, or a social account.</li>
                    <li>After you sign-in you will be redirected to the <b>token</b> page. You can also select your name from the header.</li>
                    <li>Then, select the <b> Access token to call a web API</b> button.</li>
                    <li>It shows you two links to the https://jwt.ms app with the corresponding access tokens. The first access token is the one returned to the Woodgrove demo application and used to call the first web API (Account). The second one shows the access token the Account web API acquires to call the Payment web API.</li>
                    <li>Compare the access tokens to understand the on-behalf-of flow.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=OBO",
            ConfigHelpUrl = "/help/OBO"
        });

        Demos.Add(new DemoData
        {
            ID = "ReportsAndAuditingGroup",
            Title = "Reports and auditing group",
            IsSpecialListItem = true,
            Content = string.Format(GroupHeader, "Reports and auditing")
        });

        Demos.Add(new DemoData
        {
            ID = "UserLastActivity",
            Title = "User last activity",
            Icon = "bi bi-clock-history",
            Content = @"
                <p>
                    Find information about your last activity, including: when your account was created, last time you
                    sign-in and last time you reset your password.
                </p>
                <ol>
                    <li class='unauth'>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li class='unauth'>Sign-up or sign-in with your email, or a social account.</li>

                    <li class='auth'>Looks like you already signed-in. Good!</li>

                    <li>From the Woodgrove header select the profile icon, or select the <b>Start the use case</b> button below which will take you to the <b>edit profile page</b>.</li>
                    <li>Scroll down to the <b>Sign-in activity</b> to check your activity information.</li>
                </ol>
            ",
            ActionUrl = "/SignIn?handler=UserLastActivity",
            AuthorizedActionUrl = "/Profile",
            ConfigHelpUrl = "/help/UserLastActivity",
            PostSignInRedirectUri = "/profile",
            Reauth = false
        });


        Demos.Add(new DemoData
        {
            ID = "SignInLog",
            Title = "Sign-in log",
            Icon = "bi bi-flag",
            Content = @"
                <p>
                    Microsoft Entra ID emits <a href='https://learn.microsoft.com/en-us/entra/identity/monitoring-health/concept-sign-ins' class='link-dark'>sign-in logs</a> containing activity information. Each sign-in attempt
                    contains details associated with those three main components:<br>
                    <b>Who</b>: The identity (User) doing the sign-in. <b>How</b>: The client (Application) used for the access. And <b>What</b>: The target (Resource) accessed by the identity.
                    <br>&nbsp;<br> You can use the sign-in logs to answer questions such as: How many users signed into a particular application this week? How many failed sign-in attempts occurred in the last 24 hours? Are users signing in from specific browsers or operating systems?
                </p>",
            ActionUrl = "",
            ConfigHelpUrl = "/help/SignInLog"
        });


        Demos.Add(new DemoData
        {
            ID = "UserInsights",
            Title = "Gain insights into your app users activity",
            Icon = "bi bi-lightbulb",
            Content = @"
                <p>
                    The user insights provides data analytics into user activity and engagement for your registered applications within your customer tenant.
                    <br>&nbsp;<br> Use Microsoft Graph and the Microsoft Entra Admin Center to view, query and analyze user activity data. For more information, learn <a href='https://learn.microsoft.com/entra/external-id/customers/how-to-user-insights' class='link-dark'>Gain insights into your app users’ activity</a>.
                    <br>&nbsp;<br> This demo uses Microsoft Graph API to query the usage & insights (<a href='https://learn.microsoft.com/graph/api/resources/dailyuserinsightmetricsroot?view=graph-rest-beta' class='link-dark'>daily</a> and <a href='https://learn.microsoft.com/graph/api/resources/monthlyuserinsightmetricsroot?view=graph-rest-beta' class='link-dark'>monthly</a>) to uncover valuable insights that can aid strategic decisions and drive business growth.
                </p>",
            ActionUrl = "/dashboard",
            ConfigHelpUrl = ""
        });

        Demos.Add(new DemoData
        {
            ID = "DDosAndBotProtectionGroup",
            Title = "DDoS and bot protection group",
            IsSpecialListItem = true,
            Content = string.Format(GroupHeader, "DDoS and bot protection")
        });

        Demos.Add(new DemoData
        {
            ID = "CloudflareNetwork",
            Title = "Cloudflare: Blocking suspicious IP addresses",
            Icon = "bi bi-robot",
            Content = @"
                <p>
                    The Cloudflare WAF safeguards web applications from DDoS attacks, malicious bots and more. It allows you blocking requests from specific IPs or ranges. In this example, requests from the Tor network will be blocked.
                </p>
                <ol>
                    <li>For this demo, download and run the <a class='link-dark' href='https://www.torproject.org/' target='_blank'>Tor Browse</a>. It's an open-source web browser that helps people use the internet anonymously.</li>
                    <li>In the Tor browser, reopen this page <code style='color: var(--color-very-dark);'>https://woodgrovedemo.com/#usecase=CloudflareNetwork</code></li>
                    <li>Then, select the <b>start the use case</b> button (at the bottom of this page).</li>
                    <li>A Cloudflare page will display a message indicating that access from the Tor network is restricted.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=CloudflareNetwork",
            PostSignInRedirectUri = "/token",
            ExtraParams = new List<DemoDataParam>
            {
                new DemoDataParam
                {
                    Name = "domain",
                    FixedValue = "login.woodgrovegroceries.com",
                },
                new DemoDataParam
                {
                    Name = "query-string",
                    FixedValue = "enablewaf=true&r=tor",
                }
            }
        });



        Demos.Add(new DemoData
        {
            ID = "CloudflareJsChallenge",
            Title = "Cloudflare: Bot protection (non-interactive)",
            Icon = "bi bi-robot",
            Content = @"
                <p>
                    The Cloudflare Web Application Firewall (WAF) non-interactive JavaScript challenge is a security feature designed to protect websites from malicious bots and other automated threats. 
                    When a visitor's IP address shows suspicious behavior or triggers a WAF custom rule, Cloudflare presents a challenge page that requires no interaction from the visitor. 
                    Instead, the visitor's browser processes the JavaScript challenge automatically, typically within a few second.
                </p>

                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>Cloudflare automatically presents challenge page that requires no interaction from you.</li>
                    <li>Wait until the browser finishes processing the challenge.</li>
                    <li>When completed, proceed to the Sign-in page.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=CloudflareJsChallenge",
            ExtraParams = new List<DemoDataParam>
            {
                new DemoDataParam
                {
                    Name = "domain",
                    FixedValue = "login.woodgrovegroceries.com",
                },
                new DemoDataParam
                {
                    Name = "query-string",
                    FixedValue = "enablewaf=true&r=jcptcha",
                }
            }
        });


        Demos.Add(new DemoData
        {
            ID = "CloudflareInteractiveChallenge",
            Title = "Cloudflare: Bot protection (interactive)",
            Icon = "bi bi-robot",
            Content = @"
                <p>
                    Cloudflare Web Application Firewall (WAF) offers an interactive challenge with CAPTCHA as part of its bot protection features. This type of challenge is designed to distinguish between human users and automated bots by requiring the user to complete a CAPTCHA, such as clicking a checkbox or solving a puzzle.
                </p>

                <ol>
                    <li>Select the <b>start the use case</b> button at the bottom of this page.</li>
                    <li>Cloudflare will present an interactive challenge that requires you to complete it, such as clicking a checkbox.</li>
                    <li>When completed, you will land on the Sign-in page.</li>
                </ol>",
            ActionUrl = "/SignIn?handler=CloudflareInteractiveChallenge",
            ExtraParams = new List<DemoDataParam>
            {
                new DemoDataParam
                {
                    Name = "domain",
                    FixedValue = "login.woodgrovegroceries.com",
                },
                new DemoDataParam
                {
                    Name = "query-string",
                    FixedValue = "enablewaf=true&r=icptcha",
                }
            }
        });

        Demos.Add(new DemoData
        {
            ID = "OtherApplicationsGroup",
            Title = "Other applications group",
            IsSpecialListItem = true,
            Content = string.Format(GroupHeader, "Other applications")
        });

        Demos.Add(new DemoData
        {
            ID = "NativeAuth",
            Title = "Native authentication (SPA app)",
            Icon = "bi bi-browser-edge",
            Content = @"
                <p>
                    Microsoft Entra <a class='link-dark' href='https://learn.microsoft.com/en-us/entra/external-id/customers/concept-native-authentication'>native authentication</a> allows you to have full control over the design of your mobile, desktop application and <a class='link-dark' href='https://learn.microsoft.com/en-us/entra/external-id/customers/how-to-native-authentication-cors-solution-production-environment'>single page application (SPA)</a> sign-in experiences. 
                </p>
                <p>
                    It enables you to create visually appealing, pixel-perfect authentication screens that seamlessly blend into your app’s interface. With this approach, you can fully customize the user interface, including design elements, logo placement, and layout, ensuring a consistent and branded look
                </p>",
            ActionUrl = "/SignIn?handler=NativeAuth",
            RedirectUri = "https://woodgroveairline.com/"
        });


        Demos.Add(new DemoData
        {
            ID = "SPA",
            Title = "Single page app (sign-in with popup and redirection)",
            Icon = "bi bi-browser-edge",
            Content = @"
                <p>
                    A single-page application (SPA) loads a single web page from the web server and updates the content dynamically using JavaScript. 
                    The front end is written in <a class='link-dark' target='_blank' href='https://learn.microsoft.com/en-us/entra/external-id/customers/tutorial-single-page-app-vanillajs-prepare-tenant'>JavaScript</a> or TypeScript, often utilizing frameworks 
                    like <a class='link-dark' target='_blank' href='https://learn.microsoft.com/en-us/entra/external-id/customers/tutorial-single-page-app-angular-sign-in-prepare-tenant'>Angular</a>, 
                    <a class='link-dark' target='_blank' href='https://learn.microsoft.com/en-us/entra/external-id/customers/tutorial-single-page-app-react-sign-in-prepare-tenant'>React</a>, or 
                    Vue. The authentication flow is managed on the client side with JavaScript code. 
                </p>
                <p>
                    This demo presents a SPA app that offers sign-in options using a popup window, maintaining the context of the app, and redirecting to Microsoft Entra External ID.
                </p>",
            ActionUrl = "/SignIn?handler=SPA",
            RedirectUri = "https://travels.woodgrovedemo.com/"
        });



        Demos.Add(new DemoData
        {
            ID = "Saml",
            Title = "SAML web app",
            Icon = "bi bi-globe",
            Content = @"
                <p>
                    The Woodgrove Bank demo application illustrates the sign-up and sign-in authentication experiences for financial scenarios. It also demonstrates the SAML protocol federation with Microsoft Entra External ID.
                </p>",
            ActionUrl = "/SignIn?handler=Saml",
            RedirectUri = "https://woodgrovebanking.com/"
        });

        Demos.Add(new DemoData
        {
            ID = "AdvancedScenariosGroup",
            Title = "Advanced scenarios group",
            IsSpecialListItem = true,
            Content = string.Format(GroupHeader, "Advanced scenarios")
        });

        Demos.Add(new DemoData
        {
            ID = "GitHubWorkflows",
            Title = "Automation with GitHub Workflows",
            Icon = "bi bi-github",
            Content = @"
                <p>
                    <a href='https://learn.microsoft.com/en-us/powershell/microsoftgraph/overview?view=graph-powershell-1.0'  target='_blank' class='link-dark link-offset-2'>Microsoft Graph PowerShell</a> is a robust
                    solution for automating tasks, executing batch operations, maintaining and ensuring consistency across different stages such as test,  preproduction, and production environments.
                    <br>&nbsp;<br>With <a href='https://docs.github.com/en/actions/using-workflows/about-workflows' target='_blank'  class='link-dark link-offset-2'>GitHub workflow</a> you can automate process that will run one or more jobs.
                    Their benefits in accelerating and stabilizing the deployment process to Microsoft Entra external ID. It leads to a significant reduction in integration issues, faster release cycles, enhance change management, and consistency that are crucial for maintaining data integrity and smooth and seamless deployment during updates and modifications.
                </p>",
            ActionUrl = "",
            ConfigHelpUrl = "/help/GitHubWorkflows"
        });


        // Demos.Add(new DemoData
        // {
        //     ID = "Kiosk",
        //     Title = "Input constrained devices (Kiosk)",
        //     Icon = "bi bi-display",
        //     Content = @"
        //         <p>
        //             Input-constrained devices are devices that their screen or monitor is limited to text-only and they don't have a web browser. For example, smart TV, IoT device, robot,
        //             gaming console, printers. Or applications with limited user interface, such as a command line application.
        //             <br>&nbsp;<br> These devices are connected to the internet, but due to the input constrains, the
        //             authentication should be done on another device. The input constrained device gets a device code from Microsoft Entra External ID and asks the user to visit a webpage in a browser  on a second (rich device), such as smartphone, tablets, or PCs.
        //             <br>&nbsp;<br> In this use case, from the Kiosk page select sign-in. Use the second device, such as smartphone and scan the QR code. On the sign-in page enters the <i>device code</i>, and completes the sign-in. Once you signed in, the Kiosk (input-constrained device) is able  to get security tokens and authenticate you. Your name should be presented on the top-right corner of the page.
        //         </p>",
        //     ActionUrl = "/SignIn?handler=kiosk",
        //     ConfigHelpUrl = "",
        //     RedirectUrl = "https://woodgroverestaurants.com",
        // });

        Demos.Add(new DemoData
        {
            ID = "TokenSignin",
            Title = "This flow starts from the token page and is not included in the list of demos presented to the user. It's used to update the ID token.",
            Content = "",
            ServerSideOnly = true,
            Reauth = false,
            PostSignInRedirectUri = "/token"
        });

        Demos.Add(new DemoData
        {
            ID = "ProfileSignin",
            Title = "This flow starts from the profile page and is not included in the list of demos presented to the user. It's used to update the ID token.",
            Content = "",
            ServerSideOnly = true,
            Reauth = false
        });

        Demos.Add(new DemoData
        {
            ID = "ProfileReauth",
            Title = "This flow starts from the profile page and is not included in the list of demos presented to the user. It's used to update the ID token.",
            Content = "",
            ServerSideOnly = true,
            Reauth = false,
            PostSignInRedirectUri = "/profile"
        });

        Demos.Add(new DemoData
        {
            ID = "EditProfileMfa",
            Title = "This flow starts from the profile page and is not included in the list of demos presented to the user.",
            Content = "",
            ServerSideOnly = true,
            Reauth = false,
            PostSignInRedirectUri = "/profile",
            ExtraParams = new List<DemoDataParam>
            {
                new DemoDataParam
                {
                    Name = "StepUp",
                    FixedValue = "true",
                }
            }
        });

        Demos.Add(new DemoData
        {
            ID = "InvalidSession",
            Title = "This flow starts from the AuthError page and is not included in the list of demos presented to the user.",
            Content = "",
            ServerSideOnly = true
        });
    }
}

