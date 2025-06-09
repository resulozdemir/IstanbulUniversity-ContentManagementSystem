import { Injectable } from '@angular/core';
import {
  BehaviorSubject,
  Observable,
  map,
  catchError,
  throwError,
  of,
  forkJoin,
} from 'rxjs';
import { ComponentService } from './component.service';
import { SiteService } from './site.service';
import {
  Component,
  SiteComponent,
  ThemeComponent,
  ParsedComponentData,
} from '../models/component.model';
import { Site } from '../models/site.model';

interface Page {
  id: string;
  title: string;
  description: string;
  components: string[];
  siteId?: number;
}

interface ComponentData {
  id: string;
  name: string;
  description: string;
  html: string;
  css: string;
  js: string;
  data?: ParsedComponentData;
}

/**
 * Site bileşenlerini önbelleğe almak için
 */
interface CachedComponent {
  themeComponentId: number;
  siteComponentId: number;
  componentName: string;
  data: ParsedComponentData;
}

@Injectable({
  providedIn: 'root',
})
export class PageService {
  private currentPageSubject = new BehaviorSubject<Page | null>(null);
  public currentPage$ = this.currentPageSubject.asObservable();

  // Önbellek mekanizması
  private componentCache: Map<string, CachedComponent> = new Map();

  constructor(
    private componentService: ComponentService,
    private siteService: SiteService
  ) {}

  /**
   * Site ID'sine göre sayfa yükler
   */
  loadPageForSite(siteId: number): void {
    this.siteService.getSite(siteId).subscribe({
      next: (site) => {
        const page: Page = {
          id: `site-${siteId}`,
          title: site.name,
          description: `${site.name} için sayfa içeriği`,
          components: [], // API'den component ID'leri gelecek
          siteId: siteId,
        };

        // Site için component'leri yükle
        this.loadComponentsForSite(siteId, page);
      },
      error: (error) => {
        console.error(`Site ${siteId} yüklenirken hata:`, error);
        this.currentPageSubject.next(null);
      },
    });
  }

  /**
   * Site için component'leri yükler
   */
  private loadComponentsForSite(siteId: number, page: Page): void {
    this.componentService.getComponentsForSite(siteId).subscribe({
      next: (siteComponents) => {
        // Site component'lerinden component ID'lerini al ve önbelleğe at
        const componentIds = siteComponents
          .filter((sc) => sc.themeComponentId)
          .map((sc) => {
            const themeComponentId = sc.themeComponentId.toString();

            // Component verilerini önbelleğe al
            this.cacheComponentData(themeComponentId, {
              themeComponentId: sc.themeComponentId,
              siteComponentId: sc.id,
              componentName: sc.componentName,
              data: this.parseComponentData(sc.data),
            });

            return themeComponentId;
          });

        page.components = componentIds;
        this.currentPageSubject.next(page);
      },
      error: (error) => {
        console.error(`Site ${siteId} component'leri yüklenirken hata:`, error);
        this.currentPageSubject.next(page);
      },
    });
  }

  /**
   * Component verilerini önbelleğe alır
   */
  private cacheComponentData(
    themeComponentId: string,
    data: CachedComponent
  ): void {
    console.log(`🔄 Component önbelleğe alınıyor: #${themeComponentId}`, data);
    this.componentCache.set(themeComponentId, data);
  }

  /**
   * Component verilerini JSON formatından nesneye dönüştürür
   * İç içe JSON string'leri de otomatik olarak parse eder
   */
  private parseComponentData(dataString: string): ParsedComponentData {
    try {
      if (!dataString) return {};

      // Ana veriyi parse et
      const parsedData = JSON.parse(dataString);

      // Özyinelemeli olarak tüm alt JSON string'leri parse et
      this.deepParseJson(parsedData);

      return parsedData;
    } catch (error) {
      console.error('Component verisi parse edilirken hata:', error);
      return {}; // Hata durumunda boş nesne dön
    }
  }

  /**
   * Nesne içindeki tüm JSON string'leri özyinelemeli olarak parse eder
   */
  private deepParseJson(obj: any): void {
    if (!obj || typeof obj !== 'object') return;

    Object.keys(obj).forEach((key) => {
      const value = obj[key];

      // String değerleri JSON olarak parse etmeyi dene
      if (typeof value === 'string') {
        if (
          (value.startsWith('{') && value.endsWith('}')) ||
          (value.startsWith('[') && value.endsWith(']'))
        ) {
          try {
            const parsed = JSON.parse(value);
            obj[key] = parsed;

            // Parse edilen değer de bir nesne ise, özyinelemeli olarak işlemeye devam et
            this.deepParseJson(parsed);
          } catch (e) {
            // JSON parse hatası - bu bir JSON string değil, normal bir string
          }
        }
      }
      // Nesne veya dizi ise, özyinelemeli olarak alt elemanları işle
      else if (typeof value === 'object' && value !== null) {
        this.deepParseJson(value);
      }
    });
  }

  /**
   * Subdomain'e göre sayfa yükler
   */
  loadPageBySubdomain(subdomain: string): void {
    // Subdomain'i site ID'sine dönüştür
    let siteId: number;

    switch (subdomain) {
      case 'www':
        siteId = 301; // API'deki ABC Kurumsal
        break;
      case 'mdbf':
        siteId = 302; // XYZ Mağaza
        break;
      case 'fen':
        siteId = 201; // Kurumsal Site Şablonu
        break;
      case 'tip':
        siteId = 202; // E-Ticaret Sitesi Şablonu
        break;
      case 'iletisim':
        siteId = 203; // Blog Sitesi Şablonu
        break;
      default:
        siteId = 301; // Varsayılan site
    }

    this.loadPageForSite(siteId);
  }

  /**
   * Sayfa ID'sine göre sayfa yükler
   */
  loadPageById(pageId: string): void {
    // PageId'den siteId çıkar
    const siteIdMatch = pageId.match(/site-(\d+)/);
    if (siteIdMatch) {
      const siteId = parseInt(siteIdMatch[1]);
      this.loadPageForSite(siteId);
    } else {
      console.error(`Invalid page ID: ${pageId}`);
    }
  }

  /**
   * Component ID'sine göre component verilerini getirir (her seferinde fresh API call)
   */
  getComponentById(componentId: string): Observable<ComponentData | null> {
    console.log("📡 Component API'den yükleniyor (no cache):", componentId);

    const numericId = parseInt(componentId);
    if (!isNaN(numericId)) {
      // Önbellekte bu component var mı kontrol et
      const cachedComponent = this.componentCache.get(componentId);

      // ThemeComponent'i çek (HTML/CSS/JS için)
      return this.componentService.getThemeComponent(numericId).pipe(
        map((themeComponent) => {
          // Eğer javascript alanı undefined veya null ise boş string olarak ayarla
          const jsCode = themeComponent.javascript || '';
          console.log(
            `📝 Theme Component ${componentId} JavaScript:`,
            jsCode.substring(0, 100) + (jsCode.length > 100 ? '...' : '')
          );

          // Component datasını önbellekten al veya boş obje oluştur
          const componentData = cachedComponent ? cachedComponent.data : {};

          console.log(`📊 Component data: ${componentId}`, componentData);

          const result: ComponentData = {
            id: themeComponent.id.toString(),
            name: themeComponent.name,
            description: themeComponent.description,
            html: themeComponent.template,
            css: themeComponent.style,
            js: jsCode,
            data: componentData,
          };

          console.log('✅ Component data başarıyla hazırlandı:', componentId);
          return result;
        }),
        catchError((error) => {
          console.error('❌ Theme Component yüklenemedi:', componentId, error);
          return throwError(
            () => new Error(`Theme Component ${componentId} bulunamadı`)
          );
        })
      );
    }

    console.log('❌ Geçersiz component ID:', componentId);
    return throwError(() => new Error(`Geçersiz component ID: ${componentId}`));
  }
}
