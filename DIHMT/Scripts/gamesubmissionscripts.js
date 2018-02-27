$(function () {
    var maxFields = 10; //maximum input boxes allowed
    var wrapper = $(".input-fields-wrap"); //Fields wrapper

    var x = 1; //initial text box count

    $(".add-field-element").click(function (e) { //on add input button click
        e.preventDefault();
        if (x < maxFields) { //max input box allowed
            x++; //text box increment
            $(wrapper).append('<div><input type="text" name="Links" maxlength="500" /> <a href="#" class="remove_field">[-]</a></div>'); //add input box
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