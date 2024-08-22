import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { Router } from '@angular/router';
import { AuthenticateRequest } from '../../core/models/authRequest';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginForm: FormGroup;
  loginError: string | null = null;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      const loginData: AuthenticateRequest = {
        Contact: this.loginForm.value.username,
        Password: this.loginForm.value.password
      };

      this.authService.login(loginData).subscribe(
        () => {
          console.log('Login successful');
          this.router.navigate(['/backoffice']);
        },
        error => {
          console.error('Login failed:', error);
          this.loginError = 'Invalid username or password';
        }
      );
    } else {
      this.loginError = 'Please fill in all required fields';
    }
  }
}
