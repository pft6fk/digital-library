import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Author } from '../models/author.model';
import { Book, BookDetail } from '../models/book.model';
import { PagedResult } from '../models/paged-result.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CatalogService {
  constructor(private http: HttpClient) {}

  getAuthors(): Observable<Author[]> {
    return this.http.get<Author[]>(`${environment.catalogApiUrl}/authors`);
  }

  getAuthor(id: string): Observable<Author> {
    return this.http.get<Author>(`${environment.catalogApiUrl}/authors/${id}`);
  }

  getBooks(page: number, pageSize: number, authorId?: string): Observable<PagedResult<Book>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    if (authorId) params = params.set('authorId', authorId);
    return this.http.get<PagedResult<Book>>(`${environment.catalogApiUrl}/books`, { params });
  }

  getBook(id: string): Observable<BookDetail> {
    return this.http.get<BookDetail>(`${environment.catalogApiUrl}/books/${id}`);
  }

  createAuthor(data: { fullName: string; bio?: string; birthDate?: string }): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(`${environment.catalogApiUrl}/authors`, data);
  }

  createBook(data: { title: string; description?: string; authorId: string }): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(`${environment.catalogApiUrl}/books`, data);
  }
}
