import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Bien, CreateBienRequest, UpdateBienRequest } from '../models/bien.models';

@Injectable({ providedIn: 'root' })
export class BienService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/biens`;

  // Liste publique (biens PUBLIE). Le token n'est pas requis, mais l'interceptor
  // l'ajoute s'il existe — sans effet ici.
  getPublies(): Observable<Bien[]> {
    return this.http.get<Bien[]>(this.url);
  }

  getById(id: number): Observable<Bien> {
    return this.http.get<Bien>(`${this.url}/${id}`);
  }

  // Protégés : l'interceptor ajoute automatiquement le Bearer.
  getMesBiens(): Observable<Bien[]> {
    return this.http.get<Bien[]>(`${this.url}/mes-biens`);
  }

  create(req: CreateBienRequest): Observable<Bien> {
    return this.http.post<Bien>(this.url, req);
  }

  update(id: number, req: UpdateBienRequest): Observable<void> {
    return this.http.put<void>(`${this.url}/${id}`, req);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }

  // Photos (protégés : l'interceptor ajoute le Bearer)
  addPhoto(bienId: number, urlImage: string): Observable<void> {
    return this.http.post<void>(`${this.url}/${bienId}/photos`, { urlImage });
  }

  deletePhoto(bienId: number, photoId: number): Observable<void> {
    return this.http.delete<void>(`${this.url}/${bienId}/photos/${photoId}`);
  }
}
