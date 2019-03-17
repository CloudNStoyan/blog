let tabs = document.querySelectorAll('.treeview');

for (let i = 0; i < tabs.length;i++) {
	let tab = tabs[i];
	let aBtn = tab.querySelector('a');
	aBtn.addEventListener('click', function(e) {
		e.preventDefault();
		toggleTab(tab);
	});
}

function toggleTab(tab) {
	let tabAttr = tab.getAttribute('class');
	let tabIsOpened = tabAttr.includes('open-menu');

	let treeViewMenu = tab.querySelector('.treeview-menu');

	if (tabIsOpened) {
	//Close tab
		treeViewMenu.setAttribute('style','display: none;');
		tab.setAttribute('class', 'treeview');
	} else {
	//Open tab
		treeViewMenu.setAttribute('style','diplay: block;');
		tab.setAttribute('class', 'active treeview open-menu');
	}
	
}
