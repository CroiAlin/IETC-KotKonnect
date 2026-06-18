import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CandidatureService } from '../../services/api/candidature';
import { Candidature } from '../../services/api/models/candidature.models';
import { CandidatureCard } from '../../components/candidatures/candidature-card.component';

@Component({
  selector: 'app-mes-candidatures-page',
  imports: [RouterLink, CandidatureCard],
  templateUrl: './mes-candidatures.page.html',
  styleUrl: './mes-candidatures.page.css',
})
export class MesCandidaturesPage {
  private readonly candidatureService = inject(CandidatureService);

  readonly candidatures = signal<Candidature[]>([]);
  readonly chargement = signal(true);
  readonly erreur = signal<string | null>(null);

  constructor() {
    this.candidatureService.getMesCandidatures().subscribe({
      next: (candidatures) => {
        this.candidatures.set(candidatures);
        this.chargement.set(false);
      },
      error: () => {
        this.erreur.set('Impossible de charger tes candidatures.');
        this.chargement.set(false);
      },
    });
  }
}
