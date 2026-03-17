import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', redirectTo: '/books', pathMatch: 'full' },
  { path: 'login', loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent) },
  { path: 'register', loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent) },
  { path: 'authors', loadComponent: () => import('./features/authors/authors-list/authors-list.component').then(m => m.AuthorsListComponent) },
  { path: 'books', loadComponent: () => import('./features/books/books-list/books-list.component').then(m => m.BooksListComponent) },
  { path: 'books/:id', loadComponent: () => import('./features/books/book-detail/book-detail.component').then(m => m.BookDetailComponent) },
];
