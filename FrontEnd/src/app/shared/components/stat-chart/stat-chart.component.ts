import {
  Component,
  input,
  effect,
  viewChild,
  ElementRef,
  afterNextRender,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  Chart,
  registerables,
  ChartType,
  ChartData,
  ChartOptions,
  ChartConfiguration,
} from 'chart.js';

// Register all Chart.js components globally once
Chart.register(...registerables);

export interface ChartDataset {
  label: string;
  data: number[];
  backgroundColor?: string | string[];
  borderColor?: string | string[];
  borderWidth?: number;
  tension?: number;
  fill?: boolean;
  pointRadius?: number;
  pointHoverRadius?: number;
  borderRadius?: number;
  barThickness?: number;
}

@Component({
  selector: 'app-stat-chart',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="stat-chart-wrapper">
      @if (chartTitle()) {
        <h3 class="stat-chart-title">{{ chartTitle() }}</h3>
      }
      <div class="stat-chart-canvas-container">
        <canvas #chartCanvas></canvas>
      </div>
    </div>
  `,
  styles: [`
    :host {
      display: block;
      width: 100%;
    }
    .stat-chart-wrapper {
      background: white;
      border-radius: 16px;
      border: 1px solid #e2e8f0;
      padding: 24px;
      box-shadow: 0 1px 3px rgba(0, 0, 0, 0.04);
    }
    .stat-chart-title {
      font-family: 'Outfit', sans-serif;
      font-weight: 700;
      font-size: 16px;
      color: #1e293b;
      margin: 0 0 16px 0;
    }
    .stat-chart-canvas-container {
      position: relative;
      width: 100%;
      min-height: 260px;
    }
  `]
})
export class StatChartComponent {
  // ── Signal Inputs ──
  chartType = input<ChartType>('bar');
  chartLabels = input<string[]>([]);
  chartDatasets = input<ChartDataset[]>([]);
  chartTitle = input<string>('');
  chartHeight = input<string>('260px');

  // Optional overrides
  showLegend = input<boolean>(true);
  showGrid = input<boolean>(true);
  aspectRatio = input<number | undefined>(undefined);

  // Canvas ref
  canvas = viewChild<ElementRef<HTMLCanvasElement>>('chartCanvas');

  private chart: Chart | null = null;

  constructor() {
    // Wait for the DOM to be ready before creating the chart
    afterNextRender(() => {
      this.createChart();
    });

    // React to ANY input signal change and update the chart
    effect(() => {
      // Read all signals to track dependencies
      const type = this.chartType();
      const labels = this.chartLabels();
      const datasets = this.chartDatasets();
      const legend = this.showLegend();
      const grid = this.showGrid();
      const ratio = this.aspectRatio();
      const height = this.chartHeight();

      // Only update if chart already exists (initial creation is in afterNextRender)
      if (this.chart) {
        this.updateChart(type, labels, datasets, legend, grid, ratio);
      }
    });
  }

  private createChart() {
    const canvasEl = this.canvas()?.nativeElement;
    if (!canvasEl) return;

    const type = this.chartType();
    const labels = this.chartLabels();
    const datasets = this.chartDatasets();

    // Set container height
    const container = canvasEl.parentElement;
    if (container) {
      container.style.minHeight = this.chartHeight();
    }

    const config: ChartConfiguration = {
      type,
      data: {
        labels,
        datasets: datasets.map(ds => ({
          ...ds,
          borderWidth: ds.borderWidth ?? 2,
          tension: ds.tension ?? 0.4,
        })),
      },
      options: this.buildOptions(type),
    };

    this.chart = new Chart(canvasEl, config);
  }

  private updateChart(
    type: ChartType,
    labels: string[],
    datasets: ChartDataset[],
    legend: boolean,
    grid: boolean,
    ratio: number | undefined
  ) {
    if (!this.chart) return;

    // If chart type changed, destroy and recreate
    if ((this.chart.config as any).type !== type) {
      this.chart.destroy();
      const canvasEl = this.canvas()?.nativeElement;
      if (!canvasEl) return;

      const config: ChartConfiguration = {
        type,
        data: {
          labels,
          datasets: datasets.map(ds => ({
            ...ds,
            borderWidth: ds.borderWidth ?? 2,
            tension: ds.tension ?? 0.4,
          })),
        },
        options: this.buildOptions(type),
      };
      this.chart = new Chart(canvasEl, config);
      return;
    }

    // Update data in-place
    this.chart.data.labels = labels;
    this.chart.data.datasets = datasets.map(ds => ({
      ...ds,
      borderWidth: ds.borderWidth ?? 2,
      tension: ds.tension ?? 0.4,
    }));
    this.chart.options = this.buildOptions(type);
    this.chart.update('none'); // 'none' = no animation on data update for snappy feel
  }

  private buildOptions(type: ChartType): ChartOptions {
    const isPie = type === 'pie' || type === 'doughnut';
    const showGrid = this.showGrid();
    const showLegend = this.showLegend();
    const aspectRatio = this.aspectRatio();

    return {
      responsive: true,
      maintainAspectRatio: !!aspectRatio,
      ...(aspectRatio ? { aspectRatio } : {}),
      plugins: {
        legend: {
          display: showLegend,
          position: isPie ? 'bottom' : 'top',
          labels: {
            color: '#64748b',
            font: { size: 12, weight: 'bold', family: "'Inter', sans-serif" },
            padding: 16,
            usePointStyle: true,
            pointStyle: 'circle',
          },
        },
        tooltip: {
          backgroundColor: '#1e293b',
          titleColor: '#f8fafc',
          bodyColor: '#e2e8f0',
          titleFont: { size: 13, weight: 'bold' },
          bodyFont: { size: 12 },
          cornerRadius: 10,
          padding: 12,
          displayColors: true,
          boxPadding: 4,
        },
      },
      scales: isPie
        ? {}
        : {
            x: {
              grid: { display: false },
              ticks: {
                color: '#94a3b8',
                font: { size: 11, weight: 'bold' as const },
              },
              border: { display: false },
            },
            y: {
              grid: {
                display: showGrid,
                color: '#f1f5f9',
              },
              ticks: {
                color: '#94a3b8',
                font: { size: 11 },
              },
              border: { display: false },
              beginAtZero: true,
            },
          },
      animation: {
        duration: 600,
        easing: 'easeOutQuart',
      },
    } as ChartOptions;
  }
}
