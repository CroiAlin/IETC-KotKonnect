import { Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DatePipe } from '@angular/common';
import { CandidatureService } from '../../../core/services/candidature';
import { Candidature } from '../../../core/models/candidature.models';

@Component({
  selector: 'app-mes-candidatures',
  imports: [RouterLink, DatePipe],
  templateUrl: './mes-candidatures.html',
  styleUrl: './mes-candidatures.css',
})
export class MesCandidatures {
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
