import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CatalogService } from '../../../core/services/catalog.service';
import { AuthService } from '../../../core/auth/auth.service';
import { Book } from '../../../core/models/book.model';
import { Author } from '../../../core/models/author.model';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';

@Component({
  selector: 'app-books-list',
  standalone: true,
  imports: [RouterLink, PaginationComponent, FormsModule],
  template: `
    <h2>Books</h2>

    @if (auth.isAuthenticated()) {
      <div class="create-form">
        @if (!showForm()) {
          <button class="btn-add" (click)="openForm()">+ Add Book</button>
        } @else {
          <h3>Add New Book</h3>
          <form (ngSubmit)="onCreateBook()">
            <div class="form-row">
              <input type="text" [(ngModel)]="newBook.title" name="title" placeholder="Title" required />
              <select [(ngModel)]="newBook.authorId" name="authorId" required>
                <option value="" disabled>Select Author</option>
                @for (author of authorsList(); track author.id) {
                  <option [value]="author.id">{{ author.fullName }}</option>
                }
              </select>
            </div>
            <textarea [(ngModel)]="newBook.description" name="description" placeholder="Description (optional)" rows="2"></textarea>
            <div class="form-actions">
              <button type="submit" class="btn-submit">Create</button>
              <button type="button" class="btn-cancel" (click)="showForm.set(false)">Cancel</button>
            </div>
          </form>
        }
      </div>
    }

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
    .create-form { margin-bottom: 24px; padding: 16px; border: 1px solid #ddd; border-radius: 8px; background: #f9f9f9; }
    .create-form h3 { margin: 0 0 12px; }
    .form-row { display: flex; gap: 12px; margin-bottom: 8px; }
    .form-row input, .form-row select { flex: 1; padding: 8px; border: 1px solid #ccc; border-radius: 4px; }
    .create-form textarea { width: 100%; padding: 8px; border: 1px solid #ccc; border-radius: 4px; box-sizing: border-box; resize: vertical; }
    .form-actions { display: flex; gap: 8px; margin-top: 8px; }
    .btn-add { padding: 8px 16px; background: #1a1a2e; color: white; border: none; border-radius: 4px; cursor: pointer; }
    .btn-submit { padding: 8px 16px; background: #1a1a2e; color: white; border: none; border-radius: 4px; cursor: pointer; }
    .btn-cancel { padding: 8px 16px; background: #ccc; border: none; border-radius: 4px; cursor: pointer; }
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
  auth = inject(AuthService);

  books = signal<Book[]>([]);
  authorsList = signal<Author[]>([]);
  loading = signal(true);
  currentPage = signal(1);
  totalPages = signal(1);
  pageSize = 10;
  authorId?: string;
  showForm = signal(false);
  newBook = { title: '', description: '', authorId: '' };

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      this.authorId = params['authorId'];
      this.currentPage.set(1);
      this.loadBooks();
    });
  }

  openForm(): void {
    this.catalog.getAuthors().subscribe(authors => this.authorsList.set(authors));
    this.showForm.set(true);
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

  onCreateBook(): void {
    if (!this.newBook.title.trim() || !this.newBook.authorId) return;
    this.catalog.createBook({
      title: this.newBook.title,
      description: this.newBook.description || undefined,
      authorId: this.newBook.authorId,
    }).subscribe({
      next: () => {
        this.newBook = { title: '', description: '', authorId: '' };
        this.showForm.set(false);
        this.loadBooks();
      },
    });
  }
}
