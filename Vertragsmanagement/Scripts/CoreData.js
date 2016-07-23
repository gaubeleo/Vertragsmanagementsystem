//Test für Server Side Processing
//$(document).ready(function () {
//    $("#CostCentresTable").DataTable({
//        processing: true,
//        serverSide: true,
//        ajax: {
//            url: "/CoreData/LoadData",
//            type: "GET",
//            datatype: "json"
//        },
//        columns: [
//            { data: "ID", name: "Kostenstelle", autoWidth: true },
//            { data: "describtion", name: "Beschreibung", autoWidth: true }
//        ]
//    });
//});

$(document).ready(function () {
    //Create current date for file Export
    var currentDate = new Date();
    var d = currentDate.getFullYear() + "-" + ('0' + (currentDate.getMonth() + 1)).slice(-2) + "-" + ('0' + currentDate.getDate()).slice(-2);

    //Create different Popovers for different pages of CoreData
    var url = window.location.pathname.split("/");
    
    if (typeof url[2] == 'undefined' || url[2].toUpperCase() === 'index'.toUpperCase() || url[2] === "") {
        $('[data-toggle="popover"]')
            .popover({
                trigger: 'hover',
                placement: "auto bottom",
                viewport: { selector: "tbody", padding: 0 }
            });
    } else {
        $('[data-toggle="popover"]')
            .popover({
                trigger: 'hover',
                placement: "auto bottom"
            });
    };
    
    //Define DataTable for UsersTable
    var users = $('#UsersTable').DataTable({
            //dom: 'Bflrtip',
            dom: "<'row'<'col-sm-3'l><'col-sm-6 text-center'B><'col-sm-3'f>>" +
			     "<'row'<'col-sm-12'tr>>" +
			     "<'row'<'col-sm-2'i>>" +
                 "<'row'<'col-sm-10 pull-right'p>>",
            scrollY: true,
            scrollX: true,
            //responsive: true,
            lengthMenu: [7, 15, 25, 50],
            pageLength: 7,
            colReorder: true,
            buttons: [
                {
                    extend: 'collection',
                    text: 'Filter',
                    collectionLayout: 'three-column',
                    buttons: [
                        {
                            extend: 'colvis',
                            postfixButtons: ['colvisRestore'],
                            collectionLayout: 'four-column',
                            columns: '0,1,2,3,4,5,6,7,8,9,10,11,12,13',
                        },
                        {
                            text: 'Standardansicht',
                            action: function (e, dt, node, config) {
                                users.column(1).visible(true);
                                users.column(2).visible(true);
                                users.column(5).visible(true);
                                users.column(6).visible(true);
                                users.column(7).visible(true);
                                users.column(8).visible(true);
                                users.column(9).visible(true);
                                users.column(10).visible(true);
                                users.column(11).visible(true);
                                users.column(12).visible(true);
                                users.column(13).visible(true);

                                users.column(0).visible(false);
                                users.column(3).visible(false);
                                users.column(4).visible(false);
                            }
                        },
                        {
                            text: 'Erweiterte Ansicht',
                            action: function (e, dt, node, config) {
                                users.column(1).visible(true);
                                users.column(2).visible(true);
                                users.column(5).visible(true);
                                users.column(6).visible(true);
                                users.column(7).visible(true);
                                users.column(8).visible(true);
                                users.column(9).visible(true);
                                users.column(10).visible(true);
                                users.column(11).visible(true);
                                users.column(12).visible(true);
                                users.column(13).visible(true);

                                dt.column(0).visible(!dt.column(0).visible());
                                dt.column(3).visible(!dt.column(3).visible());
                                dt.column(4).visible(!dt.column(4).visible());
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
                //Define AppPath in global Layout.cshtml
                //=> <script type="text/javascript"> var AppPath = '@Url.Content("~/")' </script>
                //Still not working on published page and action icons with different size in DataTables!
                //url: AppPath + "DataTables/German.json",
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
                    targets: [3],
                    visible: false
                },
                {
                    targets: [4],
                    visible: false
                },
                {
                    targets: [6],
                    searchable: false
                },
                {
                    targets: [7],
                    searchable: false
                },
                {
                    targets: [8],
                    searchable: false
                },
                {
                    targets: [9],
                    searchable: false
                },
                {
                    targets: [10],
                    searchable: false
                },
                {
                    targets: [12],
                    orderable: false,
                    searchable: false
                },
                {
                    targets: [13],
                    orderable: false
                }
            ]
        });

    //Define DataTable for MandantsTable
    var mandants = $('#MandantsTable').DataTable({
            dom: "<'row'<'col-sm-3'l><'col-sm-6 text-center'B><'col-sm-3'f>>" +
                 "<'row'<'col-sm-12'tr>>" +
                 "<'row'<'col-sm-2'i>>" +
                 "<'row'<'col-sm-10 pull-right'p>>",
            responsive: true,
            lengthMenu: [7, 15, 25, 50],
            pageLength: 7,
            colReorder: true,
            buttons: [
                {
                    extend: 'colvis',
                    postfixButtons: ['colvisRestore']
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
                        targets: [2],
                        searchable: false,
                        orderable: false
                    }
            ]
    });

    //Define DataTable for DepartmentsTable
    var departments = $('#DepartmentsTable').DataTable({
            dom: "<'row'<'col-sm-3'l><'col-sm-6 text-center'B><'col-sm-3'f>>" +
                 "<'row'<'col-sm-12'tr>>" +
                 "<'row'<'col-sm-2'i>>" +
                 "<'row'<'col-sm-10 pull-right'p>>",
            responsive: true,
            lengthMenu: [7, 15, 25, 50],
            pageLength: 7,
            colReorder: true,
            buttons: [
                {
                    extend: 'colvis',
                    postfixButtons: ['colvisRestore']
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
                    targets: [3],
                    searchable: false,
                    orderable: false
                }
            ]
    });

    //Define DataTable for CostCentresTable
    var costCentres = $('#CostCentresTable').DataTable({
            deferRender: true,
            dom: "<'row'<'col-sm-3'l><'col-sm-6 text-center'B><'col-sm-3'f>>" +
			     "<'row'<'col-sm-12'tr>>" +
			     "<'row'<'col-sm-2'i>>" +
                 "<'row'<'col-sm-10 pull-right'p>>",
            responsive: true,
            lengthMenu: [7, 15, 25, 50],
            pageLength: 7,
            colReorder: true,
            buttons: [
                {
                    extend: 'colvis',
                    postfixButtons: ['colvisRestore']
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
                    targets: [2],
                    searchable: false,
                    orderable: false
                }
            ]
    });

    //Define DataTable for ContractSubcategorysTable
    var contractSubcategorys = $('#ContractSubcategorysTable').DataTable({
            dom: "<'row'<'col-sm-3'l><'col-sm-6 text-center'B><'col-sm-3'f>>" +
                 "<'row'<'col-sm-12'tr>>" +
                 "<'row'<'col-sm-2'i>>" +
                 "<'row'<'col-sm-10 pull-right'p>>",
            responsive: true,
            lengthMenu: [7, 15, 25, 50],
            pageLength: 7,
            colReorder: true,
            buttons: [
                {
                    extend: 'colvis',
                    postfixButtons: ['colvisRestore']
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
                    targets: [3],
                    searchable: false,
                    orderable: false
                }
            ]
    });

    //Define DataTable for ContractCategorysTable
    var contractCategorys = $('#ContractCategorysTable').DataTable({
            dom: "<'row'<'col-sm-3'l><'col-sm-6 text-center'B><'col-sm-3'f>>" +
                 "<'row'<'col-sm-12'tr>>" +
                 "<'row'<'col-sm-2'i>>" +
                 "<'row'<'col-sm-10 pull-right'p>>",
            responsive: true,
            lengthMenu: [7, 15, 25, 50],
            pageLength: 7,
            colReorder: true,
            buttons: [
                {
                    extend: 'colvis',
                    postfixButtons: ['colvisRestore']
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
                    targets: [2],
                    searchable: false,
                    orderable: false
                }
            ]
    });

    //Define DataTable for ContractPartnersTable
    var contractPartners = $('#ContractPartnersTable').DataTable({
            dom: "<'row'<'col-sm-3'l><'col-sm-6 text-center'B><'col-sm-3'f>>" +
                 "<'row'<'col-sm-12'tr>>" +
                 "<'row'<'col-sm-2'i>>" +
                 "<'row'<'col-sm-10 pull-right'p>>",
            responsive: true,
            lengthMenu: [7, 15, 25, 50],
            pageLength: 7,
            colReorder: true,
            buttons: [
                {
                    extend: 'colvis',
                    postfixButtons: ['colvisRestore']
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
                    targets: [2],
                    searchable: false,
                    orderable: false
                }
            ]
    });

    //Define DataTable for ContractSpeciesTable
    var contractSpecies = $('#ContractSpeciesTable').DataTable({
            dom: "<'row'<'col-sm-3'l><'col-sm-6 text-center'B><'col-sm-3'f>>" +
                 "<'row'<'col-sm-12'tr>>" +
                 "<'row'<'col-sm-2'i>>" +
                 "<'row'<'col-sm-10 pull-right'p>>",
            responsive: true,
            lengthMenu: [7, 15, 25, 50],
            pageLength: 7,
            colReorder: true,
            buttons: [
                {
                    extend: 'colvis',
                    postfixButtons: ['colvisRestore']
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
                    targets: [2],
                    searchable: false,
                    orderable: false
                }
            ]
    });

    //Define DataTable for OtherFeaturesTable
    var otherFeatures = $('#OtherFeaturesTable').DataTable({
        dom: "<'row'<'col-sm-3'l><'col-sm-6 text-center'B><'col-sm-3'f>>" +
             "<'row'<'col-sm-12'tr>>" +
             "<'row'<'col-sm-2'i>>" +
             "<'row'<'col-sm-10 pull-right'p>>",
        responsive: true,
        lengthMenu: [7, 15, 25, 50],
        pageLength: 7,
        colReorder: true,
        buttons: [
            {
                extend: 'colvis',
                postfixButtons: ['colvisRestore']
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
                targets: [3],
                searchable: false,
                orderable: false
            }
        ]
    });

    //Define DataTable for DeletedContractsTable
    var deletedContracts = $('#DeletedContractsTable').DataTable({
        dom: "<'row'<'col-sm-3'l><'col-sm-6 text-center'B><'col-sm-3'f>>" +
             "<'row'<'col-sm-12'tr>>" +
             "<'row'<'col-sm-2'i>>" +
             "<'row'<'col-sm-10 pull-right'p>>",
        responsive: true,
        lengthMenu: [7, 15, 25, 50],
        pageLength: 7,
        colReorder: true,
        buttons: [
            {
                extend: 'colvis',
                postfixButtons: ['colvisRestore']
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
                targets: [6],
                searchable: false,
                orderable: false
            }
        ]
    });
});