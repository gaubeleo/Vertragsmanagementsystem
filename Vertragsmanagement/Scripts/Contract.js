//Columns of DataTable
columns = [
        "intnr", "extnr", "bez",
        "ges", "kos", "zim", "kit", "mim", "avim", "mwst",
        "contract_partnerIs", "bem", "dropdowncategory", "dropdownsubcategory",
        "contract_species", "contract_signer", "contract_personInCharge",
        "contract_mappedDepartment",
        "contract_observingDepartment", "contract_partner_name", "zbgvon", "vbvon",
        "ersvon", "kdatvon", "vevon", "ervon", "esvon", "loc", "escalated", "reminded",
        "noticeReminded", "contract_otherFeatures", "costcentre_describtion",
        "contract_mappedDepartment_mandant_name", "status", "Aktionen"
];

$(document).ready(function () {
    //Fill Dropdown with availible Contract Categories
    $.ajax({
        type: "GET",
        url: "/Contract/ContractCategories",
        datatype: "Json",
        success: function (data) {
            $.each(data,
                function (index, value) {
                    $('#dropdowncategory').append('<option value="' + value.ID + '">' + value.name + '</option>');
                });
            $("#dropdownsubcategory").prop("disabled", true);
        }
    });

    //Create current date for file Export
    var currentDate = new Date();
    var d = currentDate.getFullYear() + "-" + ('0' + (currentDate.getMonth() + 1)).slice(-2) + "-" + ('0' + currentDate.getDate()).slice(-2);

    //Control Glyphicon of Filter Dropdown
    $('#collapse1').on('shown.bs.collapse', function () {
        $("#customfilter").removeClass("glyphicon-chevron-down").addClass("glyphicon-chevron-up");
    });

    $('#collapse1').on('hidden.bs.collapse', function () {
        $("#customfilter").removeClass("glyphicon-chevron-up").addClass("glyphicon-chevron-down");
    });
    
    //Configure Popover Helper
    $('[data-toggle="popover"]').popover({
        trigger: 'hover',
        placement: "auto bottom",
        viewport: {selector: "tbody", padding: 0}
    });

    //Define DataTable for ContractsTable
    var contracts = $('#ContractsTable').DataTable({
        dom: "<'row'<'col-sm-3'l><'col-sm-6 text-center'B><'col-sm-3'f>>" +
                "<'row'<'col-sm-12'tr>>" +
                "<'row'<'col-sm-2'i>>" +
                "<'row'<'col-sm-10 pull-right'p>>",
        lengthMenu: [7, 15, 25, 50],
        pageLength: 7,
        scrollX: true,
        scrollY: true,
        colReorder: true,
        buttons: [
            {
                extend: 'collection',
                text: 'Filter',
                autoClose: true,
                collectionLayout: 'three-column',
                buttons: [
                    {
                        extend: 'colvis',
                        postfixButtons: ['colvisRestore'],
                        collectionLayout: 'three-column',
                        columns:
                            '0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,31,32,33,34,35'
                    },
                    {
                        text: 'Standardansicht',
                        action: function (e, dt, node, config) {
                            contracts.column(1).visible(true);
                            contracts.column(2).visible(true);
                            contracts.column(3).visible(true);
                            contracts.column(12).visible(true);
                            contracts.column(14).visible(true);
                            contracts.column(15).visible(true);
                            contracts.column(18).visible(true);
                            contracts.column(19).visible(true);
                            contracts.column(21).visible(true);
                            contracts.column(34).visible(true);
                            contracts.column(35).visible(true);

                            contracts.column(0).visible(false);
                            contracts.column(4).visible(false);
                            contracts.column(5).visible(false);
                            contracts.column(6).visible(false);
                            contracts.column(7).visible(false);
                            contracts.column(8).visible(false);
                            contracts.column(9).visible(false);
                            contracts.column(10).visible(false);
                            contracts.column(11).visible(false);
                            contracts.column(13).visible(false);
                            contracts.column(16).visible(false);
                            contracts.column(17).visible(false);
                            contracts.column(20).visible(false);
                            contracts.column(22).visible(false);
                            contracts.column(23).visible(false);
                            contracts.column(24).visible(false);
                            contracts.column(25).visible(false);
                            contracts.column(26).visible(false);
                            contracts.column(27).visible(false);
                            contracts.column(31).visible(false);
                            contracts.column(32).visible(false);
                            contracts.column(33).visible(false);
                        }
                    },
                    {
                        text: 'Erweiterte Ansicht',
                        action: function (e, dt, node, config) {
                            contracts.column(1).visible(true);
                            contracts.column(2).visible(true);
                            contracts.column(3).visible(true);
                            contracts.column(12).visible(true);
                            contracts.column(14).visible(true);
                            contracts.column(15).visible(true);
                            contracts.column(18).visible(true);
                            contracts.column(19).visible(true);
                            contracts.column(21).visible(true);
                            contracts.column(34).visible(true);
                            contracts.column(35).visible(true);

                            dt.column(0).visible(!dt.column(0).visible());
                            dt.column(4).visible(!dt.column(4).visible());
                            dt.column(5).visible(!dt.column(5).visible());
                            dt.column(6).visible(!dt.column(6).visible());
                            dt.column(7).visible(!dt.column(7).visible());
                            dt.column(8).visible(!dt.column(8).visible());
                            dt.column(9).visible(!dt.column(9).visible());
                            dt.column(10).visible(!dt.column(10).visible());
                            dt.column(11).visible(!dt.column(11).visible());
                            dt.column(13).visible(!dt.column(13).visible());
                            dt.column(16).visible(!dt.column(16).visible());
                            dt.column(17).visible(!dt.column(17).visible());
                            dt.column(20).visible(!dt.column(20).visible());
                            dt.column(22).visible(!dt.column(22).visible());
                            dt.column(23).visible(!dt.column(23).visible());
                            dt.column(24).visible(!dt.column(24).visible());
                            dt.column(25).visible(!dt.column(25).visible());
                            dt.column(26).visible(!dt.column(26).visible());
                            dt.column(27).visible(!dt.column(27).visible());
                            dt.column(31).visible(!dt.column(31).visible());
                            dt.column(32).visible(!dt.column(32).visible());
                            dt.column(33).visible(!dt.column(33).visible());
                        }
                    }
                ]
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
        },
        columnDefs: [
            {
                targets: [0],
                visible: false
            },
            {
                targets: [4],
                visible: false
            },
            {
                targets: [5],
                visible: false
            },
            {
                targets: [6],
                visible: false
            },
            {
                targets: [7],
                visible: false
            },
            {
                targets: [8],
                visible: false
            },
            {
                targets: [9],
                visible: false
            },
            {
                targets: [10],
                visible: false
            },
            {
                targets: [11],
                visible: false
            },
            {
                targets: [13],
                visible: false
            },
            {
                targets: [16],
                visible: false
            },
            {
                targets: [17],
                visible: false
            },
            {
                targets: [20],
                visible: false
            },
            {
                targets: [22],
                visible: false
            },
            {
                targets: [23],
                visible: false
            },
            {
                targets: [24],
                visible: false
            },
            {
                targets: [25],
                visible: false
            },
            {
                targets: [26],
                visible: false
            },
            {
                targets: [27],
                visible: false
            },
            {
                targets: [28],
                visible: false,
                orderable: false
            },
            {
                targets: [29],
                visible: false,
                searchable: false,
                orderable: false
            },
            {
                targets: [30],
                visible: false,
                searchable: false,
                orderable: false
            },
            {
                targets: [31],
                visible: false
            },
            {
                targets: [32],
                visible: false
            },
            {
                targets: [33],
                visible: false
            },
            {
                targets: [35],
                searchable: false,
                orderable: false
            }
        ]
    });

    //Custom search function for columns
    var search = function ($id, $column) {
        $('#' + $id).on('keyup change', function () {
            contracts
                .columns($column)
                .search(this.value)
                .draw();
        });
    };

    var searchdropdowntext = function ($id, $column) {
        $('#' + $id).on('keyup change', function () {
            if ($("#" + $id + " option:selected").val() !== "") {
                contracts
                    .columns($column)
                    .search($("#" + $id + " option:selected").text())
                    .draw();
            } else {
                contracts
                    .columns($column)
                    .search('')
                    .draw();
            }
            
        });
    };

    var searchdropdownvalue = function ($id, $column) {
        $('#' + $id).on('keyup change', function () {
            contracts
                .columns($column)
                .search($("#" + $id + " option:selected").val())
                .draw();
        });
    };

    var searchcascadingcategorydropdown = function($id, $column) {
        $('#' + $id).on('keyup change',
            function () {
                if ($id === 'dropdowncategory'){
                    if ($("#" + $id + " option:selected").text() === "Kategorie wählen") {
                        contracts
                            .columns($column)
                            .search('')
                            .draw();
                        contracts
                            .columns(13)
                            .search('')
                            .draw();
                        $("#dropdownsubcategory").prop("disabled", true);
                        contracts.column(13).visible(false);
                    } else {
                        contracts
                            .columns($column)
                            .search($("#" + $id + " option:selected").text())
                            .draw();
                        contracts
                            .columns(13)
                            .search('')
                            .draw();
                    }

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
                                    $('#dropdownsubcategory').append('<option value="' + value.categoryID + '">' + value.name + '</option>');
                                    i += 1;
                                });
                            $('select[dropdownsubcategory] option:eq(2)').attr('selected', 'selected');
                            if (i === 0) {
                                $("#dropdownsubcategory").prop("disabled", true);
                                contracts.column(13).visible(false);
                            } else {
                                contracts.column(13).visible(true);
                                $("#dropdownsubcategory").prop("disabled", false);
                                $('select[dropdownsubcategory] option:eq(1)').attr('selected', 'selected');
                            }
                        }
                    });
                } else if ($id === 'dropdownsubcategory') {
                    if ($("#" + $id + " option:selected").text() === "Unterkategorie wählen") {
                        contracts
                            .columns($column)
                            .search('')
                            .draw();
                    } else {
                        contracts
                            .columns($column)
                            .search($("#" + $id + " option:selected").text())
                            .draw();
                    }
                }
        });
    };

    //Search for Int. Vertragsnummer
    search('intnr', 0);

    //Search for Ext. Vertragsnummer
    search('extnr', 1);

    //Search for Bezeichnung
    search('bez', 2);

    //Search for Zahlungsintervall in Monaten
    search('zim', 5);

    //Search for Kündigungsfrist in Tagen
    search('kit', 6);

    //Search for Mindestlaufzeit in Monaten
    search('min', 7);

    //Search for Auto. Verlängerung in Monaten
    search('avim', 8);

    //Search for Mehrwertsteuer
    search('mwst', 9);

    //Search for Vertragspartner ist
    searchdropdowntext('contract_partnerIs', 10);

    //Search for Bemerkungen
    search('bem', 11);

    //Search for Kategorie
    searchcascadingcategorydropdown('dropdowncategory', 12);

    //Search for Unterkategorie
    searchcascadingcategorydropdown('dropdownsubcategory', 13);

    //Search for Art
    searchdropdowntext('contract_species', 14);

    //Search for Unterzeichner
    searchdropdowntext('contract_signer', 15);

    //Search for sachlicher Verantwortlicher
    searchdropdowntext('contract_personInCharge', 16);

    //Search for zugeordnete Abteilung
    searchdropdowntext('contract_mappedDepartment', 17);

    //Search for überwachende Abteilung
    searchdropdowntext('contract_observingDepartment', 18);

    //Search for Vertragspartner
    searchdropdowntext('contract_partner_name', 19);

    //Search for Standort
    search('loc', 27);

    //Search for Status
    searchdropdownvalue('status', 34);

    //Search for eskalierten Verträgen
    $(':checkbox').change(function () {
        contracts.column(28).visible(true);
        if ($(this).is(":checked")) {
            contracts
                .columns(28)
                .search(true)
                .draw();
        } else {
            contracts
                .columns(28)
                .search('')
                .draw();
        }
        contracts.column(28).visible(false);

        //Remove row highlighting or reset visibility of escalated contracts
        contracts.rows()
            .iterator('row',
                function (context, index) {
                    //alert($(this.row(index).node()).attr('class'));
                    if ($(this.row(index).node()).attr('class').indexOf('highlight2') > -1) {
                        $(this.row(index).node()).removeClass('highlight2');
                        $(this.row(index).node()).addClass('highlight');
                    } else if ($(this.row(index).node()).attr('class').indexOf('highlight') > -1) {
                        $(this.row(index).node()).removeClass('highlight');
                        $(this.row(index).node()).addClass('highlight2');
                    }
                });
    });

    //Search for Weiteren Merkmalen
    searchdropdowntext('contract_otherFeatures', 31);

    //Search for Kostenstelle
    searchdropdownvalue('costcentre_describtion', 32);

    //Search for Mandant
    searchdropdowntext('contract_mappedDepartment_mandant_name', 33);

    //Disable fields, when column is hidden
    $('#ContractsTable').on('column-visibility.dt', function (e, settings, column, state) {
        switch (column) {
            case 28:
                break;
            case 29:
                break;
            case 30:
                break;
            case 35:
                break;
            default:
                if (state === false) {
                    document.getElementById(columns[column]).disabled = true;
                    $("#" + columns[column]).find("input,button,textarea,select").prop("disabled", true);
                } else {
                    document.getElementById(columns[column]).disabled = false;
                    $("#" + columns[column]).find("input,button,textarea,select").prop("disabled", false);
                }
                yadcf.exResetFilters(contracts, [column]);
                $("#" + columns[column]).val("");
                contracts
                    .search('')
                    .columns(column)
                    .draw();
                break;
        }
    });

    var datepickerDefaults = {
        showTodayButton: true,
        toolbarPlacement: 'top',
        showClear: true,
        showClose: true,
        //daysOfWeekDisabled: [0, 6], //Deactivate Saturoda and Sunday
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
    };

    yadcf.init(contracts,
    [
        {
            column_number: 3,
            filter_container_id: 'ges',
            filter_type: 'range_number',
            filter_reset_button_text: false,
            ignore_char: ',00'
        },
        {
            column_number: 4,
            filter_container_id: 'kos',
            filter_type: 'range_number',
            filter_reset_button_text: false,
            ignore_char: ',00'
        },
        {
            column_number: 20,
            filter_container_id: 'zbgvon',
            locale: 'de',
            filter_type: 'range_date',
            datepicker_type: 'bootstrap-datetimepicker',
            date_format: 'YYYY-MM-DD',
            filter_reset_button_text: false,
            filter_plugin_options: datepickerDefaults
        },
        {
            column_number: 21,
            filter_container_id: 'vbvon',
            locale: 'de',
            filter_type: 'range_date',
            datepicker_type: 'bootstrap-datetimepicker',
            date_format: 'YYYY-MM-DD',
            filter_reset_button_text: false,
            filter_plugin_options: datepickerDefaults
        },
        {
            column_number: 22,
            filter_container_id: 'ersvon',
            locale: 'de',
            filter_type: 'range_date',
            datepicker_type: 'bootstrap-datetimepicker',
            date_format: 'YYYY-MM-DD',
            filter_reset_button_text: false,
            filter_plugin_options: datepickerDefaults
        },
        {
            column_number: 23,
            filter_container_id: 'kdatvon',
            locale: 'de',
            filter_type: 'range_date',
            datepicker_type: 'bootstrap-datetimepicker',
            date_format: 'YYYY-MM-DD',
            filter_reset_button_text: false,
            filter_plugin_options: datepickerDefaults
        },
        {
            column_number: 24,
            filter_container_id: 'vevon',
            locale: 'de',
            filter_type: 'range_date',
            datepicker_type: 'bootstrap-datetimepicker',
            date_format: 'YYYY-MM-DD',
            filter_reset_button_text: false,
            filter_plugin_options: datepickerDefaults
        },
        {
            column_number: 25,
            filter_container_id: 'ervon',
            locale: 'de',
            filter_type: 'range_date',
            datepicker_type: 'bootstrap-datetimepicker',
            date_format: 'YYYY-MM-DD',
            filter_reset_button_text: false,
            filter_plugin_options: datepickerDefaults
        },
        {
            column_number: 26,
            filter_container_id: 'esvon',
            locale: 'de',
            filter_type: 'range_date',
            datepicker_type: 'bootstrap-datetimepicker',
            date_format: 'YYYY-MM-DD',
            filter_reset_button_text: false,
            filter_plugin_options: datepickerDefaults
        }
    ]);

    //Reset all filters
    $('#reset').on('click', function () {
        $("#dropdownsubcategory").prop("disabled", true);
        yadcf.exResetAllFilters(contracts);
        contracts
            .search('')
            .columns()
            .search('')
            .draw();
        contracts.rows().iterator('row', function (context, index) {
            if ($(this.row(index).node()).attr('class').indexOf('highlight2') > -1) {
                $(this.row(index).node()).removeClass('highlight2');
                $(this.row(index).node()).addClass('highlight');
            }
        });
    });

    //Filter Contract List via Links from Übersicht
    $(window).load(function () {
        var preselect = $("#status").attr("preselect");
        switch (preselect) {
            case "":
                break;
            case "eskaliert":
                $('[value="esk"').prop('checked', true);
                alert("Benutzerdefinierter Filter automatisch gesetzt.\nEs werden nur eskalierte Verträge angezeigt.");
                $('[value="esk"').trigger("change");
                break;
            default:
                $('.form-control option[value=' + preselect + ']').prop("selected", true);
                $("#status").trigger("change");
                alert("Benutzerdefinierter Filter automatisch gesetzt.\nEs werden nur " + preselect + "e Verträge angezeigt.");
                break;
        }
    });
});

//Disable Fields in other Browsers than IE
$(document).ready(function () {
   $.each(columns,function (index, value) {
        switch (index) {
            case 4:
            case 20:
            case 22:
            case 23:
            case 24:
            case 25:
            case 26:
                $("#" + value).find("input,button,textarea,select").prop("disabled", true);
                break;
            default:
                break;
        }
    });
});