import { Component, OnInit, inject, ChangeDetectorRef, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuctionService } from '../../../super-admin/auctions/auction.service';
import { OrgContextService } from '../../../../core/services/org-context.service';

@Component({
  selector: 'app-auctions',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './auctions.html'
})
export class AuctionsComponent implements OnInit {
  private auctionService = inject(AuctionService);
  private orgCtx = inject(OrgContextService);
  private cdr = inject(ChangeDetectorRef);

  readonly activeOrg = this.orgCtx.activeOrg;
  auctions: any[] = [];
  
  stats = {
    total: 0,
    active: 0,
    completed: 0,
    cancelled: 0
  };

  constructor() {
    effect(() => {
      const org = this.activeOrg();
      if (org && org.id) {
        this.loadAuctions(org.id);
      }
    });
  }

  ngOnInit() {
  }

  loadAuctions(orgId: string) {
    this.auctionService.getOrganizationAuctions(orgId).subscribe({
      next: (response: any) => {
        const data = response.data || response.items || response || [];
        const items = Array.isArray(data) ? data : [];

        this.auctions = items.map((auction: any) => {
          return {
            ...auction,
            statusClass: this.getStatusClass(auction.status),
            timeLeft: this.calculateTimeLeft(auction.endDate)
          };
        });

        this.stats.total = items.length;
        this.stats.active = items.filter((a: any) => a.status === 'Active').length;
        this.stats.completed = items.filter((a: any) => a.status === 'Completed').length;
        this.stats.cancelled = items.filter((a: any) => a.status === 'Cancelled').length;

        this.cdr.detectChanges();
      },
      error: (error: any) => {
        console.error('Error fetching auctions', error);
      }
    });
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      'Active': 'bg-emerald-100 text-emerald-700',
      'Completed': 'bg-blue-100 text-blue-700',
      'Cancelled': 'bg-rose-100 text-rose-700',
      'Pending': 'bg-amber-100 text-amber-700'
    };
    return map[status] ?? 'bg-slate-100 text-slate-700';
  }

  calculateTimeLeft(endDateStr: string): string {
    if (!endDateStr) return 'N/A';
    const endDate = new Date(endDateStr);
    const now = new Date();
    const diff = endDate.getTime() - now.getTime();

    if (diff <= 0) return 'Ended';

    const days = Math.floor(diff / (1000 * 60 * 60 * 24));
    const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));

    if (days > 0) return `${days}d ${hours}h`;
    if (hours > 0) return `${hours}h ${minutes}m`;
    return `${minutes}m`;
  }
}
