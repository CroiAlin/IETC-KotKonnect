import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProfilService } from '../../../core/services/profil';
import { Profil } from '../../../core/models/profil.models';

@Component({
  selector: 'app-profil-detail',
  imports: [RouterLink],
  templateUrl: './profil-detail.html',
  styleUrl: './profil-detail.css',
})
export class ProfilDetail {
  private readonly route = inject(ActivatedRoute);
  private readonly profilService = inject(ProfilService);

  readonly profil = signal<Profil | null>(null);
  readonly chargement = signal(true);
  readonly erreur = signal<string | null>(null);

  constructor() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.profilService.getProfil(id).subscribe({
      next: (profil) => {
        this.profil.set(profil);
        this.chargement.set(false);
      },
      error: (err) => {
        // 403 = le backend a refusé (cet étudiant n'a pas postulé chez ce proprio).
        this.erreur.set(
          err.status === 403 ? "Tu n'as pas accès à ce profil." : 'Profil introuvable.',
        );
        this.chargement.set(false);
      },
    });
  }
}
