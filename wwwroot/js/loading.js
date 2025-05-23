﻿// Screen loading animation
window.addEventListener('load', () => {
    setTimeout(() => {
        const g = document.getElementById('global-loading');
        if (g) g.style.display = 'none';
    }, 1000);
});

// form-submit button loading animation
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('form').forEach(function (form) {

        // ✅ Skip delete forms to let SweetAlert handle them
        if (form.classList.contains('delete-form')) return;


        // ✅ Skip delete forms to let SweetAlert handle them
        if (form.classList.contains('remove-feature')) return;

        form.addEventListener('submit', function (e) {
            const submitButton = form.querySelector('button[type="submit"]');
            if (submitButton) {
                submitButton.classList.add('btn-loading');
                submitButton.disabled = true;

                // Prevent actual submission
                e.preventDefault();

                // Wait 3 seconds, then submit the form
                setTimeout(function () {
                    form.submit();
                }, 1500);
            }
        });
    });
});