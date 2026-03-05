"use strict";

var KTApp = function() {

    var initPageLoader = function() {
        // Remove page loader after page load
        setTimeout(function() {
            var pageLoader = document.querySelector('.page-loader');
            if (pageLoader) {
                pageLoader.classList.add('d-none');
                setTimeout(function() {
                    pageLoader.style.display = 'none';
                }, 300);
            }

            var body = document.getElementById('kt_body');
            if (body) {
                body.classList.remove('page-loading');
            }
        }, 500);
    };

    var initAside = function() {
        // Aside toggle
        const asideToggle = document.getElementById('kt_aside_toggle');
        if (asideToggle) {
            asideToggle.addEventListener('click', function(e) {
                e.preventDefault();
                const body = document.body;
                body.classList.toggle('aside-minimize');
            });
        }

        // Mobile aside toggle
        const mobileToggle = document.getElementById('kt_aside_mobile_toggle');
        if (mobileToggle) {
            mobileToggle.addEventListener('click', function() {
                const body = document.body;
                body.classList.toggle('aside-minimize');
            });
        }
    };

    var initScrollTop = function() {
        const scrollTop = document.getElementById('kt_scrolltop');
        if (scrollTop) {
            window.addEventListener('scroll', function() {
                if (window.scrollY > 300) {
                    scrollTop.classList.add('show');
                } else {
                    scrollTop.classList.remove('show');
                }
            });

            scrollTop.addEventListener('click', function(e) {
                e.preventDefault();
                window.scrollTo({
                    top: 0,
                    behavior: 'smooth'
                });
            });
        }
    };

    var initDropdowns = function() {
        // Initialize Bootstrap dropdowns
        const dropdownElements = document.querySelectorAll('[data-bs-toggle="dropdown"]');
        dropdownElements.forEach(function(element) {
            new bootstrap.Dropdown(element);
        });
    };

    var initTooltips = function() {
        // Initialize Bootstrap tooltips
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        if (tooltipTriggerList.length > 0) {
            tooltipTriggerList.map(function(tooltipTriggerEl) {
                return new bootstrap.Tooltip(tooltipTriggerEl);
            });
        }
    };

    var initNotifications = function() {
        // Check for TempData messages and show as toastr
        // Messages should be set by server-side code in a global variable
        if (window.toastrSuccess) {
            setTimeout(function() {
                if (typeof toastr !== 'undefined') {
                    toastr.success(window.toastrSuccess);
                }
            }, 500);
        }

        if (window.toastrError) {
            setTimeout(function() {
                if (typeof toastr !== 'undefined') {
                    toastr.error(window.toastrError);
                }
            }, 500);
        }
    };

    return {
        init: function() {
            initPageLoader();
            initAside();
            initScrollTop();
            initDropdowns();
            initTooltips();
            initNotifications();
        }
    };
}();

// Initialize on document ready
jQuery(document).ready(function() {
    KTApp.init();
});
