import { Routes } from '@angular/router';
import { Login } from './login/login';
import { Journey } from './journey/journey';
import { Dashboard } from './journey/dashboard/dashboard';
import { History } from './journey/history/history';
import { User } from './journey/user/user';
import { Period } from './journey/period/period';
import { authGuard, managerGuard, hrAdminGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  { path: '', component: Login },
  {
    path: '',
    component: Journey,
    canActivate: [authGuard],
    children: [
      { path: 'dashboard', component: Dashboard },
      { path: 'history', component: History },
      { path: 'users', component: User, canActivate: [managerGuard] },
      { path: 'periods', component: Period, canActivate: [hrAdminGuard] },
    ],
  },
];
