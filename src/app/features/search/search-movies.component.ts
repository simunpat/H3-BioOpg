import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatTableModule } from '@angular/material/table';
import { MockElasticSearchService } from '../../search/mock-elastic-search.service';
import { Movie } from '../../models/movie';

@Component({
    selector: 'app-search-movies',
    standalone: true,
    imports: [CommonModule, FormsModule, MatFormFieldModule, MatInputModule, MatTableModule],
    template: `
        <section>
            <h1>Search Movies</h1>

            <mat-form-field appearance="outline" style="width:320px;">
                <mat-label>Query</mat-label>

                <input matInput [(ngModel)]="query" (ngModelChange)="run()" />
            </mat-form-field>

            <table mat-table [dataSource]="results()" class="mat-elevation-z1" style="width:100%;">
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

                <tr mat-header-row *matHeaderRowDef="displayedColumns"></tr>

                <tr mat-row *matRowDef="let row; columns: displayedColumns"></tr>
            </table>
        </section>
    `,
})
export class SearchMoviesComponent {
    private readonly search = inject(MockElasticSearchService);
    protected query = '';
    protected readonly results = signal<Movie[]>([]);
    protected readonly displayedColumns = ['title', 'genre', 'durationMin'];

    constructor() {
        this.run();
    }

    async run(): Promise<void> {
        const res = await this.search.searchMovies({
            index: 'movies',
            query: this.query,
            size: 20,
        });

        this.results.set(res.hits.hits.map((h) => h._source));
    }
}
