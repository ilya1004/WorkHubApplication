import { Routes } from '@angular/router';
import {LoginPageComponent} from './common/pages/login-page/login-page.component';
import {RegisterPageComponent} from './common/pages/register-page/register-page.component';
import {HomeComponent as FreelancerHomeComponent} from './freelancer-app/pages/home/home.component';
import {MyProjectsComponent as FreelancerMyProjectsComponent} from './freelancer-app/pages/my-projects/my-projects.component';
import {canActivateFreelancerAuth} from './common/services/auth/access.guard';
import {LayoutComponent as FreelancerLayoutComponent} from './freelancer-app/components/layout/layout.component';
import {ConfirmEmailComponent} from './common/pages/confirm-email/confirm-email.component';


export const routes: Routes = [
  {path: 'login', component: LoginPageComponent},
  {path: 'register', component: RegisterPageComponent},
  {path: 'confirm-email', component: ConfirmEmailComponent},
  {
    path: 'freelancer', component: FreelancerLayoutComponent, children: [
      {path: 'home', component: FreelancerHomeComponent},
      {path: 'my-projects', component: FreelancerMyProjectsComponent},
    ],
    canActivate: [canActivateFreelancerAuth]
  }

];
