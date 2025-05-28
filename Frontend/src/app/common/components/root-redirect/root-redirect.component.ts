import {Component, OnInit} from '@angular/core';
import {Router} from "@angular/router";
import {TokenService} from "../../../core/services/auth/token.service";

@Component({
  selector: 'app-root-redirect',
  imports: [],
  template: '',
})
export class RootRedirectComponent implements OnInit {
  constructor(
    private router: Router,
    private tokenService: TokenService
  ) {}
  
  ngOnInit() {
    const role = this.tokenService.getUserRole();
    if (this.tokenService.isAuthenticated() && role) {
      switch (role) {
        case 'Freelancer':
          this.router.navigate(['/freelancer/home']);
          break;
        case 'Employer':
          this.router.navigate(['/employer/home']);
          break;
        case 'Admin':
          this.router.navigate(['/admin/home']);
          break;
        default:
          this.router.navigate(['/login']);
      }
    } else {
      this.router.navigate(['/login']);
    }
  }
}