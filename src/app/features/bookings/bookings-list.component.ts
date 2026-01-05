import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { BookingsService } from '../../services/bookings.service';
import { ScreeningsService } from '../../services/screenings.service';
import { AuditoriumsService } from '../../services/auditoriums.service';
import { Booking } from '../../models/booking';
import { Screening } from '../../models/screening';
type SeatCell = { id: string; auditoriumId: string; row: number; number: number };
import { v4 as uuidv4 } from 'uuid';
import { AuthService } from '../../core/auth/auth.service';
import { UsersService } from '../../services/users.service';
import { Router } from '@angular/router';

@Component({
    selector: 'app-bookings-list',
    standalone: true,
    imports: [
        CommonModule,
        RouterModule,
        FormsModule,
        MatButtonModule,
        MatFormFieldModule,
        MatInputModule,
        MatTableModule,
    ],
    template: `
        <section>
            <h1>Bookings</h1>
            <form
                (ngSubmit)="loadScreening()"
                style="display:flex; gap:8px; align-items:center; flex-wrap:wrap; margin:8px 0;"
            >
                <mat-form-field appearance="outline">
                    <mat-label>Screening ID</mat-label>

                    <input matInput name="screeningId" [(ngModel)]="screeningId" required />
                </mat-form-field>

                <button mat-raised-button color="primary" type="submit">Load</button>
            </form>

            <div *ngIf="screening() as s">
                <p>
                    <strong>Screening:</strong> {{ s.movieId }} in {{ s.auditoriumId }} at
                    {{ formatDateTime(s.startTime) }} — {{ s.price }} DKK
                </p>

                <div
                    style="display:grid; gap:4px;"
                    [style.gridTemplateColumns]="'repeat(' + cols + ', 32px)'"
                >
                    <button
                        *ngFor="let seat of seats()"
                        [style.width.px]="32"
                        [style.height.px]="32"
                        [style.background]="isSelected(seat.id) ? '#3f51b5' : '#e0e0e0'"
                        [style.color]="isSelected(seat.id) ? 'white' : 'black'"
                        (click)="toggleSeat(seat.id)"
                        mat-button
                    >
                        {{ seat.row }}-{{ seat.number }}
                    </button>
                </div>

                <div style="margin-top:8px;">
                    <button mat-raised-button color="accent" (click)="confirm()">
                        Confirm Booking
                    </button>
                </div>
            </div>

            <h2 style="margin-top:16px;">All Bookings</h2>

            <table mat-table [dataSource]="bookings()" class="mat-elevation-z1" style="width:100%;">
                <ng-container matColumnDef="id">
                    <th mat-header-cell *matHeaderCellDef>ID</th>

                    <td mat-cell *matCellDef="let b">{{ b.id }}</td>
                </ng-container>

                <ng-container matColumnDef="screeningId">
                    <th mat-header-cell *matHeaderCellDef>Screening</th>

                    <td mat-cell *matCellDef="let b">{{ b.screeningId }}</td>
                </ng-container>

                <ng-container matColumnDef="userId">
                    <th mat-header-cell *matHeaderCellDef>User</th>

                    <td mat-cell *matCellDef="let b">{{ b.userId }}</td>
                </ng-container>

                <ng-container matColumnDef="seats">
                    <th mat-header-cell *matHeaderCellDef>Seats</th>

                    <td mat-cell *matCellDef="let b">{{ displaySeats(b) }}</td>
                </ng-container>

                <ng-container matColumnDef="price">
                    <th mat-header-cell *matHeaderCellDef>Total</th>

                    <td mat-cell *matCellDef="let b">{{ b.totalPrice }}</td>
                </ng-container>

                <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>

                <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
            </table>
        </section>
    `,
})
export class BookingsListComponent {
    private readonly bookingsService = inject(BookingsService);
    private readonly screeningsService = inject(ScreeningsService);
    private readonly auditoriumsService = inject(AuditoriumsService);
    private readonly auth = inject(AuthService);
    private readonly usersService = inject(UsersService);
    private readonly router = inject(Router);

    protected readonly bookings = signal<Booking[]>([]);
    protected readonly screening = signal<Screening | null>(null);
    protected readonly seats = signal<SeatCell[]>([]);
    protected readonly displayedColumns = ['id', 'screeningId', 'userId', 'seats', 'price'];
    protected screeningId = '';
    protected cols = 12;
    private selected = new Set<string>();

    constructor() {
        this.reloadBookings();
    }

    reloadBookings(): void {
        this.bookingsService.list().subscribe((items) => this.bookings.set(items));
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
