import {Component} from '@angular/core';
import {NzCardComponent} from 'ng-zorro-antd/card';
import {NzFlexDirective} from 'ng-zorro-antd/flex';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {NzSpaceComponent, NzSpaceItemDirective} from 'ng-zorro-antd/space';
import {NzInputDirective} from 'ng-zorro-antd/input';
import {NzButtonComponent} from 'ng-zorro-antd/button';
import {NzSpinComponent} from 'ng-zorro-antd/spin';
import {RouterLink} from '@angular/router';
import {PasswordResetService} from '../../../core/services/auth/password-reset.service';
import {catchError, throwError} from 'rxjs';
import {NgIf} from '@angular/common';
import {NzMessageService} from "ng-zorro-antd/message";

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
    NzSpinComponent,
    RouterLink,
    NgIf,
    NzSpaceItemDirective
  ],
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss'],
  providers: [NzMessageService]
})
export class ForgotPasswordComponent {
  forgotPasswordForm = new FormGroup<ForgotPasswordForm>({
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] })
  });
  
  isLoading: boolean = false;
  
  constructor(
    private passwordResetService: PasswordResetService,
    private message: NzMessageService
  ) {}
  
  onSubmit(): void {
    if (this.forgotPasswordForm.invalid) {
      this.message.error('Please enter a valid email address.', { nzDuration: 3000 });
      return;
    }
    
    this.isLoading = true;
    
    const email = this.forgotPasswordForm.controls.email.value;
    this.passwordResetService.forgotPassword(email)
      .pipe(
        catchError(error => {
          this.isLoading = false;
          if (error.status === 404) {
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
          this.message.success('Reset link has been sent to your email.', { nzDuration: 3000 });
          this.forgotPasswordForm.reset();
        }
      });
  }
}
