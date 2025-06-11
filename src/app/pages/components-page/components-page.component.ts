import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import {
  ThemeComponentService,
  ThemeComponent,
  Theme,
} from '../../services/theme-component.service';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-components-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatExpansionModule,
    MatInputModule,
    MatFormFieldModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    HttpClientModule,
    RouterModule,
  ],
  templateUrl: './components-page.component.html',
  styleUrl: './components-page.component.css',
})
export class ComponentsPageComponent implements OnInit {
  components: ThemeComponent[] = [];
  themes: Theme[] = [];
  isLoading = true;

  constructor(
    private themeComponentService: ThemeComponentService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.isLoading = true;

    // Load themes first
    this.themeComponentService.getThemes().subscribe({
      next: (themes) => {
        this.themes = themes;

        // Then load components
        this.themeComponentService.getThemeComponents().subscribe({
          next: (components) => {
            this.components = components;
            this.isLoading = false;
          },
          error: (error) => {
            console.error('Error loading components:', error);
            this.snackBar.open('Bileşenler yüklenirken hata oluştu!', 'Tamam', {
              duration: 3000,
            });
            this.isLoading = false;
          },
        });
      },
      error: (error) => {
        console.error('Error loading themes:', error);
        this.snackBar.open('Temalar yüklenirken hata oluştu!', 'Tamam', {
          duration: 3000,
        });
        this.isLoading = false;
      },
    });
  }

  getThemeName(themeId: number): string {
    const theme = this.themes.find((t) => t.id === themeId);
    return theme ? theme.name : 'Bilinmeyen Tema';
  }

  saveComponent(component: ThemeComponent): void {
    this.themeComponentService
      .updateThemeComponent(component.id, component)
      .subscribe({
        next: () => {
          this.snackBar.open('Bileşen başarıyla güncellendi!', 'Tamam', {
            duration: 3000,
          });
        },
        error: (error) => {
          console.error('Error updating component:', error);
          this.snackBar.open('Bileşen güncellenirken hata oluştu!', 'Tamam', {
            duration: 3000,
          });
        },
      });
  }

  deleteComponent(id: number): void {
    if (confirm('Bu bileşeni silmek istediğinizden emin misiniz?')) {
      this.themeComponentService.deleteThemeComponent(id).subscribe({
        next: () => {
          this.components = this.components.filter((c) => c.id !== id);
          this.snackBar.open('Bileşen başarıyla silindi!', 'Tamam', {
            duration: 3000,
          });
        },
        error: (error) => {
          console.error('Error deleting component:', error);
          this.snackBar.open('Bileşen silinirken hata oluştu!', 'Tamam', {
            duration: 3000,
          });
        },
      });
    }
  }
}
