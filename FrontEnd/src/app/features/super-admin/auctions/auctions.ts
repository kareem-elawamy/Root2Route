import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuctionService } from './auction.service'; 

@Component({
  selector: 'app-auctions',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './auctions.html'
})
export class Auctions implements OnInit {
  private auctionService = inject(AuctionService);

  marketStats = {
    velocity: '+12.4%',
    bidders: '1,248',
    volume: '$842k'
  };

  activeLots: any[] = [];

  ngOnInit() {
    this.loadAuctions();
  }

  loadAuctions() {
    // رجعنا نكلم السيرفر الحقيقي تاني
    this.auctionService.getActiveAuctions().subscribe({
      next: (response: any) => {
        this.activeLots = response.data || response || []; 
        
        // هيطبع الداتا الحقيقية في الكونسول
        console.log('Active Auctions:', this.activeLots);
      },
      error: (error: any) => {
        console.error('Error fetching auctions', error);
      }
    });
  }

  viewBidHistory(lotId: string) {
    console.log(`فتح سجل المزايدات للمحصول رقم: ${lotId}`);
  }

  createNewLot() {
    console.log('جاري فتح شاشة إضافة مزاد جديد...');
  }
}