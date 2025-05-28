import {Component, OnInit} from '@angular/core';
import {ReactiveFormsModule} from "@angular/forms";
import {NzFormModule} from 'ng-zorro-antd/form';
import {NzInputModule} from 'ng-zorro-antd/input';
import {NzButtonModule} from "ng-zorro-antd/button";
import {NzSelectModule} from 'ng-zorro-antd/select';
import {NzTableModule} from 'ng-zorro-antd/table';
import {NzDatePickerModule} from 'ng-zorro-antd/date-picker';
import {NzFlexModule} from 'ng-zorro-antd/flex';
import {NzDividerModule} from 'ng-zorro-antd/divider';
import {NzCardModule} from 'ng-zorro-antd/card';
import {CommonModule, DatePipe, NgForOf, NgIf} from '@angular/common';
import {ProjectToolsService} from "../../services/project-tools.service";
import {NzMessageService} from 'ng-zorro-antd/message';
import {Project} from "../../../core/interfaces/project/project.interface";
import {Category} from '../../../core/interfaces/project/category.interface';
import {ApplicationStatus, FreelancerApplication} from '../../../core/interfaces/project/freelancer-application.interface';
import {FreelancerUser} from '../../../core/interfaces/freelancer/freelancer-user.interface';
import {PROJECT_STATUSES} from "../../../core/data/constants";
import {NzTagComponent} from "ng-zorro-antd/tag";
import {NzModalService} from "ng-zorro-antd/modal";
import {NzDescriptionsComponent, NzDescriptionsItemComponent} from "ng-zorro-antd/descriptions";
import {PaginatedResult} from "../../../core/interfaces/common/paginated-result.interface";
import {CategoriesService} from "../../../core/services/categories/categories.service";
import {CreateProjectComponent} from "./create-project/create-project.component";
import {EditProjectComponent} from "./edit-project/edit-project.component";
import {ProjectCreateData} from "../../interfaces/project-tools/create-project.interface";
import {ProjectUpdateData} from "../../interfaces/project-tools/update-project.interface";
import {FinanceService} from "../../services/finance.service";
import {ProjectStatus} from '../../../core/interfaces/project/lifecycle.interface';

@Component({
  selector: 'app-project-tools',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    NzFormModule,
    NzInputModule,
    NzButtonModule,
    NzTableModule,
    NzSelectModule,
    NzDatePickerModule,
    NzFlexModule,
    NzDividerModule,
    NzCardModule,
    NgForOf,
    NgIf,
    DatePipe,
    NzTagComponent,
    NzDescriptionsComponent,
    NzDescriptionsItemComponent,
    CreateProjectComponent,
    EditProjectComponent
  ],
  providers: [NzModalService],
  templateUrl: './project-tools.component.html',
  styleUrls: ['./project-tools.component.scss']
})
export class ProjectToolsComponent implements OnInit {
  constructor(
    private projectService: ProjectToolsService,
    private categoriesService: CategoriesService,
    private financeService: FinanceService,
    private message: NzMessageService,
    private modal: NzModalService
  ) {}
  
  projects: Project[] = [];
  categories: Category[] = [];
  
  selectedProject: Project | null = null;
  applications: FreelancerApplication[] = [];
  selectedApplication: FreelancerApplication | null = null;
  freelancerDetails: FreelancerUser | null = null;
  hasAcceptedApplication = false;
  
  isCreating = false;
  isEditing = false;
  isViewingApplications = false;
  isViewingApplicationDetails = false;
  
  applicationPageNo = 1;
  applicationPageSize = 5;
  applicationTotalCount = 0;
  
  projectPageNo = 1;
  projectPageSize = 10;
  projectTotalCount = 0;
  
  ngOnInit(): void {
    this.loadProjects();
    this.loadCategories();
  }
  
  disablePastDates = (current: Date): boolean => {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    today.setDate(today.getDate() + 1);
    return current < today;
  };
  
  loadProjects(): void {
    this.projectService.getEmployerProjects(this.projectPageNo, this.projectPageSize).subscribe({
      next: (result: PaginatedResult<Project>) => {
        this.projects = result.items;
        this.projectTotalCount = result.totalCount;
      },
      error: () => this.message.error('Failed to load projects.')
    });
  }
  
  onProjectPageChange(page: number): void {
    this.projectPageNo = page;
    this.loadProjects();
  }
  
  onProjectPageSizeChange(size: number): void {
    this.projectPageSize = size;
    this.projectPageNo = 1;
    this.loadProjects();
  }
  
  loadCategories(): void {
    this.categoriesService.getCategories(1, 100).subscribe({
      next: (categories) => {
        this.categories = categories.items;
      },
      error: () => this.message.error('Failed to load categories.')
    });
  }
  
  loadApplications(projectId: string): void {
    this.projectService.getApplicationsByProject(projectId, this.applicationPageNo, this.applicationPageSize).subscribe({
      next: (result) => {
        this.applications = result.items;
        this.applicationTotalCount = result.totalCount;
        this.hasAcceptedApplication = this.applications.some(app => app.status === ApplicationStatus.Accepted);
      },
      error: () => this.message.error('Failed to load applications.')
    });
  }
  
  loadFreelancerDetails(freelancerId: string): void {
    this.projectService.getFreelancerInfo(freelancerId).subscribe({
      next: (data) => this.freelancerDetails = data,
      error: () => this.message.error('Failed to load freelancer details.')
    });
  }
  
  onCreateProject(): void {
    this.isCreating = !this.isCreating;
    this.isEditing = false;
    this.isViewingApplications = false;
  }
  
  onProjectCreated(projectCreateData: ProjectCreateData): void {
    
    const requestData = {
      project: {
        title: projectCreateData.project.title,
        description: projectCreateData.project.description,
        budget: projectCreateData.project.budget,
        categoryId: projectCreateData.project.categoryId
      },
      lifecycle: {
        applicationsStartDate: projectCreateData.lifecycle.applicationsStartDate.toISOString(),
        applicationsDeadline: projectCreateData.lifecycle.applicationsDeadline.toISOString(),
        workStartDate: projectCreateData.lifecycle.workStartDate.toISOString(),
        workDeadline: projectCreateData.lifecycle.workDeadline.toISOString()
      }
    };
    
    this.projectService.createProject(requestData).subscribe({
      next: (response: { projectId: string }) => {
        this.message.success('Project created successfully!');
        this.loadProjects();
        
        this.financeService.createPaymentIntent(response.projectId, projectCreateData.paymentMethodId).subscribe({
          next: () => {
            this.message.success('Payment intent created successfully!');
            this.isCreating = false;
          },
          error: (err) => {
            this.message.error('Failed to create payment intent: ' + (err.message || err));
          }
        });
      },
      error: (err) => {
        this.message.error('Failed to create project: ' + (err.message || err));
      }
    });
  }
  
  onEditProject(project: Project): void {
    if (project.lifecycle.status !== 0) {
      this.message.warning('Only projects in "Published" status can be edited.');
      return;
    }
    if (this.selectedProject?.id === project.id) {
      this.isEditing = !this.isEditing;
    } else {
      this.isEditing = true;
    }
    this.selectedProject = project;
    this.isCreating = false;
    this.isViewingApplications = false;
  }
  
  onProjectUpdated(data: ProjectUpdateData): void {
    const requestData = {
      project: {
        title: data.project.title,
        description: data.project.description,
        budget: data.project.budget,
        categoryId: data.project.categoryId
      },
      lifecycle: {
        applicationsStartDate: data.lifecycle.applicationsStartDate.toISOString(),
        applicationsDeadline: data.lifecycle.applicationsDeadline.toISOString(),
        workStartDate: data.lifecycle.workStartDate.toISOString(),
        workDeadline: data.lifecycle.workDeadline.toISOString()
      }
    };
    if (this.selectedProject) {
      this.projectService.updateProject(this.selectedProject.id, requestData).subscribe({
        next: () => {
          this.message.success('Project updated successfully!');
          this.loadProjects();
          this.isEditing = false;
        },
        error: () => this.message.error('Failed to update project.')
      });
    }
  }
  
  onViewApplications(project: Project): void {
    if (this.selectedProject?.id === project.id) {
      this.isViewingApplications = !this.isViewingApplications;
    } else {
      this.isViewingApplications = true;
    }
    this.selectedProject = project;
    this.isCreating = false;
    this.isEditing = false;
    this.loadApplications(project.id);
  }
  
  cancelProject(projectId: string): void {
    this.modal.confirm({
      nzTitle: 'Are you sure you want to cancel this project?',
      nzOnOk: () => {
        this.projectService.cancelProject(projectId).subscribe({
          next: () => {
            this.message.success('Project cancelled successfully!');
            this.loadProjects();
          },
          error: () => this.message.error('Failed to cancel project.')
        });
      }
    });
  }
  
  deleteProject(projectId: string): void {
    this.modal.confirm({
      nzTitle: 'Are you sure you want to delete this project?',
      nzContent: 'This action cannot be undone.',
      nzOkText: 'Delete',
      nzOkDanger: true,
      nzOnOk: () => {
        this.projectService.deleteProject(projectId).subscribe({
          next: () => {
            this.message.success('Project deleted successfully!');
            this.loadProjects();
          },
          error: (err) => {
            this.message.error('Failed to delete project: ' + (err.message || err));
          }
        });
      }
    });
  }
  
  showApplicationDetails(application: FreelancerApplication): void {
    if (this.selectedApplication?.id === application.id) {
      this.isViewingApplicationDetails = !this.isViewingApplicationDetails;
    } else {
      this.isViewingApplicationDetails = true;
    }
    if (this.selectedApplication === null || this.selectedApplication.id !== application.id) {
      this.loadFreelancerDetails(application.freelancerId);
    }
    this.selectedApplication = application;
  }
  
  acceptApplication(applicationId: string, projectId: string): void {
    this.projectService.acceptApplication(projectId, applicationId).subscribe({
      next: () => {
        this.message.success('Application accepted!');
        this.loadApplications(projectId);
        this.selectedApplication = null;
      },
      error: () => this.message.error('Failed to accept application.')
    });
  }
  
  rejectApplication(applicationId: string, projectId: string): void {
    this.projectService.rejectApplication(projectId, applicationId).subscribe({
      next: () => {
        this.message.success('Application rejected!');
        this.loadApplications(projectId);
        this.selectedApplication = null;
      },
      error: () => this.message.error('Failed to reject application.')
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
    return PROJECT_STATUSES.find(s => s.value === statuses[status])?.label || 'Unknown';
  }
  
  getApplicationStatusLabel(status: number): string {
    return ['Pending', 'Accepted', 'Rejected'][status] || 'Unknown';
  }
  protected readonly ProjectStatus = ProjectStatus;
}