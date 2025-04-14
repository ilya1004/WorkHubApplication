import {Component, OnInit} from '@angular/core';
import {CommonModule} from "@angular/common";
import {NzFlexDirective} from 'ng-zorro-antd/flex';
import {NzCardComponent} from "ng-zorro-antd/card";
import {NzDescriptionsModule} from "ng-zorro-antd/descriptions";
import {NzButtonModule} from "ng-zorro-antd/button";
import {NzTableModule} from "ng-zorro-antd/table";
import {FreelancerAccount} from "../../interfaces/finance/freelancer-account.interface";
import {Transfer} from "../../interfaces/finance/transfer.interface";
import {FinanceService} from "../../services/finance.service";
import {NzMessageService} from "ng-zorro-antd/message";

@Component({
  selector: 'app-my-finances',
  standalone: true,
  imports: [
    CommonModule,
    NzFlexDirective,
    NzCardComponent,
    NzDescriptionsModule,
    NzButtonModule,
    NzTableModule
  ],
  templateUrl: './my-finances.component.html',
  styleUrls: ['./my-finances.component.scss'],
  providers: [NzMessageService]
})
export class MyFinancesComponent implements OnInit {
  account: FreelancerAccount | null = null;
  transfers: Transfer[] = [];
  isLoadingAccount: boolean = true;
  isLoadingTransfers: boolean = true;
  isCreatingAccount: boolean = false;
  
  constructor(
    private financeService: FinanceService,
    private message: NzMessageService
  ) {}
  
  ngOnInit(): void {
    this.loadAccount();
    this.loadTransfers();
  }
  
  loadAccount(): void {
    this.isLoadingAccount = true;
    this.financeService.getFreelancerAccount().subscribe({
      next: (account) => {
        this.account = account;
        this.isLoadingAccount = false;
      },
      error: (error) => {
        if (error.status === 404) {
          this.account = null;
        } else {
          this.message.warning('Failed to load account information. Maybe you don\'t have one', { nzDuration: 3000 });
          console.error('Error loading account:', error);
        }
        this.isLoadingAccount = false;
      }
    });
  }
  
  loadTransfers(): void {
    this.isLoadingTransfers = true;
    this.financeService.getFreelancerTransfers().subscribe({
      next: (result) => {
        this.transfers = result.items;
        this.isLoadingTransfers = false;
      },
      error: (error) => {
        console.error('Error loading transfers:', error);
        this.isLoadingTransfers = false;
      }
    });
  }
  
  createAccount(): void {
    this.isCreatingAccount = true;
    this.financeService.createFreelancerAccount().subscribe({
      next: () => {
        this.isCreatingAccount = false;
        this.message.success('Account created successfully!', { nzDuration: 3000 });
        setTimeout(() => this.loadAccount(), 2000);
      },
      error: (error) => {
        this.isCreatingAccount = false;
        this.message.error('Failed to create account. Please try again.', { nzDuration: 3000 });
        console.error('Error creating account:', error);
      }
    });
  }
  
  refreshAccount(): void {
    this.loadAccount();
  }
  
  formatMetadata(metadata: { [key: string]: string }): string {
    return Object.entries(metadata).map(([key, value]) => `${key}: ${value}`).join(', ');
  }
}