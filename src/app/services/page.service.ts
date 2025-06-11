import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { Page } from '../models/page.model';
import { PageRequest } from '../models/page-request.model';

@Injectable({
  providedIn: 'root',
})
export class PageService {
  private readonly apiUrl = 'http://localhost:5019/api';

  constructor(private http: HttpClient) {}

  /**
   * Get pages for a specific site
   */
  getPagesBySiteId(siteId: number): Observable<Page[]> {
    return this.http.get<Page[]>(`${this.apiUrl}/Pages?siteId=${siteId}`).pipe(
      catchError((error) => {
        console.error('Error fetching pages:', error);
        return of([]);
      })
    );
  }

  /**
   * Get a page by its ID
   */
  getPageById(id: number): Observable<Page> {
    return this.http.get<Page>(`${this.apiUrl}/Pages/${id}`).pipe(
      tap((page) => console.log('Fetched page:', page)),
      catchError((error) => {
        console.error('Error fetching page:', error);
        console.log('Error details:', {
          status: error.status,
          statusText: error.statusText,
          error: error.error,
        });
        throw error;
      })
    );
  }

  /**
   * Create a new page
   */
  createPage(pageRequest: PageRequest): Observable<any> {
    // Generate a unique 8-digit ID if not provided
    if (!pageRequest.id) {
      pageRequest.id = this.generateUniqueId();
    }

    // Ensure showInMenu is properly set as a boolean
    const processedRequest = {
      ...pageRequest,
      showInMenu: pageRequest.showInMenu === true,
    };

    // Log the full request payload
    console.log(
      'Creating page with payload:',
      JSON.stringify(processedRequest, null, 2)
    );
    console.log(
      'showInMenu type:',
      typeof processedRequest.showInMenu,
      'value:',
      processedRequest.showInMenu
    );

    return this.http.post(`${this.apiUrl}/Pages`, processedRequest).pipe(
      tap((response) => {
        console.log('API Response:', response);
        console.log(
          'Response showInMenu type:',
          typeof (response as any).showInMenu,
          'value:',
          (response as any).showInMenu
        );
      }),
      catchError((error) => {
        console.error('Error creating page:', error);
        console.log('Error details:', {
          status: error.status,
          statusText: error.statusText,
          error: error.error,
        });
        throw error;
      })
    );
  }

  /**
   * Update an existing page
   */
  updatePage(id: number, pageRequest: PageRequest): Observable<any> {
    // Ensure showInMenu is properly set as a boolean
    const processedRequest = {
      ...pageRequest,
      showInMenu: pageRequest.showInMenu === true,
    };

    // Log the full request payload
    console.log(
      'Updating page with payload:',
      JSON.stringify(processedRequest, null, 2)
    );
    console.log(
      'showInMenu type:',
      typeof processedRequest.showInMenu,
      'value:',
      processedRequest.showInMenu
    );

    return this.http.put(`${this.apiUrl}/Pages/${id}`, processedRequest).pipe(
      tap((response) => {
        console.log('Update response:', response);
        console.log(
          'Response showInMenu type:',
          typeof (response as any).showInMenu,
          'value:',
          (response as any).showInMenu
        );
      }),
      catchError((error) => {
        console.error('Error updating page:', error);
        console.log('Error details:', {
          status: error.status,
          statusText: error.statusText,
          error: error.error,
        });
        throw error;
      })
    );
  }

  /**
   * Delete a page by its ID
   */
  deletePage(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/Pages/${id}`).pipe(
      tap((response) => console.log('Delete response:', response)),
      catchError((error) => {
        console.error('Error deleting page:', error);
        console.log('Error details:', {
          status: error.status,
          statusText: error.statusText,
          error: error.error,
        });
        throw error;
      })
    );
  }

  /**
   * Generate a unique 8-digit ID for a page
   */
  private generateUniqueId(): number {
    // Generate a random number between 10000000 and 99999999 (8 digits)
    return Math.floor(10000000 + Math.random() * 90000000);
  }
}
