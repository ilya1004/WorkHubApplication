import {Component, OnInit} from '@angular/core';
import {EmployerAccount} from "../../../../employer-app/interfaces/finance/employer-account.interface";
import {PaymentsService} from "../../../services/payments/payments.service";
import {PaginatedResult} from "../../../../core/interfaces/common/paginated-result.interface";
import {NzTableComponent, NzTableModule} from "ng-zorro-antd/table";
import {NgForOf} from "@angular/common";

@Component({
  selector: 'app-employer-accounts',
  standalone: true,
  imports: [
    NzTableComponent,
    NzTableModule,
    NgForOf
  ],
  templateUrl: './employer-accounts.component.html',
  styleUrl: './employer-accounts.component.scss'
})
export class EmployerAccountsComponent implements OnInit {
  accounts: EmployerAccount[] = [];
  totalCount = 0;
  loading = false;
  pageNo = 1;
  pageSize = 10;
  
  constructor(private paymentsService: PaymentsService) {}
  
  ngOnInit(): void {
    this.loadAccounts();
  }
  
  loadAccounts(): void {
    this.loading = true;
    this.paymentsService.getAllEmployerAccounts(this.pageNo, this.pageSize).subscribe({
      next: (result: PaginatedResult<EmployerAccount>) => {
        this.accounts = result.items;
        this.totalCount = result.totalCount;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading employer accounts:', error);
        this.loading = false;
      }
    });
  }
  
  onPageChange(page: number): void {
    this.pageNo = page;
    this.loadAccounts();
  }
  
  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.pageNo = 1;
    this.loadAccounts();
  }
}
