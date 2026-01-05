import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

type AnyResult = unknown;

@Component({
    selector: 'app-hack-sql',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './hack-sql.component.html',
    styleUrls: ['./hack-sql.component.scss'],
})
export class HackSqlComponent {
    private readonly http = inject(HttpClient);

    rawSql = '';
    output: AnyResult = null;
    loading = false;

    run(): void {
        this.loading = true;
        this.http.post<AnyResult>(`/hackapi/hack/sql`, { sql: this.rawSql }).subscribe({
            next: (r) => (this.output = r),
            error: (e) => (this.output = e?.error ?? e?.message ?? 'Request failed'),
            complete: () => (this.loading = false),
        });
    }
}
