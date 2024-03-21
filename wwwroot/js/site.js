// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function showGraphAPI(){
    $("#stepNavigatorContainer").css('visibility', 'hidden');
    $("#showGraphAPI").hide();
    $(".bs-stepper").hide();
    $("#showEntraAdminCenter").show();
    $("#GraphApiContent").show();
    

}

function showEntraAdminCenter(){
    $("#stepNavigatorContainer").css('visibility', 'visible');;
    $("#showGraphAPI").show();
    $(".bs-stepper").show();
    $("#showEntraAdminCenter").hide();
    $("#GraphApiContent").hide();
}
