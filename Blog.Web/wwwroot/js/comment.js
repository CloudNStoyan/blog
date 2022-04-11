const commentWrapper = document.querySelector('.comment-wrapper');
const createCommentWrapper = commentWrapper.querySelector('.create-comment-wrapper');
const commentsWrapper = commentWrapper.querySelector('.comments');

const isInViewport = (el) => {
    const rect = el.getBoundingClientRect();

    return (
        rect.top >= 0 &&
            rect.left >= 0 &&
            rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&
            rect.right <= (window.innerWidth || document.documentElement.clientWidth)
    );
};

const commentLoader = commentWrapper.querySelector('.comment-loader');

let isLoadingComments = false;
let loadedAllComments = false;
let offset = 10;

const postJson = JSON.parse(document.querySelector('main[data-post-json]').dataset.postJson);

const loadCommentsIfNeeded = async () => {
    if (!isInViewport(commentLoader) || isLoadingComments || loadedAllComments) {
        return;
    }

    isLoadingComments = true;
    commentLoader.classList.remove('hide');

    const filter = {
        postId: postJson['post_id'],
        offset: offset
    };

    const comments = await fetch(`/api/comment/get?${new URLSearchParams(filter)}`)
        .then(response => response.json())
        .catch(error => console.error(error));

    if (comments.length === 0) {
        loadedAllComments = true;
        isLoadingComments = false;
        commentLoader.classList.add('hide');
        return;
    }

    appendCommentsToPage(comments);

    offset += 10;

    isLoadingComments = false;
    commentLoader.classList.add('hide');
};

document.addEventListener('scroll', loadCommentsIfNeeded);

const notScrollable = !(document.body.clientHeight > window.innerHeight);

if (notScrollable) {
    loadCommentsIfNeeded();
}

const appendCommentsToPage = (comments) => {
    comments.forEach(comment => {
        const commentElement = document.createElement('div');
        commentElement.className = 'comment';
        commentElement.innerHTML =
            `
            <div class="avatar">
                <img src="${comment['avatar_url']}" alt="${comment['username']}"/>
            </div>
            <div class="comment-body">
                <div class="user">
                    <span>${comment['username']}</span>
                    <span class="created">${comment['human_date']}</span>
                    ${comment['edited'] ? '<span>(edited)</span>' : ''}
                </div>
                <p>${comment['content']}</p>
            </div>
            `;

        commentsWrapper.appendChild(commentElement);
    });
};


const addCommentToPage = (comment) => {
    const commentElement = document.createElement('div');
    commentElement.className = 'comment new';
    commentElement.innerHTML =
        `
            <div class="avatar">
                <img src="${comment['avatar_url']}" alt="${comment['username']}"/>
            </div>
            <div class="comment-body">
                <div class="user">
                    <span>${comment['username']}</span>
                    <span class="created">${comment['human_date']}</span>
                </div>
                <p>${comment['content']}</p>
            </div>
            `;

    commentsWrapper.insertBefore(commentElement, commentsWrapper.firstElementChild);
};

const addReplyToPage = (comment, parentEl) => {
    const commentElement = document.createElement('div');
    commentElement.className = 'comment new';
    commentElement.innerHTML =
        `
            <div class="avatar">
                <img src="${comment['avatar_url']}" alt="${comment['username']}"/>
            </div>
            <div class="comment-body">
                <div class="user">
                    <span>${comment['username']}</span>
                    <span class="created">${comment['human_date']}</span>
                </div>
                <p>${comment['content']}</p>
            </div>
            `;

    const childrenComments = parentEl.querySelector('.children.comments');

    if (!childrenComments) {
        const childrenCommentsEl = document.createElement('div');
        childrenCommentsEl.className = 'children comments';
        childrenCommentsEl.appendChild(commentElement);

        parentEl.appendChild(childrenCommentsEl);
        return;
    }

    childrenComments.appendChild(commentElement);
};

const commentInput = createCommentWrapper.querySelector('textarea');
const commentCancelBtn = createCommentWrapper.querySelector('.cancel');
const createCommentEl = createCommentWrapper.querySelector('.create-comment');

const activeCreateCommentClass = 'active';

commentCancelBtn.addEventListener('click',
    (e) => {
        e.preventDefault();

        createCommentEl.classList.remove(activeCreateCommentClass);
    });

commentInput.addEventListener('focus',
    () => {
        createCommentEl.classList.add(activeCreateCommentClass);
    });

const cleanCreateCommentFields = () => {
    commentInput.value = '';
};

const handleCommentResponse = (comment, parentEl) => {
    if (!parentEl) {
        addCommentToPage(comment);
    } else {
        addReplyToPage(comment, parentEl);
    }
    cleanCreateCommentFields();
    createToast('Successfully submitted a comment.');
};

const createComment = (content, parentId, parentEl) => {
    const comment = {
        content: content,
        postId: postJson['post_id'],
        parentId: parentId
    };

    fetch(`/api/comment/create?${new URLSearchParams(comment)}`,
            {
                method: 'POST'
            })
        .then(response => response.json())
        .then(comment => handleCommentResponse(comment, parentEl))
        .catch(error => {
            console.error(error);

            createToast('Error, try again.');
        });
};

const editComment = async (content, commentId) => {
    const comment = {
        content,
        commentId
    }

    const commentResponse = await fetch(`/api/comment/edit?${new URLSearchParams(comment)}`,
            {
                method: 'POST'
            })
        .then(response => response.json())
        .catch(error => {
            console.error(error);

            createToast('Error, try again.');
        });

    return commentResponse;
}

const commentBtn = createCommentWrapper.querySelector('.comment');

commentBtn.addEventListener('click',
    (e) => {
        e.preventDefault();

        const commentContent = commentInput.value.trim();

        if (commentContent.length === 0) {
            createToast('You can\'t comment nothing.');
            return;
        }

        createComment(commentContent);
    });

const editCommentHandler = (e) => {
    e.preventDefault();

    const commentEl = e.target.parentElement.parentElement.parentElement;;

    const createCommentEl = document.createElement('div');
    createCommentEl.className = 'create-comment active';

    const commentBody = commentEl.querySelector('.comment-body');
    commentBody.classList.add('hide');

    commentEl.insertBefore(createCommentEl, commentEl.children[1]);

    const commentContent = commentBody.querySelector('p');

    const createCommentTextarea = document.createElement('textarea');
    createCommentTextarea.setAttribute('rows', '4');
    createCommentTextarea.setAttribute('placeholder', 'Edit comment...');
    createCommentTextarea.value = commentContent.innerText;
    createCommentEl.appendChild(createCommentTextarea);

    const actionsEl = document.createElement('div');
    actionsEl.className = 'actions';
    createCommentEl.appendChild(actionsEl);

    const cancelBtn = document.createElement('a');
    cancelBtn.className = 'cancel';
    cancelBtn.innerText = 'Cancel';
    cancelBtn.href = '#';
    cancelBtn.addEventListener('click',
        (cancelE) => {
            cancelE.preventDefault();

            createCommentEl.remove();
            commentBody.classList.remove('hide');
        });
    actionsEl.appendChild(cancelBtn);

    const commentBtn = document.createElement('a');
    commentBtn.className = 'comment';
    commentBtn.innerText = 'Edit';
    commentBtn.href = '#';
    commentBtn.addEventListener('click',
        async (btnE) => {
            btnE.preventDefault();

            const editedComment = await editComment(createCommentTextarea.value, Number(e.target.dataset.commentId));

            const commentUser = commentBody.querySelector('.user');

            if (!commentUser.querySelector('.edited')) {
                const commentEditedEl = document.createElement('span')
                commentEditedEl.innerText = '(edited)';
                commentEditedEl.className = 'edited';
                commentUser.appendChild(commentEditedEl);
            }

            commentContent.innerText = editedComment.content;

            createCommentEl.remove();
            commentBody.classList.remove('hide');
        });
    actionsEl.appendChild(commentBtn);
}

const userJson = JSON.parse(document.querySelector('main[data-user-json]').dataset.userJson);

const replyToComment = (e) => {
    e.preventDefault();

    const commentEl = e.target.parentElement.parentElement.parentElement;

    const createCommentWrapperEl = document.createElement('div');
    createCommentWrapperEl.className = 'create-comment-wrapper';
    commentEl.appendChild(createCommentWrapperEl);

    const avatarEl = document.createElement('div');
    avatarEl.className = 'avatar';
    avatarEl.innerHTML =
        `<img src="${userJson['avatar']}" alt="${userJson['username']}">`;
    createCommentWrapperEl.appendChild(avatarEl);

    const createCommentEl = document.createElement('div');
    createCommentEl.className = 'create-comment active';
    createCommentWrapperEl.appendChild(createCommentEl);

    const createCommentTextarea = document.createElement('textarea');
    createCommentTextarea.setAttribute('rows', '4');
    createCommentTextarea.setAttribute('placeholder', 'Add a comment...');
    createCommentEl.appendChild(createCommentTextarea);

    const actionsEl = document.createElement('div');
    actionsEl.className = 'actions';
    createCommentEl.appendChild(actionsEl);

    const cancelBtn = document.createElement('a');
    cancelBtn.className = 'cancel';
    cancelBtn.innerText = 'Cancel';
    cancelBtn.href = '#';
    cancelBtn.addEventListener('click',
        (cancelE) => {
            cancelE.preventDefault();

            createCommentWrapperEl.remove();
        });
    actionsEl.appendChild(cancelBtn);

    const commentBtn = document.createElement('a');
    commentBtn.className = 'comment';
    commentBtn.innerText = 'Comment';
    commentBtn.href = '#';
    commentBtn.addEventListener('click',
        (btnE) => {
            btnE.preventDefault();

            console.log(e);

            let parentEl = commentEl;

            if (e.target.hasAttribute('data-is-child')) {
                parentEl = commentEl.parentElement.parentElement;
            }

            createComment(createCommentTextarea.value.trim(), Number(e.target.dataset.commentId), parentEl);
            createCommentWrapperEl.remove();
        });
    actionsEl.appendChild(commentBtn);
};