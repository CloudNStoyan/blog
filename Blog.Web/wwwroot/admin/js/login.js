let form = document.querySelector('.content > form');

form.addEventListener('submit', function() {
	let checkbox = form.querySelector('#customCheck1');
	rememberUsername(checkbox);
});

function rememberUsername(checkbox) {
	if (checkbox.getAttribute('checked') != undefined) {
		console.log(checkbox);		
	}
}