const menuBtn = document.querySelector('.menu');
const navMenu = document.querySelector('.navigation nav');
const menuOverlay = document.querySelector('.menu-overlay');

const menuOpenClass = 'show';

const openMenu = (e) => {
    e.preventDefault();
    
    navMenu.classList.add(menuOpenClass);
}

const closeMenu = (e) => {
    e.preventDefault();

    navMenu.classList.remove(menuOpenClass);
}

if (menuBtn) {
    menuBtn.addEventListener('click', openMenu);
}

if (menuOverlay) {
    menuOverlay.addEventListener('click', closeMenu);
}
