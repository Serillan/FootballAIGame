$(document).ready(function () {
    $("tr.selectable-row").click(function () {
        $(this).toggleClass("success");
        if ($(this).is("[data-selected]")) {
            $(this).removeAttr("data-selected");
        } else {
            $(this).attr("data-selected", "");
        }
        $(this).siblings("tr").removeAttr("data-selected");
        $(this).siblings("tr").removeClass("success");
    });
});