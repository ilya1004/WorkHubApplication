import {FormControl} from "@angular/forms";

export interface CreateProjectForm {
  title: FormControl<string>;
  description: FormControl<string>;
  budget: FormControl<number>;
  categoryId: FormControl<string | null>;
  applicationsStartDate: FormControl<Date>;
  applicationsDeadline: FormControl<Date>;
  workStartDate: FormControl<Date>;
  workDeadline: FormControl<Date>;
  paymentMethodId: FormControl<string>;
}

export interface ProjectCreateData {
  project: {
    title: string;
    description: string;
    budget: number;
    categoryId: string | null;
  };
  lifecycle: {
    applicationsStartDate: Date;
    applicationsDeadline: Date;
    workStartDate: Date;
    workDeadline: Date;
  };
  paymentMethodId: string;
}