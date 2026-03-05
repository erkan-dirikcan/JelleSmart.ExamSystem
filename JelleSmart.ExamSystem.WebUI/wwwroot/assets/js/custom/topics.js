"use strict";

var KTTopics = function() {

    var initTable = function() {
        const table = document.getElementById('topicsTable');
        if (!table) return;

        // Search functionality
        const searchInput = document.querySelector('[data-kt-topic-table-filter="search"]');
        if (searchInput) {
            searchInput.addEventListener('keyup', function(e) {
                const value = e.target.value.toLowerCase();
                table.querySelectorAll('tbody tr').forEach(function(row) {
                    const text = row.textContent.toLowerCase();
                    row.style.display = text.includes(value) ? '' : 'none';
                });
            });
        }
    };

    return {
        init: function() {
            initTable();
        }
    };
}();

// Initialize on document ready
jQuery(document).ready(function() {
    KTTopics.init();
});
