import { Component, OnInit, inject, effect, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReviewsService } from '../../reviews.service';
import { OrgContextService } from '../../../../core/services/org-context.service';
import { SkeletonComponent } from '../../../../shared/components/skeleton/skeleton.component';

@Component({
  selector: 'app-reviews',
  standalone: true,
  imports: [CommonModule, SkeletonComponent],
  templateUrl: './reviews.html',
  styleUrl: './reviews.css'
})
export class ReviewsComponent implements OnInit {
  private reviewsService = inject(ReviewsService);
  private orgCtx = inject(OrgContextService);


  readonly activeOrg = this.orgCtx.activeOrg;
  reviews = signal<any[]>([]);
  
  stats = signal({
    total: 0,
    average: 0
  });
  
  isHelpOpen = signal(false);
  isLoading = signal(true);

  toggleReviewsHelp() {
    this.isHelpOpen.update(v => !v);
  }

  closeReviewsHelp() {
    this.isHelpOpen.set(false);
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
    this.isLoading.set(true);
    this.reviewsService.getOrganizationReviews(orgId).subscribe({
      next: (response: any) => {
        const data = response.data || response || [];
        const items = Array.isArray(data) ? data.map(r => ({
          ...r,
          userName: r.reviewerName
        })) : [];

        this.reviews.set(items);
        this.stats.set({
          total: items.length,
          average: this.calculateAverage(items)
        });

        this.isLoading.set(false);
      },
      error: (error: any) => {
        console.error('Error fetching reviews', error);
        this.isLoading.set(false);
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
