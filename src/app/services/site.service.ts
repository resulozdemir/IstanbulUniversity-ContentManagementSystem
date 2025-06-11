import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Site } from '../models/site.model';

@Injectable({
  providedIn: 'root',
})
export class SiteService {
  private readonly apiUrl = 'http://localhost:5019/api';

  constructor(private http: HttpClient) {}

  /**
   * Get all sites from the API
   */
  getSites(): Observable<Site[]> {
    return this.http.get<Site[]>(`${this.apiUrl}/Sites`);
  }

  /**
   * Get a single site by ID
   */
  getSite(id: number): Observable<Site> {
    return this.http.get<Site>(`${this.apiUrl}/Sites/${id}`);
  }

  /**
   * Create a new site
   */
  createSite(site: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/Sites`, site);
  }

  /**
   * Update an existing site
   */
  updateSite(id: number, site: any): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/Sites/${id}`, site);
  }

  /**
   * Delete a site
   */
  deleteSite(id: number): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/Sites/${id}`);
  }
}
