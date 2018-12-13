let button = document.querySelector('header.navigation > a.btn');
button.addEventListener('click', function() {
    let list = document.querySelector('header.navigation > nav');
    let style = list.getAttribute('style');
    if (style == "" || style == undefined) {
        list.style = "display: flex;";
    } else {
        list.style = "";
    }
});