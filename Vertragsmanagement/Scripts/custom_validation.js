
/// <reference path="jquery-1.4.4-vsdoc.js" />
/// <reference path="jquery.validate-vsdoc.js" />
/// <reference path="jquery.validate.unobtrusive.js" />

// Calculate Contract-endDate using startDate and minimum-runtime of the contract
// Disable button ist the required fields are not filled in yet
(function ($) {
    if ($("#contract_startDate").val() === "" || $('#contract_earliestNoticePeriod').val() === "")
        $("#calc_endDate_btn").prop("disabled", true);

    $("#contract_startDate").on("change", function () {
        if ($("#contract_startDate").val() === "" || $('#contract_earliestNoticePeriod').val() === ""){
            $("#calc_endDate_btn").prop("disabled", true);
            return;
        }
        $("#calc_endDate_btn").prop("disabled", false);
    });

    $("#contract_earliestNoticePeriod").on("change", function () {
        if ($("#contract_startDate").val() === "" || $('#contract_earliestNoticePeriod').val() === "") {
            $("#calc_endDate_btn").prop("disabled", true);
            return;
        }
        $("#calc_endDate_btn").prop("disabled", false);
    });

    $("#calc_endDate_btn").click(function () {
        var start_date = $("#contract_startDate").val()
        var min_runtime = $('#contract_earliestNoticePeriod').val()

        if (start_date === "" || min_runtime === "")
            return;

        var end_date = $.change_date(start_date, 0, parseInt(min_runtime), 0);
        $("#contract_endDate").val(end_date);
        $("#contract_endDate").valid();
        $("#contract_endDate").trigger("change");
    });
})(jQuery);


//Show Validation-Summery-Title on the bottom of the page after some an Error
(function ($) {
    $('#validation-summery').on("change", function () {
        $('#validation-summery-title').removeClass("invisible");
    });

    var originalAddClassMethod = jQuery.fn.addClass;

    jQuery.fn.addClass = function () {
        var result = originalAddClassMethod.apply(this, arguments);

        if (arguments[0] == "validation-summary-errors")
            $('#validation-summery').trigger("change");

        return result;
    };

})(jQuery);

//client-side validation for depandant-required inputs
(function ($) {
    $.validator.unobtrusive.adapters.add("alwaysrequired", {}, function (options) {
        options.rules['require'] = true;
        options.messages['require'] = options.message;
        }
    );

    $.validator.unobtrusive.adapters.add("requiredifdispatcher", {}, function (options) {
        if ($("#contract_startDate").attr("signer") == "False") {
            options.rules['require'] = true;
            options.messages['require'] = options.message;
        }
    });

    jQuery.validator.addMethod('require', function (val, element, params) {
        if (!val || val === "") {
            return false;
        }
        return true;
    }, '');

})(jQuery);


// Easy-Datepicker client-side validation
(function ($) {

    $.validator.unobtrusive.adapters.addSingleVal("afterdate", "other");
    $.validator.addMethod("afterdate",
        function (val, element, other) {

            var modelPrefix = element.name.substr(0, element.name.lastIndexOf(".") + 1)
            var otherVal = $("[name=\"" + modelPrefix + other + "\"]").val();

            if (!val) {
                return true;
            }

            if (otherVal) {
                if (Date.parse(val) > Date.parse(otherVal)) {
                    return true;
                }
            }
            return false;
        }
    );

})(jQuery);


// Easy-datepicker client-side validation
(function ($) {

    $.validator.unobtrusive.adapters.addSingleVal("beforedate", "other");
    $.validator.addMethod("beforedate",
        function (val, element, other) {

            var modelPrefix = element.name.substr(0, element.name.lastIndexOf(".") + 1)
            var otherVal = $("[name=\"" + modelPrefix + other + "\"]").val();

            if (!val) {
                return true;
            }

            if (otherVal) {
                if (Date.parse(val) < Date.parse(otherVal)) {
                    return true;
                }
            }
            return false;
        }
    );

})(jQuery);

// Date manipulation
(function ($) {
    // get date in format yyyy-mm-dd
    $.format_date = function (date) {
        var d = new Date(date),
            month = '' + (d.getMonth() + 1),
            day = '' + d.getDate(),
            year = d.getFullYear();

        if (month.length < 2) month = '0' + month;
        if (day.length < 2) day = '0' + day;

        return [year, month, day].join('-');
    }

    // add years, months, or days to a date and return the new date
    $.change_date = function (org_date, years, months, days) {
        var date = new Date(org_date);

        date.setYear(1900 + date.getYear() + years);
        date.setMonth(date.getMonth() + months);
        date.setDate(date.getDate() + days);

        return $.format_date(date);
    };

    // Eventlistener for Easy-Datepicker
    $('#easy-reminding-picker').on("change", function () {
        var new_date = null;
        var end_date = $("#contract_endDate").val();

        if ($(this).val() == "custom") {
            return;
        }

        if ($(this).val() == "") {
            $("#contract_remindingDate").val("");
            return;
        }
        if (end_date !== "") {
            if ($(this).val() == "1_week_before_endDate") {
                var new_date = $.change_date(end_date, 0, 0, -7);
            }
            else if ($(this).val() == "1_month_before_endDate") {
                var new_date = $.change_date(end_date, 0, -1, 0);
            }
            else if ($(this).val() == "3_months_before_endDate") {
                var new_date = $.change_date(end_date, 0, -3, 0);
            }
            else if ($(this).val() == "6_months_before_endDate") {
                var new_date = $.change_date(end_date, 0, -6, 0);
            }
        }
        if (new_date) {
            // validate dependant dates
            $("#contract_remindingDate").val(new_date);
            $("#contract_remindingDate").valid();
            $('#easy-escalation-picker').trigger("change");
            $('#contract_escalationDate').valid();
        }
    });

    $('#easy-escalation-picker').on("change", function () {
        var new_date = null;

        var end_date = $("#contract_endDate").val();
        var reminding_date = $("#contract_remindingDate").val();

        if ($(this).val() == "custom") {
            return;
        }

        if ($(this).val() == "") {
            $("#contract_escalationDate").val("");
            return;
        }
        if (reminding_date !== "") {
            if ($(this).val() == "1_week_after_remindingDate") {
                var new_date = $.change_date(reminding_date, 0, 0, 7);
            }
            else if ($(this).val() == "1_month_after_remindingDate") {
                var new_date = $.change_date(reminding_date, 0, 1, 0);
            }
        }
        if (end_date !== "") {
            if ($(this).val() == "1_week_before_endDate") {
                var new_date = $.change_date(end_date, 0, 0, -7);
            }
            else if ($(this).val() == "1_month_before_endDate") {
                var new_date = $.change_date(end_date, 0, -1, 0);
            }
        }
        if (new_date) {
            // validate dependant dates
            $("#contract_escalationDate").val(new_date);
            $("#contract_escalationDate").valid();
        }
    });
})(jQuery);

(function ($) {
    // fire change events after datetimepicker-change
    $('#datetimepicker').on("dp.change", function () {
        $("#contract_startDate").trigger("change");
    });

    $('#datetimepicker1').on("dp.change", function () {
        $("#contract_endDate").trigger("change");
    });

    $('#datetimepicker2').on("dp.change", function () {
        $("#contract_remindingDate").trigger("change");
    });

    $('#datetimepicker3').on("dp.change", function () {
        $("#contract_escalationDate").trigger("change");
    });

    $('#datetimepicker4').on("dp.change", function () {
        $("#contract_paymentBegin").trigger("change");
    });


})(jQuery);

(function ($) {
    // validate dependant dates
    $("#contract_startDate").on("change", function () {
        if ($("#contract_endDate").val() != "")
            $("#contract_endDate").valid();
        if ($("#contract_remindingDate").val() != "")
            $("#contract_remindingDate").valid();
        if ($("#contract_escalationDate").val() != "")
            $("#contract_escalationDate").valid();
    });
    $("#contract_endDate").on("change", function () {
        if ($("#contract_remindingDate").val() != "")
            $("#contract_remindingDate").valid();
        if ($("#contract_escalationDate").val() != "")
            $("#contract_escalationDate").valid();

        //fire validation for dependant dates
        $("#easy-escalation-picker").trigger("change");
        $("#easy-reminding-picker").trigger("change");
    });

    $("#contract_remindingDate").on("change", function () {
        if ($("#contract_escalationDate").val() != "")
            $("#contract_escalationDate").valid();

        //fire validation for dependant dates
        $("#easy-escalation-picker").trigger("change");
        $("#easy-reminding-picker").val("custom");
    });

    $("#contract_escalationDate").on("change", function () {
        $("#easy-escalation-picker").val("custom");
    });

    // remove validation for empty input fields
    $('.form-control').on("change", function () {
        if ($(this).val() == "") {
            $(this).valid();
            $(this).removeClass("valid");
        }
    });
})(jQuery);