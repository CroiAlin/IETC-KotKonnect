import { Routes } from '@angular/router';
import { authGuard } from '../services/guards/auth-guard';
import { roleGuard } from '../services/guards/role-guard';

// Routes de la feature Profil.
export const profilRoutes: Routes = [
  {
    path: 'mon-profil',
    canActivate: [authGuard],
    loadComponent: () => import('../pages/profil/mon-profil.page').then((m) => m.MonProfilPage),
  },
  {
    path: 'profils/:id',
    canActivate: [roleGuard('PROPRIETAIRE')],
    loadComponent: () => import('../pages/profil/profil-detail.page').then((m) => m.ProfilDetailPage),
  },
];
