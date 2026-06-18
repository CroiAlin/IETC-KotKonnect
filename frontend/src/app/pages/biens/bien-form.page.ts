import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { BienService } from '../../services/api/bien';
import { Photo, StatutBien } from '../../services/api/models/bien.models';

@Component({
  selector: 'app-bien-form-page',
  imports: [ReactiveFormsModule],
  templateUrl: './bien-form.page.html',
  styleUrl: './bien-form.page.css',
})
export class BienFormPage {
  private readonly fb = inject(FormBuilder);
  private readonly bienService = inject(BienService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly erreur = signal<string | null>(null);
  readonly modeEdition = signal(false);
  readonly photos = signal<Photo[]>([]);

  private bienId: number | null = null;
  private statutActuel: StatutBien = 'BROUILLON';

  readonly form = this.fb.nonNullable.group({
    titre: ['', [Validators.required]],
    description: [''],
    adresse: ['', [Validators.required]],
    ville: ['', [Validators.required]],
    codePostal: ['', [Validators.required]],
    surface: [0, [Validators.required, Validators.min(1)]],
    nombrePieces: [1, [Validators.required, Validators.min(1)]],
    loyerBase: [0, [Validators.required, Validators.min(0)]],
    charges: [0, [Validators.required, Validators.min(0)]],
  });

  constructor() {
    // id dans l'URL -> mode édition.
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.bienId = Number(idParam);
      this.modeEdition.set(true);
      this.bienService.getById(this.bienId).subscribe({
        next: (bien) => {
          this.statutActuel = bien.statut;
          this.photos.set(bien.photos);
          this.form.patchValue({
            titre: bien.titre,
            description: bien.description ?? '',
            adresse: bien.adresse,
            ville: bien.ville,
            codePostal: bien.codePostal,
            surface: bien.surface,
            nombrePieces: bien.nombrePieces,
            loyerBase: bien.loyerBase,
            charges: bien.charges,
          });
        },
        error: () => this.erreur.set('Bien introuvable.'),
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid) {
      return;
    }
    const valeurs = this.form.getRawValue();

    if (this.modeEdition() && this.bienId !== null) {
      // On garde le statut (publier est une action séparée).
      this.bienService.update(this.bienId, { ...valeurs, statut: this.statutActuel }).subscribe({
        next: () => this.router.navigate(['/mes-biens']),
        error: (err) => this.erreur.set(err.error?.message ?? 'Modification impossible'),
      });
    } else {
      this.bienService.create(valeurs).subscribe({
        next: () => this.router.navigate(['/mes-biens']),
        error: (err) => this.erreur.set(err.error?.message ?? 'Impossible de créer le bien'),
      });
    }
  }

  ajouterPhoto(url: string): void {
    const lien = url.trim();
    if (!lien || this.bienId === null) {
      return;
    }
    this.bienService.addPhoto(this.bienId, lien).subscribe({
      next: () => this.rechargerPhotos(),
      error: () => this.erreur.set("Impossible d'ajouter la photo."),
    });
  }

  supprimerPhoto(photoId: number): void {
    if (this.bienId === null) {
      return;
    }
    this.bienService.deletePhoto(this.bienId, photoId).subscribe({
      next: () => this.rechargerPhotos(),
      error: () => this.erreur.set('Impossible de supprimer la photo.'),
    });
  }

  // Recharge les photos sans toucher au formulaire.
  private rechargerPhotos(): void {
    if (this.bienId === null) {
      return;
    }
    this.bienService.getById(this.bienId).subscribe({
      next: (bien) => this.photos.set(bien.photos),
    });
  }
}
