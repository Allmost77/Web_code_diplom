window.diplomAuth = {
    async login(email, password) {
        const response = await fetch('/api/auth/web-login', {
            method: 'POST',
            credentials: 'same-origin',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ email, password })
        });

        if (!response.ok) {
            return {
                ok: false,
                status: response.status
            };
        }

        const auth = await response.json();

        localStorage.setItem('diplom_access_token', auth.accessToken);
        localStorage.setItem('diplom_token_expires_at', auth.expiresAt);
        localStorage.setItem('diplom_auth_user', JSON.stringify(auth.user));

        return {
            ok: true,
            user: auth.user
        };
    },

    async logout() {
        await fetch('/api/auth/logout', {
            method: 'POST',
            credentials: 'same-origin'
        });

        localStorage.removeItem('diplom_access_token');
        localStorage.removeItem('diplom_token_expires_at');
        localStorage.removeItem('diplom_auth_user');
    }
};
