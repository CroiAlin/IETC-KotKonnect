import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Bien } from '../../services/api/models/bien.models';

// Présentationnel : carte d'un bien.
@Component({
  selector: 'app-bien-card',
  imports: [RouterLink],
  templateUrl: './bien-card.component.html',
  styleUrl: './bien-card.component.css',
})
export class BienCard {
  readonly bien = input.required<Bien>();
}
