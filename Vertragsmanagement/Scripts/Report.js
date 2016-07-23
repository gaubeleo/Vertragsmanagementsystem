$(document).ready(function () {
    //Fill Dropdown with availible Contract Categories
    $.ajax({
        type: "GET",
        url: "/Contract/ContractCategories",
        datatype: "Json",
        success: function (data) {
            $.each(data, function (index, value) {
                    $('#dropdowncategory').append('<option value="' + value.ID + '">' + value.name + '</option>');
                });
            $("#dropdownsubcategory").prop("disabled", true);
        }
    });

    //Fill Dropdown with the correct Contract Sub Categories
    $('#dropdowncategory').change(function () {
        $('#dropdownsubcategory').empty();
        if ($('#dropdowncategory').val() === "") {
            $('#dropdownsubcategory').append('<option value = "" >Unterkategorie wählen</option>');
            $("#dropdownsubcategory").prop("disabled", true);
        }
        else {
            $('#dropdownsubcategory').empty();
            $.ajax({
                type: "POST",
                url: "/Contract/ContractSubcategories",
                datatype: "Json",
                data: { contractID: $('#dropdowncategory').val() },
                success: function (data) {
                    var i = 0;
                    $('#dropdownsubcategory').append('<option value="">Unterkategorie wählen</option>');
                    $.each(data,
                        function (index, value) {
                            console.log(value);
                            $('#dropdownsubcategory').append('<option value="' + value.ID + '">' + value.name + '</option>');
                            i += 1;
                        });
                    $('select[dropdownsubcategory] option:eq(2)').attr('selected', 'selected');
                    if (i === 0) {
                        $("#dropdownsubcategory").prop("disabled", true);
                    } else {
                        $("#dropdownsubcategory").prop("disabled", false);
                        $('select[dropdownsubcategory] option:eq(1)').attr('selected', 'selected');
                    }
                }
            });
        }
    });

    //Create current date for file Export
    var currentDate = new Date();
    var d = currentDate.getFullYear() + "-" + ('0' + (currentDate.getMonth() + 1)).slice(-2) + "-" + ('0' + currentDate.getDate()).slice(-2);
    
    //Define DataTable for ReportMandantsTable
    var reportMandantsTable = $('#ReportMandantsTable').DataTable({
        dom: "<'row'<'col-sm-3'l><'col-sm-6 text-center'B><'col-sm-3'f>>" +
            "<'row'<'col-sm-12'tr>>" +
            "<'row'<'col-sm-2'i>>" +
            "<'row'<'col-sm-10 pull-right'p>>",
        lengthMenu: [7, 10, 25, 50],
        pageLength: 7,
        colReorder: true,
        buttons: [
            {
                extend: 'colvis',
                text: 'Filter',
                postfixButtons: ['colvisRestore'],
                collectionLayout: 'two-column'
            },
            {
                extend: 'csv',
                exportOptions: {
                    columns: ':not(.all)'
                },
                filename: d + '--*'
            },
            {
                extend: 'excel',
                exportOptions: {
                    columns: ':visible:not(.all)'
                },
                filename: d + '--*'
            },
            {
                extend: 'pdf',
                exportOptions: {
                    columns: ':visible:not(.all)'
                },
                orientation: 'landscape',
                pageSize: 'LEGAL',
                filename: d + '--*'
            },
            {
                extend: 'print',
                exportOptions: {
                    columns: ':visible:not(.all)'
                },
                title: d + '--*'
            }
        ],
        language: {
            url: "//cdn.datatables.net/plug-ins/1.10.12/i18n/German.json",
            buttons: {
                colvis: 'Spalten auswählen',
                colvisRestore: 'Zurücksetzen',
                print: 'Drucken'
            }
        }
    });
});