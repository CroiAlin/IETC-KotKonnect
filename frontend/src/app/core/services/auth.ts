import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest, Role } from '../models/auth.models';

interface SessionUser{
  email: string;
  role: Role;
}

@Injectable({
  providedIn: 'root',
})

export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly authUrl = `${environment.apiUrl}/auth`;

  private readonly ACCESS_KEY ='kk_access';
  private readonly REFRESH_KEY = 'kk_refresh';
  private readonly USER_KEY = 'kk_user';

  private readonly _user = signal<SessionUser | null>(this.lireUser());
  readonly user = this._user.asReadonly();
  readonly isAuthenticated = computed(() => this._user() !== null);

  login(body: LoginRequest) : Observable<AuthResponse>{
    return this.http.post<AuthResponse>(`${this.authUrl}/login`, body).pipe(
      tap((res) => this.ouvrirSession(res)),
    );
  }

  register(body: RegisterRequest): Observable<AuthResponse>{
    return this.http.post<AuthResponse>(`${this.authUrl}/register`, body).pipe(
      tap((res) => this.ouvrirSession(res)),
    );
  }

  refresh(): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.authUrl}/refresh`, {refreshToken: this.getRefreshToken()}).pipe(
      tap((res) => this.ouvrirSession(res)),
    );

  }

  logout(): void {
    localStorage.removeItem(this.ACCESS_KEY);
    localStorage.removeItem(this.REFRESH_KEY);
    localStorage.removeItem(this.USER_KEY);
    this._user.set(null);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.ACCESS_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_KEY);
  }

  private ouvrirSession(res: AuthResponse): void {
    localStorage.setItem(this.ACCESS_KEY, res.accessToken);
    localStorage.setItem(this.REFRESH_KEY, res.refreshToken);
    localStorage.setItem(this.USER_KEY, JSON.stringify({email: res.email, role: res.role}));

    this._user.set({email: res.email, role: res.role});
  }

  private lireUser(): SessionUser | null {
    const user_json = localStorage.getItem(this.USER_KEY);

    if (user_json){
      return JSON.parse(user_json) as SessionUser;
    }
    else {
      return null;
    }
  }
}
