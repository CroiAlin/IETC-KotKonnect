import { Routes } from '@angular/router';

// Routes de la feature Auth.
export const authRoutes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('../pages/auth/login.page').then((m) => m.LoginPage),
  },
  {
    path: 'register',
    loadComponent: () => import('../pages/auth/register.page').then((m) => m.RegisterPage),
  },
];
