import { Injectable } from '@angular/core';
import {
  BehaviorSubject,
  Observable,
  catchError,
  of,
  map,
  switchMap,
} from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

export interface Site {
  id: number;
  name: string;
}

export interface Page {
  id: number;
  name: string;
  routing: string;
  siteId: number;
  siteName: string;
  html: string;
  style: string;
  javascript: string;
  metaTitle: string | null;
  metaDescription: string | null;
  metaKeywords: string | null;
  isDeleted: number;
  showInMenu: boolean;
  menuOrder: number | null;
  layout: string | null;
  parentId: number | null;
  parentName: string | null;
  createdDate: string;
  modifiedDate: string | null;
  childPages: Page[];
}

export interface PageComponent {
  componentId: number;
  selector: string;
}

export interface ComponentData {
  id: string;
  name: string;
  description: string;
  html: string;
  css: string;
  js: string;
  data?: any;
  template?: string;
  style?: string;
  javascript?: string;
  formJson?: string;
  formHtml?: string;
  formJs?: string;
  themeId?: number;
  componentId?: number;
}

export interface ComponentSiteData {
  id: number;
  siteId: number;
  themeComponentId: number;
  componentId: number | null;
  componentName: string;
  data: string;
  isDeleted: number;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class PageService {
  // Environment'tan API URL'ini al
  private baseUrl = environment.apiUrl.endsWith('/api')
    ? environment.apiUrl.substring(0, environment.apiUrl.length - 4)
    : environment.apiUrl;

  private currentSiteSubject = new BehaviorSubject<Site | null>(null);
  public currentSite$ = this.currentSiteSubject.asObservable();

  private currentPageSubject = new BehaviorSubject<Page | null>(null);
  public currentPage$ = this.currentPageSubject.asObservable();

  private sitesSubject = new BehaviorSubject<Site[]>([]);
  public sites$ = this.sitesSubject.asObservable();

  private pagesSubject = new BehaviorSubject<Page[]>([]);
  public pages$ = this.pagesSubject.asObservable();

  constructor(private http: HttpClient) {
    console.log('PageService initialized with base URL:', this.baseUrl);
  }

  /**
   * API'den tüm siteleri getirir
   */
  getSites(): Observable<Site[]> {
    const url = `${this.baseUrl}/api/Sites`;
    console.log('Fetching sites from URL:', url);
    return this.http.get<Site[]>(url).pipe(
      catchError((error) => {
        console.error('Siteler yüklenirken hata oluştu:', error);
        return of([]);
      })
    );
  }

  /**
   * Siteleri yükler ve cache'ler
   */
  loadSites(): void {
    this.getSites().subscribe((sites) => {
      this.sitesSubject.next(sites);

      // Eğer site varsa ve seçili site yoksa, ilk siteyi seç
      if (sites.length > 0 && !this.currentSiteSubject.value) {
        this.selectSite(sites[0]);
      }
    });
  }

  /**
   * Bir siteyi seçer ve sayfalarını yükler
   */
  selectSite(site: Site): void {
    console.log('Selecting site:', site);

    if (!site || !site.id) {
      console.error('Invalid site selected:', site);
      return;
    }

    // Önce mevcut siteyi güncelle
    this.currentSiteSubject.next(site);

    // Site ID ile sayfaları yükle
    this.loadPagesForSite(site.id);
  }

  /**
   * Site ID'sine göre sayfaları getirir
   */
  getPagesForSite(siteId: number): Observable<Page[]> {
    const url = `${this.baseUrl}/api/Pages?siteId=${siteId}`;
    console.log('Fetching pages for site ID:', siteId, 'URL:', url);

    return this.http.get(url, { observe: 'response' }).pipe(
      map((response) => {
        console.log('Raw API Response Headers:', response.headers);
        console.log('Raw API Response Status:', response.status);
        console.log('Raw API Response Body:', response.body);

        if (Array.isArray(response.body)) {
          return response.body as Page[];
        } else if (response.body && typeof response.body === 'object') {
          // Eğer body bir dizi değilse, ama nesne ise, sayfaları içeren bir alan olabilir
          const possiblePages = Object.values(response.body);
          if (Array.isArray(possiblePages[0])) {
            return possiblePages[0] as Page[];
          }
          // Doğrudan nesneyi dizi olarak dön
          return [response.body as unknown as Page];
        }
        return [] as Page[];
      }),
      catchError((error) => {
        console.error(
          `Site ID: ${siteId} için sayfalar yüklenirken hata oluştu:`,
          error
        );
        return of([]);
      })
    );
  }

  /**
   * Site için sayfaları yükler ve cache'ler
   */
  loadPagesForSite(siteId: number): void {
    console.log('Loading pages for site ID:', siteId);
    this.getPagesForSite(siteId).subscribe((pages) => {
      console.log(
        'Received pages for site ID:',
        siteId,
        'Pages count:',
        pages.length,
        'Pages:',
        pages
      );
      this.pagesSubject.next(pages);

      // Ana sayfayı bul (routing === '/' olan)
      const homePage = pages.find((p) => p.routing === '/');
      console.log('Found homepage:', homePage);

      // Ana sayfa varsa seç, yoksa ilk sayfayı seç
      if (homePage) {
        this.selectPage(homePage.id);
      } else if (pages.length > 0) {
        this.selectPage(pages[0].id);
      } else {
        console.warn('No pages found for site ID:', siteId);
        this.currentPageSubject.next(null);
      }
    });
  }

  /**
   * Sayfa ID'sine göre sayfa detaylarını getirir
   */
  getPageById(pageId: number): Observable<Page> {
    const url = `${this.baseUrl}/api/Pages/${pageId}`;
    console.log('Fetching page details for ID:', pageId, 'URL:', url);
    return this.http.get<Page>(url).pipe(
      catchError((error) => {
        console.error(`Sayfa ID: ${pageId} yüklenirken hata oluştu:`, error);
        return of(null as unknown as Page);
      })
    );
  }

  /**
   * Sayfa ID'sine göre sayfayı seçer ve detaylarını yükler
   */
  selectPage(pageId: number): void {
    this.getPageById(pageId).subscribe((page) => {
      if (page) {
        this.currentPageSubject.next(page);
      }
    });
  }

  /**
   * Sayfa HTML içeriğinden component ID'lerini ayıklar
   */
  extractComponents(html: string): PageComponent[] {
    if (!html) {
      return [];
    }

    const components: PageComponent[] = [];

    // Angular syntax: <app-component-viewer [componentId]="1"></app-component-viewer>
    const angularRegex =
      /<app-component-viewer\s+\[componentId\]="(\d+)"[^>]*><\/app-component-viewer>/g;
    let match;
    while ((match = angularRegex.exec(html)) !== null) {
      components.push({
        componentId: parseInt(match[1], 10),
        selector: match[0],
      });
    }

    // Regular HTML syntax: <app-component-viewer componentId="1"></app-component-viewer>
    const htmlRegex =
      /<app-component-viewer\s+componentId="(\d+)"[^>]*><\/app-component-viewer>/g;
    while ((match = htmlRegex.exec(html)) !== null) {
      components.push({
        componentId: parseInt(match[1], 10),
        selector: match[0],
      });
    }

    // Log found components
    console.log('Extracted components from HTML:', components);

    return components;
  }

  /**
   * Component ID'sine göre component verilerini getirir
   * ComponentViewerComponent tarafından kullanılır
   */
  getComponentById(componentId: string): Observable<ComponentData> {
    const url = `${this.baseUrl}/api/Components/themecomponent/${componentId}`;
    console.log('Fetching theme component with ID:', componentId, 'URL:', url);

    // API'den component template verilerini getir
    return this.http.get<any>(url).pipe(
      switchMap((componentTemplate) => {
        // Component template bilgilerini aldıktan sonra, ilgili site verisini almaya çalışalım
        return this.getComponentSiteData(componentId).pipe(
          map((siteData) => {
            // Site verisi varsa ve component verisini içeriyorsa, birleştir
            const componentData: ComponentData = {
              id: componentTemplate.id.toString(),
              name: componentTemplate.name,
              description: componentTemplate.description,
              // HTML, CSS ve JS için template, style ve javascript kullanıyoruz
              html: componentTemplate.template || '',
              css: componentTemplate.style || '',
              js: componentTemplate.javascript || '',
              // Orijinal alanları da koru
              template: componentTemplate.template,
              style: componentTemplate.style,
              javascript: componentTemplate.javascript,
              formJson: componentTemplate.formJson,
              themeId: componentTemplate.themeId,
              componentId: componentTemplate.componentId,
            };

            // Site verisinden data alanını işle
            if (siteData) {
              try {
                // Data string'se, JSON olarak parse et
                if (typeof siteData.data === 'string') {
                  componentData.data = JSON.parse(siteData.data);
                } else {
                  componentData.data = siteData.data;
                }
                console.log('Component data parsed:', componentData.data);
              } catch (e) {
                console.error('Error parsing component data:', e);
                componentData.data = {};
              }
            }

            return componentData;
          })
        );
      }),
      catchError((error) => {
        console.error(
          `Component ID: ${componentId} yüklenirken hata oluştu:`,
          error
        );
        // Hata durumunda varsayılan component verileri dön
        return of(this.createDefaultComponent(componentId));
      })
    );
  }

  /**
   * Component ID'sine göre site verisini getirir
   */
  getComponentSiteData(
    componentId: string
  ): Observable<ComponentSiteData | null> {
    const url = `${this.baseUrl}/api/Components/sitedata/${componentId}`;
    console.log(
      'Fetching component site data with ID:',
      componentId,
      'URL:',
      url
    );

    return this.http.get<ComponentSiteData>(url).pipe(
      catchError((error) => {
        console.warn(
          `Component site data for ID: ${componentId} not found:`,
          error
        );
        return of(null);
      })
    );
  }

  /**
   * Varsayılan bir component verisi oluşturur
   */
  private createDefaultComponent(componentId: string): ComponentData {
    return {
      id: componentId,
      name: `Component ${componentId}`,
      description: `Bu bir varsayılan component açıklamasıdır (${componentId})`,
      html: `<div class="component-${componentId}">Example HTML content</div>`,
      css: `.component-${componentId} { padding: 15px; border: 1px solid #eee; }`,
      js: `console.log('Component ${componentId} loaded');`,
    };
  }
}
