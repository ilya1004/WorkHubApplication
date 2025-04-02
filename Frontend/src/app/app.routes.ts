import { Routes } from '@angular/router';
import {LoginPageComponent} from './common/pages/login-page/login-page.component';
import {RegisterPageComponent} from './common/pages/register-page/register-page.component';

export const routes: Routes = [
  {path: '', component: LoginPageComponent},
  {path: 'register', component: RegisterPageComponent},
];
