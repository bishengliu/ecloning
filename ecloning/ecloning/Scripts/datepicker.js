$(function () { // will trigger when the document is ready
    $('.datepicker1').datepicker({
        format: 'dd-M-yyyy'
    }); //Initialise any date pickers
    $('.datepicker2').datepicker({
        format: 'dd-M-yyyy'
    }); //Initialise any date pickers
    $(".datepicker1").on("dp.change", function (e) {
        $('.datepicker2').data("DatePicker").minDate(e.date);
    });
    $(".datepicker2").on("dp.change", function (e) {
        $('.datepicker1').data("DatePicker").maxDate(e.date);
    });
    //$("#datepicker1").on("dp.change", function (e) {
    //    $('#datepicker2').data("DatePicker").minDate(e.date);
    //});
    //$("#datepicker2").on("dp.change", function (e) {
    //    $('#datepicker1').data("DatePicker").maxDate(e.date);
    //});
});