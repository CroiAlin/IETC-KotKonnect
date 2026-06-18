import { Routes } from '@angular/router';
import { roleGuard } from '../services/guards/role-guard';

// Routes de la feature Candidatures.
export const candidaturesRoutes: Routes = [
  {
    path: 'mes-candidatures',
    canActivate: [roleGuard('ETUDIANT')],
    loadComponent: () =>
      import('../pages/candidatures/mes-candidatures.page').then((m) => m.MesCandidaturesPage),
  },
  {
    path: 'candidatures-recues',
    canActivate: [roleGuard('PROPRIETAIRE')],
    loadComponent: () =>
      import('../pages/candidatures/candidatures-recues.page').then((m) => m.CandidaturesRecuesPage),
  },
];
