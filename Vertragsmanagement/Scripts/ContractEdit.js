$(document).ready(function () {
    //Get submitted Category and Subcategory of Contract by hidden field in view
    var categoryValues = document.getElementById('Categorys').value;
    if (categoryValues === "")
        var categorys = "";
    else
        var categorys = categoryValues.split(",");

    //Fill Dropdown with availible Contract Categories
    $.ajax({
        type: "GET",
        url: "/Contract/ContractCategories",
        datatype: "Json",
        success: function(data) {
            $.each(data,
                function(index, value) {
                    $('#contract_categoryID').append('<option value="' + value.ID + '">' + value.name + '</option>');
                });
            $('#contract_categoryID option[value=' + categorys[0] + ']').attr('selected', true);

            if (categorys === "" || categorys[1] === "") {
                $("#contract_subcategoryID").prop("disabled", true);
            }
            else {
                $.ajax({
                    type: "POST",
                    url: "/Contract/ContractSubcategories",
                    datatype: "Json",
                    data: { contractID: $('#contract_categoryID').val() },
                    success: function (data) {
                        $.each(data, function (index, value) {
                            $('#contract_subcategoryID').append('<option value="' + value.ID + '">' + value.name + '</option>');
                        });
                        $('#contract_subcategoryID option[value=' + categorys[1] + ']').attr('selected', true);
                    }
                });
            }
        },
        error: function (request, status, error) {
            $("#contract_subcategoryID").prop("disabled", true);
        }
    });

    //Fill Dropdown with the correct Contract Sub Categories
    $('#contract_categoryID').change(function () {
        $('#contract_subcategoryID').empty();
        if ($('#contract_categoryID').val() === "") {
            $('#contract_subcategoryID').append('<option value = "" >Unterkategorie wählen</option>');
            $("#contract_subcategoryID").prop("disabled", true);
        }
        else {
            $.ajax({
                type: "POST",
                url: "/Contract/ContractSubcategories",
                datatype: "Json",
                data: { contractID: $('#contract_categoryID').val() },
                success: function (data) {
                    var i = 0;
                    $('#contract_subcategoryID').append('<option value = "" >Unterkategorie wählen</option>');
                    $.each(data, function (index, value) {
                        $('#contract_subcategoryID').append('<option value="' + value.ID + '">' + value.name + '</option>');
                        i += 1;
                    });
                    $('select[contract_subcategoryID] option:eq(2)').attr('selected', 'selected');
                    if (i === 0) {
                        $("#contract_subcategoryID").prop("disabled", true);
                    } else {
                        $("#contract_subcategoryID").prop("disabled", false);
                        $('select[contract_subcategoryID] option:eq(1)').attr('selected', 'selected');
                    }
                }//,
                //error: function (request, status, error) {
                //    console.log($('#contract_categoryID').val());
                //    $('#contract_subcategoryID').append('<option value = "" >Unterkategorie wählen</option>');
                //    $("#contract_subcategoryID").prop("disabled", true);
                //}
            });
        }
    });

    //Get submitted Dates of Contract by hidden field in view
    var values = document.getElementById('Dates').value;
    var dates = values.split(",");

    //Convert inputted date of view to specific format
    var convertdate = function (modelelement) {
        //console.log(modelelement);
        if (modelelement === undefined || modelelement === "")
            return "";
        var string = modelelement.split(" ")[0];
        var datum = string.split(".");
        return datum[2] + "-" + datum[1] + "-" + datum[0];
    };

    //Define Datepickers
    $('#datetimepicker').datetimepicker({
        format: 'YYYY-MM-DD',
        showTodayButton: true,
        toolbarPlacement: 'top',
        defaultDate: convertdate(dates[0]),
        showClear: true,
        showClose: true,
        tooltips: {
            today: 'Heutiges Datum',
            clear: 'Zurücksetzen',
            close: 'Schließen',
            selectMonth: 'Monat wählen',
            prevMonth: 'Vorheriger Monat',
            nextMonth: 'Nächster Monat',
            selectYear: 'Jahr wählen',
            prevYear: 'Vorheriges Jahr',
            nextYear: 'Nächstes Jahr',
            selectDecade: 'Jahrzehnt wählen',
            prevDecade: 'Vorheriges Jahrzent',
            nextDecade: 'Nächstes Jahrzent',
            prevCentury: 'Vorheriges Jahrhundert',
            nextCentury: 'Nächstes Jahrhundert'
        }
    });

    $('#datetimepicker1').datetimepicker({
        format: 'YYYY-MM-DD',
        showTodayButton: true,
        toolbarPlacement: 'top',
        defaultDate: convertdate(dates[1]),
        showClear: true,
        showClose: true,
        tooltips: {
            today: 'Heutiges Datum',
            clear: 'Zurücksetzen',
            close: 'Schließen',
            selectMonth: 'Monat wählen',
            prevMonth: 'Vorheriger Monat',
            nextMonth: 'Nächster Monat',
            selectYear: 'Jahr wählen',
            prevYear: 'Vorheriges Jahr',
            nextYear: 'Nächstes Jahr',
            selectDecade: 'Jahrzehnt wählen',
            prevDecade: 'Vorheriges Jahrzent',
            nextDecade: 'Nächstes Jahrzent',
            prevCentury: 'Vorheriges Jahrhundert',
            nextCentury: 'Nächstes Jahrhundert'
        }
    });

    $('#datetimepicker2').datetimepicker({
        format: 'YYYY-MM-DD',
        showTodayButton: true,
        toolbarPlacement: 'top',
        defaultDate: convertdate(dates[2]),
        showClear: true,
        showClose: true,
        tooltips: {
            today: 'Heutiges Datum',
            clear: 'Zurücksetzen',
            close: 'Schließen',
            selectMonth: 'Monat wählen',
            prevMonth: 'Vorheriger Monat',
            nextMonth: 'Nächster Monat',
            selectYear: 'Jahr wählen',
            prevYear: 'Vorheriges Jahr',
            nextYear: 'Nächstes Jahr',
            selectDecade: 'Jahrzehnt wählen',
            prevDecade: 'Vorheriges Jahrzent',
            nextDecade: 'Nächstes Jahrzent',
            prevCentury: 'Vorheriges Jahrhundert',
            nextCentury: 'Nächstes Jahrhundert'
        }
    });

    $('#datetimepicker3').datetimepicker({
        format: 'YYYY-MM-DD',
        showTodayButton: true,
        toolbarPlacement: 'top',
        defaultDate: convertdate(dates[3]),
        showClear: true,
        showClose: true,
        tooltips: {
            today: 'Heutiges Datum',
            clear: 'Zurücksetzen',
            close: 'Schließen',
            selectMonth: 'Monat wählen',
            prevMonth: 'Vorheriger Monat',
            nextMonth: 'Nächster Monat',
            selectYear: 'Jahr wählen',
            prevYear: 'Vorheriges Jahr',
            nextYear: 'Nächstes Jahr',
            selectDecade: 'Jahrzehnt wählen',
            prevDecade: 'Vorheriges Jahrzent',
            nextDecade: 'Nächstes Jahrzent',
            prevCentury: 'Vorheriges Jahrhundert',
            nextCentury: 'Nächstes Jahrhundert'
        }
    });

    $('#datetimepicker4').datetimepicker({
        format: 'YYYY-MM-DD',
        showTodayButton: true,
        toolbarPlacement: 'top',
        defaultDate: convertdate(dates[4]),
        showClear: true,
        showClose: true,
        tooltips: {
            today: 'Heutiges Datum',
            clear: 'Zurücksetzen',
            close: 'Schließen',
            selectMonth: 'Monat wählen',
            prevMonth: 'Vorheriger Monat',
            nextMonth: 'Nächster Monat',
            selectYear: 'Jahr wählen',
            prevYear: 'Vorheriges Jahr',
            nextYear: 'Nächstes Jahr',
            selectDecade: 'Jahrzehnt wählen',
            prevDecade: 'Vorheriges Jahrzent',
            nextDecade: 'Nächstes Jahrzent',
            prevCentury: 'Vorheriges Jahrhundert',
            nextCentury: 'Nächstes Jahrhundert'
        }
    });
});