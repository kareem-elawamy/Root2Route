import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductService } from './product.service';
import { ToastService } from '../../../core/services/toast.service';
import { ConfirmDialogService } from '../../../core/services/confirm-dialog.service';
import { SkeletonComponent } from '../../../shared/components/skeleton/skeleton.component';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, SkeletonComponent, FormsModule],
  templateUrl: './products.html'
})
export class Products implements OnInit {
  private productService = inject(ProductService);

  private toast = inject(ToastService);
  private confirmDialog = inject(ConfirmDialogService);

  isLoading = signal(true);
  products = signal<any[]>([]);

  // Filtering
  showFilters = signal(false);
  searchQuery = signal('');
  statusFilter = signal('All');

  filteredProducts = computed(() => {
    const query = this.searchQuery().toLowerCase();
    const status = this.statusFilter();
    
    return this.products().filter(p => {
      const matchesSearch = p.name?.toLowerCase().includes(query) || p.description?.toLowerCase().includes(query) || p.id?.toLowerCase().includes(query);
      const matchesStatus = status === 'All' 
        || (status === 'Direct Sale' && p.isAvailableForDirectSale)
        || (status === 'Auction' && p.isAvailableForAuction)
        || (status === 'Inactive' && !p.isAvailableForDirectSale && !p.isAvailableForAuction);
      return matchesSearch && matchesStatus;
    });
  });
  
  stats = {
    total: 0,
    lowStock: 0,
    activeAuctions: 0
  };

  chartData = computed(() => {
    const all = this.products();
    const total = all.length || 1;
    const directSale = all.filter(p => p.isAvailableForDirectSale).length;
    const auction = all.filter(p => p.isAvailableForAuction && !p.isAvailableForDirectSale).length;
    const inactive = total - directSale - auction;

    const directPercent = Math.round((directSale / total) * 100);
    const auctionPercent = Math.round((auction / total) * 100);
    const inactivePercent = Math.round((inactive / total) * 100);

    return {
      directPercent,
      auctionPercent,
      inactivePercent,
      gradient: `conic-gradient(
        #10b981 0% ${directPercent}%, 
        #f59e0b ${directPercent}% ${directPercent + auctionPercent}%, 
        #cbd5e1 ${directPercent + auctionPercent}% 100%
      )`
    };
  });

  ngOnInit() {
    this.loadProducts();
  }

  loadProducts() {
    this.productService.getAllProducts(1, 50).subscribe({
      next: (response: any) => {
        const data = response.data || response.items || response || [];
        const items = Array.isArray(data) ? data : [];

        const processed = items.map((product: any) => {
          let imageUrl = product.mainImageUrl || (product.images && product.images.length > 0 ? product.images[0] : null);
          if (imageUrl && !imageUrl.startsWith('http')) {
            // Construct full absolute URL if it is a relative path
            imageUrl = imageUrl.startsWith('/') 
              ? `https://root2route.runasp.net${imageUrl}` 
              : `https://root2route.runasp.net/${imageUrl}`;
          }

          return {
            ...product,
            mainImageUrl: imageUrl,
            displayPrice: product.isAvailableForDirectSale ? product.directSalePrice : product.startBiddingPrice,
            priceLabel: product.isAvailableForDirectSale ? 'Direct Sale' : 'Auction Starting',
            statusLabel: product.isAvailableForDirectSale || product.isAvailableForAuction ? 'Active' : 'Inactive',
            statusClass: product.isAvailableForDirectSale || product.isAvailableForAuction ? 'bg-emerald-100 text-emerald-700' : 'bg-slate-100 text-slate-700'
          };
        });

        this.products.set(processed);

        this.stats.total = response.totalCount || this.products().length;
        this.stats.lowStock = this.products().filter((p: any) => p.stockQuantity < 10).length;
        this.stats.activeAuctions = this.products().filter((p: any) => p.isAvailableForAuction).length;

        this.isLoading.set(false);

      },
      error: (error: any) => {
        console.error('Error fetching products', error);
        this.toast.error('Failed to load products.');
        this.isLoading.set(false);
      }
    });
  }

  async deleteProduct(product: any) {
    const isAuctionProduct = product.isAvailableForAuction;
    const confirmed = await this.confirmDialog.open({
      title: 'Delete Product',
      message: isAuctionProduct
        ? 'This product is marked for auction. If it has active or upcoming auctions, deletion will be blocked. Continue?'
        : 'Are you sure you want to delete this product?',
      confirmLabel: 'Delete',
      isDestructive: true
    });
    if (!confirmed) return;

    this.productService.deleteProduct(product.id).subscribe({
      next: (res: any) => {
        if (res?.succeeded === false) {
          this.toast.error(res.message || 'Cannot delete this product.');
          return;
        }
        this.toast.success('Product deleted successfully');
        this.loadProducts();
      },
      error: (err: any) => {
        console.error(err);
        const msg = err?.error?.message || err?.error?.Message || 'Error deleting product';
        this.toast.error(msg);
      }
    });
  }

  // Edit Flow
  isDrawerOpen = signal(false);
  isProcessing = signal(false);
  
  formState = {
    id: '',
    name: '',
    description: '',
    stockQuantity: 0,
    isAvailableForDirectSale: false,
    directSalePrice: 0,
    isAvailableForAuction: false,
    startBiddingPrice: 0,
    barcode: '',
    productType: 0
  };

  mapProductTypeToNumber(typeStr: string | number): number {
    if (typeof typeStr === 'number') return typeStr;
    const s = (typeStr || '').toLowerCase();
    if (s.includes('vegetable')) return 0;
    if (s.includes('fruit')) return 1;
    if (s.includes('seed')) return 2;
    if (s.includes('equipment')) return 3;
    if (s.includes('fertilizer')) return 4;
    return 0;
  }

  openEditDrawer(product: any) {
    this.formState = {
      id: product.id,
      name: product.name || '',
      description: product.description || '',
      stockQuantity: product.stockQuantity || 0,
      isAvailableForDirectSale: product.isAvailableForDirectSale || false,
      directSalePrice: product.directSalePrice || 0,
      isAvailableForAuction: product.isAvailableForAuction || false,
      startBiddingPrice: product.startBiddingPrice || 0,
      barcode: product.barcode || '',
      productType: this.mapProductTypeToNumber(product.productType)
    };
    this.isDrawerOpen.set(true);
  }

  closeDrawer() {
    this.isDrawerOpen.set(false);
  }

  toggleFilters() {
    this.showFilters.set(!this.showFilters());
  }

  exportCatalog() {
    const data = this.filteredProducts();
    if (data.length === 0) {
      this.toast.error('No products to export.');
      return;
    }

    const headers = ['ID', 'Name', 'Description', 'Stock Quantity', 'Direct Sale', 'Auction', 'Display Price'];
    const csvRows = [headers.join(',')];

    for (const p of data) {
      const row = [
        p.id,
        `"${(p.name || '').replace(/"/g, '""')}"`,
        `"${(p.description || '').replace(/"/g, '""')}"`,
        p.stockQuantity || 0,
        p.isAvailableForDirectSale ? 'Yes' : 'No',
        p.isAvailableForAuction ? 'Yes' : 'No',
        p.displayPrice || 0
      ];
      csvRows.push(row.join(','));
    }

    const csvContent = csvRows.join('\n');
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.setAttribute('href', url);
    link.setAttribute('download', 'products_catalog.csv');
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    
    this.toast.success('Catalog exported successfully!');
  }

  saveProduct() {
    if (!this.formState.name) {
      this.toast.error('Product Name is required.');
      return;
    }
    
    this.isProcessing.set(true);
    
    this.productService.updateProduct(this.formState).subscribe({
      next: () => {
        this.toast.success('Product updated successfully!');
        this.loadProducts();
        this.closeDrawer();
        this.isProcessing.set(false);
      },
      error: (err: any) => {
        console.error(err);
        this.toast.error('Failed to update product.');
        this.isProcessing.set(false);
      }
    });
  }
}
