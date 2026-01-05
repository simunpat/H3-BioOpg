import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class UploadService {
    constructor(private readonly http: HttpClient) {}

    uploadPoster(file: File): Observable<{ url: string }> {
        const form = new FormData();
        form.append('file', file);

        return this.http.post<{ url: string }>(`/upload/poster`, form);
    }
}
