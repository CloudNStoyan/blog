const menuBtn = document.querySelector('.menu');
const navMenu = document.querySelector('.navigation nav');

const toggleMenu = (e) => {
    e.preventDefault();

    menuBtn.classList.toggle('hide');
    navMenu.classList.toggle('show');
}

if (menuBtn) {
    menuBtn.addEventListener('click', toggleMenu);
}

