$(document).ready(function () {
    /* - works for dynamically created elements as well 
       - mousedown instread of click, because while mouse is 
       being pressed, the control might change and event won't 
       fire  */
    $(document).on("mousedown", "tr.clickable-row", function () {
        window.location = $(this).data("href");
    });
});