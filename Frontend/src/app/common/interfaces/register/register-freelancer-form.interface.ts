import {FormControl} from '@angular/forms';

export interface RegisterFreelancerForm {
  userName: FormControl<string>;
  firstName: FormControl<string>;
  lastName: FormControl<string>;
  email: FormControl<string>;
  password: FormControl<string>;
  passwordConfirm: FormControl<string>;
}
