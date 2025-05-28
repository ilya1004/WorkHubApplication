import {Component, OnInit} from '@angular/core';
import {CommonModule} from "@angular/common";
import {NzTableModule} from 'ng-zorro-antd/table';
import {NzButtonModule} from "ng-zorro-antd/button";
import {NzSpinModule} from 'ng-zorro-antd/spin';
import {NzFormModule} from "ng-zorro-antd/form";
import {NzInputModule} from "ng-zorro-antd/input";
import {NzCardModule} from "ng-zorro-antd/card";
import {NzModalModule, NzModalService} from "ng-zorro-antd/modal";
import {FormsModule} from "@angular/forms";
import {NzMessageService} from "ng-zorro-antd/message";
import {EmployerIndustry} from '../../../../core/interfaces/employer/employer-industry.interface';
import {EmployerIndustriesService} from "../../../services/employer-industries/employer-industries.service";
import {PaginatedResult} from "../../../../core/interfaces/common/paginated-result.interface";
import {NzFlexDirective} from "ng-zorro-antd/flex";

interface IndustryForm {
  id: string | null;
  name: string;
}

@Component({
  selector: 'app-employer-industries',
  standalone: true,
  imports: [
    CommonModule,
    NzTableModule,
    NzButtonModule,
    NzSpinModule,
    NzFormModule,
    NzInputModule,
    NzCardModule,
    NzModalModule,
    FormsModule,
    NzFlexDirective
  ],
  providers: [NzModalService, NzMessageService],
  templateUrl: './employer-industries.component.html',
  styleUrl: './employer-industries.component.scss'
})
export class EmployerIndustriesComponent implements OnInit {
  industries: EmployerIndustry[] = [];
  totalCount = 0;
  loading = false;
  pageNo = 1;
  pageSize = 5;
  
  showCreateForm = false;
  showEditForm = false;
  createForm: IndustryForm = { id: null, name: '' };
  editForm: IndustryForm = { id: null, name: '' };
  
  constructor(
    private industriesService: EmployerIndustriesService,
    private modal: NzModalService,
    private message: NzMessageService
  ) {}
  
  ngOnInit(): void {
    this.loadIndustries();
  }
  
  loadIndustries(): void {
    this.loading = true;
    this.industriesService.getAllIndustries(this.pageNo, this.pageSize).subscribe({
      next: (result: PaginatedResult<EmployerIndustry>) => {
        this.industries = result.items;
        this.totalCount = result.totalCount;
        this.loading = false;
      },
      error: () => {
        this.message.error('Failed to load industries', { nzDuration: 2000 });
        this.loading = false;
      }
    });
  }
  
  onPageChange(page: number): void {
    this.pageNo = page;
    this.loadIndustries();
  }
  
  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.pageNo = 1;
    this.loadIndustries();
  }
  
  toggleCreateForm(): void {
    this.showCreateForm = !this.showCreateForm;
    this.showEditForm = false;
    this.createForm = { id: null, name: '' };
  }
  
  createIndustry(): void {
    if (!this.createForm.name.trim()) {
      this.message.error('Name cannot be empty', { nzDuration: 2000 });
      return;
    }
    if (this.createForm.name.length > 200) {
      this.message.error('Name must not exceed 200 characters', { nzDuration: 2000 });
      return;
    }
    
    this.loading = true;
    this.industriesService.createIndustry(this.createForm.name).subscribe({
      next: () => {
        this.message.success('Industry created successfully', { nzDuration: 2000 });
        this.showCreateForm = false;
        this.createForm = { id: null, name: '' };
        this.loadIndustries();
      },
      error: () => {
        this.message.error('Failed to create industry', { nzDuration: 2000 });
        this.loading = false;
      }
    });
  }
  
  editIndustry(industry: EmployerIndustry): void {
    this.showEditForm = true;
    this.showCreateForm = false;
    this.editForm = { id: industry.id, name: industry.name };
  }
  
  updateIndustry(): void {
    if (!this.editForm.name.trim()) {
      this.message.error('Name cannot be empty', { nzDuration: 2000 });
      return;
    }
    if (this.editForm.name.length > 200) {
      this.message.error('Name must not exceed 200 characters', { nzDuration: 2000 });
      return;
    }
    if (!this.editForm.id) {
      this.message.error('Invalid industry ID', { nzDuration: 2000 });
      return;
    }
    
    this.loading = true;
    this.industriesService.updateIndustry(this.editForm.id, this.editForm.name).subscribe({
      next: () => {
        this.message.success('Industry updated successfully', { nzDuration: 2000 });
        this.showEditForm = false;
        this.editForm = { id: null, name: '' };
        this.loadIndustries();
      },
      error: () => {
        this.message.error('Failed to update industry', { nzDuration: 2000 });
        this.loading = false;
      }
    });
  }
  
  cancelEdit(): void {
    this.showEditForm = false;
    this.editForm = { id: null, name: '' };
  }
  
  deleteIndustry(industryId: string): void {
    this.modal.confirm({
      nzTitle: 'Are you sure you want to delete this industry?',
      nzOnOk: () => {
        this.loading = true;
        this.industriesService.deleteIndustry(industryId).subscribe({
          next: () => {
            this.message.success('Industry deleted successfully', { nzDuration: 2000 });
            this.loadIndustries();
          },
          error: () => {
            this.message.error('Failed to delete industry', { nzDuration: 2000 });
            this.loading = false;
          }
        });
      }
    });
  }
}
