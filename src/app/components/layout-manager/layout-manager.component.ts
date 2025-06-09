import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TemplateService } from '../../services/template.service';
import { ThemeService } from '../../services/theme.service';
import { Subscription } from 'rxjs';
import { Site, Template, TemplateTheme } from '../../models/template.model';
import { Theme } from '../../models/theme.model';

@Component({
  selector: 'app-layout-manager',
  standalone: true,
  imports: [CommonModule],
  template: `
    <ng-container *ngIf="currentSite && currentTemplate && currentTheme">
      <!-- Dinamik olarak ilgili layout bileşenini yükle -->
      <div [ngSwitch]="currentTemplate.layout">
        <ng-container *ngSwitchCase="'kurum-layout'">
          <!-- Kurum Layout -->
          <div class="kurum-layout">
            <header [innerHTML]="currentTheme.header"></header>
            <main>
              <h1>{{ currentSite.name }}</h1>
              <p>{{ currentSite.description }}</p>
            </main>
            <footer [innerHTML]="currentTheme.footer"></footer>
          </div>
        </ng-container>
        
        <ng-container *ngSwitchCase="'fakulte-layout'">
          <!-- Fakülte Layout -->
          <div class="fakulte-layout">
            <header [innerHTML]="currentTheme.header"></header>
            <div class="content-with-sidebar">
              <aside>
                <h3>Fakülte Menüsü</h3>
                <ul>
                  <li><a href="#">Akademik Kadro</a></li>
                  <li><a href="#">Bölümler</a></li>
                  <li><a href="#">Etkinlikler</a></li>
                </ul>
              </aside>
              <main>
                <h1>{{ currentSite.name }}</h1>
                <p>{{ currentSite.description }}</p>
              </main>
            </div>
            <footer [innerHTML]="currentTheme.footer"></footer>
          </div>
        </ng-container>
        
        <ng-container *ngSwitchCase="'kulup-layout'">
          <!-- Kulüp Layout -->
          <div class="kulup-layout">
            <header [innerHTML]="currentTheme.header"></header>
            <div class="banner">
              <h1>{{ currentSite.name }}</h1>
              <p>{{ currentSite.description }}</p>
            </div>
            <main>
              <div class="content">
                <!-- İçerik buraya gelecek -->
              </div>
            </main>
            <footer [innerHTML]="currentTheme.footer"></footer>
          </div>
        </ng-container>
        
        <ng-container *ngSwitchDefault>
          <!-- Varsayılan düzen -->
          <div class="default-layout">
            <header>{{ currentSite.name }}</header>
            <main>{{ currentSite.description }}</main>
            <footer>© 2024</footer>
          </div>
        </ng-container>
      </div>
    </ng-container>
  `,
  styles: [`
    /* Genel stiller */
    header {
      padding: 20px;
      background-color: #00733E;
      color: white;
    }
    
    footer {
      padding: 20px;
      background-color: #333;
      color: white;
      text-align: center;
    }
    
    main {
      padding: 20px;
    }
    
    /* Kurum Layout Stilleri */
    .kurum-layout {
      display: flex;
      flex-direction: column;
      min-height: 100vh;
    }
    
    /* Fakülte Layout Stilleri */
    .fakulte-layout {
      display: flex;
      flex-direction: column;
      min-height: 100vh;
    }
    
    .content-with-sidebar {
      display: flex;
      flex: 1;
    }
    
    .content-with-sidebar aside {
      width: 250px;
      padding: 20px;
      background-color: #f5f5f5;
      border-right: 1px solid #ddd;
    }
    
    .content-with-sidebar main {
      flex: 1;
    }
    
    /* Kulüp Layout Stilleri */
    .kulup-layout {
      display: flex;
      flex-direction: column;
      min-height: 100vh;
    }
    
    .kulup-layout .banner {
      padding: 40px 20px;
      background-color: #800080;
      color: white;
      text-align: center;
    }
  `]
})
export class LayoutManagerComponent implements OnInit, OnDestroy {
  currentSite: Site | null = null;
  currentTemplate: Template | null = null;
  currentTemplateTheme: TemplateTheme | null = null;
  currentTheme: Theme | null = null;
  
  private subscription: Subscription = new Subscription();

  constructor(
    private templateService: TemplateService,
    private themeService: ThemeService
  ) {}

  ngOnInit(): void {
    // Site değişimini takip et
    const siteSubscription = this.templateService.currentSite$.subscribe(
      site => {
        if (site) {
          this.currentSite = site;
          this.loadLayoutData(site);
        }
      }
    );
    
    this.subscription.add(siteSubscription);
    
    // Başlangıçta varsayılan site yükle
    this.templateService.loadSiteBySubdomain('www');
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  private loadLayoutData(site: Site): void {
    // Site için template-tema bilgisini yükle
    const templateTheme = this.templateService.getTemplateThemeById(
      site.templateThemeId
    );
    
    if (templateTheme) {
      this.currentTemplateTheme = templateTheme;
      
      // Template bilgisini yükle
      const template = this.templateService.getTemplateById(
        templateTheme.templateId
      );
      
      if (template) {
        this.currentTemplate = template;
      }
      
      // Tema bilgisini yükle
      const theme = this.themeService.getThemeById(templateTheme.themeId);
      
      if (theme) {
        this.currentTheme = theme;
      }
    }
  }
} 