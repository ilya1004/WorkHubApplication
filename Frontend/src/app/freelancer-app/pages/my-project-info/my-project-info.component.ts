import {Component, OnInit} from '@angular/core';
import {NzCardModule} from "ng-zorro-antd/card";
import {EmployerUser} from '../../../core/interfaces/employer/employer-user.interface';
import {ActivatedRoute, Router, RouterModule} from "@angular/router";
import {ProjectsService} from "../../../core/services/projects/projects.service";
import {Project} from "../../../core/interfaces/project/project.interface";
import {CommonModule} from "@angular/common";
import {NzDescriptionsModule} from "ng-zorro-antd/descriptions";
import {NzGridModule} from "ng-zorro-antd/grid";
import {UsersService} from "../../../core/services/users/users.service";
import {PROJECT_STATUSES} from "../../../core/data/constants";
import {NzFlexDirective} from "ng-zorro-antd/flex";
import {NzButtonComponent} from "ng-zorro-antd/button";
import {NzMessageService} from "ng-zorro-antd/message";
import {ProjectChatComponent} from "./project-chat/project-chat.component";
import {ProjectStatus} from "../../../core/interfaces/project/lifecycle.interface";

@Component({
  selector: 'app-my-project-info',
  standalone: true,
  imports: [
    CommonModule,
    NzCardModule,
    NzDescriptionsModule,
    NzGridModule,
    RouterModule,
    NzFlexDirective,
    ProjectChatComponent,
    NzButtonComponent,
    ProjectChatComponent,
  ],
  styleUrl: './my-project-info.component.scss',
  templateUrl: './my-project-info.component.html'
})
export class MyProjectInfoComponent implements OnInit {
  project: Project | null = null;
  employer: EmployerUser | null = null;
  loading = false;
  submitting = false;

  constructor(
    private route: ActivatedRoute,
    private projectsService: ProjectsService,
    private usersService: UsersService,
    private message: NzMessageService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    const projectId = this.route.snapshot.paramMap.get('projectId');
    if (projectId) {
      this.loadProject(projectId);
    }
  }

  loadProject(projectId: string): void {
    this.loading = true;
    this.projectsService.getProjectById(projectId).subscribe({
      next: (project: Project) => {
        this.project = project;
        this.loadEmployer(project.employerId);
      },
      error: (error) => {
        console.error('Error loading project:', error);
        this.loading = false;
      }
    });
  }

  loadEmployer(employerId: string): void {
    this.usersService.getEmployerInfo(employerId).subscribe({
      next: (employer: EmployerUser) => {
        this.employer = employer;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading employer:', error);
        this.loading = false;
      }
    });
  }

  getStatusLabel(status: number): string {
    const statuses = [
      'Published',
      'AcceptingApplications',
      'WaitingForWorkStart',
      'InProgress',
      'PendingForReview',
      'Completed',
      'Expired',
      'Cancelled'
    ];
    return PROJECT_STATUSES.find(s =>
      s.value === statuses[status])?.label || 'Unknown';
  }
  
  canRequestAcceptance(): boolean {
    if (!this.project || this.project.lifecycle.acceptanceRequested)
      return false;
    const status = this.project.lifecycle.status;
    return status === ProjectStatus.InProgress || status === ProjectStatus.Expired;
  }
  
  requestAcceptance(): void {
    if (!this.project) return;
    
    this.submitting = true;
    this.projectsService.requestProjectAcceptance(this.project.id).subscribe({
      next: () => {
        this.submitting = false;
        if (this.project) {
          this.project.lifecycle.acceptanceRequested = true;
        }
        this.message.success('Acceptance request sent successfully!');
      },
      error: (error) => {
        this.submitting = false;
        console.error('Error requesting acceptance:', error);
        this.message.error('Failed to send acceptance request.');
      }
    });
  }
  
  goBack(): void {
    this.router.navigate(['/freelancer/my-projects']);
  }
  
  protected readonly ProjectStatus = ProjectStatus;
}
