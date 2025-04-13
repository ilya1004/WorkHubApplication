import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {EmployerUser} from "../../core/interfaces/employer/employer-user.interface";
import {EmployerIndustry} from "../../core/interfaces/employer/employer-industry.interface";
import {PaginatedResult} from "../../core/interfaces/common/paginated-result.interface";
import {environment} from "../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  
  constructor(
    private http: HttpClient
  ) {}
  
  getUserData(): Observable<EmployerUser> {
    return this.http.get<EmployerUser>(
      `${environment.IDENTITY_SERVICE_API_URL}users/my-employer-info`
    );
  }
  
  updateEmployerProfile(formData: FormData): Observable<void> {
    return this.http.put<void>(
      `${environment.IDENTITY_SERVICE_API_URL}users/update-employer`,
      formData
    );
  }
  
  changePassword(request: { email: string; currentPassword: string; newPassword: string }): Observable<void> {
    return this.http.post<void>(
      `${environment.IDENTITY_SERVICE_API_URL}users/change-password`,
      request
    );
  }
  
  getAvailableIndustries(): Observable<PaginatedResult<EmployerIndustry>> {
    return this.http.get<PaginatedResult<EmployerIndustry>>(
      `${environment.IDENTITY_SERVICE_API_URL}employer-industries`
    );
  }
}
