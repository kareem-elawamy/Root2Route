import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { WithdrawalService } from '../../../core/services/withdrawal.service';
import { ToastService } from '../../../core/services/toast.service';
import { SkeletonComponent } from '../../../shared/components/skeleton/skeleton.component';

@Component({
  selector: 'app-withdrawals',
  standalone: true,
  imports: [CommonModule, FormsModule, SkeletonComponent],
  templateUrl: './withdrawals.html',
  styleUrl: './withdrawals.css'
})
export class Withdrawals implements OnInit {
  private withdrawalService = inject(WithdrawalService);
  private toast = inject(ToastService);

  pendingWithdrawals = signal<any[]>([]);
  isLoading = signal(true);
  isActioning = signal(false);

  // Modal / Action state
  selectedWithdrawal = signal<any>(null);
  showNoteModal = signal(false);
  actionType = signal<'approve' | 'reject' | null>(null);
  adminNote = signal('');

  // Selected filter
  filterStatus = signal('Pending'); // Pending or Recent

  // Local history of processed requests in the current session
  recentActions = signal<any[]>([]);

  ngOnInit() {
    this.loadPending();
  }

  loadPending() {
    this.isLoading.set(true);
    this.withdrawalService.getPendingWithdrawals().subscribe({
      next: (res: any) => {
        const data = res.data || res;
        this.pendingWithdrawals.set(Array.isArray(data) ? data : []);
        this.isLoading.set(false);
      },
      error: (err: any) => {
        console.error('Failed to load pending withdrawals', err);
        this.toast.error('Failed to fetch pending withdrawal requests.');
        this.isLoading.set(false);
      }
    });
  }

  openNoteModal(withdrawal: any, type: 'approve' | 'reject') {
    this.selectedWithdrawal.set(withdrawal);
    this.actionType.set(type);
    this.adminNote.set('');
    this.showNoteModal.set(true);
  }

  closeNoteModal() {
    this.showNoteModal.set(false);
    this.selectedWithdrawal.set(null);
    this.actionType.set(null);
    this.adminNote.set('');
  }

  submitAction() {
    const w = this.selectedWithdrawal();
    const type = this.actionType();
    const note = this.adminNote().trim();

    if (!w || !type) return;

    this.isActioning.set(true);

    if (type === 'approve') {
      this.withdrawalService.approveWithdrawal(w.id, note).subscribe({
        next: (res: any) => {
          if (res.succeeded) {
            this.toast.success(`Withdrawal of ${w.amount} EGP approved.`);
            this.addRecentAction(w, 'Approved', note);
            this.loadPending();
          } else {
            this.toast.error(res.message || 'Failed to approve request.');
          }
          this.isActioning.set(false);
          this.closeNoteModal();
        },
        error: (err) => {
          console.error(err);
          this.toast.error('An error occurred during approval.');
          this.isActioning.set(false);
          this.closeNoteModal();
        }
      });
    } else {
      this.withdrawalService.rejectWithdrawal(w.id, note).subscribe({
        next: (res: any) => {
          if (res.succeeded) {
            this.toast.success(`Withdrawal of ${w.amount} EGP rejected.`);
            this.addRecentAction(w, 'Rejected', note);
            this.loadPending();
          } else {
            this.toast.error(res.message || 'Failed to reject request.');
          }
          this.isActioning.set(false);
          this.closeNoteModal();
        },
        error: (err) => {
          console.error(err);
          this.toast.error('An error occurred during rejection.');
          this.isActioning.set(false);
          this.closeNoteModal();
        }
      });
    }
  }

  processWithdrawal(w: any) {
    if (!confirm(`Are you sure you want to mark withdrawal of ${w.amount} EGP for ${w.orgName || w.organization?.name} as completed/paid?`)) {
      return;
    }

    this.isActioning.set(true);
    this.withdrawalService.processWithdrawal(w.id).subscribe({
      next: (res: any) => {
        if (res.succeeded) {
          this.toast.success('Withdrawal marked as fully processed.');
          this.updateRecentActionStatus(w.id, 'Processed');
          this.loadPending();
        } else {
          this.toast.error(res.message || 'Failed to process request.');
        }
        this.isActioning.set(false);
      },
      error: (err) => {
        console.error(err);
        this.toast.error('An error occurred.');
        this.isActioning.set(false);
      }
    });
  }

  addRecentAction(w: any, status: string, note: string) {
    const action = {
      ...w,
      status,
      adminNote: note,
      actionDate: new Date()
    };
    this.recentActions.update(actions => [action, ...actions]);
  }

  updateRecentActionStatus(id: string, newStatus: string) {
    this.recentActions.update(actions => 
      actions.map(a => a.id === id ? { ...a, status: newStatus } : a)
    );
  }
}
