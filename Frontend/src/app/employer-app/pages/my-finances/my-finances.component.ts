import {Component, OnInit} from '@angular/core';
import {CommonModule, DatePipe} from "@angular/common";
import {NzFlexDirective} from "ng-zorro-antd/flex";
import {NzCardComponent} from "ng-zorro-antd/card";
import {NzDescriptionsModule} from "ng-zorro-antd/descriptions";
import {NzAlertModule} from "ng-zorro-antd/alert";
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzTableModule } from 'ng-zorro-antd/table';
import {FinanceService} from "../../services/finance.service";
import {EmployerAccount} from "../../interfaces/finance/employer-account.interface";
import {Charge} from "../../interfaces/finance/charge.interface";
import {NzModalModule, NzModalService} from "ng-zorro-antd/modal";
import {PaymentMethod} from "../../interfaces/finance/payment-method.interface";
import {AddPaymentMethodModalComponent} from "./add-payment-method-modal/add-payment-method-modal.component";
import {PaymentIntent} from "../../interfaces/finance/payment-intent.interface";
import {NzSpinModule} from "ng-zorro-antd/spin";

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
    NzTableModule,
    NzModalModule,
    NzSpinModule,
    DatePipe
  ],
  templateUrl: './my-finances.component.html',
  styleUrls: ['./my-finances.component.scss']
})
export class MyFinancesComponent implements OnInit {
  account: EmployerAccount | null = null;
  charges: Charge[] = [];
  paymentMethods: PaymentMethod[] = [];
  paymentIntents: PaymentIntent[] = [];
  
  isLoadingAccount: boolean = true;
  isLoadingCharges: boolean = true;
  isLoadingPaymentMethods: boolean = true;
  isLoadingPaymentIntents: boolean = true;
  isCreatingAccount: boolean = false;
  accountSuccessMessage: string | null = null;
  accountErrorMessage: string | null = null;
  
  // Пагинация для Charges
  chargesPageNo: number = 1;
  chargesPageSize: number = 10;
  chargesTotalCount: number = 0;
  
  // Пагинация для PaymentIntents
  paymentIntentsPageNo: number = 1;
  paymentIntentsPageSize: number = 10;
  paymentIntentsTotalCount: number = 0;
  
  constructor(
    private financeService: FinanceService,
    private modalService: NzModalService
  ) {}
  
  ngOnInit(): void {
    this.loadAccount();
    this.loadCharges();
    this.loadPaymentMethods();
    this.loadPaymentIntents();
  }
  
  showAddPaymentMethodModal(): void {
    const modal = this.modalService.create({
      nzTitle: 'Add Payment Method',
      nzContent: AddPaymentMethodModalComponent,
      nzFooter: null,
      nzOnOk: () => { }
    });
    
    modal.componentInstance?.paymentMethodSaved.subscribe(() => {
      this.accountSuccessMessage = 'Payment method saved successfully!';
      this.loadPaymentMethods();
      modal.close();
      setTimeout(() => this.accountSuccessMessage = null, 5000);
    });
  }
  
  deletePaymentMethod(paymentMethodId: string): void {
    this.financeService.deletePaymentMethod(paymentMethodId).subscribe({
      next: () => {
        this.accountSuccessMessage = 'Payment method deleted successfully!';
        this.loadPaymentMethods();
        setTimeout(() => this.accountSuccessMessage = null, 5000);
      },
      error: (error) => {
        this.accountErrorMessage = 'Failed to delete payment method.';
        console.error('Error deleting payment method:', error);
      }
    });
  }
  
  loadAccount(): void {
    this.isLoadingAccount = true;
    this.financeService.getEmployerAccount().subscribe({
      next: (account) => {
        this.account = account;
        this.isLoadingAccount = false;
      },
      error: (error) => {
        if (error.status === 404) {
          this.account = null;
        } else {
          this.accountErrorMessage = 'Failed to load account information.';
          console.error('Error loading account:', error);
        }
        this.isLoadingAccount = false;
      }
    });
  }
  
  loadCharges(): void {
    this.isLoadingCharges = true;
    this.financeService.getEmployerPayments({ pageNo: this.chargesPageNo, pageSize: this.chargesPageSize }).subscribe({
      next: (result) => {
        this.charges = result.items;
        this.chargesTotalCount = result.totalCount;
        this.isLoadingCharges = false;
      },
      error: (error) => {
        console.error('Error loading charges:', error);
        this.isLoadingCharges = false;
        this.accountErrorMessage = 'Failed to load payments.';
      }
    });
  }
  
  loadPaymentMethods(): void {
    this.isLoadingPaymentMethods = true;
    this.financeService.getMyPaymentMethods().subscribe({
      next: (methods) => {
        this.paymentMethods = methods;
        this.isLoadingPaymentMethods = false;
      },
      error: (error) => {
        console.error('Error loading payment methods:', error);
        this.isLoadingPaymentMethods = false;
        this.accountErrorMessage = 'Failed to load payment methods.';
      }
    });
  }
  
  loadPaymentIntents(): void {
    this.isLoadingPaymentIntents = true;
    this.financeService.getEmployerPaymentIntents({ pageNo: this.paymentIntentsPageNo, pageSize: this.paymentIntentsPageSize }).subscribe({
      next: (result) => {
        this.paymentIntents = result.items;
        this.paymentIntentsTotalCount = result.totalCount;
        this.isLoadingPaymentIntents = false;
      },
      error: (error) => {
        console.error('Error loading payment intents:', error);
        this.isLoadingPaymentIntents = false;
        this.accountErrorMessage = 'Failed to load payment intents.';
      }
    });
  }
  
  onChargesPageChange(page: number): void {
    this.chargesPageNo = page;
    this.loadCharges();
  }
  
  onPaymentIntentsPageChange(page: number): void {
    this.paymentIntentsPageNo = page;
    this.loadPaymentIntents();
  }
  
  createAccount(): void {
    this.isCreatingAccount = true;
    this.financeService.createEmployerAccount().subscribe({
      next: () => {
        this.isCreatingAccount = false;
        this.accountSuccessMessage = 'Account created successfully!';
        this.loadAccount();
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
}