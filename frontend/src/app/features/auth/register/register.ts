import { Component, inject, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth';
import { Role } from '../../../core/models/auth.models';

@Component({
  selector: 'app-register',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly erreur = signal<string | null>(null);

  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    motDePasse: ['', [Validators.required, Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$/)]],
    role: ['ETUDIANT' as Role, [Validators.required]],
    nom: ['', [Validators.required]],
    prenom: ['', [Validators.required]],
  });

  onSubmit(): void {
    if (this.form.invalid) {
      return;
    }
    this.auth.register(this.form.getRawValue()).subscribe({
      next: () => this.router.navigate(['/']),
      error: (err) => this.erreur.set(err.error?.message ?? 'Inscription impossible'),
    });
  }
}
