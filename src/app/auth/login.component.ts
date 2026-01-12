import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../core/auth/auth.service';
import { HttpClient } from '@angular/common/http';

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
        private readonly http: HttpClient,
        private readonly router: Router
    ) {}

    onSubmit(): void {
        const email = this.email.trim().toLowerCase();
        if (!email || !this.password) {
            this.error = 'Email and password are required';
            return;
        }

        this.http
            .post<{ token: string }>('/api/auth/login', { email, password: this.password })
            .subscribe({
                next: (res) => {
                    if (!res?.token) {
                        this.error = 'Unexpected response';
                        return;
                    }

                    this.auth.loginWithJwt(res.token);
                    void this.router.navigate(['/']);
                },
                error: () => {
                    this.error = 'Invalid email or password';
                },
            });
    }
}
