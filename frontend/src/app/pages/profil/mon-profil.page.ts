import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { ProfilService } from '../../services/api/profil';

@Component({
  selector: 'app-mon-profil-page',
  imports: [ReactiveFormsModule],
  templateUrl: './mon-profil.page.html',
  styleUrl: './mon-profil.page.css',
})
export class MonProfilPage {
  private readonly fb = inject(FormBuilder);
  private readonly profilService = inject(ProfilService);
  private readonly toastr = inject(ToastrService);

  readonly chargement = signal(true);
  readonly enCours = signal(false);

  readonly form = this.fb.nonNullable.group({
    nom: ['', [Validators.required]],
    prenom: ['', [Validators.required]],
    telephone: [''],
    ville: [''],
    ecole: [''],
    budgetMax: this.fb.control<number | null>(null),
  });

  constructor() {
    // Pré-remplissage du formulaire.
    this.profilService.getMonProfil().subscribe({
      next: (profil) => {
        this.form.patchValue({
          nom: profil.nom,
          prenom: profil.prenom,
          telephone: profil.telephone ?? '',
          ville: profil.ville ?? '',
          ecole: profil.ecole ?? '',
          budgetMax: profil.budgetMax ?? null,
        });
        this.chargement.set(false);
      },
      error: () => {
        this.toastr.error('Impossible de charger ton profil.');
        this.chargement.set(false);
      },
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      return;
    }

    const valeurs = this.form.getRawValue();

    this.enCours.set(true);

    this.profilService.updateMonProfil(valeurs).subscribe({
      next: () => {
        this.toastr.success('Profil mis à jour !');
        this.enCours.set(false);
      },
      error: () => {
        this.toastr.error('Mise à jour impossible');
        this.enCours.set(false);
      },
    });
  }
}
