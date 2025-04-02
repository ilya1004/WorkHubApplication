import { Routes } from '@angular/router';
import {LoginPageComponent} from './common/pages/login-page/login-page.component';
import {RegisterPageComponent} from './common/pages/register-page/register-page.component';
import {HomeComponent as FreelancerHomeComponent} from './freelancer-app/pages/home/home.component';
import {MyProjectsComponent as FreelancerMyProjectsComponent} from './freelancer-app/pages/my-projects/my-projects.component';


export const routes: Routes = [
  {path: 'login', component: LoginPageComponent},
  {path: 'register', component: RegisterPageComponent},
  {path: 'freelancer/home', component: FreelancerHomeComponent},
  {path: 'freelancer/my-projects', component: FreelancerMyProjectsComponent},
];
