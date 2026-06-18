export type Role = 'ETUDIANT' | 'PROPRIETAIRE';

export interface LoginRequest {
    email : string;
    motDePasse: string;
}

export interface RegisterRequest {
    email: string;
    motDePasse: string;
    role: Role;
    nom: string;
    prenom: string;
}

export interface AuthResponse {
    accessToken: string;
    refreshToken: string;
    email: string;
    role: Role;
    nom: string;
    prenom: string;
}