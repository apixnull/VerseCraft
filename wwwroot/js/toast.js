function showToast(type, message) {
    const container = document.getElementById('toast-container');
    if (!container) return;

    const icons = {
        success: '<i class="bi bi-check-circle-fill"></i>',
        error: '<i class="bi bi-x-circle-fill"></i>',
        info: '<i class="bi bi-info-circle-fill"></i>',
        warning: '<i class="bi bi-exclamation-circle-fill"></i>'
    };

    const toast = document.createElement('div');
    toast.className = `toast-message ${type}`;
    toast.innerHTML = `
    <div class="toast-icon">${icons[type] || ''}</div>
    <div class="toast-content">${message}</div>
  `;

    container.appendChild(toast);

    // auto-remove
    const t = setTimeout(() => toast.remove(), 5000);

    // drag-to-dismiss
    let startX, dx, dragging = false;
    toast.addEventListener('pointerdown', e => {
        dragging = true;
        startX = e.clientX;
        toast.setPointerCapture(e.pointerId);
        toast.style.transition = 'none';
    });
    toast.addEventListener('pointermove', e => {
        if (!dragging) return;
        dx = e.clientX - startX;
        toast.style.transform = `translateX(${dx}px)`;
        toast.style.opacity = `${1 - Math.abs(dx) / 200}`;
    });
    toast.addEventListener('pointerup', e => {
        dragging = false;
        toast.releasePointerCapture(e.pointerId);
        toast.style.transition = '';
        if (Math.abs(dx) > 80) {
            clearTimeout(t);
            toast.remove();
        } else {
            toast.style.transform = '';
            toast.style.opacity = '';
        }
    });
}
