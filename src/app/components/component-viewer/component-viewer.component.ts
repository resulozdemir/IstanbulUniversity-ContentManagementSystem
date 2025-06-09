import {
  Component,
  Input,
  ViewChild,
  ViewContainerRef,
  ComponentRef,
  OnChanges,
  SimpleChanges,
  Injector,
  OnInit,
  OnDestroy,
  createComponent,
  ElementRef,
  ApplicationRef,
  EnvironmentInjector,
  Type,
  NgZone,
  ChangeDetectorRef,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PageService, ComponentData } from '../../pages/page.service';
import { Observable, of, forkJoin, Subject, BehaviorSubject } from 'rxjs';
import { map, catchError, switchMap, tap, takeUntil } from 'rxjs/operators';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';

interface AngularContext {
  [key: string]: any;
  $data?: any;
  $changeDetector?: ChangeDetectorRef;
  $onInit?: () => void;
  $onDestroy?: () => void;
}

@Component({
  selector: 'app-component-viewer',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="component-container">
      <div #dynamicComponentContainer></div>
    </div>
  `,
  styles: [
    `
      .component-viewer {
        display: flex;
        flex-direction: column;
        gap: 20px;
      }

      .preview-section {
        padding: 20px;
        border-radius: 8px;
        background-color: white;
        box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
      }

      .component-preview {
        padding: 20px;
        border: 1px solid #eee;
        border-radius: 4px;
      }

      .code-sections {
        background-color: #f5f5f5;
        padding: 20px;
        border-radius: 8px;
      }

      .code-section {
        margin-bottom: 20px;
      }

      pre {
        background-color: #fff;
        padding: 15px;
        border-radius: 4px;
        border: 1px solid #ddd;
        overflow-x: auto;
        font-family: 'Courier New', Courier, monospace;
      }

      .component-container {
        display: block;
        width: 100%;
      }

      .nested-component-container {
        display: contents;
      }

      .error-placeholder {
        padding: 10px;
        background-color: #ffeeee;
        border: 1px solid #ffcccc;
        color: #cc0000;
        border-radius: 4px;
        margin: 5px 0;
      }

      .loading-placeholder {
        padding: 10px;
        background-color: #f0f0f0;
        border: 1px solid #e0e0e0;
        color: #666;
        border-radius: 4px;
        margin: 5px 0;
        font-style: italic;
      }
    `,
  ],
})
export class ComponentViewerComponent implements OnChanges, OnInit, OnDestroy {
  @Input() cHtml: string = '';
  @Input() cCss: string = '';
  @Input() cJs: string = '';
  @Input() componentId: string = '';

  @ViewChild('dynamicComponentContainer', {
    static: true,
    read: ElementRef,
  })
  container!: ElementRef;

  // Bootstrap CSS CDN link
  private bootstrapCssLink =
    'https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css';
  // Bootstrap JS CDN link
  private bootstrapJsLink =
    'https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js';

  private loadedComponents: Map<string, ComponentData> = new Map();
  private componentContexts: Map<string, AngularContext> = new Map();
  private sanitizedHtml: SafeHtml = '';
  private loadingError: string | null = null;
  private destroy$ = new Subject<void>();

  constructor(
    private pageService: PageService,
    private sanitizer: DomSanitizer,
    private ngZone: NgZone,
    private changeDetectorRef: ChangeDetectorRef,
    private injector: Injector
  ) {}

  ngOnInit() {
    // Load Bootstrap CSS
    this.loadBootstrapCSS();

    if (this.componentId) {
      this.loadComponentById(this.componentId);
    } else if (this.cHtml || this.cCss || this.cJs) {
      this.renderComponent();
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['componentId'] && !changes['componentId'].firstChange) {
      this.loadComponentById(this.componentId);
    } else if (
      (changes['cHtml'] && !changes['cHtml'].firstChange) ||
      (changes['cCss'] && !changes['cCss'].firstChange) ||
      (changes['cJs'] && !changes['cJs'].firstChange)
    ) {
      this.renderComponent();
    }
  }

  ngOnDestroy() {
    // Cleanup all component contexts
    this.componentContexts.forEach((context) => {
      if (context.$onDestroy) {
        try {
          context.$onDestroy();
        } catch (e) {
          console.error('Error in component $onDestroy:', e);
        }
      }
    });

    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadComponentById(id: string) {
    console.log(`Loading component by ID: ${id}`);

    this.pageService.getComponentById(id).subscribe({
      next: (component: ComponentData) => {
        if (component) {
          console.log(`Component ${id} loaded:`, component);
          // Template, style ve javascript alanlarını HTML, CSS ve JS için kullan
          this.cHtml = component.html; // Bu zaten template'ın işlenmiş hali
          this.cCss = component.css; // Bu zaten style'ın işlenmiş hali
          this.cJs = component.js; // Bu zaten javascript'in işlenmiş hali

          // Component verilerini değerlendir
          if (component.data) {
            try {
              let parsedData = component.data;

              // Eğer hala string ise parse et
              if (typeof parsedData === 'string') {
                parsedData = JSON.parse(parsedData);
              }

              console.log(`Component ${id} data:`, parsedData);

              // Bileşeni yüklerken verilerini de al
              component.data = parsedData;
            } catch (e) {
              console.error(`Error processing component data for ID ${id}:`, e);
            }
          }

          // Store the component for future reference
          this.loadedComponents.set(id, component);

          this.loadNestedComponents().subscribe(() => {
            this.renderComponent();
          });
        } else {
          console.error(`Component with ID ${id} not found`);
          this.loadingError = `Component with ID ${id} not found`;
        }
      },
      error: (error: any) => {
        console.error(`Error loading component with ID ${id}:`, error);
        this.loadingError = `Error loading component: ${error.message}`;
      },
    });
  }

  private loadNestedComponents(): Observable<boolean> {
    if (!this.cHtml) return of(true);

    // Find all component IDs in the HTML
    const regex =
      /<app-component-viewer\s+\[componentId\]="([^"]+)"\s*><\/app-component-viewer>/g;
    const componentIds: string[] = [];
    let match;

    const htmlContent = this.cHtml;
    const regexClone = new RegExp(regex);

    while ((match = regexClone.exec(htmlContent)) !== null) {
      let componentId = match[1];
      if (componentId.startsWith("'") && componentId.endsWith("'")) {
        componentId = componentId.slice(1, -1);
      }
      componentIds.push(componentId);
    }

    if (componentIds.length === 0) {
      return of(true);
    }

    // Load all components in parallel
    const observables = componentIds.map((id) =>
      this.pageService.getComponentById(id).pipe(
        tap((component: ComponentData) => {
          if (component) {
            this.loadedComponents.set(id, component);
          }
        }),
        catchError(() => of(null))
      )
    );

    return forkJoin(observables).pipe(
      switchMap(() => {
        // Now recursively load nested components inside the loaded components
        const nestedObservables = Array.from(this.loadedComponents.values())
          .filter((comp) => comp?.html)
          .map((comp) => this.loadNestedComponentsFromHtml(comp.html));

        if (nestedObservables.length === 0) {
          return of(true);
        }

        return forkJoin(nestedObservables).pipe(map(() => true));
      })
    );
  }

  private loadNestedComponentsFromHtml(html: string): Observable<boolean> {
    if (!html) return of(true);

    // Find all component IDs in the HTML
    const regex =
      /<app-component-viewer\s+\[componentId\]="([^"]+)"\s*><\/app-component-viewer>/g;
    const componentIds: string[] = [];
    let match;

    const regexClone = new RegExp(regex);

    while ((match = regexClone.exec(html)) !== null) {
      let componentId = match[1];
      if (componentId.startsWith("'") && componentId.endsWith("'")) {
        componentId = componentId.slice(1, -1);
      }
      // Only add if not already loaded
      if (!this.loadedComponents.has(componentId)) {
        componentIds.push(componentId);
      }
    }

    if (componentIds.length === 0) {
      return of(true);
    }

    // Load all components in parallel
    const observables = componentIds.map((id) =>
      this.pageService.getComponentById(id).pipe(
        tap((component: ComponentData) => {
          if (component) {
            this.loadedComponents.set(id, component);
          }
        }),
        catchError(() => of(null))
      )
    );

    return forkJoin(observables).pipe(
      switchMap(() => {
        // Now recursively load nested components inside the loaded components
        const nestedObservables = componentIds
          .map((id) => this.loadedComponents.get(id))
          .filter((comp): comp is ComponentData => !!comp && !!comp.html)
          .map((comp) => this.loadNestedComponentsFromHtml(comp.html));

        if (nestedObservables.length === 0) {
          return of(true);
        }

        return forkJoin(nestedObservables).pipe(map(() => true));
      })
    );
  }

  private renderComponent() {
    if (!this.container) return;

    try {
      console.log('Rendering component...');
      console.log('HTML:', this.cHtml);
      console.log('CSS:', this.cCss);
      console.log('JS:', this.cJs);

      // Process HTML to handle nested component viewers
      const processedHtml = this.processNestedComponentViewers(this.cHtml);
      const containerElement = this.container.nativeElement;

      // Clear the container
      containerElement.innerHTML = '';

      // Add the CSS
      const styleElement = document.createElement('style');
      styleElement.textContent = this.cCss || '';
      containerElement.appendChild(styleElement);

      // Create unique ID for this component instance
      const componentInstanceId = `comp-${Date.now()}-${Math.floor(
        Math.random() * 10000
      )}`;

      // Create component root element
      const contentDiv = document.createElement('div');
      contentDiv.className = 'component-content'; // Add Bootstrap container class
      contentDiv.setAttribute('data-component-instance', componentInstanceId);

      // Process Angular-style template before adding to DOM
      const angularProcessedHtml = this.processAngularTemplate(processedHtml);

      // Parse component data
      const componentData = this.parseComponentData();

      // Process handlebars templates if present
      const finalHtml = this.processHandlebarsTemplate(
        angularProcessedHtml,
        componentData
      );

      contentDiv.innerHTML = finalHtml;
      containerElement.appendChild(contentDiv);

      // Execute JavaScript to create Angular component context for main component
      if (this.cJs && this.cJs.trim()) {
        setTimeout(() => {
          try {
            console.log('Initializing main component...');
            this.createAngularComponent(
              this.cJs,
              contentDiv,
              componentInstanceId,
              componentData
            );
            console.log('Main component initialized successfully');

            // Initialize all nested components after parent is initialized
            // Use a small delay to ensure DOM is stable
            setTimeout(() => {
              try {
                this.initializeAllNestedComponents(containerElement);
              } catch (e) {
                console.error('Error initializing nested components:', e);
              }
            }, 100);
          } catch (e) {
            console.error('Error initializing main component:', e);
            this.showScriptError(e, contentDiv);
          }
        }, 50);
      } else {
        // If no JS in parent component, still initialize the nested components
        setTimeout(() => {
          try {
            this.initializeAllNestedComponents(containerElement);
          } catch (e) {
            console.error('Error initializing nested components:', e);
          }
        }, 100);
      }
    } catch (error) {
      console.error('Error rendering component:', error);
      this.showRenderError(error);
    }
  }

  /**
   * Parses the component data from JSON if available
   */
  private parseComponentData(): any {
    try {
      const component = this.loadedComponents.get(this.componentId);
      if (component?.data) {
        if (typeof component.data === 'string') {
          console.log('Component data before parsing:', component.data);
          const parsedData = JSON.parse(component.data);
          console.log('Component data after parsing:', parsedData);

          // Deep parse any potential string arrays or objects in the data
          this.deepParseJsonValues(parsedData);

          return parsedData;
        }
        return component.data;
      }

      // If no component data found from loaded components, try to get from the API response
      const params = new URLSearchParams(window.location.search);
      const siteId = params.get('siteId');
      const componentId = params.get('componentId');

      if (siteId && componentId) {
        // Try to load the component data directly - this is for debugging purposes
        console.log(
          `Attempting to load component data for siteId=${siteId}, componentId=${componentId}`
        );
        this.pageService
          .getComponentById(componentId)
          .subscribe((component: ComponentData) => {
            if (component && component.data) {
              console.log('Loaded component data:', component.data);
            }
          });
      }

      return {};
    } catch (e) {
      console.error('Error parsing component data:', e);
      return {};
    }
  }

  /**
   * Recursively parses JSON string values in an object
   * This helps with nested structures that might be serialized twice
   */
  private deepParseJsonValues(obj: any): any {
    if (!obj || typeof obj !== 'object') return;

    Object.keys(obj).forEach((key) => {
      const value = obj[key];

      // If it's an array, try to parse items if they're strings
      if (Array.isArray(value)) {
        console.log(`Found array at ${key}, length: ${value.length}`);

        // Process array items
        for (let i = 0; i < value.length; i++) {
          if (
            typeof value[i] === 'string' &&
            (value[i].startsWith('{') || value[i].startsWith('['))
          ) {
            try {
              console.log(
                `Parsing array item string at ${key}[${i}]:`,
                value[i]
              );
              value[i] = JSON.parse(value[i]);
              console.log(`Parsed to:`, value[i]);
            } catch (e) {
              console.log(`Failed to parse ${key}[${i}]`, e);
            }
          }

          // Recursively process object items
          if (typeof value[i] === 'object' && value[i] !== null) {
            this.deepParseJsonValues(value[i]);
          }
        }
      }
      // If it's a string that looks like JSON, try to parse it
      else if (
        typeof value === 'string' &&
        (value.startsWith('{') || value.startsWith('['))
      ) {
        try {
          console.log(`Parsing string value at ${key}:`, value);
          obj[key] = JSON.parse(value);
          console.log(`Parsed to:`, obj[key]);

          // Recursively parse the new object
          this.deepParseJsonValues(obj[key]);
        } catch (e) {
          console.log(`Failed to parse ${key}`, e);
        }
      }
      // If it's an object, recursively process it
      else if (typeof value === 'object' && value !== null) {
        this.deepParseJsonValues(value);
      }
    });

    return obj;
  }

  /**
   * Process Handlebars template syntax including {{variable}}, {{#each}}, and {{#if}}
   */
  private processHandlebarsTemplate(template: string, data: any): string {
    if (!template) return '';

    console.log('🔍 Handlebars template işleniyor', data);
    console.log('Template:', template);

    // 1. #each döngülerini işle
    let result = this.processHandlebarsEach(template, data);

    // 2. #if koşullarını işle
    result = this.processHandlebarsIf(result, data);

    // 3. Değişkenleri değiştir
    result = this.processHandlebarsVariables(result, data);

    console.log('✅ Handlebars template işlendi');
    console.log('İşlenmiş template:', result);

    return result;
  }

  /**
   * {{#each array}} ... {{/each}} şeklindeki döngüleri işler
   */
  private processHandlebarsEach(template: string, data: any): string {
    const eachRegex = /\{\{#each\s+([^}]+)\}\}([\s\S]*?)\{\{\/each\}\}/g;

    return template.replace(eachRegex, (match, arrayName, content) => {
      try {
        const trimmedArrayName = arrayName.trim();
        console.log(`🔄 #each döngüsü işleniyor: ${trimmedArrayName}`);

        let array = this.getNestedProperty(data, trimmedArrayName);
        console.log(`${trimmedArrayName} için değer:`, array);

        // Eğer JSON string gelirse parse et
        if (
          typeof array === 'string' &&
          (array.startsWith('[') || array.startsWith('{'))
        ) {
          try {
            array = JSON.parse(array);
            console.log(`${trimmedArrayName} string'den parse edildi:`, array);
          } catch (e) {
            console.warn(
              `⚠️ ${trimmedArrayName} JSON olarak parse edilemedi:`,
              e
            );
          }
        }

        // Eğer nesne gelirse key-value dizisine dönüştür
        if (
          !Array.isArray(array) &&
          typeof array === 'object' &&
          array !== null
        ) {
          array = Object.entries(array).map(([key, value]) => ({ key, value }));
          console.log(
            `${trimmedArrayName} nesnesi diziye dönüştürüldü:`,
            array
          );
        }

        if (!array || !Array.isArray(array) || array.length === 0) {
          console.warn(
            `⚠️ '${trimmedArrayName}' bir dizi değil veya boş:`,
            array
          );
          return `<!-- Boş veya geçersiz dizi: ${trimmedArrayName} -->`;
        }

        console.log(`✓ ${array.length} elemanlı dizi işleniyor`);

        return array
          .map((item, index) => {
            // İşlenecek öğe için context oluştur
            const itemContext = {
              ...item,
              '@index': index, // Handlebars'ın @index özelliğini taklit et
            };

            // Önce iç döngüleri işle (özyinelemeli olarak)
            let processedContent = this.processHandlebarsEach(
              content,
              itemContext
            );

            // Sonra iç koşulları işle
            processedContent = this.processHandlebarsIf(
              processedContent,
              itemContext
            );

            // En son değişkenleri işle
            processedContent = this.processHandlebarsVariables(
              processedContent,
              itemContext
            );

            return processedContent;
          })
          .join('');
      } catch (error: any) {
        console.error(`❌ #each işleme hatası (${arrayName}):`, error);
        return `<!-- Hata: #each ${arrayName} - ${error.message} -->`;
      }
    });
  }

  /**
   * {{#if condition}} ... {{else}} ... {{/if}} şeklindeki koşulları işler
   */
  private processHandlebarsIf(template: string, data: any): string {
    const ifRegex =
      /\{\{#if\s+([^}]+)\}\}([\s\S]*?)(?:\{\{else\}\}([\s\S]*?))?\{\{\/if\}\}/g;

    return template.replace(
      ifRegex,
      (match, condition, ifContent, elseContent = '') => {
        try {
          const trimmedCondition = condition.trim();
          console.log(`🔍 #if koşulu işleniyor: ${trimmedCondition}`);

          const conditionValue = this.getNestedProperty(data, trimmedCondition);
          console.log(`${trimmedCondition} için değer:`, conditionValue);

          if (conditionValue) {
            // True durumunda if içeriğini işle
            let processedContent = this.processHandlebarsVariables(
              ifContent,
              data
            );
            return processedContent;
          } else {
            // False durumunda else içeriğini işle (varsa)
            let processedContent = this.processHandlebarsVariables(
              elseContent,
              data
            );
            return processedContent;
          }
        } catch (error: any) {
          console.error(`❌ #if işleme hatası (${condition}):`, error);
          return `<!-- Hata: #if ${condition} - ${error.message} -->`;
        }
      }
    );
  }

  /**
   * {{variable}} ve {{obj.prop}} formatındaki değişkenleri değiştirir
   */
  private processHandlebarsVariables(template: string, data: any): string {
    if (!template) return '';

    // 1. Önce noktalı değişkenleri işle (obj.prop)
    const dotNotationRegex = /\{\{([^{}#\/]+\.[^{}]+)\}\}/g;
    let result = template.replace(dotNotationRegex, (match, path) => {
      try {
        const trimmedPath = path.trim();
        const value = this.getNestedProperty(data, trimmedPath);

        return this.formatValue(value);
      } catch (error: any) {
        console.error(`❌ Noktalı değişken hatası (${path}):`, error);
        return `<!-- Hata: ${path} - ${error.message} -->`;
      }
    });

    // 2. Basit değişkenleri işle
    const simpleVarRegex = /\{\{([^{}#\/]+)\}\}/g;
    result = result.replace(simpleVarRegex, (match, key) => {
      try {
        const trimmedKey = key.trim();

        // @index gibi özel değişkenleri kontrol et
        if (trimmedKey.startsWith('@')) {
          if (data && trimmedKey in data) {
            return this.formatValue(data[trimmedKey]);
          }
          return '';
        }

        // Normal değişkenleri işle
        if (data && trimmedKey in data) {
          return this.formatValue(data[trimmedKey]);
        }

        return '';
      } catch (error: any) {
        console.error(`❌ Basit değişken hatası (${key}):`, error);
        return `<!-- Hata: ${key} - ${error.message} -->`;
      }
    });

    return result;
  }

  /**
   * Değişken değerini formatlı şekilde döndürür
   */
  private formatValue(value: any): string {
    if (value === undefined || value === null) {
      return '';
    } else if (typeof value === 'object') {
      return JSON.stringify(value);
    } else {
      return String(value);
    }
  }

  /**
   * İç içe obje yapılarında nokta gösterimiyle değer alır (obj.prop.subprop)
   */
  private getNestedProperty(obj: any, path: string): any {
    if (!obj || !path) return undefined;

    // Eğer direkt olarak nesnede varsa hemen döndür
    if (path in obj) {
      return obj[path];
    }

    // Nokta ile ayrılmış path'i array'e dönüştür
    const parts = path.split('.');
    let current = obj;

    for (const part of parts) {
      if (current === null || current === undefined) {
        return undefined;
      }

      current = current[part];
    }

    return current;
  }

  private showRenderError(error: any) {
    if (!this.container) return;

    const containerElement = this.container.nativeElement;
    const errorDiv = document.createElement('div');
    errorDiv.className = 'error-placeholder';
    errorDiv.textContent = `Error rendering component: ${
      error.message || 'Unknown error'
    }`;
    containerElement.appendChild(errorDiv);
  }

  private processNestedComponentViewers(html: string): string {
    if (!html) return '';

    // Replace app-component-viewer tags with placeholders that will be replaced with actual content
    const regex =
      /<app-component-viewer\s+\[componentId\]="([^"]+)"\s*><\/app-component-viewer>/g;

    return html.replace(regex, (match, componentIdExpr) => {
      // Extract the component ID from the expression (could be a string literal or a variable)
      let componentId = componentIdExpr;
      if (componentIdExpr.startsWith("'") && componentIdExpr.endsWith("'")) {
        componentId = componentIdExpr.slice(1, -1);
      }

      console.log(`Processing nested component with ID: ${componentId}`);

      // Get the preloaded component
      const component = this.loadedComponents.get(componentId);

      if (component) {
        // Process nested component's HTML recursively to handle multi-level nesting
        const nestedHtml = this.processNestedComponentViewers(component.html);

        // Generate unique ID for nested component
        const nestedComponentId = `nested-comp-${componentId}-${Date.now()}-${Math.floor(
          Math.random() * 10000
        )}`;

        console.log(
          `Created nested component instance with ID: ${nestedComponentId}`
        );

        // Create a container div with the nested component's styles
        return `<div class="nested-component-container" data-component-id="${componentId}" data-nested-component-instance="${nestedComponentId}">
          <style>${component.css || ''}</style>
          ${nestedHtml || ''}
        </div>`;
      }

      return `<div class="error-placeholder">
        Component with ID ${componentId} not found
      </div>`;
    });
  }

  /**
   * Creates an Angular-like component context for the given JavaScript
   */
  private createAngularComponent(
    javascript: string,
    containerElement: HTMLElement,
    instanceId: string,
    componentData: any
  ): AngularContext | null {
    if (!javascript || !containerElement) return null;

    try {
      // Parse the component script
      const componentDef = this.parseAngularComponent(javascript);

      // Create a change detector for this component
      const cd = new BehaviorSubject<void>(undefined);

      // Apply Bootstrap classes to main elements within container
      this.applyBootstrapClassesToElements(containerElement);

      // Create context with properties and methods
      const context: AngularContext = {
        ...componentDef.properties,
        $data: componentData || {},
        $changeDetector: {
          detectChanges: () => {
            cd.next();
            this.ngZone.run(() => {
              this.changeDetectorRef.detectChanges();
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

      // Store the context for this component instance
      this.componentContexts.set(instanceId, context);

      // Set up DOM event handlers
      this.setupEventHandlers(containerElement, context);

      // Call onInit if defined
      if (componentDef.onInit) {
        context.$onInit = componentDef.onInit;
        setTimeout(() => {
          try {
            if (context.$onInit) {
              context.$onInit();
            }
          } catch (e) {
            console.error(`Error in onInit:`, e);
            this.showScriptError(e, containerElement);
          }
        }, 0);
      }

      // Store onDestroy if defined
      if (componentDef.onDestroy) {
        context.$onDestroy = componentDef.onDestroy;
      }

      return context;
    } catch (e) {
      console.error('Error creating Angular component:', e);
      this.showScriptError(e, containerElement);
      return null;
    }
  }

  /**
   * Applies Bootstrap CSS classes to common HTML elements within a container
   */
  private applyBootstrapClassesToElements(container: HTMLElement): void {
    // Apply Bootstrap classes to various elements
    // Tables
    const tables = container.querySelectorAll('table:not([class*="table"])');
    tables.forEach((table) => {
      table.classList.add('table', 'table-striped');
    });

    // Buttons
    const buttons = container.querySelectorAll(
      'button:not([class*="btn"]), input[type="button"]:not([class*="btn"])'
    );
    buttons.forEach((button) => {
      button.classList.add('btn', 'btn-primary');
    });

    // Form inputs
    const inputs = container.querySelectorAll(
      'input[type="text"]:not([class*="form-control"]), input[type="email"]:not([class*="form-control"]), input[type="password"]:not([class*="form-control"]), textarea:not([class*="form-control"])'
    );
    inputs.forEach((input) => {
      input.classList.add('form-control');
    });

    // Select elements
    const selects = container.querySelectorAll(
      'select:not([class*="form-select"])'
    );
    selects.forEach((select) => {
      select.classList.add('form-select');
    });

    // Form groups
    const formGroups = container.querySelectorAll('div.form-group');
    formGroups.forEach((group) => {
      group.classList.add('mb-3');
    });
  }

  /**
   * Initialize all nested components recursively throughout the DOM tree
   */
  private initializeAllNestedComponents(rootElement: HTMLElement) {
    console.log('Initializing all nested components from root');
    const nestedElements = rootElement.querySelectorAll(
      '[data-nested-component-instance]'
    );

    console.log(`Found ${nestedElements.length} total nested components`);

    nestedElements.forEach((nestedEl) => {
      try {
        const componentId = nestedEl.getAttribute('data-component-id');
        const instanceId = nestedEl.getAttribute(
          'data-nested-component-instance'
        );

        // Check if this component was already initialized
        if (
          componentId &&
          instanceId &&
          !this.componentContexts.has(instanceId)
        ) {
          console.log(
            `Initializing nested component ${componentId} with instance ID ${instanceId}`
          );
          const component = this.loadedComponents.get(componentId);

          if (component && component.js && component.js.trim()) {
            // Parse component data
            let componentData = {};
            try {
              if (component.data) {
                if (typeof component.data === 'string') {
                  componentData = JSON.parse(component.data);
                } else {
                  componentData = component.data;
                }
              }
            } catch (e) {
              console.error(
                `Error parsing data for nested component ${componentId}:`,
                e
              );
            }

            try {
              // Log the JavaScript code being processed
              console.log(
                `Processing JS for component ${componentId}:`,
                component.js.substring(0, 100) + '...'
              );

              this.createAngularComponent(
                component.js,
                nestedEl as HTMLElement,
                instanceId,
                componentData
              );
              console.log(`Successfully initialized component ${componentId}`);
            } catch (e) {
              console.error(
                `Error creating Angular component ${componentId}:`,
                e
              );
              this.showScriptError(e, nestedEl as HTMLElement);
            }
          } else {
            console.warn(
              `Component ${componentId} not found or has no JavaScript code`
            );
          }
        }
      } catch (e) {
        console.error('Error in nested component initialization:', e);
      }
    });
  }

  /**
   * Parse Angular component code to extract properties, methods, and lifecycle hooks
   */
  private parseAngularComponent(code: string): {
    properties: { [key: string]: any };
    onInit?: () => void;
    onDestroy?: () => void;
  } {
    if (!code || !code.trim()) {
      return { properties: {} };
    }

    try {
      // Function to extract property declarations
      const extractProperties = (code: string) => {
        const result: { [key: string]: any } = {};

        // Extract property declarations like "propertyName = value;"
        const propRegex = /([a-zA-Z_$][a-zA-Z0-9_$]*)\s*=\s*([^;]+);/g;
        let match;
        while ((match = propRegex.exec(code)) !== null) {
          const propName = match[1].trim();
          const propValue = match[2].trim();

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
              // String value
              result[propName] = propValue.substring(1, propValue.length - 1);
            } else if (propValue.startsWith('[') && propValue.endsWith(']')) {
              // Array value
              try {
                const arrayStr = propValue.replace(/'/g, '"');
                result[propName] = JSON.parse(arrayStr);
              } catch {
                result[propName] = [];
              }
            } else if (propValue.startsWith('{') && propValue.endsWith('}')) {
              // Object value
              try {
                const objStr = propValue.replace(/'/g, '"');
                result[propName] = JSON.parse(objStr);
              } catch {
                result[propName] = {};
              }
            } else {
              // Default to storing as string
              result[propName] = propValue;
            }
          } catch (e) {
            console.warn(`Could not evaluate property ${propName}:`, e);
            result[propName] = null;
          }
        }

        return result;
      };

      // Improved method extraction that actually implements the methods
      const extractMethods = (code: string) => {
        const result: { [key: string]: Function } = {};

        try {
          // First, identify method names and parameter lists
          const methodNameRegex =
            /([a-zA-Z_$][a-zA-Z0-9_$]*)\s*\(([^)]*)\)\s*\{/g;
          let match;

          while ((match = methodNameRegex.exec(code)) !== null) {
            const methodName = match[1].trim();
            const params = match[2].split(',').map((p) => p.trim());
            const startPos = match.index;

            // Find the matching closing brace for this method
            try {
              const openBracePos = code.indexOf('{', startPos);

              if (openBracePos !== -1) {
                // Count opening and closing braces to find the matching closing brace
                let braceCount = 1;
                let closeBracePos = -1;

                for (let i = openBracePos + 1; i < code.length; i++) {
                  if (code[i] === '{') {
                    braceCount++;
                  } else if (code[i] === '}') {
                    braceCount--;
                    if (braceCount === 0) {
                      closeBracePos = i;
                      break;
                    }
                  }
                }

                if (closeBracePos !== -1) {
                  // Extract method body including nested braces
                  const body = code.substring(openBracePos + 1, closeBracePos);

                  // Create a real function with the actual implementation
                  try {
                    result[methodName] = new Function(...params, body);
                    console.log(`Successfully created method: ${methodName}`);
                  } catch (e) {
                    console.error(
                      `Error creating function for method ${methodName}:`,
                      e
                    );

                    // Fallback to a stub function that logs and does basic operations
                    result[methodName] = function (...args: any[]) {
                      console.log(
                        `Method ${methodName} called with args:`,
                        args
                      );

                      // Try to handle common scenarios (alerts, window.open, etc.)
                      if (methodName.toLowerCase().includes('alert')) {
                        window.alert(args[0] || 'Alert triggered');
                      } else if (
                        methodName.toLowerCase().includes('open') ||
                        methodName.toLowerCase().includes('navigate')
                      ) {
                        const url = args[0] || '';
                        if (url && typeof url === 'string') {
                          window.open(url, '_blank');
                        }
                      }

                      return null;
                    };
                  }
                }
              }
            } catch (e) {
              console.error(`Error parsing method ${methodName}:`, e);

              // Create a fallback stub function
              result[methodName] = function (...args: any[]) {
                console.log(`Method ${methodName} called with args:`, args);
                return null;
              };
            }
          }
        } catch (e) {
          console.error('Error extracting methods:', e);
        }

        return result;
      };

      // Improved lifecycle hook extraction
      const getLifecycleHook = (
        code: string,
        hookName: string
      ): (() => void) | undefined => {
        try {
          const hookRegex = new RegExp(`${hookName}\\s*\\(\\)\\s*\\{`, 'g');
          const match = hookRegex.exec(code);

          if (match) {
            const startPos = match.index;
            const openBracePos = code.indexOf('{', startPos);

            if (openBracePos !== -1) {
              // Count opening and closing braces to find the matching closing brace
              let braceCount = 1;
              let closeBracePos = -1;

              for (let i = openBracePos + 1; i < code.length; i++) {
                if (code[i] === '{') {
                  braceCount++;
                } else if (code[i] === '}') {
                  braceCount--;
                  if (braceCount === 0) {
                    closeBracePos = i;
                    break;
                  }
                }
              }

              if (closeBracePos !== -1) {
                // Extract hook body including nested braces
                const body = code.substring(openBracePos + 1, closeBracePos);

                // Create a real function with the actual implementation
                try {
                  return new Function(body) as unknown as () => void;
                } catch (e) {
                  console.error(`Error creating function for ${hookName}:`, e);

                  // Return a stub function that just logs
                  return () => {
                    console.log(`Lifecycle hook ${hookName} called (stub)`);
                  };
                }
              }
            }
          }
          return undefined;
        } catch (e) {
          console.error(`Error checking for lifecycle hook ${hookName}:`, e);
          return undefined;
        }
      };

      // Build component definition
      const properties = {
        ...extractProperties(code),
        ...extractMethods(code),
      };

      // Extract lifecycle hooks
      const onInit = getLifecycleHook(code, 'ngOnInit');
      const onDestroy = getLifecycleHook(code, 'ngOnDestroy');

      return {
        properties,
        onInit,
        onDestroy,
      };
    } catch (error) {
      console.error('Error parsing Angular component:', error);
      return { properties: {} };
    }
  }

  /**
   * Apply data bindings from the component context to the DOM
   */
  private applyDataBindings(
    element: HTMLElement,
    context: AngularContext,
    cd: BehaviorSubject<void> | null
  ) {
    // Process property bindings [property]="expression"
    const boundElements = element.querySelectorAll(
      '[data-bind-class],[data-bind-style],[data-bind-src],[data-bind-href],[data-bind-value],[data-bind-disabled],[data-bind-checked],[data-bind-hidden],[data-bind-readonly]'
    );

    boundElements.forEach((el) => {
      // Check each possible binding attribute
      const possibleBindings = [
        'class',
        'style',
        'src',
        'href',
        'value',
        'disabled',
        'checked',
        'hidden',
        'readonly',
      ];

      possibleBindings.forEach((binding) => {
        const bindingAttr = `data-bind-${binding}`;
        const bindingExpr = el.getAttribute(bindingAttr);

        if (bindingExpr) {
          // Get value from context
          const value = this.evaluateExpression(bindingExpr, context);

          // Apply to element
          if (binding === 'class') {
            // Class binding requires special handling
            if (typeof value === 'string') {
              el.className = value;
            } else if (typeof value === 'object') {
              // Handle object form like [class.active]="isActive"
              Object.entries(value).forEach(([className, shouldApply]) => {
                if (shouldApply) {
                  el.classList.add(className);
                } else {
                  el.classList.remove(className);
                }
              });
            }
          } else if (binding === 'style') {
            // Style binding
            if (typeof value === 'string') {
              (el as HTMLElement).style.cssText = value;
            } else if (typeof value === 'object') {
              Object.entries(value).forEach(([styleName, styleValue]) => {
                (el as HTMLElement).style.setProperty(
                  styleName,
                  styleValue as string
                );
              });
            }
          } else if (binding === 'value') {
            // Value binding for inputs
            (el as HTMLInputElement).value = value;
          } else {
            // Standard attribute
            if (value === true) {
              el.setAttribute(binding, '');
            } else if (value === false) {
              el.removeAttribute(binding);
            } else {
              el.setAttribute(binding, value);
            }
          }
        }
      });
    });

    // Handle {{interpolation}}
    this.processInterpolation(element, context);
  }

  /**
   * Process all {{interpolation}} expressions in the element's text content
   */
  private processInterpolation(element: HTMLElement, context: AngularContext) {
    const processNode = (node: Node) => {
      if (node.nodeType === Node.TEXT_NODE) {
        const text = node.textContent || '';
        if (text.includes('{{') && text.includes('}}')) {
          node.textContent = text.replace(/\{\{([^}]+)\}\}/g, (_, expr) => {
            const value = this.evaluateExpression(expr.trim(), context);
            return value !== undefined && value !== null
              ? value.toString()
              : '';
          });
        }
      } else if (node.nodeType === Node.ELEMENT_NODE) {
        // Skip nodes with data-bind or data-event attributes as they're handled separately
        const el = node as HTMLElement;
        if (
          !el.hasAttribute('data-component-instance') &&
          !el.hasAttribute('data-nested-component-instance')
        ) {
          // Process child nodes
          Array.from(node.childNodes).forEach(processNode);
        }
      }
    };

    // Process all child nodes
    Array.from(element.childNodes).forEach(processNode);
  }

  /**
   * Set up event handlers for (event)="handler()" expressions
   */
  private setupEventHandlers(element: HTMLElement, context: AngularContext) {
    // Find all elements with event bindings
    const eventElements = element.querySelectorAll(
      '[data-event-click],[data-event-input],[data-event-change],[data-event-focus],[data-event-blur],[data-event-submit]'
    );

    eventElements.forEach((el) => {
      // Check for different event types
      const possibleEvents = [
        'click',
        'input',
        'change',
        'focus',
        'blur',
        'submit',
      ];

      possibleEvents.forEach((eventName) => {
        const eventAttr = `data-event-${eventName}`;
        const handlerExpr = el.getAttribute(eventAttr);

        if (handlerExpr) {
          // Extract method name and parameters
          const methodMatch = handlerExpr.match(/([^(]+)(?:\((.*)\))?/);

          if (methodMatch) {
            const methodName = methodMatch[1].trim();

            // Check if method exists in context
            if (
              context[methodName] &&
              typeof context[methodName] === 'function'
            ) {
              // Add event listener
              el.addEventListener(eventName, (event) => {
                // Call the method in the component context
                this.ngZone.run(() => {
                  context[methodName].call(context, event);

                  // Trigger change detection
                  if (context.$changeDetector) {
                    context.$changeDetector.detectChanges();
                  }
                });
              });
            } else {
              console.warn(
                `Method "${methodName}" not found in component context`
              );
            }
          }
        }
      });
    });
  }

  /**
   * Evaluate an expression in the context of a component
   */
  private evaluateExpression(expression: string, context: AngularContext): any {
    try {
      // For simple property access, just return directly
      if (/^[a-zA-Z_$][a-zA-Z0-9_$]*$/.test(expression)) {
        return context[expression];
      }

      // For more complex expressions, use Function
      const fn = new Function(
        'context',
        `with(context) { return ${expression}; }`
      );
      return fn(context);
    } catch (error) {
      console.error(`Error evaluating expression "${expression}":`, error);
      return '';
    }
  }

  private showScriptError(error: any, containerElement: HTMLElement) {
    // Add an error message to the container
    const errorDiv = document.createElement('div');
    errorDiv.className = 'error-placeholder';
    errorDiv.style.margin = '10px 0';

    // Create a more detailed error message
    let errorMessage = `Script error: ${error.message || 'Unknown error'}`;

    // Add stack trace if available
    if (error.stack) {
      const shortStack = error.stack.split('\n').slice(0, 3).join('\n');
      errorMessage += `\nStack: ${shortStack}`;
    }

    // Add component info if available
    const componentId = containerElement.getAttribute('data-component-id');
    const instanceId =
      containerElement.getAttribute('data-nested-component-instance') ||
      containerElement.getAttribute('data-component-instance');

    if (componentId) {
      errorMessage += `\nComponent ID: ${componentId}`;
    }

    if (instanceId) {
      errorMessage += `\nInstance ID: ${instanceId}`;
    }

    console.error('Detailed script error:', {
      message: error.message,
      stack: error.stack,
      componentId,
      instanceId,
    });

    errorDiv.textContent = errorMessage;
    containerElement.appendChild(errorDiv);
  }

  private formatCodeForLog(code: string): string {
    // Truncate and format code for console logging
    if (!code) return 'Empty code';

    const maxLength = 200;
    if (code.length <= maxLength) return code;

    return code.substring(0, maxLength) + '... [truncated]';
  }

  /**
   * Process Angular-style template syntax including property and event bindings
   */
  private processAngularTemplate(html: string): string {
    if (!html) return '';

    // Replace [property]="expression" with data-bind-property="expression"
    let processedHtml = html.replace(
      /\[([^\]]+)\]="([^"]+)"/g,
      (match, property, value) => {
        // Skip componentId attributes in app-component-viewer tags as they're handled separately
        if (
          property === 'componentId' &&
          match.includes('app-component-viewer')
        ) {
          return match;
        }
        return `data-bind-${property}="${value}"`;
      }
    );

    // Replace (event)="handler()" with data-event-event="handler"
    processedHtml = processedHtml.replace(
      /\(([^\)]+)\)="([^"]+)"/g,
      'data-event-$1="$2"'
    );

    console.log(
      'Processed Angular template:',
      processedHtml.substring(0, 200) + '...'
    );

    return processedHtml;
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
      console.log('Loading Bootstrap CSS from CDN in component-viewer');
      const linkElement = document.createElement('link');
      linkElement.rel = 'stylesheet';
      linkElement.href = this.bootstrapCssLink;
      document.head.appendChild(linkElement);
    } else {
      console.log('Bootstrap CSS is already loaded in component-viewer');
    }

    // Check if Bootstrap JS is already loaded
    const existingScript = document.querySelector(
      `script[src="${this.bootstrapJsLink}"]`
    );

    if (!existingScript) {
      console.log('Loading Bootstrap JS from CDN in component-viewer');
      const scriptElement = document.createElement('script');
      scriptElement.src = this.bootstrapJsLink;
      document.body.appendChild(scriptElement);
    } else {
      console.log('Bootstrap JS is already loaded in component-viewer');
    }
  }
}
