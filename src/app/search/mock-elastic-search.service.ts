import { Injectable } from '@angular/core';
import { SearchService } from './search.service';
import { EsSearchRequest, EsSearchResponse } from './types';
import { Movie } from '../models/movie';
import { Screening } from '../models/screening';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class MockElasticSearchService implements SearchService {
    constructor(private readonly http: HttpClient) {}

    async searchMovies(req: EsSearchRequest): Promise<EsSearchResponse<Movie>> {
        const [all, screenings] = await Promise.all([
            firstValueFrom(this.http.get<Movie[]>('/api/movies')),
            firstValueFrom(this.http.get<Screening[]>('/api/screenings')),
        ]);

        const q = extractQueryString(req);

        // Build set of movies that have upcoming screenings within 0..6 days from today
        const today = new Date();
        const end = new Date(today);

        end.setDate(end.getDate() + 6);

        const movieIdsWithUpcoming = new Set(
            (screenings ?? [])
                .filter((s) => {
                    const d = new Date(s.startTime);
                    return !Number.isNaN(d.getTime()) && d >= today && d <= end;
                })
                .map((s) => s.movieId)
        );

        // Text filter first, then restrict to movies that have upcoming screenings
        const textFiltered = !q
            ? all
            : all.filter(
                  (m) => m.title.toLowerCase().includes(q) || m.genre.toLowerCase().includes(q)
              );

        const filtered = textFiltered.filter((m) => movieIdsWithUpcoming.has(m.id));

        return toHits<Movie>('movies', filtered, req);
    }
}

function extractQueryString(req: EsSearchRequest): string {
    const anyReq = req.query as any;

    if (typeof anyReq === 'string') return anyReq.toLowerCase();

    if (anyReq?.query) return String(anyReq.query).toLowerCase();

    return '';
}

function toHits<T>(index: string, items: T[], req: EsSearchRequest): EsSearchResponse<T> {
    const from = req.from ?? 0;
    const size = req.size ?? 10;
    const page = items.slice(from, from + size);

    return {
        hits: {
            total: { value: items.length },
            hits: page.map((it, i) => ({
                _index: index,
                _id: String(from + i),
                _source: it,
            })),
        },
    };
}
