import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink],
  template: `
    <div class="auth-container">
      <h2>Register</h2>
      @if (error()) {
        <div class="error">{{ error() }}</div>
      }
      <form (ngSubmit)="onSubmit()">
        <div class="form-group">
          <label for="name">Name</label>
          <input id="name" type="text" [(ngModel)]="name" name="name" required />
        </div>
        <div class="form-group">
          <label for="email">Email</label>
          <input id="email" type="email" [(ngModel)]="email" name="email" required />
        </div>
        <div class="form-group">
          <label for="password">Password</label>
          <input id="password" type="password" [(ngModel)]="password" name="password" required />
        </div>
        <button type="submit" class="btn-submit" [disabled]="loading()">
          {{ loading() ? 'Registering...' : 'Register' }}
        </button>
      </form>
      <p class="auth-link">Already have an account? <a routerLink="/login">Login</a></p>
    </div>
  `,
  styles: [`
    .auth-container { max-width: 400px; margin: 40px auto; padding: 24px; border: 1px solid #ddd; border-radius: 8px; }
    h2 { margin-bottom: 20px; }
    .form-group { margin-bottom: 16px; }
    .form-group label { display: block; margin-bottom: 4px; font-weight: 500; }
    .form-group input { width: 100%; padding: 8px; border: 1px solid #ccc; border-radius: 4px; box-sizing: border-box; }
    .btn-submit { width: 100%; padding: 10px; background: #1a1a2e; color: white; border: none; border-radius: 4px; cursor: pointer; font-size: 1rem; }
    .btn-submit:disabled { opacity: 0.6; }
    .error { background: #ffebee; color: #c62828; padding: 10px; border-radius: 4px; margin-bottom: 16px; }
    .auth-link { text-align: center; margin-top: 16px; }
  `],
})
export class RegisterComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  name = '';
  email = '';
  password = '';
  error = signal('');
  loading = signal(false);

  onSubmit(): void {
    this.loading.set(true);
    this.error.set('');
    this.auth.register(this.email, this.password, this.name).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/books']);
      },
      error: (err) => {
        this.loading.set(false);
        this.error.set(err.error?.error || 'Registration failed.');
      },
    });
  }
}
