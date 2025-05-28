import {Component, OnInit} from '@angular/core';
import {AbstractControl, FormControl, FormGroup, ReactiveFormsModule, ValidationErrors, Validators} from '@angular/forms';
import {NzFlexDirective} from 'ng-zorro-antd/flex';
import {NzCardComponent} from 'ng-zorro-antd/card';
import {NzInputDirective, NzInputGroupComponent} from 'ng-zorro-antd/input';
import {NzSpaceComponent, NzSpaceItemDirective} from 'ng-zorro-antd/space';
import {NzButtonComponent} from 'ng-zorro-antd/button';
import {NzSpinComponent} from 'ng-zorro-antd/spin';
import {ActivatedRoute, Router, RouterLink} from '@angular/router';
import {PasswordResetService} from '../../../core/services/auth/password-reset.service';
import {catchError, throwError} from 'rxjs';
import {NgIf} from '@angular/common';
import {NzIconDirective} from 'ng-zorro-antd/icon';
import {NzMessageService} from "ng-zorro-antd/message";

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
    NzSpinComponent,
    RouterLink,
    NzSpaceItemDirective,
    NgIf,
    NzInputGroupComponent,
    NzIconDirective
  ],
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss'],
  providers: [NzMessageService]
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
  code: string = '';
  newPasswordVisible: boolean = false;
  confirmPasswordVisible: boolean = false;
  
  constructor(
    private passwordResetService: PasswordResetService,
    private route: ActivatedRoute,
    private router: Router,
    private message: NzMessageService
  ) {}
  
  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const email = params['email'];
      this.code = params['code'] || '';
      if (email) {
        this.resetPasswordForm.patchValue({ email });
      }
      if (!this.code) {
        this.message.error('Invalid or missing reset token.', { nzDuration: 3000 });
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
      this.message.error('Please fill in all fields correctly.', { nzDuration: 3000 });
      return;
    }
    
    this.isLoading = true;
    
    const { email, newPassword } = this.resetPasswordForm.value;
    this.passwordResetService.resetPassword(email!, newPassword!, this.code)
      .pipe(
        catchError(error => {
          this.isLoading = false;
          if (error.status === 400) {
            this.message.error(error.error?.detail || 'Invalid reset code or data.', { nzDuration: 3000 });
          } else if (error.status === 404) {
            this.message.error('User with this email does not exist.', { nzDuration: 3000 });
          } else {
            this.message.error('Something went wrong. Please try again later.', { nzDuration: 3000 });
          }
          return throwError(() => error);
        })
      )
      .subscribe({
        next: () => {
          this.isLoading = false;
          this.message.success('Password has been reset successfully. Redirecting to login...', { nzDuration: 3000 });
          setTimeout(() => this.router.navigate(['/login']), 3000);
        }
      });
  }
}