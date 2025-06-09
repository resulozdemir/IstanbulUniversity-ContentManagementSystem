import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterOutlet, RouterLink } from '@angular/router';
import { SiteService } from './services/site.service';
import { Site } from './models/site.model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterOutlet, RouterLink],
  template: `
    <div class="app-container">
      <div class="admin-panel">
        <h2>İstanbul Üniversitesi CMS Görüntüleyici</h2>

        <div class="control-group">
          <label for="site">Site Seçimi: </label>
          <select id="site" [(ngModel)]="selectedSiteId" (change)="loadSite()">
            <option *ngFor="let site of availableSites" [value]="site.id">
              {{ site.name }} ({{ site.domain || 'Site Şablonu' }})
            </option>
          </select>
        </div>

        <div class="status">
          <div class="active-site">
            <span>Aktif Site:</span> {{ activeSiteName }}
          </div>
        </div>

        <div class="tools">
          <a routerLink="/pages" class="nav-link">Sayfa Görüntüleyici</a>
          <a routerLink="/debug" class="debug-link">JS Debugger</a>
        </div>
      </div>

      <div class="content-view">
        <router-outlet></router-outlet>
      </div>
    </div>
  `,
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  selectedSiteId: number = 0;
  availableSites: Site[] = [];
  activeSiteName: string = '';

  get isDebugRoute(): boolean {
    return window.location.pathname.includes('/debug');
  }

  constructor(private siteService: SiteService) {}

  ngOnInit(): void {
    console.log('🚀 AppComponent başlatılıyor...');

    // Mevcut siteleri API'den yükle
    this.loadAvailableSites();

    // Aktif site adını takip et
    this.siteService.currentSite$.subscribe((site) => {
      console.log('🎯 Aktif site değişti:', site);
      this.activeSiteName = site ? site.name : '';
    });
  }

  /**
   * API'den tüm siteleri yükler
   */
  loadAvailableSites(): void {
    console.log('📥 Siteler yükleniyor...');
    this.siteService.loadSites().subscribe((sites) => {
      console.log('📝 Gelen siteler:', sites);

      // API dokümanına göre ispublish === 1 olan siteleri filtrele
      this.availableSites = sites.filter((site) => {
        return site.ispublish === 1;
      });

      console.log('✅ Publish edilmiş siteler:', this.availableSites);

      // Varsayılan site yükle
      if (this.availableSites.length > 0 && this.selectedSiteId === 0) {
        this.selectedSiteId = this.availableSites[0].id;
        console.log('🎯 Varsayılan site seçiliyor:', this.selectedSiteId);
        this.loadSite();
      }
    });
  }

  /**
   * Seçilen siteyi yükler
   */
  loadSite(): void {
    console.log('🎯 Site yükleniyor:', this.selectedSiteId);
    if (this.selectedSiteId) {
      this.siteService.selectSite(this.selectedSiteId);
    }
  }
}
