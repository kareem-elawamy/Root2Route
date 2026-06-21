import { Component, OnInit, inject, effect, signal, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrderService } from '../../order.service';
import { OrgContextService } from '../../../../core/services/org-context.service';
import { ToastService } from '../../../../core/services/toast.service';
import { BaseChartDirective } from 'ng2-charts';
import { Chart, ChartConfiguration, ChartData, ChartType, registerables } from 'chart.js';
import { SkeletonComponent } from '../../../../shared/components/skeleton/skeleton.component';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, BaseChartDirective, SkeletonComponent],
  templateUrl: './orders.html',
  styleUrl: './orders.css'
})
export class OrdersComponent implements OnInit {
  private orderService = inject(OrderService);
  private orgCtx = inject(OrgContextService);

  private toast = inject(ToastService);

  readonly activeOrg = this.orgCtx.activeOrg;
  orders: any[] = [];
  allOrders: any[] = [];

  activeFilter = signal('All');
  isFilterDropdownOpen = signal(false);
  isHelpOpen = signal(false);
  isLoading = signal(true);

  // Drawer state
  isDrawerOpen = signal(false);
  selectedOrder = signal<any | null>(null);
  selectedNewStatus = signal<number>(0);
  statusNote = signal<string>('');
  isUpdatingStatus = signal(false);

  stats = {
    total: 0,
    pending: 0,
    completed: 0,
    cancelled: 0
  };

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

  public revenueChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: false }
    },
    scales: {
      y: { beginAtZero: true }
    }
  };
  public revenueChartType: ChartType = 'bar';
  public revenueChartData: ChartData<'bar', number[], string | string[]> = {
    labels: [],
    datasets: [{ data: [], backgroundColor: '#006B3D', borderRadius: 6 }]
  };

  constructor() {
    Chart.register(...registerables);
    effect(() => {
      const org = this.activeOrg();
      if (org && org.id) {
        this.loadOrders(org.id);
      }
    });
  }

  ngOnInit() { }

  @HostListener('document:click')
  onDocumentClick() {
    this.isFilterDropdownOpen.set(false);
  }

  toggleHelp(): void {
    this.isHelpOpen.update(v => !v);
  }

  closeHelp(): void {
    this.isHelpOpen.set(false);
  }

  loadOrders(orgId: string) {
    this.isLoading.set(true);
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
        this.stats.pending = items.filter((o: any) => o.status === 'Pending' || o.status === 'Processing' || o.status === 'Confirmed' || o.status === 'Shipped').length;
        this.stats.completed = items.filter((o: any) => o.status === 'Completed' || o.status === 'Delivered').length;
        this.stats.cancelled = items.filter((o: any) => o.status === 'Cancelled').length;

        this.filterOrders();
        this.isLoading.set(false);
      },
      error: (error: any) => {
        console.error('Error fetching orders', error);
        this.isLoading.set(false);
      }
    });
  }

  getStatusClass(status: string): string {
    const map: Record<string, string> = {
      'Pending': 'status-pending',
      'Confirmed': 'status-processing',
      'Processing': 'status-processing',
      'Shipped': 'status-shipped',
      'Delivered': 'status-completed',
      'Completed': 'status-completed',
      'Cancelled': 'status-cancelled'
    };
    return map[status] ?? 'status-default';
  }

  getStatusNumber(status: string): number {
    const map: Record<string, number> = {
      'Pending': 0,
      'Confirmed': 1,
      'Shipped': 2,
      'Delivered': 3,
      'Cancelled': 4,
      'Processing': 5
    };
    return map[status] ?? 0;
  }

  openDetail(order: any): void {
    this.selectedOrder.set(order);
    this.selectedNewStatus.set(this.getStatusNumber(order.status));
    this.statusNote.set('');
    this.isDrawerOpen.set(true);

    this.orderService.getOrderById(order.id).subscribe({
      next: (response: any) => {
        const fullDetails = response.data || response;
        if (fullDetails) {
          this.selectedOrder.set({
            ...fullDetails,
            statusClass: this.getStatusClass(fullDetails.status)
          });
          this.selectedNewStatus.set(this.getStatusNumber(fullDetails.status));
        }
      },
      error: (err: any) => {
        console.error('Error fetching full order details:', err);
      }
    });
  }

  closeDetail(): void {
    this.isDrawerOpen.set(false);
    this.selectedOrder.set(null);
  }

  updateStatus(): void {
    const order = this.selectedOrder();
    if (!order) return;

    this.isUpdatingStatus.set(true);
    this.orderService.changeStatus(order.id, this.selectedNewStatus(), this.statusNote()).subscribe({
      next: (res) => {
        this.toast.success('Order status updated successfully');
        this.closeDetail();
        const activeOrg = this.activeOrg();
        if (activeOrg && activeOrg.id) {
          this.loadOrders(activeOrg.id);
        }
        this.isUpdatingStatus.set(false);
      },
      error: (err) => {
        console.error('Failed to update status', err);
        this.toast.error('Failed to update order status');
        this.isUpdatingStatus.set(false);
      }
    });
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
    } else if (filter === 'Completed') {
      this.orders = this.allOrders.filter(o => o.status === 'Completed' || o.status === 'Delivered');
    } else {
      this.orders = this.allOrders.filter(o => o.status === filter);
    }
    this.updateCharts();

  }

  exportOrders(): void {
    if (this.orders.length === 0) {
      this.toast.warning("No orders to export.");
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

  updateCharts(): void {
    const statusCounts: Record<string, number> = {};
    const dateRevenue: Record<string, number> = {};

    this.orders.forEach(o => {
      // Status count
      statusCounts[o.status] = (statusCounts[o.status] || 0) + 1;

      // Date revenue
      const dateObj = new Date(o.orderDate);
      const dateKey = !isNaN(dateObj.getTime()) ? dateObj.toLocaleDateString() : 'Unknown';
      const amount = o.finalTotal || o.totalAmount || 0;
      dateRevenue[dateKey] = (dateRevenue[dateKey] || 0) + amount;
    });

    // Setup Status Doughnut
    const statusLabels = Object.keys(statusCounts);
    const statusColors = statusLabels.map(status => {
      switch (status) {
        case 'Completed': return '#34c759';
        case 'Pending': return '#ffcc00';
        case 'Processing': return '#007aff';
        case 'Shipped': return '#af52de';
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

    // Setup Revenue Bar
    const dateLabels = Object.keys(dateRevenue).sort((a, b) => {
      if (a === 'Unknown') return -1;
      if (b === 'Unknown') return 1;
      return new Date(a).getTime() - new Date(b).getTime();
    });

    this.revenueChartData = {
      labels: dateLabels,
      datasets: [
        {
          label: 'Revenue ($)',
          data: dateLabels.map(d => dateRevenue[d]),
          backgroundColor: '#006B3D',
          borderRadius: 6
        }
      ]
    };
  }
}
