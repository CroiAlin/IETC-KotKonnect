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
        path: 'mes-biens',
        canActivate: [roleGuard('PROPRIETAIRE')],
        loadComponent: () => import('./features/biens/mes-biens/mes-biens').then((m) => m.MesBiens),
    },
    {
        path: '',
        canActivate: [authGuard],
        loadComponent: () => import('./features/home/home').then((m) => m.Home),
    },
    { path: '**', redirectTo: ''},
];
