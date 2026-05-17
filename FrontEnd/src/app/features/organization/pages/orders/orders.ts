import { Component, OnInit, inject, ChangeDetectorRef, effect } from '@angular/core';
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

  loadOrders(orgId: string) {
    this.orderService.getReceivedOrders(orgId).subscribe({
      next: (response: any) => {
        const data = response.data || response.items || response || [];
        const items = Array.isArray(data) ? data : [];

        this.orders = items.map((order: any) => {
          return {
            ...order,
            statusClass: this.getStatusClass(order.status)
          };
        });

        this.stats.total = items.length;
        this.stats.pending = items.filter((o: any) => o.status === 'Pending' || o.status === 'Processing').length;
        this.stats.completed = items.filter((o: any) => o.status === 'Completed' || o.status === 'Shipped').length;
        this.stats.cancelled = items.filter((o: any) => o.status === 'Cancelled').length;

        this.cdr.detectChanges();
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
}
