import {Component, OnInit} from '@angular/core';
import {Project} from "../../../core/interfaces/project/project.interface";
import {ProjectsService} from "../../../core/services/projects/projects.service";
import {CommonModule} from "@angular/common";
import {NzCardModule} from "ng-zorro-antd/card";
import {NzButtonModule} from "ng-zorro-antd/button";
import {NzDescriptionsModule} from "ng-zorro-antd/descriptions";
import {ActivatedRoute, RouterLink} from "@angular/router";
import {EmployerUser} from "../../../core/interfaces/employer/employer-user.interface";
import {FreelancerUser} from "../../../core/interfaces/freelancer/freelancer-user.interface";
import {UsersService} from "../../../core/services/users/users.service";
import {ApplicationStatus} from "../../../core/interfaces/project/freelancer-application.interface";
import {TokenService} from "../../../core/services/auth/token.service";
import {NzFlexDirective} from "ng-zorro-antd/flex";
import {FreelancerApplicationsService} from "../../../core/services/freelancer-applications/freelancer-applications.service";
import {NzTagComponent} from "ng-zorro-antd/tag";
import {ProjectStatus} from "../../../core/interfaces/project/lifecycle.interface";
import {NzMessageService} from "ng-zorro-antd/message";
import {FinanceService} from "../../services/finance.service";
import {NzAlertModule} from "ng-zorro-antd/alert";

@Component({
  selector: 'app-project-info',
  standalone: true,
  imports: [
    CommonModule,
    NzCardModule,
    NzDescriptionsModule,
    NzButtonModule,
    RouterLink,
    NzFlexDirective,
    NzTagComponent,
    NzAlertModule
  ],
  providers: [NzMessageService],
  templateUrl: './project-info.component.html',
  styleUrl: './project-info.component.scss'
})
export class ProjectInfoComponent implements OnInit {
  project: Project | null = null;
  employer: EmployerUser | null = null;
  freelancer: FreelancerUser | null = null;
  loading = false;
  isApplying = false;
  isCancelling = false;
  currentUserId: string | null = '';
  hasFreelancerAccount: boolean = true;
  isCheckingAccount: boolean = false;
  
  constructor(
    private route: ActivatedRoute,
    private projectsService: ProjectsService,
    private freelancerApplicationsService: FreelancerApplicationsService,
    private usersService: UsersService,
    private tokenService: TokenService,
    private financeService: FinanceService,
    private message: NzMessageService
  ) {
    this.currentUserId = this.tokenService.getUserId();
  }
  
  ngOnInit(): void {
    const projectId = this.route.snapshot.paramMap.get('projectId');
    if (projectId) {
      this.loadProject(projectId);
      this.checkFreelancerAccount();
    }
  }
  
  loadProject(projectId: string): void {
    this.loading = true;
    this.projectsService.getProjectById(projectId).subscribe({
      next: (project: Project) => {
        this.project = project;
        this.loadEmployer(project.employerId);
        if (project.freelancerId && project.lifecycle.status > ProjectStatus.AcceptingApplications) {
          this.loadFreelancer(project.freelancerId);
        } else {
          this.freelancer = null;
          this.loading = false;
        }
      },
      error: (error) => {
        this.message.error('Failed to load project details.', { nzDuration: 3000 });
        console.error('Error loading project:', error);
        this.loading = false;
      }
    });
  }
  
  loadEmployer(employerId: string): void {
    this.usersService.getEmployerInfo(employerId).subscribe({
      next: (employer: EmployerUser) => {
        this.employer = employer;
        this.checkLoadingComplete();
      },
      error: (error) => {
        this.message.error('Failed to load employer information.', { nzDuration: 3000 });
        console.error('Error loading employer:', error);
        this.checkLoadingComplete();
      }
    });
  }
  
  loadFreelancer(freelancerId: string): void {
    this.usersService.getFreelancerInfo(freelancerId).subscribe({
      next: (freelancer: FreelancerUser) => {
        this.freelancer = freelancer;
        this.checkLoadingComplete();
      },
      error: (error) => {
        this.message.error('Failed to load freelancer information.', { nzDuration: 3000 });
        console.error('Error loading freelancer:', error);
        this.checkLoadingComplete();
      }
    });
  }
  
  private checkLoadingComplete(): void {
    if (this.project && this.employer && (!this.project.freelancerId || this.freelancer || this.project.lifecycle.status <= 1)) {
      this.loading = false;
    }
  }
  
  checkFreelancerAccount(): void {
    this.isCheckingAccount = true;
    this.financeService.getFreelancerAccount().subscribe({
      next: () => {
        this.hasFreelancerAccount = true;
        this.isCheckingAccount = false;
      },
      error: (error) => {
        if (error.status === 404) {
          this.hasFreelancerAccount = false;
        } else {
          this.message.error('Failed to check account status.', { nzDuration: 3000 });
          console.error('Error checking account:', error);
        }
        this.isCheckingAccount = false;
      }
    });
  }
  
  getStatusLabel(status: number): string {
    const statuses = [
      'Published',
      'Accepting Applications',
      'Waiting For Work Start',
      'In Progress',
      'Pending For Review',
      'Completed',
      'Expired',
      'Cancelled'
    ];
    return statuses[status] || 'Unknown';
  }
  
  canApply(): boolean {
    if (!this.project || this.project.lifecycle.status !== ProjectStatus.AcceptingApplications) return false;
    return !this.hasApplication() &&
      !this.hasAcceptedApplication() &&
      this.currentUserId !== this.project.employerId &&
      this.hasFreelancerAccount;
  }
  
  hasApplication(): boolean {
    return !!this.project?.freelancerApplications.some(app =>
      app.freelancerId === this.currentUserId && app.status === ApplicationStatus.Pending);
  }
  
  hasAcceptedApplication(): boolean {
    return !!this.project?.freelancerApplications.some(app =>
      app.freelancerId === this.currentUserId && app.status === ApplicationStatus.Accepted);
  }
  
  applyForProject(): void {
    if (!this.project) return;
    this.isApplying = true;
    this.freelancerApplicationsService.createFreelancerApplication(this.project.id.toString()).subscribe({
      next: () => {
        this.isApplying = false;
        this.message.success('Application submitted successfully!', { nzDuration: 3000 });
        this.loadProject(this.project!.id.toString());
      },
      error: (error) => {
        this.isApplying = false;
        this.message.error('Failed to submit application. Please try again.', { nzDuration: 3000 });
        console.error('Error applying for project:', error);
      }
    });
  }
  
  cancelApplication(): void {
    if (!this.project) return;
    const application = this.project.freelancerApplications.find(app => app.freelancerId === this.currentUserId && app.status === ApplicationStatus.Pending);
    if (!application) return;
    
    this.isCancelling = true;
    this.freelancerApplicationsService.cancelFreelancerApplication(application.id).subscribe({
      next: () => {
        this.isCancelling = false;
        this.message.success('Application cancelled successfully!', { nzDuration: 3000 });
        this.loadProject(this.project!.id.toString());
      },
      error: (error) => {
        this.isCancelling = false;
        this.message.error('Failed to cancel application. Please try again.', { nzDuration: 3000 });
        console.error('Error cancelling application:', error);
      }
    });
  }
}