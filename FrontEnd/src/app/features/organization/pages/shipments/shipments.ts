import { Component, OnInit, inject, effect, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShipmentsService } from '../../shipments.service';
import { OrderService } from '../../order.service';
import { OrgContextService } from '../../../../core/services/org-context.service';
import { FormsModule } from '@angular/forms';
import { ToastService } from '../../../../core/services/toast.service';
import { ConfirmDialogService } from '../../../../core/services/confirm-dialog.service';
import { AuthService } from '../../../../core/services/auth.service';

import { SkeletonComponent } from '../../../../shared/components/skeleton/skeleton.component';

@Component({
  selector: 'app-shipments',
  standalone: true,
  imports: [CommonModule, FormsModule, SkeletonComponent],
  templateUrl: './shipments.html',
  // styleUrl: './shipments.css'
})
export class ShipmentsComponent implements OnInit {
  private shipmentsService = inject(ShipmentsService);
  private orderService = inject(OrderService);
  private orgCtx = inject(OrgContextService);

  private toast = inject(ToastService);
  private confirmDialog = inject(ConfirmDialogService);
  public readonly auth = inject(AuthService);

  readonly activeOrg = this.orgCtx.activeOrg;
  
  shipments: any[] = [];
  filteredShipments: any[] = [];
  
  activeTab = signal('Pending'); // 'Pending', 'In Transit', 'Delivered'
  isLoading = signal(true);
  
  stats = {
    total: 0,
    pending: 0,
    inTransit: 0,
    delivered: 0
  };
  
  isHelpOpen = false;

  toggleShipmentsHelp() {
    this.isHelpOpen = !this.isHelpOpen;
  }

  closeShipmentsHelp() {
    this.isHelpOpen = false;
  }

  // Dispatch Modal State
  isDispatchModalOpen = false;
  selectedOrderId: string | null = null;
  dispatchForm = {
    carrierName: '',
    trackingNumber: '',
    driverPhone: ''
  };

  canManageShipments(): boolean {
    const roles = this.auth.currentUser()?.roles ?? [];
    return roles.includes('Owner') || roles.includes('OrganizationOwner') || roles.includes('Admin');
  }

  constructor() {
    effect(() => {
      const org = this.activeOrg();
      if (org && org.id) {
        this.loadShipments(org.id);
      }
    });
  }

  ngOnInit() {}

  loadShipments(orgId: string) {
    this.isLoading.set(true);
    this.orderService.getReceivedOrders(orgId).subscribe({
      next: (response: any) => {
        const data = response.data || response.items || response || [];
        const items = Array.isArray(data) ? data : [];

        // For shipments, we care about orders that are Processing, Shipped, or Completed
        this.shipments = items.filter((o: any) => 
          o.status === 'Processing' || o.status === 'Shipped' || o.status === 'Completed'
        );

        this.stats.total = this.shipments.length;
        this.stats.pending = this.shipments.filter(o => o.status === 'Processing').length;
        this.stats.inTransit = this.shipments.filter(o => o.status === 'Shipped').length;
        this.stats.delivered = this.shipments.filter(o => o.status === 'Completed').length;

        this.applyFilter();
        this.isLoading.set(false);
      },
      error: (error: any) => {
        console.error('Error fetching shipments', error);
        this.isLoading.set(false);
      }
    });
  }

  setTab(tab: string) {
    this.activeTab.set(tab);
    this.applyFilter();
  }

  applyFilter() {
    const tab = this.activeTab();
    if (tab === 'Pending') {
      this.filteredShipments = this.shipments.filter(o => o.status === 'Processing');
    } else if (tab === 'In Transit') {
      this.filteredShipments = this.shipments.filter(o => o.status === 'Shipped');
    } else if (tab === 'Delivered') {
      this.filteredShipments = this.shipments.filter(o => o.status === 'Completed');
    }

  }

  openDispatchModal(orderId: string) {
    this.selectedOrderId = orderId;
    this.dispatchForm = { carrierName: '', trackingNumber: '', driverPhone: '' };
    this.isDispatchModalOpen = true;
  }

  closeDispatchModal() {
    this.isDispatchModalOpen = false;
    this.selectedOrderId = null;
  }

  dispatchOrder() {
    if (!this.selectedOrderId || !this.dispatchForm.carrierName || !this.dispatchForm.trackingNumber) return;

    this.shipmentsService.dispatchOrder({
      orderId: this.selectedOrderId,
      carrierName: this.dispatchForm.carrierName,
      trackingNumber: this.dispatchForm.trackingNumber,
      driverPhone: this.dispatchForm.driverPhone
    }).subscribe({
      next: () => {
        this.toast.success('Order dispatched successfully!');
        this.closeDispatchModal();
        const org = this.activeOrg();
        if (org) this.loadShipments(org.id);
      },
      error: (error: any) => {
        console.error('Error dispatching order', error);
        this.toast.error('Failed to dispatch order.');
      }
    });
  }

  async markAsDelivered(shipmentId: string) {
    const confirmed = await this.confirmDialog.open({
      title: 'Mark as Delivered',
      message: 'Are you sure you want to mark this shipment as Delivered?',
      confirmLabel: 'Mark Delivered',
    });
    if (!confirmed) return;

    this.shipmentsService.updateShipmentStatus(shipmentId, 3).subscribe({
      next: () => {
        this.toast.success('Shipment marked as delivered!');
        const org = this.activeOrg();
        if (org) this.loadShipments(org.id);
      },
      error: (error: any) => {
        console.error('Error updating status', error);
        this.toast.error('Failed to update shipment status.');
      }
    });
  }
}
