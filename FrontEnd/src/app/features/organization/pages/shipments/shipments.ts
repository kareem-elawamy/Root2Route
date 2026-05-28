import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ShipmentsService } from '../../shipments.service';
import { OrgContextService } from '../../../../core/services/org-context.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-shipments',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './shipments.html'
})
export class ShipmentsComponent implements OnInit {
  private shipmentsService = inject(ShipmentsService);
  private orgCtx = inject(OrgContextService);
  private cdr = inject(ChangeDetectorRef);

  addresses: any[] = [];
  
  newAddress = {
    street: '',
    city: '',
    state: '',
    country: '',
    zipCode: ''
  };

  ngOnInit() {
    this.loadAddresses();
  }

  loadAddresses() {
    this.shipmentsService.getMyAddresses().subscribe({
      next: (response: any) => {
        const data = response.data || response || [];
        this.addresses = Array.isArray(data) ? data : [];
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        console.error('Error fetching addresses', error);
      }
    });
  }

  addAddress() {
    this.shipmentsService.addAddress(this.newAddress).subscribe({
      next: () => {
        alert('Address added successfully!');
        this.newAddress = {
          street: '',
          city: '',
          state: '',
          country: '',
          zipCode: ''
        };
        this.loadAddresses();
      },
      error: (error: any) => {
        console.error('Error adding address', error);
        alert('Failed to add address.');
      }
    });
  }
}
