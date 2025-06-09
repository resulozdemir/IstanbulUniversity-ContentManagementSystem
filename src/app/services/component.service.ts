import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import {
  Component,
  SiteComponent,
  SiteComponentData,
  ParsedComponentData,
  ComponentForm,
  ThemeComponent,
} from '../models/component.model';

@Injectable({
  providedIn: 'root',
})
export class ComponentService {
  private apiUrl = environment.apiUrl || 'http://localhost:5019/api';

  constructor(private http: HttpClient) {}

  /**
   * Belirli bir component'in HTML, CSS, JS kodlarını getirir
   * /api/Components/{id}
   */
  getComponent(componentId: number): Observable<Component> {
    console.log(
      '🔍 Component yükleniyor:',
      componentId,
      `${this.apiUrl}/Components/${componentId}`
    );
    return this.http
      .get<Component>(`${this.apiUrl}/Components/${componentId}`)
      .pipe(
        map((component) => {
          console.log('✅ Component başarıyla yüklendi:', component);
          return component;
        }),
        catchError((error) => {
          console.error(
            '❌ Component getirilirken hata oluştu:',
            componentId,
            error
          );
          console.error(
            '❌ API URL:',
            `${this.apiUrl}/Components/${componentId}`
          );
          console.error('❌ HTTP Status:', error.status);
          console.error('❌ Error Body:', error.error);
          return throwError(
            () => new Error(`Component ${componentId} bulunamadı`)
          );
        })
      );
  }

  /**
   * Belirli bir theme component'in HTML, CSS, JS kodlarını getirir
   * /api/Components/themecomponent/{id}
   */
  getThemeComponent(themeComponentId: number): Observable<ThemeComponent> {
    console.log(
      '🔍 Theme Component yükleniyor:',
      themeComponentId,
      `${this.apiUrl}/Components/themecomponent/${themeComponentId}`
    );
    return this.http
      .get<ThemeComponent>(
        `${this.apiUrl}/Components/themecomponent/${themeComponentId}`
      )
      .pipe(
        map((themeComponent) => {
          console.log('✅ Theme Component başarıyla yüklendi:', themeComponent);

          // javascript alanının boş olup olmadığını kontrol et
          if (!themeComponent.javascript) {
            console.warn(
              '⚠️ Theme Component için javascript alanı boş!',
              themeComponentId
            );
            // Boş bir değer yerine boş string atayalım
            themeComponent.javascript = '';
          } else {
            // JavaScript kodunu detaylı logla
            console.log(
              '🔎 JavaScript Kodu (ham hali):',
              themeComponent.javascript
            );
            console.log(
              '🔎 JavaScript Kodu (ilk 300 karakter):',
              themeComponent.javascript.substring(0, 300)
            );

            // Kod içinde beklenmeyen karakterleri kontrol et
            const suspiciousChars = themeComponent.javascript.match(
              /[^\w\s\(\)\{\}\[\]\.\,\;\:\'\"\`\+\-\*\/\%\&\|\!\=\<\>\?\@\#\$\^\\]/g
            );
            if (suspiciousChars && suspiciousChars.length > 0) {
              console.warn(
                '⚠️ JavaScript kodunda şüpheli karakterler bulundu:',
                [...new Set(suspiciousChars)]
              );
            }
          }

          return themeComponent;
        }),
        catchError((error) => {
          console.error(
            '❌ Theme Component getirilirken hata oluştu:',
            themeComponentId,
            error
          );
          console.error(
            '❌ API URL:',
            `${this.apiUrl}/Components/themecomponent/${themeComponentId}`
          );
          console.error('❌ HTTP Status:', error.status);
          console.error('❌ Error Body:', error.error);
          return throwError(
            () => new Error(`Theme Component ${themeComponentId} bulunamadı`)
          );
        })
      );
  }

  /**
   * Belirli bir sitenin componentlerini getirir (theme ilişkisi ile)
   * /api/Components/forsite/{siteId}
   */
  getComponentsForSite(siteId: number): Observable<SiteComponent[]> {
    console.log(
      '🔍 Site componentleri yükleniyor:',
      siteId,
      `${this.apiUrl}/Components/forsite/${siteId}`
    );
    return this.http
      .get<SiteComponent[]>(`${this.apiUrl}/Components/forsite/${siteId}`)
      .pipe(
        map((siteComponents) => {
          console.log(
            '✅ Site componentleri başarıyla yüklendi:',
            siteComponents
          );
          console.log(
            "📊 Site component ID'leri:",
            siteComponents.map((sc) => ({
              id: sc.id,
              themeComponentId: sc.themeComponentId,
              componentId: sc.componentId,
              componentName: sc.componentName,
            }))
          );
          return siteComponents;
        }),
        catchError((error) => {
          console.error(
            '❌ Site componentleri getirilirken hata oluştu:',
            siteId,
            error
          );
          return throwError(
            () => new Error(`Site ${siteId} için component bulunamadı`)
          );
        })
      );
  }

  /**
   * Belirli bir site component'inin kullandığı data'yı getirir
   * /api/Components/sitedata/{id}
   */
  getSiteComponentData(siteComponentId: number): Observable<SiteComponentData> {
    return this.http
      .get<SiteComponentData>(
        `${this.apiUrl}/Components/sitedata/${siteComponentId}`
      )
      .pipe(
        catchError((error) => {
          console.error(
            'Site component verisi getirilirken hata oluştu:',
            error
          );
          return throwError(
            () => new Error('Site component verisi bulunamadı')
          );
        })
      );
  }

  /**
   * Component verilerini analiz ederek kullanışlı hale getirir
   */
  parseComponentData(componentData: string): ParsedComponentData {
    try {
      return JSON.parse(componentData);
    } catch (error) {
      console.error('Component verisi parse edilirken hata oluştu:', error);
      return {}; // Hata durumunda boş nesne dön
    }
  }

  /**
   * Component form JSON'ını parse eder
   */
  parseComponentForm(formJson: string): ComponentForm {
    try {
      return JSON.parse(formJson);
    } catch (error) {
      console.error('Component form JSON parse edilirken hata oluştu:', error);
      return { fields: [] }; // Hata durumunda boş form dön
    }
  }

  /**
   * Site component'ini günceller
   */
  updateSiteComponent(
    siteComponentId: number,
    data: Partial<SiteComponentData>
  ): Observable<SiteComponentData> {
    return this.http
      .put<SiteComponentData>(
        `${this.apiUrl}/Components/sitedata/${siteComponentId}`,
        data
      )
      .pipe(
        catchError((error) => {
          console.error('Site component güncellenirken hata oluştu:', error);
          return throwError(() => new Error('Site component güncellenemedi'));
        })
      );
  }

  /**
   * Site component'inin data alanını günceller
   */
  updateSiteComponentData(
    siteComponentId: number,
    newData: ParsedComponentData
  ): Observable<SiteComponentData> {
    const dataString = JSON.stringify(newData);
    return this.updateSiteComponent(siteComponentId, {
      data: dataString,
      dataJson: dataString,
    });
  }

  /**
   * Debugging amaçlı - Component'in JavaScript kodunu ham haliyle döndürür
   */
  getComponentJavaScript(componentId: number): Observable<string> {
    return this.getThemeComponent(componentId).pipe(
      map((component) => {
        if (!component.javascript) {
          return '// Bu komponent için JavaScript kodu bulunmamaktadır.';
        }
        return component.javascript;
      }),
      catchError((error) => {
        console.error('JavaScript kodu alınırken hata:', error);
        return of('// Hata: JavaScript kodu alınamadı.');
      })
    );
  }
}
