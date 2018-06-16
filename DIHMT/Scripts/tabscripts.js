$(function() {
    $("#tabs").tabs();
    var tabTitle = "Secondary tags";
    var tabCounter = 2;
    var platforms = [];

    $(".platform-id").each(function () {
        platforms.push($(this).val());
    });
});
