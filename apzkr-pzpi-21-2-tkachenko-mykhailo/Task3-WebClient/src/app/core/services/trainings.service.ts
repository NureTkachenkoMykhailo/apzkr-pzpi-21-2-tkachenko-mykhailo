import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AuthService } from './auth.service';
import { ResponseObject } from '../models/responseObject';
import { Training } from '../models/training';

@Injectable({
  providedIn: 'root'
})
export class TrainingService {
  private apiUrl = 'https://localhost:7112/backoffice/trainings';

  constructor(private http: HttpClient, private authService: AuthService) {}

  private getAuthHeaders(): HttpHeaders {
    const token = this.authService.getToken()?.accessToken;
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json'
    });
  }

  getTrainings(): Observable<Training[]> {
    return this.http.get<ResponseObject<Training[]>>(this.apiUrl, { headers: this.getAuthHeaders() })
      .pipe(map(response => response.payload || []));
  }

  getTrainingById(id: number): Observable<Training> {
    return this.http.get<ResponseObject<Training>>(`${this.apiUrl}/${id}`, { headers: this.getAuthHeaders() })
      .pipe(map(response => response.payload as Training));
  }

  createTraining(training: Training): Observable<Training> {
    return this.http.post<ResponseObject<Training>>(this.apiUrl, training, { headers: this.getAuthHeaders() })
      .pipe(map(response => response.payload as Training));
  }

  updateTraining(id: number, training: Training): Observable<Training> {
    return this.http.patch<ResponseObject<Training>>(`${this.apiUrl}/${id}`, training, { headers: this.getAuthHeaders() })
      .pipe(map(response => response.payload as Training));
  }

  deleteTraining(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers: this.getAuthHeaders() });
  }
}
