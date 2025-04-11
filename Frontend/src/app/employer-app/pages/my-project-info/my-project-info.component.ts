import { Component } from '@angular/core';
import { Project } from '../../../core/interfaces/project/project.interface';
import {FreelancerUser} from "../../../core/interfaces/freelancer/freelancer-user.interface";
import {ActivatedRoute, Router, RouterModule} from "@angular/router";
import {ProjectsService} from "../../../core/services/projects/projects.service";
import {UsersService} from "../../../freelancer-app/services/users.service";
import {NzMessageService} from "ng-zorro-antd/message";
import {NzButtonComponent} from "ng-zorro-antd/button";
import {NzFlexDirective} from "ng-zorro-antd/flex";
import {NzDescriptionsComponent, NzDescriptionsItemComponent, NzDescriptionsModule} from "ng-zorro-antd/descriptions";
import {CommonModule, NgIf} from "@angular/common";
import {NzCardModule} from "ng-zorro-antd/card";
import {NzGridModule} from "ng-zorro-antd/grid";
import {ProjectStatus} from "../../../core/interfaces/project/project-status.interface";
import {ProjectChatComponent} from "./project-chat/project-chat.component";

@Component({
  selector: 'app-my-project-info',
  imports: [
    CommonModule,
    NzCardModule,
    NzDescriptionsModule,
    NzGridModule,
    RouterModule,
    NzFlexDirective,
    NzButtonComponent,
    NzFlexDirective,
    NzDescriptionsItemComponent,
    NzDescriptionsComponent,
    NgIf,
    ProjectChatComponent,
  ],
  templateUrl: './my-project-info.component.html',
  styleUrl: './my-project-info.component.scss'
})
export class MyProjectInfoComponent {
  project: Project | null = null;
  freelancer: FreelancerUser | null = null;
  loading = false;
  submitting = false;
  
  protected readonly ProjectStatus = ProjectStatus;
  
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
        if (project.freelancerId) {
          this.loadFreelancer(project.freelancerId);
        } else {
          this.loading = false;
        }
      },
      error: (error) => {
        console.error('Error loading project:', error);
        this.loading = false;
        this.message.error('Failed to load project.');
      }
    });
  }
  
  loadFreelancer(freelancerId: string): void {
    this.usersService.getFreelancerInfo(freelancerId).subscribe({
      next: (freelancer: FreelancerUser) => {
        this.freelancer = freelancer;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading freelancer:', error);
        this.loading = false;
        this.message.error('Failed to load freelancer information.');
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
    return statuses[status] || 'Unknown';
  }
  
  canCompleteProject(): boolean {
    if (!this.project || this.project.lifecycle.status === ProjectStatus.Completed) {
      return false;
    }
    const status = this.project.lifecycle.status;
    return (
      status === ProjectStatus.PendingForReview &&
      this.project.lifecycle.acceptanceRequested
    );
  }
  
  setProjectAcceptanceStatus(status: boolean): void {
    if (!this.project) return;
    
    this.submitting = true;
    this.projectsService.setProjectAcceptanceStatus(this.project.id, status).subscribe({
      next: () => {
        this.submitting = false;
        if (this.project) {
          this.project.lifecycle.status = ProjectStatus.Completed;
        }
        this.message.success('Project marked as completed successfully!');
      },
      error: (error) => {
        this.submitting = false;
        console.error('Error completing project:', error);
        this.message.error('Failed to complete project.');
      }
    });
  }
  
  goBack(): void {
    this.router.navigate(['/employer/my-projects']);
  }
}
