export type StatutBien = 'BROUILLON' | 'PUBLIE' | 'LOUE' | 'SUPPRIME';

export interface Photo {
  photoID: number;
  urlImage: string;
  ordre: number;
}

export interface Bien {
  bienID: number;
  proprietaireID: number;
  titre: string;
  description?: string | null;
  adresse: string;
  ville: string;
  codePostal: string;
  surface: number;
  nombrePieces: number;
  loyerBase: number;
  charges: number;
  statut: StatutBien;
  photos: Photo[];
}

export interface CreateBienRequest {
  titre: string;
  description?: string | null;
  adresse: string;
  ville: string;
  codePostal: string;
  surface: number;
  nombrePieces: number;
  loyerBase: number;
  charges: number;
}

export interface UpdateBienRequest extends CreateBienRequest {
  statut: StatutBien;
}
