import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { IdentityToken } from '../models/identityToken';
import { AuthenticateRequest } from '../models/authRequest';

interface ResponseObject<T> {
  statusCode?: number;
  isSuccessfulResult: boolean;
  errors: Array<any>;
  payload?: T;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private tokenKey = 'authToken';
  private authUrl = 'https://localhost:7112/backoffice/authenticate';

  constructor(private http: HttpClient) { }

  login(authRequest: AuthenticateRequest): Observable<IdentityToken> {
    return this.http.post<ResponseObject<IdentityToken>>(this.authUrl, authRequest)
      .pipe(
        map(({isSuccessfulResult, payload}) => {
          if (isSuccessfulResult && payload) {
            return payload;
          } else {
            throw new Error('Authentication failed');
          }
        }),
        tap(token => this.setToken(token))
    );
  }

  logout() {
    localStorage.removeItem(this.tokenKey);
  }

  isAuthenticated(): boolean {
    const identityToken = this.getToken();
    if (identityToken == null) {
      return false;
    }

    const expiresAt = new Date(identityToken.expiresAt);
    const currentDate = new Date();

    console.log("expiresAt", expiresAt);
    console.log("currentDate", currentDate);

    return expiresAt > currentDate;
  }

  private setToken(token: IdentityToken) {
    console.log("Identity token set");
    console.log(token);
    localStorage.setItem(this.tokenKey, JSON.stringify(token));
  }

  getToken(): IdentityToken | null {
    const token = localStorage.getItem(this.tokenKey);
    const parsedToken: IdentityToken = token ? JSON.parse(token) : null;
    console.log("token from localstorage retrieved: ", parsedToken);

    return parsedToken;
  }
}
