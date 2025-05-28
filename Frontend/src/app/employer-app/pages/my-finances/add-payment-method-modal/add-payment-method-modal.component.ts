import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {NzButtonComponent} from "ng-zorro-antd/button";
import {loadStripe, Stripe, StripeCardElement} from "@stripe/stripe-js";
import {FinanceService} from "../../../services/finance.service";
import {environment} from "../../../../../environments/environment";

@Component({
  selector: 'app-add-payment-method-modal',
  standalone: true,
  imports: [
    NzButtonComponent
  ],
  templateUrl: './add-payment-method-modal.component.html',
  styleUrl: './add-payment-method-modal.component.scss'
})
export class AddPaymentMethodModalComponent implements OnInit {
  @Output() paymentMethodSaved = new EventEmitter<void>();
  private stripe: Stripe | null = null;
  private cardElement: StripeCardElement | null = null;
  
  constructor(private financeService: FinanceService) {}
  
  async ngOnInit(): Promise<void> {
    this.stripe = await loadStripe(environment.STRIPE_PUBLIC_KEY);
    if (this.stripe) {
      const elements = this.stripe.elements();
      this.cardElement = elements.create('card', {
        style: {
          base: {
            fontSize: '16px',
            color: '#32325d',
            '::placeholder': { color: '#aab7c4' }
          },
          invalid: { color: '#fa755a' }
        }
      });
      this.cardElement.mount('#card-element');
    }
  }
  
  async savePaymentMethod(): Promise<void> {
    if (!this.stripe || !this.cardElement) return;
    
    const { paymentMethod, error } = await this.stripe.createPaymentMethod({
      type: 'card',
      card: this.cardElement
    });
    
    if (error) {
      console.error('Stripe error:', error);
      return;
    }
    
    if (paymentMethod) {
      this.financeService.savePaymentMethod(paymentMethod.id).subscribe({
        next: () => {
          this.paymentMethodSaved.emit();
        },
        error: (err) => {
          console.error('Error saving payment method:', err);
        }
      });
    }
  }
}