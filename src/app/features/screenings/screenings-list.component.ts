import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ScreeningsService } from '../../services/screenings.service';
import { Screening } from '../../models/screening';
import { MoviesService } from '../../services/movies.service';
import { AuditoriumsService } from '../../services/auditoriums.service';
import { Movie } from '../../models/movie';
import { Auditorium } from '../../models/auditorium';
import { v4 as uuidv4 } from 'uuid';

@Component({
    selector: 'app-screenings-list',
    standalone: true,
    imports: [CommonModule, RouterModule, FormsModule],
    templateUrl: './screenings-list.component.html',
    styleUrls: ['./screenings-list.component.scss'],
})
export class ScreeningsListComponent {
    private readonly screeningsService = inject(ScreeningsService);
    private readonly moviesService = inject(MoviesService);
    private readonly auditoriumsService = inject(AuditoriumsService);

    protected readonly screenings = signal<Screening[]>([]);
    protected readonly movies = signal<Movie[]>([]);
    protected readonly auditoriums = signal<Auditorium[]>([]);
    protected sortBy: 'movie' | 'auditorium' | 'startTime' | 'price' = 'startTime';
    protected sortDir: 'asc' | 'desc' = 'asc';
    protected readonly sorted = signal<Screening[]>([]);

    protected form: {
        movieId?: string;
        auditoriumId?: string;
        startLocal?: string;
        price?: number;
    } = {};

    constructor() {
        this.refresh();
    }

    refresh(): void {
        this.screeningsService.list().subscribe((items) => {
            this.screenings.set(items);
            this.applySort();
        });

        this.moviesService.list().subscribe((items) => this.movies.set(items));
        this.auditoriumsService.list().subscribe((items) => this.auditoriums.set(items));
    }

    protected applySort(): void {
        const items = [...this.screenings()];
        const by = this.sortBy;
        const dir = this.sortDir === 'asc' ? 1 : -1;
        items.sort((a, b) => {
            let av: number | string = 0;
            let bv: number | string = 0;
            switch (by) {
                case 'movie':
                    av = this.movieTitle(a.movieId).toLowerCase();
                    bv = this.movieTitle(b.movieId).toLowerCase();
                    break;
                case 'auditorium':
                    av = this.auditoriumName(a.auditoriumId).toLowerCase();
                    bv = this.auditoriumName(b.auditoriumId).toLowerCase();
                    break;
                case 'startTime':
                    av = new Date(a.startTime).getTime();
                    bv = new Date(b.startTime).getTime();
                    break;
                case 'price':
                    av = a.price;
                    bv = b.price;
                    break;
            }
            if (av < bv) return -1 * dir;
            if (av > bv) return 1 * dir;
            return 0;
        });
        this.sorted.set(items);
    }

    protected toggleSort(by: 'movie' | 'auditorium' | 'startTime' | 'price'): void {
        if (this.sortBy === by) {
            this.sortDir = this.sortDir === 'asc' ? 'desc' : 'asc';
        } else {
            this.sortBy = by;
            this.sortDir = 'asc';
        }
        this.applySort();
    }

    add(): void {
        if (
            !this.form.movieId ||
            !this.form.auditoriumId ||
            !this.form.startLocal ||
            !this.form.price
        )
            return;

        const iso = new Date(this.form.startLocal).toISOString();

        const s: Screening = {
            id: uuidv4(),
            movieId: this.form.movieId,
            auditoriumId: this.form.auditoriumId,
            startTime: iso,
            price: Number(this.form.price),
        };

        this.screeningsService.create(s).subscribe(() => {
            this.form = {};

            this.refresh();
        });
    }

    remove(id: string): void {
        this.screeningsService.delete(id).subscribe(() => this.refresh());
    }

    movieTitle(id: string): string {
        return this.movies().find((m) => m.id === id)?.title ?? id;
    }

    auditoriumName(id: string): string {
        return this.auditoriums().find((a) => a.id === id)?.name ?? id;
    }

    formatDateTime(iso: string): string {
        const d = new Date(iso);

        if (Number.isNaN(d.getTime())) return iso;

        return d.toLocaleString([], { dateStyle: 'medium', timeStyle: 'short', hour12: false });
    }
}
