import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { forkJoin, of } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { MatDialogModule } from '@angular/material/dialog';

import { SiteService } from '../../services/site.service';
import { PageService } from '../../services/page.service';
import { Site } from '../../models/site.model';
import { Page } from '../../models/page.model';
import { ConfirmDialogComponent } from '../../shared/components/confirm-dialog/confirm-dialog.component';

interface SiteWithPages {
  site: Site;
  pages: Page[];
}

@Component({
  selector: 'app-pages-page',
  standalone: true,
  imports: [
    CommonModule,
    MatExpansionModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    MatDialogModule,
    DatePipe,
  ],
  templateUrl: './pages-page.component.html',
  styleUrl: './pages-page.component.css',
})
export class PagesPageComponent implements OnInit {
  displayedColumns: string[] = [
    'id',
    'name',
    'routing',
    'createdDate',
    'showInMenu',
    'actions',
  ];
  sitesWithPages: SiteWithPages[] = [];
  loading = true;
  error: string | null = null;

  constructor(
    private siteService: SiteService,
    private pageService: PageService,
    private router: Router,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loadSitesAndPages();
  }

  loadSitesAndPages(): void {
    this.loading = true;
    this.error = null;

    this.siteService
      .getSites()
      .pipe(
        switchMap((sites) => {
          if (sites.length === 0) {
            return of([]);
          }

          const sitesWithPagesObservables = sites.map((site) =>
            this.pageService.getPagesBySiteId(site.id).pipe(
              catchError(() => of([])),
              switchMap((pages) => of({ site, pages }))
            )
          );

          return forkJoin(sitesWithPagesObservables);
        }),
        catchError((err) => {
          console.error('Error loading sites and pages:', err);
          this.error =
            'Siteler ve sayfalar yüklenirken bir hata oluştu. Lütfen daha sonra tekrar deneyin.';
          return of([]);
        })
      )
      .subscribe({
        next: (data) => {
          this.sitesWithPages = data;
          this.loading = false;
        },
        error: (err) => {
          console.error('Unhandled error:', err);
          this.error = 'Beklenmeyen bir hata oluştu.';
          this.loading = false;
        },
      });
  }

  editPage(site: Site, page: Page): void {
    this.router.navigate(['/admin/add-page'], {
      queryParams: {
        siteId: site.id,
        siteName: site.name,
        pageId: page.id,
        mode: 'edit',
      },
    });
  }

  deletePage(site: Site, page: Page): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'Sayfa Silme Onayı',
        message: `"${page.name}" sayfasını silmek istediğinizden emin misiniz? Bu işlem geri alınamaz.`,
        confirmButtonText: 'Evet, Sil',
        cancelButtonText: 'İptal',
      },
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.pageService.deletePage(page.id).subscribe({
          next: () => {
            // Reload all pages data from server
            this.loadSitesAndPages();

            this.snackBar.open(
              `"${page.name}" sayfası başarıyla silindi.`,
              'Kapat',
              {
                duration: 5000,
                panelClass: 'success-snackbar',
              }
            );
          },
          error: (error) => {
            console.error('Error deleting page:', error);
            this.snackBar.open(
              'Sayfa silinirken bir hata oluştu. Lütfen tekrar deneyin.',
              'Kapat',
              {
                duration: 5000,
                panelClass: 'error-snackbar',
              }
            );
          },
        });
      }
    });
  }

  viewPage(site: Site, page: Page): void {
    if (site.domain) {
      window.open(`http://${site.domain}${page.routing}`, '_blank');
    }
  }

  navigateToAddPage(event: Event, site: Site): void {
    // Stop the accordion from toggling when clicking the button
    event.stopPropagation();
    // Navigate to the add page form with the site ID as a parameter
    this.router.navigate(['/admin/add-page'], {
      queryParams: { siteId: site.id, siteName: site.name },
    });
  }
}
