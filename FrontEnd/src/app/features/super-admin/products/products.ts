import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductService } from './product.service';
import { ToastService } from '../../../core/services/toast.service';
import { ConfirmDialogService } from '../../../core/services/confirm-dialog.service';
import { SkeletonComponent } from '../../../shared/components/skeleton/skeleton.component';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, SkeletonComponent],
  templateUrl: './products.html'
})
export class Products implements OnInit {
  private productService = inject(ProductService);

  private toast = inject(ToastService);
  private confirmDialog = inject(ConfirmDialogService);

  isLoading = signal(true);
  products = signal<any[]>([]);
  
  stats = {
    total: 0,
    lowStock: 0,
    activeAuctions: 0
  };

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
        this.isLoading.set(false);
      }
    });
  }

  deleteProduct(id: string) {
    this.confirmDialog.open({
      title: 'Delete Product',
      message: 'Are you sure you want to delete this product?',
      confirmLabel: 'Delete',
      isDestructive: true
    }).subscribe((confirmed: any) => {
      if (!confirmed) return;

      this.productService.deleteProduct(id).subscribe({
        next: () => {
          this.toast.success('Product deleted successfully');
          this.loadProducts();
        },
        error: (err: any) => {
          console.error(err);
          this.toast.error('Error deleting product');
        }
      });
    });
  }
}
