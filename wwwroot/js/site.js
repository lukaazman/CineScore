// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener('DOMContentLoaded', () => {
    const searchForm = document.getElementById('navbarSearchForm');
    const searchInput = document.getElementById('navbarSearchInput');
    const searchToggle = document.getElementById('navbarSearchToggle');

    if (!searchForm || !searchInput || !searchToggle) {
        return;
    }

    const showInput = () => {
        searchInput.classList.add('show');
        searchInput.removeAttribute('aria-hidden');
        searchInput.focus();
    };

    const hideInput = () => {
        searchInput.classList.remove('show');
        searchInput.setAttribute('aria-hidden', 'true');
        searchInput.value = '';
    };

    searchToggle.addEventListener('click', (event) => {
        event.preventDefault();
        const isVisible = searchInput.classList.contains('show');

        if (!isVisible) {
            showInput();
            return;
        }

        if (searchInput.value.trim() !== '') {
            searchForm.submit();
        }
        else {
            hideInput();
        }
    });

    searchForm.addEventListener('submit', (event) => {
        if (searchInput.value.trim() === '') {
            event.preventDefault();
            hideInput();
        }
        else {
            searchInput.value = searchInput.value.trim();
        }
    });

    searchInput.addEventListener('keydown', (event) => {
        if (event.key === 'Escape') {
            hideInput();
        }
    });
});
