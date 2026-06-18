import { Routes } from '@angular/router';
import { authGuard } from './services/guards/auth-guard';
import { authRoutes } from './routes/auth.routes';
import { biensRoutes } from './routes/biens.routes';
import { candidaturesRoutes } from './routes/candidatures.routes';
import { profilRoutes } from './routes/profil.routes';

// On assemble les routes feature par feature.
export const routes: Routes = [
  ...authRoutes,
  ...biensRoutes,
  ...candidaturesRoutes,
  ...profilRoutes,
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () => import('./pages/home/home.page').then((m) => m.HomePage),
  },
  { path: '**', redirectTo: '' },
];
