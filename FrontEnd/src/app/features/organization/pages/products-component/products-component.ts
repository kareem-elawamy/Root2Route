import { Component, inject, OnInit, signal, ChangeDetectorRef, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OrgContextService } from '../../../../core/services/org-context.service';
import { ProductService } from '../../../super-admin/products/product.service';

export interface Product {
  id: string;
  name: string;
  sku: string;
  category: string;
  price: number;
  stock: number;
  dateAdded: string;
  status: string;
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
  imports: [CommonModule, FormsModule],
  templateUrl: './products-component.html',
  styleUrl: './products-component.css',
})
export class ProductsComponent implements OnInit {
  private readonly productService = inject(ProductService);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly orgCtx = inject(OrgContextService);

  readonly kpis = signal<ProductKpi[]>([
    { label: 'Total Items',      value: '0',   trend: '+0%',       isUp: true,  icon: 'inventory_2',    accentClass: 'text-primary' },
    { label: 'Active Listings',  value: '0',   trend: 'Stable',    isUp: true,  icon: 'store',          accentClass: 'text-secondary' },
    { label: 'Low Stock Alerts', value: '0',   trend: 'Action Req.', isUp: false, icon: 'warning',      accentClass: 'text-tertiary' },
    { label: 'Inventory Value',  value: '$0',  trend: '↑ 0%',      isUp: true,  icon: 'payments',       accentClass: 'text-primary' },
  ]);

  productsList: Product[] = [];
  allProducts: Product[] = [];

  searchTerm = signal('');
  showPending = signal(false);
  activeFilter = signal('All');
  isFilterDropdownOpen = signal(false);
  isCreatingProduct = signal(false);
  isCreateModalOpen = signal(false);
  activeDropdownId = signal<string | null>(null);

  ngOnInit(): void {
    this.loadProducts();
  }

  @HostListener('document:click')
  onDocumentClick() {
    this.activeDropdownId.set(null);
    this.isFilterDropdownOpen.set(false);
  }

  loadProducts(): void {
    const orgId = this.orgCtx.getActiveOrgId();
    if (!orgId) return;

    const request$ = this.showPending() 
      ? this.productService.getPendingProductsByOrg(orgId)
      : this.productService.getProductsByOrg(orgId);

    request$.subscribe({
      next: (response: any) => {
        const data = response?.data || response || {};
        let products: any[] = [];
        
        if (Array.isArray(data)) {
          products = data;
        } else if (data.products && Array.isArray(data.products)) {
          products = data.products;
        } else if (data.items && Array.isArray(data.items)) {
          products = data.items;
        }

        this.allProducts = products.map((p: any): Product => ({
          id:        p.id,
          name:      p.name      || 'Unnamed Product',
          sku:       p.sku || p.barcode || 'N/A',
          category:  this.mapProductType(p.productType),
          price:     p.directSalePrice ?? p.price ?? 0,
          stock:     p.stockQuantity ?? p.stock ?? 0,
          dateAdded: p.dateAdded || p.createdOn || new Date().toLocaleDateString(),
          status:    this.mapStatus(p.status),
          imageUrl:  p.imageUrl  || (p.images && p.images.length > 0 ? p.images[0] : ''),
        }));
        
        this.filterProducts();
      },
      error: (err) => {
        console.error('Error loading products', err);
        this.allProducts = [];
        this.filterProducts();
      }
    });
  }

  mapProductType(type: any): string {
    if (type === 0 || type === 'RawCrop') return 'Raw Crop';
    if (type === 1 || type === 'Processed') return 'Processed';
    if (type === 2 || type === 'Tool') return 'Tool';
    if (type === 3 || type === 'Chemical') return 'Chemical';
    return 'Uncategorized';
  }

  mapStatus(status: any): string {
    if (status === 1 || status === 'Active') return 'Active';
    if (status === 0 || status === 'Draft') return 'Draft';
    if (status === 2 || status === 'OutOfStock') return 'Out of Stock';
    return 'Active';
  }

  filterProducts(): void {
    const term = this.searchTerm().toLowerCase();
    const filter = this.activeFilter();

    let filtered = [...this.allProducts];

    // Text search
    if (term) {
      filtered = filtered.filter(p => 
        p.name.toLowerCase().includes(term) || 
        p.sku.toLowerCase().includes(term) ||
        p.category.toLowerCase().includes(term)
      );
    }

    // Category / Status Filter
    if (filter !== 'All') {
      filtered = filtered.filter(p => p.status === filter || p.category === filter);
    }

    this.productsList = filtered;
    this.updateKpis();
    this.cdr.detectChanges();
  }

  toggleFilterDropdown(event: Event): void {
    event.stopPropagation();
    this.isFilterDropdownOpen.set(!this.isFilterDropdownOpen());
  }

  setFilter(filter: string): void {
    this.activeFilter.set(filter);
    this.isFilterDropdownOpen.set(false);
    this.filterProducts();
  }

  onSearchChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.searchTerm.set(input.value);
    this.filterProducts();
  }

  togglePending(): void {
    this.showPending.set(!this.showPending());
    this.loadProducts();
  }

  updateKpis(): void {
    const totalItems = this.productsList.length;
    const activeListings = this.productsList.filter(p => p.status === 'Active').length;
    const lowStock = this.productsList.filter(p => p.stock > 0 && p.stock <= 10).length;
    const inventoryValue = this.productsList.reduce((sum, p) => sum + (p.price * p.stock), 0);

    this.kpis.set([
      { label: 'Total Items',      value: totalItems.toLocaleString(),         trend: '+0%',          isUp: true,  icon: 'inventory_2', accentClass: 'text-primary'   },
      { label: 'Active Listings',  value: activeListings.toLocaleString(),     trend: 'Stable',       isUp: true,  icon: 'store',       accentClass: 'text-secondary' },
      { label: 'Low Stock Alerts', value: lowStock.toLocaleString(),           trend: lowStock > 0 ? 'Action Req.' : 'Good', isUp: lowStock === 0, icon: 'warning',     accentClass: 'text-tertiary'  },
      { label: 'Inventory Value',  value: '$' + inventoryValue.toLocaleString(undefined, {minimumFractionDigits: 2}), trend: '↑ 0%',          isUp: true,  icon: 'payments',    accentClass: 'text-primary'   },
    ]);
  }

  // ── Modal Logic ──
  openCreateModal(): void {
    this.isCreateModalOpen.set(true);
  }

  closeCreateModal(): void {
    this.isCreateModalOpen.set(false);
  }

  submitProduct(event: Event): void {
    event.preventDefault();
    const form = event.target as HTMLFormElement;
    const orgId = this.orgCtx.getActiveOrgId();
    if (!orgId) return;

    const formData = new FormData();
    formData.append('OrganizationId', orgId);
    formData.append('Name', (form.elements.namedItem('prodName') as HTMLInputElement).value);
    formData.append('Description', (form.elements.namedItem('prodDesc') as HTMLInputElement).value);
    formData.append('ProductType', (form.elements.namedItem('prodType') as HTMLSelectElement).value);
    formData.append('StockQuantity', (form.elements.namedItem('prodStock') as HTMLInputElement).value);
    
    const isDirect = (form.elements.namedItem('isDirect') as HTMLInputElement).checked;
    const isAuction = (form.elements.namedItem('isAuction') as HTMLInputElement).checked;
    
    formData.append('IsAvailableForDirectSale', String(isDirect));
    formData.append('IsAvailableForAuction', String(isAuction));
    
    if (isDirect) {
      formData.append('DirectSalePrice', (form.elements.namedItem('directPrice') as HTMLInputElement).value || '0');
    }
    if (isAuction) {
      formData.append('StartBiddingPrice', (form.elements.namedItem('auctionPrice') as HTMLInputElement).value || '0');
    }

    const fileInput = form.elements.namedItem('prodImage') as HTMLInputElement;
    if (fileInput.files && fileInput.files.length > 0) {
      formData.append('Images', fileInput.files[0]);
    }

    this.isCreatingProduct.set(true);
    this.productService.createProduct(formData).subscribe({
      next: () => {
        this.isCreatingProduct.set(false);
        this.closeCreateModal();
        this.loadProducts();
      },
      error: (err) => {
        this.isCreatingProduct.set(false);
        console.error(err);
        alert('Error creating product. Check console.');
      }
    });
  }

  // ── Row Actions ──
  toggleDropdown(id: string, event: Event): void {
    event.stopPropagation();
    if (this.activeDropdownId() === id) {
      this.activeDropdownId.set(null);
    } else {
      this.activeDropdownId.set(id);
    }
  }

  deleteProduct(id: string): void {
    if(confirm('Are you sure you want to delete this product?')) {
      this.productService.deleteProduct(id).subscribe({
        next: () => {
          this.loadProducts();
        },
        error: (err) => { console.error(err); alert('Error deleting product'); }
      });
    }
  }

  changeStatus(id: string, newStatus: number): void {
    this.productService.changeStatus(id, newStatus).subscribe({
      next: () => this.loadProducts(),
      error: (err) => { console.error(err); alert('Error changing status'); }
    });
  }

  getStatusClasses(status: string): string {
    const map: Record<string, string> = {
      'Active':       'm3-status-active',
      'Low Stock':    'm3-status-warning',
      'Out of Stock': 'm3-status-error',
      'Draft':        'm3-status-draft',
    };
    return map[status] ?? 'm3-status-draft';
  }

  getDotClasses(status: string): string {
    const map: Record<string, string> = {
      'Active':       'm3-dot-active',
      'Low Stock':    'm3-dot-warning',
      'Out of Stock': 'm3-dot-error',
      'Draft':        'm3-dot-draft',
    };
    return map[status] ?? 'm3-dot-draft';
  }
}
