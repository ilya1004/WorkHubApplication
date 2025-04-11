import {Component} from '@angular/core';
import {NzCardComponent} from 'ng-zorro-antd/card';
import {NzInputDirective, NzInputGroupComponent} from 'ng-zorro-antd/input';
import {NzIconDirective} from 'ng-zorro-antd/icon';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {NzButtonComponent} from 'ng-zorro-antd/button';
import {NzFlexDirective} from 'ng-zorro-antd/flex';
import {NzSpaceComponent, NzSpaceItemDirective} from 'ng-zorro-antd/space';
import {AuthService} from '../../../core/services/auth/auth.service';
import {NgIf} from '@angular/common';
import {Router, RouterLink} from '@angular/router';
import {NzAlertComponent} from 'ng-zorro-antd/alert';
import {routes} from '../../../app.routes';
import {catchError, of, tap, throwError} from 'rxjs';
import {CookieService} from 'ngx-cookie-service';
import {TokenService} from "../../../core/services/auth/token.service";

interface LoginForm {
  email: FormControl<string>;
  password: FormControl<string>;
}

@Component({
  standalone: true,
  selector: 'app-login-page',
  imports: [NzCardComponent, NzInputDirective, NzInputGroupComponent, NzIconDirective, ReactiveFormsModule, NzButtonComponent, NzFlexDirective, NzSpaceComponent, NzSpaceItemDirective, NgIf, RouterLink, NzAlertComponent],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.scss'
})
export class LoginPageComponent {
  passwordVisible = false;
  errorMessage: string | null = null;

  constructor(
    private authService: AuthService,
    private router: Router,
    private tokenService: TokenService,
    ) { }

  form = new FormGroup<LoginForm>({
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required] })
  });

  onSubmitLogin() {
    this.errorMessage = null;

    if (this.form.valid) {
      const payload = this.form.getRawValue();
      this.authService.login(payload)
        .pipe(
          catchError(error => {
            if (400 <= error.status && error.status < 500) {
              this.errorMessage = error.error.detail;
            }
            return throwError(() => error);
          })
        )
        .subscribe(response => {
          if (response.body && response.body.accessToken && response.body.refreshToken) {
            this.tokenService.setTokens(response.body.accessToken, response.body.refreshToken);
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
    } else {
      this.errorMessage = 'Please fill in all required fields.';
    }
  }

  onCloseMessage() {
    this.errorMessage = null;
  }
}
