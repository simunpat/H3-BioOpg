import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { AuthService } from '../../core/auth/auth.service';
import { BookingsService } from '../../services/bookings.service';
import { MoviesService } from '../../services/movies.service';
import { ScreeningsService } from '../../services/screenings.service';
import { AuditoriumsService } from '../../services/auditoriums.service';
import { Booking } from '../../models/booking';
import { Movie } from '../../models/movie';
import { Screening } from '../../models/screening';
import { Auditorium } from '../../models/auditorium';

type BookingViewModel = {
    id: string;
    movieTitle: string;
    posterUrl?: string | null;
    startTimeIso: string;
    startTimeFormatted: string;
    auditoriumName?: string;
    seatLabels: string[];
    totalPrice: number;
};

@Component({
    selector: 'app-my-bookings',
    standalone: true,
    imports: [CommonModule, RouterModule, MatIconModule],
    templateUrl: './my-bookings.component.html',
    styleUrls: ['./my-bookings.component.scss'],
})
export class MyBookingsComponent {
    private readonly auth = inject(AuthService);
    private readonly bookingsService = inject(BookingsService);
    private readonly moviesService = inject(MoviesService);
    private readonly screeningsService = inject(ScreeningsService);
    private readonly auditoriumsService = inject(AuditoriumsService);

    protected readonly isLoading = signal<boolean>(true);
    protected readonly bookings = signal<Booking[]>([]);

    protected readonly screenings = signal<Screening[]>([]);
    protected readonly movies = signal<Movie[]>([]);
    protected readonly auditoriums = signal<Auditorium[]>([]);

    protected readonly vm = computed<BookingViewModel[]>(() => {
        const bookings = this.bookings();
        const screeningsById = new Map(this.screenings().map((s) => [s.id, s]));
        const moviesById = new Map(this.movies().map((m) => [m.id, m]));
        const audsById = new Map(this.auditoriums().map((a) => [a.id, a]));

        return bookings.map((b) => {
            const s = screeningsById.get(b.screeningId);
            const m = s ? moviesById.get(s.movieId) : undefined;
            const a = s ? audsById.get(s.auditoriumId) : undefined;
            const startIso = s?.startTime ?? '';

            return {
                id: b.id,
                movieTitle: m?.title ?? 'Untitled movie',
                posterUrl: m?.posterUrl ?? null,
                startTimeIso: startIso,
                startTimeFormatted: this.formatDateTime(startIso),
                auditoriumName: a?.name,
                seatLabels: (b.seats ?? []).map((x) => `R${x.row}-S${x.number}`),
                totalPrice: b.totalPrice,
            };
        });
    });

    constructor() {
        const userId = this.auth.userId();

        if (!userId) {
            // Guard should prevent reaching here, but fail-safe
            this.isLoading.set(false);
            return;
        }

        // Parallelize lookups for nicer labels
        this.isLoading.set(true);

        this.bookingsService.listByUser(userId).subscribe((items) => {
            this.bookings.set(items ?? []);
            // Load lookups after bookings (but not strictly required)
            this.screeningsService.list().subscribe((ss) => this.screenings.set(ss ?? []));
            this.moviesService.list().subscribe((mm) => this.movies.set(mm ?? []));
            this.auditoriumsService.list().subscribe((aa) => this.auditoriums.set(aa ?? []));
            this.isLoading.set(false);
        });
    }

    protected formatDateTime(iso: string): string {
        if (!iso) return '';

        const d = new Date(iso);

        if (Number.isNaN(d.getTime())) return iso;

        return d.toLocaleString([], { dateStyle: 'medium', timeStyle: 'short', hour12: false });
    }
}
