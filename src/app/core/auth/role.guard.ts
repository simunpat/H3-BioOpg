import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

export const adminGuard: CanActivateFn = () => {
    const auth = inject(AuthService);
    const router = inject(Router);
    const token = auth.getToken();

    if (token && token.isAdmin) {
        return true;
    }

    void router.navigate(['/login']);
    return false;
};
