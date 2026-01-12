import { Component, inject, signal, AfterViewInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MoviesService } from '../../services/movies.service';
import { Movie } from '../../models/movie';
import { v4 as uuidv4 } from 'uuid';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../core/auth/auth.service';
import { UploadService } from '../../services/upload.service';
import Dropzone from 'dropzone';
(Dropzone as any).autoDiscover = false;

@Component({
    selector: 'app-movies-list',
    standalone: true,
    imports: [
        CommonModule,
        RouterModule,
        FormsModule,
        MatTableModule,
        MatButtonModule,
        MatFormFieldModule,
        MatInputModule,
    ],
    template: `
        <section>
            <h1>Movies</h1>
            <form
                *ngIf="auth.isAdmin()"
                (ngSubmit)="save()"
                style="display: flex; gap: 8px; align-items: center; flex-wrap: wrap; margin: 8px 0;"
            >
                <mat-form-field appearance="outline">
                    <mat-label>Title</mat-label>

                    <input matInput name="title" [(ngModel)]="form.title" required />
                </mat-form-field>

                <mat-form-field appearance="outline">
                    <mat-label>Genre</mat-label>

                    <input matInput name="genre" [(ngModel)]="form.genre" required />
                </mat-form-field>

                <mat-form-field appearance="outline">
                    <mat-label>Duration (min)</mat-label>

                    <input
                        matInput
                        type="number"
                        name="durationMin"
                        [(ngModel)]="form.durationMin"
                        required
                    />
                </mat-form-field>

                <div id="poster-dropzone" class="dropzone">
                    <div class="dz-message">Drop poster here or click to upload</div>
                </div>

                <button mat-raised-button color="primary" type="submit">
                    {{ editingId ? 'Save' : 'Add' }}
                </button>

                <button *ngIf="editingId" mat-button type="button" (click)="cancelEdit()">
                    Cancel
                </button>
            </form>

            <table mat-table [dataSource]="movies()" class="mat-elevation-z1" style="width: 100%;">
                <ng-container matColumnDef="poster">
                    <th mat-header-cell *matHeaderCellDef>Poster</th>
                    <td mat-cell *matCellDef="let m">
                        <img
                            *ngIf="m.posterUrl"
                            [src]="m.posterUrl"
                            alt="{{ m.title }} poster"
                            style="width: 48px; height: 72px; object-fit: cover; border-radius: 4px;"
                        />
                    </td>
                </ng-container>
                <ng-container matColumnDef="title">
                    <th mat-header-cell *matHeaderCellDef>Title</th>

                    <td mat-cell *matCellDef="let m">{{ m.title }}</td>
                </ng-container>

                <ng-container matColumnDef="genre">
                    <th mat-header-cell *matHeaderCellDef>Genre</th>

                    <td mat-cell *matCellDef="let m">{{ m.genre }}</td>
                </ng-container>

                <ng-container matColumnDef="durationMin">
                    <th mat-header-cell *matHeaderCellDef>Duration</th>

                    <td mat-cell *matCellDef="let m">{{ m.durationMin }}</td>
                </ng-container>

                <ng-container matColumnDef="actions">
                    <th mat-header-cell *matHeaderCellDef>
                        <span *ngIf="auth.isAdmin()">Actions</span>
                    </th>

                    <td mat-cell *matCellDef="let m">
                        <a mat-button [routerLink]="['/movies', m.id]">View</a>
                        <ng-container *ngIf="auth.isAdmin()">
                            <button mat-button (click)="startEdit(m)">Edit</button>
                            <button mat-button color="warn" (click)="remove(m.id)">Delete</button>
                        </ng-container>
                    </td>
                </ng-container>

                <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>

                <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
            </table>
        </section>
    `,
})
export class MoviesListComponent implements AfterViewInit, OnDestroy {
    private readonly service = inject(MoviesService);
    private readonly uploadService = inject(UploadService);
    protected readonly auth = inject(AuthService);
    protected readonly movies = signal<Movie[]>([]);
    protected readonly displayedColumns = ['poster', 'title', 'genre', 'durationMin', 'actions'];
    protected form: Partial<Movie> = { title: '', genre: '', durationMin: 100 };
    protected editingId: string | null = null;
    private dropzoneInstance: Dropzone | null = null;
    protected posterFile: File | null = null;

    constructor() {
        this.refresh();
    }

    ngAfterViewInit(): void {
        const el = document.getElementById('poster-dropzone');
        if (!el) return;

        this.dropzoneInstance = new Dropzone(el, {
            url: '/noop', // not used; we upload manually on submit
            autoProcessQueue: false,
            maxFiles: 1,
            acceptedFiles: 'image/jpeg,image/png',
            maxFilesize: 5,
            addRemoveLinks: true,
        });

        const dz = this.dropzoneInstance!;

        dz.on('addedfile', (file: File) => {
            this.posterFile = file;
            // Ensure only one file stays in the UI
            const dz: any = this.dropzoneInstance!;

            if (dz.files.length > 1) {
                dz.removeFile(dz.files[0]);
            }
        });

        dz.on('removedfile', () => {
            this.posterFile = null;
        });
    }

    ngOnDestroy(): void {
        if (this.dropzoneInstance) {
            this.dropzoneInstance.destroy();
            this.dropzoneInstance = null;
        }
    }

    refresh(): void {
        this.service.list().subscribe((items) => this.movies.set(items));
    }

    startEdit(m: Movie): void {
        this.editingId = m.id;
        this.form = { title: m.title, genre: m.genre, durationMin: m.durationMin };
    }

    cancelEdit(): void {
        this.editingId = null;
        this.form = { title: '', genre: '', durationMin: 100 };
    }

    save(): void {
        if (!this.form.title || !this.form.genre || !this.form.durationMin) return;
        if (!this.auth.isAdmin()) return;

        if (this.editingId) {
            const movie: Movie = {
                id: this.editingId,
                title: this.form.title,
                durationMin: Number(this.form.durationMin),
                genre: this.form.genre,
            };

            const doUpdate = (m: Movie) =>
                this.service.update(m).subscribe(() => {
                    this.cancelEdit();
                    this.refresh();
                });

            if (this.posterFile) {
                this.uploadService.uploadPoster(this.posterFile).subscribe({
                    next: ({ url }) => doUpdate({ ...movie, posterUrl: url }),
                    error: (err) => {
                        const msg =
                            err?.error?.error ||
                            err?.message ||
                            `HTTP ${err?.status ?? ''} during upload`;

                        // eslint-disable-next-line no-console
                        console.error('Poster upload failed:', err);

                        alert(
                            `Poster upload failed (JPG/PNG, max 5MB). ${msg}. Saving without changing poster.`
                        );
                        doUpdate(movie);
                    },
                });
            } else {
                doUpdate(movie);
            }
        } else {
            this.add();
        }
    }

    add(): void {
        if (!this.form.title || !this.form.genre || !this.form.durationMin) return;

        if (!this.auth.isAdmin()) return;

        const movie: Movie = {
            id: uuidv4(),
            title: this.form.title,
            durationMin: Number(this.form.durationMin),
            genre: this.form.genre,
        };

        const afterCreate = () => {
            this.form = { title: '', genre: '', durationMin: 100 };

            if (this.dropzoneInstance) this.dropzoneInstance.removeAllFiles(true);

            this.posterFile = null;
            this.refresh();
        };

        if (this.posterFile) {
            this.uploadService.uploadPoster(this.posterFile).subscribe({
                next: ({ url }) =>
                    this.service.create({ ...movie, posterUrl: url }).subscribe(afterCreate),
                error: (err) => {
                    const msg =
                        err?.error?.error ||
                        err?.message ||
                        `HTTP ${err?.status ?? ''} during upload`;

                    // eslint-disable-next-line no-console
                    console.error('Poster upload failed:', err);

                    alert(
                        `Poster upload failed (JPG/PNG, max 5MB). ${msg}. Creating movie without poster.`
                    );
                    this.service.create(movie).subscribe(afterCreate);
                },
            });
        } else {
            this.service.create(movie).subscribe(afterCreate);
        }
    }

    remove(id: string): void {
        if (!this.auth.isAdmin()) return;

        this.service.delete(id).subscribe(() => this.refresh());
    }
}
