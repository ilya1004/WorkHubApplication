import {Component, OnInit} from '@angular/core';
import {AppUser} from "../../../interfaces/users/app-user.interface";
import {UsersService} from "../../../../core/services/users/users.service";
import {PaginatedResult} from "../../../../core/interfaces/common/paginated-result.interface";
import {NzTableComponent, NzTableModule} from "ng-zorro-antd/table";
import {DatePipe, NgForOf} from "@angular/common";
import {NzButtonComponent} from "ng-zorro-antd/button";
import {RouterLink} from "@angular/router";

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [
    NzTableComponent,
    NzTableModule,
    NgForOf,
    DatePipe,
    NzButtonComponent,
    RouterLink
  ],
  templateUrl: './users.component.html',
  styleUrl: './users.component.scss'
})
export class UsersComponent implements OnInit {
  users: AppUser[] = [];
  totalCount = 0;
  loading = false;
  pageNo = 1;
  pageSize = 10;
  
  constructor(private usersService: UsersService) {}
  
  ngOnInit(): void {
    this.loadUsers();
  }
  
  loadUsers(): void {
    this.loading = true;
    this.usersService.getAllUsers(this.pageNo, this.pageSize).subscribe({
      next: (result: PaginatedResult<AppUser>) => {
        this.users = result.items;
        this.totalCount = result.totalCount;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading users:', error);
        this.loading = false;
      }
    });
  }
  
  onPageChange(page: number): void {
    this.pageNo = page;
    this.loadUsers();
  }
  
  onPageSizeChange(size: number): void {
    this.pageSize = size;
    this.pageNo = 1;
    this.loadUsers();
  }
}