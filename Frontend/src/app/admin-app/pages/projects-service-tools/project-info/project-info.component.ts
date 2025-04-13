import {Component, OnInit} from '@angular/core';
import {Project} from "../../../../core/interfaces/project/project.interface";
import {EmployerUser} from "../../../../core/interfaces/employer/employer-user.interface";
import {ActivatedRoute, Router} from "@angular/router";
import {ProjectsService} from '../../../../core/services/projects/projects.service';
import {UsersService} from "../../../../core/services/users/users.service";
import {FreelancerUser} from "../../../../core/interfaces/freelancer/freelancer-user.interface";
import {NzButtonComponent} from "ng-zorro-antd/button";
import {NzCardComponent} from "ng-zorro-antd/card";
import {NzDescriptionsComponent, NzDescriptionsItemComponent} from "ng-zorro-antd/descriptions";
import {DatePipe, NgIf} from "@angular/common";
import {NzTagComponent} from "ng-zorro-antd/tag";

@Component({
  selector: 'app-project-info',
  standalone: true,
  imports: [
    NzButtonComponent,
    NzCardComponent,
    NzDescriptionsComponent,
    NgIf,
    NzDescriptionsItemComponent,
    DatePipe,
    NzTagComponent
  ],
  templateUrl: './project-info.component.html',
  styleUrl: './project-info.component.scss'
})
export class ProjectInfoComponent implements OnInit {
  project: Project | null = null;
  employer: EmployerUser | null = null;
  freelancer: FreelancerUser | null = null;
  loading = false;
  
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private projectsService: ProjectsService,
    private usersService: UsersService
  ) {}
  
  ngOnInit(): void {
    const projectId = this.route.snapshot.paramMap.get('projectId');
    if (projectId) {
      this.loadProjectInfo(projectId);
    }
  }
  
  loadProjectInfo(projectId: string): void {
    this.loading = true;
    this.projectsService.getProjectById(projectId).subscribe({
      next: (project) => {
        this.project = project;
        this.loadUserInfo();
      },
      error: (error) => {
        console.error('Error loading project:', error);
        this.loading = false;
      }
    });
  }
  
  loadUserInfo(): void {
    if (!this.project) return;
    
    if (this.project.freelancerId) {
      this.usersService.getFreelancerInfo(this.project.freelancerId).subscribe({
        next: (freelancer) => {
          this.freelancer = freelancer || null;
          this.loading = false;
        },
        error: (error) => {
          console.error('Error loading user info:', error);
          this.loading = false;
        }
      });
    }
    
    this.usersService.getEmployerInfo(this.project.employerId).subscribe({
      next: (employer) => {
        this.employer = employer;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading user info:', error);
        this.loading = false;
      }
    });
  }
  
  getStatusLabel(status: number): string {
    const statuses = [
      'Published',
      'Accepting Applications',
      'Waiting for Work Start',
      'In Progress',
      'Pending for Review',
      'Completed',
      'Expired',
      'Cancelled'
    ];
    return statuses[status] || 'Unknown';
  }
  
  navigateBack(): void {
    this.router.navigate(['/admin/projects-service-tools']);
  }
}