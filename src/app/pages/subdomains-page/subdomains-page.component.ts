import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { DatePipe } from '@angular/common';

import { SiteService } from '../../services/site.service';
import {
  ThemeComponentService,
  Theme,
} from '../../services/theme-component.service';
import { Site } from '../../models/site.model';
import { ConfirmDialogComponent } from '../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-subdomains-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatExpansionModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    MatCheckboxModule,
    MatInputModule,
    MatFormFieldModule,
    MatSelectModule,
    MatSnackBarModule,
    MatCardModule,
    MatDialogModule,
    DatePipe,
  ],
  templateUrl: './subdomains-page.component.html',
  styleUrl: './subdomains-page.component.css',
})
export class SubdomainsPageComponent implements OnInit {
  sites: Site[] = [];
  themes: Theme[] = [];
  loading = true;
  error: string | null = null;

  // New site form
  newSite = {
    name: '',
    domain: '',
    themeId: 0,
    isPublish: 0,
  };

  // Edited sites tracking
  editedSites: { [key: number]: any } = {};

  constructor(
    private siteService: SiteService,
    private themeService: ThemeComponentService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadSites();
    this.loadThemes();
  }

  loadSites(): void {
    this.loading = true;
    this.error = null;

    this.siteService.getSites().subscribe({
      next: (data) => {
        this.sites = data;
        this.loading = false;
        // Reset edited sites tracking when loading new data
        this.editedSites = {};
      },
      error: (err) => {
        console.error('Error loading sites:', err);
        this.error =
          'Siteler yüklenirken bir hata oluştu. Lütfen daha sonra tekrar deneyin.';
        this.loading = false;
      },
    });
  }

  loadThemes(): void {
    this.themeService.getThemes().subscribe({
      next: (data) => {
        this.themes = data;
      },
      error: (err) => {
        console.error('Error loading themes:', err);
        this.snackBar.open('Temalar yüklenirken hata oluştu', 'Tamam', {
          duration: 3000,
        });
      },
    });
  }

  prepareSiteForEdit(site: Site): void {
    // Create a copy of the site data for editing if not already being edited
    if (!this.editedSites[site.id]) {
      this.editedSites[site.id] = {
        id: site.id,
        name: site.name,
        domain: site.domain,
        templateId: 0,
        themeId: site.themeId,
        language: 'empty',
        isTemplate: 0,
        analyticId: 'empty',
        pbysId: 0,
        key: 'empty',
        googleSiteVerification: 'empty',
        isPublish: site.ispublish,
        isDeleted: 0,
      };
    }
  }

  // Yeni metot - panel açıldığında verileri yükle
  onPanelOpened(site: Site): void {
    this.prepareSiteForEdit(site);
  }

  updateSite(site: Site): void {
    if (!this.editedSites[site.id]) {
      this.prepareSiteForEdit(site);
      return;
    }

    const editedSite = this.editedSites[site.id];

    this.loading = true;
    this.siteService.updateSite(site.id, editedSite).subscribe({
      next: () => {
        this.snackBar.open('Site başarıyla güncellendi', 'Tamam', {
          duration: 3000,
        });
        this.loadSites(); // Reload the sites to reflect changes
      },
      error: (err) => {
        console.error('Error updating site:', err);
        this.snackBar.open('Site güncellenirken hata oluştu', 'Tamam', {
          duration: 3000,
        });
        this.loading = false;
      },
    });
  }

  cancelEdit(site: Site): void {
    // Remove the site from edited sites to cancel the edit
    if (this.editedSites[site.id]) {
      delete this.editedSites[site.id];
    }
  }

  deleteSite(site: Site): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'Site Silme',
        message: `"${site.name}" sitesini silmek istediğinize emin misiniz?`,
        confirmButtonText: 'Sil',
        cancelButtonText: 'İptal',
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.loading = true;
        this.siteService.deleteSite(site.id).subscribe({
          next: () => {
            this.snackBar.open('Site başarıyla silindi', 'Tamam', {
              duration: 3000,
            });
            this.loadSites(); // Reload the sites to reflect changes
          },
          error: (err) => {
            console.error('Error deleting site:', err);
            this.snackBar.open('Site silinirken hata oluştu', 'Tamam', {
              duration: 3000,
            });
            this.loading = false;
          },
        });
      }
    });
  }

  createSite(): void {
    // Prepare the new site object according to the API requirements
    const newSiteRequest = {
      id: Math.floor(100000 + Math.random() * 900000), // 6 basamaklı unique ID
      name: this.newSite.name,
      domain: this.newSite.domain,
      templateId: 0,
      themeId: this.newSite.themeId,
      language: 'empty',
      isTemplate: 0,
      analyticId: 'empty',
      pbysId: 0,
      key: 'empty',
      googleSiteVerification: 'empty',
      isPublish: this.newSite.isPublish,
      isDeleted: 0,
    };

    this.loading = true;
    this.siteService.createSite(newSiteRequest).subscribe({
      next: () => {
        this.snackBar.open('Site başarıyla oluşturuldu', 'Tamam', {
          duration: 3000,
        });
        // Reset the new site form
        this.newSite = {
          name: '',
          domain: '',
          themeId: 0,
          isPublish: 0,
        };
        this.loadSites(); // Reload the sites to reflect changes
      },
      error: (err) => {
        console.error('Error creating site:', err);
        this.snackBar.open('Site oluşturulurken hata oluştu', 'Tamam', {
          duration: 3000,
        });
        this.loading = false;
      },
    });
  }

  resetNewSiteForm(): void {
    this.newSite = {
      name: '',
      domain: '',
      themeId: 0,
      isPublish: 0,
    };
  }

  viewSite(site: Site): void {
    if (site.domain) {
      window.open(`http://${site.domain}`, '_blank');
    }
  }
}
