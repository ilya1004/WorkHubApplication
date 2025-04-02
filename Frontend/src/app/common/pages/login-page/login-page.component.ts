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
import {RouterLink} from '@angular/router';

interface LoginForm {
  email: FormControl<string>;
  password: FormControl<string>;
}

@Component({
  standalone: true,
  selector: 'app-login-page',
  imports: [NzCardComponent, NzInputDirective, NzInputGroupComponent, NzIconDirective, ReactiveFormsModule, NzButtonComponent, NzFlexDirective, NzSpaceComponent, NzSpaceItemDirective, NgIf, RouterLink],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.scss'
})
export class LoginPageComponent {
  passwordVisible = false;

  constructor(private authService: AuthService) {}

  form = new FormGroup<LoginForm>({
    email: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.email] }),
    password: new FormControl('', { nonNullable: true, validators: [Validators.required] })
  });

  onSubmitLogin() {
    if (this.form.valid) {
      const payload: { email: string, password: string } = this.form.getRawValue();
      this.authService.login(payload);
    } else {
      console.error('Invalid form:', this.getFormValidationErrors());
    }
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

  onClickRegister() {

  }
}
