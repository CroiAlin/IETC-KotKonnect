import { Component, inject, signal } from '@angular/core';
import { BienService } from '../../../core/services/bien';
import { Bien } from '../../../core/models/bien.models';

@Component({
  selector: 'app-biens-list',
  templateUrl: './biens-list.html',
  styleUrl: './biens-list.css',
})
export class BiensList {
  private readonly bienService = inject(BienService);

  readonly biens = signal<Bien[]>([]);
  readonly chargement = signal(true);
  readonly erreur = signal<string | null>(null);

  constructor() {
    this.bienService.getPublies().subscribe({
      next: (biens) => {
        this.biens.set(biens);
        this.chargement.set(false);
      },
      error: () => {
        this.erreur.set('Impossible de charger les biens.');
        this.chargement.set(false);
      },
    });
  }
}
