import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AiLabService } from './ai-lab.service';
import { ToastService } from '../../../core/services/toast.service';

@Component({
  selector: 'app-ai-lab',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './ai-lab.html'
})
export class AiLab implements OnInit {
  private aiLabService = inject(AiLabService);
  private toast = inject(ToastService);

  isPlaygroundLoading = signal(false);
  selectedFileName = signal('');

  // 🟢 ضفنا متغيرات للـ offset عشان الـ HTML يقرأ منها
  metrics = signal({
    modelAverageConfidence: '0%',
    confidenceOffset: 351.85,

    totalDiagnoses: '0',
    diagnosesOffset: 351.85,

    accuracyDrift: '0%',
    driftOffset: 351.85
  });

  diseases = signal<any[]>([]);

  playgroundResult = signal({
    topPrediction: 'Waiting for upload...',
    confidenceLevel: 0
  });

  // 🟢 محيط الدائرة الثابت
  private readonly CIRCUMFERENCE = 351.85;

  ngOnInit() {
    this.loadAiData();
  }

  loadAiData() {
    this.aiLabService.getAiDashboardData().subscribe({
      next: (response: any) => {
        const confidenceStr = response.accuracyTrend?.data?.averageConfidence || '94.2%';
        const driftStr = response.accuracyTrend?.data?.drift || '-0.3%';
        const totalDiag = response.accuracyTrend?.data?.totalDiagnoses || '12,840';

        this.metrics.set({
          modelAverageConfidence: confidenceStr,
          confidenceOffset: this.calculateOffset(confidenceStr),

          totalDiagnoses: totalDiag,
          diagnosesOffset: this.calculateOffsetForNumber(totalDiag, 15000),

          accuracyDrift: driftStr,
          driftOffset: this.calculateOffset(driftStr)
        });

        this.diseases.set(response.topDiseases?.data || [
          { name: 'Late Blight', colorClass: 'bg-red-500' },
          { name: 'Rust', colorClass: 'bg-orange-500' },
          { name: 'Septoria', colorClass: 'bg-yellow-500' },
          { name: 'Powdery Mildew', colorClass: 'bg-purple-500' }
        ]);
      },
      error: (error: any) => {
        console.error('Error fetching AI data', error);
        // Fallback data
        this.metrics.set({
          modelAverageConfidence: '94.2%', confidenceOffset: this.calculateOffset('94.2%'),
          totalDiagnoses: '12,840', diagnosesOffset: this.calculateOffsetForNumber('12840', 15000),
          accuracyDrift: '-0.3%', driftOffset: this.calculateOffset('-0.3%')
        });
      }
    });
  }

  // 🟢 دالة بتحول النص زي '94.2%' لرقم وتحسب رسمة الدائرة
  private calculateOffset(percentString: string): number {
    // استخراج الرقم من النص
    let percent = parseFloat(percentString.replace(/[^0-9.-]+/g, '')) || 0;
    percent = Math.abs(percent); // لو سالب نخليه موجب للرسم
    if (percent > 100) percent = 100; // أقصى حاجة 100%

    return this.CIRCUMFERENCE - (percent / 100) * this.CIRCUMFERENCE;
  }

  // 🟢 دالة بتحسب رسمة الدائرة للأرقام العادية بناءً على هدف معين (مثلاً الهدف 15000 تشخيص)
  private calculateOffsetForNumber(valueStr: string, target: number): number {
    let value = parseFloat(valueStr.replace(/[^0-9.-]+/g, '')) || 0;
    let percent = (value / target) * 100;
    if (percent > 100) percent = 100;

    return this.CIRCUMFERENCE - (percent / 100) * this.CIRCUMFERENCE;
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      this.selectedFileName.set(file.name);
      this.runAnalysis(file);
    }
  }

  runAnalysis(file: File): void {
    this.isPlaygroundLoading.set(true);
    this.playgroundResult.set({ topPrediction: 'Analyzing...', confidenceLevel: 0 });

    this.aiLabService.analyzeImage(file).subscribe({
      next: (response: any) => {
        const data = response.data || response;
        this.playgroundResult.set({
          topPrediction: data.prediction || data.diseaseName || data.label || 'Unknown',
          confidenceLevel: data.confidence || data.confidenceScore || 0
        });
        this.isPlaygroundLoading.set(false);
      },
      error: (err: any) => {
        console.error('Analysis error', err);
        this.playgroundResult.set({ topPrediction: 'Analysis failed', confidenceLevel: 0 });
        this.isPlaygroundLoading.set(false);
        this.toast.error('Image analysis failed. Please try again.');
      }
    });
  }
}