import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PlantAnalysisService } from '../../../../core/services/plant-analysis.service';
import { ToastService } from '../../../../core/services/toast.service';

@Component({
  selector: 'app-plant-analysis',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="apple-dashboard" style="padding: 24px;">
      <header class="apple-header" style="margin-bottom: 24px;">
        <div class="header-text">
          <h1 class="header-title" style="margin: 0;">Plant Disease Detection</h1>
          <p class="header-subtitle" style="margin-top: 8px;">Upload a plant leaf image and our AI model will diagnose potential diseases.</p>
        </div>
      </header>

      <div class="apple-card" style="padding: 32px; max-width: 720px;">
        <!-- Upload Area -->
        <div class="upload-zone" (click)="fileInput.click()"
             (dragover)="$event.preventDefault()" (drop)="onDrop($event)"
             [style.borderColor]="preview() ? '#10b981' : '#d1d5db'">
          @if (preview()) {
            <img [src]="preview()" alt="Plant preview" style="max-height: 280px; border-radius: 12px; object-fit: contain;" />
          } @else {
            <span class="material-symbols-outlined" style="font-size: 48px; color: #94a3b8;">cloud_upload</span>
            <p style="margin: 12px 0 0; font-size: 15px; color: #64748b; font-weight: 600;">Drop an image here or click to upload</p>
            <p style="margin: 4px 0 0; font-size: 12px; color: #94a3b8;">Supports JPG, PNG up to 10MB</p>
          }
        </div>
        <input #fileInput type="file" accept="image/*" hidden (change)="onFileSelected($event)" />

        <button class="analyze-btn" (click)="analyze()" [disabled]="isAnalyzing() || !selectedFile">
          @if (isAnalyzing()) {
            <span class="spinner"></span> Analyzing...
          } @else {
            <span class="material-symbols-outlined" style="font-size: 20px;">biotech</span>
            Analyze Plant
          }
        </button>

        <!-- Results -->
        @if (result()) {
          <div class="result-card">
            <h3 style="margin: 0 0 16px; font-size: 18px; font-weight: 800; color: #0f172a;">Analysis Result</h3>
            <div class="result-row">
              <span class="result-label">Disease</span>
              <span class="result-value" [style.color]="result()?.prediction?.toLowerCase()?.includes('healthy') ? '#16a34a' : '#dc2626'">
                {{ result()?.prediction || 'Unknown' }}
              </span>
            </div>
            <div class="result-row">
              <span class="result-label">Confidence</span>
              <span class="result-value">{{ (result()?.confidence || 0) | number:'1.1-1' }}%</span>
            </div>
            @if (result()?.expertAdvice) {
              <div class="result-row" style="flex-direction: column; align-items: flex-start; gap: 8px;">
                <span class="result-label">Expert Advice</span>
                <p style="margin: 0; font-size: 14px; color: #475569; line-height: 1.6; white-space: pre-line;">{{ result()?.expertAdvice }}</p>
              </div>
            }
          </div>
        }
      </div>
    </div>
  `,
  styles: [`
    .upload-zone { border: 2px dashed #d1d5db; border-radius: 16px; padding: 48px; display: flex; flex-direction: column; align-items: center; justify-content: center; cursor: pointer; transition: all .2s; background: #fafafa; min-height: 200px; }
    .upload-zone:hover { border-color: #10b981; background: #f0fdf4; }
    .analyze-btn { width: 100%; margin-top: 20px; padding: 14px; background: #10b981; color: white; border: none; border-radius: 12px; font-size: 15px; font-weight: 700; cursor: pointer; display: flex; align-items: center; justify-content: center; gap: 8px; transition: background .2s; }
    .analyze-btn:hover { background: #059669; }
    .analyze-btn:disabled { opacity: .5; cursor: not-allowed; }
    .result-card { margin-top: 24px; padding: 24px; background: #f8fafc; border-radius: 16px; border: 1px solid #e2e8f0; }
    .result-row { display: flex; align-items: center; justify-content: space-between; padding: 12px 0; border-bottom: 1px solid #f1f5f9; }
    .result-row:last-child { border-bottom: none; }
    .result-label { font-size: 13px; font-weight: 700; color: #64748b; text-transform: uppercase; letter-spacing: .05em; }
    .result-value { font-size: 16px; font-weight: 800; color: #0f172a; }
    .spinner { width: 18px; height: 18px; border: 2px solid white; border-top-color: transparent; border-radius: 50%; animation: spin .6s linear infinite; }
    @keyframes spin { to { transform: rotate(360deg); } }
  `]
})
export class PlantAnalysisComponent {
  private plantAnalysis = inject(PlantAnalysisService);
  private toast = inject(ToastService);

  selectedFile: File | null = null;
  preview = signal<string | null>(null);
  isAnalyzing = signal(false);
  result = signal<any>(null);

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
      const reader = new FileReader();
      reader.onload = (e) => this.preview.set(e.target?.result as string);
      reader.readAsDataURL(this.selectedFile);
      this.result.set(null);
    }
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    if (event.dataTransfer?.files && event.dataTransfer.files.length > 0) {
      this.selectedFile = event.dataTransfer.files[0];
      const reader = new FileReader();
      reader.onload = (e) => this.preview.set(e.target?.result as string);
      reader.readAsDataURL(this.selectedFile);
      this.result.set(null);
    }
  }

  analyze() {
    if (!this.selectedFile) return;
    this.isAnalyzing.set(true);
    this.result.set(null);

    const formData = new FormData();
    formData.append('Image', this.selectedFile);

    this.plantAnalysis.analyzePlantImage(formData).subscribe({
      next: (res: any) => {
        this.isAnalyzing.set(false);
        const data = res.data || res;
        this.result.set(data);
      },
      error: (err) => {
        this.isAnalyzing.set(false);
        console.error('Analysis failed', err);
        this.toast.error('Plant analysis failed. Please try again.');
      }
    });
  }
}
