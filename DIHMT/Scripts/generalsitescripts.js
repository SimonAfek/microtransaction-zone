$(function () {
    var submissionform = $("#submissionform");

    if (typeof submissionform.areYouSure === "function") {
        submissionform.areYouSure();
    }

    submissionform.submit(function (e) {
        e.preventDefault();

        $(".submission-form-submit-button").prop({ disabled: true });
        $(".successmessage").text("");
        $(".failuremessage").text("");

        $.ajax({
            type: submissionform.attr("method"),
            url: submissionform.attr("action"),
            data: submissionform.serialize(),
            success: function (data) {
                $(".successmessage").text(data);
            },
            error: function (data) {
                if (data.status === 400) {
                    $(".failuremessage").text(data.responseText.replace(/^"(.*)"$/, "$1").replace(/u0027/g, "\'").replace(/\\/g, ""));
                } else {
                    $(".failuremessage").text("The server experienced an error while trying to process your submission. Sorry about that - please try again.");
                }

                if (typeof grecaptcha.reset === "function") {
                    grecaptcha.reset();
                }
                
                $(".submission-form-submit-button").prop({ disabled: false });
            }
        });
    });
});

$(function () {
    // Submission form swapping
    
    $(".submission-form-edit-button").click(function () {
        $("#submissionform").show();
        $(".game-rating-box").hide();
    });

    $(".submission-form-cancel-button").click(function () {
        $("#submissionform").hide();
        $(".game-rating-box").show();
    });
});

$(function () {
    // https://www.sanwebe.com/2013/03/addremove-input-fields-dynamically-with-jquery
    var maxFields = 10; // Maximum input boxes allowed
    var wrapper = $(".input-fields-wrap"); // Fields wrapper
    var addButton = $(".add-field-element"); // Class name of "Add new input"-element

    var x = $(".links-input").length; // Initial text box count

    $(addButton).click(function (e) { // On add input button click
        e.preventDefault();
        if (x < maxFields) { // True unless the field count is at capacity
            x++; // Increment count of text boxes
            $(wrapper).append('<div class="wide-input-box d-inline-flex form-horizontal links-margin"><input type="text" name="Links" class="form-control wide-input links-input" maxlength="500" />&emsp;<button href="#" class="remove_field btn btn-primary">-</button></div >'); // Add input box with corresponding remove-button
        }
    });

    $(wrapper).on("click", ".remove_field", function (e) { //user click on remove text
        e.preventDefault();
        $(this).parent("div").remove();
        x--;
    });
});

$(function () {
    function syncSpotless() {
        if ($(".spotless-checkbox").is(":checked")) {
            $(".monetization-input").prop({ disabled: true, checked: false });
            $(".hide-when-spotless").hide();
        } else {
            $(".monetization-input").prop({ disabled: false });
            $(".hide-when-spotless").show();
        }
    }

    $(".spotless-checkbox").click(syncSpotless);
    syncSpotless();
});

$(function() {
    $(".advanced-search-spotless-radio").click(function() {
        if ($(this).val() === "3") { // "Spotless"
            $(".advanced-search-blockflags-checkbox").prop({ checked: false });
            $("#blockflagsdiv").hide();
        } else {
            $("#blockflagsdiv").show();
        }
    });
});

function captchaComplete() {
    $(".submission-form-submit-button").prop({ disabled: false });
}
