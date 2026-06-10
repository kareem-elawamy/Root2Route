import { Component, OnInit, inject, signal, HostListener } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuctionService } from '../../../super-admin/auctions/auction.service';
import { ProductService } from '../../../super-admin/products/product.service';
import { OrgContextService } from '../../../../core/services/org-context.service';
import { BaseChartDirective } from 'ng2-charts';
import { ToastService } from '../../../../core/services/toast.service';
import { ConfirmDialogService } from '../../../../core/services/confirm-dialog.service';
import { AuthService } from '../../../../core/services/auth.service';
import { Chart, ChartConfiguration, ChartData, ChartType, registerables } from 'chart.js';

export interface AuctionKpi {
  label: string;
  value: string;
  trend: string;
  isUp: boolean;
  icon: string;
  accentClass: string;
}

@Component({
  selector: 'app-auctions',
  standalone: true,
  imports: [CommonModule, FormsModule, BaseChartDirective],
  templateUrl: './auctions.html',
  styleUrl: './auctions.css'
})
export class AuctionsComponent implements OnInit {
  private auctionService = inject(AuctionService);
  private productService = inject(ProductService);
  private orgCtx = inject(OrgContextService);

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private toast = inject(ToastService);
  private confirmDialog = inject(ConfirmDialogService);
  public readonly auth = inject(AuthService);

  readonly kpis = signal<AuctionKpi[]>([
    { label: 'Total Auctions', value: '0', trend: '+0%', isUp: true, icon: 'gavel', accentClass: 'text-primary' },
    { label: 'Active Bidding', value: '0', trend: 'Stable', isUp: true, icon: 'trending_up', accentClass: 'text-secondary' },
    { label: 'Completed', value: '0', trend: 'Great', isUp: true, icon: 'check_circle', accentClass: 'text-tertiary' },
    { label: 'Cancelled', value: '0', trend: 'Action Req.', isUp: false, icon: 'cancel', accentClass: 'text-primary' },
  ]);

  auctionsList: any[] = [];
  allAuctions: any[] = [];
  orgProducts: any[] = [];

  searchTerm = signal('');
  activeFilter = signal('All');
  isFilterDropdownOpen = signal(false);
  isCreateModalOpen = signal(false);
  isCreatingAuction = signal(false);
  activeDropdownId = signal<string | null>(null);
  isHelpOpen = signal(false);

  // Chart properties
  public statusChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { position: 'right' }
    }
  };
  public statusChartType: ChartType = 'doughnut';
  public statusChartData: ChartData<'doughnut', number[], string | string[]> = {
    labels: [],
    datasets: [{ data: [] }]
  };

  constructor() {
    Chart.register(...registerables);
  }

  ngOnInit() {
    this.loadAuctions();

    this.route.queryParams.subscribe(params => {
      if (params['create'] === 'true') {
        this.router.navigate([], {
          relativeTo: this.route,
          queryParams: { create: null },
          queryParamsHandling: 'merge'
        });
        
        // Wait briefly for UI to initialize
        setTimeout(() => this.openCreateModal(), 100);
      }
    });
  }

  @HostListener('document:click')
  onDocumentClick() {
    this.activeDropdownId.set(null);
    this.isFilterDropdownOpen.set(false);
  }

  toggleAuctionsHelp(): void {
    this.isHelpOpen.update(v => !v);
  }

  closeAuctionsHelp(): void {
    this.isHelpOpen.set(false);
  }

  loadAuctions() {
    const orgId = this.orgCtx.getActiveOrgId();
    if (!orgId) return;

    this.auctionService.getOrganizationAuctions(orgId).subscribe({
      next: (response: any) => {
        const data = response.data || response.items || response || [];
        const items = Array.isArray(data) ? data : [];

        this.allAuctions = items.map((auction: any) => ({
          ...auction,
          timeLeft: this.calculateTimeLeft(auction.endDate),
          status: auction.status || 'Pending'
        }));

        this.filterAuctions();
      },
      error: (error: any) => {
        console.error('Error fetching auctions', error);
        this.allAuctions = [];
        this.filterAuctions();
      }
    });
  }

  filterAuctions(): void {
    const term = this.searchTerm().toLowerCase();
    const filter = this.activeFilter();

    let filtered = [...this.allAuctions];

    if (term) {
      filtered = filtered.filter(a => 
        (a.title || '').toLowerCase().includes(term) || 
        (a.productName || '').toLowerCase().includes(term)
      );
    }

    if (filter !== 'All') {
      filtered = filtered.filter(a => a.status === filter);
    }

    this.auctionsList = filtered;
    this.updateKpis();
    this.updateCharts();

  }

  onSearchChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchTerm.set(input.value);
    this.filterAuctions();
  }

  toggleFilterDropdown(event: Event): void {
    event.stopPropagation();
    this.isFilterDropdownOpen.set(!this.isFilterDropdownOpen());
  }

  setFilter(filter: string): void {
    this.activeFilter.set(filter);
    this.isFilterDropdownOpen.set(false);
    this.filterAuctions();
  }

  updateKpis(): void {
    const total = this.auctionsList.length;
    const active = this.auctionsList.filter(a => a.status === 'Active').length;
    const completed = this.auctionsList.filter(a => a.status === 'Completed').length;
    const cancelled = this.auctionsList.filter(a => a.status === 'Cancelled').length;

    this.kpis.set([
      { label: 'Total Auctions', value: total.toLocaleString(), trend: '+0%', isUp: true, icon: 'gavel', accentClass: 'text-primary' },
      { label: 'Active Bidding', value: active.toLocaleString(), trend: 'Stable', isUp: true, icon: 'trending_up', accentClass: 'text-secondary' },
      { label: 'Completed', value: completed.toLocaleString(), trend: completed > 0 ? 'Great' : '—', isUp: true, icon: 'check_circle', accentClass: 'text-tertiary' },
      { label: 'Cancelled', value: cancelled.toLocaleString(), trend: cancelled > 0 ? 'Action Req.' : 'Good', isUp: cancelled === 0, icon: 'cancel', accentClass: 'text-error' },
    ]);
  }

  updateCharts(): void {
    const statusCounts: Record<string, number> = {};

    this.auctionsList.forEach(a => {
      const status = a.status || 'Pending';
      statusCounts[status] = (statusCounts[status] || 0) + 1;
    });

    const statusLabels = Object.keys(statusCounts);
    const statusColors = statusLabels.map(status => {
      switch (status) {
        case 'Active': return '#34c759';
        case 'Completed': return '#007aff';
        case 'Pending': return '#ffcc00';
        case 'Cancelled': return '#ff3b30';
        default: return '#8e8e93';
      }
    });

    this.statusChartData = {
      labels: statusLabels,
      datasets: [
        {
          data: statusLabels.map(l => statusCounts[l]),
          backgroundColor: statusColors,
          borderWidth: 0
        }
      ]
    };
  }

  // ── Actions ──
  toggleDropdown(id: string, event: Event): void {
    event.stopPropagation();
    if (this.activeDropdownId() === id) {
      this.activeDropdownId.set(null);
    } else {
      this.activeDropdownId.set(id);
    }
  }

  cancelAuction(id: string): void {
    this.confirmDialog.open({
      title: 'Cancel Auction',
      message: 'Are you sure you want to cancel this auction?',
      confirmLabel: 'Cancel Auction',
      isDestructive: true
    }).subscribe(confirmed => {
      if (!confirmed) return;
      
      this.auctionService.cancelAuction(id).subscribe({
        next: () => {
          this.toast.success('Auction cancelled successfully.');
          this.loadAuctions();
        },
        error: (err) => { 
          console.error(err); 
          this.toast.error('Error cancelling auction'); 
        }
      });
    });
  }

  // ── Modal Logic ──
  openCreateModal(): void {
    const orgId = this.orgCtx.getActiveOrgId();
    if (!orgId) return;

    // Load available products for this organization
    this.productService.getProductsByOrg(orgId).subscribe({
      next: (res: any) => {
        let products = res.data || res.items || res || [];
        if (products.length === 0) {
          this.toast.warning('You must add a product first before creating an auction.');
          return;
        }
        this.orgProducts = products;
        this.isCreateModalOpen.set(true);

      },
      error: (err) => {
        console.error('Error fetching products', err);
        this.toast.error('Could not fetch products to create an auction.');
      }
    });
  }

  closeCreateModal(): void {
    this.isCreateModalOpen.set(false);
  }

  submitAuction(event: Event): void {
    event.preventDefault();
    const form = event.target as HTMLFormElement;
    
    const payload = {
      title: (form.elements.namedItem('aucTitle') as HTMLInputElement).value,
      productId: (form.elements.namedItem('aucProduct') as HTMLSelectElement).value,
      startDate: (form.elements.namedItem('aucStartDate') as HTMLInputElement).value,
      endDate: (form.elements.namedItem('aucEndDate') as HTMLInputElement).value,
      startPrice: parseFloat((form.elements.namedItem('aucStartPrice') as HTMLInputElement).value),
      minimumBidIncrement: parseFloat((form.elements.namedItem('aucMinBid') as HTMLInputElement).value),
      reservePrice: parseFloat((form.elements.namedItem('aucReserve') as HTMLInputElement).value) || null
    };

    this.isCreatingAuction.set(true);
    this.auctionService.createAuction(payload).subscribe({
      next: () => {
        this.isCreatingAuction.set(false);
        this.toast.success('Auction created successfully!');
        this.closeCreateModal();
        this.loadAuctions();
      },
      error: (err) => {
        this.isCreatingAuction.set(false);
        console.error(err);
        this.toast.error('Error creating auction.');
      }
    });
  }

  // ── Helpers ──

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
