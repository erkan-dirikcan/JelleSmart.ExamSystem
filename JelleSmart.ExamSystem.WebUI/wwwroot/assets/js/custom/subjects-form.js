"use strict";

var SubjectsForm = function () {

    var initForm = function () {
        const form = document.getElementById('subjectForm');
        if (!form) return;

        const submitButton = form.querySelector('button[type="submit"]');

        form.addEventListener('submit', function (e) {
            // Show loading state
            if (submitButton) {
                submitButton.setAttribute('data-kt-indicator', 'on');
                submitButton.disabled = true;
            }
        });
    };

    var initValidation = function () {
        // Check for validation errors
        const validationSummary = document.querySelector('[asp-validation-summary="ModelOnly"]');
        if (validationSummary && validationSummary.textContent.trim()) {
            validationSummary.classList.remove('d-none');
        }
    };

    return {
        init: function () {
            initForm();
            initValidation();
        }
    };
}();

// Initialize on document ready
jQuery(document).ready(function () {
    SubjectsForm.init();
});
