var validUseCases = [];

$(document).ready(function () {
    // Fetch demos from API
    $.ajax({
        url: '/api/demos',
        method: 'GET',
        success: function (response) {
            const $dropdown = $('#demosDropdown');
            const $useCases = $('#useCases');

            // Clear existing content
            $dropdown.empty();
            $useCases.empty();

            // Iterate through demos and add to dropdown
            response.forEach(demo => {
                if (demo.isSpecialListItem) {
                    // For special items, directly add the content
                    $dropdown.append(demo.content);
                } else {

                    // Add the demo ID to the valid use cases array
                    validUseCases.push(demo.id);

                    // Check if the demo is hidden
                    let isHidden = ''
                    if (demo.isHidden) {
                        isHidden = 'display: none;';
                    }

                    // For regular items, use the standard format to add to the dropdown 
                    let htmlListItem = `<li><a class="dropdown-item" href="#usecase=${demo.id}" style="${isHidden}"><i class="${demo.icon}"></i>&nbsp; ${demo.title}</a></li>`;
                    $dropdown.append(htmlListItem);


                    // Set the title for the use case
                    let title = demo.title;
                    if (demo.cardTitle) {
                        title = demo.cardTitle;
                    }

                    // Prepare the start the use case button
                    let startButton = '';
                    let actionUrl = '';

                    if (demo.actionUrl) {
                        actionUrl = demo.actionUrl;
                    }

                    // If the AuthorizedActionUrl is not empty, send the user to the AuthorizedActionUrl
                    if (authenticated && demo.authorizedActionUrl) {
                        actionUrl = demo.authorizedActionUrl
                    }

                    // Set the button text
                    let buttonText = "Start the use case";

                    // Get the atlernative text from the demo object
                    if (demo.actionText) {
                        buttonText = demo.actionText;
                    }

                    // If the user is authenticated, use the authorizedActionText
                    if (authenticated && demo.authorizedActionText) {
                        buttonText = demo.authorizedActionText;
                    }

                    if (actionUrl !== '') {
                        // If the actionUrl is not empty, create a button to start the use case
                        startButton = `<a id="${demo.id}_start" class="btn header-button" role="button" href="${actionUrl}"><i class="bi bi-play-circle"></i> ${buttonText}</a>`;
                    }

                    // Prepare the configuration help button
                    let configHelpButton = ''
                    if (demo.configHelpUrl) {
                        configHelpButton = `<br>&nbsp;<br><i class="bi bi-play-circle"></i> <a class="" style="margin-top: 100px;" role="button" href="${demo.configHelpUrl}"> Learn how we configured the Woodgrove tenant</a>`;
                    }

                    // Add the content to the useCases HTML element
                    let htmlContent = `
                        <div id="useCase_${demo.id}" class="useCase">
                          <h4>${title}</h4>
                          <div>${demo.content}</div>

                          ${startButton}

                          ${configHelpButton}

                        </div>`;
                    $useCases.append(htmlContent);
                }
            });

            // Handle specific demo cases
            // Get the domain name from the customDomainValue input value and add it to the customDomain HTML element using Jquery
            let customDomainValue = $('#customDomainValue').val();
            $('#customDomain').text(customDomainValue);

            // Call this function only when the data is loaded
            checkForHashParam(1)

            // Listen to the URL hash change event
            window.addEventListener("hashchange", function () {
                checkForHashParam(3);
            });
        },
        error: function (xhr, status, error) {
            console.error('Error fetching demos:', error);
            $('#demosDropdown').html('<li class="dropdown-item">Error loading demos</li>');
        }
    });

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

    // Iterate through all list items, and hide those who don't match the search query
    for (i = 0; i < a.length; i++) {

        // Skip LI elements with the ID "UseCaseSearchContainer"
        if (a[i].id === "UseCaseSearchContainer") {
            continue;
        }

        txtValue = a[i].textContent || a[i].innerText;
        if ((txtValue.toUpperCase().indexOf(filter) > -1)) {
            a[i].style.display = "";
        } else {
            if (a[i].getAttribute("class") != "UseCaseSearch")
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

    if (($('#offcanvasRight').length > 0) && usecase && (validUseCases.indexOf(usecase) > -1)) {

        $("#offcanvasRight").offcanvas('show');
        useCaseId = "useCase_" + usecase;

        $(".useCase").hide();
        $("#" + useCaseId).show();

        // Telemetry to improve the demo 
        var triggerIDs = ["Link", "Start", "Select"];

        $.get("/api/SelectUseCase/usecase?id=" + useCaseId.replace('useCase_', '') + "&trigger=" + triggerIDs[trigger - 1] + "&referral=" + document.referrer, function (data) {

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



