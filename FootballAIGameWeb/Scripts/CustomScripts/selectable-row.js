var selectableRowClickHandler = function () {
    $(this).toggleClass("success");
    if ($(this).is("[data-selected]")) {
        $(this).removeAttr("data-selected");
    } else {
        $(this).attr("data-selected", "");
    }
    $(this).siblings("tr").removeAttr("data-selected");
    $(this).siblings("tr").removeClass("success");
};

$(document)
    .ready(function () {
        /* - works for dynamically created elements as well 
           - mousedown instread of click, because while mouse is 
            being pressed, the control might change and event won't 
            fire  */
        $(document).on("mousedown", ".selectable-row", selectableRowClickHandler);
    });