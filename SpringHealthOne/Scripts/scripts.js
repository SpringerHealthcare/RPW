function showhide(id) {
    var e = document.getElementById("expand-" + id);

    if (e.style.display == 'block') {
        e.style.display = 'none';
    }
    else {
        e.style.display = 'block';

    }
}


$(".form-inline input:checkbox").click(function () {
    if ($(".form-inline input:checkbox:checked").length > 0) {
        $("#favquote").fadeIn();
        $("#favfav").fadeIn();

    }
    else {
        $("#favquote").fadeOut();
        $("#favfav").fadeOut();

    }
});

$("#savesearchbtn").click(function () {
    $("#savesearchform").toggle();
    $("#savesearchbtn").toggle();
});
$(".save").click(function () {
    $("#savesearchform").toggle();
    $("#savesearchbtn").toggle();
});
$("#searchbutton").click(function () {
    $("#loading").fadeIn("fast");
});

$('#freetext').click(function () {
    $(this).css('background', 'url() #ffffff');
});

$("#forgottenlogin").click(function () {
    $('<div></div>').dialog({
        modal: true,
        open: function () {
            $(this).load("/User/ForgottenPassword");
        },
        close: function (event, ui) {
            $(this).remove();
        },
        height: 400,
        width: 600,
        title: 'Forgotten Login'
    });
});


