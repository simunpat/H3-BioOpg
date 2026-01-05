import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

export function roleGuard(required: 'Admin' | 'Customer'): CanActivateFn {
    return () => {
        const auth = inject(AuthService);
        const router = inject(Router);
        const token = auth.getToken();

        if (token && token.role === required) {
            return true;
        }

        void router.navigate(['/login']);

        return false;
    };
}
