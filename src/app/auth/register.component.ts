import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { UsersService } from '../services/users.service';
import { User } from '../models/user';
import { v4 as uuidv4 } from 'uuid';
import SHA256 from 'crypto-js/sha256';

@Component({
    selector: 'app-register',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.scss'],
})
export class RegisterComponent {
    email = '';
    password = '';
    error = '';

    constructor(private readonly users: UsersService, private readonly router: Router) {}

    onRegister(): void {
        const email = this.email.trim().toLowerCase();
        if (!email || !this.password) return;

        this.users.findByEmail(email).subscribe((existing) => {
            if ((existing?.length ?? 0) > 0) {
                this.error = 'Email already registered';
                return;
            }

            const salt = uuidv4();
            const passwordHash = SHA256(this.password + salt).toString();
            const u: User = {
                id: uuidv4(),
                email,
                role: 'Customer',
                passwordHash,
                passwordSalt: salt,
            };
            this.users.create(u).subscribe(() => {
                this.router.navigate(['/login']);
            });
        });
    }
}
