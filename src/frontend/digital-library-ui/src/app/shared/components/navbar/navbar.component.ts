import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink],
  template: `
    <nav class="navbar">
      <div class="navbar-brand">
        <a routerLink="/books" class="brand-link">Digital Library</a>
      </div>
      <div class="navbar-links">
        <a routerLink="/books" class="nav-link">Books</a>
        <a routerLink="/authors" class="nav-link">Authors</a>
      </div>
      <div class="navbar-auth">
        @if (auth.isAuthenticated()) {
          <span class="user-name">{{ auth.currentUser()?.name }}</span>
          <button class="btn btn-logout" (click)="auth.logout()">Logout</button>
        } @else {
          <a routerLink="/login" class="btn btn-login">Login</a>
          <a routerLink="/register" class="btn btn-register">Register</a>
        }
      </div>
    </nav>
  `,
  styles: [`
    .navbar {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 12px 24px;
      background: #1a1a2e;
      color: white;
    }
    .brand-link { color: white; text-decoration: none; font-size: 1.3rem; font-weight: bold; }
    .navbar-links { display: flex; gap: 16px; }
    .nav-link { color: #ccc; text-decoration: none; }
    .nav-link:hover { color: white; }
    .navbar-auth { display: flex; align-items: center; gap: 12px; }
    .user-name { color: #a5d6a7; }
    .btn { padding: 6px 14px; border: none; border-radius: 4px; cursor: pointer; text-decoration: none; font-size: 0.9rem; }
    .btn-login { background: #4fc3f7; color: #000; }
    .btn-register { background: #81c784; color: #000; }
    .btn-logout { background: #ef5350; color: white; }
  `],
})
export class NavbarComponent {
  auth = inject(AuthService);
}
