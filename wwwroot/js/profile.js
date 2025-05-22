var disableAlert, signInAlert, mfaFulfilled, verificationAlert;

$(document).ready(function () {

    disableAlert = new bootstrap.Modal(document.getElementById('disableAlert'), { keyboard: false });
    signInAlert = new bootstrap.Modal(document.getElementById('signInAlert'), { keyboard: false });
    mfaAlert = new bootstrap.Modal(document.getElementById('mfaAlert'), { keyboard: false });
    verificationAlert = new bootstrap.Modal(document.getElementById('verificationAlert'), { keyboard: false });

    // Get user profile
    getUserAttributes();
    getUserRoles();
    getUserMoreInfo();

    // Check if MFA requirement has been fullfilled
    mfaFulfilled = ($("#MfaFulfilled").length == 1)

    // Enable or disable the profile attributes
    $("#inputCity").prop("disabled", !mfaFulfilled)
    $("#inputCountry").prop("disabled", !mfaFulfilled)
    $("#inputDisplayName").prop("disabled", !mfaFulfilled)
    $("#inputGivenName").prop("disabled", !mfaFulfilled)
    $("#inputSpecialDiet").prop("disabled", true)
    $("#inputSurname").prop("disabled", !mfaFulfilled)
    $("#inputEmailMfa").prop("disabled", !mfaFulfilled)
    $("#inputPhoneNumber").prop("disabled", !mfaFulfilled)
    $("#inputSignInEmail").prop("disabled", !mfaFulfilled)

    // Show or hide the edit profile buttonss
    if (mfaFulfilled) {
        $(".mfaFulfilled").show();
        $(".mfaRequiredButton").hide();
    }
    else {
        $(".mfaFulfilled").hide();
        $(".mfaRequiredButton").show();
    }
});

function disableAccount() {

    $("#disableAccountButtonSpinner").show();
    $.ajax({
        url: "/api/DisableAccount",
        success: function (result) {

            if (!result.errorMessage) {
                signInAlert.show();
            }
            else {
                $("#errorMessage").text(result.errorMessage);
                $("#errorMessageContainer").show();
            }

            $("#disableAccountButtonSpinner").hide();
        },
        // The function to execute when the request fails
        error: function (xhr, status, error) {
            console.log("/DisableAccount")
            console.log("Error: " + error);
            $("#disableAccountButtonSpinner").hide();
        }
    });

    // Hide the modal dialog
    disableAlert.hide();

}

function getUserAttributes() {
    $.ajax({
        url: "/api/userattributes",
        success: function (result) {

            if (!result.errorMessage) {
                $("#inputCity").val(result.city);
                $("#inputCountry").val(result.country);
                $("#inputDisplayName").val(result.displayName);
                $("#inputGivenName").val(result.givenName);
                $("#inputSpecialDiet").val(result.specialDiet);
                $("#inputSurname").val(result.surname);
                $('#inputAccountEnabled').prop('checked', result.accountEnabled);

                // Read only attributes
                $("#inputObjectID").text(result.objectId);
                $("#inputLastPasswordChangeDateTime").text(result.lastPasswordChangeDateTime);
                $("#inputCreatedDateTime").text(result.createdDateTime);

                // Show the editProfileSection
                $("#editProfileSection").show();
                $("#editProfileSpinner").hide();

            }
            else {

                $("#errorMessageContainer").show();

                if (result.errorMessage.includes("AcquireTokenSilent")) {
                    $("#errorMessage").text('Your access token is invalid. Please sign in!');
                    $("#signInButton").show();
                    $(".hideIfNoAuthenticated").hide();
                }
                else {
                    $("#errorMessage").text(result.errorMessage);
                }
            }

        },
        // The function to execute when the request fails
        error: function (xhr, status, error) {
            console.log("/UserAttributes")
            console.log("Error: " + error);
        }
    });
}

function updateUserAttributes() {

    $("#editProfileButtonSpinner").show();
    $("#errorMessageContainer").hide();
    $("#editProfileButton").prop("disabled", true);

    var payload = {
        city: $("#inputCity").val(),
        country: $("#inputCountry").val(),
        displayName: $("#inputDisplayName").val(),
        givenName: $("#inputGivenName").val(),
        specialDiet: $("#inputSpecialDiet").val(),
        surname: $("#inputSurname").val(),
        accountEnabled: $('#inputAccountEnabled').prop('checked')
    }

    $.post("/userattributes", payload, function (result) {

        $("#editProfileButtonSpinner").hide();
        $("#editProfileButton").prop("disabled", false);

        if (result.errorMessage) {
            $("#errorMessage").text(result.errorMessage);
            $("#errorMessageContainer").show();
        }
        else {
            signInAlert.show();
        }

    });
}

function getUserRoles() {
    $.ajax({
        url: "/api/userroles",
        success: function (result) {

            if (!result.errorMessage) {
                $('#inputMemberOfCommercialAccounts').prop('checked', result.memberOfCommercialAccounts);
                $('#inputHasProductsContributorRole').prop('checked', result.hasProductsContributorRole);
                $('#inputHasOrdersManagerRole').prop('checked', result.hasOrdersManagerRole);

                // Show the editProfileSection
                $("#rolesSection").show();
                $("#rolesSpinner").hide();
            }
            else {
                $("#errorMessage").text(result.errorMessage);
                $("#errorMessageContainer").show();
            }

        },
        // The function to execute when the request fails
        error: function (xhr, status, error) {
            console.log("/UserRoles")
            console.log("Error: " + error);
        }
    });
}

function updateUserRoles() {

    $("#rolesButtonSpinner").show();
    $("#rolesButton").prop("disabled", true);

    var payload = {
        memberOfCommercialAccounts: $('#inputMemberOfCommercialAccounts').prop('checked'),
        hasProductsContributorRole: $('#inputHasProductsContributorRole').prop('checked'),
        hasOrdersManagerRole: $('#inputHasOrdersManagerRole').prop('checked')
    }

    $.post("/userroles", payload, function (result) {

        $("#rolesButtonSpinner").hide();
        $("#rolesButton").prop("disabled", false);

        signInAlert.show()
    });
}

function getUserMoreInfo() {
    $.ajax({
        url: "/api/usermoreinfo",
        success: function (result) {

            if (!result.errorMessage) {
                $("#inputIdentities").html(result.identities);

                // Sign-in name
                $("#inputSignInEmail").val(result.singInEmail);

                // MFA authentication methods
                $("#inputEmailMfa").val(result.emailMfa);
                $("#inputPhoneNumber").val(result.phoneNumber);

                // Activity
                $("#inputLastSignInDateTime").text(result.lastSignInDateTime);
                $("#inputLastSignInRequestId").text(result.lastSignInRequestId);

                // Show the sign-in name section
                if (result.singInEmail && result.singInEmail != '') {
                    $("#singInSection").show();
                    $("#singInSpinner").hide();
                }
                else {
                    // Hide for social accounts
                    $("#singInContainer").hide();
                }

                // Show the MFA section
                $("#mfaSection").show();
                $("#mfaSpinner").hide();
            }
            else {
                $("#errorMessage").text(result.errorMessage);
                $("#errorMessageContainer").show();
            }

        },
        // The function to execute when the request fails
        error: function (xhr, status, error) {
            console.log("/UserMoreInfo")
            console.log("Error: " + error);
        }
    });
}

function sendCodeForSignInEmail() {

    // Set up UI elements
    $('#inputVerificationCode').val("");
    $('#verificationError').text("")
    $('#verificationSpinner').show();
    $('#verificationContainer').hide();
    $('#verificationButtonSpinner').hide();

    // Show the alert
    verificationAlert.show();

    var payload = {
        AuthValue: $('#inputSignInEmail').val(),
        AuthType: 0
    }

    $.ajax({
        url: "/api/SendCode",
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload),
        success: function (result) {

            // Show the UI elements
            $('#verificationSpinner').hide();
            $('#verificationContainer').show();

            if (result.error) {
                // Show the error message
                $("#verificationError").text(result.error);
            }

        },
        // The function to execute when the request fails
        error: function (xhr, status, error) {
            console.log("/SendCode")
            console.log("Error: " + error);
            $("#verificationError").text(error);
        }
    });
}


function sendCodeForEmailMfa() {

    // Clear authentication method
    if ($('#inputEmailMfa').val().trim() == '') {
        return;
    }

    // Set up UI elements
    $('#inputVerificationCode').val("");
    $('#verificationError').text("")
    $('#verificationSpinner').show();
    $('#verificationContainer').hide();
    $('#verificationButtonSpinner').hide();

    // Show the alert
    verificationAlert.show();

    var payload = {
        AuthValue: $('#inputEmailMfa').val(),
        AuthType: 1
    }

    $.ajax({
        url: "/api/SendCode",
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload),
        success: function (result) {

            // Show the UI elements
            $('#verificationSpinner').hide();
            $('#verificationContainer').show();

            if (result.error) {
                console.log("/SendCode")
                console.log("Error: " + error);
                $("#verificationError").text(error);
            }
        }
    });
}

function verifyCode() {

    $('#verificationButtonSpinner').show();

    var payload = {
        VerificationCode: $('#inputVerificationCode').val()
    }

    $.ajax({
        url: "/api/VerifyCode",
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload),
        success: function (result) {

            $('#verificationButtonSpinner').hide();

            if (!result.error) {

                if (result.validationPassed) {
                    verificationAlert.hide();
                }
                else {
                    $("#verificationError").text("Invalid code");
                }

            }
            else {
                $("#verificationError").text(result.error);
            }

        },
        // The function to execute when the request fails
        error: function (xhr, status, error) {
            console.log("/VerifyCode")
            console.log("Error: " + error);
            $("#verificationError").text(error);
        }
    });
}             