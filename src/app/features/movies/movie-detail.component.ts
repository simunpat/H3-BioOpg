import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { MoviesService } from '../../services/movies.service';
import { ScreeningsService } from '../../services/screenings.service';
import { AuditoriumsService } from '../../services/auditoriums.service';
import { Movie } from '../../models/movie';
import { Screening } from '../../models/screening';
import { Auditorium } from '../../models/auditorium';

@Component({
    selector: 'app-movie-detail',
    standalone: true,
    imports: [CommonModule, RouterModule],
    templateUrl: './movie-detail.component.html',
    styleUrls: ['./movie-detail.component.scss'],
})
export class MovieDetailComponent {
    private readonly route = inject(ActivatedRoute);
    private readonly moviesService = inject(MoviesService);
    private readonly screeningsService = inject(ScreeningsService);
    private readonly auditoriumsService = inject(AuditoriumsService);

    protected readonly movie = signal<Movie | null>(null);
    protected readonly allScreenings = signal<Screening[]>([]);
    protected readonly auditoriums = signal<Auditorium[]>([]);
    protected readonly days = signal<Date[]>([]);
    protected readonly screeningsByDay = signal<Screening[][]>([[], [], [], [], [], [], []]);

    constructor() {
        const id = this.route.snapshot.paramMap.get('id');

        if (!id) return;

        this.moviesService.get(id).subscribe((m) => this.movie.set(m));
        this.auditoriumsService.list().subscribe((auds) => this.auditoriums.set(auds));

        this.screeningsService.list().subscribe((items) => {
            this.allScreenings.set(items.filter((s) => s.movieId === id));
            this.computeDays();
            this.computeBuckets();
        });
    }

    private computeDays(): void {
        const start = new Date();

        start.setHours(0, 0, 0, 0);

        const arr = Array.from({ length: 7 }, (_, i) => {
            const d = new Date(start);
            d.setDate(start.getDate() + i);
            return d;
        });
        this.days.set(arr);
    }

    private sameDay(a: Date, b: Date): boolean {
        return (
            a.getFullYear() === b.getFullYear() &&
            a.getMonth() === b.getMonth() &&
            a.getDate() === b.getDate()
        );
    }

    private computeBuckets(): void {
        const ds = this.days();
        const items = this.allScreenings();
        const buckets: Screening[][] = ds.map(() => []);

        for (const s of items) {
            const d = new Date(s.startTime);
            const idx = ds.findIndex((x) => this.sameDay(x, d));

            if (idx >= 0) buckets[idx].push(s);
        }

        // sort each bucket by time
        for (const arr of buckets) {
            arr.sort((a, b) => new Date(a.startTime).getTime() - new Date(b.startTime).getTime());
        }

        this.screeningsByDay.set(buckets);
    }

    auditoriumName(id: string): string {
        return this.auditoriums().find((a) => a.id === id)?.name ?? id;
    }

    dayLabel(d: Date, idx: number): string {
        const base = d.toLocaleDateString([], { weekday: 'short', month: 'short', day: 'numeric' });
        return idx === 0 ? `Today • ${base}` : base;
    }

    formatTime(iso: string): string {
        const dt = new Date(iso);

        if (Number.isNaN(dt.getTime())) return iso;

        return dt.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', hour12: false });
    }
}
