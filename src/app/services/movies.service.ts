import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Movie } from '../models/movie';

const BASE_URL = '/api/movies';

@Injectable({ providedIn: 'root' })
export class MoviesService {
    constructor(private readonly http: HttpClient) {}

    list(): Observable<Movie[]> {
        return this.http.get<Movie[]>(BASE_URL);
    }

    get(id: string): Observable<Movie> {
        return this.http.get<Movie>(`${BASE_URL}/${id}`);
    }

    create(movie: Movie): Observable<Movie> {
        return this.http.post<Movie>(BASE_URL, movie);
    }

    update(movie: Movie): Observable<Movie> {
        return this.http.put<Movie>(`${BASE_URL}/${movie.id}`, movie);
    }

    delete(id: string): Observable<void> {
        return this.http.delete<void>(`${BASE_URL}/${id}`);
    }
}
