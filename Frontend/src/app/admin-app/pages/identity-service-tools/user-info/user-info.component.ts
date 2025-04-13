import {Component, OnInit} from '@angular/core';
import {AppUser} from "../../../interfaces/users/app-user.interface";
import {UsersService} from "../../../../core/services/users/users.service";
import {ActivatedRoute, Router} from "@angular/router";
import {NzButtonComponent} from "ng-zorro-antd/button";
import {NzCardComponent} from "ng-zorro-antd/card";
import {NzDescriptionsComponent, NzDescriptionsItemComponent} from "ng-zorro-antd/descriptions";
import {DatePipe, NgIf} from "@angular/common";
import {NzTagComponent} from "ng-zorro-antd/tag";

@Component({
  selector: 'app-user-info',
  imports: [
    NzButtonComponent,
    NzCardComponent,
    NzDescriptionsComponent,
    NgIf,
    NzDescriptionsItemComponent,
    DatePipe,
    NzTagComponent
  ],
  templateUrl: './user-info.component.html',
  styleUrl: './user-info.component.scss'
})
export class UserInfoComponent implements OnInit {
  user: AppUser | null = null;
  loading = false;
  
  constructor(
    private usersService: UsersService,
    private route: ActivatedRoute,
    private router: Router
  ) {}
  
  ngOnInit(): void {
    const userId = this.route.snapshot.paramMap.get('userId');
    if (userId) {
      this.loadUser(userId);
    }
  }
  
  loadUser(userId: string): void {
    this.loading = true;
    this.usersService.getUserById(userId).subscribe({
      next: (user: AppUser) => {
        this.user = user;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading user:', error);
        this.loading = false;
      }
    });
  }
  
  goBack(): void {
    this.router.navigate(['/admin/identity-service-tools']);
  }
}
