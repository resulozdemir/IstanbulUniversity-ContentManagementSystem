import {
  Component,
  OnInit,
  OnDestroy,
  ElementRef,
  ViewChild,
  Renderer2,
  AfterViewInit,
  ViewContainerRef,
  ComponentRef,
  ComponentFactoryResolver,
  Injector,
  ApplicationRef,
  createComponent,
  EnvironmentInjector,
  ChangeDetectorRef,
  NgZone,
  TemplateRef,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  PageService,
  Site,
  Page,
  PageComponent,
  ComponentData,
} from '../page.service';
import { Subject, takeUntil, BehaviorSubject } from 'rxjs';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { ComponentViewerComponent } from '../../components/component-viewer/component-viewer.component';

// Angular bileşeni context'i için interface
interface PageScriptContext {
  [key: string]: any;
  $changeDetector?: ChangeDetectorRef;
  $onInit?: () => void;
  $onDestroy?: () => void;
}

@Component({
  selector: 'app-page-viewer',
  standalone: true,
  imports: [CommonModule, FormsModule, ComponentViewerComponent],
  template: `
    <div class="page-viewer-container">
      <div class="site-page-selector">
        <div class="selector-group">
          <label for="siteSelector">Site:</label>
          <select
            id="siteSelector"
            [(ngModel)]="selectedSiteId"
            (change)="onSiteChange()"
            class="selector form-select"
          >
            <option *ngFor="let site of sites" [ngValue]="site.id">
              {{ site.name }}
            </option>
          </select>
        </div>

        <div class="selector-group">
          <label for="pageSelector">Page:</label>
          <select
            id="pageSelector"
            [(ngModel)]="selectedPageId"
            (change)="onPageChange()"
            class="selector form-select"
          >
            <option *ngFor="let page of pages" [ngValue]="page.id">
              {{ page.name }}
            </option>
          </select>
        </div>

        <div class="selector-group">
          <button
            (click)="toggleDebugMode()"
            class="debug-button btn btn-primary"
          >
            {{ showDebugPanel ? 'Debugı Kapat' : 'Debug Göster' }}
          </button>
        </div>
      </div>

      <div *ngIf="currentPage" class="page-content">
        <!-- Page content container -->
        <div #pageContentContainer [innerHTML]="safePageHtml"></div>
      </div>

      <div *ngIf="!currentPage" class="no-page-message">
        <p>No page selected or available. Please select a site and page.</p>
      </div>

      <!-- Debug Panel / Kod İnceleme Paneli -->
      <div *ngIf="showDebugPanel" class="debug-panel">
        <h2>Kod İnceleme Paneli</h2>

        <div class="panel-section">
          <h3>Sayfa Bilgileri</h3>
          <div class="info-grid">
            <div>Sayfa ID:</div>
            <div>{{ currentPage?.id }}</div>
            <div>Sayfa Adı:</div>
            <div>{{ currentPage?.name }}</div>
            <div>Site ID:</div>
            <div>{{ currentPage?.siteId }}</div>
            <div>Route:</div>
            <div>{{ currentPage?.routing }}</div>
          </div>
        </div>

        <div class="panel-section">
          <h3>Sayfa HTML</h3>
          <div class="code-block">
            <pre>{{ currentPage?.html }}</pre>
          </div>
        </div>

        <div class="panel-section">
          <h3>Sayfa CSS</h3>
          <div class="code-block">
            <pre>{{ currentPage?.style }}</pre>
          </div>
        </div>

        <div class="panel-section">
          <h3>Sayfa JavaScript</h3>
          <div class="code-block">
            <pre>{{ currentPage?.javascript }}</pre>
          </div>
        </div>

        <div *ngIf="extractedComponents.length > 0" class="panel-section">
          <h3>Bulunan Bileşenler ({{ extractedComponents.length }})</h3>
          <div
            *ngFor="let comp of extractedComponents; let i = index"
            class="component-debug"
          >
            <h4>Bileşen #{{ i + 1 }} (ID: {{ comp.componentId }})</h4>
            <div
              class="component-data"
              *ngIf="loadedComponentData[comp.componentId.toString()]"
            >
              <div class="tabs">
                <button
                  (click)="activeTab[comp.componentId] = 'info'"
                  [class.active]="activeTab[comp.componentId] === 'info'"
                >
                  Bilgi
                </button>
                <button
                  (click)="activeTab[comp.componentId] = 'html'"
                  [class.active]="activeTab[comp.componentId] === 'html'"
                >
                  HTML
                </button>
                <button
                  (click)="activeTab[comp.componentId] = 'css'"
                  [class.active]="activeTab[comp.componentId] === 'css'"
                >
                  CSS
                </button>
                <button
                  (click)="activeTab[comp.componentId] = 'js'"
                  [class.active]="activeTab[comp.componentId] === 'js'"
                >
                  JS
                </button>
                <button
                  (click)="activeTab[comp.componentId] = 'data'"
                  [class.active]="activeTab[comp.componentId] === 'data'"
                >
                  Veri
                </button>
              </div>

              <div class="tab-content">
                <div
                  *ngIf="activeTab[comp.componentId] === 'info'"
                  class="info-tab"
                >
                  <div class="info-grid">
                    <div>Bileşen Adı:</div>
                    <div>
                      {{
                        loadedComponentData[comp.componentId.toString()].name
                      }}
                    </div>
                    <div>Açıklama:</div>
                    <div>
                      {{
                        loadedComponentData[comp.componentId.toString()]
                          .description
                      }}
                    </div>
                    <div>Tema ID:</div>
                    <div>
                      {{
                        loadedComponentData[comp.componentId.toString()].themeId
                      }}
                    </div>
                  </div>
                </div>

                <div
                  *ngIf="activeTab[comp.componentId] === 'html'"
                  class="code-tab"
                >
                  <pre>{{
                    loadedComponentData[comp.componentId.toString()].template
                  }}</pre>
                </div>

                <div
                  *ngIf="activeTab[comp.componentId] === 'css'"
                  class="code-tab"
                >
                  <pre>{{
                    loadedComponentData[comp.componentId.toString()].style
                  }}</pre>
                </div>

                <div
                  *ngIf="activeTab[comp.componentId] === 'js'"
                  class="code-tab"
                >
                  <pre>{{
                    loadedComponentData[comp.componentId.toString()].javascript
                  }}</pre>
                </div>

                <div
                  *ngIf="activeTab[comp.componentId] === 'data'"
                  class="code-tab"
                >
                  <pre>{{
                    getComponentDataAsString(comp.componentId.toString())
                  }}</pre>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [
    `
      .page-viewer-container {
        display: flex;
        flex-direction: column;
        width: 100%;
        height: 100%;
      }

      .site-page-selector {
        display: flex;
        gap: 20px;
        padding: 15px;
        background-color: #f5f5f5;
        border-bottom: 1px solid #ddd;
      }

      .selector-group {
        display: flex;
        align-items: center;
        gap: 8px;
      }

      .selector {
        padding: 8px 12px;
        border-radius: 4px;
        border: 1px solid #ccc;
        font-size: 14px;
        min-width: 200px;
      }

      .debug-button {
        padding: 8px 16px;
        background-color: #6200ee;
        color: white;
        border: none;
        border-radius: 4px;
        cursor: pointer;
        font-weight: bold;
      }

      .debug-button:hover {
        background-color: #3700b3;
      }

      .page-content {
        flex: 1;
        padding: 20px;
        overflow-y: auto;
      }

      .no-page-message {
        display: flex;
        justify-content: center;
        align-items: center;
        height: 100%;
        color: #666;
        font-style: italic;
      }

      .components-container {
        margin-top: 20px;
      }

      .component-wrapper {
        margin-bottom: 20px;
        border: 1px dashed #ccc;
        padding: 10px;
        border-radius: 4px;
      }

      /* Debug Panel Styles */
      .debug-panel {
        margin-top: 30px;
        padding: 20px;
        background-color: #f8f9fa;
        border: 1px solid #e9ecef;
        border-radius: 8px;
      }

      .debug-panel h2 {
        margin-top: 0;
        color: #343a40;
        border-bottom: 2px solid #dee2e6;
        padding-bottom: 10px;
      }

      .panel-section {
        margin-bottom: 20px;
      }

      .panel-section h3 {
        margin-top: 0;
        color: #495057;
        font-size: 1.2rem;
      }

      .code-block {
        background-color: #272822;
        color: #f8f8f2;
        padding: 15px;
        border-radius: 5px;
        overflow-x: auto;
        max-height: 300px;
        overflow-y: auto;
      }

      .code-block pre {
        margin: 0;
        font-family: 'Courier New', Courier, monospace;
        white-space: pre-wrap;
      }

      .component-debug {
        background-color: #fff;
        border: 1px solid #dee2e6;
        border-radius: 5px;
        margin-bottom: 15px;
        overflow: hidden;
      }

      .component-debug h4 {
        margin: 0;
        padding: 10px 15px;
        background-color: #e9ecef;
        color: #495057;
      }

      .component-data {
        padding: 15px;
      }

      .info-grid {
        display: grid;
        grid-template-columns: 120px 1fr;
        gap: 8px;
      }

      .info-grid > div:nth-child(odd) {
        font-weight: bold;
      }

      /* Tabs */
      .tabs {
        display: flex;
        border-bottom: 1px solid #dee2e6;
        margin-bottom: 15px;
      }

      .tabs button {
        padding: 8px 16px;
        border: none;
        background: none;
        cursor: pointer;
        font-weight: 500;
        color: #6c757d;
        position: relative;
      }

      .tabs button.active {
        color: #495057;
        font-weight: bold;
      }

      .tabs button.active::after {
        content: '';
        position: absolute;
        bottom: -1px;
        left: 0;
        width: 100%;
        height: 2px;
        background-color: #6200ee;
      }

      .tab-content {
        padding: 10px;
      }

      .code-tab {
        background-color: #272822;
        color: #f8f8f2;
        padding: 15px;
        border-radius: 5px;
        overflow-x: auto;
        max-height: 300px;
        overflow-y: auto;
      }

      .code-tab pre {
        margin: 0;
        font-family: 'Courier New', Courier, monospace;
        white-space: pre-wrap;
      }
    `,
  ],
})
export class PageViewerComponent implements OnInit, OnDestroy, AfterViewInit {
  @ViewChild('pageContentContainer') pageContentContainer!: ElementRef;

  // Bootstrap CSS CDN link
  private bootstrapCssLink =
    'https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css';
  // Bootstrap JS CDN link
  private bootstrapJsLink =
    'https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js';

  sites: Site[] = [];
  pages: Page[] = [];
  selectedSiteId: number | null = null;
  selectedPageId: number | null = null;
  currentPage: Page | null = null;
  pageHtml: string = '';
  safePageHtml: SafeHtml = '';
  extractedComponents: PageComponent[] = [];

  // Debug panel değişkenleri
  showDebugPanel: boolean = false;
  activeTab: { [key: string]: string } = {};
  loadedComponentData: { [key: string]: ComponentData } = {};

  // JavaScript context'lerini tutacak map
  private pageScriptContexts: Map<number, PageScriptContext> = new Map();

  private destroy$ = new Subject<void>();

  constructor(
    private pageService: PageService,
    private sanitizer: DomSanitizer,
    private renderer: Renderer2,
    private viewContainerRef: ViewContainerRef,
    private applicationRef: ApplicationRef,
    private injector: Injector,
    private environmentInjector: EnvironmentInjector,
    private ngZone: NgZone,
    private changeDetectorRef: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    // Subscribe to sites
    this.pageService.sites$
      .pipe(takeUntil(this.destroy$))
      .subscribe((sites) => {
        this.sites = sites;
        console.log('Yüklenen siteler:', sites);
      });

    // Subscribe to pages
    this.pageService.pages$
      .pipe(takeUntil(this.destroy$))
      .subscribe((pages) => {
        this.pages = pages;
        console.log('Yüklenen sayfalar:', pages);
      });

    // Subscribe to current page
    this.pageService.currentPage$
      .pipe(takeUntil(this.destroy$))
      .subscribe((page) => {
        this.currentPage = page;
        console.log('Seçilen sayfa:', page);

        if (page) {
          this.selectedSiteId = page.siteId;
          this.selectedPageId = page.id;
          this.processPageHtml(page);
        }
      });

    // Load all sites on component initialization
    this.pageService.loadSites();
  }

  ngAfterViewInit(): void {
    console.log('PageViewerComponent.ngAfterViewInit()');
    // Add Bootstrap CSS
    this.loadBootstrapCSS();

    // Sayfa içeriği değiştiğinde DOM'u güncellemek için
    if (this.currentPage) {
      this.processPageContent();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();

    // Clean up any script elements and page contexts
    this.cleanupPageScripts();
  }

  /**
   * Clean up script elements and page contexts
   */
  private cleanupPageScripts(): void {
    // Clean up all page script contexts
    this.pageScriptContexts.forEach((context, pageId) => {
      if (context.$onDestroy) {
        try {
          context.$onDestroy();
        } catch (e) {
          console.error(`Error calling onDestroy for page ${pageId}:`, e);
        }
      }

      // Remove script element
      const scriptElement = document.getElementById(`page-script-${pageId}`);
      if (scriptElement) {
        scriptElement.remove();
      }
    });

    // Clear the contexts map
    this.pageScriptContexts.clear();
  }

  toggleDebugMode(): void {
    this.showDebugPanel = !this.showDebugPanel;

    // Debug paneli açıldığında bileşen verilerini yükle
    if (this.showDebugPanel && this.extractedComponents.length > 0) {
      this.loadComponentsData();
    }
  }

  loadComponentsData(): void {
    this.extractedComponents.forEach((comp) => {
      const componentId = comp.componentId.toString();

      // Tab durumunu başlat
      if (!this.activeTab[componentId]) {
        this.activeTab[componentId] = 'info';
      }

      // Bileşen verilerini yükle
      if (!this.loadedComponentData[componentId]) {
        this.pageService.getComponentById(componentId).subscribe((data) => {
          this.loadedComponentData[componentId] = data;
          console.log(`Bileşen ${componentId} debug için yüklendi:`, data);
        });
      }
    });
  }

  getComponentDataAsString(componentId: string): string {
    const componentData = this.loadedComponentData[componentId];
    if (!componentData || !componentData.data) {
      return 'Veri yok';
    }

    try {
      return JSON.stringify(componentData.data, null, 2);
    } catch (e) {
      return String(componentData.data);
    }
  }

  onSiteChange(): void {
    if (this.selectedSiteId) {
      console.log('Site değiştirildi, ID:', this.selectedSiteId);
      const selectedSite = this.sites.find(
        (site) => site.id === this.selectedSiteId
      );
      if (selectedSite) {
        this.pageService.selectSite(selectedSite);
      }
    }
  }

  onPageChange(): void {
    if (this.selectedPageId) {
      console.log('Sayfa değiştirildi, ID:', this.selectedPageId);

      // Mevcut seçili sayfa ID'si geçerli bir sayı mı kontrol et
      const pageId = Number(this.selectedPageId);
      if (isNaN(pageId)) {
        console.error('Geçersiz sayfa ID formatı:', this.selectedPageId);
        return;
      }

      // Sayfa listesinde bu ID'ye sahip bir sayfa var mı kontrol et
      const selectedPage = this.pages.find((page) => page.id === pageId);
      if (!selectedPage) {
        console.warn("Seçilen ID'ye sahip sayfa bulunamadı:", pageId);
        console.log('Mevcut sayfa listesi:', this.pages);
      }

      this.pageService.selectPage(pageId);
    } else {
      console.warn(
        'Sayfa seçilmedi, selectedPageId değeri:',
        this.selectedPageId
      );
    }
  }

  private processPageHtml(page: Page): void {
    if (!page.html) {
      console.warn('Sayfanın HTML içeriği yok:', page);
      return;
    }

    // İlk önce HTML içeriğini doğrudan atayalım
    this.pageHtml = page.html;

    // Extract components to ensure they can be loaded
    this.extractedComponents = this.pageService.extractComponents(page.html);
    console.log('Sayfadaki bileşenler:', this.extractedComponents);

    // Apply page styles if available
    if (page.style) {
      this.applyPageStyles(page.style, page.id);
    }

    // Sayfa içeriğini işleyelim
    this.processPageContent();

    // Debug paneli açıksa bileşen verilerini yükle
    if (this.showDebugPanel && this.extractedComponents.length > 0) {
      this.loadComponentsData();
    }
  }

  private processPageContent(): void {
    // Component-viewer etiketlerini komponentlerimiz ile değiştireceğiz
    // Bu işlem için DOM'u düzgün şekilde manipüle etmeliyiz

    // Sayfa HTML'ini güvenli olarak sanitize et
    this.safePageHtml = this.sanitizer.bypassSecurityTrustHtml(this.pageHtml);

    // Bileşenleri sayfaya eklemek için setTimeout kullanacağız (AfterViewInit sonrası)
    setTimeout(() => {
      // Sayfa yüklendikten sonra, component-viewer'ları işleyelim
      this.processComponentViewers();

      // Önce JavaScript'i uygulayalım ki direktifler için context oluşsun
      if (
        this.currentPage?.javascript &&
        this.currentPage.javascript !== '//'
      ) {
        this.applyPageJavaScript(
          this.currentPage.javascript,
          this.currentPage.id
        );
      }

      // Sonra Angular direktiflerini ve veri bağlamalarını işleyelim
      if (this.currentPage) {
        this.processAngularDirectives();
      }

      // En son HTML içindeki Angular olay işleyicilerini işle
      if (this.currentPage) {
        this.processEventHandlers();
      }
    }, 100); // Bileşenlerin yüklenmesi için biraz daha uzun bir süre verelim
  }

  /**
   * HTML içindeki Angular olay işleyicilerini işleyen metot
   */
  private processEventHandlers(): void {
    if (!this.pageContentContainer || !this.currentPage) return;

    const container = this.pageContentContainer.nativeElement;
    const context =
      this.currentPage && this.pageScriptContexts.has(this.currentPage.id)
        ? this.pageScriptContexts.get(this.currentPage.id)
        : null;

    if (!context) {
      console.warn('No page context found for event handlers processing');
      return;
    }

    // Tüm elementi ve alt elementleri işle
    this.processEventHandlersForElement(container, context);

    // Değişikliklerden sonra UI'ı güncellemek için change detector'ı tetikle
    if (context.$changeDetector) {
      context.$changeDetector.detectChanges();
      console.log('Change detection triggered after event handlers processing');
    }
  }

  private processComponentViewers(): void {
    if (!this.pageContentContainer) return;

    const container = this.pageContentContainer.nativeElement;

    // Tüm component-viewer etiketlerini bulalım
    const componentViewers = container.querySelectorAll('app-component-viewer');
    console.log(
      'Found component viewers in rendered HTML:',
      componentViewers.length
    );

    componentViewers.forEach((viewer: Element) => {
      // Component ID'sini al
      let componentId = '';

      // Angular attribute syntax: [componentId]="1"
      const idWithBrackets = viewer.getAttribute('[componentId]');
      if (idWithBrackets) {
        componentId = idWithBrackets.replace(/['"]/g, '').trim();
      }
      // Regular HTML attribute: componentId="1"
      else {
        const regularId = viewer.getAttribute('componentId');
        if (regularId) {
          componentId = regularId;
        }
      }

      console.log('Processing component viewer with ID:', componentId);

      if (componentId) {
        // Component-viewer için bir div oluştur
        const componentViewerHost = document.createElement('div');
        componentViewerHost.className = 'component-viewer-host';
        componentViewerHost.setAttribute('data-component-id', componentId);

        // Orijinal component-viewer'ı bu divle değiştir
        viewer.parentNode?.replaceChild(componentViewerHost, viewer);

        // Dinamik olarak component-viewer bileşeni oluştur
        this.createComponentViewer(componentViewerHost, componentId);
      }
    });
  }

  private createComponentViewer(
    hostElement: HTMLElement,
    componentId: string
  ): void {
    // Angular 14+ yöntemini kullanarak dinamik component oluşturma
    const componentRef = createComponent(ComponentViewerComponent, {
      environmentInjector: this.environmentInjector,
      hostElement: hostElement,
    });

    // Input değerlerini ayarla
    componentRef.setInput('componentId', componentId);

    // Değişiklikleri uygula
    componentRef.changeDetectorRef.detectChanges();
  }

  private applyPageStyles(styles: string, pageId: number): void {
    // Remove any existing style for this page
    const existingStyle = document.getElementById(`page-style-${pageId}`);
    if (existingStyle) {
      existingStyle.remove();
    }

    // Create and append the new style element
    const styleElement = document.createElement('style');
    styleElement.id = `page-style-${pageId}`;
    styleElement.textContent = styles;
    document.head.appendChild(styleElement);
  }

  private applyPageJavaScript(javascript: string, pageId: number): void {
    // Remove any existing script for this page
    const existingScript = document.getElementById(`page-script-${pageId}`);
    if (existingScript) {
      existingScript.remove();
    }

    // Destroy previous context if exists
    if (this.pageScriptContexts.has(pageId)) {
      const context = this.pageScriptContexts.get(pageId);
      if (context && context.$onDestroy) {
        try {
          context.$onDestroy();
        } catch (e) {
          console.error(`Error calling onDestroy for page ${pageId}:`, e);
        }
      }
      this.pageScriptContexts.delete(pageId);
    }

    if (!javascript || !javascript.trim()) {
      console.log(`No JavaScript to apply for page ${pageId}`);
      return;
    }

    try {
      console.log(
        `Applying JavaScript for page ${pageId}:`,
        javascript.substring(0, 100) + '...'
      );

      // Parse the JavaScript code
      const pageScriptDef = this.parsePageScript(javascript);

      // Create a change detector for this script
      const cd = new BehaviorSubject<void>(undefined);

      // Create script context with lifecycle hooks
      const context: PageScriptContext = {
        ...pageScriptDef.properties,
        $changeDetector: {
          detectChanges: () => {
            cd.next();
            this.ngZone.run(() => {
              this.changeDetectorRef.detectChanges();

              // Değişiklikleri anında yansıt
              setTimeout(() => {
                if (this.currentPage && this.pageContentContainer) {
                  this.processAngularDirectives();
                }
              }, 0);
            });
          },
          markForCheck: () => {
            this.ngZone.run(() => this.changeDetectorRef.markForCheck());
          },
          detach: () => {
            // No-op implementation
          },
          reattach: () => {
            // No-op implementation
          },
          checkNoChanges: () => {
            // No-op implementation
          },
        },
      };

      // Store the context
      this.pageScriptContexts.set(pageId, context);

      // Set up DOM event handlers if any defined in the script
      if (pageScriptDef.properties) {
        this.setupPageEventHandlers(context);
      }

      // Call onInit if it exists
      if (pageScriptDef.onInit) {
        context.$onInit = pageScriptDef.onInit;
        setTimeout(() => {
          try {
            if (context.$onInit) {
              context.$onInit();
            }
          } catch (e) {
            console.error(`Error calling onInit for page ${pageId}:`, e);
          }
        }, 0);
      }

      // Store onDestroy if it exists
      if (pageScriptDef.onDestroy) {
        context.$onDestroy = pageScriptDef.onDestroy;
      }

      // Subscribe to change detection
      cd.pipe(takeUntil(this.destroy$)).subscribe(() => {
        // Apply any data binding updates when change detection runs
        // This would be implemented if needed for two-way binding
      });

      console.log(`JavaScript successfully applied for page ${pageId}`);
    } catch (error) {
      console.error(`Error applying JavaScript for page ${pageId}:`, error);
      this.showPageScriptError(error, pageId);
    }
  }

  /**
   * Parse JavaScript code to extract properties, methods, and lifecycle hooks
   */
  private parsePageScript(code: string): {
    properties: { [key: string]: any };
    onInit?: () => void;
    onDestroy?: () => void;
  } {
    if (!code || !code.trim()) {
      return { properties: {} };
    }

    console.log('Parsing JavaScript code:', code);

    try {
      // Function to extract property declarations
      const extractProperties = (code: string) => {
        const result: { [key: string]: any } = {};

        // Extract property declarations like "propertyName = value;"
        const propRegex = /([a-zA-Z_$][a-zA-Z0-9_$]*)\s*:\s*([^;,]+)[;,]/g;
        let match;
        while ((match = propRegex.exec(code)) !== null) {
          const propName = match[1].trim();
          const propValue = match[2].trim();

          console.log(`Found property: ${propName} = ${propValue}`);

          // Try to evaluate the property value
          try {
            // For simple values like strings, numbers, booleans
            if (propValue === 'true') result[propName] = true;
            else if (propValue === 'false') result[propName] = false;
            else if (propValue === 'null') result[propName] = null;
            else if (propValue === 'undefined') result[propName] = undefined;
            else if (!isNaN(Number(propValue)))
              result[propName] = Number(propValue);
            else if (propValue.startsWith('"') || propValue.startsWith("'")) {
              // String value - use Function instead of eval
              result[propName] = new Function(`return ${propValue}`)();
            } else if (propValue.startsWith('[') || propValue.startsWith('{')) {
              // Array or object
              result[propName] = new Function(`return ${propValue}`)();
            } else {
              console.warn(
                `Unable to evaluate property "${propName}": ${propValue}`
              );
            }
          } catch (e) {
            console.error(`Error evaluating property "${propName}":`, e);
          }
        }

        // CMS formatı için değişken tanımlamalarını da kontrol et
        // Örnek: colors: string[] = ['Kırmızı', 'Mavi', 'Yeşil'];
        const typedPropRegex =
          /([a-zA-Z_$][a-zA-Z0-9_$]*)\s*:\s*[a-zA-Z<>[\]]+\s*=\s*([^;]+);/g;
        while ((match = typedPropRegex.exec(code)) !== null) {
          const propName = match[1].trim();
          const propValue = match[2].trim();

          console.log(`Found typed property: ${propName} = ${propValue}`);

          try {
            // Array veya obje değeri
            if (propValue.startsWith('[') || propValue.startsWith('{')) {
              result[propName] = new Function(`return ${propValue}`)();
            } else {
              console.warn(
                `Unable to evaluate typed property "${propName}": ${propValue}`
              );
            }
          } catch (e) {
            console.error(`Error evaluating typed property "${propName}":`, e);
          }
        }

        return result;
      };

      // Function to extract methods
      const extractMethods = (code: string) => {
        const result: { [key: string]: (...args: any[]) => any } = {};

        // Look for method definitions like "methodName() { ... }"
        const methodRegex =
          /([a-zA-Z_$][a-zA-Z0-9_$]*)\s*\(\s*([^)]*)\s*\)\s*{([^{}]*(?:{[^{}]*}[^{}]*)*)}/g;
        let match;
        while ((match = methodRegex.exec(code)) !== null) {
          const methodName = match[1].trim();
          const params = match[2].trim();
          const body = match[3];

          console.log(
            `Found method: ${methodName}(${params}) with body length: ${body.length}`
          );

          try {
            // Create function with the extracted body
            result[methodName] = new Function(params, body) as (
              ...args: any[]
            ) => any;
          } catch (e) {
            console.error(`Error creating method "${methodName}":`, e);
          }
        }

        // CMS formatı için - Tek satırlık metot tanımları için de kontrol edelim
        // Örnek: showAlert() { alert('Hello World'); }
        const lines = code.split('\n');
        for (let i = 0; i < lines.length; i++) {
          const line = lines[i].trim();
          if (line && !line.startsWith('//')) {
            // Basit metot tanımlamaları için kontrol
            const simpleMethodMatch = line.match(
              /^([a-zA-Z_$][a-zA-Z0-9_$]*)\s*\(([^)]*)\)\s*{(.+)}$/
            );
            if (simpleMethodMatch) {
              const methodName = simpleMethodMatch[1].trim();
              const params = simpleMethodMatch[2].trim();
              const body = simpleMethodMatch[3].trim();

              if (!result[methodName]) {
                // Eğer daha önce eklenmemişse
                console.log(
                  `Creating simple method: ${methodName}(${params}) with body: ${body}`
                );
                try {
                  result[methodName] = new Function(
                    ...params.split(',').map((p) => p.trim()),
                    body
                  ) as (...args: any[]) => any;
                } catch (e) {
                  console.error(
                    `Error creating simple method "${methodName}":`,
                    e
                  );
                }
              }
            }
          }
        }

        return result;
      };

      // Extract lifecycle hooks
      const getLifecycleHook = (
        code: string,
        hookName: string
      ): (() => void) | undefined => {
        const hookRegex = new RegExp(
          `function\\s+${hookName}\\s*\\(\\s*\\)\\s*{([^}]*)}`,
          'g'
        );
        const match = hookRegex.exec(code);
        if (match) {
          try {
            return new Function(match[1]) as () => void;
          } catch (e) {
            console.error(`Error creating lifecycle hook "${hookName}":`, e);
            return undefined;
          }
        }
        return undefined;
      };

      // Extract properties and methods
      const properties = {
        ...extractProperties(code),
        ...extractMethods(code),
      };

      console.log('Extracted properties and methods:', Object.keys(properties));

      // Extract lifecycle hooks
      const onInit = getLifecycleHook(code, 'onInit');
      const onDestroy = getLifecycleHook(code, 'onDestroy');

      return {
        properties,
        onInit,
        onDestroy,
      };
    } catch (error) {
      console.error('Error parsing page script:', error);
      return { properties: {} };
    }
  }

  /**
   * Show script error in the console
   */
  private showPageScriptError(error: any, pageId: number): void {
    console.error(`Script error for page ${pageId}:`, error);

    // Optionally add a visual error indicator to the page
    const errorElement = document.createElement('div');
    errorElement.style.padding = '10px';
    errorElement.style.backgroundColor = '#ffeeee';
    errorElement.style.border = '1px solid #ffcccc';
    errorElement.style.color = '#cc0000';
    errorElement.style.borderRadius = '4px';
    errorElement.style.margin = '5px 0';
    errorElement.style.position = 'fixed';
    errorElement.style.bottom = '20px';
    errorElement.style.right = '20px';
    errorElement.style.zIndex = '1000';
    errorElement.textContent = `JavaScript Error: ${
      error.message || 'Unknown error'
    }`;

    document.body.appendChild(errorElement);

    // Auto-remove after 5 seconds
    setTimeout(() => {
      errorElement.remove();
    }, 5000);
  }

  /**
   * Angular direktiflerini işleyen metot (*ngFor, *ngIf vb.)
   */
  private processAngularDirectives(): void {
    if (!this.pageContentContainer || !this.currentPage) return;

    const container = this.pageContentContainer.nativeElement;

    // Sayfa context'ini al
    const pageContext =
      this.currentPage && this.pageScriptContexts.has(this.currentPage.id)
        ? this.pageScriptContexts.get(this.currentPage.id)
        : null;

    if (!pageContext) {
      console.warn('No page context found for directives processing');
      return;
    }

    // *ngFor direktiflerini işle
    this.processNgForDirectives(container, pageContext);

    // *ngIf direktiflerini işle
    this.processNgIfDirectives(container, pageContext);

    // Interpolasyon ve property binding işle
    this.processDataBindings(container, pageContext);

    // Change detection'ı tetikle
    if (pageContext.$changeDetector) {
      pageContext.$changeDetector.detectChanges();
    }
  }

  /**
   * *ngFor direktiflerini işler
   */
  private processNgForDirectives(
    container: HTMLElement,
    context: PageScriptContext
  ): void {
    // *ngFor direktifi olan elementleri bul
    const ngForElements = Array.from(container.querySelectorAll('*[\\*ngFor]'));

    ngForElements.forEach((element: Element) => {
      const ngForAttr = element.getAttribute('*ngFor');
      if (!ngForAttr) return;

      // *ngFor="let item of items; let i = index" formatını parse et
      const match = ngForAttr.match(
        /let\s+([a-zA-Z0-9_$]+)\s+of\s+([a-zA-Z0-9_$.]+)(?:;\s*let\s+([a-zA-Z0-9_$]+)\s*=\s*index)?/
      );

      if (match) {
        const itemName = match[1];
        const itemsExpression = match[2];
        const indexName = match[3] || 'index';

        try {
          // Collection verisini context'ten al
          const items = this.evaluateExpression(itemsExpression, context);

          if (items && Array.isArray(items)) {
            // Orijinal element şablonu
            const templateHtml = element.outerHTML;

            // Attribute'u kaldır
            element.removeAttribute('*ngFor');

            // Orijinal elementin parent'ını bul
            const parent = element.parentNode;
            if (!parent) return;

            // Fragment oluştur
            const fragment = document.createDocumentFragment();

            // Her bir item için elementi kopyala
            items.forEach((item, index) => {
              // Yeni elementi kopyala (temel şablon olarak kullan)
              const newItem = element.cloneNode(true) as HTMLElement;

              // Her yeni element için item ve index değerlerini içeren özel data attribute'ları ekle
              newItem.setAttribute('data-ngfor-item', JSON.stringify(item));
              newItem.setAttribute('data-ngfor-index', index.toString());

              // Geçici bir context oluştur
              const itemContext = {
                ...context,
                [itemName]: item,
                [indexName]: index,
              };

              // İçerideki interpolasyonları ve property binding'leri işle
              this.processDataBindings(newItem, itemContext);

              // Event handler'ları içerideki elemana bağla
              this.processEventHandlersForElement(newItem, itemContext);

              // Yeni elementi fragment'a ekle
              fragment.appendChild(newItem);
            });

            // Orijinal elementi parent'tan kaldır
            parent.removeChild(element);

            // Fragment'ı parent'a ekle
            parent.appendChild(fragment);
          } else {
            console.warn(`*ngFor items is not an array:`, items);
          }
        } catch (error) {
          console.error(`Error processing *ngFor directive:`, error);
        }
      } else {
        console.warn(`Invalid *ngFor syntax: ${ngForAttr}`);
      }
    });
  }

  /**
   * *ngIf direktiflerini işler
   */
  private processNgIfDirectives(
    container: HTMLElement,
    context: PageScriptContext
  ): void {
    // *ngIf direktifi olan elementleri bul
    const ngIfElements = Array.from(container.querySelectorAll('*[\\*ngIf]'));

    ngIfElements.forEach((element: Element) => {
      const ngIfAttr = element.getAttribute('*ngIf');
      if (!ngIfAttr) return;

      try {
        // Koşulu değerlendir
        const condition = this.evaluateExpression(ngIfAttr, context);

        // Attribute'u kaldır
        element.removeAttribute('*ngIf');

        // Koşul false ise, elementi gizle
        if (!condition) {
          const parent = element.parentNode;
          if (parent) {
            parent.removeChild(element);
          }
        }
      } catch (error) {
        console.error(`Error processing *ngIf directive:`, error);
      }
    });
  }

  /**
   * Veri bağlamalarını işler ({{ expression }} ve [property]="expression")
   */
  private processDataBindings(
    container: HTMLElement,
    context: PageScriptContext
  ): void {
    // Text içindeki {{ }} interpolasyonlarını işle
    this.processInterpolations(container, context);

    // Property binding [property]="expression" işle
    this.processPropertyBindings(container, context);
  }

  /**
   * Text içindeki {{ }} interpolasyonlarını işler
   */
  private processInterpolations(
    element: HTMLElement,
    context: PageScriptContext
  ): void {
    // Elementteki text node'ları bul ve işle
    const processTextNode = (node: Node) => {
      if (node.nodeType === Node.TEXT_NODE && node.textContent) {
        const text = node.textContent;
        const interpolationRegex = /{{(.*?)}}/g;

        let match;
        let newText = text;
        let hasInterpolation = false;

        // Tüm interpolasyonları bul ve değerlendir
        while ((match = interpolationRegex.exec(text)) !== null) {
          hasInterpolation = true;
          const expression = match[1].trim();
          try {
            const value = this.evaluateExpression(expression, context);
            newText = newText.replace(
              match[0],
              value !== undefined ? String(value) : ''
            );
          } catch (error) {
            console.error(
              `Error evaluating interpolation "${expression}":`,
              error
            );
            newText = newText.replace(match[0], '');
          }
        }

        // Eğer interpolasyon varsa, node'u güncelle
        if (hasInterpolation) {
          node.textContent = newText;
        }
      } else if (node.nodeType === Node.ELEMENT_NODE) {
        // Alt elementleri de işle
        Array.from(node.childNodes).forEach(processTextNode);
      }
    };

    // Tüm alt node'ları işle
    Array.from(element.childNodes).forEach(processTextNode);
  }

  /**
   * Property binding [property]="expression" işler
   */
  private processPropertyBindings(
    container: HTMLElement,
    context: PageScriptContext
  ): void {
    // Tüm elementleri arat
    const allElements = container.querySelectorAll('*');

    // Her elementin tüm niteliklerini kontrol et
    allElements.forEach((element: Element) => {
      const attributes = element.attributes;

      for (let i = 0; i < attributes.length; i++) {
        const attr = attributes[i];

        // [property]="expression" formatını kontrol et
        if (attr.name.startsWith('[') && attr.name.endsWith(']')) {
          const propertyName = attr.name.substring(1, attr.name.length - 1);
          const expression = attr.value;

          try {
            // İfadeyi değerlendir
            const value = this.evaluateExpression(expression, context);

            // Element özelliğini ayarla
            if (propertyName.includes('.')) {
              // style.color gibi nested property ise
              const parts = propertyName.split('.');
              const mainProp = parts[0];
              const subProp = parts[1];

              if (mainProp === 'style' && element instanceof HTMLElement) {
                (element as any).style[subProp] = value;
              } else if (mainProp === 'class') {
                if (value) {
                  element.classList.add(subProp);
                } else {
                  element.classList.remove(subProp);
                }
              }
            } else {
              // Normal property ise
              (element as any)[propertyName] = value;

              // Bazı özel durumlar için attribute da ayarla
              if (
                propertyName === 'disabled' ||
                propertyName === 'checked' ||
                propertyName === 'selected'
              ) {
                if (value) {
                  element.setAttribute(propertyName, 'true');
                } else {
                  element.removeAttribute(propertyName);
                }
              }
            }

            // Orijinal Angular niteliğini kaldır
            element.removeAttribute(attr.name);
            i--; // index'i düzelt çünkü attribute listesi güncellendi
          } catch (error) {
            console.error(
              `Error processing property binding [${propertyName}]:`,
              error
            );
          }
        }
      }
    });
  }

  /**
   * Tek bir element için HTML içindeki Angular olay işleyicilerini işleyen metot
   */
  private processEventHandlersForElement(
    element: HTMLElement,
    context: PageScriptContext
  ): void {
    const processElement = (el: Element) => {
      const attributes = el.attributes;
      const eventHandlers: { eventName: string; handlerName: string }[] = [];

      // (click), (input) gibi olay niteliklerini bul
      for (let i = 0; i < attributes.length; i++) {
        const attr = attributes[i];

        // Angular-benzeri olay niteliği mi kontrol et: (event)="handler()"
        if (attr.name.startsWith('(') && attr.name.endsWith(')')) {
          const eventName = attr.name.substring(1, attr.name.length - 1);
          const handlerExpression = attr.value;

          // showAlert() veya removeColor(i) gibi bir ifade içerebilir
          const match = handlerExpression.match(/([a-zA-Z0-9_$]+)\s*\((.*)\)/);
          if (match) {
            const handlerName = match[1];
            const paramStr = match[2].trim();

            eventHandlers.push({ eventName, handlerName });

            // Orijinal Angular niteliğini kaldır
            el.removeAttribute(attr.name);

            // Olay dinleyici ekle
            el.addEventListener(eventName, (event) => {
              // Eğer JavaScript context'i varsa, metodu çağır
              if (context && typeof context[handlerName] === 'function') {
                try {
                  // Parametreleri değerlendir
                  let params: any[] = [];
                  if (paramStr) {
                    if (paramStr === 'index' || paramStr === 'i') {
                      // ngFor için özel durum: index değerini al
                      const index = el.getAttribute('data-ngfor-index');
                      if (index !== null) {
                        params = [parseInt(index)];
                      }
                    } else if (paramStr === '$event') {
                      params = [event];
                    } else {
                      // Diğer parametreleri değerlendir
                      params = paramStr.split(',').map((p) => {
                        try {
                          return this.evaluateExpression(p.trim(), context);
                        } catch (e) {
                          return p.trim();
                        }
                      });
                    }
                  }

                  // Metodu çağır
                  context[handlerName].apply(context, params);

                  // Değişikliklerden sonra Angular direktiflerini tekrar işle
                  if (this.currentPage && this.pageContentContainer) {
                    // UI güncelleme için change detector'ı tetikle
                    if (context.$changeDetector) {
                      context.$changeDetector.detectChanges();
                    }
                  }
                } catch (error) {
                  console.error(
                    `Error executing event handler "${handlerName}" with params ${paramStr}:`,
                    error
                  );
                }
              } else {
                console.warn(
                  `Event handler method "${handlerName}" not found in context`
                );
              }
            });
          } else {
            // Parametre olmayan durumlar için: (click)="showAlert"
            const handlerName = handlerExpression.trim();
            eventHandlers.push({ eventName, handlerName });

            // Orijinal Angular niteliğini kaldır
            el.removeAttribute(attr.name);

            // Olay dinleyici ekle
            el.addEventListener(eventName, (event) => {
              if (context && typeof context[handlerName] === 'function') {
                try {
                  context[handlerName].call(context, event);

                  // Değişikliklerden sonra Angular direktiflerini tekrar işle
                  if (this.currentPage && this.pageContentContainer) {
                    // UI güncelleme için change detector'ı tetikle
                    if (context.$changeDetector) {
                      context.$changeDetector.detectChanges();
                    }
                  }
                } catch (error) {
                  console.error(
                    `Error executing event handler "${handlerName}":`,
                    error
                  );
                }
              } else {
                console.warn(
                  `Event handler method "${handlerName}" not found in context`
                );
              }
            });
          }
        }
      }
    };

    // Ana elementi işle
    processElement(element);

    // Alt elementleri de işle
    const childElements = element.querySelectorAll('*');
    childElements.forEach((el) => {
      processElement(el);
    });
  }

  /**
   * JavaScript ifadesini belirli bir context içinde değerlendirir
   */
  private evaluateExpression(expression: string, context: any): any {
    try {
      // Basit property erişimi ise direkt dön
      if (/^[a-zA-Z_$][a-zA-Z0-9_$]*$/.test(expression)) {
        return context[expression];
      }

      // Daha karmaşık ifadeler için Function kullan
      const fn = new Function(
        'context',
        `with(context) { return ${expression}; }`
      );
      return fn(context);
    } catch (error) {
      console.error(`Error evaluating expression "${expression}":`, error);
      return undefined;
    }
  }

  /**
   * Set up event handlers for the page
   */
  private setupPageEventHandlers(context: PageScriptContext): void {
    if (!this.pageContentContainer || !this.currentPage) return;

    const container = this.pageContentContainer.nativeElement;

    // Bu method processEventHandlers ile birlikte çalışır
    // Burada global tıklama olay dinleyicileri vs. eklenebilir

    console.log('Setting up page event handlers for container:', container);
  }

  /**
   * Bootstrap CSS'i CDN üzerinden ekler
   */
  private loadBootstrapCSS(): void {
    // Check if Bootstrap CSS is already loaded
    const existingLink = document.querySelector(
      `link[href="${this.bootstrapCssLink}"]`
    );

    if (!existingLink) {
      console.log('Loading Bootstrap CSS from CDN');
      const linkElement = document.createElement('link');
      linkElement.rel = 'stylesheet';
      linkElement.href = this.bootstrapCssLink;
      document.head.appendChild(linkElement);
    } else {
      console.log('Bootstrap CSS is already loaded');
    }

    // Check if Bootstrap JS is already loaded
    const existingScript = document.querySelector(
      `script[src="${this.bootstrapJsLink}"]`
    );

    if (!existingScript) {
      console.log('Loading Bootstrap JS from CDN');
      const scriptElement = document.createElement('script');
      scriptElement.src = this.bootstrapJsLink;
      document.body.appendChild(scriptElement);
    } else {
      console.log('Bootstrap JS is already loaded');
    }
  }
}
