import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { UsersService } from '../../services/users.service';
import { User, UserRole } from '../../models/user';
import SHA256 from 'crypto-js/sha256';

@Component({
    selector: 'app-user-form',
    standalone: true,
    imports: [CommonModule, RouterModule, FormsModule],
    templateUrl: './user-form.component.html',
    styleUrls: ['./user-form.component.scss'],
})
export class UserFormComponent {
    private readonly route = inject(ActivatedRoute);
    private readonly router = inject(Router);
    private readonly users = inject(UsersService);

    protected form: { email: string; role: UserRole; password?: string } = {
        email: '',
        role: 'Customer',
    };
    protected editingId: string | null = null;

    constructor() {
        const id = this.route.snapshot.paramMap.get('id');
        if (id) {
            this.editingId = id;
            this.users.get(id).subscribe({
                next: (u) => {
                    if (!u) return;
                    this.form = {
                        email: u.email,
                        role: u.role,
                    };
                },
                error: (err) => {
                    // eslint-disable-next-line no-console
                    console.error('Failed to load user', err);
                    alert(`Failed to load user. ${err?.error?.message || err?.message || ''}`);
                },
            });
        }
    }

    cancel(): void {
        void this.router.navigate(['/admin/users']);
    }

    private computeHashAndSalt(pw: string): { hash: string; salt: string } {
        const salt = crypto.randomUUID();
        const hash = SHA256(pw + salt).toString();
        return { hash, salt };
    }

    save(): void {
        const email = this.form.email?.trim().toLowerCase();
        const role = this.form.role;
        if (!email || !role) return;

        if (this.editingId) {
            // Update
            const base: User = {
                id: this.editingId,
                email,
                role,
                passwordHash: '', // will be preserved server-side if empty here; for json server we send something meaningful
            };

            const doUpdate = (u: User) =>
                this.users.update(u).subscribe({
                    next: () => void this.router.navigate(['/admin/users']),
                    error: (err) => {
                        // eslint-disable-next-line no-console
                        console.error('Failed to update user', err);
                        alert(
                            `Failed to update user. ${err?.error?.message || err?.message || ''}`
                        );
                    },
                });

            if (this.form.password) {
                const { hash, salt } = this.computeHashAndSalt(this.form.password);
                doUpdate({ ...base, passwordHash: hash, passwordSalt: salt });
            } else {
                // Need to keep previous hash; fetch first to retain it
                this.users.get(this.editingId).subscribe((existing) => {
                    if (!existing) return;
                    doUpdate({
                        ...base,
                        passwordHash: existing.passwordHash,
                        passwordSalt: existing.passwordSalt,
                    });
                });
            }
        } else {
            // Create
            if (!this.form.password) return;
            const { hash, salt } = this.computeHashAndSalt(this.form.password);
            const newUser: User = {
                id: crypto.randomUUID(),
                email,
                role,
                passwordHash: hash,
                passwordSalt: salt,
            };

            this.users.create(newUser).subscribe({
                next: () => void this.router.navigate(['/admin/users']),
                error: (err) => {
                    // eslint-disable-next-line no-console
                    console.error('Failed to create user', err);
                    alert(`Failed to create user. ${err?.error?.message || err?.message || ''}`);
                },
            });
        }
    }
}
