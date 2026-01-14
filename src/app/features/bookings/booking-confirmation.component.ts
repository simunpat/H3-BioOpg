import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { BookingsService } from '../../services/bookings.service';
import { ScreeningsService } from '../../services/screenings.service';
import { MoviesService } from '../../services/movies.service';
import { AuditoriumsService } from '../../services/auditoriums.service';
import { Booking } from '../../models/booking';
import { Screening } from '../../models/screening';

@Component({
    selector: 'app-booking-confirmation',
    standalone: true,
    imports: [CommonModule, RouterModule],
    templateUrl: './booking-confirmation.component.html',
    styleUrls: ['./booking-confirmation.component.scss'],
})
export class BookingConfirmationComponent {
    private readonly route = inject(ActivatedRoute);
    private readonly router = inject(Router);
    private readonly bookings = inject(BookingsService);
    private readonly screenings = inject(ScreeningsService);
    private readonly movies = inject(MoviesService);
    private readonly auditoriums = inject(AuditoriumsService);

    protected readonly booking = signal<Booking | null>(null);
    protected readonly screening = signal<Screening | null>(null);
    protected readonly movieTitle = signal<string>('');
    protected readonly auditoriumName = signal<string>('');

    constructor() {
        const id = this.route.snapshot.paramMap.get('id');

        if (!id) {
            void this.router.navigate(['/']);
            return;
        }

        this.bookings.get(id).subscribe({
            next: (b) => {
                this.booking.set(b);

                this.screenings.getById(b.screeningId).subscribe((s) => {
                    this.screening.set(s);

                    this.movies
                        .get(s.movieId)
                        .subscribe((m) => this.movieTitle.set(m?.title ?? ''));

                    this.auditoriums.list().subscribe((auds) => {
                        this.auditoriumName.set(
                            auds.find((a) => a.id === s.auditoriumId)?.name ?? ''
                        );
                    });
                });
            },
            error: () => {
                void this.router.navigate(['/']);
            },
        });
    }

    totalTickets(): number {
        const b = this.booking();

        if (!b) return 0;

        const items = b.items ?? [];

        return items.reduce((sum, i) => sum + (i.qty ?? 0), 0);
    }

    startText(): string {
        const s = this.screening();

        if (!s) return '';

        const d = new Date(s.startTime);

        if (Number.isNaN(d.getTime())) return s.startTime;
        const date = d.toLocaleDateString([], { month: 'short', day: 'numeric' });

        const time = d.toLocaleTimeString([], {
            hour: '2-digit',
            minute: '2-digit',
            hour12: false,
        });

        return `${date}, ${time}`;
    }
}
