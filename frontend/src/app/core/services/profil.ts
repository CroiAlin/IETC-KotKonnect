import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Profil, UpdateProfilRequest } from '../models/profil.models';

@Injectable({ providedIn: 'root' })
export class ProfilService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/profils`;

  // Profil de l'utilisateur connecté
  getMonProfil(): Observable<Profil> {
    return this.http.get<Profil>(`${this.url}/me`);
  }

  // Mise à jour du profil de l'utilisateur connecté
  updateMonProfil(req: UpdateProfilRequest): Observable<Profil> {
    return this.http.put<Profil>(`${this.url}/me`, req);
  }

  // Profil d'un autre utilisateur
  getProfil(utilisateurId: number): Observable<Profil> {
    return this.http.get<Profil>(`${this.url}/${utilisateurId}`);
  }
}
