import {Component, OnInit} from '@angular/core';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {NzAlertComponent} from 'ng-zorro-antd/alert';
import {NzButtonComponent} from 'ng-zorro-antd/button';
import {NzCardComponent} from 'ng-zorro-antd/card';
import {NzFlexDirective} from 'ng-zorro-antd/flex';
import {NzInputDirective} from 'ng-zorro-antd/input';
import {NzSpaceComponent, NzSpaceItemDirective} from 'ng-zorro-antd/space';
import {EmailConfirmationService} from '../../../core/services/email-confirmation/email-confirmation.service';
import {catchError, throwError} from 'rxjs';
import {NgIf} from '@angular/common';
import {ActivatedRoute, Router} from '@angular/router';

interface ConfirmEmailForm {
  email: FormControl<string>;
  code: FormControl<string>;
}

@Component({
  selector: 'app-confirm-email',
  imports: [
    FormsModule,
    NzAlertComponent,
    NzButtonComponent,
    NzCardComponent,
    NzFlexDirective,
    NzInputDirective,
    NzSpaceComponent,
    ReactiveFormsModule,
    NzSpaceItemDirective,
    NgIf
  ],
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.scss'],
  standalone: true
})
export class ConfirmEmailComponent implements OnInit {
  confirmEmailForm = new FormGroup<ConfirmEmailForm>({
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
    code: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.minLength(6), Validators.maxLength(6)] })
  });

  isSendCodeBtnDisabled: boolean = false;
  errorMessage: string | null = null;

  constructor(
    private emailConfirmationService: EmailConfirmationService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const email: string = params['email'];
      if (email) {
        this.confirmEmailForm.patchValue({ email });
      }
    });
  }

  onClickSendCode() {
    if (this.confirmEmailForm.controls.email.invalid) {
      this.errorMessage = 'Please enter a valid email address.';
      return;
    }

    const payload = { email: this.confirmEmailForm.controls.email.value };
    this.emailConfirmationService.sendEmailConfirmation(payload)
      .pipe(
        catchError(error => {
          if (400 <= error.status && error.status < 500) {
            this.errorMessage = error.error.detail;
          }
          return throwError(() => error);
        })
      )
      .subscribe({
        next: () => {
          this.isSendCodeBtnDisabled = true;
        }
      });
  }

  onCloseMessage() {
    this.errorMessage = null;
  }

  onClickConfirmEmail() {
    if (this.confirmEmailForm.invalid) {
      this.errorMessage = 'Please fill in all required fields correctly.';
      return;
    }

    const payload = this.confirmEmailForm.value;
    this.emailConfirmationService.confirmEmail(payload as { email: string, code: string })
      .pipe(
        catchError(error => {
          if (400 <= error.status && error.status < 500) {
            this.errorMessage = error.error.detail;
          }
          return throwError(() => error);
        })
      )
      .subscribe({
        next: () => {
          this.router.navigate(['/login']);
        }
      });
  }
}
