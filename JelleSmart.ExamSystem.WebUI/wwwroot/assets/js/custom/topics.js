"use strict";

var TopicsTable = function () {
    var initTable = function () {
        var table = $('#topicsTable');

        // Begin first table
        table.DataTable({
            responsive: true,
            // DOM layout
            dom: `<'row'<'col-sm-12'tr>>
                  <'row'<'col-sm-12 col-md-5'i><'col-sm-12 col-md-7 dataTables_pager'lp>>`,
            // Order settings
            order: [[0, 'asc']],
            // Column definitions
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
                    orderable: true,
                },
                {
                    targets: 4,
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
    TopicsTable.init();
});
