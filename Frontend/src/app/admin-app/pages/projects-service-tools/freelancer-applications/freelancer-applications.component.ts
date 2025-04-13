import {Component, OnInit} from '@angular/core';
import {ApplicationStatus, FreelancerApplication} from "../../../../core/interfaces/project/freelancer-application.interface";
import {FreelancerApplicationsService} from "../../../../core/services/freelancer-applications/freelancer-applications.service";
import {PaginatedResult} from "../../../../core/interfaces/common/paginated-result.interface";
import {NzTableComponent, NzTableModule} from "ng-zorro-antd/table";
import {DatePipe, NgForOf} from "@angular/common";
import {NzFormItemComponent, NzFormLabelComponent} from "ng-zorro-antd/form";
import {NzFlexDirective} from "ng-zorro-antd/flex";
import {NzDatePickerComponent} from "ng-zorro-antd/date-picker";
import {FormsModule} from "@angular/forms";
import {NzOptionComponent, NzSelectComponent} from "ng-zorro-antd/select";
import {NzButtonComponent} from "ng-zorro-antd/button";

@Component({
  selector: 'app-freelancer-applications',
  standalone: true,
  imports: [
    NzTableComponent,
    NzTableModule,
    DatePipe,
    NzFormItemComponent,
    NzFormLabelComponent,
    NzFlexDirective,
    NzDatePickerComponent,
    FormsModule,
    NzSelectComponent,
    NzOptionComponent,
    NgForOf,
    NzButtonComponent
  ],
  templateUrl: './freelancer-applications.component.html',
  styleUrl: './freelancer-applications.component.scss'
})
export class FreelancerApplicationsComponent implements OnInit {
  filterForm = {
    startDate: null as Date | null,
    endDate: null as Date | null,
    applicationStatus: null as number | null,
    pageNo: 1,
    pageSize: 5
  };
  
  applications: FreelancerApplication[] = [];
  totalCount = 0;
  loading = false;
  
  applicationStatuses = [
    { value: ApplicationStatus.Pending, label: 'Pending' },
    { value: ApplicationStatus.Accepted, label: 'Accepted' },
    { value: ApplicationStatus.Rejected, label: 'Rejected' }
  ];
  
  constructor(private freelancerApplicationsService: FreelancerApplicationsService) {}
  
  ngOnInit(): void {
    this.loadApplications();
  }
  
  loadApplications(): void {
    this.loading = true;
    const filter = {
      startDate: this.filterForm.startDate ? this.filterForm.startDate.toISOString() : null,
      endDate: this.filterForm.endDate ? this.filterForm.endDate.toISOString() : null,
      applicationStatus: this.filterForm.applicationStatus,
      pageNo: this.filterForm.pageNo,
      pageSize: this.filterForm.pageSize
    };
    
    this.freelancerApplicationsService.getFreelancerApplicationsByFilter(filter).subscribe({
      next: (result: PaginatedResult<FreelancerApplication>) => {
        this.applications = result.items;
        this.totalCount = result.totalCount;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading applications:', error);
        this.loading = false;
      }
    });
  }
  
  onPageChange(page: number): void {
    this.filterForm.pageNo = page;
    this.loadApplications();
  }
  
  onPageSizeChange(size: number): void {
    this.filterForm.pageSize = size;
    this.filterForm.pageNo = 1;
    this.loadApplications();
  }
  
  applyFilters(): void {
    this.filterForm.pageNo = 1;
    this.loadApplications();
  }
  
  resetFilters(): void {
    this.filterForm = {
      startDate: null,
      endDate: null,
      applicationStatus: null,
      pageNo: 1,
      pageSize: 5
    };
    this.loadApplications();
  }
  
  getStatusLabel(status: ApplicationStatus): string {
    return this.applicationStatuses.find(s => s.value === status)?.label || 'Unknown';
  }
}
