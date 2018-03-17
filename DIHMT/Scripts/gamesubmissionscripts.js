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
            $(wrapper).append('<div><input type="text" name="Links" size="500" class="wide-input links-input" maxlength="500" /> <a href="#" class="remove_field">[-]</a></div>'); // Add input box with corresponding remove-button
        }
    });

    $(wrapper).on("click", ".remove_field", function (e) { //user click on remove text
        e.preventDefault();
        $(this).parent("div").remove();
        x--;
    });
});

$(function () {
    $(".spotless-checkbox").click(function () {
        if ($(this).is(":checked")) {
            $(".monetization-input").prop({ disabled: true, checked: false });
            $(".check-when-spotless").prop({ checked: true });
        } else {
            $(".monetization-input").prop({ disabled: false });
        }
    });
});
