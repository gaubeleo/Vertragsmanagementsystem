$(document).ready(function () {

    //Fill Dropdown with availible Contract Categories
    $.ajax({
        type: "GET",
        url: "/Contract/ContractCategories",
        datatype: "Json",
        success: function (data) {
            $.each(data, function (index, value) {
                $('#contract_categoryID').append('<option value="' + value.ID + '">' + value.name + '</option>');
            });
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
                    $('#contract_subcategoryID').append('<option value="">Unterkategorie wählen</option>');
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
                }
            });
        }
    });

    //Define Datepickers
    $('#datetimepicker').datetimepicker({
        format: 'YYYY-MM-DD',
        showTodayButton: true,
        toolbarPlacement: 'top',
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