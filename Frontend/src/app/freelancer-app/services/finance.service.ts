import {Injectable} from '@angular/core';
import {Observable} from 'rxjs';
import {HttpClient, HttpParams} from "@angular/common/http";
import {FreelancerAccount} from "../interfaces/finance/freelancer-account.interface";
import {Transfer} from "../interfaces/finance/transfer.interface";
import {PaginatedResult} from "../../core/interfaces/common/paginated-result.interface";
import {environment} from "../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class FinanceService {
  constructor(private http: HttpClient) {}
  
  createFreelancerAccount(): Observable<void> {
    return this.http.post<void>(
      `${environment.PAYMENTS_SERVICE_API_URL}accounts/freelancer`, {}
    );
  }
  
  getFreelancerAccount(): Observable<FreelancerAccount> {
    return this.http.get<FreelancerAccount>(
      `${environment.PAYMENTS_SERVICE_API_URL}accounts/freelancer/my-account`
    );
  }
  
  getFreelancerTransfers(pageNo: number = 1, pageSize: number = 10): Observable<PaginatedResult<Transfer>> {
    const params = new HttpParams()
      .set('pageNo', pageNo.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<PaginatedResult<Transfer>>(
      `${environment.PAYMENTS_SERVICE_API_URL}payments/freelancer/my-transfers`,
      { params }
    );
  }
}