import {FormControl} from '@angular/forms';
import {FreelancerSkill} from './skill.interface';

export interface EditFreelancerForm {
  firstName: FormControl<string>;
  lastName: FormControl<string>;
  about: FormControl<string>;
  skillIds: FormControl<string[]>;
  resetImage: FormControl<boolean>;
  image: FormControl<File | null>;
}
