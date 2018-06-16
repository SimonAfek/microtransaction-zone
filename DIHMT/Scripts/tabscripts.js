$(function () {
    // Variables
    var tabTitle = "Secondary tags";
    var tabCounter = 2;
    var platforms = [];

    // Initialize tabs
    $("#tabs").tabs();


    // Populate platform list
    $(".platform-id").each(function () {
        platforms.push($(this).val());
    });
});
