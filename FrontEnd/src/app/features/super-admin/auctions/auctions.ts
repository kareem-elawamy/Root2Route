import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuctionService } from './auction.service';
import { SkeletonComponent } from '../../../shared/components/skeleton/skeleton.component';
import { ToastService } from '../../../core/services/toast.service';
import { ConfirmDialogService } from '../../../core/services/confirm-dialog.service';

@Component({
  selector: 'app-auctions',
  standalone: true,
  imports: [CommonModule, SkeletonComponent],
  templateUrl: './auctions.html'
})
export class Auctions implements OnInit {
  private auctionService = inject(AuctionService);
  private toastService = inject(ToastService);
  private confirmDialog = inject(ConfirmDialogService);

  activeTab = signal<'all' | 'active' | 'completed'>('all');


  isLoading = signal(true);

  // 2. قيم مبدئية للإحصائيات عشان الـ HTML ميضربش إيرور
  marketStats: any = {
    velocity: '0%',
    bidders: '0',
    volume: '$0'
  };

  allLots = signal<any[]>([]);
  activeLots: any[] = [];
  completedLots = signal<any[]>([]);
  isLoadingCompleted = signal(false);

  ngOnInit() {
    this.loadAuctions();
  }

  loadAuctions() {
    this.isLoading.set(true);
    // Use getAllAuctions to get ALL statuses (Upcoming, Ongoing, Completed, Cancelled)
    this.auctionService.getAllAuctions().subscribe({
      next: (response: any) => {
        const actualData = response.data || response;

        // ربط لستة المزادات (بنتأكد إنها مصفوفة)
        const items = actualData.activeLots || (Array.isArray(actualData) ? actualData : []);

        const processed = items.map((lot: any) => {
          // حساب الوقت المتبقي
          const now = new Date();
          const end = new Date(lot.endDate);
          const start = new Date(lot.startDate);
          
          let timeLeftStr = 'N/A';
          let progressPercent = '0%';
          let progressClass = 'text-slate-500 bg-slate-300';
          
          if (end > now && lot.status !== 'Cancelled') {
            const diffMs = end.getTime() - now.getTime();
            const diffHrs = Math.floor(diffMs / (1000 * 60 * 60));
            const diffDays = Math.floor(diffHrs / 24);
            const diffMins = Math.floor(diffMs / (1000 * 60));
            
            if (diffDays > 0) {
              timeLeftStr = `${diffDays} days left`;
            } else if (diffHrs > 0) {
              timeLeftStr = `${diffHrs} hours left`;
            } else {
              timeLeftStr = `${diffMins} mins left`;
            }

            const totalMs = end.getTime() - start.getTime();
            const elapsedMs = Math.max(0, now.getTime() - start.getTime());
            const percent = totalMs > 0 ? Math.min(100, (elapsedMs / totalMs) * 100) : 0;
            progressPercent = `${percent.toFixed(0)}%`;
            
            if (percent > 80) {
              progressClass = 'text-rose-500 bg-rose-500';
            } else if (percent > 50) {
              progressClass = 'text-amber-500 bg-amber-500';
            } else {
              progressClass = 'text-emerald-500 bg-emerald-500';
            }
          } else if (lot.endDate) {
            timeLeftStr = 'Ended';
            progressPercent = '100%';
            progressClass = 'text-slate-500 bg-slate-500';
          }

          // Map auction status — backend sends as STRING: "Upcoming", "Ongoing", "Completed", "Cancelled"
          const statusStr = (lot.status || 'Upcoming').toString();
          const statusMap: Record<string, {label: string, class: string}> = {
            'Upcoming': { label: 'Upcoming', class: 'bg-blue-100 text-blue-700' },
            'Ongoing': { label: 'Ongoing', class: 'bg-emerald-100 text-emerald-700' },
            'Completed': { label: 'Completed', class: 'bg-slate-100 text-slate-600' },
            'Cancelled': { label: 'Cancelled', class: 'bg-rose-100 text-rose-700' }
          };
          const statusInfo = statusMap[statusStr] || { label: statusStr, class: 'bg-slate-100 text-slate-600' };

          // Resolve image URL
          let resolvedImage = lot.image || lot.imageUrl || lot.picture || '';
          if (resolvedImage && !resolvedImage.startsWith('http')) {
            resolvedImage = resolvedImage.startsWith('/')
              ? `https://root2route.runasp.net${resolvedImage}`
              : `https://root2route.runasp.net/${resolvedImage}`;
          }

          return {
            ...lot,
            image: resolvedImage,
            title: lot.title || lot.productName || 'Unnamed Lot',
            currentBid: lot.currentHighestBid > 0 ? lot.currentHighestBid : lot.startPrice || 0,
            timeLeft: timeLeftStr,
            progressPercent: progressPercent,
            progressClass: progressClass,
            category: lot.productName || 'General',
            statusStr,
            statusLabel: statusInfo.label,
            statusClass: statusInfo.class,
          };
        });

        this.allLots.set(processed);
        this.activeLots = processed.filter((l: any) => l.statusStr === 'Ongoing');
        this.completedLots.set(processed.filter((l: any) => l.statusStr === 'Completed'));

        // ربط الإحصائيات لو الباك إند بيبعتها
        if (actualData.marketStats) {
          this.marketStats.velocity = actualData.marketStats.velocity || '0%';
          this.marketStats.bidders = actualData.marketStats.bidders || '0';
          this.marketStats.volume = actualData.marketStats.volume || '$0';
        } else {
          // Calculate stats based on all lots
          this.marketStats.bidders = processed.length > 0 ? (processed.length * 3).toString() : '0';
          const volume = processed.reduce((sum: number, lot: any) => sum + (lot.currentHighestBid > 0 ? lot.currentHighestBid : lot.startPrice || 0), 0);
          this.marketStats.volume = `$${volume.toLocaleString()}`;
          this.marketStats.velocity = this.activeLots.length > 0 ? '+12%' : '0%';
        }

        this.isLoading.set(false);

      },
      error: (error: any) => {
        console.error('Error fetching auctions', error);
        this.isLoading.set(false);
      }
    });
  }

  get displayedLots(): any[] {
    const tab = this.activeTab();
    if (tab === 'active') return this.activeLots;
    if (tab === 'completed') return this.completedLots();
    return this.allLots(); // 'all'
  }

  viewBidHistory(lotId: string) {
    this.toastService.info('Bid history feature is coming soon!');
    console.log(`Opening bid history for lot: ${lotId}`);
  }

  createNewLot() {
    this.toastService.info('Auction creation form is coming soon!');
  }

  switchTab(tab: 'all' | 'active' | 'completed') {
    this.activeTab.set(tab);
  }
  
  exportLogs() {
    const data = this.activeLots;
    if (!data.length) {
      this.toastService.error('No auctions to export.');
      return;
    }

    const headers = ['ID', 'Title', 'Current Bid', 'Start Price', 'Time Left', 'Category'];
    const rows = data.map((lot: any) => [
      lot.id || '',
      `"${(lot.title || '').replace(/"/g, '""')}"`,
      lot.currentBid || 0,
      lot.startPrice || 0,
      lot.timeLeft || '',
      `"${(lot.category || '').replace(/"/g, '""')}"`
    ]);

    const csv = [headers.join(','), ...rows.map(r => r.join(','))].join('\n');
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = `auctions_${new Date().toISOString().slice(0, 10)}.csv`;
    link.click();
    URL.revokeObjectURL(link.href);
    this.toastService.success('Auction data exported successfully.');
  }
  
  async cancelAuction(lot: any) {
    const confirmed = await this.confirmDialog.open({
      title: 'Cancel Auction',
      message: `Are you sure you want to cancel the auction for "${lot.title}"? This action cannot be undone.`,
      confirmLabel: 'Cancel Auction',
      isDestructive: true
    });
    if (!confirmed) return;

    this.auctionService.cancelAuction(lot.id).subscribe({
      next: () => {
        this.toastService.success('Auction cancelled successfully.');
        this.loadAuctions();
      },
      error: (err: any) => {
        console.error(err);
        const msg = err?.error?.message || err?.error?.Message || 'Error cancelling auction.';
        this.toastService.error(msg);
      }
    });
  }
}