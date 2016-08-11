$(document).ready(function () {
    $("tr.selectable-row").click(function () {
        $(this).addClass("success data-selected");
        $(this).siblings("tr").removeClass("success data-selected");
    });
});