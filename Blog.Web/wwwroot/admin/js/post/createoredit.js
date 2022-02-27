const tagBackendInput = document.querySelector('#Tags');
const tagInput = document.querySelector('.tag-input');
const tagsContainer = document.querySelector('.tags');

let tags = [...tagBackendInput.value.split(',')];

const faAnimations = {
    fade: 'fa-fade',
    beat: 'fa-beat',
    shake: 'fa-shake'
}

const animate = (el, animName, onEnd, duration = 500) => {
    el.classList.add(animName);
    setTimeout(() => {
        el.classList.remove(animName);
        if (typeof onEnd === 'function') {
            onEnd();
        }
    }, duration);
}

const removeTag = (tagName, tagEl) => {
    animate(tagEl, faAnimations.fade, () => tagEl.remove());
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

    if (!tagName || tagName.length === 0) {
        return;
    }

    if (tags.includes(tagName)) {
        const tagElements = Array.from(tagsContainer.children);

        const tagEl = tagElements.find(el => el.innerText.trim() === tagName);

        animate(tagEl, faAnimations.shake);

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

    animate(tag, faAnimations.beat, null, 150);

    tags.push(tagName);

    tagBackendInput.value = tags.join(',');
});

const mainForm = document.querySelector('.main-form');
const addLeftoverTag = () => {
    const tagName = tagInput.value.trim();

    if (!tagName || tagName.length === 0 || tags.includes(tagName)) {
        return;
    }

    tags.push(tagName);

    tagBackendInput.value = tags.join(',');
}
mainForm.addEventListener('submit', (e) => {
    e.preventDefault();

    addLeftoverTag();

    mainForm.submit();
});