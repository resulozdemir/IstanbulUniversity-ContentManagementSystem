import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import {
  Theme,
  ThemeComponentService,
} from '../../services/theme-component.service';

@Component({
  selector: 'app-themes-page',
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
  ],
  templateUrl: './themes-page.component.html',
  styleUrl: './themes-page.component.css',
})
export class ThemesPageComponent implements OnInit {
  themes: Theme[] = [];
  newThemeName = '';
  loading = false;

  constructor(
    private themeService: ThemeComponentService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadThemes();
  }

  loadThemes(): void {
    this.loading = true;
    this.themeService.getThemes().subscribe({
      next: (data) => {
        this.themes = data;
        this.loading = false;
      },
      error: (err) => {
        this.snackBar.open('Temalar yüklenirken hata oluştu', 'Tamam', {
          duration: 3000,
        });
        console.error('Error loading themes:', err);
        this.loading = false;
      },
    });
  }

  updateTheme(theme: Theme): void {
    this.loading = true;
    // Ensure header and footer are set to 'empty' as required
    const themeToUpdate = {
      ...theme,
      header: 'empty',
      footer: 'empty',
    };

    this.themeService.updateTheme(theme.id, themeToUpdate).subscribe({
      next: () => {
        this.snackBar.open('Tema başarıyla güncellendi', 'Tamam', {
          duration: 3000,
        });
        this.loading = false;
      },
      error: (err) => {
        this.snackBar.open('Tema güncellenirken hata oluştu', 'Tamam', {
          duration: 3000,
        });
        console.error('Error updating theme:', err);
        this.loading = false;
      },
    });
  }

  deleteTheme(theme: Theme): void {
    if (confirm(`"${theme.name}" temasını silmek istediğinize emin misiniz?`)) {
      this.loading = true;
      this.themeService.deleteTheme(theme.id).subscribe({
        next: () => {
          // Remove the theme from the local array
          const index = this.themes.findIndex((t) => t.id === theme.id);
          if (index !== -1) {
            this.themes.splice(index, 1);
          }
          this.snackBar.open('Tema başarıyla silindi', 'Tamam', {
            duration: 3000,
          });
          this.loading = false;
        },
        error: (err) => {
          this.snackBar.open('Tema silinirken hata oluştu', 'Tamam', {
            duration: 3000,
          });
          console.error('Error deleting theme:', err);
          this.loading = false;
        },
      });
    }
  }

  createTheme(): void {
    if (!this.newThemeName.trim()) {
      this.snackBar.open('Lütfen tema adı giriniz', 'Tamam', {
        duration: 3000,
      });
      return;
    }

    this.loading = true;
    // Generate a random 6-digit unique ID
    const uniqueId = Math.floor(100000 + Math.random() * 900000);

    const newTheme: Theme = {
      id: uniqueId,
      name: this.newThemeName.trim(),
      header: 'empty',
      footer: 'empty',
      isDeleted: 0,
    };

    this.themeService.createTheme(newTheme).subscribe({
      next: (createdTheme) => {
        this.themes.push(createdTheme);
        this.newThemeName = '';
        this.snackBar.open('Tema başarıyla oluşturuldu', 'Tamam', {
          duration: 3000,
        });
        this.loading = false;
      },
      error: (err) => {
        this.snackBar.open('Tema oluşturulurken hata oluştu', 'Tamam', {
          duration: 3000,
        });
        console.error('Error creating theme:', err);
        this.loading = false;
      },
    });
  }
}
