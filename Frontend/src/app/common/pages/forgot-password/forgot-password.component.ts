import { Component } from '@angular/core';
import {NzCardComponent} from 'ng-zorro-antd/card';
import {NzFlexDirective} from 'ng-zorro-antd/flex';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {NzSpaceComponent, NzSpaceItemDirective} from 'ng-zorro-antd/space';
import {NzInputDirective} from 'ng-zorro-antd/input';
import {NzButtonComponent} from 'ng-zorro-antd/button';
import {NzSpinComponent} from 'ng-zorro-antd/spin';
import {RouterLink} from '@angular/router';
import {NzAlertComponent} from 'ng-zorro-antd/alert';
import {PasswordResetService} from '../../../core/services/auth/password-reset.service';
import {catchError, throwError} from 'rxjs';
import {NgIf} from '@angular/common';

interface ForgotPasswordForm {
  email: FormControl<string>;
}

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    NzFlexDirective,
    NzCardComponent,
    NzInputDirective,
    NzSpaceComponent,
    NzButtonComponent,
    NzAlertComponent,
    NzSpinComponent,
    RouterLink,
    NgIf,
    NzSpaceItemDirective
  ],
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent {
  forgotPasswordForm = new FormGroup<ForgotPasswordForm>({
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] })
  });

  isLoading: boolean = false;
  successMessage: string | null = null;
  errorMessage: string | null = null;

  constructor(private passwordResetService: PasswordResetService) {}

  onSubmit(): void {
    if (this.forgotPasswordForm.invalid) {
      this.errorMessage = 'Please enter a valid email address.';
      return;
    }

    this.isLoading = true;
    this.successMessage = null;
    this.errorMessage = null;

    const email = this.forgotPasswordForm.controls.email.value;
    this.passwordResetService.forgotPassword(email)
      .pipe(
        catchError(error => {
          this.isLoading = false;
          if (error.status === 404) {
            this.errorMessage = 'User with this email does not exist.';
          } else {
            this.errorMessage = 'Something went wrong. Please try again later.';
          }
          return throwError(() => error);
        })
      )
      .subscribe({
        next: () => {
          this.isLoading = false;
          this.successMessage = 'Reset link has been sent to your email.';
          this.forgotPasswordForm.reset();
        }
      });
  }
}
