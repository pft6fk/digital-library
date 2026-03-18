import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { DatePipe } from '@angular/common';
import { CatalogService } from '../../../core/services/catalog.service';
import { EngagementService } from '../../../core/services/engagement.service';
import { AuthService } from '../../../core/auth/auth.service';
import { BookDetail } from '../../../core/models/book.model';
import { Comment } from '../../../core/models/comment.model';
import { BookRating } from '../../../core/models/rating.model';

@Component({
  selector: 'app-book-detail',
  standalone: true,
  imports: [FormsModule, DatePipe],
  template: `
    @if (loading()) {
      <p>Loading...</p>
    } @else if (book()) {
      <div class="book-detail">
        <h2>{{ book()!.title }}</h2>
        <p class="author">By <strong>{{ book()!.authorName }}</strong></p>
        @if (book()!.description) {
          <p class="description">{{ book()!.description }}</p>
        }
      </div>

      <div class="rating-section">
        <div class="rating-display">
          <span class="stars">
            @for (star of [1,2,3,4,5]; track star) {
              <span
                class="star"
                [class.filled]="star <= Math.round(rating().averageRating)"
                [class.clickable]="auth.isAuthenticated()"
                (click)="submitRating(star)"
              >&#9733;</span>
            }
          </span>
          <span class="rating-text">
            @if (rating().totalCount > 0) {
              {{ rating().averageRating }} / 5 ({{ rating().totalCount }} {{ rating().totalCount === 1 ? 'rating' : 'ratings' }})
            } @else {
              No ratings yet
            }
          </span>
        </div>
        @if (auth.isAuthenticated() && rating().userRating) {
          <p class="user-rating">Your rating: {{ rating().userRating }} / 5</p>
        } @else if (auth.isAuthenticated()) {
          <p class="user-rating">Click a star to rate this book</p>
        }
      </div>

      <hr />

      <h3>Comments ({{ comments().length }})</h3>

      @if (auth.isAuthenticated()) {
        <form class="comment-form" (ngSubmit)="submitComment()">
          <textarea
            [(ngModel)]="commentText"
            name="comment"
            placeholder="Write a comment..."
            rows="3"
            required
          ></textarea>
          <button type="submit" class="btn-submit" [disabled]="submitting()">
            {{ submitting() ? 'Posting...' : 'Post Comment' }}
          </button>
        </form>
      } @else {
        <p class="login-hint">Please log in to leave a comment or rate this book.</p>
      }

      <div class="comments-list">
        @for (comment of comments(); track comment.id) {
          <div class="comment-card">
            <div class="comment-header">
              <strong>{{ comment.userName }}</strong>
              <span class="comment-date">{{ comment.createdAt | date:'medium' }}</span>
            </div>
            <p>{{ comment.text }}</p>
          </div>
        } @empty {
          <p class="no-comments">No comments yet. Be the first!</p>
        }
      </div>
    } @else {
      <p>Book not found.</p>
    }
  `,
  styles: [`
    .book-detail { margin-bottom: 20px; }
    .book-detail h2 { margin-bottom: 8px; }
    .author { color: #555; font-size: 1.1rem; }
    .description { margin-top: 12px; line-height: 1.6; }
    .rating-section { margin: 16px 0; padding: 12px; background: #f9f9f9; border-radius: 8px; }
    .rating-display { display: flex; align-items: center; gap: 12px; }
    .stars { font-size: 1.5rem; letter-spacing: 2px; }
    .star { color: #ddd; cursor: default; transition: color 0.15s; }
    .star.filled { color: #f5a623; }
    .star.clickable { cursor: pointer; }
    .star.clickable:hover { color: #f5a623; }
    .rating-text { color: #555; font-size: 0.95rem; }
    .user-rating { color: #888; font-size: 0.85rem; margin-top: 6px; }
    hr { margin: 24px 0; border: none; border-top: 1px solid #eee; }
    h3 { margin-bottom: 16px; }
    .comment-form { margin-bottom: 24px; }
    .comment-form textarea { width: 100%; padding: 10px; border: 1px solid #ccc; border-radius: 4px; box-sizing: border-box; resize: vertical; }
    .btn-submit { margin-top: 8px; padding: 8px 20px; background: #1a1a2e; color: white; border: none; border-radius: 4px; cursor: pointer; }
    .btn-submit:disabled { opacity: 0.6; }
    .login-hint { color: #888; font-style: italic; margin-bottom: 16px; }
    .comments-list { display: flex; flex-direction: column; gap: 12px; }
    .comment-card { border: 1px solid #eee; border-radius: 6px; padding: 12px; }
    .comment-header { display: flex; justify-content: space-between; margin-bottom: 6px; }
    .comment-date { color: #999; font-size: 0.8rem; }
    .no-comments { color: #999; }
  `],
})
export class BookDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private catalog = inject(CatalogService);
  private engagement = inject(EngagementService);
  auth = inject(AuthService);

  book = signal<BookDetail | null>(null);
  comments = signal<Comment[]>([]);
  rating = signal<BookRating>({ averageRating: 0, totalCount: 0, userRating: null });
  loading = signal(true);
  submitting = signal(false);
  commentText = '';

  Math = Math;

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.catalog.getBook(id).subscribe({
      next: (book) => {
        this.book.set(book);
        this.loading.set(false);
        this.loadComments(id);
        this.loadRating(id);
      },
      error: () => this.loading.set(false),
    });
  }

  private loadComments(bookId: string): void {
    this.engagement.getComments(bookId).subscribe({
      next: (comments) => this.comments.set(comments),
    });
  }

  private loadRating(bookId: string): void {
    this.engagement.getBookRating(bookId).subscribe({
      next: (rating) => this.rating.set(rating),
    });
  }

  submitRating(value: number): void {
    if (!this.auth.isAuthenticated()) return;
    const bookId = this.book()?.id;
    if (!bookId) return;

    this.engagement.addRating(bookId, value).subscribe({
      next: () => this.loadRating(bookId),
    });
  }

  submitComment(): void {
    const bookId = this.book()?.id;
    if (!bookId || !this.commentText.trim()) return;

    this.submitting.set(true);
    this.engagement.addComment(bookId, this.commentText).subscribe({
      next: () => {
        this.commentText = '';
        this.submitting.set(false);
        this.loadComments(bookId);
      },
      error: () => this.submitting.set(false),
    });
  }
}
