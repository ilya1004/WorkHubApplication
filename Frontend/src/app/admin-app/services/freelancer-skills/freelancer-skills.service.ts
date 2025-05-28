import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from "@angular/common/http";
import {Observable} from "rxjs";
import {PaginatedResult} from "../../../core/interfaces/common/paginated-result.interface";
import {FreelancerSkill} from "../../../core/interfaces/freelancer/freelancer-skill.interface";
import {environment} from "../../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class FreelancerSkillsService {
  
  constructor(
    private httpClient: HttpClient
  ) {}
  
  getAllSkills(pageNo: number, pageSize: number): Observable<PaginatedResult<FreelancerSkill>> {
    const params = new HttpParams()
      .set('PageNo', pageNo.toString())
      .set('PageSize', pageSize.toString());
    return this.httpClient.get<PaginatedResult<FreelancerSkill>>(
      `${environment.IDENTITY_SERVICE_API_URL}freelancer-skills`,
      { params }
    );
  }
  
  getSkillById(id: string): Observable<FreelancerSkill> {
    return this.httpClient.get<FreelancerSkill>(
      `${environment.IDENTITY_SERVICE_API_URL}/${id}`
    );
  }
  
  createSkill(name: string): Observable<void> {
    return this.httpClient.post<void>(
      `${environment.IDENTITY_SERVICE_API_URL}freelancer-skills`,
      { name }
    );
  }
  
  updateSkill(id: string, name: string): Observable<void> {
    return this.httpClient.put<void>(
      `${environment.IDENTITY_SERVICE_API_URL}freelancer-skills/${id}`,
      { name }
    );
  }
  
  deleteSkill(id: string): Observable<void> {
    return this.httpClient.delete<void>(
      `${environment.IDENTITY_SERVICE_API_URL}freelancer-skills/${id}`
    );
  }
}
