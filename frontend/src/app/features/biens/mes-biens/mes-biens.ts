import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BienService } from '../../../core/services/bien';
import { Bien } from '../../../core/models/bien.models';

@Component({
  selector: 'app-mes-biens',
  imports: [RouterLink],
  templateUrl: './mes-biens.html',
  styleUrl: './mes-biens.css',
})
export class MesBiens {
  private readonly bienService = inject(BienService);

  readonly biens = signal<Bien[]>([]);
  readonly chargement = signal(true);
  readonly erreur = signal<string | null>(null);

  constructor() {
    this.charger();
  }

  private charger(): void {
    this.chargement.set(true);
    this.bienService.getMesBiens().subscribe({
      next: (biens) => {
        this.biens.set(biens);
        this.chargement.set(false);
      },
      error: () => {
        this.erreur.set('Impossible de charger vos biens.');
        this.chargement.set(false);
      },
    });
  }

  publier(bien: Bien): void {
    this.bienService
      .update(bien.bienID, {
        titre: bien.titre,
        description: bien.description,
        adresse: bien.adresse,
        ville: bien.ville,
        codePostal: bien.codePostal,
        surface: bien.surface,
        nombrePieces: bien.nombrePieces,
        loyerBase: bien.loyerBase,
        charges: bien.charges,
        statut: 'PUBLIE',
      })
      .subscribe({
        next: () => this.charger(),
        error: () => this.erreur.set('La publication a échoué.'),
      });
  }

  supprimer(bien: Bien): void {
    if (!confirm(`Supprimer « ${bien.titre} » ?`)) {
      return;
    }
    this.bienService.delete(bien.bienID).subscribe({
      next: () => this.charger(),
      error: () => this.erreur.set('La suppression a échoué.'),
    });
  }
}
