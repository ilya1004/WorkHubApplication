import {Component} from '@angular/core';
import {Router} from "@angular/router";
import {TokenService} from "../../../core/services/auth/token.service";
import {NzResultComponent} from "ng-zorro-antd/result";
import {NzButtonComponent} from "ng-zorro-antd/button";
import {NzFlexDirective} from "ng-zorro-antd/flex";

@Component({
  selector: 'app-not-found',
  imports: [
    NzResultComponent,
    NzButtonComponent,
    NzFlexDirective
  ],
  templateUrl: './not-found.component.html',
  styleUrl: './not-found.component.scss'
})
export class NotFoundComponent {
  constructor(
    private router: Router,
    private tokenService: TokenService
    ) {}
  
  goHome() {
    const role = this.tokenService.getUserRole();
    switch (role) {
      case 'Freelancer':
        this.router.navigate(['/freelancer/home']);
        break;
      case 'Employer':
        this.router.navigate(['/employer/my-projects']);
        break;
      case 'Admin':
        this.router.navigate(['/admin/home']);
        break;
      default:
        this.router.navigate(['/login']);
    }
  }
  
  goToLogin() {
    this.router.navigate(['/login']);
  }
}