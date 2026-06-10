import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="pagination-container" *ngIf="totalPages > 1">
      <div class="pagination-info">
        Showing page <strong>{{ currentPage }}</strong> of <strong>{{ totalPages }}</strong>
        <span *ngIf="totalCount"> &middot; {{ totalCount }} total</span>
      </div>
      <div class="pagination-controls">
        <button class="page-btn" [disabled]="currentPage <= 1" (click)="goTo(currentPage - 1)" title="Previous">
          <span class="material-symbols-outlined">chevron_left</span>
        </button>

        <ng-container *ngFor="let page of pages">
          <span *ngIf="page === -1" class="page-ellipsis">&hellip;</span>
          <button *ngIf="page !== -1"
            class="page-btn"
            [class.active]="page === currentPage"
            (click)="goTo(page)">
            {{ page }}
          </button>
        </ng-container>

        <button class="page-btn" [disabled]="currentPage >= totalPages" (click)="goTo(currentPage + 1)" title="Next">
          <span class="material-symbols-outlined">chevron_right</span>
        </button>
      </div>
    </div>
  `,
  styles: [`
    .pagination-container {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 16px 24px;
      border-top: 1px solid #ebebf0;
      gap: 16px;
      flex-wrap: wrap;
    }
    .pagination-info {
      font-size: 13px;
      color: #86868b;
      font-weight: 500;
    }
    .pagination-info strong { color: #1d1d1f; }
    .pagination-controls {
      display: flex;
      align-items: center;
      gap: 4px;
    }
    .page-btn {
      min-width: 36px;
      height: 36px;
      border-radius: 8px;
      border: 1px solid #e5e5ea;
      background: #fff;
      color: #1d1d1f;
      font-size: 14px;
      font-weight: 500;
      cursor: pointer;
      display: flex;
      align-items: center;
      justify-content: center;
      transition: all 0.2s ease;
    }
    .page-btn:hover:not(:disabled):not(.active) {
      background: #f1f5f9;
      border-color: #d1d5db;
    }
    .page-btn:disabled {
      opacity: 0.4;
      cursor: not-allowed;
    }
    .page-btn.active {
      background: var(--color-primary, #007aff);
      border-color: var(--color-primary, #007aff);
      color: #fff;
      box-shadow: 0 2px 8px rgba(0, 122, 255, 0.25);
    }
    .page-btn .material-symbols-outlined {
      font-size: 20px;
    }
    .page-ellipsis {
      width: 36px;
      height: 36px;
      display: flex;
      align-items: center;
      justify-content: center;
      color: #86868b;
      font-size: 16px;
    }
  `]
})
export class PaginationComponent {
  @Input() currentPage = 1;
  @Input() totalPages = 1;
  @Input() totalCount = 0;
  @Output() pageChange = new EventEmitter<number>();

  get pages(): number[] {
    const pages: number[] = [];
    const maxVisible = 5;

    if (this.totalPages <= maxVisible + 2) {
      for (let i = 1; i <= this.totalPages; i++) pages.push(i);
      return pages;
    }

    // Always show first page
    pages.push(1);

    const start = Math.max(2, this.currentPage - 1);
    const end = Math.min(this.totalPages - 1, this.currentPage + 1);

    if (start > 2) pages.push(-1); // ellipsis

    for (let i = start; i <= end; i++) pages.push(i);

    if (end < this.totalPages - 1) pages.push(-1); // ellipsis

    // Always show last page
    pages.push(this.totalPages);

    return pages;
  }

  goTo(page: number) {
    if (page < 1 || page > this.totalPages || page === this.currentPage) return;
    this.pageChange.emit(page);
  }
}
