import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from "./core/guards/auth.guard";
import { LoginComponent } from "./backoffice/login/login.component";
import {LoginGuard} from "./core/guards/login.guard";

const routes: Routes = [
  { path: 'backoffice/login', component: LoginComponent, canActivate: [LoginGuard] },
  {
    path: '',
    loadChildren: () =>
      import('./static-content/static-content.module')
        .then(m => m.StaticContentModule)
  },
  {
    path: 'backoffice',
    loadChildren: () =>
      import('./backoffice/backoffice.module')
        .then(m => m.BackofficeModule),
    canActivate: [AuthGuard]
  },
  { path: '**', redirectTo: '' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
