import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PageService } from '../../services/page.service';
import { SiteService } from '../../services/site.service';
import { ComponentViewerComponent } from '../component-viewer/component-viewer.component';
import { Site } from '../../models/site.model';
import { Template } from '../../models/template.model';
import { Theme } from '../../models/theme.model';
import { Subscription } from 'rxjs';
import { ComponentService } from '../../services/component.service';

@Component({
  selector: 'app-unified-viewer',
  standalone: true,
  imports: [CommonModule, ComponentViewerComponent],
  template: `
    <div class="site-viewer">
      <!-- Site Header -->
      <div class="site-header" [ngStyle]="getHeaderStyles()">
        <div class="site-title">
          <h1>{{ currentSite?.name || 'Site Görüntüleyici' }}</h1>
          <div class="site-domain" *ngIf="currentSite">
            {{ currentSite.domain }}
          </div>
        </div>
      </div>

      <ng-container *ngIf="currentSite && currentTheme">
        <!-- Template yok ise varsayılan template adı göster -->
        <div class="template-theme-info">
          <div class="template-info">
            <div class="info-label">Template:</div>
            <div class="info-value">
              {{ currentTemplate?.name || 'Varsayılan Template' }}
            </div>
            <div class="info-desc" *ngIf="currentTemplate?.description">
              {{ currentTemplate?.description }}
            </div>
            <div class="info-desc" *ngIf="!currentTemplate?.description">
              Template ID: {{ currentTemplate?.id || 'Yok' }}
            </div>
          </div>
          <div class="theme-info">
            <div class="info-label">Tema:</div>
            <div class="info-value">{{ currentTheme.name }}</div>
            <div class="info-desc" *ngIf="currentTheme.description">
              {{ currentTheme.description }}
            </div>
            <div class="info-desc" *ngIf="!currentTheme.description">
              Theme ID: {{ currentTheme.id }}
            </div>
          </div>
          <div class="combination-info">
            <div class="info-label">Template-Tema:</div>
            <div class="info-value">{{ currentSite.name }}</div>
            <div class="info-desc">
              Template ID: {{ currentTemplate?.id || 'Yok' }} / Theme ID:
              {{ currentTheme.id }}
            </div>
          </div>
        </div>

        <!-- Renk Şeması Bilgisi -->
        <div class="color-scheme-info">
          <div class="color-item">
            <div
              class="color-preview primary-color"
              [ngStyle]="{
                'background-color': currentTheme.primaryColor || '#00733E'
              }"
            ></div>
            <div class="color-text">
              <div class="color-label">Ana Renk:</div>
              <div class="color-value">
                {{ currentTheme.primaryColor || '#00733E' }}
              </div>
            </div>
          </div>
          <div class="color-item">
            <div
              class="color-preview secondary-color"
              [ngStyle]="{
                'background-color': currentTheme.secondaryColor || '#E0B200'
              }"
            ></div>
            <div class="color-text">
              <div class="color-label">İkincil Renk:</div>
              <div class="color-value">
                {{ currentTheme.secondaryColor || '#E0B200' }}
              </div>
            </div>
          </div>
        </div>

        <!-- Site Layout -->
        <div class="site-preview">
          <div class="site-content">
            <!-- Ana Menü -->
            <div class="main-navigation" [ngStyle]="getMainNavigationStyles()">
              <div class="nav-container">
                <div class="logo-container">
                  <div class="site-logo">
                    <span class="logo-text">{{ currentSite.name }}</span>
                  </div>
                </div>
                <nav class="nav-links">
                  <ul>
                    <li *ngFor="let link of siteComponents?.links || []">
                      <a href="{{ link.url }}">{{ link.text }}</a>
                    </li>
                    <li *ngIf="!siteComponents?.links?.length">
                      <a href="/">Ana Sayfa</a>
                    </li>
                  </ul>
                </nav>
              </div>
            </div>

            <!-- İçerik Alanı -->
            <div class="content-container">
              <main class="main-content">
                <div class="content-area">
                  <h2 [ngStyle]="getContentHeaderStyles()">
                    {{ currentSite.name }}
                  </h2>

                  <!-- Site içeriği -->
                  <div class="special-content">
                    <h3>
                      {{ siteComponents?.contentTitle || currentSite.name }}
                    </h3>
                    <p>
                      {{
                        siteComponents?.contentText ||
                          'Bu sitede henüz içerik bulunmamaktadır.'
                      }}
                    </p>

                    <div
                      class="subdomain-highlight"
                      [ngStyle]="getSubdomainHighlightStyle()"
                      *ngIf="siteComponents?.highlightTitle"
                    >
                      <h4>{{ siteComponents?.highlightTitle }}</h4>
                      <p>{{ siteComponents?.highlightText }}</p>
                    </div>
                  </div>

                  <!-- İçerik Componentleri -->
                  <div class="components-area">
                    <h3>Sayfa Componentleri</h3>
                    <div class="component-status-panel">
                      <div class="component-status">
                        <div class="status-title">
                          Component Yükleme Durumu:
                        </div>
                        <div
                          class="status-info"
                          *ngIf="
                            currentPage && currentPage.components.length > 0
                          "
                        >
                          {{ currentPage.components.length }} adet component
                          yükleniyor
                        </div>
                        <div class="status-info error" *ngIf="!currentPage">
                          Sayfa bilgisi yüklenemedi
                        </div>
                        <div
                          class="status-info warning"
                          *ngIf="
                            currentPage && currentPage.components.length === 0
                          "
                        >
                          Bu sayfa için component bulunamadı
                        </div>
                      </div>

                      <div
                        class="component-list"
                        *ngIf="currentPage && currentPage.components.length > 0"
                      >
                        <div class="list-title">Component Listesi:</div>
                        <ul>
                          <li
                            *ngFor="let componentId of currentPage.components"
                            [class.loading]="isLoading(componentId)"
                            [class.loaded]="isLoaded(componentId)"
                            [class.error]="hasError(componentId)"
                          >
                            <span class="component-id"
                              >Component #{{ componentId }}</span
                            >
                            <div class="component-badges">
                              <span
                                *ngIf="isLoading(componentId)"
                                class="status-badge loading"
                                >Yükleniyor</span
                              >
                              <span
                                *ngIf="isLoaded(componentId)"
                                class="status-badge loaded"
                                >Yüklendi</span
                              >
                              <span
                                *ngIf="hasError(componentId)"
                                class="status-badge error"
                                >Hata</span
                              >
                              <span
                                class="component-type-badge"
                                *ngIf="getComponentTypeName(componentId)"
                                [ngClass]="getComponentTypeClass(componentId)"
                              >
                                {{ getComponentTypeName(componentId) }}
                              </span>
                            </div>
                          </li>
                        </ul>
                      </div>
                    </div>

                    <div class="components-container">
                      <div *ngIf="currentPage">
                        <div
                          *ngFor="let componentId of currentPage.components"
                          class="component-item"
                        >
                          <div class="component-header">
                            <h4>Component #{{ componentId }}</h4>
                            <div class="component-controls">
                              <button
                                (click)="refreshComponent(componentId)"
                                title="Yenile"
                                class="control-button refresh"
                              >
                                🔄
                              </button>
                            </div>
                          </div>
                          <app-component-viewer
                            [componentId]="componentId"
                          ></app-component-viewer>
                        </div>
                      </div>
                      <div
                        *ngIf="
                          !currentPage || currentPage.components.length === 0
                        "
                        class="no-components"
                      >
                        <p>Bu sayfa için henüz component eklenmemiş.</p>
                      </div>
                    </div>
                  </div>
                </div>
              </main>
            </div>

            <!-- Footer -->
            <footer class="site-footer" [ngStyle]="getFooterStyles()">
              <div class="footer-content">
                <div
                  [innerHTML]="
                    currentTheme.footer ||
                    '© ' +
                      getCurrentYear() +
                      ' - ' +
                      currentSite.name +
                      ' - Tüm Hakları Saklıdır'
                  "
                ></div>

                <div
                  class="footer-links"
                  *ngIf="siteComponents?.footerLinks?.length"
                >
                  <ul>
                    <li *ngFor="let link of siteComponents.footerLinks">
                      <a href="{{ link.url }}">{{ link.text }}</a>
                    </li>
                  </ul>
                </div>

                <div
                  class="social-media"
                  *ngIf="siteComponents?.socialMedia?.length"
                >
                  <div class="social-icons">
                    <a
                      *ngFor="let item of siteComponents.socialMedia"
                      href="{{ item.url }}"
                      target="_blank"
                    >
                      {{ item.platform }}
                    </a>
                  </div>
                </div>
              </div>
            </footer>
          </div>
        </div>
      </ng-container>

      <!-- Site bilgileri yüklenemediğinde -->
      <div class="loading-message" *ngIf="!currentSite || !currentTheme">
        <p>Site bilgileri yükleniyor...</p>
      </div>
    </div>
  `,
  styles: [
    `
      .site-viewer {
        background-color: #f4f4f4;
        min-height: 100%;
      }

      .site-header {
        background-color: #00733e;
        color: white;
        padding: 20px;
        text-align: center;
      }

      .site-title h1 {
        margin: 0;
        font-size: 24px;
      }

      .site-domain {
        margin-top: 5px;
        background-color: rgba(255, 255, 255, 0.2);
        display: inline-block;
        padding: 5px 10px;
        border-radius: 4px;
        font-size: 14px;
      }

      /* Template-Tema İlişkisi Bilgi Paneli */
      .template-theme-info {
        background-color: white;
        margin: 15px;
        padding: 15px;
        border-radius: 6px;
        box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
        display: grid;
        grid-template-columns: repeat(3, 1fr);
        gap: 15px;
      }

      .template-info,
      .theme-info,
      .combination-info {
        padding: 10px;
        border-radius: 4px;
        border: 1px solid #eee;
      }

      .info-label {
        font-weight: 600;
        font-size: 14px;
        color: #555;
        margin-bottom: 5px;
      }

      .info-value {
        font-size: 16px;
        color: #333;
        font-weight: 500;
        margin-bottom: 5px;
      }

      .info-desc {
        font-size: 13px;
        color: #666;
        line-height: 1.4;
      }

      /* Renk Şeması Bilgisi */
      .color-scheme-info {
        margin: 0 15px 15px;
        display: flex;
        gap: 15px;
      }

      .color-item {
        background-color: white;
        flex: 1;
        padding: 10px;
        border-radius: 6px;
        box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
        display: flex;
        align-items: center;
        gap: 15px;
      }

      .color-preview {
        width: 40px;
        height: 40px;
        border-radius: 4px;
        border: 1px solid rgba(0, 0, 0, 0.1);
      }

      .color-text {
        flex: 1;
      }

      .color-label {
        font-size: 12px;
        color: #555;
      }

      .color-value {
        font-family: monospace;
        font-size: 14px;
      }

      /* Site Önizleme */
      .site-preview {
        margin: 15px;
        border-radius: 6px;
        overflow: hidden;
        box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
        background-color: #fff;
      }

      /* Fakülte Layout */
      .site-content {
        min-height: 600px;
        display: flex;
        flex-direction: column;
      }

      .main-navigation {
        background-color: #00733e;
        color: white;
        padding: 15px 30px;
      }

      .nav-container {
        display: flex;
        align-items: center;
        justify-content: space-between;
      }

      .site-logo {
        font-size: 20px;
        font-weight: bold;
      }

      .nav-links ul {
        display: flex;
        list-style: none;
        margin: 0;
        padding: 0;
        gap: 20px;
      }

      .nav-links a {
        color: white;
        text-decoration: none;
        font-size: 16px;
      }

      .content-container {
        display: flex;
        flex: 1;
      }

      .side-navigation {
        width: 240px;
        background-color: #f4f4f4;
        padding: 20px;
      }

      .side-nav-container h3 {
        margin-top: 0;
        padding-bottom: 10px;
        border-bottom: 2px solid #00733e;
        color: #00733e;
      }

      .side-nav-container ul {
        list-style: none;
        padding: 0;
        margin: 0;
      }

      .side-nav-container li {
        margin-bottom: 10px;
      }

      .side-nav-container a {
        display: block;
        padding: 8px 10px;
        color: #333;
        text-decoration: none;
        border-left: 3px solid transparent;
        transition: all 0.2s;
      }

      .side-nav-container a:hover {
        background-color: rgba(224, 178, 0, 0.1);
        border-left-color: #e0b200;
      }

      .main-content {
        flex: 1;
        padding: 20px 30px;
      }

      .main-content h2 {
        margin-top: 0;
        color: #00733e;
        border-bottom: 2px solid #e0b200;
        padding-bottom: 10px;
      }

      .site-description {
        color: #555;
        line-height: 1.6;
      }

      .special-content {
        margin: 30px 0;
      }

      .special-content h3 {
        color: #333;
      }

      .subdomain-highlight {
        background-color: rgba(224, 178, 0, 0.1);
        border-left: 3px solid #e0b200;
        padding: 15px;
        margin: 20px 0;
      }

      .subdomain-highlight h4 {
        margin-top: 0;
        color: #333;
      }

      .components-area {
        margin-top: 30px;
      }

      .components-container {
        margin-top: 15px;
      }

      .component-item {
        margin-bottom: 20px;
        padding: 15px;
        border: 1px solid #eee;
        border-radius: 4px;
      }

      .no-components {
        padding: 30px;
        text-align: center;
        background-color: #f9f9f9;
        border-radius: 4px;
        color: #666;
      }

      .site-footer {
        background-color: #333;
        color: white;
        padding: 20px 30px;
      }

      .footer-content {
        display: flex;
        flex-wrap: wrap;
        justify-content: space-between;
        align-items: center;
      }

      .footer-links ul {
        display: flex;
        list-style: none;
        margin: 0;
        padding: 0;
        gap: 20px;
      }

      .footer-links a {
        color: white;
        text-decoration: none;
        font-size: 14px;
        opacity: 0.8;
        transition: opacity 0.2s;
      }

      .footer-links a:hover {
        opacity: 1;
      }

      .social-icons {
        display: flex;
        gap: 15px;
      }

      .social-icons a {
        color: white;
        text-decoration: none;
        opacity: 0.8;
        transition: opacity 0.2s;
      }

      .social-icons a:hover {
        opacity: 1;
      }

      .loading-message {
        display: flex;
        justify-content: center;
        align-items: center;
        min-height: 300px;
        font-size: 18px;
        color: #666;
      }

      /* Eklenen CSS stilleri */
      .component-status-panel {
        margin-bottom: 20px;
        background-color: #f8f9fa;
        border-radius: 8px;
        padding: 15px;
        box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
      }

      .component-status {
        margin-bottom: 10px;
      }

      .status-title,
      .list-title {
        font-weight: bold;
        margin-bottom: 5px;
        color: #333;
      }

      .status-info {
        padding: 5px 10px;
        background-color: #e9ecef;
        border-radius: 4px;
        font-size: 14px;
      }

      .status-info.error {
        background-color: #f8d7da;
        color: #721c24;
      }

      .status-info.warning {
        background-color: #fff3cd;
        color: #856404;
      }

      .component-list ul {
        list-style: none;
        padding: 0;
        margin: 0;
      }

      .component-list li {
        padding: 8px 12px;
        margin-bottom: 5px;
        border-radius: 4px;
        background-color: #e9ecef;
        display: flex;
        justify-content: space-between;
        align-items: center;
      }

      .component-list li.loading {
        background-color: #cce5ff;
        border-left: 4px solid #0d6efd;
      }

      .component-list li.loaded {
        background-color: #d4edda;
        border-left: 4px solid #28a745;
      }

      .component-list li.error {
        background-color: #f8d7da;
        border-left: 4px solid #dc3545;
      }

      .status-badge {
        font-size: 12px;
        padding: 2px 6px;
        border-radius: 10px;
        font-weight: bold;
      }

      .status-badge.loading {
        background-color: #0d6efd;
        color: white;
      }

      .status-badge.loaded {
        background-color: #28a745;
        color: white;
      }

      .status-badge.error {
        background-color: #dc3545;
        color: white;
      }

      .component-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 10px;
        padding: 10px 15px;
        border-bottom: 1px solid #eee;
        background-color: #f8f9fa;
      }

      .component-header h4 {
        margin: 0;
        font-size: 16px;
        color: #495057;
      }

      .component-controls button.control-button {
        background: none;
        border: none;
        cursor: pointer;
        font-size: 16px;
        padding: 6px 10px;
        border-radius: 4px;
        transition: background-color 0.2s;
      }

      .component-controls button.control-button:hover {
        background-color: #e9ecef;
      }

      .component-controls button.refresh:hover {
        background-color: #cce5ff;
      }

      .component-badges {
        display: flex;
        gap: 5px;
      }

      .component-id {
        font-weight: 500;
      }

      .component-type-badge {
        font-size: 12px;
        padding: 2px 8px;
        border-radius: 10px;
        background-color: #6c757d;
        color: white;
        font-weight: bold;
      }

      /* Özel component tipleri için renkler */
      .component-type-badge.navbar {
        background-color: #007bff;
      }

      .component-type-badge.header {
        background-color: #6610f2;
      }

      .component-type-badge.footer {
        background-color: #6f42c1;
      }

      .component-type-badge.slider {
        background-color: #e83e8c;
      }

      .component-type-badge.content {
        background-color: #28a745;
      }

      /* Diğer component türleri için renkler */
      .component-type-badge.ana-bileşen {
        background-color: #fd7e14;
      }

      .component-type-badge.layout {
        background-color: #20c997;
      }

      .component-type-badge.form {
        background-color: #17a2b8;
      }

      .component-type-badge.özel-bileşen {
        background-color: #6c757d;
      }
    `,
  ],
})
export class UnifiedViewerComponent implements OnInit, OnDestroy {
  currentPage: any = null;
  currentSite: Site | null = null;
  currentTemplate: Template | null = null;
  currentTheme: Theme | null = null;
  siteComponents: any = null;

  // Component durumunu izlemek için
  private componentStates: Map<string, 'loading' | 'loaded' | 'error'> =
    new Map();
  private componentErrors: Map<string, string> = new Map();
  private componentTypes: Map<string, string> = new Map();

  private subscriptions: Subscription = new Subscription();

  constructor(
    private pageService: PageService,
    private siteService: SiteService,
    private componentService: ComponentService
  ) {}

  ngOnInit(): void {
    console.log('🚀 UnifiedViewerComponent başlatılıyor...');

    // Site bilgilerini takip et
    this.subscriptions.add(
      this.siteService.currentSite$.subscribe((site) => {
        console.log('📝 Site değişti:', site);
        this.currentSite = site;
        if (site) {
          // Site ID'sine göre sayfa yükle
          this.pageService.loadPageForSite(site.id);

          // Site component verilerini yükle
          this.loadSiteComponents(site.id);
        }
      })
    );

    // Template bilgilerini takip et
    this.subscriptions.add(
      this.siteService.currentTemplate$.subscribe((template) => {
        console.log('📄 Template değişti:', template);
        this.currentTemplate = template;
      })
    );

    // Tema bilgilerini takip et
    this.subscriptions.add(
      this.siteService.currentTheme$.subscribe((theme) => {
        console.log('🎨 Theme değişti:', theme);
        this.currentTheme = theme;
      })
    );

    // Aktif sayfayı takip et
    this.subscriptions.add(
      this.pageService.currentPage$.subscribe((page) => {
        console.log('📄 Page değişti:', page);
        this.currentPage = page;

        // Component durumlarını izle
        if (page) {
          this.trackComponentStates();
        }
      })
    );
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  /**
   * Site için API'den component verilerini yükler
   */
  loadSiteComponents(siteId: number): void {
    console.log(
      '🌐 UnifiedViewer: Site componentleri için API isteği başlatılıyor (siteId:',
      siteId,
      ')'
    );
    this.componentService.getComponentsForSite(siteId).subscribe({
      next: (components) => {
        console.log(
          `✅ UnifiedViewer: ${components.length} adet component yüklendi (siteId: ${siteId})`
        );

        // API'den gelen componentleri işleyip site içerik verilerini hazırla
        if (components && components.length > 0) {
          console.log(
            '📋 UnifiedViewer: Component listesi:',
            components.map((c) => ({
              id: c.id,
              name: c.componentName,
              themeComponentId: c.themeComponentId,
            }))
          );
          this.processSiteComponents(components);
        } else {
          console.warn(
            '⚠️ UnifiedViewer: Site için component bulunamadı (siteId:',
            siteId,
            ')'
          );
        }
      },
      error: (error) => {
        console.error(
          '❌ UnifiedViewer: Site componentleri yüklenirken hata:',
          error
        );
      },
    });
  }

  /**
   * API'den gelen component verilerini işler
   */
  processSiteComponents(components: any[]): void {
    console.log(
      '🔄 UnifiedViewer: Site componentleri işleniyor (' +
        components.length +
        ' adet)'
    );

    // Component verilerini işleyerek uygulamanın kullanabileceği format oluştur
    // Bu örnek için basit bir yapı kuruyoruz, gerçekte daha karmaşık olabilir
    try {
      const navigationComp = components.find(
        (c) =>
          c.componentName?.toLowerCase().includes('nav') ||
          c.componentId?.toLowerCase().includes('nav')
      );

      const contentComp = components.find(
        (c) =>
          c.componentName?.toLowerCase().includes('content') ||
          c.componentId?.toLowerCase().includes('content')
      );

      const footerComp = components.find(
        (c) =>
          c.componentName?.toLowerCase().includes('footer') ||
          c.componentId?.toLowerCase().includes('footer')
      );

      console.log('🔍 UnifiedViewer: Component tipleri tespit edildi:', {
        navbar: navigationComp
          ? `ID: ${navigationComp.id}, themeComponentId: ${navigationComp.themeComponentId}`
          : 'bulunamadı',
        content: contentComp
          ? `ID: ${contentComp.id}, themeComponentId: ${contentComp.themeComponentId}`
          : 'bulunamadı',
        footer: footerComp
          ? `ID: ${footerComp.id}, themeComponentId: ${footerComp.themeComponentId}`
          : 'bulunamadı',
      });

      const processedData: any = {};

      // Navigasyon komponenti - menü bağlantıları için kullanılır
      if (navigationComp && navigationComp.data) {
        console.log('🧩 UnifiedViewer: Navbar component verisi işleniyor');
        try {
          const navData = JSON.parse(navigationComp.data);
          console.log('📊 UnifiedViewer: Navbar verisi:', navData);

          if (navData.menuItems) {
            processedData.links = navData.menuItems;
            console.log(
              '✅ UnifiedViewer: Navbar menü öğeleri eklendi:',
              navData.menuItems
            );
          }
          if (navData.sideMenuItems) {
            processedData.menuItems = navData.sideMenuItems;
            console.log(
              '✅ UnifiedViewer: Yan menü öğeleri eklendi:',
              navData.sideMenuItems
            );
          }
        } catch (error) {
          console.error(
            '❌ UnifiedViewer: Navbar verisi JSON olarak parse edilemedi:',
            error
          );
        }
      } else {
        console.warn(
          '⚠️ UnifiedViewer: Navbar component bulunamadı veya verisi boş'
        );
      }

      // İçerik komponenti - sayfa içeriği için kullanılır
      if (contentComp && contentComp.data) {
        console.log('🧩 UnifiedViewer: İçerik component verisi işleniyor');
        try {
          const contentData = JSON.parse(contentComp.data);
          console.log('📊 UnifiedViewer: İçerik verisi:', contentData);

          processedData.contentTitle =
            contentData.title || contentData.contentTitle;
          processedData.contentText =
            contentData.text || contentData.contentText;

          if (contentData.highlight) {
            processedData.highlightTitle = contentData.highlight.title;
            processedData.highlightText = contentData.highlight.text;
          }
          console.log('✅ UnifiedViewer: İçerik verisi eklendi');
        } catch (error) {
          console.error(
            '❌ UnifiedViewer: İçerik verisi JSON olarak parse edilemedi:',
            error
          );
        }
      } else {
        console.warn(
          '⚠️ UnifiedViewer: İçerik component bulunamadı veya verisi boş'
        );
      }

      // Footer komponenti - altbilgi için kullanılır
      if (footerComp && footerComp.data) {
        console.log('🧩 UnifiedViewer: Footer component verisi işleniyor');
        try {
          const footerData = JSON.parse(footerComp.data);
          console.log('📊 UnifiedViewer: Footer verisi:', footerData);

          if (footerData.links) {
            processedData.footerLinks = footerData.links;
            console.log('✅ UnifiedViewer: Footer bağlantıları eklendi');
          }
          if (footerData.socialMedia) {
            processedData.socialMedia = Object.entries(
              footerData.socialMedia
            ).map(([platform, url]) => ({ platform, url: url as string }));
            console.log('✅ UnifiedViewer: Sosyal medya bağlantıları eklendi');
          }
        } catch (error) {
          console.error(
            '❌ UnifiedViewer: Footer verisi JSON olarak parse edilemedi:',
            error
          );
        }
      } else {
        console.warn(
          '⚠️ UnifiedViewer: Footer component bulunamadı veya verisi boş'
        );
      }

      console.log(
        '✅ UnifiedViewer: Tüm componentler başarıyla işlendi:',
        processedData
      );
      this.siteComponents = processedData;
    } catch (error) {
      console.error(
        '❌ UnifiedViewer: Component verilerini işlerken hata:',
        error
      );
    }
  }

  getHeaderStyles() {
    if (!this.currentTheme) return {};

    return {
      'background-color': this.currentTheme.primaryColor || '#00733E',
    };
  }

  getMainNavigationStyles() {
    if (!this.currentTheme) return {};

    return {
      'background-color': this.currentTheme.primaryColor || '#00733E',
    };
  }

  getSidebarHeaderStyles() {
    if (!this.currentTheme) return {};

    return {
      color: this.currentTheme.primaryColor || '#00733E',
      'border-bottom-color': this.currentTheme.secondaryColor || '#E0B200',
    };
  }

  getContentHeaderStyles() {
    if (!this.currentTheme) return {};

    return {
      color: this.currentTheme.primaryColor || '#00733E',
      'border-bottom-color': this.currentTheme.secondaryColor || '#E0B200',
    };
  }

  getSubdomainHighlightStyle() {
    if (!this.currentTheme) return {};

    const secondaryColor = this.currentTheme.secondaryColor || '#E0B200';
    return {
      'background-color': `${secondaryColor}15`,
      'border-left-color': secondaryColor,
    };
  }

  getFooterStyles() {
    if (!this.currentTheme) return {};

    const primaryColor = this.currentTheme.primaryColor || '#00733E';
    return {
      'background-color': this.darkenColor(primaryColor, 30),
    };
  }

  getSiteTypeForDisplay(): string {
    if (!this.currentSite) return 'Menü';

    // Domain veya template bilgisine göre site türünü belirleme
    const domain = this.currentSite.domain || '';
    const template = this.currentTemplate?.name || '';

    if (
      domain.includes('fakulte') ||
      domain.includes('faculty') ||
      template.includes('Fakülte')
    ) {
      return 'Fakülte Menüsü';
    } else if (
      domain.includes('enstitu') ||
      domain.includes('institute') ||
      template.includes('Enstitü')
    ) {
      return 'Enstitü Menüsü';
    } else if (
      domain.includes('merkez') ||
      domain.includes('center') ||
      template.includes('Merkez')
    ) {
      return 'Merkez Menüsü';
    } else if (
      domain.includes('magaza') ||
      domain.includes('shop') ||
      template.includes('Mağaza')
    ) {
      return 'Mağaza Menüsü';
    } else if (domain.includes('blog') || template.includes('Blog')) {
      return 'Blog Menüsü';
    } else {
      return 'Site Menüsü';
    }
  }

  getCurrentYear(): number {
    return new Date().getFullYear();
  }

  getDefaultMenuItems(): { text: string; url: string }[] {
    // Varsayılan menü öğeleri - API veri döndürmediğinde kullanılır
    return [
      { text: 'Ana Sayfa', url: '/' },
      { text: 'Hakkımızda', url: '/hakkimizda' },
      { text: 'İletişim', url: '/iletisim' },
    ];
  }

  private darkenColor(hex: string, percent: number): string {
    try {
      // Renk koyulaştırma fonksiyonu
      let r = parseInt(hex.substring(1, 3), 16);
      let g = parseInt(hex.substring(3, 5), 16);
      let b = parseInt(hex.substring(5, 7), 16);

      r = Math.round((r * (100 - percent)) / 100);
      g = Math.round((g * (100 - percent)) / 100);
      b = Math.round((b * (100 - percent)) / 100);

      r = Math.max(0, Math.min(255, r));
      g = Math.max(0, Math.min(255, g));
      b = Math.max(0, Math.min(255, b));

      return '#' + ((1 << 24) + (r << 16) + (g << 8) + b).toString(16).slice(1);
    } catch (error) {
      return '#333333'; // Renk işlenemezse varsayılan koyu gri
    }
  }

  isLoading(componentId: string): boolean {
    return this.componentStates.get(componentId) === 'loading';
  }

  isLoaded(componentId: string): boolean {
    return this.componentStates.get(componentId) === 'loaded';
  }

  hasError(componentId: string): boolean {
    return this.componentStates.get(componentId) === 'error';
  }

  getErrorMessage(componentId: string): string | null {
    return this.componentErrors.get(componentId) || null;
  }

  refreshComponent(componentId: string): void {
    console.log('🔄 UnifiedViewer: Component yenileniyor:', componentId);

    // Component durumunu yükleniyor olarak güncelle
    this.componentStates.set(componentId, 'loading');
    this.componentErrors.delete(componentId);

    // Component'i yeniden yükle
    this.pageService.getComponentById(componentId).subscribe({
      next: (component) => {
        if (component) {
          console.log('✅ UnifiedViewer: Component yenilendi:', componentId);
          this.componentStates.set(componentId, 'loaded');
        } else {
          console.error('❌ UnifiedViewer: Component bulunamadı:', componentId);
          this.componentStates.set(componentId, 'error');
          this.componentErrors.set(componentId, 'Component bulunamadı');
        }
      },
      error: (error) => {
        console.error(
          '❌ UnifiedViewer: Component yenilenirken hata:',
          componentId,
          error
        );
        this.componentStates.set(componentId, 'error');
        this.componentErrors.set(
          componentId,
          error.message || 'Bilinmeyen hata'
        );
      },
    });
  }

  // Component listesi değiştiğinde durumları izle
  private trackComponentStates(): void {
    if (this.currentPage && this.currentPage.components) {
      console.log(
        '🔄 UnifiedViewer: Component durumları izleniyor',
        this.currentPage.components
      );

      // Tüm componentleri yükleniyor olarak işaretle
      this.currentPage.components.forEach((componentId: string) => {
        if (!this.componentStates.has(componentId)) {
          console.log(
            '🔍 UnifiedViewer: Component durumu başlatılıyor:',
            componentId
          );
          this.componentStates.set(componentId, 'loading');

          // Component durumunu izle
          this.refreshComponent(componentId);
        }
      });
    }
  }

  /**
   * Component tipini belirler ve gösterir
   */
  getComponentTypeName(componentId: string): string | null {
    // Eğer önceden belirlenmiş bir tip varsa onu kullan
    if (this.componentTypes.has(componentId)) {
      return this.componentTypes.get(componentId) || null;
    }

    // Sayısal ID'lere göre bazı özel component tipleri belirle
    const numericId = parseInt(componentId);
    if (!isNaN(numericId)) {
      let typeName = '';

      // Bilinen component ID'lerine göre tip belirle
      switch (numericId) {
        case 1:
          typeName = 'Navbar';
          break;
        case 2:
          typeName = 'Header';
          break;
        case 3:
          typeName = 'Footer';
          break;
        case 4:
          typeName = 'Slider';
          break;
        case 5:
          typeName = 'Content';
          break;
        default:
          // ID'ye göre bir tahmin yap
          if (numericId >= 1 && numericId <= 10) {
            typeName = 'Ana Bileşen';
          } else if (numericId >= 11 && numericId <= 20) {
            typeName = 'Layout';
          } else if (numericId >= 21 && numericId <= 30) {
            typeName = 'Form';
          } else {
            typeName = 'Özel Bileşen';
          }
      }

      // Belirlenen tipi sakla
      this.componentTypes.set(componentId, typeName);
      return typeName;
    }

    return null;
  }

  getComponentTypeClass(componentId: string): string {
    const typeName = this.getComponentTypeName(componentId);
    if (!typeName) return '';

    // Tip adını küçük harfe çevir ve boşlukları kaldır (CSS sınıfı için)
    return typeName.toLowerCase().replace(/\s+/g, '-');
  }
}
