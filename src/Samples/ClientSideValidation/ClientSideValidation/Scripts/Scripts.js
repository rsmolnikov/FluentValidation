$(function () {
    // Prevent dynamic validation.  Only allow eager validation.
    $.validator.setDefaults({
        onfocusout: function (element) {
        },
        onkeyup: function (element) {
        }
    });
   
    $('form').clientValidation();
});