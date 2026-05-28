import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export type PeriodStatus = 'Open' | 'InReview' | 'Closed';

export interface MonthlyPeriod {
  id: number;
  year: number;
  month: number;
  status: PeriodStatus;
  closedById?: number;
  closedAt?: string;
}

@Injectable({ providedIn: 'root' })
export class MonthlyPeriodService {
  private http = inject(HttpClient);

  getAll() {
    return this.http.get<MonthlyPeriod[]>(`${environment.apiUrl}/api/monthlyperiods`);
  }

  create(year: number, month: number) {
    return this.http.post<MonthlyPeriod>(`${environment.apiUrl}/api/monthlyperiods`, { year, month });
  }

  startReview(id: number) {
    return this.http.put<MonthlyPeriod>(`${environment.apiUrl}/api/monthlyperiods/${id}`, { status: 'InReview' });
  }

  close(id: number, closedById: number) {
    return this.http.put<MonthlyPeriod>(`${environment.apiUrl}/api/monthlyperiods/${id}`, { status: 'Closed', closedById });
  }
}
