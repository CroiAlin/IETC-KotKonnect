import { Component, input, output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Candidature, StatutCandidature } from '../../services/api/models/candidature.models';

// Présentationnel réutilisé par 2 pages ; émet l'action via output.
@Component({
  selector: 'app-candidature-card',
  imports: [DatePipe, RouterLink],
  templateUrl: './candidature-card.component.html',
  styleUrl: './candidature-card.component.css',
})
export class CandidatureCard {
  readonly candidature = input.required<Candidature>();
  readonly vueProprietaire = input(false);
  readonly changerStatut = output<{ id: number; statut: StatutCandidature }>();
}
