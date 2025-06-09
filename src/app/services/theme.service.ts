import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Theme } from '../models/theme.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private apiUrl = environment.apiUrl || 'http://localhost:5019/api';

  constructor(private http: HttpClient) {}

  /**
   * Tüm temaları getirir
   * /api/Themes
   */
  getThemes(): Observable<Theme[]> {
    return this.http.get<Theme[]>(`${this.apiUrl}/Themes`).pipe(
      catchError((error) => {
        console.error('Tema listesi getirilirken hata oluştu:', error);
        return of([]);
      })
    );
  }

  /**
   * Belirli bir temayı getirir
   * /api/Themes/{id}
   */
  getTheme(id: number): Observable<Theme> {
    return this.http.get<Theme>(`${this.apiUrl}/Themes/${id}`).pipe(
      catchError((error) => {
        console.error(`Theme ID: ${id} getirilirken hata oluştu:`, error);
        throw error;
      })
    );
  }
}
