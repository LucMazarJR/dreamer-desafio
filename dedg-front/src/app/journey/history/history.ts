import { Component, signal, computed } from '@angular/core';
import {
  LucideCalendar,
  LucideSearch,
  LucidePlus,
  LucideChevronLeft,
  LucideChevronRight,
  LucideChevronDown,
} from '@lucide/angular';

type EventType = 'Entrada' | 'Saída Intervalo' | 'Retorno Intervalo' | 'Saída';

interface HistoryRecord {
  id: number;
  name: string;
  role: string;
  initials: string;
  avatarColor: string;
  dateDay: string;
  dateYear: string;
  time: string;
  eventType: EventType;
  delay?: string;
  isLate?: boolean;
}

@Component({
  selector: 'app-history',
  imports: [
    LucideCalendar,
    LucideSearch,
    LucidePlus,
    LucideChevronLeft,
    LucideChevronRight,
    LucideChevronDown,
  ],
  templateUrl: './history.html',
})
export class History {
  // TODO: substituir por dados da API
  records: HistoryRecord[] = [
    {
      id: 1,
      name: 'Ana Martins',
      role: 'Analista Financeiro',
      initials: 'AM',
      avatarColor: 'bg-violet-500',
      dateDay: '14 Nov',
      dateYear: '2026',
      time: '08:02:45',
      eventType: 'Entrada',
    },
    {
      id: 2,
      name: 'João Santos',
      role: 'Desenvolvedor Fullstack',
      initials: 'JS',
      avatarColor: 'bg-teal-500',
      dateDay: '14 Nov',
      dateYear: '2026',
      time: '12:15:10',
      eventType: 'Saída Intervalo',
    },
    {
      id: 3,
      name: 'Carla Pereira',
      role: 'Gerente de Projetos',
      initials: 'CP',
      avatarColor: 'bg-pink-500',
      dateDay: '14 Nov',
      dateYear: '2026',
      time: '13:15:00',
      eventType: 'Retorno Intervalo',
      delay: 'ATRASO DE 15M',
      isLate: true,
    },
    {
      id: 4,
      name: 'Roberto Lima',
      role: 'Coordenador de Vendas',
      initials: 'RL',
      avatarColor: 'bg-blue-500',
      dateDay: '14 Nov',
      dateYear: '2026',
      time: '18:15:33',
      eventType: 'Saída',
    },
  ];

  totalRecords = 1284;
  pageSize = 10;
  currentPage = signal(1);
  totalPages = computed(() => Math.ceil(this.totalRecords / this.pageSize));

  eventBadgeClass(record: HistoryRecord): string {
    if (record.isLate) return 'bg-red-50 text-red-500 border border-red-200';
    switch (record.eventType) {
      case 'Entrada':
        return 'bg-teal-50 text-teal-600 border border-teal-200';
      default:
        return 'bg-slate-100 text-slate-500 border border-slate-200';
    }
  }
}
