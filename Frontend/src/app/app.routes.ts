import { Routes } from '@angular/router';
import {LoginPageComponent} from './common/pages/login-page/login-page.component';
import {RegisterPageComponent} from './common/pages/register-page/register-page.component';
import {HomeComponent as FreelancerHomeComponent} from './freelancer-app/pages/home/home.component';
import {MyProjectsComponent as FreelancerMyProjectsComponent} from './freelancer-app/pages/my-projects/my-projects.component';
import {canActivateFreelancerAuth} from './core/services/auth/access.guard';
import {LayoutComponent as FreelancerLayoutComponent} from './freelancer-app/components/layout/layout.component';
import {ConfirmEmailComponent} from './common/pages/confirm-email/confirm-email.component';
import {ProfileComponent as FreelancerProfileComponent} from './freelancer-app/pages/profile/profile.component';
import {ProjectInfoComponent as FreelancerProjectInfoComponent} from "./freelancer-app/pages/project-info/project-info.component";
import {MyProjectInfoComponent as FreelancerMyProjectInfoComponent} from "./freelancer-app/pages/my-project-info/my-project-info.component";
import {ForgotPasswordComponent} from './common/pages/forgot-password/forgot-password.component';
import {ResetPasswordComponent} from './common/pages/reset-password/reset-password.component';


export const routes: Routes = [
  { path: 'login', component: LoginPageComponent },
  { path: 'register', component: RegisterPageComponent },
  { path: 'confirm-email', component: ConfirmEmailComponent },
  { path: 'forgot-password', component: ForgotPasswordComponent },
  { path: 'reset-password', component: ResetPasswordComponent },
  {
    path: 'freelancer', component: FreelancerLayoutComponent, children: [
      { path: 'home', component: FreelancerHomeComponent },
      { path: 'home/project/:projectId', component: FreelancerProjectInfoComponent },
      { path: 'my-projects', component: FreelancerMyProjectsComponent },
      { path: 'my-projects/:projectId', component: FreelancerMyProjectInfoComponent },
      { path: 'my-profile', component: FreelancerProfileComponent },
    ],
    canActivate: [canActivateFreelancerAuth]
  }
];
