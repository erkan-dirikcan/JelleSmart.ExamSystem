"use strict";

var SubjectsTable = function () {
    var initTable = function () {
        var table = $('#subjectsTable');

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
    SubjectsTable.init();
});
