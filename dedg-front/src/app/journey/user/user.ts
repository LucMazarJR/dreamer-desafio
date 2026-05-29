import { Component, signal, computed, inject, OnInit } from '@angular/core';
import {
  LucidePlus,
  LucideSearch,
  LucidePencil,
  LucideHash,
  LucideChevronDown,
  LucideChevronUp,
  LucideX,
  LucideUsers,
  LucideList,
} from '@lucide/angular';
import { AuthService } from '../../core/auth/auth.service';
import {
  UserApiService,
  UserApiResponse,
  UserRole,
  CreateUserRequest,
  UpdateUserRequest,
} from '../../core/user/user-api.service';
import { buildTimezoneOptions } from '../../core/timezone-options';

interface TeamGroup {
  manager: UserApiResponse | null; // null = unassigned
  members: UserApiResponse[];
}

interface FormData {
  name: string;
  email: string;
  password: string;
  role: UserRole;
  cpf: string;
  timeZone: string;
  managerId: string;
  isActive: boolean;
}

@Component({
  selector: 'app-user',
  imports: [
    LucidePlus,
    LucideSearch,
    LucidePencil,
    LucideHash,
    LucideChevronDown,
    LucideChevronUp,
    LucideX,
    LucideUsers,
    LucideList,
  ],
  templateUrl: './user.html',
})
export class User implements OnInit {
  auth = inject(AuthService);
  private userApiSvc = inject(UserApiService);

  users = signal<UserApiResponse[]>([]);
  loading = signal(true);
  loadError = signal('');
  actionError = signal('');

  viewMode = signal<'list' | 'teams'>('list');
  searchQuery = signal('');
  expandedManagers = signal<Set<number>>(new Set());

  managerSearch = signal('');
  managerDropdownOpen = signal(false);

  showForm = signal(false);
  editingUser = signal<UserApiResponse | null>(null);
  formData = signal<FormData>({
    name: '', email: '', password: '', role: 'Collaborator',
    cpf: '', timeZone: 'America/Sao_Paulo', managerId: '', isActive: true,
  });
  formError = signal('');
  formLoading = signal(false);

  readonly timezoneOptions = buildTimezoneOptions();

  readonly roleOptions: { value: UserRole; label: string }[] = [
    { value: 'Collaborator', label: 'Colaborador' },
    { value: 'Manager',      label: 'Gestor' },
    { value: 'HrAdmin',      label: 'Administrador RH' },
  ];

  filteredUsers = computed(() => {
    const q = this.searchQuery().toLowerCase().trim();
    if (!q) return this.users();
    return this.users().filter(
      (u) => u.name.toLowerCase().includes(q) || u.email.toLowerCase().includes(q),
    );
  });

  // Grouped view — only when HrAdmin, teams mode, and no search active
  isTeamsView = computed(
    () => this.auth.isHrAdmin() && this.viewMode() === 'teams' && !this.searchQuery().trim(),
  );

  teamGroups = computed((): TeamGroup[] => {
    const all = this.users();
    const memberMap = new Map<number, UserApiResponse[]>();
    const unassigned: UserApiResponse[] = [];

    for (const u of all) {
      if (u.managerId != null) {
        const bucket = memberMap.get(u.managerId) ?? [];
        bucket.push(u);
        memberMap.set(u.managerId, bucket);
      } else {
        unassigned.push(u);
      }
    }

    const groups: TeamGroup[] = [];
    for (const [managerId, members] of memberMap) {
      const manager = all.find((u) => u.id === managerId) ?? null;
      groups.push({ manager, members });
    }
    groups.sort((a, b) => (a.manager?.name ?? '').localeCompare(b.manager?.name ?? ''));

    if (unassigned.length > 0) groups.push({ manager: null, members: unassigned });

    return groups;
  });

  availableManagers = computed(() =>
    this.users().filter((u) => u.role === 'Manager' || u.role === 'HrAdmin'),
  );

  managerSearchResults = computed(() => {
    const q = this.managerSearch().toLowerCase().trim();
    return this.availableManagers().filter(
      (m) =>
        !this.isSelf(m.id) &&
        (!q || m.name.toLowerCase().includes(q) || m.email.toLowerCase().includes(q)),
    );
  });

  ngOnInit() {
    this.loadUsers();
  }

  private loadUsers() {
    this.loading.set(true);
    this.loadError.set('');
    const currentUserId = this.auth.currentUser()?.userId;
    const managerId = this.auth.isHrAdmin() ? undefined : currentUserId;

    this.userApiSvc.getAll(managerId).subscribe({
      next: (users) => { this.users.set(users); this.loading.set(false); },
      error: () => {
        this.loadError.set('Não foi possível carregar os usuários. Tente recarregar a página.');
        this.loading.set(false);
      },
    });
  }

  openCreateForm() {
    this.editingUser.set(null);
    this.formData.set({
      name: '', email: '', password: '', role: 'Collaborator',
      cpf: '', timeZone: 'America/Sao_Paulo', managerId: '', isActive: true,
    });
    this.managerSearch.set('');
    this.managerDropdownOpen.set(false);
    this.formError.set('');
    this.showForm.set(true);
  }

  openEditForm(user: UserApiResponse) {
    this.editingUser.set(user);
    this.formData.set({
      name: user.name, email: user.email, password: '', role: user.role,
      cpf: user.cpf ?? '', timeZone: user.timeZone,
      managerId: user.managerId != null ? String(user.managerId) : '',
      isActive: user.isActive,
    });
    const manager = user.managerId != null
      ? this.availableManagers().find((m) => m.id === user.managerId) ?? null
      : null;
    this.managerSearch.set(manager?.name ?? '');
    this.managerDropdownOpen.set(false);
    this.formError.set('');
    this.showForm.set(true);
  }

  closeForm() {
    this.showForm.set(false);
    this.editingUser.set(null);
    this.formError.set('');
    this.managerSearch.set('');
    this.managerDropdownOpen.set(false);
  }

  onManagerSearch(value: string) {
    this.managerSearch.set(value);
    this.managerDropdownOpen.set(true);
    if (!value.trim()) this.patchForm({ managerId: '' });
  }

  selectManager(manager: UserApiResponse) {
    this.patchForm({ managerId: String(manager.id) });
    this.managerSearch.set(manager.name);
    this.managerDropdownOpen.set(false);
  }

  clearManager() {
    this.patchForm({ managerId: '' });
    this.managerSearch.set('');
    this.managerDropdownOpen.set(false);
  }

  onManagerBlur() {
    this.managerDropdownOpen.set(false);
    const selectedId = this.formData().managerId;
    if (selectedId) {
      const found = this.availableManagers().find((m) => String(m.id) === selectedId);
      this.managerSearch.set(found?.name ?? '');
    } else {
      this.managerSearch.set('');
    }
  }

  patchForm(patch: Partial<FormData>) {
    this.formData.update((s) => ({ ...s, ...patch }));
  }

  submitForm() {
    if (this.formLoading()) return;
    const f = this.formData();
    const editing = this.editingUser();

    if (!f.name.trim())  { this.formError.set('Nome é obrigatório.'); return; }
    if (!f.email.trim()) { this.formError.set('E-mail é obrigatório.'); return; }
    if (!editing && !f.password) { this.formError.set('Senha é obrigatória.'); return; }
    if (!editing && f.password.length < 6) {
      this.formError.set('A senha deve ter pelo menos 6 caracteres.'); return;
    }

    this.formError.set('');
    this.formLoading.set(true);
    const managerId = f.managerId ? parseInt(f.managerId, 10) : undefined;

    if (editing) {
      const dto: UpdateUserRequest = {
        name: f.name, email: f.email,
        timeZone: f.timeZone,
        managerId: f.managerId ? parseInt(f.managerId, 10) : null,
        isActive: f.isActive,
        role: f.role,
      };
      this.userApiSvc.update(editing.id, dto).subscribe({
        next: (updated) => {
          if (updated.isActive) {
            this.users.update((list) => list.map((u) => (u.id === updated.id ? updated : u)));
          } else {
            // deactivated — remove from view (backend returns inactive users filtered)
            this.users.update((list) => list.filter((u) => u.id !== updated.id));
          }
          this.formLoading.set(false);
          this.closeForm();
        },
        error: (err) => {
          this.formLoading.set(false);
          this.formError.set(err?.error?.message ?? 'Erro ao salvar. Tente novamente.');
        },
      });
    } else {
      const dto: CreateUserRequest = {
        name: f.name, email: f.email, password: f.password,
        role: f.role, timeZone: f.timeZone,
        ...(f.cpf ? { cpf: f.cpf } : {}),
        ...(managerId != null ? { managerId } : {}),
      };
      this.userApiSvc.create(dto).subscribe({
        next: (created) => {
          this.users.update((list) => [created, ...list]);
          this.formLoading.set(false);
          this.closeForm();
        },
        error: (err) => {
          this.formLoading.set(false);
          this.formError.set(err?.error?.message ?? 'Erro ao criar usuário. Tente novamente.');
        },
      });
    }
  }

  // Teams
  toggleTeam(managerId: number) {
    this.expandedManagers.update((set) => {
      const next = new Set(set);
      next.has(managerId) ? next.delete(managerId) : next.add(managerId);
      return next;
    });
  }

  isExpanded(managerId: number | null): boolean {
    if (managerId === null) return true; // unassigned always visible
    return this.expandedManagers().has(managerId);
  }

  // Display helpers
  avatarColor(id: number): string {
    const colors = ['bg-blue-500', 'bg-pink-500', 'bg-orange-500', 'bg-violet-500', 'bg-teal-500', 'bg-rose-500', 'bg-amber-500', 'bg-cyan-500'];
    return colors[id % colors.length];
  }

  initials(name: string): string {
    return name.split(' ').slice(0, 2).map((n) => n[0]).join('').toUpperCase();
  }

  roleLabel(role: UserRole): string {
    const labels: Record<UserRole, string> = {
      Collaborator: 'Colaborador', Manager: 'Gestor', HrAdmin: 'Administrador RH',
    };
    return labels[role];
  }

  tzLabel(tz: string): string {
    return tz.split('/').pop()?.replace(/_/g, ' ') ?? tz;
  }

  isSelf(userId: number): boolean {
    return userId === this.auth.currentUser()?.userId;
  }
}
