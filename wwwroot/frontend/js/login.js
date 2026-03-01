const API_URL = 'https://localhost:7166/api/AuthApi/login';

document.getElementById('loginForm').addEventListener('submit', async function (e) {
    e.preventDefault();

    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;

    document.getElementById('errorMessage').classList.add('d-none');

    try {
        const res = await fetch(API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, password })
        });

        if (res.ok) {
            const data = await res.json();
            localStorage.setItem('jwtToken', data.token);
            window.location.href = '../index.html';
        } else {
            showError('Неверный логин или пароль.');
        }
    } catch (err) {
        showError('Не удалось подключиться к серверу.');
        console.error(err);
    }
});

function showError(msg) {
    const el = document.getElementById('errorMessage');
    el.textContent = msg;
    el.classList.remove('d-none');
}