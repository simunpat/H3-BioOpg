import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterOutlet } from '@angular/router';
import { AuthService } from './core/auth/auth.service';

@Component({
    selector: 'app-root',
    imports: [CommonModule, RouterOutlet, RouterLink],
    templateUrl: './app.html',
    styleUrl: './app.scss',
})
export class App {
    protected readonly auth = inject(AuthService);
}
