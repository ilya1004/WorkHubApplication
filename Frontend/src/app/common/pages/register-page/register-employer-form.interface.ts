import {FormControl} from '@angular/forms';

export interface RegisterEmployerForm {
  companyName: FormControl<string>;
  userName: FormControl<string>;
  email: FormControl<string>;
  password: FormControl<string>;
  passwordConfirm: FormControl<string>;
}
