import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ScreeningsService } from '../../services/screenings.service';
import { AuditoriumsService } from '../../services/auditoriums.service';
import { BookingsService } from '../../services/bookings.service';
import { UsersService } from '../../services/users.service';
import { AuthService } from '../../core/auth/auth.service';
import { Screening } from '../../models/screening';
import { Booking } from '../../models/booking';
import { v4 as uuidv4 } from 'uuid';
import { MoviesService } from '../../services/movies.service';

type SeatCell = { id: string; auditoriumId: string; row: number; number: number };
import { TicketTypesService } from '../../services/ticket-types.service';
import { TicketType } from '../../models/ticket-type';

@Component({
    selector: 'app-booking-screen',
    standalone: true,
    imports: [CommonModule, RouterModule, FormsModule],
    templateUrl: './booking-screen.component.html',
    styleUrls: ['./booking-screen.component.scss'],
})
export class BookingScreenComponent {
    private readonly route = inject(ActivatedRoute);
    private readonly router = inject(Router);
    private readonly screeningsService = inject(ScreeningsService);
    private readonly auditoriumsService = inject(AuditoriumsService);
    private readonly bookingsService = inject(BookingsService);
    private readonly ticketTypesService = inject(TicketTypesService);
    private readonly moviesService = inject(MoviesService);
    private readonly usersService = inject(UsersService);
    private readonly auth = inject(AuthService);

    protected readonly screening = signal<Screening | null>(null);
    protected readonly seats = signal<SeatCell[]>([]);
    protected readonly occupiedSeatIds = signal<Set<string>>(new Set());
    protected readonly ticketTypes = signal<TicketType[]>([]);
    protected readonly movieTitle = signal<string>('');
    protected readonly auditoriumName = signal<string>('');
    protected typeCounts: Record<string, number> = {};
    protected cols = 12;
    private selected = new Set<string>();

    constructor() {
        const id = this.route.snapshot.paramMap.get('id');

        if (!id) {
            void this.router.navigate(['/movies']);
            return;
        }

        // Load screening (fallback to list().find to avoid service changes order dependency)
        this.screeningsService.list().subscribe((all) => {
            const s = all.find((x) => x.id === id) ?? null;

            this.screening.set(s);

            if (!s) return;

            // Load movie title
            this.moviesService.get(s.movieId).subscribe((m) => {
                this.movieTitle.set(m?.title ?? '');
            });

            // Load ticket types and init counts
            this.ticketTypesService.list().subscribe((types) => {
                this.ticketTypes.set(types);
                this.typeCounts = Object.fromEntries(types.map((t) => [t.id, 0]));

                const adult = types.find((t) => t.name.toLowerCase().includes('adult'));

                if (adult) this.typeCounts[adult.id] = 1;
            });

            // Load seats for auditorium from rows/cols
            this.auditoriumsService.list().subscribe((auds) => {
                const aud = auds.find((a) => a.id === s.auditoriumId);
                if (!aud) return;
                this.cols = aud.cols;
                this.auditoriumName.set(aud.name);
                this.seats.set(this.generateSeats(aud.id, aud.rows, aud.cols));
            });

            // Load bookings for this screening to compute occupied seats
            this.bookingsService.listByScreening(s.id).subscribe((bookings) => {
                const taken = bookings.flatMap((b) =>
                    b.seats.map((sel) => `${s.auditoriumId}-r${sel.row}-${sel.number}`)
                );
                this.occupiedSeatIds.set(new Set(taken));
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

    validTickets(): boolean {
        const n = this.totalTickets();

        return Number.isFinite(n) && n >= 1 && n <= 20;
    }

    isOccupied(id: string): boolean {
        return this.occupiedSeatIds().has(id);
    }

    isSelected(id: string): boolean {
        return this.selected.has(id);
    }

    selectedSize(): number {
        return this.selected.size;
    }

    totalTickets(): number {
        return Object.values(this.typeCounts).reduce((a, b) => a + (Number.isFinite(b) ? b : 0), 0);
    }

    toggleSeat(id: string): void {
        if (this.isOccupied(id)) return;

        if (this.selected.has(id)) {
            this.selected.delete(id);
        } else {
            if (this.selected.size >= this.totalTickets()) return;

            this.selected.add(id);
        }
    }

    totalPrice(): number {
        const s = this.screening();

        if (!s) return 0;

        const types = this.ticketTypes();
        const map = new Map(types.map((t) => [t.id, t]));

        let total = 0;

        for (const [typeId, qty] of Object.entries(this.typeCounts)) {
            const t = map.get(typeId);

            if (!t || !qty) continue;

            total += Math.round(qty * s.price * t.multiplier);
        }

        return total;
    }

    confirm(): void {
        const s = this.screening();

        if (!s) return;

        if (this.selected.size !== this.totalTickets()) return;

        const token = this.auth.getToken();

        if (!token) {
            void this.router.navigate(['/login']);
            return;
        }

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

            const items = Object.entries(this.typeCounts)
                .filter(([, qty]) => (qty ?? 0) > 0)
                .map(([ticketTypeId, qty]) => ({ ticketTypeId, qty: Number(qty) }));

            const booking: Booking = {
                id: uuidv4(),
                screeningId: s.id,
                userId: user.id,
                seats,
                items,
                totalPrice: seats.length * s.price,
            };

            booking.totalPrice = this.totalPrice();

            this.bookingsService.create(booking).subscribe(() => {
                void this.router.navigate(['/bookings', booking.id, 'confirmation']);
            });
        });
    }

    formatDateTime(iso: string): string {
        const d = new Date(iso);

        if (Number.isNaN(d.getTime())) return iso;

        const date = d.toLocaleDateString([], { month: 'short', day: 'numeric' });

        const time = d.toLocaleTimeString([], {
            hour: '2-digit',
            minute: '2-digit',
            hour12: false,
        });

        return `${date}, ${time}`;
    }

    ticketPriceFor(kind: 'adult' | 'child'): number {
        const s = this.screening();

        if (!s) return 0;

        const t = this.ticketTypes().find((tt) => tt.name.toLowerCase().includes(kind));

        if (!t) return 0;

        return Math.round(s.price * t.multiplier);
    }
}
