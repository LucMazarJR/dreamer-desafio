import { Component, signal, computed } from '@angular/core';
import {
  LucidePlus,
  LucideSearch,
  LucideListFilter,
  LucidePencil,
  LucideFingerprint,
  LucideChevronLeft,
  LucideChevronRight,
} from '@lucide/angular';

type UserStatus = 'Ativo' | 'Inativo';

interface Employee {
  id: number;
  name: string;
  initials: string;
  avatarColor: string;
  email: string;
  role: string;
  status: UserStatus;
}

@Component({
  selector: 'app-user',
  imports: [
    LucidePlus,
    LucideSearch,
    LucideListFilter,
    LucidePencil,
    LucideFingerprint,
    LucideChevronLeft,
    LucideChevronRight,
  ],
  templateUrl: './user.html',
})
export class User {
  // TODO: substituir por dados da API
  employees: Employee[] = [
    {
      id: 4421,
      name: 'Ricardo Alves',
      initials: 'RA',
      avatarColor: 'bg-blue-500',
      email: 'ricardo.alves@ddg.com',
      role: 'Analista de Dados',
      status: 'Ativo',
    },
    {
      id: 8832,
      name: 'Juliana Mendes',
      initials: 'JM',
      avatarColor: 'bg-pink-500',
      email: 'juliana.m@ddg.com',
      role: 'Coordenadora RH',
      status: 'Ativo',
    },
    {
      id: 2109,
      name: 'Marcos Silva',
      initials: 'MS',
      avatarColor: 'bg-orange-500',
      email: 'marcos.silva@ddg.com',
      role: 'Suporte Técnico',
      status: 'Inativo',
    },
    {
      id: 5543,
      name: 'Beatriz Ramos',
      initials: 'BR',
      avatarColor: 'bg-violet-500',
      email: 'beatriz.ramos@ddg.com',
      role: 'Gerente Operacional',
      status: 'Ativo',
    },
  ];

  totalUsers = 128;
  pageSize = 10;
  currentPage = signal(1);
  totalPages = computed(() => Math.ceil(this.totalUsers / this.pageSize));

  statusClass(status: UserStatus): string {
    return status === 'Ativo'
      ? 'bg-teal-50 text-teal-600 border border-teal-200'
      : 'bg-slate-100 text-slate-500 border border-slate-200';
  }
}
