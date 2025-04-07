import {Component, OnInit} from '@angular/core';
import {Project} from "../../interfaces/my-projects/project.interface";
import {ProjectsService} from "../../services/projects.service";
import {CommonModule} from "@angular/common";
import {NzCardModule} from "ng-zorro-antd/card";
import {NzButtonModule} from "ng-zorro-antd/button";
import {NzDescriptionsModule} from "ng-zorro-antd/descriptions";
import {ActivatedRoute, RouterLink} from "@angular/router";
import {EmployerUserDto} from "../../../core/interfaces/employer-user-dto.interface";
import {FreelancerUserDto} from "../../../core/interfaces/freelancer-user-dto.interface";
import {UsersService} from "../../services/users.service";

@Component({
  selector: 'app-project-info',
  standalone: true,
  imports: [
    CommonModule,
    NzCardModule,
    NzDescriptionsModule,
    NzButtonModule,
    RouterLink
  ],
  templateUrl: './project-info.component.html',
  styleUrl: './project-info.component.scss'
})
export class ProjectInfoComponent implements OnInit {
  project: Project | null = null;
  employer: EmployerUserDto | null = null;
  freelancer: FreelancerUserDto | null = null;
  loading = false;

  constructor(
    private route: ActivatedRoute,
    private projectsService: ProjectsService,
    private usersService: UsersService,
  ) { }

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
        if (project.freelancerId) {
          this.loadFreelancer(project.freelancerId);
        } else {
          this.freelancer = null;
          this.loading = false;
        }
      },
      error: (error) => {
        console.error('Error loading project:', error);
        this.loading = false;
      }
    });
  }

  loadEmployer(employerId: string): void {
    this.usersService.getEmployerInfo(employerId).subscribe({
      next: (employer: EmployerUserDto) => {
        this.employer = employer;
        this.checkLoadingComplete();
      },
      error: (error) => {
        console.error('Error loading employer:', error);
        this.checkLoadingComplete();
      }
    });
  }

  loadFreelancer(freelancerId: string): void {
    this.usersService.getFreelancerInfo(freelancerId).subscribe({
      next: (freelancer: FreelancerUserDto) => {
        this.freelancer = freelancer;
        this.checkLoadingComplete();
      },
      error: (error) => {
        console.error('Error loading freelancer:', error);
        this.checkLoadingComplete();
      }
    });
  }

  private checkLoadingComplete(): void {
    if (this.project && this.employer && (!this.project.freelancerId || this.freelancer)) {
      this.loading = false;
    }
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
}
