import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuctionService } from './auction.service';
import { SkeletonComponent } from '../../../shared/components/skeleton/skeleton.component';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-auctions',
  standalone: true,
  imports: [CommonModule, SkeletonComponent],
  templateUrl: './auctions.html'
})
export class Auctions implements OnInit {
  private auctionService = inject(AuctionService);
  private toastService = inject(ToastService);


  isLoading = signal(true);

  // 2. قيم مبدئية للإحصائيات عشان الـ HTML ميضربش إيرور
  marketStats: any = {
    velocity: '0%',
    bidders: '0',
    volume: '$0'
  };

  activeLots: any[] = [];

  ngOnInit() {
    this.loadAuctions();
  }

  loadAuctions() {
    this.isLoading.set(true);
    this.auctionService.getActiveAuctions().subscribe({
      next: (response: any) => {
        const actualData = response.data || response;

        // ربط لستة المزادات (بنتأكد إنها مصفوفة)
        const items = actualData.activeLots || (Array.isArray(actualData) ? actualData : []);

        this.activeLots = items.map((lot: any) => {
          // حساب الوقت المتبقي
          const now = new Date();
          const end = new Date(lot.endDate);
          const start = new Date(lot.startDate);
          
          let timeLeftStr = 'N/A';
          let progressPercent = '0%';
          let progressClass = 'text-slate-500 bg-slate-300';
          
          if (end > now) {
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

          return {
            ...lot,
            title: lot.title || lot.productName || 'Unnamed Lot',
            currentBid: lot.currentHighestBid > 0 ? lot.currentHighestBid : lot.startPrice || 0,
            timeLeft: timeLeftStr,
            progressPercent: progressPercent,
            progressClass: progressClass,
            category: lot.productName || 'General',
          };
        });

        // ربط الإحصائيات لو الباك إند بيبعتها
        if (actualData.marketStats) {
          this.marketStats.velocity = actualData.marketStats.velocity || '0%';
          this.marketStats.bidders = actualData.marketStats.bidders || '0';
          this.marketStats.volume = actualData.marketStats.volume || '$0';
        } else {
          // Calculate dummy stats based on active lots
          this.marketStats.bidders = items.length > 0 ? (items.length * 3).toString() : '0';
          const volume = items.reduce((sum: number, lot: any) => sum + (lot.currentHighestBid > 0 ? lot.currentHighestBid : lot.startPrice || 0), 0);
          this.marketStats.volume = `$${volume.toLocaleString()}`;
          this.marketStats.velocity = items.length > 0 ? '+12%' : '0%';
        }

        console.log('Active Auctions:', this.activeLots);

        this.isLoading.set(false);

      },
      error: (error: any) => {
        console.error('Error fetching auctions', error);
        this.isLoading.set(false);
      }
    });
  }

  viewBidHistory(lotId: string) {
    this.toastService.info('Bid history feature is coming soon!');
    console.log(`فتح سجل المزايدات للمحصول رقم: ${lotId}`);
  }

  createNewLot() {
    this.toastService.info('Auction creation form is coming soon!');
    console.log('جاري فتح شاشة إضافة مزاد جديد...');
  }
  
  exportLogs() {
    this.toastService.info('Exporting logs... Please wait.');
  }
  
  verifyBidders() {
    this.toastService.info('Verification module is under maintenance.');
  }
}