import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Track } from "../models/track";
import { AuthService } from "./auth.service";
import {ResponseObject} from "../models/responseObject";

@Injectable({
  providedIn: 'root'
})
export class TrackService {
  private apiUrl = 'https://localhost:7112/backoffice/tracks';

  constructor(private http: HttpClient, private authService: AuthService) {}

  private getAuthHeaders(): HttpHeaders {
    const token = this.authService.getToken()?.accessToken;
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }

  getTracks(): Observable<Track[]> {
    return this.http.get<ResponseObject<Track[]>>(this.apiUrl, { headers: this.getAuthHeaders() })
      .pipe(map(response => response.payload || []));
  }

  getTrackById(id: number): Observable<Track> {
    return this.http.get<ResponseObject<Track>>(`${this.apiUrl}/${id}`, { headers: this.getAuthHeaders() })
      .pipe(map(response => response.payload as Track));
  }

  createTrack(track: Track): Observable<Track> {
    return this.http.post<ResponseObject<Track>>(this.apiUrl, track, { headers: this.getAuthHeaders() })
      .pipe(map(response => response.payload as Track));
  }

  updateTrack(id: number, track: Track): Observable<Track> {
    return this.http.patch<ResponseObject<Track>>(`${this.apiUrl}/${id}`, track, { headers: this.getAuthHeaders() })
      .pipe(map(response => response.payload as Track));
  }

  deleteTrack(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers: this.getAuthHeaders() });
  }
}
