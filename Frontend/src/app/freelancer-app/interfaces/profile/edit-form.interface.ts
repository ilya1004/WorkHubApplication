import {FormControl} from '@angular/forms';

export interface EditFreelancerForm {
  firstName: FormControl<string>;
  lastName: FormControl<string>;
  about: FormControl<string>;
  skillIds: FormControl<string[]>;
  resetImage: FormControl<boolean>;
  image: FormControl<File | null>;
}
