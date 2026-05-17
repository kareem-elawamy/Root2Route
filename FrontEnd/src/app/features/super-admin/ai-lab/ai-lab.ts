import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AiLabService } from './ai-lab.service';

@Component({
  selector: 'app-ai-lab',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './ai-lab.html'
})
export class AiLab implements OnInit {
  private aiLabService = inject(AiLabService);
  private cdr = inject(ChangeDetectorRef);

  // 🟢 ضفنا متغيرات للـ offset عشان الـ HTML يقرأ منها
  metrics = {
    modelAverageConfidence: '0%',
    confidenceOffset: 351.85,

    totalDiagnoses: '0',
    diagnosesOffset: 351.85,

    accuracyDrift: '0%',
    driftOffset: 351.85
  };

  diseases: any[] = [];

  playgroundResult = {
    topPrediction: 'Waiting for upload...',
    confidenceLevel: 0
  };

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

        this.metrics = {
          modelAverageConfidence: confidenceStr,
          confidenceOffset: this.calculateOffset(confidenceStr),

          totalDiagnoses: totalDiag,
          // بالنسبة للعدد الإجمالي، ممكن نعتبر إن الهدف 15000 مثلاً عشان نرسم الدائرة
          diagnosesOffset: this.calculateOffsetForNumber(totalDiag, 15000),

          accuracyDrift: driftStr,
          driftOffset: this.calculateOffset(driftStr)
        };

        this.diseases = response.topDiseases?.data || [
          { name: 'Late Blight', colorClass: 'bg-red-500' },
          { name: 'Rust', colorClass: 'bg-orange-500' },
          { name: 'Septoria', colorClass: 'bg-yellow-500' },
          { name: 'Powdery Mildew', colorClass: 'bg-purple-500' }
        ];

        this.playgroundResult = {
          topPrediction: response.playground?.prediction || 'Tomato Early Blight',
          confidenceLevel: response.playground?.confidence || 98.4
        };

        this.cdr.detectChanges();
      },
      error: (error: any) => {
        console.error('Error fetching AI data', error);
        // Fallback data
        this.metrics = {
          modelAverageConfidence: '94.2%', confidenceOffset: this.calculateOffset('94.2%'),
          totalDiagnoses: '12,840', diagnosesOffset: this.calculateOffsetForNumber('12840', 15000),
          accuracyDrift: '-0.3%', driftOffset: this.calculateOffset('-0.3%')
        };
        this.cdr.detectChanges();
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
}