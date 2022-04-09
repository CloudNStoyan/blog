const content = document.querySelector('.content');
const headings = content.querySelectorAll('h1,h2,h3,h4,h5,h6');

const addAnchor = (el) => {
    const anchor = document.createElement('a');
    anchor.href = `#${el.id}`;
    anchor.innerHTML = '<i class="fa-solid fa-link"></i>';
    anchor.className = 'heading-anchor';
    el.insertBefore(anchor, el.firstChild);
}

headings.forEach(addAnchor);

const codeWrappers = content.querySelectorAll('pre code');

const addCopyToClipboard = (el) => {
    const copyBtn = document.createElement('a');
    copyBtn.href = '#';
    copyBtn.addEventListener('click',
        (e) => {
            e.preventDefault();

            navigator.clipboard.writeText(el.innerText.trim());
            createToast('Copied to clipboard.');
        });
    copyBtn.className = 'copy-btn';
    copyBtn.innerText = 'Copy';
    el.parentElement.insertBefore(copyBtn, el);
}

codeWrappers.forEach(addCopyToClipboard);