import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { OrgContextService } from '../../../../core/services/org-context.service';
import { OrganizationService } from '../../../super-admin/organizations/organization.service';
import { ToastService } from '../../../../core/services/toast.service';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './settings.html',
  styleUrl: './settings.css'
})
export class SettingsComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly orgCtx = inject(OrgContextService);
  private readonly orgApi = inject(OrganizationService);
  private readonly toast = inject(ToastService);

  readonly activeOrg = this.orgCtx.activeOrg;
  
  settingsForm!: FormGroup;
  isSaving = signal(false);
  logoPreview = signal<string | null>(null);
  selectedLogoFile: File | null = null;

  ngOnInit(): void {
    this.settingsForm = this.fb.group({
      name: ['', Validators.required],
      description: [''],
      address: [''],
      contactEmail: ['', [Validators.required, Validators.email]],
      contactPhone: [''],
      type: [0]
    });

    // Populate form with active org
    const org = this.activeOrg();
    if (org) {
      this.settingsForm.patchValue({
        name: org.name || '',
        description: org.description || '',
        address: org.address || '',
        contactEmail: org.contactEmail || '',
        contactPhone: org.contactPhone || '',
        type: org.type || 0
      });
      if (org.logoPath) {
        this.logoPreview.set(`https://root2route.runasp.net${org.logoPath}`);
      }
    }
  }

  onLogoSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedLogoFile = input.files[0];
      const reader = new FileReader();
      reader.onload = (e) => this.logoPreview.set(e.target?.result as string);
      reader.readAsDataURL(this.selectedLogoFile);
    }
  }

  saveSettings(): void {
    if (this.settingsForm.invalid) {
      this.settingsForm.markAllAsTouched();
      return;
    }

    const org = this.activeOrg();
    if (!org) return;

    this.isSaving.set(true);
    const formVals = this.settingsForm.value;
    
    const formData = new FormData();
    formData.append('Name', formVals.name);
    formData.append('Description', formVals.description || '');
    formData.append('Address', formVals.address || '');
    formData.append('ContactEmail', formVals.contactEmail || '');
    formData.append('ContactPhone', formVals.contactPhone || '');
    formData.append('Type', formVals.type.toString());

    if (this.selectedLogoFile) {
      formData.append('Logo', this.selectedLogoFile);
    }

    this.orgApi.updateOrganization(org.id, formData).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.toast.success('Settings updated successfully!');
        // Refresh context
        this.orgCtx.myOrganization().subscribe();
      },
      error: (err) => {
        this.isSaving.set(false);
        console.error('Failed to update settings', err);
        this.toast.error('Failed to update settings.');
      }
    });
  }
}
