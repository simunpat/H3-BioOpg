import { Component } from '@angular/core';
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
    users: User[] = [];

    constructor(private readonly usersService: UsersService) {
        this.load();
    }

    load(): void {
        this.usersService.list().subscribe((u) => (this.users = u ?? []));
    }

    remove(u: User): void {
        if (!confirm(`Delete user ${u.email}?`)) return;
        this.usersService.delete(u.id).subscribe(() => this.load());
    }
}
