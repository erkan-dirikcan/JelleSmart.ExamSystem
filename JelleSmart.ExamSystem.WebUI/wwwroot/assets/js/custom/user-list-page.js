"use strict";
var ApplicationTable = function () {

    var initTable = function () {
        var table = $('#user-list-table');
        var selectedApplicationId = "";

        // begin first table
        table.DataTable({
            responsive: true,
            columnDefs: [
                {
                    width: '250px',
                    targets: 4,
                }
            ]
        });

    };

    return {


        init: function () {
            initTable();
        }

    };

}();

jQuery(document).ready(function () {
    ApplicationTable.init();
});
