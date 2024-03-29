﻿// Order by

const filterForm = document.querySelector('.filter-form');

const orderByButtons = document.querySelectorAll('.order a');

const orderByInput = document.querySelector('#Filter_OrderBy');
const sortInput = document.querySelector('#Filter_Sort');

orderByButtons.forEach(button => button.addEventListener('click',
    (e) => {
        e.preventDefault();

        if (button.classList.contains('active')) {
            const currentSort = sortInput.value;

            sortInput.value = (currentSort === 'Ascending') ? 'Descending' : 'Ascending';
            filterForm.submit();
            return;
        }

        orderByInput.value = button.dataset.orderBy;
        sortInput.value = 'Ascending';
        filterForm.submit();
    }));

// Pagination

const paginationButtons = document.querySelectorAll('.pagination .page-item[data-page] .page-link');
const limit = Number(document.querySelector('#Filter_Limit').value);

const offsetInput = document.querySelector('#Filter_Offset');

paginationButtons.forEach(button => {
    button.addEventListener('click', (e) => {
        e.preventDefault();

        const parent = button.parentElement;

        if (parent.classList.contains('disabled')) {
            return;
        }

        const page = Number(parent.dataset.page) - 1;

        offsetInput.value = page * limit;

        filterForm.submit();
    });
});

// Clickable Tags

const tags = document.querySelectorAll('.tag');
const tagInput = document.querySelector('#Filter_TagId');

const tagIsClicked = (tagEl) => {
    const tagId = tagEl.dataset.tagId;

    tagInput.value = tagId;

    filterForm.submit();
}

if (tags) {
    tags.forEach(tag => tag.addEventListener('click', (e) => {
        e.preventDefault();
        tagIsClicked(tag);
    }));
}