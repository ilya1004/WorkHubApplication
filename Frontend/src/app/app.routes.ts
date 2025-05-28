import {Routes} from '@angular/router';
import {LoginPageComponent} from './common/pages/login-page/login-page.component';
import {RegisterPageComponent} from './common/pages/register-page/register-page.component';
import {ForgotPasswordComponent} from './common/pages/forgot-password/forgot-password.component';
import {ResetPasswordComponent} from './common/pages/reset-password/reset-password.component';
import {ConfirmEmailComponent} from './common/pages/confirm-email/confirm-email.component';
import {NotFoundComponent} from "./common/pages/not-found/not-found.component";
import {RootRedirectComponent} from "./common/components/root-redirect/root-redirect.component";

import {canActivateFreelancerApp} from './core/guards/freelancer-auth.guard';
import {HomeComponent as FreelancerHomeComponent} from './freelancer-app/pages/home/home.component';
import {MyProjectsComponent as FreelancerMyProjectsComponent} from './freelancer-app/pages/my-projects/my-projects.component';
import {LayoutComponent as FreelancerLayoutComponent} from './freelancer-app/components/layout/layout.component';
import {ProjectInfoComponent as FreelancerProjectInfoComponent} from "./freelancer-app/pages/project-info/project-info.component";
import {MyProjectInfoComponent as FreelancerMyProjectInfoComponent} from "./freelancer-app/pages/my-project-info/my-project-info.component";
import {MyApplicationsComponent} from "./freelancer-app/pages/my-applications/my-applications.component";
import {MyFinancesComponent as FreelancerMyFinancesComponent} from "./freelancer-app/pages/my-finances/my-finances.component";
import {ProfileComponent as FreelancerProfileComponent} from './freelancer-app/pages/profile/profile.component';

import {canActivateEmployerApp} from "./core/guards/employer-auth.guard";
import {LayoutComponent as EmployerLayoutComponent} from "./employer-app/components/layout/layout.component";
import {ProfileComponent as EmployerProfileComponent} from "./employer-app/pages/profile/profile.component";
import {MyProjectsComponent as EmployerMyProjectsComponent} from "./employer-app/pages/my-projects/my-projects.component";
import {ProjectToolsComponent} from './employer-app/pages/project-tools/project-tools.component';
import {MyProjectInfoComponent as EmployerMyProjectInfoComponent} from "./employer-app/pages/my-project-info/my-project-info.component";
import {MyFinancesComponent as EmployerMyFinancesComponent} from "./employer-app/pages/my-finances/my-finances.component";

import {canActivateAdminApp} from "./core/guards/admin-auth.guard";
import {LayoutComponent as AdminLayoutComponent} from "./admin-app/components/layout/layout.component";
import {HomeComponent as AdminHomeComponent} from "./admin-app/pages/home/home.component";
import {ProjectsServiceToolsComponent} from "./admin-app/pages/projects-service-tools/projects-service-tools.component";
import {IdentityServiceToolsComponent} from "./admin-app/pages/identity-service-tools/identity-service-tools.component";
import {PaymentsServiceToolsComponent} from "./admin-app/pages/payments-service-tools/payments-service-tools.component";
import {ChatServiceToolsComponent} from "./admin-app/pages/chat-service-tools/chat-service-tools.component";
import {
  ProjectInfoComponent as AdminProjectInfoComponent
} from "./admin-app/pages/projects-service-tools/project-info/project-info.component";
import {UserInfoComponent} from "./admin-app/pages/identity-service-tools/user-info/user-info.component";


export const routes: Routes = [
  { path: '', component: RootRedirectComponent },
  { path: 'login', component: LoginPageComponent },
  { path: 'register', component: RegisterPageComponent },
  { path: 'confirm-email', component: ConfirmEmailComponent },
  { path: 'forgot-password', component: ForgotPasswordComponent },
  { path: 'reset-password', component: ResetPasswordComponent },
  {
    path: 'freelancer',
    component: FreelancerLayoutComponent,
    canActivate: [canActivateFreelancerApp],
    children: [
      { path: 'home', component: FreelancerHomeComponent },
      { path: 'home/project/:projectId', component: FreelancerProjectInfoComponent },
      { path: 'my-projects', component: FreelancerMyProjectsComponent },
      { path: 'my-projects/:projectId', component: FreelancerMyProjectInfoComponent },
      { path: 'my-applications', component: MyApplicationsComponent },
      { path: 'my-finances', component: FreelancerMyFinancesComponent },
      { path: 'my-profile', component: FreelancerProfileComponent },
    ],
  },
  {
    path: 'employer',
    component: EmployerLayoutComponent,
    canActivate: [canActivateEmployerApp],
    children: [
      { path: 'my-projects', component: EmployerMyProjectsComponent },
      { path: 'my-projects/:projectId', component: EmployerMyProjectInfoComponent },
      { path: 'project-tools', component: ProjectToolsComponent },
      { path: 'my-finances', component: EmployerMyFinancesComponent },
      { path: 'my-profile', component: EmployerProfileComponent },
    ]
  },
  {
    path: 'admin',
    component: AdminLayoutComponent,
    canActivate: [canActivateAdminApp],
    children: [
      { path: 'home', component: AdminHomeComponent },
      { path: 'projects-service-tools', component: ProjectsServiceToolsComponent},
      { path: 'projects-service-tools/project/:projectId', component: AdminProjectInfoComponent },
      { path: 'identity-service-tools', component: IdentityServiceToolsComponent},
      { path: 'identity-service-tools/user/:userId', component: UserInfoComponent},
      { path: 'payments-service-tools', component: PaymentsServiceToolsComponent},
      { path: 'chat-service-tools', component: ChatServiceToolsComponent}
    ]
  },
  { path: '**', component: NotFoundComponent }
];
