import { Component, inject, OnInit, signal, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { OrgContextService } from '../../../../core/services/org-context.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ProductService } from '../../../super-admin/products/product.service';
import { ToastService } from '../../../../core/services/toast.service';
import { ConfirmDialogService } from '../../../../core/services/confirm-dialog.service';
import { BaseChartDirective } from 'ng2-charts';
import { Chart, ChartConfiguration, ChartData, ChartType, registerables } from 'chart.js';

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
  rejectionReason?: string;
  originalData?: any;
}

export interface ProductKpi {
  label: string;
  value: string;
  trend: string;
  isUp: boolean;
  icon: string;
  accentClass: string;
}

import { SkeletonComponent } from '../../../../shared/components/skeleton/skeleton.component';

@Component({
  selector: 'app-products-component',
  standalone: true,
  imports: [CommonModule, FormsModule, BaseChartDirective, SkeletonComponent],
  templateUrl: './products-component.html',
  styleUrl: './products-component.css',
})
export class ProductsComponent implements OnInit {
  private readonly productService = inject(ProductService);

  private readonly orgCtx = inject(OrgContextService);
  private readonly toast = inject(ToastService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  public readonly auth = inject(AuthService);

  constructor() {
    Chart.register(...registerables);
  }

  readonly kpis = signal<ProductKpi[]>([
    { label: 'Total Items',      value: '0',   trend: '+0%',       isUp: true,  icon: 'inventory_2',    accentClass: 'text-primary' },
    { label: 'Approved Products',  value: '0',   trend: 'Stable',    isUp: true,  icon: 'check_circle',   accentClass: 'text-secondary' },
    { label: 'Pending Approval', value: '0',   trend: 'Action Req.', isUp: false, icon: 'pending',        accentClass: 'text-tertiary' },
    { label: 'Inventory Value',  value: '$0',  trend: '↑ 0%',      isUp: true,  icon: 'payments',       accentClass: 'text-primary' },
  ]);

  productsList: Product[] = [];
  allProducts: Product[] = [];

  searchTerm = signal('');
  showPending = signal(false);
  activeFilter = signal('All');
  isFilterDropdownOpen = signal(false);
  isLoading = signal(true);
  isCreatingProduct = signal(false);
  isCreateModalOpen = signal(false);
  editingProduct = signal<Product | null>(null);
  activeDropdownId = signal<string | null>(null);
  isHelpOpen = signal(false);

  // Chart properties
  public categoryChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { position: 'right' }
    }
  };
  public categoryChartType: ChartType = 'doughnut';
  public categoryChartData: ChartData<'doughnut', number[], string | string[]> = {
    labels: [],
    datasets: [ { data: [] } ]
  };

  public inventoryChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { display: false }
    },
    scales: {
      y: { beginAtZero: true }
    }
  };
  public inventoryChartType: ChartType = 'bar';
  public inventoryChartData: ChartData<'bar', number[], string | string[]> = {
    labels: [],
    datasets: [ { data: [], backgroundColor: '#1d1d1f', borderRadius: 6 } ]
  };

  ngOnInit(): void {
    this.loadProducts();
  }

  @HostListener('document:click')
  onDocumentClick() {
    this.activeDropdownId.set(null);
    this.isFilterDropdownOpen.set(false);
  }

  toggleProductsHelp(): void {
    this.isHelpOpen.update(v => !v);
  }

  closeProductsHelp(): void {
    this.isHelpOpen.set(false);
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

        this.allProducts = products.map((p: any): Product => {
          let imageUrl = p.imageUrl || (p.images && p.images.length > 0 ? p.images[0] : '');
          if (imageUrl && !imageUrl.startsWith('http')) {
            imageUrl = imageUrl.startsWith('/')
              ? `https://root2route.runasp.net${imageUrl}`
              : `https://root2route.runasp.net/${imageUrl}`;
          }
          return {
          id:        p.id,
          name:      p.name      || 'Unnamed Product',
          sku:       p.sku || p.barcode || 'N/A',
          category:  this.mapProductType(p.productType),
          price:     p.directSalePrice ?? p.price ?? 0,
          stock:     p.stockQuantity ?? p.stock ?? 0,
          dateAdded: p.dateAdded || p.createdOn || new Date().toLocaleDateString(),
          status:    this.mapStatus(p.status),
          imageUrl:  imageUrl,
          rejectionReason: p.rejectionReason || null,
          originalData: p
          };
        });
        
        this.filterProducts();
      },
      error: (err) => {
        console.error('Error loading products', err);
        this.allProducts = [];
        this.filterProducts();
        this.isLoading.set(false);
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
    if (status === 1 || status === 'Approved') return 'Approved';
    if (status === 0 || status === 'Pending') return 'Pending';
    if (status === 2 || status === 'Rejected') return 'Rejected';
    return 'Pending';
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
    this.updateCharts();
    this.isLoading.set(false);

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
    const approvedListings = this.productsList.filter(p => p.status === 'Approved').length;
    const pendingListings = this.productsList.filter(p => p.status === 'Pending').length;
    const inventoryValue = this.productsList.reduce((sum, p) => sum + (p.price * p.stock), 0);

    this.kpis.set([
      { label: 'Total Items',      value: totalItems.toLocaleString(),         trend: '+0%',          isUp: true,  icon: 'inventory_2', accentClass: 'text-primary'   },
      { label: 'Approved Products',value: approvedListings.toLocaleString(),   trend: 'Stable',       isUp: true,  icon: 'check_circle',accentClass: 'text-secondary' },
      { label: 'Pending Approval', value: pendingListings.toLocaleString(),    trend: pendingListings > 0 ? 'Wait' : 'Clear', isUp: pendingListings === 0, icon: 'pending', accentClass: 'text-tertiary'  },
      { label: 'Inventory Value',  value: '$' + inventoryValue.toLocaleString(undefined, {minimumFractionDigits: 2}), trend: '↑ 0%',          isUp: true,  icon: 'payments',    accentClass: 'text-primary'   },
    ]);
  }

  updateCharts(): void {
    const categoryCounts: Record<string, number> = {};
    const categoryValues: Record<string, number> = {};

    this.productsList.forEach(p => {
      categoryCounts[p.category] = (categoryCounts[p.category] || 0) + 1;
      const val = p.price * p.stock;
      categoryValues[p.category] = (categoryValues[p.category] || 0) + val;
    });

    const labels = Object.keys(categoryCounts);
    
    this.categoryChartData = {
      labels: labels,
      datasets: [
        {
          data: labels.map(l => categoryCounts[l]),
          backgroundColor: ['#006B3D', '#34c759', '#ffcc00', '#ff3b30', '#007aff'],
          borderWidth: 0
        }
      ]
    };

    this.inventoryChartData = {
      labels: labels,
      datasets: [
        {
          label: 'Inventory Value ($)',
          data: labels.map(l => categoryValues[l]),
          backgroundColor: '#1d1d1f',
          borderRadius: 6
        }
      ]
    };
  }

  // ── Modal Logic ──
  openCreateModal(): void {
    this.editingProduct.set(null);
    this.isCreateModalOpen.set(true);
  }

  openEditModal(product: Product): void {
    this.editingProduct.set(product);
    this.isCreateModalOpen.set(true);
  }

  closeCreateModal(): void {
    this.isCreateModalOpen.set(false);
    this.editingProduct.set(null);
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
    const directPrice = (form.elements.namedItem('directPrice') as HTMLInputElement).value || '0';
    const auctionPrice = (form.elements.namedItem('auctionPrice') as HTMLInputElement).value || '0';
    const prodName = (form.elements.namedItem('prodName') as HTMLInputElement).value;
    const prodDesc = (form.elements.namedItem('prodDesc') as HTMLInputElement).value;
    const prodType = (form.elements.namedItem('prodType') as HTMLSelectElement).value;
    const prodStock = (form.elements.namedItem('prodStock') as HTMLInputElement).value;

    const currentEdit = this.editingProduct();

    if (currentEdit) {
      // Edit flow (JSON)
      const updateCmd = {
        id: currentEdit.id,
        name: prodName,
        description: prodDesc,
        productType: parseInt(prodType),
        stockQuantity: parseInt(prodStock),
        isAvailableForDirectSale: isDirect,
        directSalePrice: isDirect ? parseFloat(directPrice) : 0,
        isAvailableForAuction: isAuction,
        startBiddingPrice: isAuction ? parseFloat(auctionPrice) : 0,
        barcode: currentEdit.originalData?.barcode || null,
        weightUnit: currentEdit.originalData?.weightUnit || 0
      };

      this.isCreatingProduct.set(true);
      this.productService.updateProduct(updateCmd).subscribe({
        next: () => {
          this.isCreatingProduct.set(false);
          this.toast.success('Product updated successfully!');
          this.closeCreateModal();
          this.loadProducts();
        },
        error: (err) => {
          this.isCreatingProduct.set(false);
          console.error(err);
          this.toast.error('Error updating product. Check console.');
        }
      });
    } else {
      // Create flow (FormData)
      const formData = new FormData();
      formData.append('OrganizationId', orgId);
      formData.append('Name', prodName);
      formData.append('Description', prodDesc);
      formData.append('ProductType', prodType);
      formData.append('StockQuantity', prodStock);
      
      formData.append('IsAvailableForDirectSale', String(isDirect));
      formData.append('IsAvailableForAuction', String(isAuction));
      
      if (isDirect) formData.append('DirectSalePrice', directPrice);
      if (isAuction) formData.append('StartBiddingPrice', auctionPrice);

      const fileInput = form.elements.namedItem('prodImage') as HTMLInputElement;
      if (fileInput.files && fileInput.files.length > 0) {
        formData.append('Images', fileInput.files[0]);
      }

      this.isCreatingProduct.set(true);
      this.productService.createProduct(formData).subscribe({
        next: () => {
          this.isCreatingProduct.set(false);
          this.toast.success('Product created successfully!');
          this.closeCreateModal();
          this.loadProducts();
        },
        error: (err) => {
          this.isCreatingProduct.set(false);
          console.error(err);
          this.toast.error('Error creating product. Check console.');
        }
      });
    }
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

  async deleteProduct(id: string): Promise<void> {
    const confirmed = await this.confirmDialog.open({
      title: 'Delete Product',
      message: 'Are you sure you want to delete this product? This action cannot be undone.',
      confirmLabel: 'Delete Product',
      isDestructive: true
    });
    if (!confirmed) return;

    this.productService.deleteProduct(id).subscribe({
      next: () => {
        this.toast.success('Product deleted successfully.');
        this.loadProducts();
      },
      error: (err) => {
        console.error(err);
        this.toast.error('Error deleting product');
      }
    });
  }



  getStatusClasses(status: string): string {
    const map: Record<string, string> = {
      'Approved':       'm3-status-active',
      'Pending':        'm3-status-warning',
      'Rejected':       'm3-status-error',
    };
    return map[status] ?? 'm3-status-draft';
  }

  getDotClasses(status: string): string {
    const map: Record<string, string> = {
      'Approved':       'm3-dot-active',
      'Pending':        'm3-dot-warning',
      'Rejected':       'm3-dot-error',
    };
    return map[status] ?? 'm3-dot-draft';
  }

  onImageError(event: Event): void {
    const imgElement = event.target as HTMLImageElement;
    imgElement.src = 'assets/images/placeholder.png';
  }
}
