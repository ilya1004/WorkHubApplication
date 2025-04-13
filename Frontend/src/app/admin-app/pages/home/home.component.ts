import {Component, OnInit} from '@angular/core';
import {HealthCheckEntry, HealthCheckResponse, ServiceHealth} from "../../interfaces/health-checks.interface";
import {HealthChecksService} from "../../services/health-checks.service";
import {forkJoin} from "rxjs";
import {NzSpinComponent} from "ng-zorro-antd/spin";
import {NzCardComponent} from "ng-zorro-antd/card";
import {CommonModule, NgForOf, NgIf} from "@angular/common";
import {NzTableComponent, NzTableModule} from "ng-zorro-antd/table";
import {NzTagComponent} from "ng-zorro-antd/tag";
import {NzInputModule} from "ng-zorro-antd/input";
import {NzSelectModule} from "ng-zorro-antd/select";
import {NzButtonModule} from "ng-zorro-antd/button";
import {NzFormModule} from "ng-zorro-antd/form";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {NzFlexDirective} from "ng-zorro-antd/flex";
import {ProjectsService} from "../../../core/services/projects/projects.service";
import {CategoriesService} from "../../../core/services/categories/categories.service";
import {Router} from "@angular/router";
import {Project} from "../../../core/interfaces/project/project.interface";
import {Category} from "../../../core/interfaces/project/category.interface";
import {PaginatedResult} from "../../../core/interfaces/common/paginated-result.interface";
import {PROJECT_STATUSES} from "../../../core/data/constants";
import {NzIconDirective} from "ng-zorro-antd/icon";


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
    ReactiveFormsModule,
    NzTagComponent,
    NzIconDirective,
    NzFlexDirective
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent{
  
  projectsService: ServiceHealth = { name: 'Projects Service', response: null, error: null };
  identityService: ServiceHealth = { name: 'Identity Service', response: null, error: null };
  paymentsService: ServiceHealth = { name: 'Payments Service', response: null, error: null };
  chatService: ServiceHealth = { name: 'Chat Service', response: null, error: null };
  loading = false;
  
  constructor(private healthChecksService: HealthChecksService) {}
  
  ngOnInit(): void {
    this.loadHealthChecks();
  }
  
  loadHealthChecks(): void {
    this.loading = true;
    const requests = this.healthChecksService.getHealthChecks();
    
    forkJoin(requests).subscribe({
      next: ([projects, identity, payments, chat]) => {
        this.projectsService = { ...this.projectsService, response: projects, error: null };
        this.identityService = { ...this.identityService, response: identity, error: null };
        this.paymentsService = { ...this.paymentsService, response: payments, error: null };
        this.chatService = { ...this.chatService, response: chat, error: null };
        this.loading = false;
      },
      error: (error) => {
        this.projectsService = { ...this.projectsService, error: `Failed to load: ${error.message}` };
        this.identityService = { ...this.identityService, error: `Failed to load: ${error.message}` };
        this.paymentsService = { ...this.paymentsService, error: `Failed to load: ${error.message}` };
        this.chatService = { ...this.chatService, error: `Failed to load: ${error.message}` };
        this.loading = false;
      }
    });
  }
  
  getStatusColor(status: string): string {
    return status === 'Healthy' ? 'green' : 'red';
  }
  
  getEntries(response: HealthCheckResponse | null): HealthCheckEntry[] {
    return response?.entries ? Object.values(response.entries) : [];
  }
  
  getEntryKeys(response: HealthCheckResponse | null): string[] {
    return response?.entries ? Object.keys(response.entries) : [];
  }
  
  listOfData: Person[] = [
    {
      key: '1',
      name: 'John Brown',
      age: 32,
      address: 'New York No. 1 Lake Park'
    },
    {
      key: '2',
      name: 'Jim Green',
      age: 42,
      address: 'London No. 1 Lake Park'
    },
    {
      key: '3',
      name: 'Joe Black',
      age: 32,
      address: 'Sidney No. 1 Lake Park'
    }
  ];
}

interface Person {
  key: string;
  name: string;
  age: number;
  address: string;
}