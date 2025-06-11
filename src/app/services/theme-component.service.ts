import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from './api-url.token';

export interface ThemeComponent {
  id: number;
  themeId: number;
  componentId: number;
  name: string;
  description: string;
  template: string;
  style: string;
  javascript: string;
  formJson: string;
  formHtml: string;
  formJs: string;
  isDeleted: number;
}

export interface Theme {
  id: number;
  name: string;
  header: string | null;
  footer: string | null;
  isDeleted: number;
}

@Injectable({
  providedIn: 'root',
})
export class ThemeComponentService {
  constructor(
    private http: HttpClient,
    @Inject(API_URL) private apiUrl: string
  ) {}

  getThemeComponents(): Observable<ThemeComponent[]> {
    return this.http.get<ThemeComponent[]>(
      `${this.apiUrl}/api/Components/themecomponent`
    );
  }

  getThemes(): Observable<Theme[]> {
    return this.http.get<Theme[]>(`${this.apiUrl}/api/Themes`);
  }

  updateThemeComponent(id: number, component: ThemeComponent): Observable<any> {
    return this.http.put<any>(
      `${this.apiUrl}/api/Components/themecomponent/${id}`,
      component
    );
  }

  deleteThemeComponent(id: number): Observable<any> {
    return this.http.delete<any>(
      `${this.apiUrl}/api/Components/themecomponent/${id}`
    );
  }

  updateTheme(id: number, theme: Theme): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/api/Themes/${id}`, theme);
  }

  deleteTheme(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/api/Themes/${id}`);
  }

  createTheme(theme: Theme): Observable<Theme> {
    return this.http.post<Theme>(`${this.apiUrl}/api/Themes`, theme);
  }
}
