import {Injectable} from '@angular/core';
import {Observable} from "rxjs";
import {FreelancerUser} from "../../interfaces/freelancer/freelancer-user.interface";
import {EmployerUser} from "../../interfaces/employer/employer-user.interface";
import {HttpClient, HttpParams} from "@angular/common/http";
import {environment} from "../../../../environments/environment";
import {PaginatedResult} from "../../interfaces/common/paginated-result.interface";
import {AppUser} from "../../../admin-app/interfaces/users/app-user.interface";

@Injectable({
  providedIn: 'root'
})
export class UsersService {

  constructor(
    private httpClient: HttpClient,
  ) { }

  getFreelancerInfo(userId: string): Observable<FreelancerUser> {
    return this.httpClient.get<FreelancerUser>(
      `${environment.IDENTITY_SERVICE_API_URL}users/freelancer-info/${userId}`
    );
  }

  getEmployerInfo(userId: string): Observable<EmployerUser> {
    return this.httpClient.get<EmployerUser>(
      `${environment.IDENTITY_SERVICE_API_URL}users/employer-info/${userId}`
    );
  }
  
  getAllUsers(pageNo: number, pageSize: number): Observable<PaginatedResult<AppUser>> {
    const params = new HttpParams()
      .set('PageNo', pageNo.toString())
      .set('PageSize', pageSize.toString());
    
    return this.httpClient.get<PaginatedResult<AppUser>>(
      `${environment.IDENTITY_SERVICE_API_URL}users`,
      { params }
    );
  }
  
  getUserById(userId: string): Observable<AppUser> {
    return this.httpClient.get<AppUser>(
      `${environment.IDENTITY_SERVICE_API_URL}users/${userId}`
    );
  }
  
  deleteUser(userId: string): Observable<void> {
    return this.httpClient.delete<void>(
      `${environment.IDENTITY_SERVICE_API_URL}users/${userId}`
    );
  }
}
