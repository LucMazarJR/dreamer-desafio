import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { environment } from '../../../environments/environment';

interface AuthResponse {
  token: string;
  expiresAt: string;
  userId: number;
  name: string;
  role: string;
  timeZone: string;
}

interface MeResponse {
  id: number;
  name: string;
  role: string;
  timeZone: string;
}

export interface StoredUser {
  userId: number;
  name: string;
  role: string;
  timezone: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TOKEN_KEY = 'ddg_token';
  private readonly USER_KEY = 'ddg_user';

  private _token = signal<string | null>(this.readFromStorage(this.TOKEN_KEY));
  private _user = signal<StoredUser | null>(this.readUserFromStorage());

  token = this._token.asReadonly();
  currentUser = this._user.asReadonly();
  isAuthenticated = computed(() => !!this._token());
  isManager = computed(() => {
    const role = this._user()?.role;
    return role === 'Manager' || role === 'HrAdmin';
  });

  constructor(private http: HttpClient, private router: Router) {}

  login(email: string, password: string, rememberMe: boolean) {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/api/auth/login`, { email, password })
      .pipe(
        tap((res) => {
          const storage = rememberMe ? localStorage : sessionStorage;
          const user: StoredUser = { userId: res.userId, name: res.name, role: res.role, timezone: res.timeZone };
          this._token.set(res.token);
          this._user.set(user);
          storage.setItem(this.TOKEN_KEY, res.token);
          storage.setItem(this.USER_KEY, JSON.stringify(user));
        })
      );
  }

  // Called when a token exists but user data was lost (e.g. old session before this feature)
  hydrateUser() {
    return this.http.get<MeResponse>(`${environment.apiUrl}/api/auth/me`).pipe(
      tap((res) => {
        const user: StoredUser = { userId: res.id, name: res.name, role: res.role, timezone: res.timeZone };
        this._user.set(user);
        const storage = localStorage.getItem(this.TOKEN_KEY) ? localStorage : sessionStorage;
        storage.setItem(this.USER_KEY, JSON.stringify(user));
      })
    );
  }

  logout() {
    this._token.set(null);
    this._user.set(null);
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    sessionStorage.removeItem(this.TOKEN_KEY);
    sessionStorage.removeItem(this.USER_KEY);
    this.router.navigate(['/']);
  }

  private readFromStorage(key: string): string | null {
    return localStorage.getItem(key) ?? sessionStorage.getItem(key);
  }

  private readUserFromStorage(): StoredUser | null {
    const raw = localStorage.getItem(this.USER_KEY) ?? sessionStorage.getItem(this.USER_KEY);
    return raw ? JSON.parse(raw) : null;
  }
}
