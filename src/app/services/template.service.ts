import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Template, TemplateSite } from '../models/template.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class TemplateService {
  private apiUrl = environment.apiUrl || 'http://localhost:5019/api';

  constructor(private http: HttpClient) {}

  /**
   * Tüm template'leri getirir
   * /api/Templates
   */
  getTemplates(): Observable<Template[]> {
    return this.http.get<Template[]>(`${this.apiUrl}/Templates`).pipe(
      catchError((error) => {
        console.error('Template listesi getirilirken hata oluştu:', error);
        return of([]);
      })
    );
  }

  /**
   * Belirli bir template'i getirir
   * /api/Templates/{id}
   */
  getTemplate(id: number): Observable<Template> {
    return this.http.get<Template>(`${this.apiUrl}/Templates/${id}`).pipe(
      catchError((error) => {
        console.error(`Template ID: ${id} getirilirken hata oluştu:`, error);
        throw error;
      })
    );
  }

  /**
   * Template'in kullanıldığı siteleri getirir
   * /api/Templates/{id}/sites
   */
  getTemplateSites(templateId: number): Observable<TemplateSite[]> {
    return this.http
      .get<TemplateSite[]>(`${this.apiUrl}/Templates/${templateId}/sites`)
      .pipe(
        catchError((error) => {
          console.error(
            `Template ID: ${templateId} siteleri getirilirken hata oluştu:`,
            error
          );
          return of([]);
        })
      );
  }
}
