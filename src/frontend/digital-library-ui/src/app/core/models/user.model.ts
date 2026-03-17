export interface AuthUser {
  userId: string;
  email: string;
  name: string;
}

export interface AuthResponse {
  accessToken: string;
  expiresAt: string;
  userId: string;
  name: string;
  email: string;
}
