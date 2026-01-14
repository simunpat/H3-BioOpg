import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuditoriumsService } from '../../services/auditoriums.service';
import { Auditorium } from '../../models/auditorium';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { v4 as uuidv4 } from 'uuid';

@Component({
    selector: 'app-auditoriums-list',
    standalone: true,
    imports: [CommonModule, RouterModule, FormsModule],
    templateUrl: './auditoriums-list.component.html',
    styleUrls: ['./auditoriums-list.component.scss'],
})
export class AuditoriumsListComponent {
    private readonly service = inject(AuditoriumsService);
    protected readonly auditoriums = signal<Auditorium[]>([]);
    protected readonly displayedColumns = ['name', 'size', 'actions'];
    protected form: Partial<Auditorium> = { name: '', rows: 10, cols: 12 };
    protected readonly pageSize = 10;
    protected readonly pageIndex = signal(0);

    protected readonly totalPages = computed(() =>
        Math.max(1, Math.ceil(this.auditoriums().length / this.pageSize))
    );

    protected readonly pagedAuditoriums = computed(() => {
        const start = this.pageIndex() * this.pageSize;
        return this.auditoriums().slice(start, start + this.pageSize);
    });

    constructor() {
        this.refresh();
    }

    refresh(): void {
        this.service.list().subscribe((items) => {
            this.auditoriums.set(items);
            this.pageIndex.set(0);
        });
    }

    add(): void {
        if (!this.form.name || !this.form.rows || !this.form.cols) return;

        const a: Auditorium = {
            id: uuidv4(),
            name: this.form.name,
            rows: Number(this.form.rows),
            cols: Number(this.form.cols),
        };

        this.service.create(a).subscribe(() => {
            this.form = { name: '', rows: 10, cols: 12 };

            this.refresh();
        });
    }

    remove(id: string): void {
        this.service.delete(id).subscribe(() => this.refresh());
    }

    protected setPage(i: number): void {
        const clamped = Math.max(0, Math.min(i, this.totalPages() - 1));
        this.pageIndex.set(clamped);
    }

    protected prevPage(): void {
        this.setPage(this.pageIndex() - 1);
    }

    protected nextPage(): void {
        this.setPage(this.pageIndex() + 1);
    }
}
