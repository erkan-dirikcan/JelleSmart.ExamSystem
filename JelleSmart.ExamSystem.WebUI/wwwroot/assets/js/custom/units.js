"use strict";

var UnitsTable = function () {
    var initTable = function () {
        var table = $('#unitsTable');

        // Begin first table
        table.DataTable({
            responsive: true,
            // DOM layout
            dom: `<'row'<'col-sm-12'tr>>
                  <'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>`,
            // Order settings
            order: [[0, 'asc']],
            // Column definitions - 4 sütun var artık (Sınıf sütunu kaldırıldı)
            columnDefs: [
                {
                    targets: 0,
                    orderable: true,
                },
                {
                    targets: 1,
                    orderable: true,
                },
                {
                    targets: 2,
                    orderable: true,
                },
                {
                    targets: 3,
                    orderable: false,
                    className: 'text-end'
                }
            ],
            language: {
                url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/tr.json'
            }
        });
    };

    return {
        init: function () {
            initTable();
        }
    };
}();

// Initialize on document ready
jQuery(document).ready(function () {
    UnitsTable.init();
});
