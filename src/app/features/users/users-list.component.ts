import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { UsersService } from '../../services/users.service';
import { User } from '../../models/user';

@Component({
    selector: 'app-users-list',
    standalone: true,
    imports: [CommonModule, RouterModule],
    templateUrl: './users-list.component.html',
    styleUrls: ['./users-list.component.scss'],
})
export class UsersListComponent {
    private readonly usersService = inject(UsersService);
    protected readonly users = signal<User[]>([]);
    protected readonly pageSize = 10;
    protected readonly pageIndex = signal(0);

    protected readonly totalPages = computed(() =>
        Math.max(1, Math.ceil(this.users().length / this.pageSize))
    );

    protected readonly pagedUsers = computed(() => {
        const start = this.pageIndex() * this.pageSize;
        return this.users().slice(start, start + this.pageSize);
    });

    constructor() {
        this.load();
    }

    load(): void {
        this.usersService.list().subscribe((u) => {
            this.users.set(u ?? []);
            this.pageIndex.set(0);
        });
    }

    remove(u: User): void {
        if (!confirm(`Delete user ${u.email}?`)) return;
        this.usersService.delete(u.id).subscribe(() => this.load());
    }

    protected setPage(i: number): void {
        const clamped = Math.max(0, Math.min(i, this.totalPages() - 1));
        this.pageIndex.set(clamped);
    }

    protected prevPage(): void {
        this.setPage(this.pageIndex() - 1);
    }

    protected nextPage(): void {
        this.setPage(this.pageIndex() + 1);
    }
}
