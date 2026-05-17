import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductService } from './product.service';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './products.html'
})
export class Products implements OnInit {
  private productService = inject(ProductService);
  private cdr = inject(ChangeDetectorRef);

  products: any[] = [];
  
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

        this.products = items.map((product: any) => {
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

        this.stats.total = response.totalCount || this.products.length;
        this.stats.lowStock = this.products.filter(p => p.stockQuantity < 10).length;
        this.stats.activeAuctions = this.products.filter(p => p.isAvailableForAuction).length;

        this.cdr.detectChanges();
      },
      error: (error: any) => {
        console.error('Error fetching products', error);
      }
    });
  }

  deleteProduct(id: string) {
    if (confirm('Are you sure you want to delete this product?')) {
      this.productService.deleteProduct(id).subscribe({
        next: () => {
          alert('Product deleted successfully');
          this.loadProducts();
        },
        error: (err) => {
          console.error(err);
          alert('Error deleting product');
        }
      });
    }
  }
}
