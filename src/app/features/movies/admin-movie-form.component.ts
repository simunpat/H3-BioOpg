import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MoviesService } from '../../services/movies.service';
import { UploadService } from '../../services/upload.service';
import { Movie } from '../../models/movie';
import { v4 as uuidv4 } from 'uuid';
import Dropzone from 'dropzone';
(Dropzone as any).autoDiscover = false;

@Component({
    selector: 'app-admin-movie-form',
    standalone: true,
    imports: [CommonModule, RouterModule, FormsModule],
    templateUrl: './admin-movie-form.component.html',
    styleUrls: ['./admin-movie-form.component.scss'],
})
export class AdminMovieFormComponent {
    private readonly route = inject(ActivatedRoute);
    private readonly router = inject(Router);
    private readonly movies = inject(MoviesService);
    private readonly upload = inject(UploadService);

    protected form: Partial<Movie> = { title: '', genre: '', durationMin: 100 };
    protected editingId: string | null = null;
    private dropzoneInstance: Dropzone | null = null;
    protected posterFile: File | null = null;

    constructor() {
        const id = this.route.snapshot.paramMap.get('id');

        if (id) {
            this.editingId = id;
            this.movies.get(id).subscribe({
                next: (m) => {
                    if (!m) return;
                    this.form = {
                        title: m.title,
                        genre: m.genre,
                        durationMin: m.durationMin,
                        posterUrl: m.posterUrl,
                    };
                },
                error: (err) => {
                    // eslint-disable-next-line no-console
                    console.error('Failed to load movie', err);
                    alert(`Failed to load movie. ${err?.error?.message || err?.message || ''}`);
                },
            });
        }
    }

    ngAfterViewInit(): void {
        const el = document.getElementById('poster-dropzone');

        if (!el) return;

        this.dropzoneInstance = new Dropzone(el, {
            url: '/noop',
            autoProcessQueue: false,
            maxFiles: 1,
            acceptedFiles: 'image/jpeg,image/png',
            maxFilesize: 5,
            addRemoveLinks: true,
        });

        const dz = this.dropzoneInstance!;

        dz.on('addedfile', (file: File) => {
            this.posterFile = file;

            const dzAny: any = this.dropzoneInstance!;

            if (dzAny.files.length > 1) {
                dzAny.removeFile(dzAny.files[0]);
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

    cancel(): void {
        void this.router.navigate(['/admin/movies']);
    }

    save(): void {
        if (!this.form.title || !this.form.genre || !this.form.durationMin) return;

        if (this.editingId) {
            const base: Movie = {
                id: this.editingId,
                title: this.form.title,
                durationMin: Number(this.form.durationMin),
                genre: this.form.genre,
                posterUrl: this.form.posterUrl,
            };

            const doUpdate = (m: Movie) =>
                this.movies.update(m).subscribe({
                    next: () => void this.router.navigate(['/admin/movies']),
                    error: (err) => {
                        // eslint-disable-next-line no-console
                        console.error('Failed to update movie', err);
                        alert(`Failed to update movie. ${err?.error?.message || err?.message || ''}`);
                    },
                });

            if (this.posterFile) {
                this.upload.uploadPoster(this.posterFile).subscribe({
                    next: ({ url }) => doUpdate({ ...base, posterUrl: url }),
                    error: (err) => {
                        // eslint-disable-next-line no-console
                        console.error('Poster upload failed', err);
                        alert(
                            `Poster upload failed (JPG/PNG, max 5MB). ${err?.error?.error || err?.message || ''}. Saving without changing poster.`
                        );
                        doUpdate(base);
                    },
                });
            } else {
                doUpdate(base);
            }
        } else {
            const movie: Movie = {
                id: uuidv4(),
                title: this.form.title,
                durationMin: Number(this.form.durationMin),
                genre: this.form.genre,
                posterUrl: this.form.posterUrl,
            };

            const afterCreate = () => {
                void this.router.navigate(['/admin/movies']);
            };

            if (this.posterFile) {
                this.upload.uploadPoster(this.posterFile).subscribe({
                    next: ({ url }) =>
                        this.movies.create({ ...movie, posterUrl: url }).subscribe({
                            next: afterCreate,
                            error: (err) => {
                                // eslint-disable-next-line no-console
                                console.error('Failed to create movie', err);
                                alert(`Failed to create movie. ${err?.error?.message || err?.message || ''}`);
                            },
                        }),
                    error: (err) => {
                        // eslint-disable-next-line no-console
                        console.error('Poster upload failed', err);
                        alert(
                            `Poster upload failed (JPG/PNG, max 5MB). ${err?.error?.error || err?.message || ''}. Creating movie without poster.`
                        );
                        this.movies.create(movie).subscribe({
                            next: afterCreate,
                            error: (e2) => {
                                // eslint-disable-next-line no-console
                                console.error('Failed to create movie', e2);
                                alert(`Failed to create movie. ${e2?.error?.message || e2?.message || ''}`);
                            },
                        });
                    },
                });
            } else {
                this.movies.create(movie).subscribe({
                    next: afterCreate,
                    error: (err) => {
                        // eslint-disable-next-line no-console
                        console.error('Failed to create movie', err);
                        alert(`Failed to create movie. ${err?.error?.message || err?.message || ''}`);
                    },
                });
            }
        }
    }
}
