import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from '../../../../environments/environment';
import { MatchResult } from '../models/match-result.model';
import { MatchSummary } from '../models/match-summary.model';

@Injectable({ providedIn: 'root' })
export class MatchingService {
  private readonly baseUrl = `${environment.apiUrl}/api/matching`;

  constructor(private http: HttpClient) {}

  runMatching(systemFile: File, providerFile: File): Observable<MatchSummary> {
    const form = new FormData();
    form.append('systemFile', systemFile);
    form.append('providerFile', providerFile);
    return this.http.post<MatchSummary>(`${this.baseUrl}/run`, form);
  }

  getResults(filter: string = 'all'): Observable<MatchResult[]> {
    const params = new HttpParams().set('filter', filter);
    return this.http.get<MatchResult[]>(`${this.baseUrl}/results`, { params });
  }

  getSummary(): Observable<MatchSummary> {
    return this.http.get<MatchSummary>(`${this.baseUrl}/summary`);
  }

  resolve(id: number, resolutionSide: string): Observable<MatchResult> {
    return this.http.put<MatchResult>(`${this.baseUrl}/${id}/resolve`, { resolutionSide });
  }
}
