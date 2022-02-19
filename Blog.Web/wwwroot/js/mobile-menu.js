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

const openSearchBtn = document.querySelector('.open-search');
const closeSearchBtn = document.querySelector('.close-search');

const navigationContainer = document.querySelector('.navigation .container');

const openSearch = (e) => {
    e.preventDefault();

    navigationContainer.classList.add('search');
}

const closeSearch = (e) => {
    e.preventDefault();

    navigationContainer.classList.remove('search');
}

if (openSearchBtn) {
    openSearchBtn.addEventListener('click', openSearch);
}

if (closeSearchBtn) {
    closeSearchBtn.addEventListener('click', closeSearch);
}