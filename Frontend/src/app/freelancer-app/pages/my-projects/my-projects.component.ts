import { Component } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { NzFormItemComponent, NzFormLabelComponent } from "ng-zorro-antd/form";
import { NzOptionComponent, NzSelectComponent } from "ng-zorro-antd/select";
import { NzColDirective, NzRowDirective } from "ng-zorro-antd/grid";
import {
  NzTableCellDirective,
  NzTableComponent,
  NzTbodyComponent,
  NzTheadComponent,
  NzThMeasureDirective, NzTrDirective
} from "ng-zorro-antd/table";
import { MyProjectsService } from "../../services/my-projects.service";
import { Project } from "../../../core/interfaces/project/project.interface";
import { NzInputDirective, NzInputGroupComponent } from "ng-zorro-antd/input";
import { NzButtonComponent } from "ng-zorro-antd/button";
import { DatePipe, NgForOf } from "@angular/common";
import { PaginatedResult } from "../../../core/interfaces/common/paginated-result.interface";
import {PROJECT_STATUSES} from "../../../core/data/constants";
import {NzWaveDirective} from "ng-zorro-antd/core/wave";
import {Router} from "@angular/router";
import {NzFlexDirective} from "ng-zorro-antd/flex";

@Component({
  selector: 'app-my-projects',
  imports: [
    ReactiveFormsModule,
    NzFormItemComponent,
    NzFormLabelComponent,
    NzSelectComponent,
    NzOptionComponent,
    NzColDirective,
    NzRowDirective,
    NzTableComponent,
    NzInputDirective,
    NzButtonComponent,
    NgForOf,
    DatePipe,
    FormsModule,
    NzInputGroupComponent,
    NzWaveDirective,
    NzTableCellDirective,
    NzTbodyComponent,
    NzThMeasureDirective,
    NzTheadComponent,
    NzTrDirective,
    NzFlexDirective
  ],
  templateUrl: './my-projects.component.html',
  styleUrl: './my-projects.component.scss'
})
export class MyProjectsComponent {
  constructor(
    private myProjectsService: MyProjectsService,
    private router: Router
  ) { }

  // Available status options
  projectStatuses = PROJECT_STATUSES;

  // Filter form with string status
  filterForm = {
    projectStatus: null as string | null,
    employerId: null as string | null,
    pageNo: 1,
    pageSize: 10
  };

  // Table data
  projects: Project[] = [];
  totalCount = 0;
  loading = false;

  ngOnInit(): void {
    this.loadProjects();
  }

  loadProjects(): void {
    this.loading = true;
    this.myProjectsService.getMyFreelancerProjects(this.filterForm).subscribe({
      next: (result: PaginatedResult<Project>) => {
        this.projects = result.items;
        this.totalCount = result.totalCount;
        this.loading = false;
      },
      error: (error: any) => {
        console.error('Error loading freelancer projects:', error);
        this.loading = false;
      }
    });
  }

  onPageChange(page: number): void {
    this.filterForm.pageNo = page;
    this.loadProjects();
  }

  onPageSizeChange(size: number): void {
    this.filterForm.pageSize = size;
    this.filterForm.pageNo = 1;
    this.loadProjects();
  }

  applyFilters(): void {
    this.filterForm.pageNo = 1;
    this.loadProjects();
  }

  resetFilters(): void {
    this.filterForm = {
      projectStatus: null,
      employerId: null,
      pageNo: 1,
      pageSize: 10
    };
    this.loadProjects();
  }

  // Helper method to get status label
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

  navigateToProject(projectId: number): void {
    this.router.navigate(['/freelancer/my-projects', projectId]);
  }
}