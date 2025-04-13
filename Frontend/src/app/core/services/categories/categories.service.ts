import {Injectable} from '@angular/core';
import {Observable} from "rxjs";
import {PaginatedResult} from "../../interfaces/common/paginated-result.interface";
import {Category} from "../../interfaces/project/category.interface";
import {HttpClient, HttpParams} from "@angular/common/http";
import {environment} from "../../../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class CategoriesService {
  constructor(private httpClient: HttpClient) {}
  
  getCategories(pageNo: number, pageSize: number): Observable<PaginatedResult<Category>> {
    const params = new HttpParams()
      .set('PageNo', pageNo.toString())
      .set('PageSize', pageSize.toString());
    return this.httpClient.get<PaginatedResult<Category>>(
      `${environment.PROJECTS_SERVICE_API_URL}categories`,
      { params }
    );
  }
  
  getCategoryById(categoryId: string): Observable<Category> {
    return this.httpClient.get<Category>(
      `${environment.PROJECTS_SERVICE_API_URL}categories/${categoryId}`
    );
  }
  
  createCategory(name: string): Observable<void> {
    return this.httpClient.post<void>(
      `${environment.PROJECTS_SERVICE_API_URL}categories`,
      { name }
    );
  }
  
  updateCategory(categoryId: string, name: string): Observable<void> {
    return this.httpClient.put<void>(
      `${environment.PROJECTS_SERVICE_API_URL}categories/${categoryId}`,
      { name }
    );
  }
  
  deleteCategory(categoryId: string): Observable<void> {
    return this.httpClient.delete<void>(
      `${environment.PROJECTS_SERVICE_API_URL}categories/${categoryId}`
    );
  }
}