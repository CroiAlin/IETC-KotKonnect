import { Routes } from '@angular/router';
import { roleGuard } from '../services/guards/role-guard';

// Parent `biens` + routes enfants (liste / détail / formulaire), mêmes URLs.
export const biensRoutes: Routes = [
  {
    path: 'biens',
    children: [
      {
        path: '',
        loadComponent: () => import('../pages/biens/biens.page').then((m) => m.BiensPage),
      },
      {
        path: 'nouveau',
        canActivate: [roleGuard('PROPRIETAIRE')],
        loadComponent: () => import('../pages/biens/bien-form.page').then((m) => m.BienFormPage),
      },
      {
        path: ':id/modifier',
        canActivate: [roleGuard('PROPRIETAIRE')],
        loadComponent: () => import('../pages/biens/bien-form.page').then((m) => m.BienFormPage),
      },
      {
        path: ':id',
        loadComponent: () => import('../pages/biens/bien-detail.page').then((m) => m.BienDetailPage),
      },
    ],
  },
  {
    path: 'mes-biens',
    canActivate: [roleGuard('PROPRIETAIRE')],
    loadComponent: () => import('../pages/biens/mes-biens.page').then((m) => m.MesBiensPage),
  },
];
