import {Component} from '@angular/core';
import {EmployerAccountsComponent} from "./employer-accounts/employer-accounts.component";
import {FreelancerAccountsComponent} from "./freelancer-accounts/freelancer-accounts.component";
import {EmployerPaymentsComponent} from "./employer-payments/employer-payments.component";
import {FreelancerTransfersComponent} from "./freelancer-transfers/freelancer-transfers.component";


@Component({
  selector: 'app-payments-service-tools',
  standalone: true,
  imports: [
    EmployerAccountsComponent,
    FreelancerAccountsComponent,
    EmployerPaymentsComponent,
    FreelancerTransfersComponent
  ],
  templateUrl: './payments-service-tools.component.html',
  styleUrl: './payments-service-tools.component.scss'
})
export class PaymentsServiceToolsComponent {

}
