$(function () {
    // Variables
    var tabTitle = "Secondary tags";
    var index = 0;
    var tabCounter = 2;
    var platforms = [];

    // Initialize tabs
    var tabs = $("#tabs").tabs();
    
    // Populate platform list
    $(".platform-id").each(function () {
        platforms.push($(this).val());
    });

    $("#tagset-addtab").click(function (e) {
        e.preventDefault();
        e.stopPropagation();

        var postData = {
            Index: index++,
            TabIndex: tabCounter++,
            Platforms: platforms
        };

        var newTabHeader = '<li><a href="#tabs-' + tabCounter +'">Additional tags: '+ index +'</a></li>';

        $.ajax({
            type: "POST",
            url: "/Game/Tab",
            data: postData,
            success: function (data) {
                tabs.find(".ui-tabs-nav").append(newTabHeader);
                tabs.append(data);
                tabs.tabs("refresh");
            },
            error: function (data) {
                tabCounter--;
                index--;
                console.log(data);
            }
        });
    });
});
