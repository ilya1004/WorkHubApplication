import { Injectable } from '@angular/core';
import {PaginatedResult} from "../../core/interfaces/paginated-result.interface";
import {FreelancerSkill} from "../interfaces/profile/skill.interface";
import {PROJECTS_SERVICE_API_URL} from "../../core/constants";
import {Project} from "../interfaces/my-projects/project.interface";
import {HttpClient, HttpParams} from "@angular/common/http";
import {Observable} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class MyProjectsService {

  constructor(
      private httpClient: HttpClient
  ) { }

  getMyFreelancerProjects(filter: {
    projectsStatus: number | null,
    employerId: number | null,
    pageNo: number,
    pageSize: number
  } ): Observable<PaginatedResult<Project>> {
    let params = new HttpParams()
        .set('pageNo', filter.pageNo.toString())
        .set('pageSize', filter.pageSize.toString());

    if (filter.projectsStatus !== null) {
      params = params.set('projectsStatus', filter.projectsStatus.toString());
    }

    if (filter.employerId !== null) {
      params = params.set('employerId', filter.employerId.toString());
    }

    return this.httpClient.get<PaginatedResult<Project>>(
        `${PROJECTS_SERVICE_API_URL}projects/my-freelancer-projects-filter`,
        { params },
    );
  }

  getMyEmployerProjects(filter: {
    updatedAtStartDate: string | null,
    updatedAtEndDate: string | null,
    projectStatus: number | null,
    acceptanceRequestedAndNotConfirmed: boolean | null,
    pageNo: number,
    pageSize: number
  }): Observable<PaginatedResult<Project>> {

    let params = new HttpParams()
        .set('pageNo', filter.pageNo.toString())
        .set('pageSize', filter.pageSize.toString());

    if (filter.updatedAtStartDate !== null) {
      params = params.set('updatedAtStartDate', filter.updatedAtStartDate);
    }

    if (filter.updatedAtEndDate !== null) {
      params = params.set('updatedAtEndDate', filter.updatedAtEndDate);
    }

    if (filter.projectStatus !== null) {
      params = params.set('projectStatus', filter.projectStatus.toString());
    }

    if (filter.acceptanceRequestedAndNotConfirmed !== null) {
      params = params.set('acceptanceRequestedAndNotConfirmed', filter.acceptanceRequestedAndNotConfirmed.toString());
    }

    return this.httpClient.get<PaginatedResult<Project>>(
        `${PROJECTS_SERVICE_API_URL}projects/my-employer-projects-filter`,
        { params }
    );
  }

  getProjectsByFilter(filter: {
    title: string | null,
    budgetFrom: number | null,
    budgetTo: number | null,
    categoryId: string | null,
    employerId: string | null,
    projectStatus: number | null,
    pageNo: number,
    pageSize: number
  }): Observable<PaginatedResult<Project>> {

    let params = new HttpParams()
        .set('pageNo', filter.pageNo.toString())
        .set('pageSize', filter.pageSize.toString());

    if (filter.title !== null) {
      params = params.set('title', filter.title);
    }

    if (filter.budgetFrom !== null) {
      params = params.set('budgetFrom', filter.budgetFrom.toString());
    }

    if (filter.budgetTo !== null) {
      params = params.set('budgetTo', filter.budgetTo.toString());
    }

    if (filter.categoryId !== null) {
      params = params.set('categoryId', filter.categoryId);
    }

    if (filter.employerId !== null) {
      params = params.set('employerId', filter.employerId);
    }

    if (filter.projectStatus !== null) {
      params = params.set('projectStatus', filter.projectStatus.toString());
    }

    return this.httpClient.get<PaginatedResult<Project>>(
        `${PROJECTS_SERVICE_API_URL}projects/by-filter`,
        { params }
    );
  }

  getAllCategories() {
    const params = new HttpParams({
      fromObject: {
        pageNo: 1,
        pageSize: 100,
      }});
    return this.httpClient.get<PaginatedResult<Project>>(
        `${PROJECTS_SERVICE_API_URL}projects/by-filter`,
        { params },
    );
  }
}
