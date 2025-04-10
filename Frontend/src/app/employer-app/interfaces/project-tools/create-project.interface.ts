import {FormControl} from "@angular/forms";

export interface CreateProjectForm {
  title: FormControl<string>;
  description: FormControl<string | null>;
  budget: FormControl<number>;
  categoryId: FormControl<string | null>;
  applicationsStartDate: FormControl<Date>;
  applicationsDeadline: FormControl<Date>;
  workStartDate: FormControl<Date>;
  workDeadline: FormControl<Date>;
}