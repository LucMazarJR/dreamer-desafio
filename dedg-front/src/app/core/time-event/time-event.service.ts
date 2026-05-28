import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export type EventType = 'Entry' | 'Exit' | 'BreakStart' | 'BreakEnd';

export interface TimeEventResponse {
  id: number;
  userId: number;
  eventType: EventType;
  recordedAt: string;
  timezoneAtRecording: string;
  isTravel: boolean;
  observation?: string;
  createdAt: string;
}

export interface UpdateTimeEventRequest {
  eventType?: EventType;
  recordedAt?: string;
  timezoneAtRecording?: string;
  isTravel?: boolean;
  observation?: string;
}

export interface TimeEventSummary {
  userId: number;
  year: number;
  month: number;
  daysWorked: number;
  totalWorkedMinutes: number;
  totalExpectedMinutes: number;
  overtimeMinutes: number;
  negativeMinutes: number;
  periodStatus: string;
}

@Injectable({ providedIn: 'root' })
export class TimeEventService {
  constructor(private http: HttpClient) {}

  getEvents(userId: number) {
    return this.http.get<TimeEventResponse[]>(`${environment.apiUrl}/api/timeevents`, {
      params: { userId },
    });
  }

  getAllEvents(userId?: number) {
    return this.http.get<TimeEventResponse[]>(`${environment.apiUrl}/api/timeevents`, {
      ...(userId != null ? { params: { userId } } : {}),
    });
  }

  getSummary(userId: number, year: number, month: number) {
    return this.http.get<TimeEventSummary>(`${environment.apiUrl}/api/timeevents/summary`, {
      params: { userId, year, month },
    });
  }

  update(id: number, dto: UpdateTimeEventRequest) {
    return this.http.put<TimeEventResponse>(`${environment.apiUrl}/api/timeevents/${id}`, dto);
  }

  register(
    userId: number,
    eventType: EventType,
    options?: { observation?: string; isTravel?: boolean; timezone?: string },
  ) {
    return this.http.post<TimeEventResponse>(`${environment.apiUrl}/api/timeevents`, {
      userId,
      eventType,
      recordedAt: new Date().toISOString(),
      timezoneAtRecording: options?.timezone ?? Intl.DateTimeFormat().resolvedOptions().timeZone,
      isTravel: options?.isTravel ?? false,
      ...(options?.observation ? { observation: options.observation } : {}),
    });
  }
}
