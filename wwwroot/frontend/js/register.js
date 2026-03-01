const API_URL = 'https://localhost:7166/api/AuthApi/register';

document.getElementById('registerForm').addEventListener('submit', async function (e) {
    e.preventDefault();

    const username = document.getElementById('username').value;
    const password = document.getElementById('password').value;
    const confirmPassword = document.getElementById('confirmPassword').value;

    const errorBlock = document.getElementById('errorMessage');
    errorBlock.classList.add('d-none');
    errorBlock.innerHTML = '';

    if (password !== confirmPassword) {
        showError('Passwords do not match!');
        return;
    }

    try {
        const res = await fetch(API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ username, password })
        });

        if (res.ok) {
            window.location.href = 'login.html';
        } else {
            const data = await res.json();
            let msg = 'Registration error:<br>';

            if (Array.isArray(data)) {
                data.forEach(err => { msg += `- ${err.description}<br>`; });
            } else if (data.errors) {
                for (const key in data.errors) {
                    msg += `- ${data.errors[key][0]}<br>`;
                }
            } else {
                msg += 'Unknown error.';
            }

            showError(msg);
        }
    } catch (err) {
        showError('Server connection error.');
        console.error(err);
    }
});

function showError(msg) {
    const el = document.getElementById('errorMessage');
    el.innerHTML = msg;
    el.classList.remove('d-none');
}