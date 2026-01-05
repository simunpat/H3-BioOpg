import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MoviesService } from '../../services/movies.service';
import { Movie } from '../../models/movie';

@Component({
    selector: 'app-admin-movies',
    standalone: true,
    imports: [CommonModule, RouterModule, FormsModule],
    templateUrl: './admin-movies.component.html',
    styleUrls: ['./admin-movies.component.scss'],
})
export class AdminMoviesComponent {
    private readonly service = inject(MoviesService);
    protected readonly movies = signal<Movie[]>([]);
    protected readonly displayedColumns = ['poster', 'title', 'genre', 'durationMin', 'actions'];

    constructor() {
        this.refresh();
    }

    posterUrl(m: Movie): string {
        return m.posterUrl || '/uploads/posters/template-poster.png';
    }

    refresh(): void {
        this.service.list().subscribe((items) => this.movies.set(items));
    }

    remove(id: string): void {
        this.service.delete(id).subscribe(() => this.refresh());
    }
}
