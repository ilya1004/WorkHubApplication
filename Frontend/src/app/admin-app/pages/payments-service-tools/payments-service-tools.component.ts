import { Component } from '@angular/core';
import {NgForOf} from "@angular/common";
import {NzFlexDirective} from "ng-zorro-antd/flex";
import {NzIconDirective} from "ng-zorro-antd/icon";
import {
  NzTableCellDirective,
  NzTableComponent,
  NzTbodyComponent,
  NzTheadComponent,
  NzThMeasureDirective,
  NzTrDirective
} from "ng-zorro-antd/table";
import {NzTagComponent} from "ng-zorro-antd/tag";
import {HealthCheckEntry, HealthCheckResponse, ServiceHealth} from "../../interfaces/health-checks/health-checks.interface";
import {HealthChecksService} from "../../services/health-checks/health-checks.service";
import {forkJoin} from "rxjs";

@Component({
  selector: 'app-payments-service-tools',
  standalone: true,
  imports: [
    NgForOf,
    NzFlexDirective,
    NzIconDirective,
    NzTableCellDirective,
    NzTableComponent,
    NzTagComponent,
    NzTbodyComponent,
    NzThMeasureDirective,
    NzTheadComponent,
    NzTrDirective,
  ],
  templateUrl: './payments-service-tools.component.html',
  styleUrl: './payments-service-tools.component.scss'
})
export class PaymentsServiceToolsComponent {
  
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
}
