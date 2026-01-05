import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Auditorium } from '../models/auditorium';

const BASE_URL = '/api/auditoriums';

@Injectable({ providedIn: 'root' })
export class AuditoriumsService {
    constructor(private readonly http: HttpClient) {}

    list(): Observable<Auditorium[]> {
        return this.http.get<Auditorium[]>(BASE_URL);
    }

    create(aud: Auditorium): Observable<Auditorium> {
        return this.http.post<Auditorium>(BASE_URL, aud);
    }

    update(aud: Auditorium): Observable<Auditorium> {
        return this.http.put<Auditorium>(`${BASE_URL}/${aud.id}`, aud);
    }

    delete(id: string): Observable<void> {
        return this.http.delete<void>(`${BASE_URL}/${id}`);
    }
}
