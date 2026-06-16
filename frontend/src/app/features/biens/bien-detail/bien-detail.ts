import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../../core/services/auth';
import { CandidatureService } from '../../../core/services/candidature';
import { BienService } from '../../../core/services/bien';
import { Bien } from '../../../core/models/bien.models';

@Component({
  selector: 'app-bien-detail',
  imports: [RouterLink],
  templateUrl: './bien-detail.html',
  styleUrl: './bien-detail.css',
})
export class BienDetail {
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

  // Photo affichée en grand dans la galerie
  readonly photoActive = signal<string | null>(null);

  constructor() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.bienService.getById(id).subscribe({
      next: (bien) => {
        this.bien.set(bien);
        this.photoActive.set(bien.photos[0]?.urlImage ?? null);
        this.chargement.set(false);
      },
      error: () => {
        this.erreur.set('Bien introuvable.');
        this.chargement.set(false);
      },
    });
  }

  choisirPhoto(url: string): void {
    this.photoActive.set(url);
  }

  //Etudiant fait une candidature pour un logement
  postuler(): void {
    const bien = this.bien();
    if (!bien) return;

    this.enCours.set(true);
    this.candidatureService.postuler({ bienID: bien.bienID, messageEtudiant: this.message() || null}).subscribe({
      next: () => {
        this.toastr.success('Candidature envoyée !');
        this.message.set('');
        this.enCours.set(false);
      },
      error: (err) => {
        this.enCours.set(false);
        if (err.status === 409){
          this.toastr.warning('Tu as déjà postulé à ce logement.');
        }
        else{
          this.toastr.error("Impossible d'envoyer la candidature.");
        }
      },
    });
  }
}
