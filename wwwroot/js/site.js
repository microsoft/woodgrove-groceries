// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function showGraphAPI() {
    $("#stepNavigatorContainer").css('visibility', 'hidden');
    $("#showGraphAPI").hide();
    $(".bs-stepper").hide();
    $("#showEntraAdminCenter").show();
    $("#GraphApiContent").show();


}

function showEntraAdminCenter() {
    $("#stepNavigatorContainer").css('visibility', 'visible');;
    $("#showGraphAPI").show();
    $(".bs-stepper").show();
    $("#showEntraAdminCenter").hide();
    $("#GraphApiContent").hide();
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

    if (graph != null) {
        // Show help's Graph
        showGraphAPI();
    }
    else {
        // Start a demo
        showUseCase(eventType);
    }
}

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

    var useCases = ["Default", "OnlineRetail", "CustomDomain", "AssignmentRequired", "StepUp", "CSA", "PolicyAgreement", "EmailAndPassword", "OBO", "SSO", "MFA", "CA", "ForceSignIn", "UserInsights", "ModifyAttributeValues", "BlockSignUp", "CompanyBranding", "Language", "PreSelectLanguage", "SSPR", "Social", "LoginHint", "TokenAugmentation", "TokenClaims", "PreAttributeCollection", "PostAttributeCollection", "ProfileEdit", "DeleteAccount", "Activity", "RBAC", "GBAC", "CustomAttributes", "Kiosk", "Finance"];

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
    window.location.hash = '';
})



function onPreSelectLanguagesSelected() {

    var preSelectLanguages = document.getElementById("preSelectLanguages");
    window.location = "/SignIn?handler=PreSelectLanguage&ui_locales=" + preSelectLanguages.options[preSelectLanguages.selectedIndex].value
}
