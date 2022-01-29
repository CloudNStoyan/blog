const filterForm = document.querySelector('form.list-wrapper');

function bindSelectToInput(inputSelector, selectSelector) {
    const input = document.querySelector(inputSelector);
    const select = document.querySelector(selectSelector);

    select.addEventListener('change',
        () => {
            input.value = select.children[select.selectedIndex].value;

            filterForm.submit();
        });
}

bindSelectToInput('#Filter_OrderBy', '#OrderBy');
bindSelectToInput('#Filter_Sort', '#Sort');

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