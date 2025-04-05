import {Component, OnInit} from '@angular/core';
import {NzCardModule} from "ng-zorro-antd/card";
import { EmployerUserDto } from '../../../core/interfaces/employer-user-dto.interface';
import {ActivatedRoute, RouterModule} from "@angular/router";
import {ProjectsService} from "../../services/projects.service";
import {Project} from "../../interfaces/my-projects/project.interface";
import {CommonModule} from "@angular/common";
import {NzDescriptionsModule} from "ng-zorro-antd/descriptions";
import {NzGridModule} from "ng-zorro-antd/grid";
import {UsersService} from "../../services/users.service";
import {PROJECT_STATUSES} from "../../../core/constants";
import {NzFlexDirective} from "ng-zorro-antd/flex";

@Component({
  imports: [
    CommonModule,
    NzCardModule,
    NzDescriptionsModule,
    NzGridModule,
    RouterModule,
    NzFlexDirective
  ],
  selector: 'app-my-project-info',
  standalone: true,
  styleUrl: './my-project-info.component.scss',
  templateUrl: './my-project-info.component.html'
})
export class MyProjectInfoComponent implements OnInit {
  project: Project | null = null;
  employer: EmployerUserDto | null = null;
  loading = false;

  constructor(
    private route: ActivatedRoute,
    private homeService: ProjectsService,
    private usersService: UsersService,
  ) {}

  ngOnInit(): void {
    const projectId = this.route.snapshot.paramMap.get('projectId');
    if (projectId) {
      this.loadProject(projectId);
    }
  }

  loadProject(projectId: string): void {
    this.loading = true;
    this.homeService.getProjectById(projectId).subscribe({
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
      next: (employer: EmployerUserDto) => {
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
}
