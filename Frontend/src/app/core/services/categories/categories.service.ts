import { Injectable } from '@angular/core';
import {Observable} from "rxjs";
import {PaginatedResult} from "../../interfaces/common/paginated-result.interface";
import {Category} from "../../interfaces/project/category.interface";
import {HttpClient, HttpParams} from "@angular/common/http";
import {PROJECTS_SERVICE_API_URL} from "../../data/constants";

@Injectable({
  providedIn: 'root'
})
export class CategoriesService {

  constructor(private httpClient: HttpClient) { }
  
  getCategories(pageNo: number, pageSize: number): Observable<PaginatedResult<Category>> {
    const params = new HttpParams()
      .set('PageNo', pageNo.toString())
      .set('PageSize', pageSize.toString());
    return this.httpClient.get<PaginatedResult<Category>>(
      `${PROJECTS_SERVICE_API_URL}categories`,
      { params }
    );
  }
}
