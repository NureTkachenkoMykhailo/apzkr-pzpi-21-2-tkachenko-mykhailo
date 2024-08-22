import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {Observable} from 'rxjs';
import {map} from 'rxjs/operators';
import {Identity} from "../models/identity";
import {ResponseObject} from "../models/responseObject";
import {AuthService} from "./auth.service";

@Injectable({
  providedIn: 'root'
})
export class InstructorService {
  private apiUrl = 'https://localhost:7112/backoffice/instructors'; // Update with your actual API URL

  constructor(private http: HttpClient, private authService: AuthService) { }

  getInstructors(): Observable<Identity[]> {
    const token = this.authService.getToken()?.accessToken;

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    return this.http.get<ResponseObject<Identity[]>>(this.apiUrl, { headers }).pipe(
      map(response => response.payload)
    );
  }

  deleteInstructor(id: number): Observable<void> {
    const token = this.authService.getToken()?.accessToken;

    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });

    return this.http.delete<void>(`${this.apiUrl}/${id}`, { headers });
  }
}
