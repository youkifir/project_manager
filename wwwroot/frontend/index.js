const token = localStorage.getItem('jwtToken');
        if (!token) {
            window.location.href = '/html/login.html';
        }

        function parseJwt(token) {
            try {
                return JSON.parse(atob(token.split('.')[1]));
            } catch (e) {
                return null;
            }
        }

        const payload = parseJwt(token);
        console.log('JWT Payload:', payload);
        const username = payload?.unique_name
            || payload?.name
            || payload?.sub
            || payload?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']
            || 'User';

        document.getElementById('navUsername').textContent = username;
        document.getElementById('welcomeUsername').textContent = username;

        // Logout
        document.getElementById('logoutBtn').addEventListener('click', function () {
            localStorage.removeItem('jwtToken');
            window.location.href = '/html/login.html';
        });