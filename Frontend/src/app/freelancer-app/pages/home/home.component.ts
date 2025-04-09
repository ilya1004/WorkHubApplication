import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, FormsModule, ReactiveFormsModule} from "@angular/forms";
import {ProjectsService} from "../../services/projects.service";
import {Project} from "../../../core/interfaces/project/project.interface";
import {PaginatedResult} from "../../../core/interfaces/common/paginated-result.interface";
import {CommonModule} from "@angular/common";
import {NzTableModule} from "ng-zorro-antd/table";
import {NzFormModule} from "ng-zorro-antd/form";
import {NzInputModule} from "ng-zorro-antd/input";
import {NzSelectModule} from "ng-zorro-antd/select";
import {NzButtonModule} from "ng-zorro-antd/button";
import {Category} from "../../../core/interfaces/project/category.interface";
import {PROJECT_STATUSES} from "../../../core/data/constants";
import {Router} from "@angular/router";

@Component({
  selector: 'app-home',
  imports: [
    CommonModule,
    NzTableModule,
    NzInputModule,
    NzSelectModule,
    NzButtonModule,
    NzFormModule,
    FormsModule,
    ReactiveFormsModule
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  constructor(
    private homeService: ProjectsService,
    private router: Router
  ) { }

  // Filter form with string status
  filterForm = {
    title: null as string | null,
    budgetFrom: null as number | null,
    budgetTo: null as number | null,
    categoryId: null as string | null,
    employerId: null as string | null,
    projectStatus: null as string | null, // Changed to string
    pageNo: 1,
    pageSize: 10
  };

  // Table data
  projects: Project[] = [];
  totalCount = 0;
  loading = false;
  categories: Category[] = [];

  ngOnInit(): void {
    this.loadProjects();
    this.loadCategories();
  }

  loadProjects(): void {
    this.loading = true;
    // Convert filter form to match service expectations
    const filterForService = {
      ...this.filterForm,
      projectStatus: this.convertStringStatusToNumber(this.filterForm.projectStatus)
    };

    this.homeService.getProjectsByFilter(filterForService).subscribe({
      next: (result: PaginatedResult<Project>) => {
        this.projects = result.items;
        this.totalCount = result.totalCount;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading projects:', error);
        this.loading = false;
      }
    });
  }

  loadCategories(): void {
    this.homeService.getAllCategories().subscribe({
      next: (result: PaginatedResult<Project>) => {
        const allCategories = result.items.map(project => project.category);
        this.categories = Array.from(new Set(allCategories.map(c => c.id)))
          .map(id => allCategories.find(c => c.id === id)!);
      },
      error: (error) => {
        console.error('Error loading categories:', error);
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
      title: null,
      budgetFrom: null,
      budgetTo: null,
      categoryId: null,
      employerId: null,
      projectStatus: null,
      pageNo: 1,
      pageSize: 10
    };
    this.loadProjects();
  }

  // Convert string status to number for backend
  private convertStringStatusToNumber(status: string | null): number | null {
    if (!status) return null;
    switch (status) {
      case 'Published': return 0;
      case 'AcceptingApplications': return 1;
      case 'WaitingForWorkStart': return 2;
      case 'InProgress': return 3;
      case 'PendingForReview': return 4;
      case 'Completed': return 5;
      case 'Expired': return 6;
      case 'Cancelled': return 7;
      default: return null;
    }
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

  navigateToProject(projectId: number): void {
    this.router.navigate(['/freelancer/home/project', projectId]);
  }

  protected readonly PROJECT_STATUSES = PROJECT_STATUSES;
}