import { Component, inject, signal } from '@angular/core';
import { BienService } from '../../services/api/bien';
import { Bien } from '../../services/api/models/bien.models';
import { BienCard } from '../../components/biens/bien-card.component';

@Component({
  selector: 'app-biens-page',
  imports: [BienCard],
  templateUrl: './biens.page.html',
  styleUrl: './biens.page.css',
})
export class BiensPage {
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
