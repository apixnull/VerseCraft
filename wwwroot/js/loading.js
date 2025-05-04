// Screen loading animation
window.addEventListener('load', () => {
    setTimeout(() => {
        const g = document.getElementById('global-loading');
        if (g) g.style.display = 'none';
    }, 1000);
});

// form-submit button loading animation
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('form').forEach(function (form) {
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