import {Component} from '@angular/core';
import {NzCardComponent} from 'ng-zorro-antd/card';
import {NzInputDirective, NzInputGroupComponent} from 'ng-zorro-antd/input';
import {NzIconDirective} from 'ng-zorro-antd/icon';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {NzButtonComponent} from 'ng-zorro-antd/button';
import {NzFlexDirective} from 'ng-zorro-antd/flex';
import {NzSpaceComponent, NzSpaceItemDirective} from 'ng-zorro-antd/space';
import {AuthService} from '../../../core/services/auth/auth.service';
import {NgIf} from '@angular/common';
import {Router, RouterLink} from '@angular/router';
import {catchError, throwError} from 'rxjs';
import {TokenService} from "../../../core/services/auth/token.service";
import {NzMessageService} from "ng-zorro-antd/message";

interface LoginForm {
  email: FormControl<string>;
  password: FormControl<string>;
}

@Component({
  standalone: true,
  selector: 'app-login-page',
  imports: [
    ReactiveFormsModule,
    NzCardComponent,
    NzInputDirective,
    NzInputGroupComponent,
    NzIconDirective,
    NzButtonComponent,
    NzFlexDirective,
    NzSpaceComponent,
    NzSpaceItemDirective,
    NgIf,
    RouterLink
  ],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.scss',
  providers: [NzMessageService]
})
export class LoginPageComponent {
  passwordVisible = false;
  
  form = new FormGroup<LoginForm>({
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required] })
  });
  
  constructor(
    private authService: AuthService,
    private router: Router,
    private tokenService: TokenService,
    private message: NzMessageService
  ) {}
  
  onSubmitLogin() {
    if (this.form.invalid) {
      this.message.error('Please fill in all required fields.', { nzDuration: 3000 });
      return;
    }
    
    const payload = this.form.getRawValue();
    this.authService.login(payload)
      .pipe(
        catchError(error => {
          if (400 <= error.status && error.status < 500) {
            this.message.error(error.error.detail || 'Invalid email or password.', { nzDuration: 3000 });
          } else {
            this.message.error('Something went wrong. Please try again later.', { nzDuration: 3000 });
          }
          return throwError(() => error);
        })
      )
      .subscribe(response => {
        if (response.body && response.body.accessToken && response.body.refreshToken) {
          this.tokenService.setTokens(response.body.accessToken, response.body.refreshToken);
          this.message.success('Logged in successfully!', { nzDuration: 3000 });
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
      });
  }
}