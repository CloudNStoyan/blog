const filterForm = document.querySelector('.filter-group form');

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