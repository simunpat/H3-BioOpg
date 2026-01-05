import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../models/user';

const BASE_URL = '/api/users';

@Injectable({ providedIn: 'root' })
export class UsersService {
    constructor(private readonly http: HttpClient) {}

    list(): Observable<User[]> {
        return this.http.get<User[]>(BASE_URL);
    }

    get(id: string): Observable<User> {
        return this.http.get<User>(`${BASE_URL}/${id}`);
    }

    findByEmail(email: string): Observable<User[]> {
        const params = new HttpParams().set('email', email);
        return this.http.get<User[]>(BASE_URL, { params });
    }

    create(user: User): Observable<User> {
        return this.http.post<User>(BASE_URL, user);
    }

    update(user: User): Observable<User> {
        return this.http.put<User>(`${BASE_URL}/${user.id}`, user);
    }

    delete(id: string): Observable<void> {
        return this.http.delete<void>(`${BASE_URL}/${id}`);
    }
}
