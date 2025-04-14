import {Component} from '@angular/core';
import {AbstractControl, FormControl, FormGroup, FormsModule, ReactiveFormsModule, ValidationErrors, Validators} from '@angular/forms';
import {NgIf} from '@angular/common';
import {NzButtonComponent} from 'ng-zorro-antd/button';
import {NzCardComponent} from 'ng-zorro-antd/card';
import {NzFlexDirective} from 'ng-zorro-antd/flex';
import {NzIconDirective} from 'ng-zorro-antd/icon';
import {NzInputDirective, NzInputGroupComponent} from 'ng-zorro-antd/input';
import {NzSpaceComponent, NzSpaceItemDirective} from 'ng-zorro-antd/space';
import {Router, RouterLink} from '@angular/router';
import {AuthService} from '../../../core/services/auth/auth.service';
import {NzRadioComponent, NzRadioGroupComponent} from 'ng-zorro-antd/radio';
import {RegisterFreelancerForm} from '../../interfaces/register/register-freelancer-form.interface';
import {RegisterEmployerForm} from '../../interfaces/register/register-employer-form.interface';
import {catchError, tap, throwError} from 'rxjs';
import {NzMessageService} from "ng-zorro-antd/message";

@Component({
  selector: 'app-register-page',
  standalone: true,
  imports: [
    FormsModule,
    NgIf,
    NzButtonComponent,
    NzCardComponent,
    NzFlexDirective,
    NzIconDirective,
    NzInputDirective,
    NzInputGroupComponent,
    NzSpaceComponent,
    ReactiveFormsModule,
    NzSpaceItemDirective,
    RouterLink,
    NzRadioGroupComponent,
    NzRadioComponent
  ],
  templateUrl: './register-page.component.html',
  styleUrl: './register-page.component.scss',
  providers: [NzMessageService]
})
export class RegisterPageComponent {
  passwordVisible = false;
  passwordConfirmVisible = false;
  registerUserState: string = 'freelancer';
  
  freelancerForm = new FormGroup<RegisterFreelancerForm>({
    userName: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.maxLength(200)
      ]
    }),
    firstName: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.maxLength(100)
      ]
    }),
    lastName: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.maxLength(100)
      ]
    }),
    email: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.email
      ]
    }),
    password: new FormControl('', {
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
    passwordConfirm: new FormControl('', {
      nonNullable: true
    })
  }, {
    validators: this.passwordsMatchValidator
  });
  
  employerForm = new FormGroup<RegisterEmployerForm>({
    userName: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.maxLength(200)
      ]
    }),
    companyName: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.maxLength(100)
      ]
    }),
    email: new FormControl('', {
      nonNullable: true,
      validators: [
        Validators.required,
        Validators.email
      ]
    }),
    password: new FormControl('', {
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
    passwordConfirm: new FormControl('', {
      nonNullable: true
    })
  }, { validators: this.passwordsMatchValidator });
  
  constructor(
    private authService: AuthService,
    private router: Router,
    private message: NzMessageService
  ) {}
  
  private passwordsMatchValidator(group: AbstractControl): ValidationErrors | null {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('passwordConfirm')?.value;
    return password === confirmPassword ? null : { passwordsMismatch: true };
  }
  
  onSubmitRegister() {
    if (this.registerUserState === 'freelancer') {
      if (this.freelancerForm.invalid) {
        this.message.error('Please fill in all fields correctly.', { nzDuration: 3000 });
        return;
      }
      const payload = this.freelancerForm.getRawValue();
      this.authService.registerFreelancer(payload)
        .pipe(
          tap(response => {
            if (response.status === 201) {
              this.message.success('Registration successful! Please confirm your email.', { nzDuration: 3000 });
              this.router.navigate(['/confirm-email'], {
                queryParams: { email: payload.email }
              });
            }
          }),
          catchError(error => {
            console.error('Freelancer registration failed:', error);
            if (error.status === 400) {
              this.message.error('Invalid data. Please check your input.', { nzDuration: 3000 });
            } else {
              this.message.error('Something went wrong. Try again later.', { nzDuration: 3000 });
            }
            return throwError(() => error);
          })
        )
        .subscribe();
    } else {
      if (this.employerForm.invalid) {
        this.message.error('Please fill in all fields correctly.', { nzDuration: 3000 });
        return;
      }
      const payload = this.employerForm.getRawValue();
      this.authService.registerEmployer(payload)
        .pipe(
          tap(response => {
            if (response.status === 201) {
              this.message.success('Registration successful! Please confirm your email.', { nzDuration: 3000 });
              this.router.navigate(['/confirm-email'], {
                queryParams: { email: payload.email }
              });
            }
          }),
          catchError(error => {
            console.error('Employer registration failed:', error);
            if (error.status === 400) {
              this.message.error('Invalid data. Please check your input.', { nzDuration: 3000 });
            } else {
              this.message.error('Something went wrong. Try again later.', { nzDuration: 3000 });
            }
            return throwError(() => error);
          })
        )
        .subscribe();
    }
  }
}