import {Component, OnInit} from '@angular/core';
import {PaymentsService} from "../../../services/payments/payments.service";
import {PaginatedResult} from "../../../../core/interfaces/common/paginated-result.interface";
import {FreelancerAccount} from "../../../../freelancer-app/interfaces/finance/freelancer-account.interface";
import {NzTableComponent, NzTableModule} from "ng-zorro-antd/table";
import {NgForOf} from "@angular/common";

@Component({
  selector: 'app-freelancer-accounts',
  standalone: true,
  imports: [
    NzTableComponent,
    NzTableModule,
    NgForOf
  ],
  templateUrl: './freelancer-accounts.component.html',
  styleUrl: './freelancer-accounts.component.scss'
})
export class FreelancerAccountsComponent implements OnInit {
  accounts: FreelancerAccount[] = [];
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
    this.paymentsService.getAllFreelancerAccounts(this.pageNo, this.pageSize).subscribe({
      next: (result: PaginatedResult<FreelancerAccount>) => {
        this.accounts = result.items;
        this.totalCount = result.totalCount;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading freelancer accounts:', error);
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
