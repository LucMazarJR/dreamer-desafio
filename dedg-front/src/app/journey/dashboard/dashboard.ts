import { Component, signal, computed, OnInit, OnDestroy, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import {
  LucideLogIn,
  LucideCoffee,
  LucideUtensils,
  LucideLogOut,
  LucideClipboardList,
  LucideCircleCheckBig,
} from '@lucide/angular';
import { AuthService } from '../../core/auth/auth.service';
import { TimeEventService, TimeEventResponse, EventType } from '../../core/time-event/time-event.service';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink, LucideLogIn, LucideCoffee, LucideUtensils, LucideLogOut, LucideCircleCheckBig],
  templateUrl: './dashboard.html',
})
export class Dashboard implements OnInit, OnDestroy {
  private auth = inject(AuthService);
  private timeEventSvc = inject(TimeEventService);

  private timer: ReturnType<typeof setInterval> | null = null;
  private now = signal(new Date());

  time = computed(() =>
    this.now().toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: false })
  );
  date = computed(() =>
    this.now().toLocaleDateString('pt-BR', { day: 'numeric', month: 'long', year: 'numeric' })
  );

  todayEvents = signal<TimeEventResponse[]>([]);
  periodStatus = signal<string | null>(null);
  loadError = signal('');
  registerError = signal('');
  registering = signal(false);

  private eventsLoaded = signal(false);
  private summaryLoaded = signal(false);
  loading = computed(() => !this.eventsLoaded() || !this.summaryLoaded());

  lastEvent = computed(() => {
    const events = this.todayEvents();
    return events.length ? events[events.length - 1] : null;
  });

  // Live counter — recomputes every second while user is clocked in
  workedMinutes = computed(() => this.computeWorkedMinutes(this.todayEvents(), this.now()));

  periodWarning = computed(() => {
    const ps = this.periodStatus();
    if (ps === 'Closed') return { level: 'error', text: 'O período atual está fechado. Novos registros não são permitidos.' };
    if (ps === 'NoPeriod') return { level: 'error', text: 'Não há período aberto para este mês. Contate o gestor.' };
    if (ps === 'InReview') return { level: 'warning', text: 'O período está em revisão pelo gestor. Você ainda pode registrar ponto.' };
    return null;
  });

  homeTimezone = computed(() => this.auth.currentUser()?.timezone ?? null);

  journeyStatus = computed(() => {
    const last = this.lastEvent();
    if (!last) return null;
    const time = this.formatTime(last.recordedAt, last.timezoneAtRecording);
    switch (last.eventType) {
      case 'Entry':      return `Jornada em andamento desde ${time}.`;
      case 'BreakStart': return `Em intervalo desde ${time}.`;
      case 'BreakEnd':   return `Retornou do intervalo às ${time}.`;
      case 'Exit':       return `Jornada encerrada às ${time}. Bom descanso!`;
    }
  });

  ngOnInit() {
    this.timer = setInterval(() => this.now.set(new Date()), 1000);

    const user = this.auth.currentUser();
    if (this.auth.isAuthenticated() && (!user || !user.timezone)) {
      this.auth.hydrateUser().subscribe({
        next: () => this.loadData(),
        error: () => this.auth.logout(),
      });
    } else {
      this.loadData();
    }
  }

  ngOnDestroy() {
    if (this.timer) clearInterval(this.timer);
  }

  private loadData() {
    const userId = this.auth.currentUser()?.userId;
    if (!userId) return;
    this.loadTodayEvents(userId);
    this.loadSummary(userId);
  }

  private loadTodayEvents(userId: number) {
    this.timeEventSvc.getEvents(userId).subscribe({
      next: (events) => {
        const today = new Date().toDateString();
        this.todayEvents.set(
          events
            .filter((e) => new Date(this.normalizeUtc(e.recordedAt)).toDateString() === today)
            .sort((a, b) => new Date(this.normalizeUtc(a.recordedAt)).getTime() - new Date(this.normalizeUtc(b.recordedAt)).getTime())
        );
        this.eventsLoaded.set(true);
      },
      error: () => {
        this.loadError.set('Não foi possível carregar os registros do dia. Tente recarregar a página.');
        this.eventsLoaded.set(true);
      },
    });
  }

  private loadSummary(userId: number) {
    const now = new Date();
    this.timeEventSvc.getSummary(userId, now.getFullYear(), now.getMonth() + 1).subscribe({
      next: (s) => {
        this.periodStatus.set(s.periodStatus);
        this.summaryLoaded.set(true);
      },
      error: () => this.summaryLoaded.set(true),
    });
  }

  register(eventType: EventType) {
    const userId = this.auth.currentUser()?.userId;
    if (!userId || this.registering() || !this.isButtonEnabled(eventType)) return;

    this.registerError.set('');
    this.registering.set(true);

    this.timeEventSvc.register(userId, eventType).subscribe({
      next: (event) => {
        const today = new Date().toDateString();
        if (new Date(this.normalizeUtc(event.recordedAt)).toDateString() === today) {
          this.todayEvents.update((list) =>
            [...list, event].sort(
              (a, b) => new Date(this.normalizeUtc(a.recordedAt)).getTime() - new Date(this.normalizeUtc(b.recordedAt)).getTime()
            )
          );
        }
        this.registering.set(false);
      },
      error: (err) => {
        this.registering.set(false);
        this.registerError.set(
          err?.error?.message ?? 'Erro ao registrar ponto. Tente novamente.'
        );
      },
    });
  }

  isButtonEnabled(type: EventType): boolean {
    if (this.loading() || this.registering()) return false;
    const ps = this.periodStatus();
    if (ps === 'Closed' || ps === 'NoPeriod') return false;
    const last = this.lastEvent()?.eventType ?? null;
    switch (type) {
      case 'Entry':      return last === null;
      case 'BreakStart': return last === 'Entry' || last === 'BreakEnd';
      case 'BreakEnd':   return last === 'BreakStart';
      case 'Exit':       return last === 'Entry' || last === 'BreakEnd';
    }
  }

  buttonClass(type: EventType): string {
    return this.isButtonEnabled(type)
      ? 'cursor-pointer hover:shadow-lg transition-shadow'
      : 'opacity-40 cursor-not-allowed';
  }

  formatMinutes(minutes: number): string {
    const h = Math.floor(minutes / 60);
    const m = minutes % 60;
    return `${String(h).padStart(2, '0')}:${String(m).padStart(2, '0')}`;
  }

  // Appends 'Z' if the ISO string has no timezone suffix (SQL Server returns DateTime without Kind)
  private normalizeUtc(iso: string): string {
    return /Z$|[+-]\d{2}:?\d{2}$/.test(iso) ? iso : iso + 'Z';
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
      Entry: 'Entrada Realizada',
      Exit: 'Saída Registrada',
      BreakStart: 'Saída p/ Intervalo',
      BreakEnd: 'Retorno de Intervalo',
    };
    return labels[type];
  }

  private computeWorkedMinutes(events: TimeEventResponse[], now: Date): number {
    let workedMs = 0;
    let entryTime: Date | null = null;

    for (const event of events) {
      const t = new Date(this.normalizeUtc(event.recordedAt));
      switch (event.eventType) {
        case 'Entry':
          entryTime = t;
          break;
        case 'BreakStart':
        case 'Exit':
          if (entryTime) { workedMs += t.getTime() - entryTime.getTime(); entryTime = null; }
          break;
        case 'BreakEnd':
          entryTime = t;
          break;
      }
    }

    if (entryTime) workedMs += now.getTime() - entryTime.getTime();
    return Math.floor(workedMs / 60000);
  }
}
