import { Component, OnInit, inject, ChangeDetectorRef, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReviewsService } from '../../reviews.service';
import { OrgContextService } from '../../../../core/services/org-context.service';

@Component({
  selector: 'app-reviews',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reviews.html',
  // styleUrl: './reviews.css'
})
export class ReviewsComponent implements OnInit {
  private reviewsService = inject(ReviewsService);
  private orgCtx = inject(OrgContextService);
  private cdr = inject(ChangeDetectorRef);

  readonly activeOrg = this.orgCtx.activeOrg;
  reviews: any[] = [];
  
  stats = {
    total: 0,
    average: 0
  };
  
  isHelpOpen = false;

  toggleReviewsHelp() {
    this.isHelpOpen = !this.isHelpOpen;
  }

  closeReviewsHelp() {
    this.isHelpOpen = false;
  }

  constructor() {
    effect(() => {
      const org = this.activeOrg();
      if (org && org.id) {
        this.loadReviews(org.id);
      }
    });
  }

  ngOnInit() {
  }

  loadReviews(orgId: string) {
    this.reviewsService.getOrganizationReviews(orgId).subscribe({
      next: (response: any) => {
        const data = response.data || response || [];
        const items = Array.isArray(data) ? data.map(r => ({
          ...r,
          userName: r.reviewerName
        })) : [];

        this.reviews = items;
        this.stats.total = items.length;
        this.stats.average = this.calculateAverage(items);

        this.cdr.detectChanges();
      },
      error: (error: any) => {
        console.error('Error fetching reviews', error);
      }
    });
  }

  calculateAverage(reviews: any[]): number {
    if (reviews.length === 0) return 0;
    const sum = reviews.reduce((acc, r) => acc + (r.rating || 0), 0);
    return Math.round((sum / reviews.length) * 10) / 10;
  }

  getStars(rating: number): number[] {
    return Array(Math.floor(rating)).fill(0);
  }

  getEmptyStars(rating: number): number[] {
    return Array(5 - Math.floor(rating)).fill(0);
  }
}
