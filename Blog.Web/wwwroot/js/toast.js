const toastContainer = document.querySelector('.toast-container');

const createToast = (toastText) => {
    const toast = document.createElement('div');
    toast.className = 'toast';
    toast.innerText = toastText;
    setTimeout(() => {
        toast.classList.add('remove');
        setTimeout(() => toast.remove(), 1500);
    }, 2000);
    toastContainer.appendChild(toast);
}