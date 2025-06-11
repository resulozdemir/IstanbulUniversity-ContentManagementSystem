import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from './api-url.token';

export interface Component {
  id: number;
  name: string;
  description: string;
  template: string | null;
  style: string | null;
  formjson: string | null;
  tagName: string | null;
  column1: string | null;
  column2: string | null;
  column3: string | null;
  column4: string | null;
}

export interface ThemeComponent {
  id: number;
  themeId: number;
  componentId: number;
  name: string;
  description: string;
  template: string;
  style: string;
  javascript: string;
  formJson: string | null;
  formHtml: string | null;
  formJs: string | null;
  isDeleted: number;
}

@Injectable({
  providedIn: 'root',
})
export class ComponentService {
  constructor(
    private http: HttpClient,
    @Inject(API_URL) private apiUrl: string
  ) {}

  createComponent(component: Component): Observable<any> {
    console.log('Creating component with data:', component);
    return this.http.post<any>(`${this.apiUrl}/api/Components`, component);
  }

  createThemeComponent(themeComponent: ThemeComponent): Observable<any> {
    console.log('Creating theme component with data:', themeComponent);
    return this.http.post<any>(
      `${this.apiUrl}/api/Components/themecomponent`,
      themeComponent
    );
  }
}
