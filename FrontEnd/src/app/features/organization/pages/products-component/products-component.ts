import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { OrgContextService } from '../../../../core/services/org-context.service';
import { ProductResponse } from '../../../../core/model/Organization/productResponse';
import { ProductOverviewResponse } from '../../../../core/model/Organization/productOverviewResponse';

@Component({
  selector: 'app-products-component',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './products-component.html',
  styleUrl: './products-component.css',
})
export class ProductsComponent implements OnInit {
  private readonly orgCtx = inject(OrgContextService);
  private readonly fb = inject(FormBuilder);

  overview = signal<ProductOverviewResponse | null>(null);
  products = signal<ProductResponse[]>([]);

  showAddModal = signal(false);
  productForm: FormGroup;
  selectedFiles: File[] = [];

  currentPage = 1;
  pageSize = 10;

  constructor() {
    this.productForm = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      stockQuantity: [0, [Validators.required, Validators.min(0)]],
      isAvailableForDirectSale: [true],
      directSalePrice: [0, Validators.min(0)],
      isAvailableForAuction: [false],
      startBiddingPrice: [0, Validators.min(0)],
      barcode: [''],
      expiryDate: [null],
      weightUnit: [null],
      productType: [0, Validators.required] // Assuming 0 is a default ProductType enum
    });
  }

  ngOnInit(): void {
    this.fetchOverview();
    this.fetchProducts();
  }

  fetchOverview(): void {
    this.orgCtx.productOverview().subscribe({
      next: (data) => this.overview.set(data)
    });
  }

  fetchProducts(): void {
    this.orgCtx.products(this.currentPage, this.pageSize).subscribe({
      next: (data) => this.products.set(data)
    });
  }

  onFileSelected(event: any): void {
    if (event.target.files) {
      this.selectedFiles = Array.from(event.target.files);
    }
  }

  submitProduct(): void {
    if (this.productForm.invalid) return;

    const formData = new FormData();
    const activeOrgId = this.orgCtx.getActiveOrgId();
    
    if (!activeOrgId) return;

    formData.append('OrganizationId', activeOrgId);
    
    Object.keys(this.productForm.controls).forEach(key => {
      const value = this.productForm.get(key)?.value;
      if (value !== null && value !== undefined) {
        formData.append(key, value);
      }
    });

    this.selectedFiles.forEach(file => {
      formData.append('Images', file);
    });

    this.orgCtx.addProduct(formData).subscribe({
      next: () => {
        this.showAddModal.set(false);
        this.productForm.reset({
          stockQuantity: 0,
          isAvailableForDirectSale: true,
          directSalePrice: 0,
          isAvailableForAuction: false,
          startBiddingPrice: 0,
          productType: 0
        });
        this.selectedFiles = [];
        this.fetchOverview();
        this.fetchProducts();
      },
      error: (err) => console.error('Error adding product:', err)
    });
  }

  openAddModal(): void {
    this.showAddModal.set(true);
  }

  closeAddModal(): void {
    this.showAddModal.set(false);
  }

  nextPage(): void {
    this.currentPage++;
    this.fetchProducts();
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.fetchProducts();
    }
  }
}
