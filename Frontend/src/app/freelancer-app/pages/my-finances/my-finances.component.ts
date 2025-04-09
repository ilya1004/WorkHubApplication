import {Component, OnInit} from '@angular/core';
import {CommonModule} from "@angular/common";
import { NzFlexDirective } from 'ng-zorro-antd/flex';
import {NzCardComponent} from "ng-zorro-antd/card";
import {NzDescriptionsModule} from "ng-zorro-antd/descriptions";
import {NzButtonModule} from "ng-zorro-antd/button";
import {NzAlertModule} from "ng-zorro-antd/alert";
import {NzTableModule} from "ng-zorro-antd/table";
import {FreelancerAccount} from "../../interfaces/finance/freelancer-account.interface";
import {Transfer} from "../../interfaces/finance/transfer.interface";
import {FinanceService} from "../../services/finance.service";

@Component({
  selector: 'app-my-finances',
  standalone: true,
  imports: [
    CommonModule,
    NzFlexDirective,
    NzCardComponent,
    NzDescriptionsModule,
    NzButtonModule,
    NzAlertModule,
    NzTableModule
  ],
  templateUrl: './my-finances.component.html',
  styleUrls: ['./my-finances.component.scss']
})
export class MyFinancesComponent implements OnInit {
  account: FreelancerAccount | null = null;
  transfers: Transfer[] = [];
  isLoadingAccount: boolean = true;
  isLoadingTransfers: boolean = true;
  isCreatingAccount: boolean = false;
  accountSuccessMessage: string | null = null;
  accountErrorMessage: string | null = null;
  
  constructor(private financeService: FinanceService) {}
  
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
          this.account = null; // Счет не создан
        } else {
          this.accountErrorMessage = 'Failed to load account information.';
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
        this.accountSuccessMessage = 'Account created successfully!';
        this.loadAccount(); // Перезагружаем информацию о счете
        setTimeout(() => this.accountSuccessMessage = null, 5000);
      },
      error: (error) => {
        this.isCreatingAccount = false;
        this.accountErrorMessage = 'Failed to create account. Please try again.';
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