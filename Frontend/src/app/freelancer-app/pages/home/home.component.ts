import {Component, OnInit} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {ProjectsService} from "../../../core/services/projects/projects.service";
import {Project} from "../../../core/interfaces/project/project.interface";
import {PaginatedResult} from "../../../core/interfaces/common/paginated-result.interface";
import {CommonModule, DatePipe, NgForOf} from "@angular/common";
import {NzTableComponent, NzTableModule} from "ng-zorro-antd/table";
import {NzFormItemComponent, NzFormLabelComponent, NzFormModule} from "ng-zorro-antd/form";
import {NzInputDirective, NzInputModule} from "ng-zorro-antd/input";
import {NzOptionComponent, NzSelectComponent, NzSelectModule} from "ng-zorro-antd/select";
import {NzButtonComponent, NzButtonModule} from "ng-zorro-antd/button";
import {Category} from "../../../core/interfaces/project/category.interface";
import {PROJECT_STATUSES} from "../../../core/data/constants";
import {Router} from "@angular/router";
import {NzFlexDirective} from "ng-zorro-antd/flex";
import {CategoriesService} from "../../../core/services/categories/categories.service";
import {NzColDirective, NzRowDirective} from "ng-zorro-antd/grid";

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    NzTableModule,
    NzInputModule,
    NzSelectModule,
    NzButtonModule,
    NzFormModule,
    FormsModule,
    ReactiveFormsModule,
    NzFlexDirective,
    NzTableComponent,
    NzTableModule,
    NzButtonComponent,
    NgForOf,
    DatePipe,
    NzFlexDirective,
    NzFormItemComponent,
    NzFormLabelComponent,
    NzInputDirective,
    FormsModule,
    NzSelectComponent,
    NzOptionComponent,
    NzColDirective,
    NzRowDirective,
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  constructor(
    private projectsService: ProjectsService,
    private categoriesService: CategoriesService,
    private router: Router
  ) { }
  
  filterForm = {
    title: null as string | null,
    budgetFrom: null as number | null,
    budgetTo: null as number | null,
    categoryId: null as string | null,
    employerId: null as string | null,
    projectStatus: null as string | null,
    pageNo: 1,
    pageSize: 10
  };
  
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
    this.projectsService.getProjectsByFilter(this.filterForm).subscribe({
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
    this.categoriesService.getCategories(1, 100).subscribe({
      next: (result: PaginatedResult<Category>) => {
        this.categories = result.items;
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

  navigateToProject(projectId: string): void {
    this.router.navigate(['/freelancer/home/project', projectId]);
  }

  protected readonly projectStatuses = PROJECT_STATUSES;
}