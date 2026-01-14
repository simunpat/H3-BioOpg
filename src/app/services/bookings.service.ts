import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Booking } from '../models/booking';

const BASE_URL = '/api/bookings';

@Injectable({ providedIn: 'root' })
export class BookingsService {
    constructor(private readonly http: HttpClient) {}

    list(): Observable<Booking[]> {
        return this.http.get<Booking[]>(BASE_URL);
    }

    get(id: string): Observable<Booking> {
        return this.http.get<Booking>(`${BASE_URL}/${id}`);
    }

    listByScreening(screeningId: string): Observable<Booking[]> {
        const params = new HttpParams().set('screeningId', screeningId);
        return this.http.get<Booking[]>(BASE_URL, { params });
    }

    listByUser(userId: string): Observable<Booking[]> {
        const params = new HttpParams().set('userId', userId);
        return this.http.get<Booking[]>(BASE_URL, { params });
    }

    create(b: Booking): Observable<Booking> {
        return this.http.post<Booking>(BASE_URL, b);
    }

    update(b: Booking): Observable<Booking> {
        return this.http.put<Booking>(`${BASE_URL}/${b.id}`, b);
    }

    delete(id: string): Observable<void> {
        return this.http.delete<void>(`${BASE_URL}/${id}`);
    }
}
