import { Routes } from '@angular/router';
import { authGuard } from './shared/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./features/auth/pages/sign-in').then((module) => module.default),
  },
  {
    path: 'register',
    loadComponent: () => import('./features/auth/pages/sign-up').then((module) => module.default),
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () => import('./features/dashboard/pages/dashboard').then((module) => module.default),
  },
  {
    path: 'workspaces/create',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/dashboard/pages/create-workspace').then((module) => module.default),
  },
  {
    path: 'auth-test',
    canActivate: [authGuard],
    loadComponent: () => import('./features/auth/pages/auth-test').then((module) => module.default),
  },
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full',
  },
  {
    path: '**',
    redirectTo: 'login',
  },
];
