import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import {Observable} from "rxjs";
import {PaginatedResult} from "../../../core/interfaces/common/paginated-result.interface";
import {EmployerIndustry} from "../../../core/interfaces/employer/employer-industry.interface";
import {environment} from "../../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class EmployerIndustriesService {
  
  constructor(
    private httpClient: HttpClient
  ) {}
  
  getAllIndustries(pageNo: number, pageSize: number): Observable<PaginatedResult<EmployerIndustry>> {
    const params = new HttpParams()
      .set('PageNo', pageNo.toString())
      .set('PageSize', pageSize.toString());
    return this.httpClient.get<PaginatedResult<EmployerIndustry>>(
      `${environment.IDENTITY_SERVICE_API_URL}employer-industries`,
      { params }
    );
  }
  
  getIndustryById(id: string): Observable<EmployerIndustry> {
    return this.httpClient.get<EmployerIndustry>(
      `${environment.IDENTITY_SERVICE_API_URL}employer-industries/${id}`
    );
  }
  
  createIndustry(name: string): Observable<void> {
    return this.httpClient.post<void>(
      `${environment.IDENTITY_SERVICE_API_URL}employer-industries`,
      { name }
    );
  }
  
  updateIndustry(id: string, name: string): Observable<void> {
    return this.httpClient.put<void>(
      `${environment.IDENTITY_SERVICE_API_URL}employer-industries/${id}`,
      { name }
    );
  }
  
  deleteIndustry(id: string): Observable<void> {
    return this.httpClient.delete<void>(
      `${environment.IDENTITY_SERVICE_API_URL}employer-industries/${id}`
    );
  }
}
