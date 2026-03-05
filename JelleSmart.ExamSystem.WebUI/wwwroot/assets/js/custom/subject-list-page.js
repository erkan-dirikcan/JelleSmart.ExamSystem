"use strict";
$(document).ready(function () {
    $("#data-Table-Search").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $("#data-Table tbody tr").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
        });
    });
});


var SubjectList = function () {

    var initPage = function () {
        var table = $('#SubjectTable');

        // begin first table
        table.DataTable({
            responsive: true,
            language: {
                url: "//cdn.datatables.net/plug-ins/1.13.6/i18n/tr.json"
            },
            ordering: false,
            paging: true,
            columnDefs: [

            ]
        });
    };

    return {
        init: function () {
            initPage();
        }
    };
}();

jQuery(document).ready(function () {
    SubjectList.init();
});
