import {FormControl} from "@angular/forms";

export interface UpdateProjectForm {
  title: FormControl<string>;
  description: FormControl<string>;
  budget: FormControl<number>;
  categoryId: FormControl<string | null>;
  applicationsStartDate: FormControl<Date>;
  applicationsDeadline: FormControl<Date>;
  workStartDate: FormControl<Date>;
  workDeadline: FormControl<Date>;
}