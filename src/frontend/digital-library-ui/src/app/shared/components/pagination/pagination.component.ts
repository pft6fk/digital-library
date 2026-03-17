import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-pagination',
  standalone: true,
  template: `
    <div class="pagination">
      <button class="page-btn" [disabled]="currentPage() <= 1" (click)="pageChange.emit(currentPage() - 1)">Previous</button>
      <span class="page-info">Page {{ currentPage() }} of {{ totalPages() }}</span>
      <button class="page-btn" [disabled]="currentPage() >= totalPages()" (click)="pageChange.emit(currentPage() + 1)">Next</button>
    </div>
  `,
  styles: [`
    .pagination { display: flex; align-items: center; justify-content: center; gap: 16px; margin: 20px 0; }
    .page-btn { padding: 8px 16px; border: 1px solid #ccc; background: white; border-radius: 4px; cursor: pointer; }
    .page-btn:disabled { opacity: 0.5; cursor: not-allowed; }
    .page-btn:hover:not(:disabled) { background: #f0f0f0; }
    .page-info { font-size: 0.9rem; color: #666; }
  `],
})
export class PaginationComponent {
  currentPage = input.required<number>();
  totalPages = input.required<number>();
  pageChange = output<number>();
}
