import { Component, computed, input, signal } from '@angular/core';
import { Photo } from '../../services/api/models/bien.models';

// Présentationnel : galerie de photos d'un bien.
@Component({
  selector: 'app-photo-gallery',
  imports: [],
  templateUrl: './photo-gallery.component.html',
  styleUrl: './photo-gallery.component.css',
})
export class PhotoGallery {
  readonly photos = input.required<Photo[]>();
  readonly titre = input('');

  // Clic prime, sinon la 1re photo.
  private readonly choixManuel = signal<string | null>(null);
  readonly photoActive = computed(() => this.choixManuel() ?? this.photos()[0]?.urlImage ?? null);

  choisirPhoto(url: string): void {
    this.choixManuel.set(url);
  }
}
