import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import {Observable} from "rxjs";
import {EmployerAccount} from "../interfaces/finance/employer-account.interface";
import {PaginatedResult} from "../../core/interfaces/common/paginated-result.interface";
import {Charge} from "../interfaces/finance/charge.interface";
import {PaymentMethod} from "../interfaces/finance/payment-method.interface";
import {environment} from "../../../environments/environment";
import {PaymentIntent} from "../interfaces/finance/payment-intent.interface";

@Injectable({
  providedIn: 'root'
})
export class FinanceService {
  constructor(private httpClient: HttpClient) {}
  
  getEmployerAccount(): Observable<EmployerAccount> {
    return this.httpClient.get<EmployerAccount>(
      `${environment.PAYMENTS_SERVICE_API_URL}accounts/employer/my-account`
    );
  }
  
  createEmployerAccount(): Observable<void> {
    return this.httpClient.post<void>(
      `${environment.PAYMENTS_SERVICE_API_URL}accounts/employer`, {}
    );
  }
  
  getEmployerPayments(params: { pageNo: number; pageSize: number }): Observable<PaginatedResult<Charge>> {
    let httpParams = new HttpParams()
      .set('PageNo', params.pageNo.toString())
      .set('PageSize', params.pageSize.toString());
    return this.httpClient.get<PaginatedResult<Charge>>(
      `${environment.PAYMENTS_SERVICE_API_URL}payments/employer/my-payments`,
      { params: httpParams }
    );
  }
  
  savePaymentMethod(paymentMethodId: string): Observable<void> {
    return this.httpClient.post<void>(
      `${environment.PAYMENTS_SERVICE_API_URL}payment-methods/${paymentMethodId}`, {}
    );
  }
  
  getMyPaymentMethods(): Observable<PaymentMethod[]> {
    return this.httpClient.get<PaymentMethod[]>(
      `${environment.PAYMENTS_SERVICE_API_URL}payment-methods/my-payment-methods`
    );
  }
  
  deletePaymentMethod(paymentMethodId: string): Observable<void> {
    return this.httpClient.delete<void>(
      `${environment.PAYMENTS_SERVICE_API_URL}payment-methods/${paymentMethodId}`
    );
  }
  
  createPaymentIntent(projectId: string, paymentMethodId: string): Observable<any> {
    return this.httpClient.post(
      `${environment.PAYMENTS_SERVICE_API_URL}payments/pay-for-project/${projectId}/with-method/${paymentMethodId}`, {}
    );
  }
  
  getEmployerPaymentIntents(params: { pageNo: number; pageSize: number }): Observable<PaginatedResult<PaymentIntent>> {
    let httpParams = new HttpParams()
      .set('PageNo', params.pageNo.toString())
      .set('PageSize', params.pageSize.toString());
    return this.httpClient.get<PaginatedResult<PaymentIntent>>(
      `${environment.PAYMENTS_SERVICE_API_URL}payments/employer/my-payment-intents`,
      { params: httpParams }
    );
  }
}