import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { CatalogService } from '../../../core/services/catalog.service';
import { AuthService } from '../../../core/auth/auth.service';
import { Author } from '../../../core/models/author.model';

@Component({
  selector: 'app-authors-list',
  standalone: true,
  imports: [RouterLink, DatePipe, FormsModule],
  template: `
    <h2>Authors</h2>

    @if (auth.isAuthenticated()) {
      <div class="create-form">
        <h3>{{ showForm() ? 'Add New Author' : '' }}</h3>
        @if (!showForm()) {
          <button class="btn-add" (click)="showForm.set(true)">+ Add Author</button>
        } @else {
          <form (ngSubmit)="onCreateAuthor()">
            <div class="form-row">
              <input type="text" [(ngModel)]="newAuthor.fullName" name="fullName" placeholder="Full Name" required />
              <input type="date" [(ngModel)]="newAuthor.birthDate" name="birthDate" />
            </div>
            <textarea [(ngModel)]="newAuthor.bio" name="bio" placeholder="Bio (optional)" rows="2"></textarea>
            <div class="form-actions">
              <button type="submit" class="btn-submit">Create</button>
              <button type="button" class="btn-cancel" (click)="showForm.set(false)">Cancel</button>
            </div>
          </form>
        }
      </div>
    }

    @if (loading()) {
      <p>Loading authors...</p>
    } @else {
      <div class="authors-grid">
        @for (author of authors(); track author.id) {
          <div class="author-card">
            <h3>{{ author.fullName }}</h3>
            @if (author.birthDate) {
              <p class="birth-date">Born: {{ author.birthDate | date:'mediumDate' }}</p>
            }
            @if (author.bio) {
              <p class="bio">{{ author.bio }}</p>
            }
            <a [routerLink]="['/books']" [queryParams]="{ authorId: author.id }" class="books-link">
              View Books
            </a>
          </div>
        } @empty {
          <p>No authors found.</p>
        }
      </div>
    }
  `,
  styles: [`
    h2 { margin-bottom: 20px; }
    .create-form { margin-bottom: 24px; padding: 16px; border: 1px solid #ddd; border-radius: 8px; background: #f9f9f9; }
    .create-form h3 { margin: 0 0 12px; }
    .form-row { display: flex; gap: 12px; margin-bottom: 8px; }
    .form-row input { flex: 1; padding: 8px; border: 1px solid #ccc; border-radius: 4px; }
    .create-form textarea { width: 100%; padding: 8px; border: 1px solid #ccc; border-radius: 4px; box-sizing: border-box; resize: vertical; }
    .form-actions { display: flex; gap: 8px; margin-top: 8px; }
    .btn-add { padding: 8px 16px; background: #1a1a2e; color: white; border: none; border-radius: 4px; cursor: pointer; }
    .btn-submit { padding: 8px 16px; background: #1a1a2e; color: white; border: none; border-radius: 4px; cursor: pointer; }
    .btn-cancel { padding: 8px 16px; background: #ccc; border: none; border-radius: 4px; cursor: pointer; }
    .authors-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); gap: 16px; }
    .author-card { border: 1px solid #ddd; border-radius: 8px; padding: 16px; }
    .author-card h3 { margin: 0 0 8px; }
    .birth-date { color: #666; font-size: 0.85rem; }
    .bio { color: #444; font-size: 0.9rem; margin: 8px 0; }
    .books-link { color: #1a1a2e; font-weight: 500; }
  `],
})
export class AuthorsListComponent implements OnInit {
  private catalog = inject(CatalogService);
  auth = inject(AuthService);
  authors = signal<Author[]>([]);
  loading = signal(true);
  showForm = signal(false);
  newAuthor = { fullName: '', bio: '', birthDate: '' };

  ngOnInit(): void {
    this.loadAuthors();
  }

  loadAuthors(): void {
    this.catalog.getAuthors().subscribe({
      next: (data) => { this.authors.set(data); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }

  onCreateAuthor(): void {
    if (!this.newAuthor.fullName.trim()) return;
    this.catalog.createAuthor({
      fullName: this.newAuthor.fullName,
      bio: this.newAuthor.bio || undefined,
      birthDate: this.newAuthor.birthDate || undefined,
    }).subscribe({
      next: () => {
        this.newAuthor = { fullName: '', bio: '', birthDate: '' };
        this.showForm.set(false);
        this.loadAuthors();
      },
    });
  }
}
