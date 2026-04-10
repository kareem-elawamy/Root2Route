import { Component, OnInit, inject } from '@angular/core';
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

  metrics = {
    modelAverageConfidence: '0%',
    totalDiagnoses: '0',
    accuracyDrift: '0%'
  };

  diseases: any[] = [];
  
  playgroundResult = {
    topPrediction: 'Waiting for upload...',
    confidenceLevel: 0
  };

  ngOnInit() {
    this.loadAiData();
  }

  loadAiData() {
    this.aiLabService.getAiDashboardData().subscribe({
      next: (response: any) => {
        console.log('AI Data from multiple endpoints:', response);
        
        // ربط داتا الـ Accuracy (لو الباك إند باعتها هنعرضها، لو لأ هنعرض الداتا الافتراضية)
        this.metrics = {
          modelAverageConfidence: response.accuracyTrend?.data?.averageConfidence || '94.2%',
          totalDiagnoses: response.accuracyTrend?.data?.totalDiagnoses || '12,840',
          accuracyDrift: response.accuracyTrend?.data?.drift || '-0.3%'
        };

        // ربط داتا الـ Diseases
        this.diseases = response.topDiseases?.data || [
          { name: 'Late Blight', colorClass: 'bg-red-500' },
          { name: 'Rust', colorClass: 'bg-orange-500' },
          { name: 'Septoria', colorClass: 'bg-yellow-500' },
          { name: 'Powdery Mildew', colorClass: 'bg-purple-500' }
        ];

        // الـ Playground ملوش Endpoint في الصورة، فهنسيبه ثابت دلوقتي
        this.playgroundResult = {
          topPrediction: 'Tomato Early Blight',
          confidenceLevel: 98.4
        };
      },
      error: (error: any) => {
        console.error('Error fetching AI data', error);
        // الداتا الافتراضية في حالة الإيرور
        this.metrics = { modelAverageConfidence: '94.2%', totalDiagnoses: '12,840', accuracyDrift: '-0.3%' };
        this.diseases = [
          { name: 'Late Blight', colorClass: 'bg-red-500' },
          { name: 'Rust', colorClass: 'bg-orange-500' },
          { name: 'Septoria', colorClass: 'bg-yellow-500' },
          { name: 'Powdery Mildew', colorClass: 'bg-purple-500' }
        ];
      }
    });
  }
}