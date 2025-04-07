import {Component, OnInit} from '@angular/core';
import {
  AbstractControl,
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  ValidationErrors,
  Validators
} from '@angular/forms';
import {NzFlexDirective} from 'ng-zorro-antd/flex';
import {NzCardComponent} from 'ng-zorro-antd/card';
import {NzInputDirective, NzInputGroupComponent, NzInputGroupWhitSuffixOrPrefixDirective} from 'ng-zorro-antd/input';
import {NzSpaceComponent, NzSpaceItemDirective} from 'ng-zorro-antd/space';
import {NzButtonComponent} from 'ng-zorro-antd/button';
import {NzAlertComponent} from 'ng-zorro-antd/alert';
import { NzSpinComponent } from 'ng-zorro-antd/spin';
import {ActivatedRoute, Router, RouterLink} from '@angular/router';
import {PasswordResetService} from '../../../core/services/password-reset/password-reset.service';
import {catchError, throwError} from 'rxjs';
import {NgIf} from '@angular/common';
import {NzIconDirective} from 'ng-zorro-antd/icon';

interface ResetPasswordForm {
  email: FormControl<string>;
  newPassword: FormControl<string>;
  confirmPassword: FormControl<string>;
}

@Component({
  selector: 'app-reset-password',
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
    NzSpaceItemDirective,
    NgIf,
    NzInputGroupWhitSuffixOrPrefixDirective,
    NzInputGroupComponent,
    NzIconDirective
  ],
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit {
  resetPasswordForm = new FormGroup<ResetPasswordForm>({
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
    newPassword: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.minLength(8),
        Validators.pattern(/[a-z]/),
        Validators.pattern(/[A-Z]/),
        Validators.pattern(/[0-9]/),
        Validators.pattern(/[^a-zA-Z0-9]/)
      ]
    }),
    confirmPassword: new FormControl('', { nonNullable: true })
  }, { validators: this.passwordsMatchValidator });

  isLoading: boolean = false;
  successMessage: string | null = null;
  errorMessage: string | null = null;
  code: string = '';
  newPasswordVisible: boolean = false;
  confirmPasswordVisible: boolean = false;

  constructor(
    private passwordResetService: PasswordResetService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const email = params['email'];
      this.code = params['code'] || '';
      if (email) {
        this.resetPasswordForm.patchValue({ email });
      }
      if (!this.code) {
        this.errorMessage = 'Invalid or missing reset token.';
        this.router.navigate(['/forgot-password']);
      }
    });
  }

  private passwordsMatchValidator(group: AbstractControl): ValidationErrors | null {
    const password = group.get('newPassword')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { mismatch: true };
  }

  onSubmit(): void {
    if (this.resetPasswordForm.invalid) {
      this.errorMessage = 'Please fill in all fields correctly.';
      return;
    }

    this.isLoading = true;
    this.successMessage = null;
    this.errorMessage = null;

    const { email, newPassword } = this.resetPasswordForm.value;
    this.passwordResetService.resetPassword(email!, newPassword!, this.code)
      .pipe(
        catchError(error => {
          this.isLoading = false;
          if (error.status === 400) {
            this.errorMessage = error.error?.detail || 'Invalid reset code or data.';
          } else if (error.status === 404) {
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
          this.successMessage = 'Password has been reset successfully. Redirecting to login...';
          setTimeout(() => this.router.navigate(['/login']), 3000);
        }
      });
  }
}
