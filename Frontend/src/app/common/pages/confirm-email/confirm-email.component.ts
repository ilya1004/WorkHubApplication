import {Component, OnInit} from '@angular/core';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {NzButtonComponent} from 'ng-zorro-antd/button';
import {NzCardComponent} from 'ng-zorro-antd/card';
import {NzFlexDirective} from 'ng-zorro-antd/flex';
import {NzInputDirective} from 'ng-zorro-antd/input';
import {NzSpaceComponent, NzSpaceItemDirective} from 'ng-zorro-antd/space';
import {EmailConfirmationService} from '../../../core/services/auth/email-confirmation.service';
import {catchError, throwError} from 'rxjs';
import {NgIf} from '@angular/common';
import {ActivatedRoute, Router, RouterLink} from '@angular/router';
import {NzMessageService} from "ng-zorro-antd/message";

interface ConfirmEmailForm {
  email: FormControl<string>;
  code: FormControl<string>;
}

@Component({
  selector: 'app-confirm-email',
  imports: [
    ReactiveFormsModule,
    NzButtonComponent,
    NzCardComponent,
    NzFlexDirective,
    NzInputDirective,
    NzSpaceComponent,
    NzSpaceItemDirective,
    NgIf,
    RouterLink
  ],
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.scss'],
  standalone: true,
  providers: [NzMessageService]
})
export class ConfirmEmailComponent implements OnInit {
  confirmEmailForm = new FormGroup<ConfirmEmailForm>({
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
    code: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(6), Validators.maxLength(6)] })
  });
  
  isSendCodeBtnDisabled: boolean = false;
  
  constructor(
    private emailConfirmationService: EmailConfirmationService,
    private router: Router,
    private route: ActivatedRoute,
    private message: NzMessageService
  ) {}
  
  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const email: string = params['email'];
      if (email) {
        this.isSendCodeBtnDisabled = true;
        this.confirmEmailForm.patchValue({ email });
      }
    });
  }
  
  onClickSendCode() {
    if (this.confirmEmailForm.controls.email.invalid) {
      this.message.error('Please enter a valid email address.', { nzDuration: 3000 });
      return;
    }
    
    const payload = { email: this.confirmEmailForm.controls.email.value };
    this.emailConfirmationService.sendEmailConfirmation(payload)
      .pipe(
        catchError(error => {
          if (400 <= error.status && error.status < 500) {
            this.message.error(error.error.detail || 'Failed to send code.', { nzDuration: 3000 });
          } else {
            this.message.error('An unexpected error occurred.', { nzDuration: 3000 });
          }
          return throwError(() => error);
        })
      )
      .subscribe({
        next: () => {
          this.isSendCodeBtnDisabled = true;
          this.message.success('Confirmation code sent successfully!', { nzDuration: 3000 });
        }
      });
  }
  
  onClickConfirmEmail() {
    if (this.confirmEmailForm.invalid) {
      this.message.error('Please fill in all required fields correctly.', { nzDuration: 3000 });
      return;
    }
    
    const payload = this.confirmEmailForm.value;
    this.emailConfirmationService.confirmEmail(payload as { email: string, code: string })
      .pipe(
        catchError(error => {
          if (400 <= error.status && error.status < 500) {
            this.message.error(error.error.detail || 'Invalid confirmation code.', { nzDuration: 3000 });
          } else {
            this.message.error('An unexpected error occurred.', { nzDuration: 3000 });
          }
          return throwError(() => error);
        })
      )
      .subscribe({
        next: () => {
          this.message.success('Email confirmed successfully!', { nzDuration: 3000 });
          this.router.navigate(['/login']);
        }
      });
  }
}