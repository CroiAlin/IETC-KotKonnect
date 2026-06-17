import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth-guard';
import { roleGuard } from './core/guards/role-guard';


export const routes: Routes = [
    {
        path: 'login',
        loadComponent: () => import('./features/auth/login/login').then((m) => m.Login),
    },
    {
        path: 'register',
        loadComponent: () => import('./features/auth/register/register').then((m) => m.Register),
    },
    {
        path: 'biens',
        loadComponent: () => import('./features/biens/biens-list/biens-list').then((m) => m.BiensList),
    },
    {
        path: 'biens/nouveau',
        canActivate: [roleGuard('PROPRIETAIRE')],
        loadComponent: () => import('./features/biens/bien-form/bien-form').then((m) => m.BienForm),
    },
    {
        path: 'biens/:id/modifier',
        canActivate: [roleGuard('PROPRIETAIRE')],
        loadComponent: () => import('./features/biens/bien-form/bien-form').then((m) => m.BienForm),
    },
    {
        path: 'biens/:id',
        loadComponent: () => import('./features/biens/bien-detail/bien-detail').then((m) => m.BienDetail),
    },
    {
        path: 'mes-biens',
        canActivate: [roleGuard('PROPRIETAIRE')],
        loadComponent: () => import('./features/biens/mes-biens/mes-biens').then((m) => m.MesBiens),
    },
    {
        path: 'mes-candidatures',
        canActivate: [roleGuard('ETUDIANT')],
        loadComponent: () =>
            import('./features/candidatures/mes-candidatures/mes-candidatures').then((m) => m.MesCandidatures),
    },
    {
        path: 'candidatures-recues',
        canActivate: [roleGuard('PROPRIETAIRE')],
        loadComponent: () =>
            import('./features/candidatures/candidatures-recues/candidatures-recues').then((m) => m.CandidaturesRecues),
    },
    {
        path: 'mon-profil',
        canActivate: [authGuard],
        loadComponent: () => import('./features/profil/mon-profil/mon-profil').then((m) => m.MonProfil),
    },
    {
        path: 'profils/:id',
        canActivate: [roleGuard('PROPRIETAIRE')],
        loadComponent: () => import('./features/profil/profil-detail/profil-detail').then((m) => m.ProfilDetail),
    },
    {
        path: '',
        canActivate: [authGuard],
        loadComponent: () => import('./features/home/home').then((m) => m.Home),
    },
    { path: '**', redirectTo: ''},
];
