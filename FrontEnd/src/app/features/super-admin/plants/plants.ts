import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PlantService } from './plant.service';
import { ToastService } from '../../../core/services/toast.service';
import { ConfirmDialogService } from '../../../core/services/confirm-dialog.service';
import { SkeletonComponent } from '../../../shared/components/skeleton/skeleton.component';

@Component({
  selector: 'app-plants',
  standalone: true,
  imports: [CommonModule, FormsModule, SkeletonComponent],
  templateUrl: './plants.html'
})
export class Plants implements OnInit {
  private plantService = inject(PlantService);
  private toast = inject(ToastService);
  private confirmDialog = inject(ConfirmDialogService);

  isLoading = signal(true);
  isDrawerOpen = false;
  isStepsDrawerOpen = false;
  isEditing = false;
  isEditingStep = false;
  selectedPlant: any = null;

  stats = {
    total: '0'
  };

  plants = signal<any[]>([]);
  plantSteps = signal<any[]>([]);

  // Filtering
  searchQuery = signal('');
  seasonFilter = signal('All');

  filteredPlants = computed(() => {
    const query = this.searchQuery().toLowerCase();
    const season = this.seasonFilter().toLowerCase();
    
    return this.plants().filter(plant => {
      const matchesSearch = plant.name?.toLowerCase().includes(query) || plant.scientificName?.toLowerCase().includes(query);
      const matchesSeason = season === 'all' || plant.plantingSeason?.toLowerCase().includes(season);
      return matchesSearch && matchesSeason;
    });
  });

  // Unique seasons for the filter dropdown
  uniqueSeasons = computed(() => {
    const seasons = this.plants()
      .map(p => p.plantingSeason)
      .filter(s => !!s)
      .flatMap(s => s.split(/[\s,]+/)) // Split "Spring, Summer" into ["Spring", "Summer"]
      .map(s => s.trim().toLowerCase())
      .filter(s => s.length > 0);
    
    // Capitalize first letter and get unique
    const unique = [...new Set(seasons)].map(s => s.charAt(0).toUpperCase() + s.slice(1));
    return unique.sort();
  });

  // Step Form State
  stepFormState = {
    id: '',
    stepOrder: 1,
    title: '',
    instruction: '',
    plantInfoId: ''
  };

  // Form State
  formState = {
    id: '',
    name: '',
    scientificName: '',
    description: '',
    idealSoil: '',
    plantingSeason: '',
    medicalBenefits: ''
  };
  selectedFile: File | null = null;
  isProcessing = signal(false);

  ngOnInit() {
    this.loadPlants();
  }

  loadPlants() {
    this.isLoading.set(true);
    this.plantService.getAllPlants().subscribe({
      next: (response: any) => {
        const rawData = response.data || response || [];
        const items = Array.isArray(rawData) ? rawData : [];

        const processed = items.map((plant: any) => {
          let imageUrl = plant.imageUrl || '';
          if (imageUrl && !imageUrl.startsWith('http')) {
            imageUrl = imageUrl.startsWith('/')
              ? `https://root2route.runasp.net${imageUrl}`
              : `https://root2route.runasp.net/${imageUrl}`;
          }
          return {
            ...plant,
            imageUrl,
            initial: (plant.name ? plant.name.charAt(0) : '?').toUpperCase(),
            bgClass: 'bg-emerald-100 text-emerald-700'
          };
        });

        this.plants.set(processed);
        this.stats.total = this.plants().length.toString();
        this.isLoading.set(false);
      },
      error: (error: any) => {
        console.error('Error fetching plants', error);
        this.toast.error('Failed to load plants.');
        this.isLoading.set(false);
      }
    });
  }

  openAddDrawer() {
    this.isEditing = false;
    this.selectedPlant = null;
    this.resetForm();
    this.isDrawerOpen = true;
  }

  openEditDrawer(plant: any) {
    this.isEditing = true;
    this.selectedPlant = plant;
    this.formState = {
      id: plant.id,
      name: plant.name || '',
      scientificName: plant.scientificName || '',
      description: plant.description || '',
      idealSoil: plant.idealSoil || '',
      plantingSeason: plant.plantingSeason || '',
      medicalBenefits: plant.medicalBenefits || ''
    };
    this.selectedFile = null;
    this.isDrawerOpen = true;
  }

  closeDrawer() {
    this.isDrawerOpen = false;
    this.selectedPlant = null;
    this.resetForm();
  }

  resetForm() {
    this.formState = {
      id: '',
      name: '',
      scientificName: '',
      description: '',
      idealSoil: '',
      plantingSeason: '',
      medicalBenefits: ''
    };
    this.selectedFile = null;
    this.isProcessing.set(false);
  }

  onFileSelected(event: any) {
    if (event.target.files && event.target.files.length > 0) {
      this.selectedFile = event.target.files[0];
    }
  }

  savePlant() {
    if (!this.formState.name || !this.formState.scientificName || !this.formState.description) {
      this.toast.error('Please fill in all required fields (Name, Scientific Name, Description).');
      return;
    }

    if (!this.isEditing && !this.selectedFile) {
      this.toast.error('Please upload a plant image.');
      return;
    }

    this.isProcessing.set(true);

    const formData = new FormData();
    if (this.isEditing) {
      formData.append('Id', this.formState.id);
    }
    formData.append('Name', this.formState.name);
    formData.append('ScientificName', this.formState.scientificName);
    formData.append('Description', this.formState.description);
    formData.append('IdealSoil', this.formState.idealSoil);
    formData.append('PlantingSeason', this.formState.plantingSeason);
    formData.append('MedicalBenefits', this.formState.medicalBenefits);

    if (this.selectedFile) {
      formData.append('Image', this.selectedFile);
    }

    const request$ = this.isEditing
      ? this.plantService.editPlant(formData)
      : this.plantService.createPlant(formData);

    request$.subscribe({
      next: () => {
        this.toast.success(`Plant ${this.isEditing ? 'updated' : 'added'} successfully!`);
        this.loadPlants();
        this.closeDrawer();
      },
      error: (error: any) => {
        console.error('Error saving plant', error);
        this.toast.error(`Failed to ${this.isEditing ? 'update' : 'add'} plant.`);
        this.isProcessing.set(false);
      }
    });
  }

  async deletePlant(id: string, event: Event) {
    event.stopPropagation();

    const confirmed = await this.confirmDialog.open({
      title: 'Delete Plant',
      message: 'Are you sure you want to delete this plant?',
      confirmLabel: 'Delete',
      isDestructive: true
    });

    if (!confirmed) {
      return;
    }

    this.plantService.deletePlant(id).subscribe({
      next: () => {
        this.toast.success('Plant deleted successfully.');
        this.loadPlants();
      },
      error: (error: any) => {
        console.error('Error deleting plant', error);
        this.toast.error('Failed to delete plant.');
      }
    });
  }

  // --- Plant Guide Steps Methods ---

  openStepsDrawer(plant: any) {
    this.selectedPlant = plant;
    this.isStepsDrawerOpen = true;
    this.loadSteps(plant.id);
    this.resetStepForm();
  }

  closeStepsDrawer() {
    this.isStepsDrawerOpen = false;
    this.selectedPlant = null;
    this.plantSteps.set([]);
    this.resetStepForm();
  }

  loadSteps(plantId: string) {
    this.isProcessing.set(true);
    this.plantService.getStepsByPlantId(plantId).subscribe({
      next: (response: any) => {
        const data = response.data || response;
        this.plantSteps.set(data.steps || []);
        this.isProcessing.set(false);
      },
      error: (error: any) => {
        console.error('Error loading steps', error);
        this.isProcessing.set(false);
        this.plantSteps.set([]);
      }
    });
  }

  resetStepForm() {
    this.isEditingStep = false;
    this.stepFormState = {
      id: '',
      stepOrder: this.plantSteps().length + 1,
      title: '',
      instruction: '',
      plantInfoId: this.selectedPlant?.id || ''
    };
  }

  editStep(step: any) {
    this.isEditingStep = true;
    this.stepFormState = {
      id: step.id,
      stepOrder: step.stepOrder,
      title: step.title || '',
      instruction: step.instruction || '',
      plantInfoId: this.selectedPlant.id
    };
  }

  saveStep() {
    if (!this.stepFormState.title || !this.stepFormState.instruction) {
      this.toast.error('Please fill in step title and instruction.');
      return;
    }

    this.isProcessing.set(true);

    const request$ = this.isEditingStep
      ? this.plantService.editStep(this.stepFormState)
      : this.plantService.createStep(this.stepFormState);

    request$.subscribe({
      next: () => {
        this.toast.success(`Step ${this.isEditingStep ? 'updated' : 'added'} successfully.`);
        this.loadSteps(this.selectedPlant.id);
        this.resetStepForm();
      },
      error: (error: any) => {
        console.error('Error saving step', error);
        this.toast.error('Failed to save step.');
        this.isProcessing.set(false);
      }
    });
  }

  async deleteStep(id: string) {
    const confirmed = await this.confirmDialog.open({
      title: 'Delete Step',
      message: 'Are you sure you want to delete this step?',
      confirmLabel: 'Delete',
      isDestructive: true
    });

    if (!confirmed) return;

    this.isProcessing.set(true);
    this.plantService.deleteStep(id).subscribe({
      next: () => {
        this.toast.success('Step deleted successfully.');
        this.loadSteps(this.selectedPlant.id);
        this.resetStepForm();
      },
      error: (error: any) => {
        console.error('Error deleting step', error);
        this.toast.error('Failed to delete step.');
        this.isProcessing.set(false);
      }
    });
  }
}
