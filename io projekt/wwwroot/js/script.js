$(document).ready(function () {
    
    $(".header-dropdown-trigger").click(function () {
        $(this).toggleClass("active");
        $(".header-dropdown").toggleClass("expand");
    });

    $(".header-dropdown li").click(function () {
        $(".header-dropdown-trigger").removeClass("active");
        $(".header-dropdown").removeClass("expand");
    });

    $(".comment-trigger").click(function () {
        $(this).parent().parent().toggleClass("post--commenting");
    });

    $(".button--flag").click(function () {
        $(this).parent().parent().toggleClass("post--commenting");
    });

    $(".button--confirm").click(function () {
        $(this).parent().parent().parent().parent().parent().toggleClass("post--commenting");
    });

    $(".button.cancel").click(function () {
        $(this).parent().parent().parent().parent().parent().toggleClass("post--commenting");
    });
});
