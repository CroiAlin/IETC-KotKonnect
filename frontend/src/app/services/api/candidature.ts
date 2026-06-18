import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Candidature,
  CreateCandidatureRequest,
  StatutCandidature,
} from './models/candidature.models';

@Injectable({ providedIn: 'root' })
export class CandidatureService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/candidatures`;

  // Étudiant : postuler à un bien publié
  postuler(req: CreateCandidatureRequest): Observable<Candidature> {
    return this.http.post<Candidature>(this.url, req);
  }

  // Étudiant : ses propres candidatures
  getMesCandidatures(): Observable<Candidature[]> {
    return this.http.get<Candidature[]>(`${this.url}/mes-candidatures`);
  }

  // Propriétaire : candidatures reçues sur ses biens
  getRecues(): Observable<Candidature[]> {
    return this.http.get<Candidature[]>(`${this.url}/recues`);
  }

  // Propriétaire : faire évoluer le statut (VU / ACCEPTE / REFUSE)
  changerStatut(id: number, statut: StatutCandidature): Observable<void> {
    return this.http.put<void>(`${this.url}/${id}/statut`, { statut });
  }
}
