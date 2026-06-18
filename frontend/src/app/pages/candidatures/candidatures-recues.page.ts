import { Component, inject, signal } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { CandidatureService } from '../../services/api/candidature';
import { Candidature, StatutCandidature } from '../../services/api/models/candidature.models';
import { CandidatureCard } from '../../components/candidatures/candidature-card.component';

@Component({
  selector: 'app-candidatures-recues-page',
  imports: [CandidatureCard],
  templateUrl: './candidatures-recues.page.html',
  styleUrl: './candidatures-recues.page.css',
})
export class CandidaturesRecuesPage {
  private readonly candidatureService = inject(CandidatureService);
  private readonly toastr = inject(ToastrService);

  readonly candidatures = signal<Candidature[]>([]);
  readonly chargement = signal(true);
  readonly erreur = signal<string | null>(null);

  constructor() {
    this.candidatureService.getRecues().subscribe({
      next: (candidatures) => {
        this.candidatures.set(candidatures);
        this.chargement.set(false);
      },
      error: () => {
        this.erreur.set('Impossible de charger les candidatures reçues.');
        this.chargement.set(false);
      },
    });
  }

  changerStatut(id: number, statut: StatutCandidature): void {
    this.candidatureService.changerStatut(id, statut).subscribe({
      next: () => {
        this.candidatures.update((liste) =>
          liste.map((c) => (c.candidatureID === id ? { ...c, statut } : c)),
        );
        this.toastr.success('Statut mis à jour.');
      },
      error: () => this.toastr.error('Impossible de changer le statut'),
    });
  }
}
