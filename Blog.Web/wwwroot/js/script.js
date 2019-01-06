let button = document.querySelector('header.navigation > a.btn');
let popup = document.querySelector('div.popup');

button.addEventListener('click', function() {
    let list = document.querySelector('header.navigation > nav');
    let style = list.getAttribute('style');
    if (style == "" || style == undefined) {
        list.style = "display: flex;";
	popup.style = "display: absolute;";
    } else {
        list.style = "";
	popup.style = "display: none;";
    }
});

popup.addEventListener('click', function() {
    let list = document.querySelector('header.navigation > nav');
    list.style = "";
    popup.style = "display: none;";
});