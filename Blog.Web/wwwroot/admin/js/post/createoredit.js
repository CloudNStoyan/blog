const tagBackendInput = document.querySelector('#Tags');
const tagInput = document.querySelector('.tag-input');
const tagsContainer = document.querySelector('.tags');

let tags = [...tagBackendInput.value.split(',')];

const removeTag = (tagName, tagEl) => {
    tagEl.remove();
    tags = tags.filter(tag => tag !== tagName);
    tagBackendInput.value = tags.join(',');
}

tagsContainer.querySelectorAll('.tag')
    .forEach(tagEl => tagEl.addEventListener('click', (e) => {
        e.preventDefault();

        removeTag(tagEl.innerText, tagEl);
    }));

tagInput.addEventListener('keydown', (e) => {
    if (e.code !== 'Comma') {
        return;
    }

    e.preventDefault();

    const tagName = tagInput.value.trim();

    tagInput.value = '';

    if (!tagName || tagName.length === 0 || tags.includes(tagName)) {
        return;
    }
    
    const tag = document.createElement('a');
    tag.href = '#';
    tag.innerText = tagName;
    tag.className = 'tag';
    tag.addEventListener('click', (tagE) => {
        tagE.preventDefault();
        removeTag(tagName, tag);
    });

    tagsContainer.insertBefore(tag, tagsContainer.lastElementChild);

    tags.push(tagName);

    tagBackendInput.value = tags.join(',');
});