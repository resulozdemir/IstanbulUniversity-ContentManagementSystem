import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  BehaviorSubject,
  Observable,
  catchError,
  map,
  throwError,
  switchMap,
  tap,
} from 'rxjs';
import { Site } from '../models/site.model';
import { Template } from '../models/template.model';
import { Theme } from '../models/theme.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class SiteService {
  private apiUrl = environment.apiUrl || 'http://localhost:5019/api';

  // Aktif site, template ve tema için BehaviorSubject'ler
  private currentSiteSubject = new BehaviorSubject<Site | null>(null);
  private currentTemplateSubject = new BehaviorSubject<Template | null>(null);
  private currentThemeSubject = new BehaviorSubject<Theme | null>(null);

  // Observable'lar
  public currentSite$ = this.currentSiteSubject.asObservable();
  public currentTemplate$ = this.currentTemplateSubject.asObservable();
  public currentTheme$ = this.currentThemeSubject.asObservable();

  constructor(private http: HttpClient) {
    // Varsayılan olarak ilk siteyi yükle
    this.loadSites().subscribe((sites) => {
      if (sites && sites.length > 0) {
        this.selectSite(sites[0].id);
      }
    });
  }

  /**
   * Tüm siteleri API'den getirir
   * /api/Sites
   */
  loadSites(): Observable<Site[]> {
    console.log('🔍 Sites yükleniyor...', `${this.apiUrl}/Sites`);
    return this.http.get<Site[]>(`${this.apiUrl}/Sites`).pipe(
      tap((sites) => {
        console.log('✅ Sites başarıyla yüklendi:', sites);
      }),
      catchError((error) => {
        console.error('❌ Siteler yüklenirken hata oluştu:', error);
        return throwError(() => new Error('Siteler yüklenirken hata oluştu'));
      })
    );
  }

  /**
   * Belirli bir site ID'sine göre site bilgisini getirir
   * /api/Sites/{id}
   */
  getSite(id: number): Observable<Site> {
    console.log('🔍 Site yükleniyor:', id, `${this.apiUrl}/Sites/${id}`);
    return this.http.get<Site>(`${this.apiUrl}/Sites/${id}`).pipe(
      tap((site) => {
        console.log('✅ Site başarıyla yüklendi:', site);
      }),
      catchError((error) => {
        console.error(`❌ Site ID: ${id} yüklenirken hata oluştu:`, error);
        return throwError(() => new Error(`Site ID: ${id} bulunamadı`));
      })
    );
  }

  /**
   * Belirli bir site ID'sine göre siteyi seçer ve template ve temayı yükler
   */
  selectSite(siteId: number): void {
    console.log('🎯 Site seçiliyor:', siteId);
    this.getSite(siteId)
      .pipe(
        tap((site) => {
          if (site) {
            console.log(
              '✅ Site yüklendi, BehaviorSubject güncelleniyor:',
              site
            );
            console.log('🔍 Site objesi detayları:', {
              'site.templateId': site.templateId,
              'typeof templateId': typeof site.templateId,
              'templateId > 0': site.templateId && site.templateId > 0,
              'site tüm alanları': Object.keys(site),
            });
            this.currentSiteSubject.next(site);

            // Site için template'i yükle (eğer templateId varsa)
            console.log('🔍 Template ID kontrol ediliyor:', site.templateId);
            if (site.templateId && site.templateId > 0) {
              console.log('📄 Template yükleniyor:', site.templateId);
              this.getTemplate(site.templateId).subscribe({
                next: (template) => {
                  console.log('✅ Template yüklendi:', template);
                  this.currentTemplateSubject.next(template);
                },
                error: (error) => {
                  console.error('❌ Template yüklenirken hata:', error);
                  this.currentTemplateSubject.next(null);
                },
              });
            } else {
              console.log(
                '⚠️ Template ID yok veya 0, template yüklenmiyor. TemplateId:',
                site.templateId
              );
              this.currentTemplateSubject.next(null);
            }

            // Site için temayı yükle
            console.log('🎨 Theme ID kontrol ediliyor:', site.themeId);
            if (site.themeId) {
              console.log('🎨 Theme yükleniyor:', site.themeId);
              this.getTheme(site.themeId).subscribe({
                next: (theme) => {
                  console.log('✅ Theme yüklendi:', theme);
                  this.currentThemeSubject.next(theme);
                },
                error: (error) => {
                  console.error('❌ Theme yüklenirken hata:', error);
                  this.currentThemeSubject.next(null);
                },
              });
            } else {
              console.log('⚠️ Theme ID yok, theme yüklenmiyor');
              this.currentThemeSubject.next(null);
            }
          } else {
            console.error('❌ Site null geldi!');
            this.currentSiteSubject.next(null);
          }
        }),
        catchError((error) => {
          console.error(`❌ Site seçilirken hata oluştu:`, error);
          this.currentSiteSubject.next(null);
          return throwError(() => error);
        })
      )
      .subscribe({
        error: (error) => {
          console.error('❌ Site seçimi başarısız:', error);
        },
      });
  }

  /**
   * Belirli bir template ID'sine göre template bilgisini getirir
   * /api/Templates/{id}
   */
  getTemplate(id: number): Observable<Template> {
    console.log(
      '📄 Template yükleniyor:',
      id,
      `${this.apiUrl}/Templates/${id}`
    );
    return this.http.get<Template>(`${this.apiUrl}/Templates/${id}`).pipe(
      tap((template) => {
        console.log('✅ Template başarıyla yüklendi:', template);
      }),
      catchError((error) => {
        console.error(`❌ Template ID: ${id} yüklenirken hata oluştu:`, error);
        return throwError(() => new Error(`Template ID: ${id} bulunamadı`));
      })
    );
  }

  /**
   * Tüm template'leri API'den getirir
   * /api/Templates
   */
  getTemplates(): Observable<Template[]> {
    return this.http.get<Template[]>(`${this.apiUrl}/Templates`).pipe(
      catchError((error) => {
        console.error("Template'ler yüklenirken hata oluştu:", error);
        return throwError(
          () => new Error("Template'ler yüklenirken hata oluştu")
        );
      })
    );
  }

  /**
   * Belirli bir tema ID'sine göre tema bilgisini getirir
   * /api/Themes/{id}
   */
  getTheme(id: number): Observable<Theme> {
    console.log('🎨 Theme yükleniyor:', id, `${this.apiUrl}/Themes/${id}`);
    return this.http.get<Theme>(`${this.apiUrl}/Themes/${id}`).pipe(
      tap((theme) => {
        console.log('✅ Theme başarıyla yüklendi:', theme);
      }),
      catchError((error) => {
        console.error(`❌ Theme ID: ${id} yüklenirken hata oluştu:`, error);
        return throwError(() => new Error(`Theme ID: ${id} bulunamadı`));
      })
    );
  }

  /**
   * Tüm temaları API'den getirir
   * /api/Themes
   */
  getThemes(): Observable<Theme[]> {
    return this.http.get<Theme[]>(`${this.apiUrl}/Themes`).pipe(
      catchError((error) => {
        console.error('Temalar yüklenirken hata oluştu:', error);
        return throwError(() => new Error('Temalar yüklenirken hata oluştu'));
      })
    );
  }
}
