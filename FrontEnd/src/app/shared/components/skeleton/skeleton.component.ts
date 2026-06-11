import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-skeleton',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="skeleton-wrapper" [ngClass]="type">
      @if (type === 'table') {
        <div class="skeleton-table">
          <!-- Header -->
          <div class="skeleton-row header">
            @for (col of columnArray; track $index) {
              <div class="skeleton-cell"></div>
            }
          </div>
          <!-- Body -->
          @for (row of rowArray; track $index) {
            <div class="skeleton-row">
              @for (col of columnArray; track $index) {
                <div class="skeleton-cell"></div>
              }
            </div>
          }
        </div>
      } @else if (type === 'card') {
        <div class="skeleton-card">
          <div class="skeleton-avatar"></div>
          <div class="skeleton-lines">
            <div class="skeleton-line title"></div>
            <div class="skeleton-line"></div>
            <div class="skeleton-line short"></div>
          </div>
        </div>
      } @else if (type === 'list') {
        <div class="skeleton-list">
          @for (row of rowArray; track $index) {
            <div class="skeleton-list-item">
              <div class="skeleton-avatar small"></div>
              <div class="skeleton-lines">
                <div class="skeleton-line"></div>
              </div>
            </div>
          }
        </div>
      }
    </div>
  `,
  styleUrl: './skeleton.component.css'
})
export class SkeletonComponent {
  @Input() type: 'table' | 'card' | 'list' = 'table';
  @Input() rows: number = 5;
  @Input() columns: number = 4;

  get rowArray(): number[] {
    return Array(this.rows).fill(0);
  }

  get columnArray(): number[] {
    return Array(this.columns).fill(0);
  }
}
