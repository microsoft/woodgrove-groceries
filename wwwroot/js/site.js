// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function showGraphAPI() {
    // $("#stepNavigatorContainer").css('visibility', 'hidden');
    // $("#showGraphAPI").hide();
    // $(".bs-stepper").hide();
    // $("#showEntraAdminCenter").show();
    // $("#GraphApiContent").show();

    const triggerEl = document.querySelector('#helpSelector button[data-bs-target="#microsoftGraph"]')
    bootstrap.Tab.getInstance(triggerEl).show() // Select tab by name
}

function showPowerShell() {
    const triggerEl = document.querySelector('#helpSelector button[data-bs-target="#graphPowerShell"]')
    bootstrap.Tab.getInstance(triggerEl).show() // Select tab by name
}


// When the page is loaded start the demo
var waitForJQuery = setInterval(function () {
    if (typeof $ != 'undefined') {

        clearInterval(waitForJQuery);

        checkForHashParam(1);;
    }
}, 500);

// Listen to the URL hash change event
window.addEventListener("hashchange", function () {
    checkForHashParam(3);
});

function checkForHashParam(eventType) {
    var myUrl = new URL(window.location.href.replace(/#/g, "?"));

    var graph = myUrl.searchParams.get("graph");
    var powerShell = myUrl.searchParams.get("ps");
    var usecase = myUrl.searchParams.get("usecase");
    var cmd = myUrl.searchParams.get("cmd");

    // If searchParams is empty, hide the offcanvas and exist
    if (myUrl.searchParams === null || myUrl.searchParams.size == 0 || (graph === null && powerShell === null && usecase === null && cmd === null)) {
        $("#offcanvasRight").offcanvas('hide');
        return;
    }

    if (graph != null) {
        // Show help's Graph
        showGraphAPI();
    }
    else if (powerShell != null) {
        // Show help's Graph
        showPowerShell();
    }
    else {
        // Start a demo
        showUseCase(eventType);
    }
}


// Filter the use cases
function filterUseCases() {
    var input, filter, ul, li, a, i;
    input = document.getElementById("serachUseCase");
    filter = input.value.toUpperCase();
    div = document.getElementById("demosDropdown");
    a = div.getElementsByTagName("li");

    for (i = 0; i < a.length; i++) {
        txtValue = a[i].textContent || a[i].innerText;
        if ((txtValue.toUpperCase().indexOf(filter) > -1)) {
            a[i].style.display = "";
        } else {
            console.log(a[i].id)
            if (a[i].id != "UseCaseSearchContainer")
                a[i].style.display = "none";
        }
    }

    // Hide the use cases' header
    if (filter === "") {
        $(".UseCaseHeader").show();
    }
    else {
        $(".UseCaseHeader").hide();
    }

}

// Clear the filter
function clearFilterUseCases() {
    document.getElementById("serachUseCase").value = '';
    filterUseCases();
}

document.querySelectorAll('.dropdown-menu .no-close').forEach(item => {
    item.addEventListener('click', event => {
        event.stopPropagation(); // Prevent Bootstrap from closing the dropdown
    });
});

// Show the demo
function showUseCase(trigger) {
    var myUrl = new URL(window.location.href.replace(/#/g, "?"));
    var usecase = myUrl.searchParams.get("usecase");
    var cmd = myUrl.searchParams.get("cmd");

    if (cmd === "StepUpCompleted") {
        completeOrder();
        return;
    }

    // Click on the right button
    if (trigger === 2) {
        usecase = 'Default';
    }

    var useCases = ["Default", "SignUpLink", "ArkoseFraudProtection", "MSA", "SPA", "NativeAuth", "EmailOtp", "OnlineRetail", "DisableAccount", "CustomDomain", "CustomEmail", "AssignmentRequired", "StepUp", "CSA", "PolicyAgreement", "EmailAndPassword", "OBO", "SSO", "GithubWorkflows", "TokenTTL", "MFA", "CA", "ForceSignIn", "UserInsights", "SignInLog", "ModifyAttributeValues", "BlockSignUp", "CompanyBranding", "Language", "PreSelectLanguage", "SSPR", "Social", "ActAs", "LoginHint", "TokenAugmentation", "TokenClaims", "PreAttributeCollection", "PostAttributeCollection", "ProfileEdit", "DeleteAccount", "UserLastActivity", "RBAC", "GBAC", "CustomAttributes", "Kiosk", "Saml"];

    if (($('#offcanvasRight').length > 0) && usecase && (useCases.indexOf(usecase) > -1)) {

        $("#offcanvasRight").offcanvas('show');
        useCaseId = "useCase_" + usecase;

        $(".useCase").hide();
        $("#" + useCaseId).show();

        // Telemetry to improve the demo 
        var triggerIDs = ["Link", "Start", "Select"];

        $.get("/SelectUseCase/usecase?id=" + useCaseId.replace('useCase_', '') + "&trigger=" + triggerIDs[trigger - 1] + "&referral=" + document.referrer, function (data) {

        }).fail(function (response) {
            console.log("Telemetry error:");
            console.log(response)
        });
    }
    else {
        $("#offcanvasRight").offcanvas('hide');
        window.location.hash = '';
    }
}

const myOffcanvas = document.getElementById('offcanvasRight');
myOffcanvas.addEventListener('hidden.bs.offcanvas', () => {

    var myUrl = new URL(window.location.href.replace(/#/g, "?"));
    var usecase = myUrl.searchParams.get("usecase");

    if (usecase) {
        window.location.hash = '';
    }

})



function onPreSelectLanguagesSelected() {

    var preSelectLanguages = document.getElementById("preSelectLanguages");
    window.location = "/SignIn?handler=PreSelectLanguage&ui_locales=" + preSelectLanguages.options[preSelectLanguages.selectedIndex].value
}

/********* Stepper *********/
var stepper
$(document).ready(function () {

    $('.feedback').popover({ placement: "top", trigger: "hover", content: "Foud a bug or have a question? Want to provide feedback? Click on this button and raise an issue on GitHub." });


    if ($('.pop').length > 0) {
        $('.pop').on('click', function () {
            $('.imagepreview').attr('src', $(this).find('img').attr('src'));
            $('#imagemodal').modal('show');
        });
    }

    if ($('.bs-stepper').length > 0) {

        stepper = new Stepper($('.bs-stepper')[0], {
            linear: false,
            animation: true
        })

        // Add the links to the pages
        if ($('#stepNavigator').length > 0) {

            var items = $('.bs-stepper-pane').length;

            for (let i = 0; i < items; i++) {
                $('#stepNavigator').append('<li><a class="dropdown-item" onclick="stepper.to(' + (i + 1) + '); return false;" href="#">' + (i + 1) + '</a></li>')
            }
        }

        $('.bs-stepper')[0].addEventListener('shown.bs-stepper', function (event) {

            $("#stepNumber").html(event.detail.indexStep + 1)

            if (event.detail.indexStep == 0) {
                // Disable previous button
                $("#movePrevious").css("pointer-events", "none");
                $("#movePrevious").css("color", "gray");
            }
            else {
                // Enable previous button
                $("#movePrevious").css("pointer-events", "auto");
                $("#movePrevious").css("color", "");
            }

            if (event.detail.indexStep + 1 == $('.bs-stepper-pane').length) {
                // Disable next button
                $("#moveNext").css("pointer-events", "none");
                $("#moveNext").css("color", "gray");
            }
            else {
                // Enable steps  next button
                $("#moveNext").css("pointer-events", "auto");
                $("#moveNext").css("color", "");
            }

        })
    }
});
/********* End of stepper *********/

/********* Help selector *********/
const triggerTabList = document.querySelectorAll('#helpSelector button')
triggerTabList.forEach(triggerEl => {
    const tabTrigger = new bootstrap.Tab(triggerEl)

    triggerEl.addEventListener('click', event => {
        event.preventDefault()
        tabTrigger.show()
    })
})
/********* End of help selector *********/


/********* PowerShell *********/

if ($('#microsoftGraph').length > 0 && $('#graphPowerShell').length > 0) {
    // Get the source and target div elements
    var sourceDiv = document.getElementById("microsoftGraph");
    var targetDiv = document.getElementById("graphPowerShell");

    // Copy the content from source div to target div
    targetDiv.innerHTML = sourceDiv.innerHTML;

    const codes = document.querySelectorAll('#graphPowerShell pre.replace')
    codes.forEach(triggerEl => {

        triggerEl.innerHTML = syntaxHighlight(triggerEl.innerHTML)
        triggerEl.innerHTML = "$params = " + triggerEl.innerHTML;

        if (triggerEl.getAttribute("ignoreNull") === '') {
            // Remove null attribures
            var lines = triggerEl.innerHTML.split('\n');
            lines = lines.filter(function (str) { return str.includes("$undefinedVariable") === false; });
            //console.log(array.length)
            triggerEl.innerHTML = lines.join('\n')
        }
    })

    // Convert JSON example to list (HTML table in list format)
    const examples = document.querySelectorAll('#graphPowerShell pre.toList')
    examples.forEach(triggerEl => {

        triggerEl.innerHTML = '<table class="table">' + jsonToList(JSON.parse(triggerEl.innerHTML.replaceAll('"highlight"', "'highlight'")), '') + '</table>';
    });

    // Convert JSON example to list (HTML table in list format)
    const examplesTable = document.querySelectorAll('#graphPowerShell pre.toTable')
    examplesTable.forEach(triggerEl => {

        triggerEl.innerHTML = '<table class="table">' + jsonToTable(JSON.parse(triggerEl.innerHTML.replaceAll('"highlight"', "'highlight'"))) + '</table>';
    });
}

function jsonToList(json, parentKey) {
    var rows = '';

    for (const key in json) {

        if (key.startsWith('@')) {
            continue;
        }

        if (!(typeof json[key] === "object")) {
            var keyName = capitalizeFirstLetter(((parentKey === undefined || parentKey === '') ? '' : parentKey + '.') + key)
            rows += `<tr scope="row"><td>${keyName}</td><td>${json[key]}<td></tr>`
        }
        else {
            rows += jsonToList(json[key], ((parentKey === undefined || parentKey === '') ? '' : parentKey + '.') + key)
        }
    }

    return rows;
}


function jsonToTable(json) {
    var row1 = '<tr scope="row">';
    var row2 = '<tr scope="row">';

    for (const key in json) {

        if (key.startsWith('@')) {
            continue;
        }

        row1 += `<td>${capitalizeFirstLetter(key)}</td>`;
        row2 += `<td>${json[key]}</td>`
    }

    row1 += '</tr>';
    row2 += '</tr>';

    return row1 + row2;
}

function capitalizeFirstLetter(string) {
    return string.charAt(0).toUpperCase() + string.slice(1);
}

function syntaxHighlight(json) {

    json = json.replace(/("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)/g, function (match) {

        //console.log(match)
        if (/^"/.test(match)) {
            if (/:$/.test(match)) {
                if (!match.startsWith('"@')) {
                    match = match.replaceAll('"', '');
                }

                match = match.replaceAll(':', ' =')
            }
        }
        else if (/true|false/.test(match)) {
            match = "$" + match;
        }
        return match;
    });

    // Remove comma at the end of the line
    json = json.replace(/,[\n]+/g, function (match) {
        return "\n";
    });

    // Replace [] to *()
    json = json.replace(/\[\][\n]+/g, function (match) {
        return " @()\n";
    });

    // Replace [ to @( at the end of the line
    json = json.replace(/\[[\n]+/g, function (match) {
        return " @(\n";
    });

    // Replace ] to ) at the end of the line
    json = json.replace(/\][\n]+/g, function (match) {
        return ")\n";
    });

    // Replace { to @{ at the end of the line
    json = json.replace(/{[\n]+/g, function (match) {
        return " @{\n";
    });

    // Replace null to @{=null at the end of the line
    json = json.replace(/null[\n]+/g, function (match) {
        return " $undefinedVariable\n";
    });

    return json;
}
/********* End of PowerShell *********/

$('.copyToClipboard').click(function () {
    navigator.clipboard.writeText($(this).next('pre').text());

    return false;
});