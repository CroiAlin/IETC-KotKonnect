// Reçu du backend (ProfilDto)
export interface Profil {
  profilID: number;
  utilisateurID: number;
  nom: string;
  prenom: string;
  telephone?: string | null;
  ville?: string | null;
  ecole?: string | null;
  budgetMax?: number | null;
}

// Envoyé en PUT pour modifier son propre profil
export interface UpdateProfilRequest {
  nom: string;
  prenom: string;
  telephone?: string | null;
  ville?: string | null;
  ecole?: string | null;
  budgetMax?: number | null;
}
