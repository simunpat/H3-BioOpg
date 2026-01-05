import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Screening } from '../models/screening';

const BASE_URL = '/api/screenings';

@Injectable({ providedIn: 'root' })
export class ScreeningsService {
    constructor(private readonly http: HttpClient) {}

    list(): Observable<Screening[]> {
        return this.http.get<Screening[]>(BASE_URL);
    }

    getById(id: string): Observable<Screening> {
        return this.http.get<Screening>(`${BASE_URL}/${id}`);
    }

    create(s: Screening): Observable<Screening> {
        return this.http.post<Screening>(BASE_URL, s);
    }

    update(s: Screening): Observable<Screening> {
        return this.http.put<Screening>(`${BASE_URL}/${s.id}`, s);
    }

    delete(id: string): Observable<void> {
        return this.http.delete<void>(`${BASE_URL}/${id}`);
    }
}
