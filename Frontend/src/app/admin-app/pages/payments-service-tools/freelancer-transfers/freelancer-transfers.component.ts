import {Component, OnInit} from '@angular/core';
import {CommonModule} from "@angular/common";
import {NzTableModule} from "ng-zorro-antd/table";
import {NzSpinModule} from "ng-zorro-antd/spin";
import {NzMessageService} from "ng-zorro-antd/message";
import {Transfer} from '../../../../freelancer-app/interfaces/finance/transfer.interface';
import {PaymentsService} from "../../../services/payments/payments.service";
import {PaginatedResult} from "../../../../core/interfaces/common/paginated-result.interface";

@Component({
  selector: 'app-freelancer-transfers',
  standalone: true,
  imports: [
    CommonModule,
    NzTableModule,
    NzSpinModule
  ],
  providers: [NzMessageService],
  templateUrl: './freelancer-transfers.component.html',
  styleUrl: './freelancer-transfers.component.scss'
})
export class FreelancerTransfersComponent implements OnInit {
  transfers: Transfer[] = [];
  totalCount = 0;
  loading = false;
  pageNo = 1;
  pageSize = 10;
  
  constructor(
    private paymentsService: PaymentsService,
    private message: NzMessageService
  ) {}
  
  ngOnInit(): void {
    this.loadTransfers();
  }
  
  loadTransfers(): void {
    this.loading = true;
    this.paymentsService.getAllFreelancerTransfers(this.pageNo, this.pageSize).subscribe({
      next: (result: PaginatedResult<Transfer>) => {
        this.transfers = result.items;
        this.totalCount = result.totalCount;
        this.loading = false;
      },
      error: () => {
        this.message.error('Failed to load freelancer transfers', { nzDuration: 2000 });
        this.loading = false;
      }
    });
  }
  
  onPageChange(page: number): void {
    this.pageNo = page;
    this.loadTransfers();
  }
  
  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.pageNo = 1;
    this.loadTransfers();
  }
}