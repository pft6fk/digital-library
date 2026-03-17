import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CatalogService } from '../../../core/services/catalog.service';
import { Book } from '../../../core/models/book.model';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-books-list',
  standalone: true,
  imports: [RouterLink, PaginationComponent],
  template: `
    <h2>Books</h2>
    @if (loading()) {
      <p>Loading books...</p>
    } @else {
      <div class="books-grid">
        @for (book of books(); track book.id) {
          <a [routerLink]="['/books', book.id]" class="book-card">
            <h3>{{ book.title }}</h3>
            @if (book.description) {
              <p class="description">{{ book.description }}</p>
            }
          </a>
        } @empty {
          <p>No books found.</p>
        }
      </div>
      @if (totalPages() > 1) {
        <app-pagination
          [currentPage]="currentPage()"
          [totalPages]="totalPages()"
          (pageChange)="onPageChange($event)"
        />
      }
    }
  `,
  styles: [`
    h2 { margin-bottom: 20px; }
    .books-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); gap: 16px; }
    .book-card { border: 1px solid #ddd; border-radius: 8px; padding: 16px; text-decoration: none; color: inherit; display: block; transition: box-shadow 0.2s; }
    .book-card:hover { box-shadow: 0 2px 8px rgba(0,0,0,0.12); }
    .book-card h3 { margin: 0 0 8px; color: #1a1a2e; }
    .description { color: #666; font-size: 0.9rem; overflow: hidden; display: -webkit-box; -webkit-line-clamp: 3; -webkit-box-orient: vertical; }
  `],
})
export class BooksListComponent implements OnInit {
  private catalog = inject(CatalogService);
  private route = inject(ActivatedRoute);

  books = signal<Book[]>([]);
  loading = signal(true);
  currentPage = signal(1);
  totalPages = signal(1);
  pageSize = 10;
  authorId?: string;

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.authorId = params['authorId'];
      this.currentPage.set(1);
      this.loadBooks();
    });
  }

  loadBooks(): void {
    this.loading.set(true);
    this.catalog.getBooks(this.currentPage(), this.pageSize, this.authorId).subscribe({
      next: (result) => {
        this.books.set(result.items);
        this.totalPages.set(result.totalPages);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  onPageChange(page: number): void {
    this.currentPage.set(page);
    this.loadBooks();
  }
}
