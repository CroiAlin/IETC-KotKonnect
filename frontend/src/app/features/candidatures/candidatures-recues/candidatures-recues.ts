import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ToastrService } from 'ngx-toastr';
import { CandidatureService } from '../../../core/services/candidature';
import { Candidature, StatutCandidature } from '../../../core/models/candidature.models';

@Component({
  selector: 'app-candidatures-recues',
  imports: [DatePipe],
  templateUrl: './candidatures-recues.html',
  styleUrl: './candidatures-recues.css',
})
export class CandidaturesRecues {
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
          liste.map((c) => (c.candidatureID === id ? {...c, statut} : c)),
        );
        this.toastr.success('Statut mis à jour.');
      },
      error: () => this.toastr.error('Impossible de changer le statut'),
    });
  }
}
