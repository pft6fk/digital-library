import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { CatalogService } from '../../../core/services/catalog.service';
import { Author } from '../../../core/models/author.model';

@Component({
  selector: 'app-authors-list',
  standalone: true,
  imports: [RouterLink, DatePipe],
  template: `
    <h2>Authors</h2>
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
  authors = signal<Author[]>([]);
  loading = signal(true);

  ngOnInit(): void {
    this.catalog.getAuthors().subscribe({
      next: (data) => { this.authors.set(data); this.loading.set(false); },
      error: () => this.loading.set(false),
    });
  }
}
