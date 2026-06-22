// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Multi-level dropdown menu: clicking a submenu toggle opens its flyout without closing the parent dropdown.
document.addEventListener('click', function (event) {
    const toggle = event.target.closest('.dropdown-submenu > .dropdown-toggle');
    if (!toggle) return;

    event.preventDefault();
    event.stopPropagation();

    const submenu = toggle.closest('.dropdown-submenu');
    const wasOpen = submenu.classList.contains('show');

    submenu.parentElement.querySelectorAll('.dropdown-submenu.show').forEach(function (el) {
        if (el !== submenu) el.classList.remove('show');
    });

    submenu.classList.toggle('show', !wasOpen);
});

document.addEventListener('hidden.bs.dropdown', function (event) {
    event.target.querySelectorAll('.dropdown-submenu.show').forEach(function (el) {
        el.classList.remove('show');
    });
});
