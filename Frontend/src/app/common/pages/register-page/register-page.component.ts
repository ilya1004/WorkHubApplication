import { Component } from '@angular/core';
import {AbstractControl, FormControl, FormGroup, FormsModule, ReactiveFormsModule, ValidationErrors, Validators} from '@angular/forms';
import {NgIf} from '@angular/common';
import {NzButtonComponent} from 'ng-zorro-antd/button';
import {NzCardComponent} from 'ng-zorro-antd/card';
import {NzFlexDirective} from 'ng-zorro-antd/flex';
import {NzIconDirective} from 'ng-zorro-antd/icon';
import {NzInputDirective, NzInputGroupComponent} from 'ng-zorro-antd/input';
import {NzSpaceComponent, NzSpaceItemDirective} from 'ng-zorro-antd/space';
import {RouterLink} from '@angular/router';

interface RegisterForm {
  username: FormControl<string>;
  firstName: FormControl<string>;
  lastName: FormControl<string>;
  email: FormControl<string>;
  password: FormControl<string>;
  passwordConfirm: FormControl<string>;
}

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
    RouterLink
  ],
  templateUrl: './register-page.component.html',
  styleUrl: './register-page.component.scss'
})
export class RegisterPageComponent {

  passwordVisible = false;
  passwordConfirmVisible = false;

  form = new FormGroup<RegisterForm>({
    username: new FormControl('', {
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
    passwordConfirm: new FormControl('', { nonNullable: true })
  }, { validators: this.passwordsMatchValidator });

  private passwordsMatchValidator(group: AbstractControl): ValidationErrors | null {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('passwordConfirm')?.value;
    return password === confirmPassword ? null : { passwordsMismatch: true };
  }

  /**
   * Отправка формы
   */
  onSubmitRegister() {
    if (this.form.valid) {
      console.log('Form submitted:', this.form.value);
    } else {
      console.error('Form errors:', this.getFormValidationErrors());
    }
  }

  /**
   * Метод для получения ошибок валидации
   */
  private getFormValidationErrors() {
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
