import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {PAYMENTS_SERVICE_API_URL} from "../../core/constants";
import {HttpClient, HttpParams} from "@angular/common/http";
import {FreelancerAccount} from "../interfaces/finance/freelancer-account.interface";
import {Transfer} from "../interfaces/finance/transfer.interface";
import {PaginatedResult} from "../../core/interfaces/paginated-result.interface";

@Injectable({
  providedIn: 'root'
})
export class FinanceService {
  constructor(private http: HttpClient) {}
  
  createFreelancerAccount(): Observable<void> {
    return this.http.post<void>(
      `${PAYMENTS_SERVICE_API_URL}accounts/freelancer`, {}
    );
  }
  
  getFreelancerAccount(): Observable<FreelancerAccount> {
    return this.http.get<FreelancerAccount>(
      `${PAYMENTS_SERVICE_API_URL}accounts/freelancer/my-account`
    );
  }
  
  getFreelancerTransfers(pageNo: number = 1, pageSize: number = 10): Observable<PaginatedResult<Transfer>> {
    const params = new HttpParams()
      .set('pageNo', pageNo.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<PaginatedResult<Transfer>>(
      `${PAYMENTS_SERVICE_API_URL}payments/freelancer/my-transfers`,
      { params }
    );
  }
}