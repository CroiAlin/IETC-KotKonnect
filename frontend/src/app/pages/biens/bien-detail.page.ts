import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../services/api/auth';
import { CandidatureService } from '../../services/api/candidature';
import { BienService } from '../../services/api/bien';
import { Bien } from '../../services/api/models/bien.models';
import { PhotoGallery } from '../../components/biens/photo-gallery.component';

@Component({
  selector: 'app-bien-detail-page',
  imports: [RouterLink, PhotoGallery],
  templateUrl: './bien-detail.page.html',
  styleUrl: './bien-detail.page.css',
})
export class BienDetailPage {
  private readonly route = inject(ActivatedRoute);
  private readonly bienService = inject(BienService);
  private readonly auth = inject(AuthService);
  private readonly candidatureService = inject(CandidatureService);
  private readonly toastr = inject(ToastrService);

  readonly bien = signal<Bien | null>(null);
  readonly chargement = signal(true);
  readonly erreur = signal<string | null>(null);
  readonly estEtudiant = computed(() => this.auth.user()?.role === 'ETUDIANT');
  readonly enCours = signal(false);
  readonly message = signal('');

  constructor() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.bienService.getById(id).subscribe({
      next: (bien) => {
        this.bien.set(bien);
        this.chargement.set(false);
      },
      error: () => {
        this.erreur.set('Bien introuvable.');
        this.chargement.set(false);
      },
    });
  }

  // Étudiant : envoie une candidature.
  postuler(): void {
    const bien = this.bien();
    if (!bien) return;

    this.enCours.set(true);
    this.candidatureService
      .postuler({ bienID: bien.bienID, messageEtudiant: this.message() || null })
      .subscribe({
        next: () => {
          this.toastr.success('Candidature envoyée !');
          this.message.set('');
          this.enCours.set(false);
        },
        error: (err) => {
          this.enCours.set(false);
          if (err.status === 409) {
            this.toastr.warning('Tu as déjà postulé à ce logement.');
          } else {
            this.toastr.error("Impossible d'envoyer la candidature.");
          }
        },
      });
  }
}
