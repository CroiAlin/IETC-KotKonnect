export type StatutCandidature = 'ENVOYE' | 'VU' | 'ACCEPTE' | 'REFUSE';

// Reçu du backend (aplati).
export interface Candidature {
  candidatureID: number;
  bienID: number;
  etudiantID: number;
  messageEtudiant?: string | null;
  statut: StatutCandidature;
  dateCandidature: string; // ISO (ex: "2026-06-10T09:15:00")
  titreBien: string;
  villeBien: string;
  emailEtudiant?: string | null; // renseigné seulement côté propriétaire
}

// Envoyé pour postuler
export interface CreateCandidatureRequest {
  bienID: number;
  messageEtudiant?: string | null;
}

// Envoyé pour changer le statut
export interface UpdateStatutRequest {
  statut: StatutCandidature;
}
