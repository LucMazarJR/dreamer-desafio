import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  return auth.isAuthenticated() ? true : router.createUrlTree(['/']);
};

// Requires Manager or HrAdmin — redirects Collaborators to dashboard
export const managerGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (!auth.isAuthenticated()) return router.createUrlTree(['/']);
  return auth.isManager() ? true : router.createUrlTree(['/dashboard']);
};

// Requires HrAdmin only — redirects everyone else to dashboard
export const hrAdminGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (!auth.isAuthenticated()) return router.createUrlTree(['/']);
  return auth.isHrAdmin() ? true : router.createUrlTree(['/dashboard']);
};
