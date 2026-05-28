import { Component, signal, computed, inject, OnInit } from '@angular/core';
import {
  LucideCalendar,
  LucideSearch,
  LucideChevronLeft,
  LucideChevronRight,
  LucidePencil,
} from '@lucide/angular';
import { forkJoin } from 'rxjs';
import { AuthService } from '../../core/auth/auth.service';
import { TimeEventService, TimeEventResponse, EventType } from '../../core/time-event/time-event.service';
import { UserApiService, UserApiResponse } from '../../core/user/user-api.service';
import { utcOffsetMinutes } from '../../core/timezone-options';

interface EditForm {
  eventType: EventType;
  date: string;   // YYYY-MM-DD no fuso do registro original
  time: string;   // HH:MM no fuso do registro original
  timezone: string;
  isTravel: boolean;
  observation: string;
}

const PAGE_SIZE = 20;

@Component({
  selector: 'app-history',
  imports: [LucideCalendar, LucideSearch, LucideChevronLeft, LucideChevronRight, LucidePencil],
  templateUrl: './history.html',
})
export class History implements OnInit {
  auth = inject(AuthService);
  private timeEventSvc = inject(TimeEventService);
  private userApiSvc = inject(UserApiService);

  events = signal<TimeEventResponse[]>([]);
  teamUsers = signal<UserApiResponse[]>([]);
  loading = signal(true);
  loadError = signal('');

  searchQuery = signal('');
  selectedYear = signal(new Date().getFullYear());
  selectedMonth = signal(new Date().getMonth() + 1);
  currentPage = signal(1);

  readonly years = Array.from({ length: 3 }, (_, i) => new Date().getFullYear() - 1 + i);
  readonly months = Array.from({ length: 12 }, (_, i) => i + 1);

  private userMap = computed(() => {
    const map = new Map<number, UserApiResponse>();
    for (const u of this.teamUsers()) map.set(u.id, u);
    return map;
  });

  showUserColumn = computed(() => this.auth.isManager());

  filteredEvents = computed(() => {
    const y = this.selectedYear();
    const m = this.selectedMonth();
    const q = this.searchQuery().toLowerCase().trim();

    return this.events()
      .filter((e) => {
        const d = new Date(this.normalizeUtc(e.recordedAt));
        if (d.getUTCFullYear() !== y || d.getUTCMonth() + 1 !== m) return false;
        if (q) {
          const name = this.userName(e.userId).toLowerCase();
          if (!name.includes(q)) return false;
        }
        return true;
      })
      .sort((a, b) =>
        new Date(this.normalizeUtc(b.recordedAt)).getTime() -
        new Date(this.normalizeUtc(a.recordedAt)).getTime(),
      );
  });

  totalPages = computed(() => Math.max(1, Math.ceil(this.filteredEvents().length / PAGE_SIZE)));

  pagedEvents = computed(() => {
    const page = Math.min(this.currentPage(), this.totalPages());
    const start = (page - 1) * PAGE_SIZE;
    return this.filteredEvents().slice(start, start + PAGE_SIZE);
  });

  visiblePages = computed((): (number | '...')[] => {
    const total = this.totalPages();
    const current = this.currentPage();
    const range = new Set<number>([1, total]);
    for (let i = Math.max(1, current - 1); i <= Math.min(total, current + 1); i++) range.add(i);
    const sorted = [...range].sort((a, b) => a - b);
    const pages: (number | '...')[] = [];
    for (let i = 0; i < sorted.length; i++) {
      if (i > 0 && sorted[i] - sorted[i - 1] > 1) pages.push('...');
      pages.push(sorted[i]);
    }
    return pages;
  });

  homeTimezone = computed(() => this.auth.currentUser()?.timezone ?? null);

  canEdit = computed(() => this.auth.isManager());
  editingId = signal<number | null>(null);
  editForm = signal<EditForm | null>(null);
  saving = signal(false);
  saveError = signal('');

  ngOnInit() {
    this.loadData();
  }

  private loadData() {
    const user = this.auth.currentUser();
    if (!user) return;

    this.loading.set(true);
    this.loadError.set('');

    if (this.auth.isHrAdmin()) {
      forkJoin({
        users: this.userApiSvc.getAll(),
        events: this.timeEventSvc.getAllEvents(),
      }).subscribe({
        next: ({ users, events }) => {
          this.teamUsers.set(users);
          this.events.set(events);
          this.loading.set(false);
        },
        error: () => this.setLoadError(),
      });
    } else if (this.auth.isManager()) {
      forkJoin({
        users: this.userApiSvc.getAll(user.userId),
        events: this.timeEventSvc.getAllEvents(),
      }).subscribe({
        next: ({ users, events }) => {
          const teamIds = new Set(users.map((u) => u.id));
          teamIds.add(user.userId);
          this.teamUsers.set(users);
          this.events.set(events.filter((e) => teamIds.has(e.userId)));
          this.loading.set(false);
        },
        error: () => this.setLoadError(),
      });
    } else {
      this.timeEventSvc.getAllEvents(user.userId).subscribe({
        next: (events) => { this.events.set(events); this.loading.set(false); },
        error: () => this.setLoadError(),
      });
    }
  }

  private setLoadError() {
    this.loadError.set('Não foi possível carregar o histórico. Tente recarregar a página.');
    this.loading.set(false);
  }

  setYear(year: number)   { this.selectedYear.set(year);   this.currentPage.set(1); }
  setMonth(month: number) { this.selectedMonth.set(month); this.currentPage.set(1); }
  setSearch(q: string)    { this.searchQuery.set(q);        this.currentPage.set(1); }

  goToPage(page: number | '...') {
    if (page === '...') return;
    this.currentPage.set(Math.max(1, Math.min(page, this.totalPages())));
  }

  userName(userId: number): string {
    const self = this.auth.currentUser();
    if (self?.userId === userId) return self.name;
    return this.userMap().get(userId)?.name ?? `#${userId}`;
  }

  userInitials(userId: number): string {
    return this.userName(userId)
      .split(' ').slice(0, 2).map((w) => w[0]).join('').toUpperCase();
  }

  avatarColor(userId: number): string {
    const colors = ['bg-blue-500', 'bg-pink-500', 'bg-orange-500', 'bg-violet-500',
                    'bg-teal-500', 'bg-rose-500', 'bg-amber-500', 'bg-cyan-500'];
    return colors[userId % colors.length];
  }

  formatDate(iso: string, tz?: string): string {
    return new Date(this.normalizeUtc(iso)).toLocaleDateString('pt-BR', {
      day: '2-digit', month: '2-digit', year: 'numeric',
      ...(tz ? { timeZone: tz } : {}),
    });
  }

  formatTime(iso: string, tz?: string): string {
    return new Date(this.normalizeUtc(iso)).toLocaleTimeString('pt-BR', {
      hour: '2-digit', minute: '2-digit', hour12: false,
      ...(tz ? { timeZone: tz } : {}),
    });
  }

  needsDualTimezone(event: TimeEventResponse): boolean {
    const home = this.homeTimezone();
    return !!home && home !== event.timezoneAtRecording;
  }

  formatTimeInHomeTimezone(iso: string): string {
    const tz = this.homeTimezone();
    return this.formatTime(iso, tz ?? undefined);
  }

  eventTypeLabel(type: EventType): string {
    const labels: Record<EventType, string> = {
      Entry: 'Entrada', Exit: 'Saída',
      BreakStart: 'Saída Intervalo', BreakEnd: 'Retorno Intervalo',
    };
    return labels[type];
  }

  eventBadgeClass(type: EventType): string {
    switch (type) {
      case 'Entry':      return 'bg-teal-50 text-teal-600 border-teal-200';
      case 'Exit':       return 'bg-slate-100 text-slate-500 border-slate-200';
      case 'BreakStart': return 'bg-amber-50 text-amber-600 border-amber-200';
      case 'BreakEnd':   return 'bg-blue-50 text-blue-600 border-blue-200';
    }
  }

  monthName(month: number): string {
    const name = new Date(2000, month - 1).toLocaleDateString('pt-BR', { month: 'long' });
    return name.charAt(0).toUpperCase() + name.slice(1);
  }

  tzShort(tz: string): string {
    return tz.split('/').pop()?.replace(/_/g, ' ') ?? tz;
  }

  // Primary: recording timezone — the actual clock at the location
  displayTime(event: TimeEventResponse): string {
    return this.formatTime(event.recordedAt, event.timezoneAtRecording);
  }

  displayTzLabel(event: TimeEventResponse): string {
    return this.tzShort(event.timezoneAtRecording);
  }

  // Secondary: viewer's home timezone — shown smaller when different
  viewerTime(event: TimeEventResponse): string {
    const home = this.homeTimezone();
    return this.formatTime(event.recordedAt, home ?? event.timezoneAtRecording);
  }

  viewerTzLabel(event: TimeEventResponse): string {
    const home = this.homeTimezone();
    return this.tzShort(home ?? event.timezoneAtRecording);
  }

  startEdit(event: TimeEventResponse) {
    const dt = new Date(this.normalizeUtc(event.recordedAt));
    const tz = event.timezoneAtRecording;
    const date = dt.toLocaleDateString('en-CA', { timeZone: tz });
    const time = dt.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit', hour12: false, timeZone: tz });
    this.editingId.set(event.id);
    this.editForm.set({ eventType: event.eventType, date, time, timezone: tz, isTravel: event.isTravel, observation: event.observation ?? '' });
    this.saveError.set('');
  }

  cancelEdit() {
    this.editingId.set(null);
    this.editForm.set(null);
    this.saveError.set('');
  }

  patchEdit(partial: Partial<EditForm>) {
    this.editForm.update((f) => (f ? { ...f, ...partial } : null));
  }

  saveEdit() {
    const form = this.editForm();
    const id = this.editingId();
    if (!form || !id || this.saving()) return;

    this.saving.set(true);
    this.saveError.set('');

    this.timeEventSvc.update(id, {
      eventType: form.eventType,
      recordedAt: this.toUtcIso(form.date, form.time, form.timezone),
      timezoneAtRecording: form.timezone,
      isTravel: form.isTravel,
      observation: form.observation || undefined,
    }).subscribe({
      next: (updated) => {
        this.events.update((list) => list.map((e) => (e.id === id ? updated : e)));
        this.editingId.set(null);
        this.editForm.set(null);
        this.saving.set(false);
      },
      error: (err) => {
        this.saving.set(false);
        this.saveError.set(err?.error?.message ?? 'Erro ao salvar correção. Tente novamente.');
      },
    });
  }

  private toUtcIso(date: string, time: string, tz: string): string {
    // Parse date+time as if it were UTC, then subtract the tz offset to get true UTC
    const localAsUtcMs = new Date(`${date}T${time}:00Z`).getTime();
    const offsetMs = utcOffsetMinutes(tz) * 60_000;
    return new Date(localAsUtcMs - offsetMs).toISOString();
  }

  private normalizeUtc(iso: string): string {
    return /Z$|[+-]\d{2}:?\d{2}$/.test(iso) ? iso : iso + 'Z';
  }
}
