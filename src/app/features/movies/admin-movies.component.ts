import { Component, inject, signal, computed } from '@angular/core';
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
    protected readonly pageSize = 6;
    protected readonly pageIndex = signal(0);

    protected readonly totalPages = computed(() =>
        Math.max(1, Math.ceil(this.movies().length / this.pageSize))
    );

    protected readonly pagedMovies = computed(() => {
        const start = this.pageIndex() * this.pageSize;
        return this.movies().slice(start, start + this.pageSize);
    });

    constructor() {
        this.refresh();
    }

    posterUrl(m: Movie): string {
        return m.posterUrl || '/uploads/posters/template-poster.png';
    }

    refresh(): void {
        this.service.list().subscribe((items) => {
            this.movies.set(items);
            this.pageIndex.set(0);
        });
    }

    remove(id: string): void {
        this.service.delete(id).subscribe(() => this.refresh());
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
