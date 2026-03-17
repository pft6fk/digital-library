import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { AuthResponse, AuthUser } from '../models/user.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly _currentUser = signal<AuthUser | null>(null);
  readonly currentUser = this._currentUser.asReadonly();
  readonly isAuthenticated = computed(() => this._currentUser() !== null);

  constructor(private http: HttpClient) {
    this.rehydrate();
  }

  private rehydrate(): void {
    const token = localStorage.getItem('access_token');
    if (token) {
      try {
        const payload = JSON.parse(atob(token.split('.')[1]));
        const exp = payload.exp * 1000;
        if (Date.now() < exp) {
          this._currentUser.set({
            userId: payload.sub,
            email: payload.email,
            name: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || payload.name || '',
          });
        } else {
          localStorage.removeItem('access_token');
        }
      } catch {
        localStorage.removeItem('access_token');
      }
    }
  }

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.identityApiUrl}/auth/login`, { email, password }).pipe(
      tap(res => {
        localStorage.setItem('access_token', res.accessToken);
        this._currentUser.set({ userId: res.userId, email: res.email, name: res.name });
      })
    );
  }

  register(email: string, password: string, name: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.identityApiUrl}/auth/register`, { email, password, name }).pipe(
      tap(res => {
        localStorage.setItem('access_token', res.accessToken);
        this._currentUser.set({ userId: res.userId, email: res.email, name: res.name });
      })
    );
  }

  logout(): void {
    localStorage.removeItem('access_token');
    this._currentUser.set(null);
  }
}
