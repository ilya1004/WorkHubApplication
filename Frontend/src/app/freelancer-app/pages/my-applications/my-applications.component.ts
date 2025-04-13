import {Component, OnInit} from '@angular/core';
import {ApplicationStatus, FreelancerApplication} from '../../../core/interfaces/project/freelancer-application.interface';
import {CommonModule, DatePipe} from "@angular/common";
import {FormsModule} from '@angular/forms';
import {NzTableModule} from "ng-zorro-antd/table";
import {NzButtonModule} from "ng-zorro-antd/button";
import {NzDatePickerModule} from "ng-zorro-antd/date-picker";
import {NzSpinModule} from "ng-zorro-antd/spin";
import {NzFormModule} from "ng-zorro-antd/form";
import {NzFlexDirective} from "ng-zorro-antd/flex";
import {NzSelectModule} from "ng-zorro-antd/select";
import {FreelancerApplicationsService} from "../../../core/services/freelancer-applications/freelancer-applications.service";
import {RouterLink} from "@angular/router";

interface FilterParams {
  startDate: Date | null;
  endDate: Date | null;
  status: ApplicationStatus | null;
  pageNo: number;
  pageSize: number;
}

@Component({
  selector: 'app-my-applications',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    NzTableModule,
    NzButtonModule,
    NzDatePickerModule,
    NzSelectModule,
    NzSpinModule,
    NzFormModule,
    NzFlexDirective,
    DatePipe,
    RouterLink
  ],
  templateUrl: './my-applications.component.html',
  styleUrls: ['./my-applications.component.scss']
})
export class MyApplicationsComponent implements OnInit {
  applications: FreelancerApplication[] = [];
  isLoading: boolean = true;
  totalCount: number = 0;
  pageNo: number = 1;
  pageSize: number = 10;
  
  filter: FilterParams = {
    startDate: null,
    endDate: null,
    status: null,
    pageNo: 1,
    pageSize: 10
  };
  
  applicationStatusOptions = [
    { label: 'Pending', value: ApplicationStatus.Pending },
    { label: 'Accepted', value: ApplicationStatus.Accepted },
    { label: 'Rejected', value: ApplicationStatus.Rejected }
  ];
  
  constructor(
    private freelancerApplicationsService: FreelancerApplicationsService
  ) {}
  
  ngOnInit(): void {
    this.loadApplications();
  }
  
  loadApplications(): void {
    this.isLoading = true;
    const params = {
      startDate: this.filter.startDate ? this.filter.startDate.toISOString() : null,
      endDate: this.filter.endDate ? this.filter.endDate.toISOString() : null,
      status: this.filter.status,
      pageNo: this.filter.pageNo,
      pageSize: this.filter.pageSize
    };
    
    this.freelancerApplicationsService.getFreelancerApplications(params).subscribe({
      next: (result) => {
        this.applications = result.items;
        this.totalCount = result.totalCount;
        this.pageNo = result.pageNo;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading applications:', error);
        this.isLoading = false;
      }
    });
  }
  
  onPageChange(page: number): void {
    this.filter.pageNo = page;
    this.loadApplications();
  }
  
  applyFilter(): void {
    this.filter.pageNo = 1;
    this.loadApplications();
  }
  
  resetFilter(): void {
    this.filter = {
      startDate: null,
      endDate: null,
      status: null,
      pageNo: 1,
      pageSize: 10
    };
    this.loadApplications();
  }
  
  getStatusLabel(status: ApplicationStatus): string {
    switch (status) {
      case ApplicationStatus.Pending:
        return 'Pending';
      case ApplicationStatus.Accepted:
        return 'Accepted';
      case ApplicationStatus.Rejected:
        return 'Rejected';
      default:
        return 'Unknown';
    }
  }
}
