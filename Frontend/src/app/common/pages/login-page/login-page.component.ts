import {Component} from '@angular/core';
import {NzCardComponent} from 'ng-zorro-antd/card';
import {NzInputDirective, NzInputGroupComponent} from 'ng-zorro-antd/input';
import {NzIconDirective} from 'ng-zorro-antd/icon';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {NzButtonComponent} from 'ng-zorro-antd/button';
import {NzFlexDirective} from 'ng-zorro-antd/flex';
import {NzSpaceComponent, NzSpaceItemDirective} from 'ng-zorro-antd/space';
import {AuthService} from '../../services/auth/auth.service';
import {NgIf} from '@angular/common';
import {Router, RouterLink} from '@angular/router';
import {NzAlertComponent} from 'ng-zorro-antd/alert';
import {routes} from '../../../app.routes';
import {catchError, tap, throwError} from 'rxjs';
import {CookieService} from 'ngx-cookie-service';

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
    private cookieService: CookieService,
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
          }),
          tap({
            next: value => {
              this.cookieService.set('access_token', value.body!.accessToken);
              this.cookieService.set('refresh_token', value.body!.refreshToken);
            },
          })
        )
        .subscribe({
          next: () => {
            this.router.navigate(['/freelancer/home']);
          }
        });
    } else {
      this.errorMessage = 'Please fill in all required fields.';
    }
  }

  onCloseMessage() {
    this.errorMessage = null;
  }

  getFormValidationErrors() {
    const errors: any[] = [];

    Object.keys(this.form.controls).forEach(key => {
      const controlErrors = this.form.get(key)?.errors;
      if (controlErrors) {
        errors.push({ field: key, errors: controlErrors });
      }
    });

    return errors;
  }
}
