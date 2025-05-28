import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import {Observable} from 'rxjs';
import {PaginatedResult} from '../../../core/interfaces/common/paginated-result.interface';
import {EmployerAccount} from "../../../employer-app/interfaces/finance/employer-account.interface";
import {environment} from "../../../../environments/environment";
import {FreelancerAccount} from "../../../freelancer-app/interfaces/finance/freelancer-account.interface";
import {Charge} from "../../../employer-app/interfaces/finance/charge.interface";
import {Transfer} from "../../../freelancer-app/interfaces/finance/transfer.interface";

@Injectable({
  providedIn: 'root'
})
export class PaymentsService {
  
  constructor(private httpClient: HttpClient) {}
  
  getAllEmployerAccounts(pageNo: number, pageSize: number): Observable<PaginatedResult<EmployerAccount>> {
    const params = new HttpParams()
      .set('PageNo', pageNo.toString())
      .set('PageSize', pageSize.toString());
    return this.httpClient.get<PaginatedResult<EmployerAccount>>(
      `${environment.PAYMENTS_SERVICE_API_URL}accounts/by-employer`,
      { params }
    );
  }
  
  getAllFreelancerAccounts(pageNo: number, pageSize: number): Observable<PaginatedResult<FreelancerAccount>> {
    const params = new HttpParams()
      .set('PageNo', pageNo.toString())
      .set('PageSize', pageSize.toString());
    return this.httpClient.get<PaginatedResult<FreelancerAccount>>(
      `${environment.PAYMENTS_SERVICE_API_URL}accounts/by-freelancer`,
      { params }
    );
  }
  
  getAllEmployerPayments(pageNo: number, pageSize: number): Observable<PaginatedResult<Charge>> {
    const params = new HttpParams()
      .set('pageNo', pageNo.toString())
      .set('pageSize', pageSize.toString());
    return this.httpClient.get<PaginatedResult<Charge>>(
      `${environment.PAYMENTS_SERVICE_API_URL}payments/employer-payments`,
      { params });
  }
  
  getAllFreelancerTransfers(pageNo: number, pageSize: number): Observable<PaginatedResult<Transfer>> {
    const params = new HttpParams()
      .set('pageNo', pageNo.toString())
      .set('pageSize', pageSize.toString());
    return this.httpClient.get<PaginatedResult<Transfer>>(
      `${environment.PAYMENTS_SERVICE_API_URL}payments/freelancer-transfers`,
      { params }
    );
  }
}
