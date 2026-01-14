import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BookingsService } from '../../services/bookings.service';
import { ScreeningsService } from '../../services/screenings.service';
import { AuditoriumsService } from '../../services/auditoriums.service';
import { MoviesService } from '../../services/movies.service';
import { Booking } from '../../models/booking';
import { Screening } from '../../models/screening';
import { Movie } from '../../models/movie';
import { User } from '../../models/user';
type SeatCell = { id: string; auditoriumId: string; row: number; number: number };
import { v4 as uuidv4 } from 'uuid';
import { AuthService } from '../../core/auth/auth.service';
import { UsersService } from '../../services/users.service';
import { Router } from '@angular/router';

@Component({
    selector: 'app-bookings-list',
    standalone: true,
    imports: [CommonModule, RouterModule, FormsModule],
    templateUrl: './bookings-list.component.html',
})
export class BookingsListComponent {
    private readonly bookingsService = inject(BookingsService);
    private readonly screeningsService = inject(ScreeningsService);
    private readonly auditoriumsService = inject(AuditoriumsService);
    private readonly moviesService = inject(MoviesService);
    private readonly auth = inject(AuthService);
    private readonly usersService = inject(UsersService);
    private readonly router = inject(Router);

    protected readonly bookings = signal<Booking[]>([]);
    protected readonly screening = signal<Screening | null>(null);
    protected readonly screeningsAll = signal<Screening[]>([]);
    protected readonly seats = signal<SeatCell[]>([]);
    protected readonly displayedColumns = ['movie', 'user', 'seats', 'price'];
    protected readonly pageSize = 10;
    protected readonly pageIndex = signal(0);

    protected readonly totalPages = computed(() =>
        Math.max(1, Math.ceil(this.bookings().length / this.pageSize))
    );

    protected readonly pagedBookings = computed(() => {
        const start = this.pageIndex() * this.pageSize;
        return this.bookings().slice(start, start + this.pageSize);
    });

    protected screeningId = '';
    protected cols = 12;
    private selected = new Set<string>();
    protected readonly movies = signal<Movie[]>([]);
    protected readonly users = signal<User[]>([]);

    constructor() {
        this.reloadBookings();
        // Preload lookups for nicer overview labels
        this.moviesService.list().subscribe((items) => this.movies.set(items ?? []));
        this.usersService.list().subscribe((items) => this.users.set(items ?? []));
        this.screeningsService.list().subscribe((items) => this.screeningsAll.set(items ?? []));
    }

    reloadBookings(): void {
        this.bookingsService.list().subscribe((items) => {
            this.bookings.set(items);
            this.pageIndex.set(0);
        });
    }

    remove(id: string): void {
        this.bookingsService.delete(id).subscribe(() => this.reloadBookings());
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

    movieTitleForScreening(screeningId: string): string {
        const s = this.screeningsAll().find((x) => x.id === screeningId);

        if (!s) return screeningId;

        const m = this.movies().find((mm) => mm.id === s.movieId);
        return m?.title ?? s.movieId;
    }

    userEmail(userId: string): string {
        const u = this.users().find((x) => x.id === userId);

        return u?.email ?? userId;
    }

    loadScreening(): void {
        this.screeningsService.list().subscribe((list) => {
            const s = list.find((x) => x.id === this.screeningId) ?? null;

            this.screening.set(s);

            if (!s) return;

            this.auditoriumsService.list().subscribe((auds) => {
                const aud = auds.find((a) => a.id === s.auditoriumId);

                if (!aud) return;

                this.cols = aud.cols;
                this.seats.set(this.generateSeats(aud.id, aud.rows, aud.cols));
                this.selected.clear();
            });
        });
    }

    private generateSeats(auditoriumId: string, rows: number, cols: number): SeatCell[] {
        const result: SeatCell[] = [];

        for (let r = 1; r <= rows; r++) {
            for (let c = 1; c <= cols; c++) {
                result.push({
                    id: `${auditoriumId}-r${r}-${c}`,
                    auditoriumId,
                    row: r,
                    number: c,
                });
            }
        }

        return result;
    }

    isSelected(id: string): boolean {
        return this.selected.has(id);
    }

    toggleSeat(id: string): void {
        if (this.selected.has(id)) this.selected.delete(id);
        else this.selected.add(id);
    }

    confirm(): void {
        const s = this.screening();

        if (!s || this.selected.size === 0) return;

        const token = this.auth.getToken();

        if (!token) {
            void this.router.navigate(['/login']);

            return;
        }

        // Ensure the logged-in user exists (registered account)
        this.usersService.get(token.sub).subscribe((user) => {
            if (!user) {
                this.auth.logout();

                return;
            }

            const seatIds = Array.from(this.selected.values());
            const seatMap = new Map(this.seats().map((s) => [s.id, s]));
            const seats = seatIds
                .map((id) => seatMap.get(id))
                .filter((s): s is SeatCell => !!s)
                .map((s) => ({ row: s.row, number: s.number }));

            const b: Booking = {
                id: uuidv4(),
                screeningId: s.id,
                userId: user.id,
                seats,
                totalPrice: seats.length * s.price,
            };

            this.bookingsService.create(b).subscribe(() => {
                this.selected.clear();
                this.reloadBookings();
            });
        });
    }

    displaySeats(b: Booking): string {
        const arr = b?.seats ?? [];

        return arr.map((x) => `${x.row}-${x.number}`).join(', ');
    }

    formatDateTime(iso: string): string {
        const d = new Date(iso);

        if (Number.isNaN(d.getTime())) return iso;

        return d.toLocaleString([], { dateStyle: 'medium', timeStyle: 'short', hour12: false });
    }
}
