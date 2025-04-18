import {Component, OnInit} from '@angular/core';
import {Project} from "../../../core/interfaces/project/project.interface";
import {Category} from "../../../core/interfaces/project/category.interface";
import {ProjectsService} from "../../../core/services/projects/projects.service";
import {CategoriesService} from '../../../core/services/categories/categories.service';
import {Router} from "@angular/router";
import {PaginatedResult} from "../../../core/interfaces/common/paginated-result.interface";
import {NzTableComponent, NzTableModule} from "ng-zorro-antd/table";
import {CommonModule, DatePipe, NgForOf} from "@angular/common";
import {NzButtonComponent, NzButtonModule} from "ng-zorro-antd/button";
import {NzFlexDirective} from "ng-zorro-antd/flex";
import {NzFormItemComponent, NzFormLabelComponent} from "ng-zorro-antd/form";
import {NzInputDirective, NzInputModule} from "ng-zorro-antd/input";
import {FormsModule} from "@angular/forms";
import {NzOptionComponent, NzSelectComponent, NzSelectModule} from "ng-zorro-antd/select";
import {CategoriesComponent} from "./categories/categories.component";
import {FreelancerApplicationsComponent} from "./freelancer-applications/freelancer-applications.component";

@Component({
  selector: 'app-projects-service-tools',
  standalone: true,
  imports: [
    NzTableComponent,
    NzTableModule,
    NzButtonComponent,
    NgForOf,
    DatePipe,
    NzFlexDirective,
    NzInputDirective,
    FormsModule,
    NzSelectComponent,
    NzOptionComponent,
    CategoriesComponent,
    CommonModule,
    NzInputModule,
    NzSelectModule,
    NzButtonModule,
    NzFormItemComponent,
    NzFormLabelComponent,
    FreelancerApplicationsComponent,
  ],
  templateUrl: './projects-service-tools.component.html',
  styleUrl: './projects-service-tools.component.scss'
})
export class ProjectsServiceToolsComponent implements OnInit {
  filterForm = {
    title: null as string | null,
    budgetFrom: null as number | null,
    budgetTo: null as number | null,
    categoryId: null as string | null,
    employerId: null as string | null,
    projectStatus: null as string | null,
    pageNo: 1,
    pageSize: 5
  };
  
  projects: Project[] = [];
  totalCount = 0;
  loading = false;
  categories: Category[] = [];
  
  constructor(
    private projectsService: ProjectsService,
    private categoriesService: CategoriesService,
    private router: Router
  ) {}
  
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
      pageSize: 5
    };
    this.loadProjects();
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
  
  navigateToProject(projectId: string): void {
    this.router.navigate(['/admin/projects-service-tools/project', projectId]);
  }
  
  onCategoryChange(): void {
    this.loadCategories();
  }
  
  protected readonly projectStatuses = [
    { value: 'Published', label: 'Published' },
    { value: 'Accepting Applications', label: 'Accepting Applications' },
    { value: 'Waiting for Work Start', label: 'Waiting for Work Start' },
    { value: 'In Progress', label: 'In Progress' },
    { value: 'Pending for Review', label: 'Pending for Review' },
    { value: 'Completed', label: 'Completed' },
    { value: 'Expired', label: 'Expired' },
    { value: 'Cancelled', label: 'Cancelled' }
  ];
}