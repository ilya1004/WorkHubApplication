import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  ValidationErrors,
  Validators
} from "@angular/forms";
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import {NzButtonModule} from "ng-zorro-antd/button";
import {NzSelectModule } from 'ng-zorro-antd/select';
import { NzTableModule } from 'ng-zorro-antd/table';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzFlexModule } from 'ng-zorro-antd/flex';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzCardModule } from 'ng-zorro-antd/card';
import {DatePipe, NgForOf, NgIf} from '@angular/common';
import {ProjectToolsService} from "../../services/project-tools.service";
import { NzMessageService } from 'ng-zorro-antd/message';
import {Project} from "../../../core/interfaces/project/project.interface";
import { Category } from '../../../core/interfaces/project/category.interface';
import { FreelancerApplication } from '../../../core/interfaces/project/freelancer-application.interface';
import { FreelancerUser } from '../../../core/interfaces/freelancer/freelancer-user.interface';
import {CreateProjectForm} from "../../interfaces/project-tools/create-project.interface";
import {UpdateProjectForm} from "../../interfaces/project-tools/update-project.interface";
import {PROJECT_STATUSES} from "../../../core/data/constants";
import {NzTagComponent} from "ng-zorro-antd/tag";
import {NzModalService} from "ng-zorro-antd/modal";
import {ProjectStatus} from "../../../core/interfaces/project/project-status.interface";
import {NzDescriptionsComponent, NzDescriptionsItemComponent} from "ng-zorro-antd/descriptions";
import {PaginatedResult} from "../../../core/interfaces/common/paginated-result.interface";
import {NzInputNumberComponent} from "ng-zorro-antd/input-number";
import {CategoriesService} from "../../../core/services/categories/categories.service";

@Component({
  selector: 'app-project-tools',
  standalone: true,
  imports: [
    FormsModule,
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
    NzInputNumberComponent
  ],
  providers: [NzModalService],
  templateUrl: './project-tools.component.html',
  styleUrls: ['./project-tools.component.scss']
})
export class ProjectToolsComponent implements OnInit {
  constructor(
    private formBuilder: FormBuilder,
    private projectService: ProjectToolsService,
    private categoriesService: CategoriesService,
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
  
  createProjectForm!: FormGroup<CreateProjectForm>;
  updateProjectForm!: FormGroup<UpdateProjectForm>;
  
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
    this.initForms();
  }
  
  initForms(): void {
    this.createProjectForm = this.formBuilder.group<CreateProjectForm>({
      title: new FormControl<string>('', {
        nonNullable: true,
        validators: [
          Validators.required,
          Validators.maxLength(200)
        ]
      }),
      description: new FormControl<string | null>('', {
        validators: [
          Validators.maxLength(1000)
        ]
      }),
      budget: new FormControl<number>(0, {
        nonNullable: true,
        validators: [
          Validators.required,
          Validators.min(0.01),
          Validators.pattern(/^\d{1,10}(\.\d{1,2})?$/)
        ]
      }),
      categoryId: new FormControl<string | null>(null, {
        validators: [ ]
      }),
      applicationsStartDate: new FormControl<Date>(null as any, {
        nonNullable: true,
        validators: [
          Validators.required
        ]
      }),
      applicationsDeadline: new FormControl<Date>(null as any, {
        nonNullable: true,
        validators: [
          Validators.required
        ]
      }),
      workStartDate: new FormControl<Date>(null as any, {
        nonNullable: true,
        validators: [
          Validators.required
        ]
      }),
      workDeadline: new FormControl<Date>(null as any, {
        nonNullable: true,
        validators: [
          Validators.required
        ]
      })
    }, { validators: this.dateSequenceValidator });
    
    this.updateProjectForm = this.formBuilder.group<UpdateProjectForm>({
      title: new FormControl<string>('', {
        nonNullable: true,
        validators: [
          Validators.required,
          Validators.maxLength(200)
        ]
      }),
      description: new FormControl<string | null>('', {
        validators: [
          Validators.maxLength(1000)
        ]
      }),
      budget: new FormControl<number>(0, {
        nonNullable: true,
        validators: [
          Validators.required,
          Validators.min(0.01),
          Validators.pattern(/^\d{1,16}(\.\d{1,2})?$/)
        ]
      }),
      categoryId: new FormControl<string | null>(null, {
        validators: []
      }),
      applicationsStartDate: new FormControl<Date>(null as any, {
        nonNullable: true,
        validators: [
          Validators.required
        ]
      }),
      applicationsDeadline: new FormControl<Date>(null as any, {
        nonNullable: true,
        validators: [
          Validators.required
        ]
      }),
      workStartDate: new FormControl<Date>(null as any, {
        nonNullable: true,
        validators: [
          Validators.required
        ]
      }),
      workDeadline: new FormControl<Date>(null as any, {
        nonNullable: true,
        validators: [
          Validators.required
        ]
      })
    }, { validators: this.dateSequenceValidator });
  }
  
  // futureDateValidator() {
  //   return (control: FormControl<Date>): { [key: string]: any } | null => {
  //     const value = control.value;
  //     if (value && value <= new Date()) {
  //       return { pastDate: true };
  //     }
  //     return null;
  //   };
  // }
  
  dateSequenceValidator(group: AbstractControl): ValidationErrors | null {
    const start = group.get('applicationsStartDate')?.value;
    const deadline = group.get('applicationsDeadline')?.value;
    const workStart = group.get('workStartDate')?.value;
    const workEnd = group.get('workDeadline')?.value;
    
    if (start && deadline && start >= deadline) {
      return { applicationsDateError: true };
    }
    if (deadline && workStart && deadline >= workStart) {
      return { workStartDateError: true };
    }
    if (workStart && workEnd && workStart >= workEnd) {
      return { workEndDateError: true };
    }
    return null;
  }
  
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
        this.hasAcceptedApplication = this.applications.some(app => app.status === 1); // Accepted
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
    this.createProjectForm.reset();
  }
  
  onEditProject(project: Project): void {
    if (project.lifecycle.status !== 0) {
      this.message.warning('Only projects in "Published" status can be edited.');
      return;
    }
    this.selectedProject = project;
    this.isEditing = true;
    this.isCreating = false;
    this.isViewingApplications = false;
    this.updateProjectForm.patchValue({
      title: project.title,
      description: project.description,
      budget: project.budget,
      categoryId: project.categoryId,
      applicationsStartDate: new Date(project.lifecycle.applicationsStartDate),
      applicationsDeadline: new Date(project.lifecycle.applicationsDeadline),
      workStartDate: new Date(project.lifecycle.workStartDate),
      workDeadline: new Date(project.lifecycle.workDeadline)
    });
  }
  
  onViewApplications(project: Project): void {
    if (this.selectedProject != null && this.selectedProject.id === project.id) {
      this.isViewingApplications = !this.isViewingApplications;
    }
    else {
      this.isViewingApplications = true;
    }
    this.selectedProject = project;
    this.isCreating = false;
    this.isEditing = false;
    this.loadApplications(project.id);
  }
  
  submitCreateProject(): void {
    if (this.createProjectForm.valid) {
      const value = this.createProjectForm.getRawValue();
      this.projectService.createProject({
        project: {
          title: value.title,
          description: value.description,
          budget: value.budget,
          categoryId: value.categoryId
        },
        lifecycle: {
          applicationsStartDate: value.applicationsStartDate,
          applicationsDeadline: value.applicationsDeadline,
          workStartDate: value.workStartDate,
          workDeadline: value.workDeadline
        }
      }).subscribe({
        next: () => {
          this.message.success('Project created successfully!');
          this.loadProjects();
          this.isCreating = false;
        },
        error: () => this.message.error('Failed to create project.')
      });
    }
  }
  
  submitUpdateProject(): void {
    if (this.updateProjectForm.valid && this.selectedProject) {
      const value = this.createProjectForm.getRawValue();
      this.projectService.updateProject(this.selectedProject.id, {
        project: {
          title: value.title,
          description: value.description,
          budget: value.budget,
          categoryId: value.categoryId
        },
        lifecycle: {
          applicationsStartDate: value.applicationsStartDate,
          applicationsDeadline: value.applicationsDeadline,
          workStartDate: value.workStartDate,
          workDeadline: value.workDeadline
        }
      }).subscribe({
        next: () => {
          this.message.success('Project updated successfully!');
          this.loadProjects();
          this.isEditing = false;
        },
        error: () => this.message.error('Failed to update project.')
      });
    }
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
  
  showApplicationDetails(application: FreelancerApplication): void {
    if (this.selectedApplication != null && this.selectedApplication.id === application.id) {
      this.isViewingApplicationDetails = !this.isViewingApplicationDetails;
    }
    else {
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
      },
      error: () => this.message.error('Failed to accept application.')
    });
  }
  
  rejectApplication(applicationId: string, projectId: string): void {
    this.projectService.rejectApplication(projectId, applicationId).subscribe({
      next: () => {
        this.message.success('Application rejected!');
        this.loadApplications(projectId);
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
    return PROJECT_STATUSES.find(s =>
      s.value === statuses[status])?.label || 'Unknown';
  }
  
  getApplicationStatusLabel(status: number): string {
    return ['Pending', 'Accepted', 'Rejected'][status] || 'Unknown';
  }
  
  protected readonly PROJECT_STATUSES = PROJECT_STATUSES;
  protected readonly ProjectStatus = ProjectStatus;
}