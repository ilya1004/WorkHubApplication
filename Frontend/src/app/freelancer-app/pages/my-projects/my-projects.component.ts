import { Component } from '@angular/core';
import {NzCardComponent} from "ng-zorro-antd/card";
import {FormBuilder, FormGroup, ReactiveFormsModule} from "@angular/forms";
import {NzFormControlComponent, NzFormItemComponent, NzFormLabelComponent} from "ng-zorro-antd/form";
import {NzOptionComponent, NzSelectComponent} from "ng-zorro-antd/select";
import {NzColDirective, NzRowDirective} from "ng-zorro-antd/grid";
import {NzTableComponent, NzTableQueryParams} from "ng-zorro-antd/table";
import {MyProjectsService} from "../../services/my-projects.service";
import {Project} from "../../interfaces/my-projects/project.interface";
import {NzTagComponent} from "ng-zorro-antd/tag";
import {NzInputDirective} from "ng-zorro-antd/input";
import {NzButtonComponent} from "ng-zorro-antd/button";
import {CurrencyPipe, DatePipe, NgForOf} from "@angular/common";

@Component({
  selector: 'app-my-projects',
  imports: [
    NzCardComponent,
    ReactiveFormsModule,
    NzFormItemComponent,
    NzFormLabelComponent,
    NzSelectComponent,
    NzFormControlComponent,
    NzOptionComponent,
    NzColDirective,
    NzRowDirective,
    NzTableComponent,
    NzTagComponent,
    NzInputDirective,
    NzButtonComponent,
    NgForOf,
    DatePipe,
    CurrencyPipe
  ],
  templateUrl: './my-projects.component.html',
  styleUrl: './my-projects.component.scss'
})
export class MyProjectsComponent {
  projects: Project[] = [];
  loading = false;
  total = 0;
  pageSize = 10;
  pageNo = 1;

  filterForm!: FormGroup;

  constructor(
      private myProjectsService: MyProjectsService,
      private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.filterForm = this.fb.group({
      projectsStatus: [null],
      employerId: [null]
    });

    this.loadData();
  }

  loadData(): void {
    const filters = this.filterForm.value;
    this.loading = true;

    this.myProjectsService.getMyFreelancerProjects({
      projectsStatus: filters.projectsStatus,
      employerId: filters.employerId,
      pageNo: this.pageNo,
      pageSize: this.pageSize
    }).subscribe(result => {
      this.projects = result.items;
      this.total = result.totalCount;
      this.loading = false;
    });
  }

  onTableParamsChange(params: NzTableQueryParams): void {
    const { pageSize, pageIndex } = params;
    this.pageSize = pageSize;
    this.pageNo = pageIndex;
    this.loadData();
  }

  onFilterSubmit(): void {
    this.pageNo = 1;
    this.loadData();
  }

  getStatusText(status: number): string {
    switch (status) {
      case 0: return 'Создан';
      case 1: return 'В процессе';
      case 2: return 'Завершён';
      default: return 'Неизвестен';
    }
  }

  getStatusColor(status: number): string {
    switch (status) {
      case 0: return 'blue';
      case 1: return 'gold';
      case 2: return 'green';
      default: return 'default';
    }
  }
}
