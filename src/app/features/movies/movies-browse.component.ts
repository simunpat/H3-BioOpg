import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MoviesService } from '../../services/movies.service';
import { Movie } from '../../models/movie';
import { MockElasticSearchService } from '../../search/mock-elastic-search.service';

@Component({
    selector: 'app-movies-browse',
    standalone: true,
    imports: [CommonModule, RouterModule, FormsModule],
    templateUrl: './movies-browse.component.html',
    styleUrls: ['./movies-browse.component.scss'],
})
export class MoviesBrowseComponent {
    private readonly moviesService = inject(MoviesService);
    private readonly searchService = inject(MockElasticSearchService);
    protected readonly allResults = signal<Movie[]>([]);
    protected readonly query = signal<string>('');
    protected readonly showAll = signal<boolean>(false);
    protected readonly movies = computed<Movie[]>(() =>
        this.showAll() ? this.allResults() : this.allResults().slice(0, 10)
    );

    constructor() {
        void this.runSearch();
    }

    posterUrl(m: Movie): string {
        return m.posterUrl || '/uploads/posters/template-poster.png';
    }

    async runSearch(): Promise<void> {
        const res = await this.searchService.searchMovies({
            index: 'movies',
            query: this.query(),
            size: 1000,
        });

        this.allResults.set(res.hits.hits.map((h) => h._source));
        this.showAll.set(false);
    }

    async onQueryChange(value: string): Promise<void> {
        this.query.set(value);
        await this.runSearch();
    }

    viewMore(): void {
        this.showAll.set(true);
    }
}
