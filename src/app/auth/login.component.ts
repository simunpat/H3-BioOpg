import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService, AuthToken } from '../core/auth/auth.service';
import { UsersService } from '../services/users.service';
import SHA256 from 'crypto-js/sha256';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss'],
})
export class LoginComponent {
    email = '';
    password = '';
    error = '';

    constructor(
        private readonly auth: AuthService,
        private readonly users: UsersService,
        private readonly router: Router
    ) {}

    onSubmit(): void {
        const email = this.email.trim().toLowerCase();
        if (!email || !this.password) {
            this.error = 'Email and password are required';
            return;
        }

        this.users.findByEmail(email).subscribe((matches) => {
            const u = (matches ?? [])[0];

            if (!u || !u.passwordSalt || !u.passwordHash) {
                this.error = 'Invalid email or password';
                return;
            }

            const computed = SHA256(this.password + u.passwordSalt).toString();
            if (computed !== u.passwordHash) {
                this.error = 'Invalid email or password';
                return;
            }

            const token: AuthToken = {
                sub: u.id,
                role: u.role,
                exp: Math.floor(Date.now() / 1000) + 60 * 60,
            };

            this.auth.loginWithToken(token);
            void this.router.navigate(['/']);
        });
    }
}
