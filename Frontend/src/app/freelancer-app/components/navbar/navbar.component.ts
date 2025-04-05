import { Component } from '@angular/core';
import {NzFlexDirective} from 'ng-zorro-antd/flex';
import {NzMenuDirective, NzMenuItemComponent} from 'ng-zorro-antd/menu';
import {NzIconDirective} from 'ng-zorro-antd/icon';
import {RouterLink} from '@angular/router';
import {NzButtonModule} from "ng-zorro-antd/button";
import {AuthService} from "../../../core/services/auth/auth.service";

@Component({
  selector: 'app-navbar',
  imports: [
    NzFlexDirective,
    NzMenuDirective,
    NzMenuItemComponent,
    NzIconDirective,
    RouterLink,
    NzButtonModule
  ],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent {
  constructor(
    private authService: AuthService
  ) { }

  isAuthenticated(): boolean {
    return this.authService.isAuthenticated();
  }

  logout(): void {
    this.authService.logout();
  }
}