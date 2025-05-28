import {Component, OnInit} from '@angular/core';
import {CommonModule} from "@angular/common";
import {NzTableModule} from "ng-zorro-antd/table";
import {NzSpinModule} from "ng-zorro-antd/spin";
import {NzMessageService} from "ng-zorro-antd/message";
import {Charge} from '../../../../employer-app/interfaces/finance/charge.interface';
import {PaymentsService} from "../../../services/payments/payments.service";
import {PaginatedResult} from "../../../../core/interfaces/common/paginated-result.interface";

@Component({
  selector: 'app-employer-payments',
  standalone: true,
  imports: [
    CommonModule,
    NzTableModule,
    NzSpinModule
  ],
  providers: [NzMessageService],
  templateUrl: './employer-payments.component.html',
  styleUrl: './employer-payments.component.scss'
})
export class EmployerPaymentsComponent implements OnInit {
  payments: Charge[] = [];
  totalCount = 0;
  loading = false;
  pageNo = 1;
  pageSize = 10;
  
  constructor(
    private paymentsService: PaymentsService,
    private message: NzMessageService
  ) {}
  
  ngOnInit(): void {
    this.loadPayments();
  }
  
  loadPayments(): void {
    this.loading = true;
    this.paymentsService.getAllEmployerPayments(this.pageNo, this.pageSize).subscribe({
      next: (result: PaginatedResult<Charge>) => {
        this.payments = result.items;
        this.totalCount = result.totalCount;
        this.loading = false;
      },
      error: () => {
        this.message.error('Failed to load employer payments', { nzDuration: 2000 });
        this.loading = false;
      }
    });
  }
  
  onPageChange(page: number): void {
    this.pageNo = page;
    this.loadPayments();
  }
  
  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.pageNo = 1;
    this.loadPayments();
  }
}