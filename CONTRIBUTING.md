# Contribution guide

Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit <https://cla.opensource.microsoft.com>.

First off, we sincerely thank you for taking the time to contribute. All types of contributions are encouraged and valued. Please note the code of conduct and follow it in all your interactions with this project. 

Please make sure to read the relevant section before making your contribution. It makes it a lot easier for others, and smoothes out the experience for all involved. The community looks forward to your contributions. 🎉

## Code of conduct

- This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information, see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
- Always use a welcoming and inclusive language.
- Be respect and show empathy toward other community members and opinions.

## Suggesting enhancements

This section guides you through submitting an enhancement suggestion including completely new features, new use case (demo), or a minor improvement.

- Make sure that you're using the latest version.
- Check the list of use cases (demos) to find out if the functionality is already covered.
- Find out whether your idea fits with the scope and aims of the project. 
- Ensure the Pull Request description clearly describes the problem and solution. Include the relevant issue number if applicable.

## Building blocks

This section provide details for the Woodgrove Groceries live demo app. This app is based on dotnet 8 web app with Razor pages. For the UI, the project uses Bootstrap 5.x, and JQuery.

Every use case (demo) must have a unique **ID**, such as **MFA** (multifactor authentication), **CA** (conditional access), or **SSPR** (self-service password reset). The use case ID is used in the following files.

### Use cases

1. [/Pages/Shared/AboutThisDemoPartial.cshtml](./Pages/Shared/AboutThisDemoPartial.cshtml) contains the content of the Bootstrap off canvas. The off canvas is shown when a visitor clicks on the **Select a use case** button (right side of the screen). 
1. To add a new use case (demo):
    1. Find the DIV element with `class="dropdown"` and add your demo under one of the categories. The link to your demo must be in the following format `href="#usecase=ID"`, where the ID represents your use case ID. For example, `href="#usecase=SSPR"`.
    1. Locate the `<div id="useCases">` element and add a child element `<div id="useCase_ID" class="useCase">` , where the ID represents your use case ID. For example, `<div id="useCase_SSPR" class="useCase">`.
    1. In the use case DIV element, add an `<h4>` header, a short description, and a link to start the demo `href="/SignIn?handler=ID"` where the ID represents the use case ID. For example, `href="/SignIn?handler=SSPR"`.
    1. If your demo has a help page, add another link to your help page. For example, `<a role="button" href="/help/SSPR">Learn how we configured the Woodgrove tenant</a>`.
    1. Finally, scroll down to the JavaScript section and add your use case ID to the `var useCases = ["Default", "AssignmentRequired",..]` collection.

1. The  [/Pages/SignIn.cshtml.cs](./Pages/SignIn.cshtml.cs) contains the actions to start the use casesS. 
1. To add a new use case, add new function with the use case ID. For example, for **SSPR** the function name will be OnGet**SSPR**. Then a call to the TrackAndAuth method. Provide the use case ID, the post redirect URL, and whether  to ignore the single-sign-on. For most of the use cases this parameter will be **true**. Otherwise the visitor might skip the sign-in experience and won't be able to experience your demo.

    ```
    public IActionResult OnGetSSPR()
    {
        return this.TrackAndAuth("SSPR", "/", true);
    }
    ``` 

### Help files

To provide help page for your use case (recommented), do the following:

1. Create a new Razor web page under the [/Areas/Help/Pages](./Areas/Help/Pages/) folder. The following command will create such a file for you. Replace the **ID** with your use case ID.

    ```
    dotnet new page --name ID --namespace woodgrovedemo.Help.Pages --output  Areas/Help/Pages
    ```

1. In the .cshtml.cs file, add the telemetry with your use case unique ID. You can copy it from another use case, such as [SSPR.cshtml.cs](./Areas/Help/Pages/SSPR.cshtml.cs).
1. Then in the **cshtml** file adds the content guide. You can copy the content from another help file. **Important**, make sure to enter the exact number of steps in the `@await Html.PartialAsync("_Steps.cshtml", 5)` code. In this example, there are five steps.
1. Create a sub folder with your use case ID under the [/wwwroot/Help/](./wwwroot/Help/) folder.
1. Add the screenshots into the folder you created.

## Create a pull request

When creating a pull request always set the target branch as **vNext**. Changes to this branch will be automatically deployed to the <https://private.woodgrovedemo.com>. So you can test it in the staging environment. After you confirm everything works as expected. Create another PR to merge the changes into the **main** branch.

## Deployment

After you completed writing your code, and tested it locally in your development workstation, it's time to publish your changes. 

- This GitHub repository uses a GitHub action called [main_woodgrove-groceries.yml](./.github/workflows/main_woodgrove-groceries.yml) which is triggered on **commit** to the **main** branch, or on pull request **merge**. 
- You can check the status of your dempoloyen in the last commit. Usually it takes 2-3 minutes. 
- The [/appsettings.json](./appsettings.json) contains empty or fake values. To change the settings (if you have access) go to Woodgrove Azure Service App and select the **App Settings** from the menu. Then add or change the app settings. **NEVER** add any information from the Woodgrove tenant that my cause security or privacy issues.


Thanks again for your help, support and contribution! :heart: :heart: :heart:
