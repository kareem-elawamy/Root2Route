import { Component, OnInit, inject, ChangeDetectorRef, effect, signal, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderService } from '../../order.service';
import { OrgContextService } from '../../../../core/services/org-context.service';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './orders.html'
})
export class OrdersComponent implements OnInit {
  private orderService = inject(OrderService);
  private orgCtx = inject(OrgContextService);
  private cdr = inject(ChangeDetectorRef);

  readonly activeOrg = this.orgCtx.activeOrg;
  orders: any[] = [];
  allOrders: any[] = [];
  
  activeFilter = signal('All');
  isFilterDropdownOpen = signal(false);
  
  stats = {
    total: 0,
    pending: 0,
    completed: 0,
    cancelled: 0
  };

  constructor() {
    // Watch for active organization changes
    effect(() => {
      const org = this.activeOrg();
      if (org && org.id) {
        this.loadOrders(org.id);
      }
    });
  }

  ngOnInit() {
    // effect will handle the loading
  }

  @HostListener('document:click')
  onDocumentClick() {
    this.isFilterDropdownOpen.set(false);
  }

  loadOrders(orgId: string) {
    this.orderService.getReceivedOrders(orgId).subscribe({
      next: (response: any) => {
        const data = response.data || response.items || response || [];
        const items = Array.isArray(data) ? data : [];

        const mappedItems = items.map((order: any) => {
          return {
            ...order,
            statusClass: this.getStatusClass(order.status)
          };
        });

        this.allOrders = mappedItems;

        this.stats.total = items.length;
        this.stats.pending = items.filter((o: any) => o.status === 'Pending' || o.status === 'Processing').length;
        this.stats.completed = items.filter((o: any) => o.status === 'Completed' || o.status === 'Shipped').length;
        this.stats.cancelled = items.filter((o: any) => o.status === 'Cancelled').length;

        this.filterOrders();
      },
      error: (error: any) => {
        console.error('Error fetching orders', error);
      }
    });
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      'Pending': 'bg-amber-100 text-amber-700',
      'Processing': 'bg-blue-100 text-blue-700',
      'Shipped': 'bg-indigo-100 text-indigo-700',
      'Completed': 'bg-emerald-100 text-emerald-700',
      'Cancelled': 'bg-rose-100 text-rose-700'
    };
    return map[status] ?? 'bg-slate-100 text-slate-700';
  }

  toggleFilterDropdown(event: Event): void {
    event.stopPropagation();
    this.isFilterDropdownOpen.set(!this.isFilterDropdownOpen());
  }

  setFilter(filter: string): void {
    this.activeFilter.set(filter);
    this.isFilterDropdownOpen.set(false);
    this.filterOrders();
  }

  filterOrders(): void {
    const filter = this.activeFilter();
    if (filter === 'All') {
      this.orders = [...this.allOrders];
    } else {
      this.orders = this.allOrders.filter(o => o.status === filter);
    }
    this.cdr.detectChanges();
  }

  exportOrders(): void {
    if (this.orders.length === 0) {
      alert("No orders to export.");
      return;
    }

    const headers = ['Order ID', 'Date', 'Receiver', 'Total', 'Status'];
    const rows = this.orders.map(order => [
      order.id,
      order.orderDate,
      order.receiverName || 'N/A',
      order.finalTotal || order.totalAmount,
      order.status
    ]);

    const csvContent = [
      headers.join(','),
      ...rows.map(e => e.join(','))
    ].join('\n');

    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.setAttribute('href', url);
    link.setAttribute('download', 'orders_export.csv');
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }
}
