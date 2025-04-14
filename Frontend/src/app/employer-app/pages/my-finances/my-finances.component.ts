import {Component, OnInit} from '@angular/core';
import {CommonModule, DatePipe} from "@angular/common";
import {NzFlexDirective} from "ng-zorro-antd/flex";
import {NzCardComponent} from "ng-zorro-antd/card";
import {NzDescriptionsModule} from "ng-zorro-antd/descriptions";
import {NzButtonModule} from 'ng-zorro-antd/button';
import {NzTableModule} from 'ng-zorro-antd/table';
import {FinanceService} from "../../services/finance.service";
import {EmployerAccount} from "../../interfaces/finance/employer-account.interface";
import {Charge} from "../../interfaces/finance/charge.interface";
import {NzModalModule, NzModalService} from "ng-zorro-antd/modal";
import {PaymentMethod} from "../../interfaces/finance/payment-method.interface";
import {AddPaymentMethodModalComponent} from "./add-payment-method-modal/add-payment-method-modal.component";
import {PaymentIntent} from "../../interfaces/finance/payment-intent.interface";
import {NzSpinModule} from "ng-zorro-antd/spin";
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
    NzTableModule,
    NzModalModule,
    NzSpinModule,
    DatePipe
  ],
  templateUrl: './my-finances.component.html',
  styleUrls: ['./my-finances.component.scss'],
  providers: [NzMessageService]
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
  
  chargesPageNo: number = 1;
  chargesPageSize: number = 10;
  chargesTotalCount: number = 0;
  
  paymentIntentsPageNo: number = 1;
  paymentIntentsPageSize: number = 10;
  paymentIntentsTotalCount: number = 0;
  
  constructor(
    private financeService: FinanceService,
    private modalService: NzModalService,
    private message: NzMessageService
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
      this.message.success('Payment method saved successfully!', { nzDuration: 3000 });
      this.loadPaymentMethods();
      modal.close();
    });
  }
  
  deletePaymentMethod(paymentMethodId: string): void {
    this.financeService.deletePaymentMethod(paymentMethodId).subscribe({
      next: () => {
        this.message.success('Payment method deleted successfully!', { nzDuration: 3000 });
        this.loadPaymentMethods();
      },
      error: (error) => {
        this.message.error('Failed to delete payment method.', { nzDuration: 3000 });
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
          this.message.warning('Failed to load account information. Maybe you don\'t have one', { nzDuration: 3000 });
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
        this.message.success('Account created successfully!', { nzDuration: 3000 });
        this.loadAccount();
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
}