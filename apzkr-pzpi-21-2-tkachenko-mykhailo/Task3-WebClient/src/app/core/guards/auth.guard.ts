import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authService: AuthService, private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): boolean {
    const isAuthenticated = this.authService.isAuthenticated()
    console.log("authService.isAuthenticated from auth guard: ", isAuthenticated);
    if (isAuthenticated) {
      return true;
    } else {
      this.router.navigate(['/backoffice/login']);
      return false;
    }
  }
}
