import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export type UserRole = 'Collaborator' | 'Manager' | 'HrAdmin';

export interface UserApiResponse {
  id: number;
  name: string;
  email: string;
  role: UserRole;
  cpf?: string;
  timeZone: string;
  managerId?: number;
  isActive: boolean;
  createdAt: string;
}

export interface CreateUserRequest {
  name: string;
  email: string;
  password: string;
  role: UserRole;
  cpf?: string;
  timeZone: string;
  managerId?: number;
}

export interface UpdateUserRequest {
  name?: string;
  email?: string;
  timeZone?: string;
  managerId?: number | null; // null = clear manager
  isActive?: boolean;
  role?: UserRole;
}

@Injectable({ providedIn: 'root' })
export class UserApiService {
  private http = inject(HttpClient);

  getAll(managerId?: number): Observable<UserApiResponse[]> {
    const params = managerId != null ? { managerId: String(managerId) } : undefined;
    return this.http.get<UserApiResponse[]>(`${environment.apiUrl}/api/users`, { params });
  }

  create(dto: CreateUserRequest) {
    return this.http.post<UserApiResponse>(`${environment.apiUrl}/api/users`, dto);
  }

  update(id: number, dto: UpdateUserRequest) {
    return this.http.put<UserApiResponse>(`${environment.apiUrl}/api/users/${id}`, dto);
  }
}
