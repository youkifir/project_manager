const API_BASE = 'https://localhost:7166/api';

// проверка токена
function parseJwt(t) {
    try {
        return JSON.parse(atob(t.split('.')[1]));
    } catch {
        return null;
    }
}

function isTokenExpired(t) {
    const data = parseJwt(t);
    if (!data) return true;
    return data.exp * 1000 < Date.now();
}

function checkAuth() {
    const t = localStorage.getItem('jwtToken');
    if (!t || isTokenExpired(t)) {
        localStorage.removeItem('jwtToken');
        window.location.href = '../html/login.html';
    }
}

checkAuth();
setInterval(checkAuth, 15000);

const token = localStorage.getItem('jwtToken');
const payload = parseJwt(token);
const username = payload?.unique_name || payload?.name || payload?.sub || 'User';

document.getElementById('navUsername').textContent = username;

document.getElementById('logoutBtn').addEventListener('click', () => {
    localStorage.removeItem('jwtToken');
    window.location.href = '../html/login.html';
});

// запрос с токеном
function getHeaders() {
    return {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + token
    };
}

async function apiFetch(url, options = {}) {
    const res = await fetch(url, {
        ...options,
        headers: getHeaders()
    });

    if (res.status === 401) {
        localStorage.removeItem('jwtToken');
        window.location.href = '../html/login.html';
        return null;
    }

    return res;
}

function showAlert(id, msg) {
    const el = document.getElementById(id);
    el.textContent = msg;
    el.classList.remove('d-none');
    setTimeout(() => el.classList.add('d-none'), 4000);
}

function formatDate(str) {
    if (!str) return '—';
    return new Date(str).toLocaleDateString('en-GB', {
        day: '2-digit', month: 'short', year: 'numeric'
    });
}

// поиск
let searchTimer;
document.getElementById('searchInput').addEventListener('input', function () {
    clearTimeout(searchTimer);
    searchTimer = setTimeout(() => loadProjects(this.value.trim()), 300);
});

// загрузка проектов
async function loadProjects(searchTerm = '') {
    const list = document.getElementById('projectsList');
    list.innerHTML = '<div class="text-center text-muted py-5">Loading...</div>';

    try {
        const url = searchTerm
            ? `${API_BASE}/ProjectsApi?searchTerm=${encodeURIComponent(searchTerm)}`
            : `${API_BASE}/ProjectsApi`;

        const res = await apiFetch(url);
        if (!res) return;

        if (!res.ok) throw new Error('Failed to load');

        const raw = await res.json();
        const projects = Array.isArray(raw) ? raw
            : Array.isArray(raw.$values) ? raw.$values
                : Array.isArray(raw.projects) ? raw.projects
                    : Array.isArray(raw.data) ? raw.data
                        : [];

        renderProjects(projects);

    } catch (err) {
        list.innerHTML = '';
        showAlert('errorMessage', 'Failed to connect to server.');
        console.error(err);
    }
}

function renderProjects(projects) {
    const list = document.getElementById('projectsList');
    list.innerHTML = '';

    if (!projects || projects.length === 0) {
        const empty = document.createElement('div');
        empty.className = 'text-center text-muted py-5';
        empty.innerHTML = '<p class="fs-5">No projects yet</p><p class="small">Click <strong>+ New Project</strong> to get started.</p>';
        list.append(empty);
        return;
    }

    projects.forEach(p => {
        const card = document.createElement('div');
        card.className = 'card shadow-sm mb-3';
        card.id = 'project-' + p.projectId;

        const body = document.createElement('div');
        body.className = 'card-body';

        const row = document.createElement('div');
        row.className = 'd-flex justify-content-between align-items-start';

        const info = document.createElement('div');

        const title = document.createElement('h5');
        title.className = 'card-title mb-1';
        title.textContent = p.name;

        const desc = document.createElement('p');
        desc.className = 'card-text text-muted small mb-2';
        if (p.description) {
            desc.textContent = p.description;
        } else {
            const em = document.createElement('em');
            em.textContent = 'No description';
            desc.append(em);
        }

        const meta = document.createElement('span');
        meta.className = 'text-muted';
        meta.style.fontSize = '0.78rem';
        meta.textContent = 'Created: ' + formatDate(p.createdAt);
        if (p.updateAt) meta.textContent += ' · Updated: ' + formatDate(p.updateAt);

        info.append(title, desc, meta);

        const btnGroup = document.createElement('div');
        btnGroup.className = 'd-flex gap-2 ms-3 flex-shrink-0';

        const tasksLink = document.createElement('a');
        tasksLink.className = 'btn btn-outline-primary btn-sm';
        tasksLink.href = 'tasks.html?projectId=' + p.projectId;
        tasksLink.textContent = 'Tasks';

        const editBtn = document.createElement('button');
        editBtn.className = 'btn btn-outline-secondary btn-sm';
        editBtn.textContent = 'Edit';
        editBtn.addEventListener('click', () => openEditModal(p.projectId, p.name, p.description || ''));

        const deleteBtn = document.createElement('button');
        deleteBtn.className = 'btn btn-outline-danger btn-sm';
        deleteBtn.textContent = 'Delete';
        deleteBtn.addEventListener('click', () => openDeleteModal(p.projectId, p.name));

        btnGroup.append(tasksLink, editBtn, deleteBtn);
        row.append(info, btnGroup);
        body.append(row);
        card.append(body);
        list.append(card);
    });
}

// создание
document.getElementById('createBtn').addEventListener('click', async () => {
    const name = document.getElementById('createName').value.trim();
    const description = document.getElementById('createDescription').value.trim();
    const errorEl = document.getElementById('createError');
    errorEl.classList.add('d-none');

    if (!name) {
        errorEl.textContent = 'Project name is required.';
        errorEl.classList.remove('d-none');
        return;
    }

    try {
        const res = await apiFetch(`${API_BASE}/ProjectsApi`, {
            method: 'POST',
            body: JSON.stringify({ name, description })
        });
        if (!res) return;

        if (res.ok) {
            bootstrap.Modal.getInstance(document.getElementById('createModal')).hide();
            document.getElementById('createName').value = '';
            document.getElementById('createDescription').value = '';
            showAlert('successMessage', 'Project created!');
            loadProjects(document.getElementById('searchInput').value.trim());
        } else {
            const data = await res.json().catch(() => null);
            errorEl.textContent = data?.message || 'Failed to create project.';
            errorEl.classList.remove('d-none');
        }
    } catch (err) {
        errorEl.textContent = 'Connection error.';
        errorEl.classList.remove('d-none');
    }
});

// редактирование
function openEditModal(id, name, description) {
    document.getElementById('editProjectId').value = id;
    document.getElementById('editName').value = name;
    document.getElementById('editDescription').value = description;
    document.getElementById('editError').classList.add('d-none');
    new bootstrap.Modal(document.getElementById('editModal')).show();
}

document.getElementById('editBtn').addEventListener('click', async () => {
    const id = document.getElementById('editProjectId').value;
    const name = document.getElementById('editName').value.trim();
    const description = document.getElementById('editDescription').value.trim();
    const errorEl = document.getElementById('editError');
    errorEl.classList.add('d-none');

    if (!name) {
        errorEl.textContent = 'Name is required.';
        errorEl.classList.remove('d-none');
        return;
    }

    try {
        const res = await apiFetch(`${API_BASE}/ProjectsApi/${id}`, {
            method: 'PUT',
            body: JSON.stringify({ name, description })
        });
        if (!res) return;

        if (res.ok) {
            bootstrap.Modal.getInstance(document.getElementById('editModal')).hide();
            showAlert('successMessage', 'Project updated!');
            loadProjects(document.getElementById('searchInput').value.trim());
        } else {
            errorEl.textContent = 'Failed to update.';
            errorEl.classList.remove('d-none');
        }
    } catch (err) {
        errorEl.textContent = 'Connection error.';
        errorEl.classList.remove('d-none');
    }
});

// удаление
function openDeleteModal(id, name) {
    document.getElementById('deleteProjectId').value = id;
    document.getElementById('deleteProjectName').textContent = name;
    new bootstrap.Modal(document.getElementById('deleteModal')).show();
}

document.getElementById('deleteBtn').addEventListener('click', async () => {
    const id = document.getElementById('deleteProjectId').value;

    try {
        const res = await apiFetch(`${API_BASE}/ProjectsApi/${id}`, {
            method: 'DELETE'
        });
        if (!res) return;

        if (res.ok) {
            bootstrap.Modal.getInstance(document.getElementById('deleteModal')).hide();
            showAlert('successMessage', 'Project deleted.');
            loadProjects(document.getElementById('searchInput').value.trim());
        } else {
            showAlert('errorMessage', 'Failed to delete.');
        }
    } catch (err) {
        showAlert('errorMessage', 'Connection error.');
    }
});

loadProjects();