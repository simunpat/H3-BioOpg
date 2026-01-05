import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TicketType } from '../models/ticket-type';

const BASE_URL = '/api/ticketTypes';

@Injectable({ providedIn: 'root' })
export class TicketTypesService {
    constructor(private readonly http: HttpClient) {}

    list(): Observable<TicketType[]> {
        return this.http.get<TicketType[]>(BASE_URL);
    }

    create(tt: TicketType): Observable<TicketType> {
        return this.http.post<TicketType>(BASE_URL, tt);
    }

    update(tt: TicketType): Observable<TicketType> {
        return this.http.put<TicketType>(`${BASE_URL}/${tt.id}`, tt);
    }

    delete(id: string): Observable<void> {
        return this.http.delete<void>(`${BASE_URL}/${id}`);
    }
}
