import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Comment } from '../models/comment.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class EngagementService {
  constructor(private http: HttpClient) {}

  getComments(bookId: string): Observable<Comment[]> {
    const params = new HttpParams().set('bookId', bookId);
    return this.http.get<Comment[]>(`${environment.engagementApiUrl}/comments`, { params });
  }

  addComment(bookId: string, text: string): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(`${environment.engagementApiUrl}/comments`, { bookId, text });
  }
}
