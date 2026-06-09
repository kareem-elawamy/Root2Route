import { Component, inject, OnInit, signal, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { OrgContextService } from '../../../../core/services/org-context.service';

export interface Product {
  id: string;
  name: string;
  sku: string;
  category: string;
  price: number;
  stock: number;
  dateAdded: string;
  status: 'Active' | 'Low Stock' | 'Out of Stock' | 'Draft';
  imageUrl: string;
}

export interface ProductKpi {
  label: string;
  value: string;
  trend: string;
  isUp: boolean;
  icon: string;
  accentClass: string;
}

@Component({
  selector: 'app-products-component',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './products-component.html',
  styleUrl: './products-component.css',
})
export class ProductsComponent implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly orgCtx = inject(OrgContextService);

  // ── KPI Signal ──────────────────────────────────────────────────────────────
  readonly kpis = signal<ProductKpi[]>([
    { label: 'Total Items',      value: '0',   trend: '+0%',       isUp: true,  icon: 'inventory_2',    accentClass: 'text-primary' },
    { label: 'Active Listings',  value: '0',   trend: 'Stable',    isUp: true,  icon: 'store',          accentClass: 'text-secondary' },
    { label: 'Low Stock Alerts', value: '0',   trend: 'Action Req.', isUp: false, icon: 'warning',      accentClass: 'text-tertiary' },
    { label: 'Inventory Value',  value: '$0',  trend: '↑ 0%',      isUp: true,  icon: 'payments',       accentClass: 'text-primary' },
  ]);

  // ── Products Array ──────────────────────────────────────────────────────────
  productsList: Product[] = [];

  // ── Fallback seed data (used when API returns empty) ────────────────────────
  private readonly fallbackProducts: Product[] = [
    {
      id: '1',
      name: 'Premium Bio-Humus',
      sku: 'AG-924-BH',
      category: 'Soil Health',
      price: 24.00,
      stock: 842,
      dateAdded: 'Oct 12, 2023',
      status: 'Active',
      imageUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuBbcMWnOU3x3aUMRb--Fh5av6KswQWVHCxS3U2_SfUAXxJTXrxDJLYyGqyOS1vHQBeyQi_q9EzbwbOjOD0JY8dGSs9ODppeWKdVKPzYINAU81sCWnDMLi2Jvrrobe4GhPIVh9GtOzFp7yhUZn_io6QHziLKmOgyeLpM3OevIM6r17QcFzV9BvZ00XChB6ZQmU5h557hcU-l7lnjsDIioH-PKrzM6mfowVaqTwC4f977XZq8U-iU9gPaqo8Bk6alFhurCFVc1sxqVlQ',
    },
    {
      id: '2',
      name: 'HydraSense Pro v4',
      sku: 'AG-102-HS',
      category: 'Equipment',
      price: 149.99,
      stock: 12,
      dateAdded: 'Nov 04, 2023',
      status: 'Low Stock',
      imageUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuAS3o7iSmjyncTvDv293tRxp37fjHtyR9K2fUIsdtUVs4zOKA7oVG6vr1PosZyJBRmIVidobtclr-n_9044Qx7sFNAGN3jIXA2fNShNWxzrSJpibh9HKLUm7w0MjWuJ00iX6-f6LXCRWvqurhNBMbTeYeuxP-UkbeB_yp3RrXC1ClgA4MxfAQcd8GUJdi-3jh_3Udam188P2BNQ1vHzlCD9gKpvtVOJ2R2KflqIkXIedOt-qGbT5PMNwja2O5_A1qBNXwgdLZFlf_w',
    },
    {
      id: '3',
      name: 'Heritage Seed Mix',
      sku: 'AG-552-SM',
      category: 'Seeds',
      price: 18.50,
      stock: 0,
      dateAdded: 'Oct 28, 2023',
      status: 'Out of Stock',
      imageUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuC3uM9vJJKiUZWcCXSbWvNNK1thmYmte4ghBymnZi0i1okDsZGbI4dpR0GdDXFwdkuUt6M0WkbNFfj07W-U83L9-yHk4QceQx_npqq3FrafmTH7RQXXccq7ur_nKc3r8UnjVruvdTHMIgd0nUoJfuA-NLHnU4d5cU1ZOUwcuKzcvgq6ZW4p-oO2Ui7ZCwz6zKBjjrd2K0AC1sESAsDsgi1dep5U3R_02weL4CjbJ0SyJH0AppEtWTI7Y3TyEpIHlL0B46biA2_aU5A',
    },
    {
      id: '4',
      name: 'AutoVent Controller',
      sku: 'AG-009-AC',
      category: 'Automation',
      price: 299.00,
      stock: 45,
      dateAdded: 'Dec 01, 2023',
      status: 'Draft',
      imageUrl: 'https://lh3.googleusercontent.com/aida-public/AB6AXuDHJDf2SQptWcEqlBPcNqmNSZKxjcJXlmL8IY-knLKgwfZUt5PatSmAWaa501_9G6rKYgroJQDTma2o6nnXxwQOw_blJr-ZCXMn_WKC0GnzJ-_4e-tqJlfIkVJrytLEvLtaixoJF2Z2bYccBbYt7gzhnGE5yo2zLvmOQtw9-SbztJcmTKqW51Bo7fKj6031LVFhvTkdvN7V7f_s-vNusfiN0hpdoEXowUFzDsbuLZEKxi3cC_UznjGTQLP0AAwGI0ZtXMJU5FQ-npI',
    },
  ];

  ngOnInit(): void {
    const orgId = this.orgCtx.getActiveOrgId();
    if (!orgId) {
      console.error('No active organization found');
      this.productsList = this.fallbackProducts;
      this.cdr.detectChanges();
      return;
    }

    this.http.get<any>(`https://root2route.runasp.net/api/v1/product/Organization/${orgId}`).subscribe({
      next: (response: any) => {
        const data = response?.data || response || {};

        let products: Product[] = [];
        if (Array.isArray(data)) {
          products = data;
        } else if (data.products && Array.isArray(data.products)) {
          products = data.products;
        }

        if (products.length > 0) {
          this.productsList = products.map((p: any): Product => ({
            id:        p.id        || crypto.randomUUID(),
            name:      p.name      || 'Unnamed Product',
            sku:       p.sku       || 'N/A',
            category:  p.category  || 'Uncategorized',
            price:     p.price     ?? 0,
            stock:     p.stock     ?? 0,
            dateAdded: p.dateAdded || 'Unknown Date',
            status:    p.status    || 'Draft',
            imageUrl:  p.imageUrl  || '',
          }));
        } else {
          this.productsList = this.fallbackProducts;
        }

        const totalItems     = data.totalItems     ?? this.productsList.length;
        const activeListings = data.activeListings  ?? this.productsList.filter(p => p.status === 'Active').length;
        const lowStock       = data.lowStockAlerts  ?? this.productsList.filter(p => p.status === 'Low Stock').length;
        const inventoryValue = data.inventoryValue  ?? this.productsList.reduce((s, p) => s + p.price * p.stock, 0);

        this.kpis.set([
          { label: 'Total Items',      value: totalItems.toLocaleString(),         trend: data.totalItemsTrend     || '+0%',          isUp: true,  icon: 'inventory_2', accentClass: 'text-primary'   },
          { label: 'Active Listings',  value: activeListings.toLocaleString(),     trend: data.activeListingsTrend || 'Stable',       isUp: true,  icon: 'store',       accentClass: 'text-secondary' },
          { label: 'Low Stock Alerts', value: lowStock.toLocaleString(),           trend: 'Action Req.',                              isUp: false, icon: 'warning',     accentClass: 'text-tertiary'  },
          { label: 'Inventory Value',  value: '$' + inventoryValue.toLocaleString(), trend: data.inventoryTrend   || '↑ 0%',          isUp: true,  icon: 'payments',    accentClass: 'text-primary'   },
        ]);

        this.cdr.detectChanges();
      },
      error: () => {
        this.productsList = this.fallbackProducts;
        const fallbackValue = this.fallbackProducts.reduce((s, p) => s + p.price * p.stock, 0);
        this.kpis.set([
          { label: 'Total Items',      value: this.fallbackProducts.length.toString(), trend: '+0%',       isUp: true,  icon: 'inventory_2', accentClass: 'text-primary'   },
          { label: 'Active Listings',  value: '1',                                    trend: 'Stable',    isUp: true,  icon: 'store',       accentClass: 'text-secondary' },
          { label: 'Low Stock Alerts', value: '1',                                    trend: 'Action Req.', isUp: false, icon: 'warning',    accentClass: 'text-tertiary'  },
          { label: 'Inventory Value',  value: '$' + fallbackValue.toLocaleString(),   trend: '↑ 0%',      isUp: true,  icon: 'payments',    accentClass: 'text-primary'   },
        ]);
        this.cdr.detectChanges();
      },
    });
  }

  // ── Status badge helpers ────────────────────────────────────────────────────
  getStatusClasses(status: string): string {
    const map: Record<string, string> = {
      'Active':       'bg-emerald-50 text-emerald-700',
      'Low Stock':    'bg-amber-50 text-amber-700',
      'Out of Stock': 'bg-rose-50 text-rose-700',
      'Draft':        'bg-surface-container-low text-on-surface-variant',
    };
    return map[status] ?? 'bg-surface-container-low text-on-surface-variant';
  }

  getDotClasses(status: string): string {
    const map: Record<string, string> = {
      'Active':       'bg-emerald-600',
      'Low Stock':    'bg-amber-600',
      'Out of Stock': 'bg-rose-600',
      'Draft':        'bg-outline',
    };
    return map[status] ?? 'bg-outline';
  }
}
