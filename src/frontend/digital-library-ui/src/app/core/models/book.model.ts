export interface Book {
  id: string;
  title: string;
  description: string | null;
  authorId: string;
}

export interface BookDetail {
  id: string;
  title: string;
  description: string | null;
  authorId: string;
  authorName: string;
}
