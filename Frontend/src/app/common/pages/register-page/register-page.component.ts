import {Component, EventEmitter} from '@angular/core';
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
import {RegisterFreelancerForm} from './register-freelancer-form.interface';
import {RegisterEmployerForm} from './register-employer-form.interface';
import {catchError, tap, throwError} from 'rxjs';


@Component({
  selector: 'app-register-page',
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
  styleUrl: './register-page.component.scss'
})
export class RegisterPageComponent {

  passwordVisible = false;
  passwordConfirmVisible = false;
  registerUserState: string = 'freelancer';

  constructor(
    private authService: AuthService,
    private router: Router,
    ) {
  }

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
        Validators.pattern(/[a-z]/), // хотя бы одна строчная буква
        Validators.pattern(/[A-Z]/), // хотя бы одна заглавная буква
        Validators.pattern(/[0-9]/), // хотя бы одна цифра
        Validators.pattern(/[^a-zA-Z0-9]/) // хотя бы один спецсимвол
      ]
    }),
    passwordConfirm: new FormControl('',
      {
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
        Validators.pattern(/[a-z]/), // хотя бы одна строчная буква
        Validators.pattern(/[A-Z]/), // хотя бы одна заглавная буква
        Validators.pattern(/[0-9]/), // хотя бы одна цифра
        Validators.pattern(/[^a-zA-Z0-9]/) // хотя бы один спецсимвол
      ]
    }),
    passwordConfirm: new FormControl('', {
      nonNullable: true
    })
  }, {
    validators: this.passwordsMatchValidator
  });

  private passwordsMatchValidator(group: AbstractControl): ValidationErrors | null {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('passwordConfirm')?.value;
    return password === confirmPassword ? null : { passwordsMismatch: true };
  }

  onSubmitRegister() {
    if (this.registerUserState === 'freelancer') {
      if (this.freelancerForm.valid) {
        const payload = this.freelancerForm.getRawValue();
        this.authService.registerFreelancer(payload)
          .pipe(
            tap(response => {
              console.log('Freelancer registration response:', response);
              if (response.status === 201) {
                this.router.navigate(['/confirm-email'], {
                  queryParams: { email: payload.email }
                });
              }
            }),
            catchError(error => {
              console.error('Freelancer registration failed:', error);
              if (error.status === 400) {
                alert('Invalid data. Please check your input.');
              } else {
                alert('Something went wrong. Try again later.');
              }
              return throwError(() => error);
            })
          )
          .subscribe();
      }
    } else {
      if (this.employerForm.valid) {
        const payload = this.employerForm.getRawValue();
        this.authService.registerEmployer(payload)
          .pipe(
            tap(response => {
              console.log('Employer registration response:', response);
              if (response.status === 201) {
                this.router.navigate(['/confirm-email'], {
                  queryParams: { email: payload.email }
                });
              }
            }),
            catchError(error => {
              console.error('Employer registration failed:', error);
              if (error.status === 400) {
                alert('Invalid data. Please check your input.');
              } else {
                alert('Something went wrong. Try again later.');
              }
              return throwError(() => error);
            })
          )
          .subscribe();
      }
    }
  }
}
